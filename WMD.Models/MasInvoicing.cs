using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class MasInvoicing
    {
        [Key]
        [Required, StringLength(25)]
        public string InvoiceNumber { get ; set  ; }
        [Required]
        public Guid TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant MasDataTenant { get; set; }
        [Required, StringLength(25)]
        public string InvoicePeriod { get ; set  ; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get  ; set ; }
        [StringLength(50)]
        public string CreatedBy { get ; set ; }
        public virtual List<MasInvoicingDetail>? MasInvoicingDetails { get; set; } = new List<MasInvoicingDetail>();
    }
}
