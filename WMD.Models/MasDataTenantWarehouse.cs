using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasDataTenantWarehouse
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "WareHouse")]
        public string HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode? MasHouseCode { get; set; }

        [Display(Name = "Tenant")]
        public Guid TenantId { get; set; }
        [Display(Name = "Tenant")]
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        public ICollection<IncRequestPurchase>? IncRequestPurchase { get; set; }
        public ICollection<IncPurchaseOrder>? IncPurchaseOrder { get; set; }
    }
}
