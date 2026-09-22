using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class ShippingLabelResponse
    {
        public string label_id { get; set; }
        public string status { get; set; }
        public string shipment_id { get; set; }
        public string external_shipment_id { get; set; }
        public string external_order_id { get; set; }
        public DateTime ship_date { get; set; }
        public DateTime created_at { get; set; }
        public AmountDetail shipment_cost { get; set; }
        public AmountDetail insurance_cost { get; set; }
        public AmountDetail requested_comparison_amount { get; set; }
        public List<object> rate_details { get; set; }
        public string tracking_number { get; set; }
        public bool is_return_label { get; set; }
        public string rma_number { get; set; }
        public bool is_international { get; set; }
        public string batch_id { get; set; }
        public string carrier_id { get; set; }
        public string service_code { get; set; }
        public string package_code { get; set; }
        public bool voided { get; set; }
        public DateTime? voided_at { get; set; }
        public string label_format { get; set; }
        public string display_scheme { get; set; }
        public string label_layout { get; set; }
        public bool trackable { get; set; }
        public string label_image_id { get; set; }
        public string carrier_code { get; set; }
        public string tracking_status { get; set; }
        public LabelDownload label_download { get; set; }
        public object form_download { get; set; }
        public object qr_code_download { get; set; }
        public object insurance_claim { get; set; }
        public object paperless_download { get; set; }
        public List<Package> packages { get; set; }
        public string charge_event { get; set; }
        public List<object> alternative_identifiers { get; set; }
        public object shipping_rule_id { get; set; }
        public string tracking_url { get; set; }
        public ShipTo ship_to { get; set; }
    }

    public class AmountDetail
    {
        public string currency { get; set; }
        public double amount { get; set; }
    }

    public class LabelDownload
    {
        public string pdf { get; set; }
        public string png { get; set; }
        public string zpl { get; set; }
        public string href { get; set; }
    }
}
