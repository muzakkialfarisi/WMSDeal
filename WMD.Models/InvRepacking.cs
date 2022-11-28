using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvRepacking
    {
        [Key]
        public int Id { get; set; }

        public int DOProductId { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }

        public int Quantity { get; set; }

        public DateTime DateRepacked { get; set; }

        public string RepackedBy { get; set; } = "";
    }
}
