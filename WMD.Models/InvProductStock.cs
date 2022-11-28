using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class InvProductStock
    {
        [Key]
        [StringLength(150)]
        public string Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData MasProductData { get; set; }
        [StringLength(25)]
        public string HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode MasHouseCode { get; set; }
        public int Stock { get; set; }

        public int QtyOrder { get; set; } = 0;

    }
}
