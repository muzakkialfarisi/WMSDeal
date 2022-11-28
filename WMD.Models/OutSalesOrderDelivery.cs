using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutsalesOrderDelivery
    {
        [Key]
        [StringLength(100)]
        [Required]
        public string OrderId { get; set; } = string.Empty;
        [ForeignKey("OrderId")]
        public OutSalesOrder? OutSalesOrder { get; set; }

        public int OrdCourier { get; set; }
        [ForeignKey("OrdCourier")]
        public MasSalesCourier? MasSalesCourier { get; set; }

        [StringLength(100)]
        public string OrdCourierService { get; set; } = string.Empty;

        [StringLength(100)]
        public string AirwayBill  { get; set; } = string.Empty;

        public decimal ShippingCost { get; set; }

        public string Status { get; set; } = "Delivery Order";

        public decimal OrdTax { get; set; }

        public float GrandWeight { get; set; }
    }
}
