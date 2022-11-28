using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class MasStore
    {
        [Key]
        public int StoreId { get; set; }

        [StringLength(150)]
        public string Name { get; set; } = "";

        public int PlatformId { get; set; }
        [ForeignKey("PlatformId")]
        public MasPlatform? MasPlatform { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; } = "";

        [StringLength(250)]
        public string Address { get; set; } = "";

        public string? KelId { get; set; } = "";
        [ForeignKey("KelId")]
        public MasKelurahan? MasKelurahan { get; set; }

        [StringLength(10)]
        public string KodePos { get; set; } = "";

        public Guid TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
    }
}