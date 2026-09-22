using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Order.Repository.DataContexts;
using Order.Repository.Entities;
using Order.Repository.Factory;
using Order.Repository.Helper;
using Order.Repository.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Order.Repository.Services
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly SOHOrderDBContext _context;

        private static string _cachedToken;
        private static DateTime _tokenExpiry = DateTime.MinValue;
        private static readonly object _tokenLock = new();

        public SalesOrderRepository(SOHOrderDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> GenerateShippingLabel(ILogger log)
        {
            var retval = false;
            try
            {
                var TryCount = 5;
                var TryCountConfig = Environment.GetEnvironmentVariable("TryCount");

                if (string.IsNullOrEmpty(TryCountConfig))
                    TryCount = Convert.ToInt32(TryCountConfig);

                var ordersList = await _context.OrderDetails
                    .Where(o => o.status != null && o.status.ToLower() == "picking completed" && o.TryCount < TryCount
                    && (o.ShippingLabel == null || o.ShippingLabel == ""))
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (ordersList == null || ordersList.Count == 0)
                {
                    log.LogInformation("No pending sales order found for shipping label create.");
                    return false;
                }

                var token = await GetAccessTokenAsync().ConfigureAwait(false);

                if (string.IsNullOrEmpty(token))
                {
                    log.LogError("Failed to retrieve access token.");
                    return false;
                }

                var DefaultCompanyID = Environment.GetEnvironmentVariable("DefaultCompanyID");
                var Publisher = Environment.GetEnvironmentVariable("Publisher");
                var CompanyName = Environment.GetEnvironmentVariable("CompanyName");
                var TradingName = Environment.GetEnvironmentVariable("TradingName");
                var AddressLine1 = Environment.GetEnvironmentVariable("AddressLine1");
                var City = Environment.GetEnvironmentVariable("City");
                var State = Environment.GetEnvironmentVariable("State");
                var Country = Environment.GetEnvironmentVariable("Country");
                var Postcode = Environment.GetEnvironmentVariable("Postcode");
                var PhoneNo = Environment.GetEnvironmentVariable("PhoneNo");
                var ServiceCode = Environment.GetEnvironmentVariable("ServiceCode");

                foreach (var order in ordersList)
                {
                    try
                    {                        
                        var model = new ShipingRequest();
                        model.ToAddress = new ShipTo()
                        {
                            name = order.CustomerName,
                            phone = !string.IsNullOrEmpty(order.CustomerPhone) ? order.CustomerPhone : "",
                            email = !string.IsNullOrEmpty(order.CustomerEmail) ? order.CustomerEmail : "",
                            address_line1 = order.AddressLine1,
                            address_line2 = !string.IsNullOrEmpty(order.AddressLine2) ? order.AddressLine2 : "",
                            city_locality = order.City,
                            state_province = order.State,
                            postal_code = order.Postcode,
                            country_code = order.Country,
                        };

                        model.FromAddress = new ShipFrom()
                        {
                            name = !string.IsNullOrEmpty(TradingName) ? TradingName : "Simple Online Healthcare Limited",
                            company_name = !string.IsNullOrEmpty(CompanyName) ? CompanyName : "Trading as Simple",
                            address_line1 = !string.IsNullOrEmpty(AddressLine1) ? AddressLine1 : "119 Racecourse Road",
                            city_locality = !string.IsNullOrEmpty(City) ? City : "ASCOT",
                            state_province = !string.IsNullOrEmpty(State) ? State : "QLD",
                            postal_code = !string.IsNullOrEmpty(Postcode) ? Postcode : "4007",
                            country_code = !string.IsNullOrEmpty(Country) ? Country : "AU",
                            phone = !string.IsNullOrEmpty(PhoneNo) ? PhoneNo : "(07) 4839 7994",
                        };

                        if (order.ColdChain == true)
                        {
                            model.ParcelHeight = Convert.ToDecimal(Environment.GetEnvironmentVariable("P_Height"));
                            model.ParcelWidth = Convert.ToDecimal(Environment.GetEnvironmentVariable("P_Width"));
                            model.ParcelLength = Convert.ToDecimal(Environment.GetEnvironmentVariable("P_Lendth"));
                        }
                        else
                        {
                            model.ParcelHeight = Convert.ToDecimal(Environment.GetEnvironmentVariable("NP_Height"));
                            model.ParcelWidth = Convert.ToDecimal(Environment.GetEnvironmentVariable("NP_Width"));
                            model.ParcelLength = Convert.ToDecimal(Environment.GetEnvironmentVariable("NP_Lendth"));
                        }

                        //var itemsTotalWeight = await _context.OrderItemDetails.Where(m => m.orderid == order.id).SumAsync(m => m.product_weight * m.quantity);

                        //log.LogInformation("Order Product Weight: " + itemsTotalWeight);

                        //if (itemsTotalWeight == 0)
                        //    itemsTotalWeight = Convert.ToDecimal(Environment.GetEnvironmentVariable("DefaultProductWeight"));

                        //log.LogInformation("Order Product Weight When Default: " + itemsTotalWeight);

                        //var ParcelWeight = Convert.ToDecimal((order.ColdChain == true) ? Environment.GetEnvironmentVariable("P_Weight") : Environment.GetEnvironmentVariable("NP_Weight"));
                        //model.ParcelWeight = ParcelWeight + Convert.ToDecimal(itemsTotalWeight);
                        //log.LogInformation("Final Parcel Weight: " + model.ParcelWeight);

                        var weight = (order.ColdChain == true) ? Environment.GetEnvironmentVariable("P_Weight") : Environment.GetEnvironmentVariable("NP_Weight");
                        model.ParcelWeight = Convert.ToDecimal(weight);

                        //var MaximumParcelWeight = Convert.ToDecimal(Environment.GetEnvironmentVariable("MaximumParcelWeight"));
                        //log.LogInformation("Maximum Parcel Weight: " + MaximumParcelWeight);

                        //if (model.ParcelWeight > MaximumParcelWeight)
                        //{
                        //    log.LogError("Parcel weight is greater than " + MaximumParcelWeight + "kg, So skipped for label generation.");
                        //    order.UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                        //    order.TryCount = 5;
                        //    _context.OrderDetails.Update(order);
                        //    await _context.SaveChangesAsync();
                        //    return false;
                        //}

                        var shippingModel = model.ToModel();
                        foreach (var item in shippingModel.shipments) item.service_code = ServiceCode;

                        var shippingDetails = await CreateShipping(shippingModel);

                        if (shippingDetails.shipments != null && shippingDetails.shipments.Count > 0)
                        {
                            var shippingLabelModel = new ShippingLabelRequest();
                            shippingLabelModel.validate_address = "no_validation";
                            var jsonShipToString = JsonConvert.SerializeObject(shippingLabelModel);
                            order.RateID = shippingDetails.shipments.Select(m => m.shipment_id).FirstOrDefault();
                            var shippingLabelDetailsNew = await CreateShippingLabelError(order.RateID, jsonShipToString);

                            //if (string.IsNullOrEmpty(shippingLabelDetailsNew))
                            //{
                            //    responseModel.status = "error";
                            //    responseModel.message = "Sorry! Failed to create a shipment label.";
                            //    responseModel.error_details = "Sorry! Failed to create a shipment label";
                            //    return responseModel;
                            //}

                            log.LogInformation("Shipengine Label Create Response: " + shippingLabelDetailsNew);

                            var shippingLabelDetails = JsonConvert.DeserializeObject<ShippingLabelResponse>(shippingLabelDetailsNew);

                            if (shippingLabelDetails == null || string.IsNullOrEmpty(shippingLabelDetails.label_id) || string.IsNullOrEmpty(shippingLabelDetails.shipment_id))
                            {
                                var shppingResponseModelError = JsonConvert.DeserializeObject<ShipmentErrorResponse>(shippingLabelDetailsNew);

                                if (shppingResponseModelError != null && shppingResponseModelError.errors != null && shppingResponseModelError.errors.Count > 0)
                                {
                                    var error = shppingResponseModelError.errors.Select(m => m.message).FirstOrDefault();

                                    if (string.IsNullOrEmpty(error))
                                        error = "Sorry! Failed to create a shipment label";

                                    log.LogError("Shipping Label Generate Error : " + order.forge_order_id + error);
                                    //return false;
                                }
                                else
                                {
                                    log.LogError("Sorry! Failed to create a shipment label: " + order.forge_order_id);
                                    //return false;
                                }

                                order.UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                                order.TryCount += 1;
                                _context.OrderDetails.Update(order);
                                await _context.SaveChangesAsync();
                            }

                            if (shippingLabelDetails != null)
                            {
                                order.TrackingNo = shippingLabelDetails.tracking_number;
                                order.TrackingURL = shippingLabelDetails.tracking_url;
                                order.ShippingLabel = shippingLabelDetails.label_download.zpl;
                                order.UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                                order.TryCount += 1;
                                _context.OrderDetails.Update(order);
                                await _context.SaveChangesAsync();
                                await CreateShipmentLabel(order.RateID, shippingLabelDetails.label_id, order.id, order.forge_order_id, shippingLabelDetails.label_download.png);
                                log.LogInformation("Shipping label generated successfully for the order: " + order.forge_order_id);

                                //Call the Invoice Create method
                                var invoicePDFPath = await CreateInvoice(order.id);
                                log.LogInformation("Invoice Path: " + invoicePDFPath);
                                retval = true;
                            }
                            else
                            {
                                order.UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                                order.TryCount += 1;
                                _context.OrderDetails.Update(order);
                                await _context.SaveChangesAsync();
                                log.LogError("Sorry! Failed to create a shipment label. " + order.forge_order_id);
                                //return false;
                            }
                        }
                        else
                        {
                            order.UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                            order.TryCount += 1;
                            _context.OrderDetails.Update(order);
                            await _context.SaveChangesAsync();

                            log.LogError("Sorry! Failed to create a shipment. " + order.forge_order_id);
                            //return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        order.UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                        order.TryCount += 1;
                        _context.OrderDetails.Update(order);
                        await _context.SaveChangesAsync();
                        log.LogError(ex, order.forge_order_id + " Error while create a shipping label. " + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while posting the pending sales order. " + ex.ToString());
                retval = false;
            }

            return retval;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            // Thread-safe cache check
            lock (_tokenLock)
            {
                if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
                    return _cachedToken;
            }

            try
            {
                var TokenURL = Environment.GetEnvironmentVariable("TokenURL");
                var ClientID = Environment.GetEnvironmentVariable("ClientID");
                var Tenant = Environment.GetEnvironmentVariable("Tenant");
                var ClientSecret = Environment.GetEnvironmentVariable("ClientSecret");
                var Scope = Environment.GetEnvironmentVariable("Scope");

                using (HttpClient client = new HttpClient())
                {
                    var requestData = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("client_id", ClientID),
                        new KeyValuePair<string, string>("client_secret", ClientSecret),
                        new KeyValuePair<string, string>("scope", Scope),
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    });

                    HttpResponseMessage response = await client.PostAsync(TokenURL, requestData);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(responseBody);
                        var token = json["access_token"]?.ToString();
                        var expiresIn = json["expires_in"]?.ToObject<int?>() ?? 3599;

                        if (!string.IsNullOrEmpty(token))
                        {
                            lock (_tokenLock)
                            {
                                _cachedToken = token;
                                _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60); // Renew 1 min before expiry
                            }
                            return token;
                        }
                    }
                }
            }
            catch
            {
                // Optionally log the exception
            }

            return string.Empty;
        }

        public async Task<ShipmentResponse> CreateShipping(ShippingShipEngineRequest model)
        {
            try
            {
                var jsonString = JsonConvert.SerializeObject(model);
                var shppingResponseData = await APICall.PostCallShip("/shipments", jsonString);
                var shppingResponseModel = JsonConvert.DeserializeObject<ShipmentResponse>(shppingResponseData);

                if (shppingResponseModel != null && !shppingResponseModel.has_errors)
                    return shppingResponseModel;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public async Task<string> CreateShippingLabelError(string shipmentid, string model)
        {
            try
            {
                return await APICall.PostCallShip("/labels/shipment/" + shipmentid, model);
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public async Task<bool> CreateShipmentLabel(string shipmentID, string labelID, int orderID, string forgeOrderId, string labelPath)
        {
            var retval = false;

            try
            {
                if (string.IsNullOrEmpty(shipmentID) || string.IsNullOrEmpty(labelID) || orderID == 0)
                    return retval;

                var label = new LabelManagement
                {
                    ShipmentID = shipmentID,
                    OrderID = orderID,
                    ForgeOrderID = forgeOrderId,
                    LabelID = labelID,
                    GroupID = "",
                    LabelPath = labelPath,
                    IsManifested = false,
                    IsCancelled = false,
                    CreatedDate = TimeZoneHelper.GetCurrentAustraliaTime(),
                    UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime()
                };

                await _context.LabelManagements.AddAsync(label);
                await _context.SaveChangesAsync();
                await Create(shipmentID + "-" + labelID + "-" + orderID + "-" + forgeOrderId, label, "success", "labelID", "shipmentlabelgenerate");
                retval = true;
            }
            catch (Exception ex)
            {
                await Create(shipmentID + "-" + labelID + "-" + orderID + "-" + forgeOrderId, ex.ToString(), "error", "", "shipmentlabelgenerate");
            }

            return retval;
        }

        public async Task<bool> Create(object from, object to, string status, string bc_id, string description)
        {
            var retval = false;

            try
            {
                if (from != null && to != null && !string.IsNullOrEmpty(status))
                {
                    var logs = new ActivityLog();
                    logs.FromRequest = JsonConvert.SerializeObject(from);
                    logs.ToResponse = JsonConvert.SerializeObject(to);
                    logs.Description = description;
                    logs.CreatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                    logs.UpdatedDate = TimeZoneHelper.GetCurrentAustraliaTime();
                    logs.Status = status;
                    logs.StatusCode = status == "success" ? (!string.IsNullOrEmpty(bc_id) ? 200 : 201) : 400;
                    await _context.ActivityLogs.AddAsync(logs);
                    await _context.SaveChangesAsync();
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);
                    var myDate = TimeZoneHelper.GetCurrentAustraliaTime();

                    var logFile = Path.Combine(logDirectory, $"ErrorLog_{myDate:yyyy-MM-dd}.txt");

                    var logMessage = $@"
                                ==============================
                                Date: {myDate}
                                Message: {ex.Message}
                                StackTrace: {ex.StackTrace}
                                InnerException: {ex.InnerException?.Message}
                                ==============================";

                    await File.AppendAllTextAsync(logFile, logMessage);
                }
                catch
                {
                    // fallback if file logging itself fails
                }
            }

            return retval;
        }

        public async Task<string> CreateInvoice(int orderID)
        {
            var retval = "Failed to create the invoice.";

            try
            {
                // Implementation for creating invoice goes here

                var token = await GetAccessTokenAsync();
                var DefaultCompanyID = Environment.GetEnvironmentVariable("DefaultCompanyID");

                var order = await _context.OrderDetails.Where(m => m.id == orderID).FirstOrDefaultAsync();
                if (order == null)
                    return "Failed to find the order details for the invoice.";

                var orderItemList = await _context.OrderItemDetails.Where(m => m.orderid == orderID).ToListAsync();
                if (orderItemList == null || orderItemList.Count == 0)
                    return "Failed to find the order item details for the invoice.";

                var salesOrderResponseData = await APICall.GetCallBC("v2.0/companies(" + DefaultCompanyID + ")/salesInvoices(" + order.InvoiceID + ")", token);

                if (!string.IsNullOrEmpty(salesOrderResponseData))
                {
                    var syncResponse = JsonConvert.DeserializeObject<BCSalesOrderResponse>(salesOrderResponseData);
                    if (syncResponse != null)
                    {
                        var salesOrderItemResponseData = await APICall.GetCallBC("v2.0/companies(" + DefaultCompanyID + ")/salesInvoices(" + order.InvoiceID + ")/salesInvoiceLines", token);

                        if (!string.IsNullOrEmpty(salesOrderItemResponseData))
                        {
                            var syncResponseInner = JsonConvert.DeserializeObject<BCSalesOrderLineListResponse>(salesOrderItemResponseData);
                            if (syncResponseInner != null && syncResponseInner.value != null && syncResponseInner.value.Count > 0)
                            {
                                var data = syncResponse.ToInvoiceResponse();
                                data.items = syncResponseInner.value.ToInvoiceResponse();

                                var pdfInvoiceResponse = syncResponse.ToPdfInvoiceResponse();
                                pdfInvoiceResponse.items = data.items;

                                retval = await CreateSalesInvoice(pdfInvoiceResponse, order, orderItemList, token).ConfigureAwait(false);
                            }
                        }
                    }
                }
                else
                {
                    retval = "Sorry! Invoice details not found.";
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                retval = ex.ToString();
            }

            return retval;
        }

        public async Task<string> CreateSalesInvoice(PdfInvoiceResponse invoicerequest, OrderDetail order, List<OrderItemDetail> orderItemList, string token)
        {
            var retval = "";
            QuestPDF.Settings.License = LicenseType.Community;

            try
            {
                var settings = await _context.AppConfigurationSettings
                .Where(o => o.IsDelete == false)
                .ToListAsync()
                .ConfigureAwait(false);

                if (settings == null || settings.Count == 0)
                    return "No settings found for the application";

                var blobConnectionString = settings.Where(m => m.SystemName == "invoiceblobconnectionstring").Select(m => m.DisplayValue).FirstOrDefault();
                var containerName = settings.Where(m => m.SystemName == "invoicepdfcontainername").Select(m => m.DisplayValue).FirstOrDefault();
                var blobStorageEndpoint = settings.Where(m => m.SystemName == "invoiceblobstorageendpoint").Select(m => m.DisplayValue).FirstOrDefault();

                var tempOrderItemList = orderItemList.Select(m => new SalesOrderItemPackWebResponse()
                {
                    product_no = m.bc_product_id,
                    product_name = m.name,
                    quantity = m.quantity,
                    unit_price = m.unit_price,
                    total_price = m.vat_rate
                }).ToList();

                foreach (var item in tempOrderItemList)
                {
                    var productDetail = await GetProductNumberByID(item.product_no, token);

                    if (!string.IsNullOrEmpty(productDetail.medication_Quantity) && !string.IsNullOrEmpty(productDetail.medicationForm))
                    {
                        if (productDetail.medicationForm.ToLower().Replace(" ", "") == "capsule" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "condom" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "condoms" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "cream" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "device" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "gel" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "jelly" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "injection/pen" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "injectionpen" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "liquid" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "liquidsachets" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "lotion" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "ointment" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "pillcutt" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "pack" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "patch" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "powder" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "serum" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "shampoo" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "sheet" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "spray" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "tablet" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "tablets" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "wafer" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "wafers" ||
                                productDetail.medicationForm.ToLower().Replace(" ", "") == "ring")
                        {
                            var myBaseQty = Convert.ToInt32(productDetail.medication_Quantity);
                            item.quantity = item.quantity * myBaseQty;
                        }
                    }
                }

                var ApprovalNo = Environment.GetEnvironmentVariable("ApprovalNo");
                var ABNNumber = Environment.GetEnvironmentVariable("ABNNumber");

                using var stream = new MemoryStream();
                byte[] logoBytes;

                using (var httpClient = new HttpClient())
                    logoBytes = await httpClient.GetByteArrayAsync("https://prod-pack-app-soh.azurewebsites.net/assets/images/LogoWithText.png");

                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.DefaultTextStyle(x => x.FontSize(10));
                        page.Header().Column(header =>
                        {
                            // Row for logo
                            header.Item().Row(row =>
                            {
                                if (logoBytes != null && logoBytes.Length > 0)
                                {
                                    row.ConstantItem(90)
                                        .Height(40)
                                        .AlignLeft()
                                        .Image(logoBytes, ImageScaling.FitArea);
                                }

                                row.RelativeItem(); // Spacer to fill row
                            });

                            // Row for centered company name
                            header.Item().AlignCenter().Column(col =>
                            {
                                col.Item().Text("RACECOURSE ROAD PHARMACY").FontSize(10).AlignCenter();
                                col.Item().Text("M.LEUNG & H.YOO 119 RACECOURSE RD").FontSize(10).AlignCenter();
                                col.Item().Text("ASCOT 4007 Ph: 32683222").FontSize(10).AlignCenter();
                            });
                        });

                        var customerAddress = "";

                        if (!string.IsNullOrEmpty(invoicerequest.shipToAddressLine1))
                            customerAddress = invoicerequest.shipToAddressLine1;

                        if (!string.IsNullOrEmpty(invoicerequest.shipToAddressLine2))
                            customerAddress += " " + invoicerequest.shipToAddressLine2;

                        if (!string.IsNullOrEmpty(invoicerequest.shipToPostCode))
                            customerAddress += " " + invoicerequest.shipToPostCode;

                        if (!string.IsNullOrEmpty(invoicerequest.shipToCity))
                            customerAddress += " " + invoicerequest.shipToCity;

                        if (!string.IsNullOrEmpty(invoicerequest.shipToState))
                            customerAddress += " " + invoicerequest.shipToState;

                        if (!string.IsNullOrEmpty(invoicerequest.shipToCountry))
                            customerAddress += " " + invoicerequest.shipToCountry;

                        page.Content().PaddingVertical(20).Column(col =>
                        {
                            col.Item().PaddingVertical(10).Text("Official Pharmacy Receipt").Bold().FontSize(14).AlignCenter();
                            //col.Item().Text("(TAX INVOICE – ABN " + ABNNumber + ")").Italic().FontSize(12).AlignCenter();

                            col.Item().Row(row =>
                            {
                                row.RelativeItem().PaddingLeft(50).Text("(TAX INVOICE – ABN " + ABNNumber + ")").Italic().FontSize(12);
                                row.ConstantItem(150).Text("Private SN20DR-SNQ: NO").FontSize(12);
                            });

                            col.Item().AlignLeft().PaddingLeft(50).PaddingTop(50).Column(customer =>
                            {
                                customer.Item().PaddingBottom(20).Row(row =>
                                {
                                    row.ConstantColumn(90).Text("Approval No:").Bold().FontSize(12);
                                    row.RelativeColumn().Text(ApprovalNo).Bold().FontSize(12);
                                });
                                customer.Item().Text("Customer Details:").Bold().FontSize(12).Underline();
                                customer.Item().PaddingTop(5).Row(row =>
                                {
                                    row.ConstantColumn(90).Text("Name:").Bold();
                                    row.RelativeColumn().Text(invoicerequest.billToName);
                                });
                                customer.Item().Row(row =>
                                {
                                    row.ConstantColumn(90).Text("Address:").Bold();
                                    row.RelativeColumn().Text(customerAddress);
                                });

                            });
                            col.Spacing(10);
                            col.Item().AlignLeft().PaddingLeft(50).PaddingTop(20).Column(Supply =>
                            {
                                Supply.Item().Text("Supply Details:").Bold().FontSize(12).Underline();
                                Supply.Item().PaddingTop(5).Row(row =>
                                {
                                    row.ConstantColumn(90).Text("Prescriber Name:").Bold();
                                    row.RelativeColumn().Text(order.PrescriberName);
                                });

                                if (!string.IsNullOrEmpty(order.PrescriberNo))
                                {
                                    Supply.Item().Row(row =>
                                    {
                                        row.ConstantColumn(90).Text("Prescriber No:").Bold();
                                        row.RelativeColumn().Text(order.PrescriberNo);
                                    });
                                }
                                Supply.Item().Row(row =>
                                {
                                    row.ConstantColumn(90).Text("Script No:").Bold();
                                    row.RelativeColumn().Text(order.forge_order_id);
                                });
                                Supply.Item().Row(row =>
                                {
                                    row.ConstantColumn(90).Text("Supply Date:").Bold();
                                    row.RelativeColumn().Text(order.CreatedDate.Date.ToString("dd/MM/yyyy"));
                                });
                            });

                            col.Spacing(10);

                            col.Item().AlignLeft().PaddingLeft(50).PaddingTop(30).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(50); // Item.
                                    cols.RelativeColumn(15);  // Quantity
                                    cols.RelativeColumn(15);  // Price
                                    cols.RelativeColumn(20); // Line Amount
                                });

                                // Header with borders
                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Item").Bold();
                                    header.Cell().Element(CellStyle).Text("Quantity").Bold();
                                    header.Cell().Element(CellStyle).Text("Price").Bold();
                                    header.Cell().Element(CellStyle).Text("GST").Bold();
                                });

                                // Data row with borders

                                if (tempOrderItemList != null && tempOrderItemList.Count > 0)
                                {
                                    foreach (var item in tempOrderItemList)
                                    {
                                        table.Cell().Element(CellStyle).Text(item.product_name);
                                        table.Cell().Element(CellStyle).Text(item.quantity);
                                        table.Cell().Element(CellStyle).Text("$" + item.unit_price);
                                        table.Cell().Element(CellStyle).Text("$" + item.total_price);
                                    }
                                }

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container
                                        .Border(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(8);
                                }
                            });

                            col.Spacing(10);

                            col.Item().AlignLeft().PaddingLeft(50).Column(totals =>
                            {
                                totals.Spacing(5);

                                // Subtotal
                                totals.Item().Row(row =>
                                {
                                    row.RelativeItem().Text("Total Paid").Bold();
                                    row.ConstantItem(80).Text("$" + order.total_amount).Bold();
                                });

                                // Optional: draw a thinner line below
                                totals.Item().PaddingTop(20).LineHorizontal(1).LineColor(Colors.Black);
                            });
                        });

                        page.Footer().AlignCenter().Column(column =>
                        {
                            column.Item().Text("RACECOURSE ROAD PHARMACY, M.LEUNG & H.YOO").FontSize(10).AlignCenter();
                            column.Item().Text("119 RACECOURSE RD, ASCOT 4007 Ph: 32683222").FontSize(10).AlignCenter();
                        });
                    });
                });

                // Generate PDF into the stream
                document.GeneratePdf(stream);
                stream.Position = 0;
                string blobName = $"{order.InvoiceID.Replace(" ", "")}.pdf";
                var blobServiceClient = new BlobServiceClient(blobConnectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();

                var uploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = "application/pdf"
                    }
                };

                var tt = await blobClient.UploadAsync(stream, uploadOptions);
                retval = blobStorageEndpoint + "" + containerName + "/" + blobName;
            }
            catch (Exception ex)
            {
                retval = ex.ToString();
            }

            return retval;
        }

        public async Task<BCProductResponse> GetProductNumberByID(string id, string token)
        {
            try
            {
                var Publisher = Environment.GetEnvironmentVariable("Publisher");
                var DefaultCompanyID = Environment.GetEnvironmentVariable("DefaultCompanyID");
                var url = $"{Publisher}/items/v2.0/itemsUpdate(id={id})?company={DefaultCompanyID}";
                var bcProductDetailResponse = await APICall.GetCallBC(url, token);

                if (string.IsNullOrEmpty(bcProductDetailResponse))
                    return null;

                return JsonConvert.DeserializeObject<BCProductResponse>(bcProductDetailResponse);
            }
            catch
            {
                return null;
            }
        }
    }
}
