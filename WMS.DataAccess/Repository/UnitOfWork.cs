using Microsoft.Extensions.Configuration;
using WMS.DataAccess;
using WMS.DataAccess.Repository;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;

namespace Core.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public UnitOfWork(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
            UserManager = new UserManager(_config);

            Menu = new MenuRepository(_db);
            User = new UserRepository(_db);
            Profile = new ProfileRepository(_db);
            Jabatan = new JabatanRepository(_db);
            UserWarehouse = new UserWarehouseRepository(_db);
            UserTenant = new UserTenantRepository(_db);

            Provinsi = new ProvinsiRepository(_db);
            Kabupaten = new KabupatenRepository(_db);
            Kecamatan = new KecamatanRepository(_db);
            Kelurahan = new KelurahanRepository(_db);
            CustomerData = new CustomerDataRepository(_db);
            HouseCode = new HouseCodeRepository(_db);
            Tenant = new TenantRepository(_db);
            TenantWarehouse = new TenantWarehouseRepository(_db);
            TenantDivision = new TenantDivisionRepository(_db);
            Product = new ProductRepository(_db);
            ProductBundling = new ProductBundlingRepository(_db);
            ProductBundlingData = new ProductBundlingDataRepository(_db);
            ProductPackaging = new ProductPackagingRepository(_db);
            ProductUnit = new ProductUnitRepository(_db);
            ProductPriority = new ProductPriorityRepository(_db);
            ProductTypeOfRepack = new ProductTypeOfRepackRepository(_db);
            Supplier = new SupplierRepository(_db);
            Store = new StoreRepository(_db);
            Invoice = new InvoiceRepository(_db);
            InvoiceDetail = new InvoiceDetailRepository(_db);
            Pricing = new PricingRepository(_db);

            StorageZone = new StorageZoneRepository(_db);
            StorageRow = new StorageRowRepository(_db);
            StorageSection = new StorageSectionRepository(_db);
            StorageColumn = new StorageColumnRepository(_db);
            StorageLevel = new StorageLevelRepository(_db);
            StorageTebal = new StorageTebalRepository(_db);
            StorageBesaran = new StorageBesaranRepository(_db);
            StorageSize = new StorageSizeRepository(_db);
            StorageCode = new StorageCodeRepository(_db);
            StorageCategory = new StorageCategoryRepository(_db);
            StorageBin = new StorageBinRepository(_db);

            RoutePick = new RoutePickRepository(_db);
            RoutePickColumn = new RoutePickColumnRepository(_db);

            PurchaseRequest = new PurchaseRequestRepository(_db);
            PurchaseRequestProduct = new PurchaseRequestProductRepository(_db);
            PurchaseOrder = new PurchaseOrderRepository(_db);
            PurchaseOrderProduct = new PurchaseOrderProductRepository(_db);
            DeliveryOrderCourier = new DeliveryOrderCourierRepository(_db);
            DeliveryOrder = new DeliveryOrderRepository(_db);
            DeliveryOrderProduct = new DeliveryOrderProductRepository(_db);
            ItemProduct = new ItemProductRepository(_db);
            DeliveryOrderArrival = new DeliveryOrderArrivalRepository(_db);
            DeliveryOrderArrivalProduct = new DeliveryOrderArrivalProductRepository(_db);
            PutAway = new PutAwayRepository(_db);

            SerialNumber = new SerialNumberRepository(_db);

            StockOpname = new StockOpnameRepository(_db);
            StockOpnameProduct = new StockOpnameProductRepository(_db);

            Repack = new RepackRepository(_db);

            SalesOrderCourier = new SalesOrderCourierRepository(_db);
            SalesOrder = new SalesOrderRepository(_db);
            SalesOrderProduct = new SalesOrderProductRepository(_db);
            SalesOrderPack = new SalesOrderPackRepository(_db);
            SalesOrderDelivery = new SalesOrderDeliveryRepository(_db);
            SalesOrderCustomer = new SalesOrderCustomerRepository(_db);
            SalesOrderConsignee = new SalesOrderConsigneeRepository(_db);
            SalesOrderStorage = new SalesOrderStorageRepository(_db);
            SalesOrderAssign = new SalesOrderAssignRepository(_db);
            SalesOrderDispatch = new SalesOrderDispatchRepository(_db);

            ProductStock = new ProductStockRepository(_db);
            ProductHistory = new ProductHistoryRepository(_db);

            Return = new ReturnRepository(_db);
            ReturnProduct = new ReturnProductRepository(_db);

            MobileAppVersion = new MobileAppVersionRepository(_db);
            Email = new EmailRepository(_config);
        }

        public IUserManager UserManager { get; private set; }

        public IMenuRepository Menu {get;private set; }
        public IUserRepository User { get; private set; }
        public IProfileRepository Profile { get; private set; }
        public IJabatanRepository Jabatan { get; private set; }
        public IUserWarehouseRepository UserWarehouse { get; private set; }
        public IUserTenantRepository UserTenant { get; private set; }

        public IProvinsiRepository Provinsi { get; private set; }
        public IKabupatenRepository Kabupaten { get; private set; }
        public IKecamatanRepository Kecamatan { get; private set; }
        public IKelurahanRepository Kelurahan { get; private set; }
        public ICustomerDataRepository CustomerData { get; private set; }
        public IHouseCodeRepository HouseCode { get; private set; }
        public ITenantRepository Tenant { get; private set; }
        public ITenantWarehouseRepository TenantWarehouse { get; private set; }
        public ITenantDivisionRepository TenantDivision { get; private set; }
        public IProductRepository Product { get; private set; }
        public IProductBundlingRepository ProductBundling { get; private set; }
        public IProductBundlingDataRepository ProductBundlingData { get; private set; }
        public IProductPackagingRepository ProductPackaging { get; private set; }
        public IProductUnitRepository ProductUnit { get; private set; }
        public IProductPriorityRepository ProductPriority { get; private set; }
        public IProductTypeOfRepackRepository ProductTypeOfRepack { get; private set; }
        public ISupplierRepository Supplier { get; private set; }
        public IStoreRepository Store { get; private set; }
        public IInvoiceRepository Invoice { get; private set; }
        public IInvoiceDetailRepository InvoiceDetail { get; private set; }
        public IPricingRepository Pricing { get; private set; }

        public IStorageZoneRepository StorageZone { get; private set; }
        public IStorageRowRepository StorageRow { get; private set; }
        public IStorageSectionRepository StorageSection { get; private set; }
        public IStorageColumnRepository StorageColumn { get; private set; }
        public IStorageLevelRepository StorageLevel { get; private set; }
        public IStorageTebalRepository StorageTebal { get; private set; }
        public IStorageBesaranRepository StorageBesaran { get; private set; }
        public IStorageSizeRepository StorageSize { get; private set; }
        public IStorageCodeRepository StorageCode { get; private set; }
        public IStorageCategoryRepository StorageCategory { get; private set; }
        public IStorageBinRepository StorageBin { get; private set; }

        public IRoutePickRepository RoutePick { get; private set; }
        public IRoutePickColumnRepository RoutePickColumn { get; private set; }

        public IPurchaseRequestRepository PurchaseRequest { get; private set; }
        public IPurchaseRequestProductRepository PurchaseRequestProduct { get; private set; }
        public IPurchaseOrderRepository PurchaseOrder { get; private set; }
        public IPurchaseOrderProductRepository PurchaseOrderProduct { get; private set; }
        public IDeliveryOrderCourierRepository DeliveryOrderCourier { get; private set; }
        public IDeliveryOrderRepository DeliveryOrder { get; private set; }
        public IDeliveryOrderProductRepository DeliveryOrderProduct { get; private set; }
        public IItemProductRepository ItemProduct { get; private set; }
        public IDeliveryOrderArrivalRepository DeliveryOrderArrival { get; private set; }
        public IDeliveryOrderArrivalProductRepository DeliveryOrderArrivalProduct { get; private set; }
        public IPutAwayRepository PutAway { get; private set; }
        public ISerialNumberRepository SerialNumber { get; private set; }

        public IStockOpnameRepository StockOpname { get; private set; }
        public IStockOpnameProductRepository StockOpnameProduct { get; private set; }

        public IRepackRepository Repack { get; private set; }

        public ISalesOrderCourierRepository SalesOrderCourier { get; private set; }
        public ISalesOrderRepository SalesOrder { get; private set; }
        public ISalesOrderProductRepository SalesOrderProduct { get; private set; }
        public ISalesOrderPackRepository SalesOrderPack { get; private set; }
        public ISalesOrderDeliveryRepository SalesOrderDelivery { get; private set; }
        public ISalesOrderCustomerRepository SalesOrderCustomer { get; private set; }
        public ISalesOrderConsigneeRepository SalesOrderConsignee { get; private set; }
        public ISalesOrderStorageRepository SalesOrderStorage { get; private set; }
        public ISalesOrderAssignRepository SalesOrderAssign { get; private set; }
        public ISalesOrderDispatchRepository SalesOrderDispatch { get; private set; }

        public IProductStockRepository ProductStock { get; private set; }
        public IProductHistoryRepository ProductHistory { get; private set; }

        public IReturnRepository Return { get; private set; }
        public IReturnProductRepository ReturnProduct { get; private set; }

        public IMobileAppVersionRepository MobileAppVersion { get; private set; }
        public IEmailRepository Email { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
