using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class Customer
    {
        public int IDCustomer { get; set; }
        [Display(Name ="Name")]
        public string NameCustomer { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Username")]
        public string UserName { get; set; }

        public string PasswordCustomer { get; set; }

        public string PasswordSalt { get; set; }

        public string Email;
        [Display(Name = "Address")]
        public string CustomerAddress { get; set; }
        [Display(Name = "BirthDay")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public int IDRole { get; set; }
        [Display(Name = "Register Date")]
        public DateTime CreatedOn { get; set; }

        public bool Active { get; set; }
        public string Avatar { get; set; }
        [Display(Name ="Role")]
        public string NameRole { get; set; }
    }
}
