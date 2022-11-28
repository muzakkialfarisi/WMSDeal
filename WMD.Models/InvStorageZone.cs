using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvStorageZone
    {
        [Key]
        [Required, StringLength(20,MinimumLength =1), Display(Name = "Zone Code")]
        [MaxLength(5,ErrorMessage ="Zone Code cannot be longer than 5 characters and less than 1 character")]
        public string ZoneCode { get; set; } = "";
        [Required, StringLength(150),Display(Name ="Zone Name")]
        public string ZoneName { get; set; } = "";
        [StringLength(150,ErrorMessage ="Zone Name cannot be longer than 150 characters"),Display(Name ="Zone Plan Photo")] 
        public string? ZonePlanPhoto { get; set; }  
        [NotMapped]
        public IFormFile? FormZonePlanPhoto { get; set; }
        [Display(Name ="Status")]
        public FlagEnum Flag { get; set; }
        public ICollection<InvStorageRow>? InvStorageRows { get; set; }
        public ICollection<MasProductData>? MasProductDatas { get; set; }
    }
}
