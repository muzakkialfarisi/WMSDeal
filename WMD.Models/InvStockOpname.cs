using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class InvStockOpname
    {
        [Key]
        public string OpnameId { get; set; }

        public Guid TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        public string HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode? MasHouseCode { get; set; }

        public DateTime DateOpname { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public int Status { get; set; }

        public virtual List<InvStockOpnameProduct>? InvStockOpnameProducts { get; set; } = new List<InvStockOpnameProduct>();
    }
}
