using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class Res_Cuisine
    {
        public int IDCuisine { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter name cuisine")]
        [Display(Name = "Name")]
        public string NameCuisine { get; set; }
       
        public bool Active { get; set; }
    }
}
