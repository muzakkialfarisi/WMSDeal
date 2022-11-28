using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesOrderCustomer
    {
        [Key]
        public string OrderId { get; set; } = "";
        [ForeignKey("OrderId")]
        public OutSalesOrder? OutSalesOrder { get; set; }

        [StringLength(150)]
        public string CustName { get; set; } = "";

        [StringLength(25)]
        public string CustPhone { get; set; } = "";

        [StringLength(300)]
        public string CustAddress { get; set; } = "";

        [StringLength(20)]
        public string? KelId { get; set; }
        [ForeignKey("KelId")]
        public MasKelurahan? MasKelurahan { get; set; }

        [StringLength(150)]
        public string CustCity { get; set; } = "";

        [StringLength(20)]
        public string CustZipCode { get; set; } = "";

        public string? KTP { get; set; } = "";

    }
}