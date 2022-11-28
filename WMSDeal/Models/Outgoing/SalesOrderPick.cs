using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSDeal.Models.Outgoing
{
    public class SalesOrderPick
    {
        [Key]
        public int Id { get; set; }
        public string PickAssignId { get; set; }
        public string OrderId { get; set; }
        public string CneeName { get; set; }
        public string TenantName { get; set; }
        public string StoreName { get; set; }
        public DateTime DateOrdered { get; set; }
        public string ProductId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string BeautyPicture { get; set; }
        public string ActualWeight { get; set; }
        public string StorageMethod { get; set; }
        public string ProductCondition { get; set; }
        public string SerialNumber { get; set; }
        public string Unit { get; set; }
        public string StorageCode { get; set; }
        public string BinCode { get; set; }
        public string BinName { get; set; }
        public string Sequence { get; set; }
        public int QtyPick { get; set; }
        public string PickedStatus { get; set; }
        public Color StatusColor { get; set; } = Color.FromHex("#000000");

    }

    public class PickOrder
    {
        [Key]
        public int Id { get; set; }
        public Guid StorageCode { get; set; }
    }

    public class Staging
    {
        [Key]
        public string UserId { get; set; }
        public string PickAssignId { get; set; }
    }
}
