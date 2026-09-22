using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Model
{
    public class SalesOrderItemPackWebResponse
    {
        public string order_item_id { get; set; }
        public string? product_no { get; set; }
        public string product_name { get; set; }
        public int quantity { get; set; }
        public int scanned_quantity { get; set; }
        public string? dosage_label { get; set; }
        public int? scanned_dosage_label { get; set; }
        public string? product_barcode { get; set; }
        public decimal unit_price { get; set; }
        public decimal total_price { get; set; }
    }
}
