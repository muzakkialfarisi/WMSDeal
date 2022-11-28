using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WMS.Models
{
    public class MasCustomerType
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustTypeId { get; set; }  
        [Required,StringLength(20)]
        [Display(Name ="Customer Type Code")]
        public string CustTypeCode { get; set; } = "";
        [Required,StringLength(150)]
        [Display(Name ="Customer Type Name")]
        public string CustTypeName { get; set; } = "";
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<MasCustomerData>? MasCustomerDatas { get; set; }
    }
}
