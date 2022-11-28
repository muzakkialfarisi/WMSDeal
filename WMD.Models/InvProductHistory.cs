using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public enum ProductHistoryType
    {
        In=1,
        Out=2,
        Op=3
    }

    public class InvProductHistory
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData MasProductData { get; set; }
        [StringLength(25)]
        public string HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode MasHouseCode { get; set; }
        public ProductHistoryType HistoryType { get; set; }
        [StringLength(25)]
        public string TrxNo { get; set; } = "";
        [StringLength(100)]
        public string Interest { get; set; } = "";
        public int Quantity { get; set; }
        [StringLength(150)]
        public string Note { get; set; } = "";
        public int Stock { get; set; }
        public DateTime DatedTime { get; set; }
        [StringLength(50)]
        public string UserBy { get; set; } = "";
        public int Flag { get; set; } = 1;
    }
}
