using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Incoming
{
    public class ProductData
    {
        [Key]
        public int DOProductId { get; set; }
        public string DONumber { get; set; }
        [ForeignKey("DONumber")]
        public DeliveryOrder DeliveryOrder { get; set; }
        public string Tenant { get; set; }
        public string DOProductCode { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; }
        public DateTime DateExpired { get; set; }
        public int QtyArrival { get; set; }
        public string ClosedNote { get; set; }
        public string ProductName { get; set; }
        public string ProductLevel { get; set; }
        public string ProductCondition { get; set; }
        public string SKU { get; set; }
        public string Unit { get; set; }
        public float ActualWeight { get; set; }
        public string BeautyPicture { get; set; }
        public string SizeCode { get; set; }
        public string ZoneCode { get; set; }
        public string SerialNumber { get; set; }
        public int Remaining { get; set; }
        public bool isArrival { get; set; }
        public Color Color { get; set; } = Color.FromHex("FFFFFF");
        public Color StatusColor { get; set; } = Color.FromHex("#ff0000");
        public virtual List<ItemProduct> ItemProduct { get; set; } = new List<ItemProduct>();
    }
}
