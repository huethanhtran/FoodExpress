using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class ImportDetail
    {
        public int IDIDetail { get; set; }
        public WarehouseImport Import { get; set; }
        public FoodExpress.Ingredient Ingredient { get; set; }
        public int Quantity { get; set; }
        public int Unit { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
    }
}
