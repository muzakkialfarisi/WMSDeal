using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesOrderProduct
    {
        [Key]
        public int OrdProductId { get; set; }
        [StringLength(100)]
        public string? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OutSalesOrder? OutSalesOrder { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public MasProductData? MasProductData { get; set; }

        public float TotalWeight { get; set; } = 0;

        public int Quantity { get; set; } = 1;

        public float UnitPrice { get; set; } = 0;

        public float SubTotal { get; set; } = 0;

        public float Discount { get; set; } = 0;

        // 0 canc
        // 1 open
        // 2 booked
        // 3 picked
        // 4 pack
        public int Flag { get; set; } = 1;

        public virtual List<OutSalesOrderStorage>? OutSalesOrderStorages { get; set; } = new List<OutSalesOrderStorage>();
        public virtual List<IncSerialNumber>? IncSerialNumbers { get; set; } = new List<IncSerialNumber>();
        public virtual OutSalesOrderPack? OutSalesOrderPack { get; set; }

    }
}
