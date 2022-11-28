using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasSalesType
    {
        [Key]
        public int StyId { get; set; }
        [StringLength(150)]
        public string StyName { get; set; } = "";
        [StringLength(25)]
        public string StyCode { get; set; } = "";
    }
}
