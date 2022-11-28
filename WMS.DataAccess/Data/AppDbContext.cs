using Microsoft.EntityFrameworkCore;
using WMS.Models;

namespace WMS.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<SecProfile> SecProfiles { get; set; }
        public DbSet<SecMenu> SecMenus { get; set; }
        public DbSet<SecAuditTrail> SecAuditTrails { get; set; }
        public DbSet<SecProfileMenu> SecProfileMenus { get; set; }
        public DbSet<SecUser> SecUsers { get; set; }
        public DbSet<SecUserTenant> SecUserTenants { get; set; }
        public DbSet<SecUserWarehouse> SecUserWarehouses { get; set; }

        public DbSet<MasJabatan> MasJabatans { get; set; }
        public DbSet<MasHouseCode> MasHouseCodes { get; set; }
        public DbSet<MasRegional> MasRegionals { get; set; }
        public DbSet<MasIndustry> MasIndustries { get; set; }
        public DbSet<MasCheckPoint> MasCheckPoints { get; set; }
        public DbSet<MasDivision> MasDivisions { get; set; }
        public DbSet<MasDirectorate> MasDirectorates { get; set; }
        public DbSet<MasSupplierData> MasSupplierData { get; set; }
        public DbSet<MasSupplierService> MasSupplierServices { get; set; }
        public DbSet<MasSupplierType> MasSupplierTypes { get; set; }
        public DbSet<MasProductTypeOfRepack> MasProductTypeOfRepacks { get; set; }
        public DbSet<MasProductPriority> MasProductPriorities { get; set; }
        public DbSet<MasProductPackaging> MasProductPackagings { get; set; }
        public DbSet<MasProductData> MasProductDatas { get; set; }
        public DbSet<MasProductBundling> MasProductBundling { get; set; }
        public DbSet<MasProductBundlingData> MasProductBundlingData { get; set; }
        public DbSet<MasService> MasServices { get; set; }
        public DbSet<MasCustomerData> MasCustomerDatas { get; set; }
        public DbSet<MasCustomerType> MasCustomerTypes { get; set; }
        public DbSet<MasPricing> MasPricings { get; set; }
        public DbSet<MasPricingAdditional> MasPricingAdditionals { get; set; }
        public DbSet<MasInvoicing> MasInvoicings { get; set; }
        public DbSet<MasInvoicingDetail> MasInvoicingDetails { get; set; }
        public DbSet<MasServiceCategory> MasServiceCategories { get; set; }
        public DbSet<MasKelurahan> MasKelurahans { get; set; }
        public DbSet<MasKecamatan> MasKecamatans { get; set; }
        public DbSet<MasKabupaten> MasKabupatens { get; set; }
        public DbSet<MasProvinsi> MasProvinsis { get; set; }
        public DbSet<MasDataTenant> MasDataTenantes { get; set; }
        public DbSet<MasUnit> MasUnits { get; set; }
        public DbSet<MasBrand> MasBrands { get; set; }
        public DbSet<MasDataTenantDivision> MasDataTenantDivisions { get; set; }
        public DbSet<MasDataTenantWarehouse> MasDataTenantWarehouses { get; set; }
        public DbSet<MasSalesType> MasSalesTypes { get; set; }
        public DbSet<MasSalesCourier> MasSalesCouriers { get; set; }
        public DbSet<MasPackingType> MasPackingType { get; set; }
        public DbSet<MasDeliveryOrderCourier> MasDeliveryOrderCouriers { get; set; }
        public DbSet<MasPlatform> MasPlatforms { get; set; }
        public DbSet<MasStore> MasStores { get; set; }

        public DbSet<InvStorageZone> InvStorageZones { get; set; }
        public DbSet<InvStorageCategory> InvStorageCategories { get; set; }
        public DbSet<InvStorageBesaran> InvStorageBesarans { get; set; }
        public DbSet<InvStorageTebal> InvStorageTebals { get; set; }
        public DbSet<InvStorageSize> InvStorageSizes { get; set; }
        public DbSet<InvStorageRow> InvStorageRows { get; set; }
        public DbSet<InvStorageSection> InvStorageSections { get; set; }
        public DbSet<InvStorageColumn> InvStorageColumns { get; set; }
        public DbSet<InvStorageLevel> InvStorageLevels { get; set; }
        public DbSet<InvStorageBin> InvStorageBins { get; set; }
        public DbSet<InvStorageCode> InvStorageCodes { get; set; }
        public DbSet<InvProductStock> InvProductStocks { get; set; }
        public DbSet<InvProductHistory> InvProductHistorys { get; set; }
        public DbSet<InvPickingRoute> InvPickingRoutes { get; set; }
        public DbSet<InvPickingRouteColumn> InvPickingRouteColumns { get; set; }
        public DbSet<InvSalesOrderPick> InvSalesOrderPicks { get; set; }
        public DbSet<InvRepacking> InvRepackings { get; set; }
        public DbSet<InvRelabeling> InvRelabelings { get; set; }

        public DbSet<IncRequestPurchase> IncRequestPurchases { get; set; }
        public DbSet<IncRequestPurchaseProduct> IncRequestPurchaseProducts { get; set; }
        public DbSet<IncPurchaseOrder> IncPurchaseOrders { get; set; }
        public DbSet<IncPurchaseOrderProduct> IncPurchaseOrdersProducts { get; set; }
        public DbSet<IncDeliveryOrder> IncDeliveryOrders { get; set; }
        public DbSet<IncDeliveryOrderProduct> IncDeliveryOrderProducts { get; set; }
        public DbSet<IncDeliveryOrderArrival> IncDeliveryOrderArrivals { get; set; }
        public DbSet<IncDeliveryOrderArrivalProduct> IncDeliveryOrderArrivalProducts { get; set; }
        public DbSet<IncQualityCheck> IncQualityChecks { get; set; }
        public DbSet<IncItemProduct> IncItemProducts { get; set; }
        public DbSet<InvProductPutaway> InvProductPutaways { get; set; }

        public DbSet<IncSerialNumber> IncSerialNumbers { get; set; }

        public DbSet<InvStockOpname> InvStockOpnames { get; set; }
        public DbSet<InvStockOpnameProduct> InvStockOpnameProducts { get; set; }

        public DbSet<OutSalesOrder> OutSalesOrders { get; set; }
        public DbSet<OutSalesOrderProduct> OutSalesOrdersProducts { get; set; }
        public DbSet<OutSalesOrderConsignee> OutSalesOrderConsignees { get; set; }
        public DbSet<OutSalesOrderCustomer> OutSalesOrderCustomers { get; set; }

        public DbSet<OutsalesOrderDelivery> OutsalesOrderDeliverys { get; set; }
        public DbSet<OutSalesOrderStorage> OutSalesOrderStorages { get; set; }
        public DbSet<OutSalesOrderPack> OutSalesOrderPacks { get; set; }
        public DbSet<OutSalesOrderAssign> OutSalesOrderAssigns { get; set; }
        public DbSet<OutSalesDispatchtoCourier> OutSalesDispatchtoCouriers { get; set; }

        public DbSet<InvReturn> InvReturns { get; set; }
        public DbSet<InvReturnProduct> InvReturnProducts { get; set; }



        public DbSet<MobileAppVersion> MobileAppVersions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SecProfile>().ToTable("SecProfile");
            modelBuilder.Entity<SecUser>().ToTable("SecUser");
            modelBuilder.Entity<SecUserTenant>().ToTable("SecUserTenant");
            modelBuilder.Entity<SecUserWarehouse>().ToTable("SecUserWarehouse");
            modelBuilder.Entity<SecMenu>().ToTable("SecMenu");
            modelBuilder.Entity<SecProfileMenu>().ToTable("SecProfileMenu");
            modelBuilder.Entity<SecAuditTrail>().ToTable("SecAuditTrail");

            modelBuilder.Entity<MasJabatan>().ToTable("MasJabatan");
            modelBuilder.Entity<MasHouseCode>().ToTable("MasHouseCode");
            modelBuilder.Entity<MasRegional>().ToTable("MasRegional");
            modelBuilder.Entity<MasIndustry>().ToTable("MasIndustry");
            modelBuilder.Entity<MasDirectorate>().ToTable("MasDirectorate");
            modelBuilder.Entity<MasDivision>().ToTable("MasDivision");
            modelBuilder.Entity<MasProvinsi>().ToTable("MasProvinsi");
            modelBuilder.Entity<MasKabupaten>().ToTable("MasKabupaten");
            modelBuilder.Entity<MasKecamatan>().ToTable("MasKecamatan");
            modelBuilder.Entity<MasKelurahan>().ToTable("MasKelurahan");
            modelBuilder.Entity<MasCustomerType>().ToTable("MasCustomerType");
            modelBuilder.Entity<MasCustomerData>().ToTable("MasCustomerData");
            modelBuilder.Entity<MasSupplierType>().ToTable("MasSupplierType");
            modelBuilder.Entity<MasSupplierService>().ToTable("MasSupplierService");
            modelBuilder.Entity<MasSupplierData>().ToTable("MasSupplierData");
            modelBuilder.Entity<MasService>().ToTable("MasService");
            modelBuilder.Entity<MasServiceCategory>().ToTable("MasServiceCategory");
            modelBuilder.Entity<MasPricing>().ToTable("MasPricing");
            modelBuilder.Entity<MasPricingAdditional>().ToTable("MasPricingAdditional");
            modelBuilder.Entity<MasInvoicing>().ToTable("MasInvoicing");
            modelBuilder.Entity<MasInvoicingDetail>().ToTable("MasInvoicingDetail");
            modelBuilder.Entity<MasCheckPoint>().ToTable("MasCheckpoint");
            modelBuilder.Entity<MasProductPriority>().ToTable("MasProductPriority");
            modelBuilder.Entity<MasProductTypeOfRepack>().ToTable("MasProductTypeOfRepack");
            modelBuilder.Entity<MasProductPackaging>().ToTable("MasProductPackaging");
            modelBuilder.Entity<MasProductData>().ToTable("MasProductData");
            modelBuilder.Entity<MasProductBundling>().ToTable("MasProductBundling");
            modelBuilder.Entity<MasProductBundlingData>().ToTable("MasProductBundlingData");
            modelBuilder.Entity<MasDataTenant>().ToTable("MasDataTenant");
            modelBuilder.Entity<MasUnit>().ToTable("MasUnit");
            modelBuilder.Entity<MasBrand>().ToTable("MasBrand");
            modelBuilder.Entity<MasDataTenantDivision>().ToTable("MasDataTenantDivision");
            modelBuilder.Entity<MasDataTenantWarehouse>().ToTable("MasDataTenantWarehouse");
            modelBuilder.Entity<MasSalesType>().ToTable("MasSalesType");
            modelBuilder.Entity<MasSalesCourier>().ToTable("MasSalesCourier");
            modelBuilder.Entity<MasPackingType>().ToTable("MasPackingType");
            modelBuilder.Entity<MasDeliveryOrderCourier>().ToTable("MasDeliveryOrderCourier");
            modelBuilder.Entity<MasPlatform>().ToTable("MasPlatform");
            modelBuilder.Entity<MasStore>().ToTable("MasStore");

            modelBuilder.Entity<InvStorageZone>().ToTable("InvStorageZone");
            modelBuilder.Entity<InvStorageCategory>().ToTable("InvStorageCategory");
            modelBuilder.Entity<InvStorageBesaran>().ToTable("InvStorageBesaran");
            modelBuilder.Entity<InvStorageTebal>().ToTable("InvStorageTebal");
            modelBuilder.Entity<InvStorageSize>().ToTable("InvStorageSize");
            modelBuilder.Entity<InvStorageRow>().ToTable("InvStorageRow");
            modelBuilder.Entity<InvStorageSection>().ToTable("InvStorageSection");
            modelBuilder.Entity<InvStorageColumn>().ToTable("InvStorageColumn");
            modelBuilder.Entity<InvStorageLevel>().ToTable("InvStorageLevel");
            modelBuilder.Entity<InvStorageBin>().ToTable("InvStorageBin");
            modelBuilder.Entity<InvStorageCode>().ToTable("InvStorageCode");
            modelBuilder.Entity<InvProductStock>().ToTable("InvProductStock");
            modelBuilder.Entity<InvProductHistory>().ToTable("InvProductHistory");
            modelBuilder.Entity<InvPickingRoute>().ToTable("InvPickingRoute");
            modelBuilder.Entity<InvPickingRouteColumn>().ToTable("InvPickingRouteColumn");
            modelBuilder.Entity<InvSalesOrderPick>().ToTable("InvSalesOrderPick");
            modelBuilder.Entity<InvRepacking>().ToTable("InvRepacking");
            modelBuilder.Entity<InvRelabeling>().ToTable("InvRelabeling");

            modelBuilder.Entity<IncRequestPurchase>().ToTable("IncRequestPurchase");
            modelBuilder.Entity<IncRequestPurchaseProduct>().ToTable("IncRequestPurchaseProduct");
            modelBuilder.Entity<IncPurchaseOrder>().ToTable("IncPurchaseOrder");
            modelBuilder.Entity<IncPurchaseOrderProduct>().ToTable("IncPurchaseOrderProduct");
            modelBuilder.Entity<IncDeliveryOrder>().ToTable("IncDeliveryOrder");
            modelBuilder.Entity<IncDeliveryOrderProduct>().ToTable("IncDeliveryOrderProduct");
            modelBuilder.Entity<IncDeliveryOrderArrival>().ToTable("IncDeliveryOrderArrival");
            modelBuilder.Entity<IncDeliveryOrderArrivalProduct>().ToTable("IncDeliveryOrderArrivalProduct");
            modelBuilder.Entity<IncQualityCheck>().ToTable("IncQualityCheck");
            modelBuilder.Entity<IncItemProduct>().ToTable("IncItemProduct");
            modelBuilder.Entity<InvProductPutaway>().ToTable("InvProductPutaway");
            modelBuilder.Entity<IncSerialNumber>().ToTable("IncSerialNumber");

            modelBuilder.Entity<InvStockOpname>().ToTable("InvStockOpname");
            modelBuilder.Entity<InvStockOpnameProduct>().ToTable("InvStockOpnameProduct");

            modelBuilder.Entity<OutSalesOrder>().ToTable("OutSalesOrder");
            modelBuilder.Entity<OutSalesOrderProduct>().ToTable("OutSalesOrderProduct");
            modelBuilder.Entity<OutSalesOrderConsignee>().ToTable("OutSalesOrderConsignee");
            modelBuilder.Entity<OutSalesOrderCustomer>().ToTable("OutSalesOrderCustomer");
            modelBuilder.Entity<OutsalesOrderDelivery>().ToTable("OutsalesOrderDelivery");
            modelBuilder.Entity<OutSalesOrderStorage>().ToTable("OutSalesOrderStorage");
            modelBuilder.Entity<OutSalesOrderPack>().ToTable("OutSalesOrderPack");
            modelBuilder.Entity<OutSalesOrderAssign>().ToTable("OutSalesOrderAssign");
            modelBuilder.Entity<OutSalesDispatchtoCourier>().ToTable("OutSalesDispatchtoCourier");

            modelBuilder.Entity<InvReturn>().ToTable("InvReturn");
            modelBuilder.Entity<InvReturnProduct>().ToTable("InvReturnProduct");


            modelBuilder.Entity<MobileAppVersion>().ToTable("MobileAppVersion");
        }
    }
}
