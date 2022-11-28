using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesOrderStorage
    {
        [Key]
        public int Id { get; set; }

        public int OrdProductId { get; set; }
        [ForeignKey("OrdProductId")]
        public OutSalesOrderProduct? OutSalesOrderProduct { get; set; }

        public string? IKU { get; set; }
        [ForeignKey("IKU")]
        public IncItemProduct? IncItemProduct { get; set; }

        public int? IdPutAway { get; set; }
        [ForeignKey("IdPutAway")]
        public InvProductPutaway? InvProductPutaway { get; set; }

        public Guid? StorageCode { get; set; }
        [ForeignKey("StorageCode")]
        public InvStorageCode? InvStorageCode { get; set; }

        public int Sequence { get; set; } = 0;

        public int QtyPick { get; set; }
        [StringLength(30)]
        public string PickedStatus { get; set; } = "Ordered";

        public DateTime? DatePicked { get; set; }
        [StringLength(50)]
        public string PickedBy { get; set; } = "";
        [StringLength(100)]
        public string QualityCheckedRemark { get; set; } = "";
        [StringLength(30)]
        public string QualityCheckedStatus { get; set; } = "";

        public DateTime? DateQualityChecked { get; set; }
        [StringLength(50)]
        public string QualityCheckedBy { get; set; } = "";
    }
}
