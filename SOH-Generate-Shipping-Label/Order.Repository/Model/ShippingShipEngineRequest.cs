using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class ShippingShipEngineRequest
    {
        public List<Shipment> shipments { get; set; }
    }

    public class Shipment
    {
        public string service_code { get; set; }
        public string confirmation { get; set; }
        public bool authority_to_leave { get; set; }
        public ShipFrom ship_from { get; set; }
        public ShipTo ship_to { get; set; }
        public List<Package> packages { get; set; }
    }
}
