using System.ComponentModel.DataAnnotations;

namespace WMS.Models
{
    public class MasPackingType
    {
        [Key]
        [StringLength(20)]
        [Required]
        public string PackTypeId { get; set; } = "";
        [StringLength(100)]
        public string? PackTypeName { get; set; }  
       public ICollection<OutSalesOrderPack> OutSalesOrderPacks { get; set; }
    }
}
