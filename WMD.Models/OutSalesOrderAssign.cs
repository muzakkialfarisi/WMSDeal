using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesOrderAssign
    {
        [Key]
        public string OrderId { get; set; } = "";
        [ForeignKey("OrderId")]
        public OutSalesOrder? OutSalesOrder { get; set; }

        public Guid? PickAssignId { get; set; }

        public Guid? UserId { get; set; }
        [ForeignKey("UserId")]
        public SecUser? SecUser { get; set; }

        public DateTime DateAssigned { get; set; }

        public DateTime DateStaged { get; set; }

        // 1 default
        // 2 done piked
        public int Flag { get; set; }

        public string? ImageStaged { get; set; }
        [NotMapped]
        public IFormFile? FormImageStaged { get; set; }
    }
}
