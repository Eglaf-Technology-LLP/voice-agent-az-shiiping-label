using Order.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Factory
{
    public static class ModelFactory
    {
        public static ShippingShipEngineRequest ToModel(this ShipingRequest request)
        {
            return new ShippingShipEngineRequest()
            {
                shipments = new List<Shipment>()
                {
                    new Shipment()
                    {
                        service_code = "",
                        confirmation = "adult_signature",
                        authority_to_leave = false,
                        ship_from = new ShipFrom()
                        {
                            name = request.FromAddress.name,
                            company_name = request.FromAddress.company_name,
                            address_line1 = request.FromAddress.address_line1,
                            city_locality = request.FromAddress.city_locality,
                            state_province = request.FromAddress.state_province,
                            postal_code = request.FromAddress.postal_code,
                            country_code = request.FromAddress.country_code,
                            phone = request.FromAddress.phone
                        },
                        ship_to = new ShipTo()
                        {
                            name = request.ToAddress.name,
                            phone = !string.IsNullOrEmpty(request.ToAddress.phone)?request.ToAddress.phone:"",
                            email = !string.IsNullOrEmpty(request.ToAddress.email)?request.ToAddress.email:"",
                            address_line1 = request.ToAddress.address_line1,
                            address_line2 = !string.IsNullOrEmpty(request.ToAddress.address_line2)?request.ToAddress.address_line2:"",
                            city_locality = request.ToAddress.city_locality,
                            state_province = request.ToAddress.state_province,
                            postal_code = request.ToAddress.postal_code,
                            country_code = request.ToAddress.country_code
                        },
                        packages = request.ToPackageModel()
                    }
                },
            };
        }

        public static List<Package> ToPackageModel(this ShipingRequest request)
        {
            return new List<Package>() {
                    new Package()
                    {
                        weight = new Weight()
                        {
                            value = request.ParcelWeight,
                            //unit = "pound"
                            unit = "kilogram"
                        },
                        dimensions = new Dimensions()
                        {
                            height = request.ParcelHeight,
                            width = request.ParcelWidth,
                            length = request.ParcelLength,
                            unit = "inch"
                        }
                    }
                };
        }

        public static SalesOrderInvoiceData ToInvoiceResponse(this BCSalesOrderResponse request)
        {
            return new SalesOrderInvoiceData()
            {
                forge_order_id = "",
                bc_invoice_id = request.id,
                customer_id = request.customerId,
                order_date = request.orderDate,
                due_date = request.requestedDeliveryDate,
                status = request.status,
                total_amount = request.totalAmountIncludingTax,
                currency = request.currencyCode,
                payment_method = "stripe",
                //invoice_pdf_url = "https://www.orbitsoft.co.uk/assets/sampleinvoice.pdf",
                billing_address = new ShippingAddress()
                {
                    address_line_1 = request.billToAddressLine1,
                    address_line_2 = request.billToAddressLine2,
                    city = request.billToCity,
                    postcode = request.billToPostCode,
                    country = request.billToCountry,
                },
                orderId = request.orderId,
                orderNumber = request.orderNumber,
                totalTaxAmount = request.totalTaxAmount,
                totalAmountExcludingTax = request.totalAmountExcludingTax,

            };
        }

        public static PdfInvoiceResponse ToPdfInvoiceResponse(this BCSalesOrderResponse request)
        {
            return new PdfInvoiceResponse()
            {
                bc_invoice_id = request.id,
                customer_id = request.customerId,
                order_date = request.orderDate,
                due_date = request.requestedDeliveryDate,
                status = request.status,
                total_amount = request.totalAmountIncludingTax,
                currency = request.currencyCode,
                payment_method = "stripe",
                billToAddressLine1 = request.billToAddressLine1,
                billToAddressLine2 = request.billToAddressLine2,
                billToCity = request.billToCity,
                billToPostCode = request.billToPostCode,
                billToCountry = request.billToCountry,
                billToState = request.billToState,
                shipToAddressLine1 = request.shipToAddressLine1,
                shipToAddressLine2 = request.shipToAddressLine2,
                shipToCity = request.shipToCity,
                shipToCountry = request.shipToCountry,
                shipToState = request.shipToState,
                shipToPostCode = request.shipToPostCode,
                billToName = request.billToName,
                shipToName = request.billToName,
                orderId = request.orderId,
                orderNumber = request.orderNumber,
                totalTaxAmount = request.totalTaxAmount,
                totalAmountExcludingTax = request.totalAmountExcludingTax,

            };
        }

        public static SalesOrderLineInvoiceResponse ToInvoiceResponse(this BCSalesOrderLineResponse request)
        {
            return new SalesOrderLineInvoiceResponse()
            {
                product_id = request.itemId,
                variant_id = request.itemVariantId,
                name = request.description,
                quantity = request.quantity,
                unit_price = request.unitPrice,
                total_price = request.amountIncludingTax,
                vat_rate = request.totalTaxAmount,
                description = request.description,
                shipmentDate = request.shipmentDate,
                unitOfMeasureCode = request.unitOfMeasureCode,
            };
        }

        public static List<SalesOrderLineInvoiceResponse> ToInvoiceResponse(this ICollection<BCSalesOrderLineResponse> salesOrderLineList)
        {
            return salesOrderLineList?.Select(t => t.ToInvoiceResponse()).ToList();
        }

    }
}
