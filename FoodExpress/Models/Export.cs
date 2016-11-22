using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodExpress.Models
{
    public class Export
    {

        public int IDExport { get; set; }
        public string Title { get; set; }
        [Display(Name = "Import Date")]
        public DateTime ExportDate { get; set; }
        [Display(Name = "Price")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Create Date")]
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public bool Active { get; set; }
        public string Exporter { get; set; }
        public List<ExportDetail> lsDetail { get; set; }
        public int IdRes { get; set; }
    }
}
