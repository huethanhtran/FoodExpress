using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
   public class OrderDetail
    {
        public long IDODetail { get; set; }
        public FoodExpress.Order Order { get; set; }
        public FoodExpress.Dish Dish { get; set; }
        public List<Attribute> Attributes { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool Active { get; set; }
    }
}
