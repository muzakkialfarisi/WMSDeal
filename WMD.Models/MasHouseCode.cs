using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    [Authorize]
    public class MasHouseCode
    {
        [Key,Required,Display(Name ="Code"),StringLength(25,MinimumLength =2)] 
        public string HouseCode { get; set; } = "";
        [Display(Name ="Warehouse Name")]
        [Required(ErrorMessage ="{0} harus diisi"),StringLength(150)]
        public string HouseName { get; set; } = "";

        [Required(ErrorMessage ="{0} harus diisi"),StringLength(200)]
        public string Address { get; set; } = "";
        [StringLength(20)]
        public string? KelId { get; set; }
        [StringLength(50)]
        public string? KodePos { get; set; }
        [StringLength(10)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? OfficePhone { get; set; }
        [StringLength(25)]
        public string? Fax { get; set; }
        [StringLength(25)]
        public string? Latitude { get; set; }
        [StringLength(50)]
        public string? Longitude { get; set; }
        [StringLength(50)]
        public string? BuildStatus { get; set; }
        [StringLength(50)]
        public string? BuildSize { get; set; }
        [StringLength(50),Display(Name ="Access Area")]
        public string? AccessArea { get; set; }
        [StringLength(50),Display(Name ="Manager")]
        public string? HouseManager { get; set; }
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        [ForeignKey("KelId")]
        public MasKelurahan? MasKelurahan { get; set; }
        public ICollection<InvStorageRow>? InvStorageRows { get; set; }
        public ICollection<MasDataTenantWarehouse>? MasDataTenantWarehouses { get; set; }
        public ICollection<OutSalesOrder>? OutSalesOrders { get; set; }
        public ICollection<SecUserWarehouse>? SecUserWarehouses { get; set; }
    }
}
