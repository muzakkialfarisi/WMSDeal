using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WMS.Models
{
    public class MasKelurahan
    {
        [Key,Required,StringLength(20)]
        public string KelId { get; set; } = "";
        [Required,StringLength(150)]
        public string KelName { get; set; } = "";
        [Required,StringLength(20)]
        public string KecId { get; set; } = "";
        [ForeignKey("KecId")]
        public MasKecamatan? MasKecamatan { get; set; }
        public ICollection<MasSupplierData>? Suppliers { get; set; }
        public ICollection<MasHouseCode>? HouseCodes { get; set; }
        public ICollection<OutSalesOrderCustomer>? OutSalesOrderCustomers { get; set; }
    }
}
