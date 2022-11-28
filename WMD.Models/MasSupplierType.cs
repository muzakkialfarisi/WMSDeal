using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasSupplierType
    {
        [Key]
        public int TypeId { get; set; }
        [Required,StringLength(20), Display(Name ="Supplier Type Code")]
        public string SupplierTypeCode { get; set; } = "";
        [Required,StringLength(150), Display(Name ="Supplier Type Name")]
        public string SupplierTypeName { get; set; } = "";
        [Display(Name = "Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<MasSupplierData>? MasSupplierData { get; set; }
    }
}
