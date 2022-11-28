using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.Models
{
    public class MasProductBundling
    {
        [Key]
        public string BundlingId { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; }
        public Guid TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }
        public string CreatedBy { get; set; }
        public string DateCreated { get; set; } = DateTime.Now.ToString();
        public string UpdatedBy { get; set; } = string.Empty;
        public string DateUdated { get; set; } = DateTime.Now.ToString();
        public FlagEnum Flag { get; set; } = FlagEnum.Active;
        public virtual List<MasProductBundlingData>? MasProductBundlingDatas { get; set; } = new List<MasProductBundlingData>();
    }
}
