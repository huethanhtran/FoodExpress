using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class Restaurant
    {

        public int IDRes { get; set; }
        [Display(Name ="Restaurant Name")]
        [Required(AllowEmptyStrings =false, ErrorMessage ="Enter restaurant name")]
        public string NameRes { get; set; }
        [MaxLength(200, ErrorMessage ="Summary is too long")]
        public string Summary { get; set; }
        [Display(Name = "Description")]
        public string DescriptionRes { get; set; }
        [Display(Name = "Address")]
        [MaxLength(500)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter restaurant address")]
        public string ResAddress { get; set; }
        [DataType(DataType.PhoneNumber, ErrorMessage ="Enter correct phone number")]
        public string Phone { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Enter correct email format (Ex : abc@gmail.com)")]
        public string Email { get; set; }

        public string Website { get; set; }

        public string Fax { get; set; }

        public System.Nullable<int> IDCity { get; set; }
       
        public System.Nullable<int> OwnerId { get; set; }
        [Display(Name = "Open Time")]
        [DataType(DataType.Time)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Set open time")]
        public System.Nullable<System.TimeSpan> TimeStart { get; set; }
        [Display(Name = "Close Time")]
        [DataType(DataType.Time)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Set close time")]
        public System.Nullable<System.TimeSpan> TimeEnd { get; set; }
        [Display(Name = "Break Time Start")]
        [DataType(DataType.Time)]
        public System.Nullable<System.TimeSpan> TimeBreakStart { get; set; }
        [Display(Name = "Break Time End")]
        [DataType(DataType.Time)]
        public System.Nullable<System.TimeSpan> TimeBreakEnd { get; set; }
        [Display(Name = "Register Date")]
        [DataType(DataType.DateTime)]
        public System.Nullable<System.DateTime> CreatedOn { get; set; }

        public bool Active { get; set; }
        [Display(Name = "Service Fee")]
        [DataType(DataType.Currency)]
        public System.Nullable<decimal> ServiceFee { get; set; }
        [DataType(DataType.Currency)]
        public System.Nullable<decimal> Commission { get; set; }
        [Display(Name = "City")]
        public string NameCity { get; set; }
        public string Avatar { get; set; }
        public List<FoodExpress.City> City { get; set; }
       
        public string Owner { get; set; }
        [Display(Name = "Category")]
        public List<Res_Categoty_Mapping> Res_Categoty_Mappings { get; set; }
        [Display(Name = "Cuisine")]
        public List<Res_Cuisine_Mapping> Res_Cuisine_Mappings { get; set; }
    }
}
