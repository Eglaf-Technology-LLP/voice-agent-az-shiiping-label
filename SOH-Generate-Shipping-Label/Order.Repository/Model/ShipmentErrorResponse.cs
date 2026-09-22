using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class ShipmentErrorResponse
    {
        public string request_id { get; set; }
        public List<Error> errors { get; set; }
    }

    public class Error
    {
        public string error_source { get; set; }
        public string error_type { get; set; }
        public string error_code { get; set; }
        public string message { get; set; }
        public string carrier_id { get; set; }
        public string carrier_code { get; set; }
        public string carrier_name { get; set; }
    }
}
