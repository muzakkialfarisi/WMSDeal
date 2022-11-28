using System.ComponentModel.DataAnnotations;
namespace WMS.Models
{
    public class MasProductPackaging
    {
        [Key]
        public int PackagingId { get; set; }
        [Required, StringLength(20)]
        [Display(Name ="Packaging Code")]
        public string PackagingCode { get; set; } = "";
        [Required, StringLength(150)]
        [Display(Name ="Packaging Name")]
        public string PackagingName { get; set; } = "";
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<MasProductData>? MasProductDatas { get; set; }
    }
}
