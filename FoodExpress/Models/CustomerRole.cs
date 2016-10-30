using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FoodExpress.Models
{
    public class CustomerRole
    {
        public int IDRole { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter role name")]
        [Display(Name = "Name")]
        public string NameRole { get; set; }

        public bool Active { get; set; }
    }
}