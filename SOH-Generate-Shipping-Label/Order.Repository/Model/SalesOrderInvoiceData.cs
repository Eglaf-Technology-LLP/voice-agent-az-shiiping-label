using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class InvoiceResponse
    {
        public InvoiceResponse()
        {
            this.status = "success";
            this.message = "";
            this.error_details = "";
        }

        public string status { get; set; }
        public string message { get; set; }
        public string? error_details { get; set; }
        public SalesOrderInvoiceData data { get; set; }
    }

    public class SalesOrderInvoiceData
    {
        public string orderId { get; set; }
        public string orderNumber { get; set; }
        public string bc_invoice_id { get; set; }
        public string forge_order_id { get; set; }
        public string customer_id { get; set; }
        public DateTime? order_date { get; set; }
        public DateTime? due_date { get; set; }
        public string status { get; set; }
        public decimal total_amount { get; set; }
        public decimal totalTaxAmount { get; set; }
        public decimal totalAmountExcludingTax { get; set; }
        public string currency { get; set; }
        public List<SalesOrderLineInvoiceResponse> items { get; set; }
        public ShippingAddress billing_address { get; set; }
        public string payment_method { get; set; }
        public string invoice_pdf_url { get; set; }
    }

    public class SalesOrderLineInvoiceResponse
    {
        public string product_id { get; set; }
        public string variant_id { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public decimal unit_price { get; set; }
        public decimal total_price { get; set; }
        public decimal vat_rate { get; set; }
        public string description { get; set; }
        public DateTime? shipmentDate { get; set; }
        public string unitOfMeasureCode { get; set; }
    }

    public class PdfInvoiceResponse
    {
        public string customer_id { get; set; }
        public string bc_invoice_id { get; set; }
        public DateTime? order_date { get; set; }
        public string orderId { get; set; }
        public DateTime? due_date { get; set; }
        public string status { get; set; }
        public string orderNumber { get; set; }
        public string billToName { get; set; }
        public string shipToName { get; set; }
        public string shipToAddressLine1 { get; set; }
        public string shipToAddressLine2 { get; set; }
        public string shipToCity { get; set; }
        public string shipToCountry { get; set; }
        public string shipToState { get; set; }
        public string shipToPostCode { get; set; }
        public string paymentTerms { get; set; }
        public string shipmentMethod { get; set; }
        public string paymentMethod { get; set; }
        public string shippingAgentCode { get; set; }
        public string packagetrackingno { get; set; }
        public decimal totalAmountExcludingTax { get; set; }
        public decimal total_amount { get; set; }
        public decimal totalTaxAmount { get; set; }
        public string currency { get; set; }
        public string billToAddressLine1 { get; set; }
        public string billToAddressLine2 { get; set; }
        public string billToCity { get; set; }
        public string billToPostCode { get; set; }
        public string billToCountry { get; set; }
        public string billToState { get; set; }
        public string payment_method { get; set; }

        public List<SalesOrderLineInvoiceResponse> items { get; set; }
    }

    public class ShippingAddress
    {
        public string address_line_1 { get; set; }
        public string? address_line_2 { get; set; }
        public string city { get; set; }
        public string? suburb { get; set; }
        public string postcode { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }
}
