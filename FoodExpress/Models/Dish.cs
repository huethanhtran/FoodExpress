using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class Dish
    {
        public int IDDish { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="Enter dish name")]
        [Display(Name ="Name")]
        public string NameDish { get; set; }
        [Display(Name = "Category")]
        public string NameDishCate { get; set; }
        public int IDDishCate { get; set; }
        public int IDRes { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; }
    }
}
