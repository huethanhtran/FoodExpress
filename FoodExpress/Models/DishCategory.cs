using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class DishCategory
    {
        public int IDDishCate { get; set; }
        [Required(AllowEmptyStrings =false, ErrorMessage ="Enter Category Name")]
        [Display(Name ="Name")]
        public string NameCate { get; set; }
        public bool Active { get; set; }
    }
}
