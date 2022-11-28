using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMS.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserManager UserManager { get; }
        IMenuRepository Menu { get; }
        IUserRepository User { get; }
        IProfileRepository Profile { get; }
        IUserTenantRepository UserTenant { get; }
        IUserWarehouseRepository UserWarehouse { get; }

        IProvinsiRepository Provinsi { get; }
        IKabupatenRepository Kabupaten { get; }
        IKecamatanRepository Kecamatan { get; }
        IKelurahanRepository Kelurahan { get; }
        ICustomerDataRepository CustomerData { get; }
        IJabatanRepository Jabatan { get; }
        IHouseCodeRepository HouseCode { get; }
        ITenantRepository Tenant { get; }
        ITenantWarehouseRepository TenantWarehouse { get; }
        ITenantDivisionRepository TenantDivision { get; }
        IProductRepository Product { get; }
        IProductBundlingRepository ProductBundling { get; }
        IProductBundlingDataRepository ProductBundlingData { get; }
        IProductPackagingRepository ProductPackaging { get; }
        IProductUnitRepository ProductUnit { get; }
        IProductPriorityRepository ProductPriority { get;  }
        IProductTypeOfRepackRepository ProductTypeOfRepack { get;  }
        ISupplierRepository Supplier { get; }
        IStoreRepository Store { get; }
        IInvoiceRepository Invoice { get; }
        IInvoiceDetailRepository InvoiceDetail { get; }
        IPricingRepository Pricing { get; }

        IStorageZoneRepository StorageZone { get; }
        IStorageRowRepository StorageRow { get; }
        IStorageSectionRepository StorageSection { get; }
        IStorageColumnRepository StorageColumn { get; }
        IStorageLevelRepository StorageLevel { get; }
        IStorageTebalRepository StorageTebal { get; }
        IStorageBesaranRepository StorageBesaran { get; }
        IStorageSizeRepository StorageSize { get; }
        IStorageCodeRepository StorageCode { get; }
        IStorageCategoryRepository StorageCategory { get; }
        IStorageBinRepository StorageBin { get; }

        IRoutePickRepository RoutePick { get; }
        IRoutePickColumnRepository RoutePickColumn { get; }

        IPurchaseRequestRepository PurchaseRequest { get; }
        IPurchaseRequestProductRepository PurchaseRequestProduct { get; }
        IPurchaseOrderRepository PurchaseOrder { get; }
        IPurchaseOrderProductRepository PurchaseOrderProduct { get; }
        IDeliveryOrderCourierRepository DeliveryOrderCourier { get; }
        IDeliveryOrderRepository DeliveryOrder { get; }
        IDeliveryOrderProductRepository DeliveryOrderProduct { get; }
        IItemProductRepository ItemProduct { get; }
        IDeliveryOrderArrivalRepository DeliveryOrderArrival { get; }
        IDeliveryOrderArrivalProductRepository DeliveryOrderArrivalProduct { get; }
        IPutAwayRepository PutAway { get; }

        ISerialNumberRepository SerialNumber { get; }

        IStockOpnameRepository StockOpname { get; }
        IStockOpnameProductRepository StockOpnameProduct { get; }

        IRepackRepository Repack { get; }

        ISalesOrderCourierRepository SalesOrderCourier { get; }
        ISalesOrderRepository SalesOrder { get; }
        ISalesOrderProductRepository SalesOrderProduct { get; }
        ISalesOrderPackRepository SalesOrderPack { get; }
        ISalesOrderDeliveryRepository SalesOrderDelivery { get; }
        ISalesOrderCustomerRepository SalesOrderCustomer { get; }
        ISalesOrderConsigneeRepository SalesOrderConsignee { get; }
        ISalesOrderStorageRepository SalesOrderStorage { get; }
        ISalesOrderAssignRepository SalesOrderAssign { get; }
        ISalesOrderDispatchRepository SalesOrderDispatch { get; }

        IProductStockRepository ProductStock { get; }
        IProductHistoryRepository ProductHistory { get; }

        IReturnRepository Return { get; }
        IReturnProductRepository ReturnProduct { get; }

        IMobileAppVersionRepository MobileAppVersion {get; }
        IEmailRepository Email { get; }

        void Save();
        Task SaveAsync();
    }
}
