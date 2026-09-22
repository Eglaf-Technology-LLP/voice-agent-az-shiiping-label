using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class BCSalesOrderResponse
    {
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string id { get; set; }
        public string number { get; set; }
        public string externalDocumentNumber { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime postingDate { get; set; }
        public string customerId { get; set; }
        public string customerNumber { get; set; }
        public string customerName { get; set; }
        public string billToName { get; set; }
        public string billToCustomerId { get; set; }
        public string billToCustomerNumber { get; set; }
        public string shipToName { get; set; }
        public string shipToContact { get; set; }
        public string sellToAddressLine1 { get; set; }
        public string sellToAddressLine2 { get; set; }
        public string sellToCity { get; set; }
        public string sellToCountry { get; set; }
        public string sellToState { get; set; }
        public string sellToPostCode { get; set; }
        public string billToAddressLine1 { get; set; }
        public string billToAddressLine2 { get; set; }
        public string billToCity { get; set; }
        public string billToCountry { get; set; }
        public string billToState { get; set; }
        public string billToPostCode { get; set; }
        public string shipToAddressLine1 { get; set; }
        public string shipToAddressLine2 { get; set; }
        public string shipToCity { get; set; }
        public string shipToCountry { get; set; }
        public string shipToState { get; set; }
        public string shipToPostCode { get; set; }
        public string shortcutDimension1Code { get; set; }
        public string shortcutDimension2Code { get; set; }
        public string currencyId { get; set; }
        public string currencyCode { get; set; }
        public bool pricesIncludeTax { get; set; }
        public string paymentTermsId { get; set; }
        public string shipmentMethodId { get; set; }
        public string salesperson { get; set; }
        public bool partialShipping { get; set; }
        public DateTime requestedDeliveryDate { get; set; }
        public decimal discountAmount { get; set; }
        public bool discountAppliedBeforeTax { get; set; }
        public decimal totalAmountExcludingTax { get; set; }
        public decimal totalTaxAmount { get; set; }
        public decimal totalAmountIncludingTax { get; set; }
        public bool fullyShipped { get; set; }
        public string status { get; set; }
        public DateTime lastModifiedDateTime { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string orderId { get; set; }
        public string orderNumber { get; set; }
    }
}
