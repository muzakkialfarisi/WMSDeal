using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class InvStorageBesaran
    {
        [Key]
        [StringLength(20)]
        public String Code { get; set; } = "";
        [Required]
        [StringLength(50)]
        public String Name { get; set; } = "";
        public int MaxPanjang { get; set; }
        public int MaxLebar { get; set; }
        public ICollection<InvStorageSize>? Sizes { get; set; }
    }
}
