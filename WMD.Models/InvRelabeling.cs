using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvRelabeling
    {
        [Key]
        public int Id { get; set; }

        public int DOProductId { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }

        public int Quantity { get; set; }

        public DateTime DateRelabelled { get; set; }

        public string RelabelledBy { get; set; } = "";
    }
}
