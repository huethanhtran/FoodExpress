using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class Customer
    {
        public int IDCustomer { get; set; }

        public string NameCustomer { get; set; }

        public string FirstName { get; set; }

        public string UserName;

        public string PasswordCustomer;

        public string PasswordSalt;

        public string Email;

        public string CustomerAddress;

        public DateTime DateOfBirth;

        public string Phone;

        public string Fax;

        public int IDRole;

        public DateTime CreatedOn;

        public bool Active;

    }
}
