using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public Res_Restaurant Restaurant { get; set; }
        public decimal OrderPrice { get; set; }
        [Display(Name = "Service Fee")]
        public decimal ServiceFee { get; set; }
        [Display(Name = "Shipping Fee")]
        public decimal ShippingFee { get; set; }
        [Display(Name = "Price")]
        public decimal TotalPrice { get; set; }
        public FoodExpress.Customer Customer { get; set; }
        public int UpdatedBy { get; set; }
        [Display(Name = "Paid")]
        public bool IsPay { get; set; }
        public bool Active { get; set; }
        [Display(Name ="Address")]
        public string AddressOrder { get; set; }
        [Display(Name = "Return Money")]
        public decimal ReturnBack { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
