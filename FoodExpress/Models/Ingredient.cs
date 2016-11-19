using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
   public class Ingredient
    {
        public int IDIngrdient { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="Enter Ingredient Name")]
        [Display(Name ="Name")]
        public string NameIngredient { get; set; }
        [Display(Name = "Restautant")]
        public string NameRes { get; set; }
        public int IDRes { get; set; }
        public bool Active { get; set; }

    }
}
