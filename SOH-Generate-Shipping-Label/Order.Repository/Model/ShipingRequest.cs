using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class ShipingRequest
    {
        public string RateID { get; set; }

        public ShipFrom FromAddress { get; set; }

        public ShipTo ToAddress { get; set; }

        public decimal ParcelWeight { get; set; }

        public decimal ParcelLength { get; set; }

        public decimal ParcelWidth { get; set; }

        public decimal ParcelHeight { get; set; }
    }

    public class ShipFrom
    {
        public string name { get; set; }
        public string company_name { get; set; }
        public string address_line1 { get; set; }
        public string city_locality { get; set; }
        public string state_province { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
        public string phone { get; set; }
    }

    public class ShipTo
    {
        public string name { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public string address_line1 { get; set; }
        public string? address_line2 { get; set; }
        public string city_locality { get; set; }
        public string state_province { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public class Package
    {
        public Weight weight { get; set; }
        public Dimensions dimensions { get; set; }
    }

    public class Weight
    {
        public decimal value { get; set; }
        public string unit { get; set; }
    }

    public class Dimensions
    {
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public string unit { get; set; }
    }
}
