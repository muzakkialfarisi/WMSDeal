using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesDispatchtoCourier
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string OrderId { get; set; }
        [ForeignKey("OrderId")]
        public OutSalesOrder? OutSalesOrder { get; set; }
        public int OrdCourier { get; set; }
        [ForeignKey("OrdCourier")]
        public MasSalesCourier? MasSalesCourier { get; set; }
        [StringLength(150)]
        public string CourierName { get; set; } = "";
        [StringLength(150)]
        public string PhotoHandOver { get; set; } = "";
        [NotMapped]
        public IFormFile? FormPhotoHandOver { get; set; }
        public DateTime DatedHandOvered { get; set; }
        [StringLength(100)]
        public string HandoveredBy { get; set; } = "";
        public int Flag { get; set; }
    }
}
