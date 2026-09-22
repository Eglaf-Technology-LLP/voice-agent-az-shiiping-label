using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class BCSalesOrderLineResponse
    {
        [JsonProperty("@odata.etag")]
        public string OdataEtag { get; set; }
        public string id { get; set; }
        public string documentId { get; set; }
        public int sequence { get; set; }
        public string itemId { get; set; }
        public string accountId { get; set; }
        public string lineType { get; set; }
        public string lineObjectNumber { get; set; }
        public string description { get; set; }
        public string description2 { get; set; }
        public string unitOfMeasureId { get; set; }
        public string unitOfMeasureCode { get; set; }
        public int quantity { get; set; }
        public decimal unitPrice { get; set; }
        public decimal discountAmount { get; set; }
        public decimal discountPercent { get; set; }
        public bool discountAppliedBeforeTax { get; set; }
        public decimal amountExcludingTax { get; set; }
        public string taxCode { get; set; }
        public decimal taxPercent { get; set; }
        public decimal totalTaxAmount { get; set; }
        public decimal amountIncludingTax { get; set; }
        public decimal invoiceDiscountAllocation { get; set; }
        public decimal netAmount { get; set; }
        public decimal netTaxAmount { get; set; }
        public decimal netAmountIncludingTax { get; set; }
        public DateTime shipmentDate { get; set; }
        public int shippedQuantity { get; set; }
        public int invoicedQuantity { get; set; }
        public int invoiceQuantity { get; set; }
        public int shipQuantity { get; set; }
        public string itemVariantId { get; set; }
        public string locationId { get; set; }
    }
}
