using System.ComponentModel.DataAnnotations;
namespace WMS.Models
{
    public class MasIndustry
    {
        [Key]
        public int ID { get; set; }
        [Required,StringLength(20)]
        [Display(Name ="Industry Code")]
        public string IndustryCode { get; set; } = "";
        [Required,StringLength(150)]
        [Display(Name ="Industry Name")]
        public string IndustryName { get; set; } = "";
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<MasCustomerData>? MasCustomerDatas { get; set; }
        public ICollection<MasSupplierData>? MasSupplierDatas { get; set; }
    }
}
