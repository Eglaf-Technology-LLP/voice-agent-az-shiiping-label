using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class ShipmentResponse
    {
        public bool has_errors { get; set; }
        public List<ShipmentRespModel> shipments { get; set; }
    }

    public class ShipmentRespModel
    {
        public List<string> errors { get; set; }
        public object address_validation { get; set; }
        public string shipment_id { get; set; }
        public string carrier_id { get; set; }
        public string service_code { get; set; }
        public object requested_shipment_service { get; set; }
        public object external_shipment_id { get; set; }
        public object shipment_number { get; set; }
        public object hold_until_date { get; set; }
        public DateTime ship_date { get; set; }
        public object ship_by_date { get; set; }
        public DateTime created_at { get; set; }
        public DateTime modified_at { get; set; }
        public string shipment_status { get; set; }
        public Address ship_to { get; set; }
        public Address ship_from { get; set; }
        public object warehouse_id { get; set; }
        public Address return_to { get; set; }
        public bool is_return { get; set; }
        public string store_id { get; set; }
        public List<ShipmentPackage> packages { get; set; }
        public Weight total_weight { get; set; }
    }

    public class LabelMessages
    {
        public string reference1 { get; set; }
        public string reference2 { get; set; }
        public string reference3 { get; set; }
    }

    public class Address
    {
        public object geolocation { get; set; }
        public string instructions { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string company_name { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string address_line3 { get; set; }
        public string city_locality { get; set; }
        public string state_province { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public string address_residential_indicator { get; set; }
    }

    public class ShipmentPackage
    {
        public string shipment_package_id { get; set; }
        public string package_id { get; set; }
        public string package_code { get; set; }
        public string package_name { get; set; }
        public Weight weight { get; set; }
        public Dimensions dimensions { get; set; }
        public InsuredValue insured_value { get; set; }
        public LabelMessages label_messages { get; set; }
        public object external_package_id { get; set; }
        public object content_description { get; set; }
        public List<object> products { get; set; }
    }

    public class InsuredValue
    {
        public string currency { get; set; }
        public double amount { get; set; }
    }
}
