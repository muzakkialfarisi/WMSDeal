using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WMS.Models
{
    public class MasKecamatan
    {
        [Key,Required,StringLength(20)]
        public string KecId { get; set; } = "";
        [Required,StringLength(150)]
        public string KecName { get; set; } = "";
        [Required,StringLength(20)]
        public string KabId { get; set; } = "";
        [ForeignKey("KabId")]
        public MasKabupaten? MasKabupaten { get; set; }
        public ICollection<MasKelurahan>? MasKelurahans { get; set; }
    }
}
