using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WMS.Models
{
    public class MasKabupaten
    {
        [Key,Required,StringLength(20)]
        public string KabId { get; set; } = "";
        [Required,StringLength(150)]
        public string KabName { get; set; } = "";
        [Required,StringLength(20)]
        public string ProId { get; set; } = "";
        [ForeignKey("ProId")]
        public MasProvinsi? MasProvinsi { get; set; } 

        public ICollection<MasKecamatan>? MasKecamatans { get; set; }
    }
}
