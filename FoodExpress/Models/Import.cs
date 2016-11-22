using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
   public class Import
    {
        public int IDImport { get; set; }
        public string Title { get; set; }
        [Display(Name ="Import Date")]
        public DateTime ImportDate { get; set; }
        [Display(Name = "Price")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Create Date")]
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public bool Active { get; set; }
        public string Importer { get; set; }
        public List<ImportDetail> lsDetail { get; set; }
        public int IdRes { get; set; }
    }
}
