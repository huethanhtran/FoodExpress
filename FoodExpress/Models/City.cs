using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class City
    {
        public int IDCity { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter name cuisine")]
        [Display(Name = "Name")]
        public string NameCity { get; set; }
        public bool Active { get; set; }
    }
}
