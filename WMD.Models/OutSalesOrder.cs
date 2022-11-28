using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS.Models
{
    public class OutSalesOrder
    {
        [Key]
        [StringLength(100)]
        [Required]
        public string OrderId { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        [ForeignKey("TenantId")]
        public MasDataTenant? MasDataTenant { get; set; }

        [StringLength(25)]
        public string? HouseCode { get; set; }
        [ForeignKey("HouseCode")]
        public MasHouseCode? MasHouseCode { get; set; }

        public int PlatformId { get; set; }
        [ForeignKey("PlatformId")]
        public MasPlatform? MasPlatform { get; set; }

        public int? StoreId { get; set; }
        [ForeignKey("StoreId")]
        public MasStore? MasStore { get; set; }

        public string StoreName { get; set; } = string.Empty;

        public int OrdSalesType { get; set; }
        [ForeignKey("OrdSalesType")]
        public MasSalesType? MasSalesType { get; set; }

        public DateTime DateOrdered { get; set; } = DateTime.Now;

        public string OrderBy { get; set; } = "";

        public int FlagApi { get; set; } = 1;

        //0 cancel
        //1 open
        //2 ordered
        //3 picked
        //4 staged
        //5 packed
        //6 dispatch
        public int Status { get; set; } = 1;

        //0 default
        //1 generated signed
        public int FlagPick { get; set; } = 0;

        public virtual List<OutSalesOrderProduct>? OutSalesOrderProducts { get; set; } = new List<OutSalesOrderProduct>();
        public virtual List<OutSalesDispatchtoCourier>? OutSalesDispatchtoCouriers { get; set; }


        public virtual OutSalesOrderCustomer? OutSalesOrderCustomer { get; set; }
        public virtual OutSalesOrderConsignee? OutSalesOrderConsignee { get; set; }
        public virtual OutsalesOrderDelivery? OutsalesOrderDelivery { get; set; }
        public virtual OutSalesOrderAssign? OutSalesOrderAssign { get; set; }
    }
}
