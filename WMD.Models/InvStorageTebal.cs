using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class InvStorageTebal
    {
        [Key]
        [StringLength(20)]
        public string Code { get; set; } = "";
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";
        public int MaxTinggi { get; set; }
        public ICollection<InvStorageSize>? Sizes { get; set; }
    }
}
