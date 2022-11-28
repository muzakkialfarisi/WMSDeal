using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvStorageSize
    {
        [Key]
        [StringLength(20)]
        [Required]
        public string SizeCode { get; set; } = "";
        [Required]
        [StringLength(100)]
        [Display(Name ="Size Name")]
        public string SizeName { get; set; } = "";
        [Required]
        [StringLength(20)]
        [Display(Name = "Wide Size")]
        public string BesaranCode { get; set; } = "";
        [ForeignKey("BesaranCode")]
        public InvStorageBesaran? InvStorageBesaran { get; set; }
        [Required]
        [StringLength(20)]
        [Display(Name ="Thick Size")]
        public string TebalCode { get; set; } = "";
        [ForeignKey("TebalCode")]
        public InvStorageTebal? InvStorageTebal { get; set; }
        public FlagEnum Flag { get; set; }
        public ICollection<InvStorageCode>? InvStorageCodes { get; set;}
        public ICollection<MasProductData>? MasProductDatas { get; set;}
    }
}
