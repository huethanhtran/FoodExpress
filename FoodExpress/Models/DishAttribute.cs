using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class DishAttribute
    {
        public int IDAttribute { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage ="Enter Attribute Name")]
        [Display(Name ="Name")]
        public string NameAttribute { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Dish")]
        public string DishName{ get; set; }
        public int IDDish { get; set; }
        public bool Active { get; set; }
    }
}
