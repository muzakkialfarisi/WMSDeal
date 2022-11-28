using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WMS.Models
{
    public class MasProvinsi
    {
        [Key,Required,StringLength(20)]
        public string ProId { get; set; } = "";
        [Required,StringLength(150)]
        public string ProName { get; set; } = "";
        [Required]
        public int RegionId { get; set; }

        [ForeignKey("RegionId")]
        public MasRegional? MasRegional { get; set; }

        public  ICollection<MasKabupaten>?MasKabupatens { get; set; }
    }
}
