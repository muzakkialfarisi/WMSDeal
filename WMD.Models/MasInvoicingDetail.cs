using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class MasInvoicingDetail
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(25)]
        public string InvoiceNumber { get; set; }
        [ForeignKey("InvoiceNumber")]
        public MasInvoicing? MasInvoicing { get; set; }
        [Required, StringLength(200)]
        public string CostName { get; set; }
        [Required, StringLength(100)]
        public string CostItem { get; set; }
        [Required,StringLength(150)]
        public float CostTotal { get; set; }
        [Required]
        public decimal CostAmount { get; set; }
        [Required]
        public decimal CostGrandAmount { get; set; }

    }
}
