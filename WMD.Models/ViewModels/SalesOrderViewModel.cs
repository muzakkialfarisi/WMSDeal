using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WMS.Models.ViewModels
{
    public class SalesOrderCreateViewModel
    {
        public OutSalesOrder? outSalesOrder { get; set; }
        public IEnumerable<MasProductData>? masProductDatas { get; set; }
        public IEnumerable<MasProductBundling>? masProductBundlings { get; set; }
    }

    public class SalesOrderAssignViewModel
    {
        public Guid UserId { get; set; }

        public Guid PickAssignId { get; set; }
        public string? OrderId { get; set; }

        public string ImageStaged { get; set; }
    }

    public class SalesOrderPickViewModel
    {
        public int OrdProductId { get; set; }
        public int ProductId { get; set; }
        public string? IKU { get; set; }
        public int? Id { get; set; }
        public Guid StorageCode { get; set; }
        public int DOProductId { get; set; }
        public int QtyPick { get; set; }
        public int QtyStock { get; set; }
    }
}
