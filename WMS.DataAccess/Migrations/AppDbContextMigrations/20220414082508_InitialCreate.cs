using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvStorageBesaran",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxPanjang = table.Column<int>(type: "int", nullable: false),
                    MaxLebar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageBesaran", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageCategory",
                columns: table => new
                {
                    StorageCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StorageCategoryCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    StorageCategoryName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageCategory", x => x.StorageCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageTebal",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaxTinggi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageTebal", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageZone",
                columns: table => new
                {
                    ZoneCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZoneName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ZonePlanPhoto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageZone", x => x.ZoneCode);
                });

            migrationBuilder.CreateTable(
                name: "MasBrand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BrandImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BrandDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasBrand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasCheckpoint",
                columns: table => new
                {
                    CheckPointId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckPointCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CheckPointName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CheckPointDescription = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasCheckpoint", x => x.CheckPointId);
                });

            migrationBuilder.CreateTable(
                name: "MasCustomerType",
                columns: table => new
                {
                    CustTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustTypeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustTypeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasCustomerType", x => x.CustTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MasDeliveryOrderCourier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasDeliveryOrderCourier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasDirectorate",
                columns: table => new
                {
                    DirCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DirName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasDirectorate", x => x.DirCode);
                });

            migrationBuilder.CreateTable(
                name: "MasIndustry",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndustryCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IndustryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasIndustry", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MasPackingType",
                columns: table => new
                {
                    PackTypeId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PackTypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasPackingType", x => x.PackTypeId);
                });

            migrationBuilder.CreateTable(
                name: "MasProductPackaging",
                columns: table => new
                {
                    PackagingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackagingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PackagingName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasProductPackaging", x => x.PackagingId);
                });

            migrationBuilder.CreateTable(
                name: "MasProductPriority",
                columns: table => new
                {
                    CargoPriorityCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CargoPriorityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CargoPriorityDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasProductPriority", x => x.CargoPriorityCode);
                });

            migrationBuilder.CreateTable(
                name: "MasProductTypeOfRepack",
                columns: table => new
                {
                    RepackCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    RepackName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepackDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasProductTypeOfRepack", x => x.RepackCode);
                });

            migrationBuilder.CreateTable(
                name: "MasRegional",
                columns: table => new
                {
                    RegionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    RegionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasRegional", x => x.RegionId);
                });

            migrationBuilder.CreateTable(
                name: "MasSalesCourier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasSalesCourier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasSalesPlatform",
                columns: table => new
                {
                    SplId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SplName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SplCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasSalesPlatform", x => x.SplId);
                });

            migrationBuilder.CreateTable(
                name: "MasSalesType",
                columns: table => new
                {
                    StyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StyCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasSalesType", x => x.StyId);
                });

            migrationBuilder.CreateTable(
                name: "MasService",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasService", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "MasSupplierService",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierServiceCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SupplierServiceName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasSupplierService", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "MasSupplierType",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierTypeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SupplierTypeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasSupplierType", x => x.TypeId);
                });

            migrationBuilder.CreateTable(
                name: "MasUnit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UnitOperantor = table.Column<int>(type: "int", nullable: false),
                    OperatorValue = table.Column<int>(type: "int", nullable: false),
                    UnitDescription = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecMenu",
                columns: table => new
                {
                    MenuId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    MenuName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MenuSort = table.Column<int>(type: "int", nullable: false),
                    Controller = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MenuGroup = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MenuKey = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    MenuFlag = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecMenu", x => x.MenuId);
                });

            migrationBuilder.CreateTable(
                name: "SecProfile",
                columns: table => new
                {
                    ProfileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecProfile", x => x.ProfileId);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageSize",
                columns: table => new
                {
                    SizeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SizeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BesaranCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TebalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageSize", x => x.SizeCode);
                    table.ForeignKey(
                        name: "FK_InvStorageSize_InvStorageBesaran_BesaranCode",
                        column: x => x.BesaranCode,
                        principalTable: "InvStorageBesaran",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStorageSize_InvStorageTebal_TebalCode",
                        column: x => x.TebalCode,
                        principalTable: "InvStorageTebal",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasDivision",
                columns: table => new
                {
                    DivId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DivCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DivName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DirCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasDivision", x => x.DivId);
                    table.ForeignKey(
                        name: "FK_MasDivision_MasDirectorate_DirCode",
                        column: x => x.DirCode,
                        principalTable: "MasDirectorate",
                        principalColumn: "DirCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasProvinsi",
                columns: table => new
                {
                    ProId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasProvinsi", x => x.ProId);
                    table.ForeignKey(
                        name: "FK_MasProvinsi_MasRegional_RegionId",
                        column: x => x.RegionId,
                        principalTable: "MasRegional",
                        principalColumn: "RegionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasServiceCategory",
                columns: table => new
                {
                    ServiceCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasServiceCategory", x => x.ServiceCategoryId);
                    table.ForeignKey(
                        name: "FK_MasServiceCategory_MasService_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "MasService",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecProfileMenu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    MenuId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IsView = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IsInsert = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IsEdit = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IsDelete = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    IsPrint = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    Flag = table.Column<int>(type: "int", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecProfileMenu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecProfileMenu_SecMenu_MenuId",
                        column: x => x.MenuId,
                        principalTable: "SecMenu",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecProfileMenu_SecProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "SecProfile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasJabatan",
                columns: table => new
                {
                    JobPosId = table.Column<int>(type: "int", nullable: false),
                    JobPosCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    JobPosName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DivId = table.Column<int>(type: "int", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasJabatan", x => x.JobPosId);
                    table.ForeignKey(
                        name: "FK_MasJabatan_MasDivision_DivId",
                        column: x => x.DivId,
                        principalTable: "MasDivision",
                        principalColumn: "DivId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasKabupaten",
                columns: table => new
                {
                    KabId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KabName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasKabupaten", x => x.KabId);
                    table.ForeignKey(
                        name: "FK_MasKabupaten_MasProvinsi_ProId",
                        column: x => x.ProId,
                        principalTable: "MasProvinsi",
                        principalColumn: "ProId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasKecamatan",
                columns: table => new
                {
                    KecId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KecName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    KabId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasKecamatan", x => x.KecId);
                    table.ForeignKey(
                        name: "FK_MasKecamatan_MasKabupaten_KabId",
                        column: x => x.KabId,
                        principalTable: "MasKabupaten",
                        principalColumn: "KabId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasKelurahan",
                columns: table => new
                {
                    KelId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KelName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    KecId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasKelurahan", x => x.KelId);
                    table.ForeignKey(
                        name: "FK_MasKelurahan_MasKecamatan_KecId",
                        column: x => x.KecId,
                        principalTable: "MasKecamatan",
                        principalColumn: "KecId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasCustomerData",
                columns: table => new
                {
                    CustId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    KelId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KodePos = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OfficePhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    HandPhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    RekeningNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TermOfPayment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoAkhir = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustTypeId = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<int>(type: "int", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasCustomerData", x => x.CustId);
                    table.ForeignKey(
                        name: "FK_MasCustomerData_MasCustomerType_CustTypeId",
                        column: x => x.CustTypeId,
                        principalTable: "MasCustomerType",
                        principalColumn: "CustTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasCustomerData_MasIndustry_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "MasIndustry",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasCustomerData_MasKelurahan_KelId",
                        column: x => x.KelId,
                        principalTable: "MasKelurahan",
                        principalColumn: "KelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasDataTenant",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KelId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KodePos = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OfficePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ShowPrice = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasDataTenant", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_MasDataTenant_MasKelurahan_KelId",
                        column: x => x.KelId,
                        principalTable: "MasKelurahan",
                        principalColumn: "KelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasHouseCode",
                columns: table => new
                {
                    HouseCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    HouseName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    KelId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    KodePos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    OfficePhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Latitude = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BuildStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BuildSize = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AccessArea = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HouseManager = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasHouseCode", x => x.HouseCode);
                    table.ForeignKey(
                        name: "FK_MasHouseCode_MasKelurahan_KelId",
                        column: x => x.KelId,
                        principalTable: "MasKelurahan",
                        principalColumn: "KelId");
                });

            migrationBuilder.CreateTable(
                name: "MasDataTenantDivision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasDataTenantDivision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasDataTenantDivision_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasPricing",
                columns: table => new
                {
                    PriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageMin = table.Column<int>(type: "int", nullable: false),
                    StorageRates = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StorageRatesType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReceivingFeeMin = table.Column<int>(type: "int", nullable: false),
                    ReceivingFeeRates = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReceivingFeeRatesType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OutgoingFeeMin = table.Column<int>(type: "int", nullable: false),
                    OutgoingFeeRates = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OutgoingFeeRatesType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SystemCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ManagementFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InsuranceFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasPricing", x => x.PriceId);
                    table.ForeignKey(
                        name: "FK_MasPricing_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasProductData",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeautyPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloseUpPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualWeight = table.Column<int>(type: "int", nullable: false),
                    Ipb = table.Column<int>(type: "int", nullable: true),
                    Panjang = table.Column<int>(type: "int", nullable: false),
                    Lebar = table.Column<int>(type: "int", nullable: false),
                    Tinggi = table.Column<int>(type: "int", nullable: false),
                    VolWight = table.Column<int>(type: "int", nullable: false),
                    Storageperiod = table.Column<int>(type: "int", nullable: true),
                    SafetyStock = table.Column<int>(type: "int", nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Resellable = table.Column<bool>(type: "bit", nullable: false),
                    ResellablePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StorageMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PackagingId = table.Column<int>(type: "int", nullable: false),
                    RePackaging = table.Column<bool>(type: "bit", nullable: false),
                    TypeOfRepackCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    CargoPriorityCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Supplier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SizeCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    ZoneCode = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasProductData", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_MasProductData_InvStorageCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InvStorageCategory",
                        principalColumn: "StorageCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasProductData_InvStorageSize_SizeCode",
                        column: x => x.SizeCode,
                        principalTable: "InvStorageSize",
                        principalColumn: "SizeCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasProductData_InvStorageZone_ZoneCode",
                        column: x => x.ZoneCode,
                        principalTable: "InvStorageZone",
                        principalColumn: "ZoneCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasProductData_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasProductData_MasProductPackaging_PackagingId",
                        column: x => x.PackagingId,
                        principalTable: "MasProductPackaging",
                        principalColumn: "PackagingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasSupplierData",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    KelId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KodePos = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OfficePhone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HandPhone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RekeningNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TermOfPayment = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SupplierTypeId = table.Column<int>(type: "int", nullable: false),
                    SupplierServiceId = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasSupplierData", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_MasSupplierData_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId");
                    table.ForeignKey(
                        name: "FK_MasSupplierData_MasIndustry_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "MasIndustry",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasSupplierData_MasKelurahan_KelId",
                        column: x => x.KelId,
                        principalTable: "MasKelurahan",
                        principalColumn: "KelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasSupplierData_MasSupplierService_SupplierServiceId",
                        column: x => x.SupplierServiceId,
                        principalTable: "MasSupplierService",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasSupplierData_MasSupplierType_SupplierTypeId",
                        column: x => x.SupplierTypeId,
                        principalTable: "MasSupplierType",
                        principalColumn: "TypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageRow",
                columns: table => new
                {
                    RowCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RowName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RowPlanPhoto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    ZoneCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageRow", x => x.RowCode);
                    table.ForeignKey(
                        name: "FK_InvStorageRow_InvStorageZone_ZoneCode",
                        column: x => x.ZoneCode,
                        principalTable: "InvStorageZone",
                        principalColumn: "ZoneCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStorageRow_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode");
                });

            migrationBuilder.CreateTable(
                name: "MasDataTenantWarehouse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasDataTenantWarehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasDataTenantWarehouse_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasDataTenantWarehouse_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutSalesOrder",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    OrdPlatform = table.Column<int>(type: "int", nullable: false),
                    OrdSalesType = table.Column<int>(type: "int", nullable: false),
                    DateOrdered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrder", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrder_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutSalesOrder_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode");
                    table.ForeignKey(
                        name: "FK_OutSalesOrder_MasSalesPlatform_OrdPlatform",
                        column: x => x.OrdPlatform,
                        principalTable: "MasSalesPlatform",
                        principalColumn: "SplId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutSalesOrder_MasSalesType_OrdSalesType",
                        column: x => x.OrdSalesType,
                        principalTable: "MasSalesType",
                        principalColumn: "StyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecUser",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailConfirmed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumberConfirmed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProfileId = table.Column<int>(type: "int", nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    JobPosId = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpireFlag = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecUser", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_SecUser_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecUser_MasJabatan_JobPosId",
                        column: x => x.JobPosId,
                        principalTable: "MasJabatan",
                        principalColumn: "JobPosId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecUser_SecProfile_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "SecProfile",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasPricingAdditional",
                columns: table => new
                {
                    AddId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceId = table.Column<int>(type: "int", nullable: false),
                    AddName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AddMin = table.Column<int>(type: "int", nullable: false),
                    AddFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AddFeeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasPricingAdditional", x => x.AddId);
                    table.ForeignKey(
                        name: "FK_MasPricingAdditional_MasPricing_PriceId",
                        column: x => x.PriceId,
                        principalTable: "MasPricing",
                        principalColumn: "PriceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvProductHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    HistoryType = table.Column<int>(type: "int", nullable: false),
                    TrxNo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Interest = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    DatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvProductHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvProductHistory_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvProductHistory_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvProductStock",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvProductStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvProductStock_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvProductStock_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageColumn",
                columns: table => new
                {
                    ColumnCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    RowCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageColumn", x => x.ColumnCode);
                    table.ForeignKey(
                        name: "FK_InvStorageColumn_InvStorageRow_RowCode",
                        column: x => x.RowCode,
                        principalTable: "InvStorageRow",
                        principalColumn: "RowCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageSection",
                columns: table => new
                {
                    SectionCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RowCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageSection", x => x.SectionCode);
                    table.ForeignKey(
                        name: "FK_InvStorageSection_InvStorageRow_RowCode",
                        column: x => x.RowCode,
                        principalTable: "InvStorageRow",
                        principalColumn: "RowCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncRequestPurchase",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantDivisionId = table.Column<int>(type: "int", nullable: true),
                    TenantHouseId = table.Column<int>(type: "int", nullable: false),
                    SpecialInstruction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateRequested = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateReviewed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateApproved = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestStatus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncRequestPurchase", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_IncRequestPurchase_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId");
                    table.ForeignKey(
                        name: "FK_IncRequestPurchase_MasDataTenantDivision_TenantDivisionId",
                        column: x => x.TenantDivisionId,
                        principalTable: "MasDataTenantDivision",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncRequestPurchase_MasDataTenantWarehouse_TenantHouseId",
                        column: x => x.TenantHouseId,
                        principalTable: "MasDataTenantWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutSalesOrderConsignee",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CneeName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CneePhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CneeAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    KelId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CneeCity = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OrdZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CneeLatitude = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CneeLongitude = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderConsignee", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderConsignee_MasKelurahan_KelId",
                        column: x => x.KelId,
                        principalTable: "MasKelurahan",
                        principalColumn: "KelId");
                    table.ForeignKey(
                        name: "FK_OutSalesOrderConsignee_OutSalesOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutSalesOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutSalesOrderCustomer",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CustPhone = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CustAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    KelId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CustCity = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CustZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderCustomer", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderCustomer_MasKelurahan_KelId",
                        column: x => x.KelId,
                        principalTable: "MasKelurahan",
                        principalColumn: "KelId");
                    table.ForeignKey(
                        name: "FK_OutSalesOrderCustomer_OutSalesOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutSalesOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutsalesOrderDelivery",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrdCourier = table.Column<int>(type: "int", nullable: false),
                    OrdCourierService = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AirwayBill = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsalesOrderDelivery", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OutsalesOrderDelivery_MasSalesCourier_OrdCourier",
                        column: x => x.OrdCourier,
                        principalTable: "MasSalesCourier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutsalesOrderDelivery_OutSalesOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutSalesOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutSalesOrderProduct",
                columns: table => new
                {
                    OrdProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<float>(type: "real", nullable: false),
                    SubTotal = table.Column<float>(type: "real", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderProduct", x => x.OrdProductId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderProduct_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderProduct_OutSalesOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutSalesOrder",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateTable(
                name: "SecAuditTrail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EventMenu = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EventFunction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventDesctiption = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecAuditTrail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecAuditTrail_SecUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SecUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStorageLevel",
                columns: table => new
                {
                    LevelCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LevelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LevelPlanPhoto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    ColumnCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SectionCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageLevel", x => x.LevelCode);
                    table.ForeignKey(
                        name: "FK_InvStorageLevel_InvStorageColumn_ColumnCode",
                        column: x => x.ColumnCode,
                        principalTable: "InvStorageColumn",
                        principalColumn: "ColumnCode");
                    table.ForeignKey(
                        name: "FK_InvStorageLevel_InvStorageSection_SectionCode",
                        column: x => x.SectionCode,
                        principalTable: "InvStorageSection",
                        principalColumn: "SectionCode");
                });

            migrationBuilder.CreateTable(
                name: "IncPurchaseOrder",
                columns: table => new
                {
                    PONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantDivisionId = table.Column<int>(type: "int", nullable: true),
                    TenantHouseId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OrderTax = table.Column<float>(type: "real", nullable: true),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncPurchaseOrder", x => x.PONumber);
                    table.ForeignKey(
                        name: "FK_IncPurchaseOrder_IncRequestPurchase_RequestId",
                        column: x => x.RequestId,
                        principalTable: "IncRequestPurchase",
                        principalColumn: "RequestId");
                    table.ForeignKey(
                        name: "FK_IncPurchaseOrder_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId");
                    table.ForeignKey(
                        name: "FK_IncPurchaseOrder_MasDataTenantDivision_TenantDivisionId",
                        column: x => x.TenantDivisionId,
                        principalTable: "MasDataTenantDivision",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncPurchaseOrder_MasDataTenantWarehouse_TenantHouseId",
                        column: x => x.TenantHouseId,
                        principalTable: "MasDataTenantWarehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncPurchaseOrder_MasSupplierData_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "MasSupplierData",
                        principalColumn: "SupplierId");
                });

            migrationBuilder.CreateTable(
                name: "IncRequestPurchaseProduct",
                columns: table => new
                {
                    RequestProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ApprovedQuantity = table.Column<int>(type: "int", nullable: true),
                    BidPrice = table.Column<float>(type: "real", nullable: true),
                    NegotiatedPrice = table.Column<float>(type: "real", nullable: true),
                    FinalPrice = table.Column<float>(type: "real", nullable: false),
                    ExpArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Supplier = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Memo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ApprovedMemo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncRequestPurchaseProduct", x => x.RequestProductId);
                    table.ForeignKey(
                        name: "FK_IncRequestPurchaseProduct_IncRequestPurchase_RequestId",
                        column: x => x.RequestId,
                        principalTable: "IncRequestPurchase",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncRequestPurchaseProduct_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "InvStorageBin",
                columns: table => new
                {
                    BinCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BinName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LevelCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PlanPhotoLocation = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageBin", x => x.BinCode);
                    table.ForeignKey(
                        name: "FK_InvStorageBin_InvStorageLevel_LevelCode",
                        column: x => x.LevelCode,
                        principalTable: "InvStorageLevel",
                        principalColumn: "LevelCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncDeliveryOrder",
                columns: table => new
                {
                    DONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DeliveryCourierId = table.Column<int>(type: "int", nullable: true),
                    DateDelivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateArrived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncDeliveryOrder", x => x.DONumber);
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrder_IncPurchaseOrder_PONumber",
                        column: x => x.PONumber,
                        principalTable: "IncPurchaseOrder",
                        principalColumn: "PONumber");
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrder_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId");
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrder_MasDeliveryOrderCourier_DeliveryCourierId",
                        column: x => x.DeliveryCourierId,
                        principalTable: "MasDeliveryOrderCourier",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrder_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode");
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrder_MasSupplierData_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "MasSupplierData",
                        principalColumn: "SupplierId");
                });

            migrationBuilder.CreateTable(
                name: "IncPurchaseOrderProduct",
                columns: table => new
                {
                    POProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DOQuantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<float>(type: "real", nullable: false),
                    SubTotal = table.Column<float>(type: "real", nullable: false),
                    ExpArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SpecialInstruction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClosedNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncPurchaseOrderProduct", x => x.POProductId);
                    table.ForeignKey(
                        name: "FK_IncPurchaseOrderProduct_IncPurchaseOrder_PONumber",
                        column: x => x.PONumber,
                        principalTable: "IncPurchaseOrder",
                        principalColumn: "PONumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncPurchaseOrderProduct_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "InvStorageCode",
                columns: table => new
                {
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageCategoryId = table.Column<int>(type: "int", nullable: false),
                    BinCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SizeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PlanPhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorageCode", x => x.StorageCode);
                    table.ForeignKey(
                        name: "FK_InvStorageCode_InvStorageBin_BinCode",
                        column: x => x.BinCode,
                        principalTable: "InvStorageBin",
                        principalColumn: "BinCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStorageCode_InvStorageCategory_StorageCategoryId",
                        column: x => x.StorageCategoryId,
                        principalTable: "InvStorageCategory",
                        principalColumn: "StorageCategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStorageCode_InvStorageSize_SizeCode",
                        column: x => x.SizeCode,
                        principalTable: "InvStorageSize",
                        principalColumn: "SizeCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncDeliveryOrderProduct",
                columns: table => new
                {
                    DOProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DONumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    POProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<float>(type: "real", nullable: false),
                    SubTotal = table.Column<float>(type: "real", nullable: false),
                    ClosedNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfExpired = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncDeliveryOrderProduct", x => x.DOProductId);
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrderProduct_IncDeliveryOrder_DONumber",
                        column: x => x.DONumber,
                        principalTable: "IncDeliveryOrder",
                        principalColumn: "DONumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrderProduct_IncPurchaseOrderProduct_POProductId",
                        column: x => x.POProductId,
                        principalTable: "IncPurchaseOrderProduct",
                        principalColumn: "POProductId");
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrderProduct_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateTable(
                name: "IncDeliveryOrderArrival",
                columns: table => new
                {
                    DOProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NotaImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateArrived = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncDeliveryOrderArrival", x => x.DOProductId);
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrderArrival_IncDeliveryOrderProduct_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderProduct",
                        principalColumn: "DOProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncItemProduct",
                columns: table => new
                {
                    IKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DOProductId = table.Column<int>(type: "int", nullable: false),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateArrived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArrivedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DatePutedAway = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PutedAwatBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncItemProduct", x => x.IKU);
                    table.ForeignKey(
                        name: "FK_IncItemProduct_IncDeliveryOrderProduct_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderProduct",
                        principalColumn: "DOProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncItemProduct_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvProductPutaway",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DOProductId = table.Column<int>(type: "int", nullable: false),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DatePutaway = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PutBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvProductPutaway", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvProductPutaway_IncDeliveryOrderProduct_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderProduct",
                        principalColumn: "DOProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvProductPutaway_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncQualityCheck",
                columns: table => new
                {
                    DOProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductImage = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DateChecked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NextStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncQualityCheck", x => x.DOProductId);
                    table.ForeignKey(
                        name: "FK_IncQualityCheck_IncDeliveryOrderArrival_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderArrival",
                        principalColumn: "DOProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutSalesOrderPick",
                columns: table => new
                {
                    PickId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdProductId = table.Column<int>(type: "int", nullable: true),
                    IKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrdQty = table.Column<int>(type: "int", nullable: false),
                    DatePicked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PickedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quality = table.Column<bool>(type: "bit", nullable: false),
                    DateChecked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderPick", x => x.PickId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderPick_IncItemProduct_IKU",
                        column: x => x.IKU,
                        principalTable: "IncItemProduct",
                        principalColumn: "IKU");
                    table.ForeignKey(
                        name: "FK_OutSalesOrderPick_OutSalesOrderProduct_OrdProductId",
                        column: x => x.OrdProductId,
                        principalTable: "OutSalesOrderProduct",
                        principalColumn: "OrdProductId");
                });

            migrationBuilder.CreateTable(
                name: "OutSalesOrderPack",
                columns: table => new
                {
                    PickId = table.Column<int>(type: "int", nullable: false),
                    DatePacked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PackedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PackTypeId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderPack", x => x.PickId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderPack_MasPackingType_PackTypeId",
                        column: x => x.PackTypeId,
                        principalTable: "MasPackingType",
                        principalColumn: "PackTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderPack_OutSalesOrderPick_PickId",
                        column: x => x.PickId,
                        principalTable: "OutSalesOrderPick",
                        principalColumn: "PickId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrder_DeliveryCourierId",
                table: "IncDeliveryOrder",
                column: "DeliveryCourierId");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrder_HouseCode",
                table: "IncDeliveryOrder",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrder_PONumber",
                table: "IncDeliveryOrder",
                column: "PONumber");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrder_SupplierId",
                table: "IncDeliveryOrder",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrder_TenantId",
                table: "IncDeliveryOrder",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrderProduct_DONumber",
                table: "IncDeliveryOrderProduct",
                column: "DONumber");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrderProduct_POProductId",
                table: "IncDeliveryOrderProduct",
                column: "POProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrderProduct_ProductId",
                table: "IncDeliveryOrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncItemProduct_DOProductId",
                table: "IncItemProduct",
                column: "DOProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncItemProduct_StorageCode",
                table: "IncItemProduct",
                column: "StorageCode");

            migrationBuilder.CreateIndex(
                name: "IX_IncPurchaseOrder_RequestId",
                table: "IncPurchaseOrder",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_IncPurchaseOrder_SupplierId",
                table: "IncPurchaseOrder",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_IncPurchaseOrder_TenantDivisionId",
                table: "IncPurchaseOrder",
                column: "TenantDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncPurchaseOrder_TenantHouseId",
                table: "IncPurchaseOrder",
                column: "TenantHouseId");

            migrationBuilder.CreateIndex(
                name: "IX_IncPurchaseOrder_TenantId",
                table: "IncPurchaseOrder",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IncPurchaseOrderProduct_PONumber",
                table: "IncPurchaseOrderProduct",
                column: "PONumber");

            migrationBuilder.CreateIndex(
                name: "IX_IncPurchaseOrderProduct_ProductId",
                table: "IncPurchaseOrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncRequestPurchase_TenantDivisionId",
                table: "IncRequestPurchase",
                column: "TenantDivisionId");

            migrationBuilder.CreateIndex(
                name: "IX_IncRequestPurchase_TenantHouseId",
                table: "IncRequestPurchase",
                column: "TenantHouseId");

            migrationBuilder.CreateIndex(
                name: "IX_IncRequestPurchase_TenantId",
                table: "IncRequestPurchase",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_IncRequestPurchaseProduct_ProductId",
                table: "IncRequestPurchaseProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncRequestPurchaseProduct_RequestId",
                table: "IncRequestPurchaseProduct",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InvProductHistory_HouseCode",
                table: "InvProductHistory",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvProductHistory_ProductId",
                table: "InvProductHistory",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvProductPutaway_DOProductId",
                table: "InvProductPutaway",
                column: "DOProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvProductPutaway_StorageCode",
                table: "InvProductPutaway",
                column: "StorageCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvProductStock_HouseCode",
                table: "InvProductStock",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvProductStock_ProductId",
                table: "InvProductStock",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageBin_LevelCode",
                table: "InvStorageBin",
                column: "LevelCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageCode_BinCode",
                table: "InvStorageCode",
                column: "BinCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageCode_SizeCode",
                table: "InvStorageCode",
                column: "SizeCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageCode_StorageCategoryId",
                table: "InvStorageCode",
                column: "StorageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageColumn_RowCode",
                table: "InvStorageColumn",
                column: "RowCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageLevel_ColumnCode",
                table: "InvStorageLevel",
                column: "ColumnCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageLevel_SectionCode",
                table: "InvStorageLevel",
                column: "SectionCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageRow_HouseCode",
                table: "InvStorageRow",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageRow_ZoneCode",
                table: "InvStorageRow",
                column: "ZoneCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageSection_RowCode",
                table: "InvStorageSection",
                column: "RowCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageSize_BesaranCode",
                table: "InvStorageSize",
                column: "BesaranCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageSize_TebalCode",
                table: "InvStorageSize",
                column: "TebalCode");

            migrationBuilder.CreateIndex(
                name: "IX_MasCustomerData_CustTypeId",
                table: "MasCustomerData",
                column: "CustTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MasCustomerData_IndustryId",
                table: "MasCustomerData",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_MasCustomerData_KelId",
                table: "MasCustomerData",
                column: "KelId");

            migrationBuilder.CreateIndex(
                name: "IX_MasDataTenant_KelId",
                table: "MasDataTenant",
                column: "KelId");

            migrationBuilder.CreateIndex(
                name: "IX_MasDataTenantDivision_TenantId",
                table: "MasDataTenantDivision",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MasDataTenantWarehouse_HouseCode",
                table: "MasDataTenantWarehouse",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_MasDataTenantWarehouse_TenantId",
                table: "MasDataTenantWarehouse",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MasDivision_DirCode",
                table: "MasDivision",
                column: "DirCode");

            migrationBuilder.CreateIndex(
                name: "IX_MasHouseCode_KelId",
                table: "MasHouseCode",
                column: "KelId");

            migrationBuilder.CreateIndex(
                name: "IX_MasJabatan_DivId",
                table: "MasJabatan",
                column: "DivId");

            migrationBuilder.CreateIndex(
                name: "IX_MasKabupaten_ProId",
                table: "MasKabupaten",
                column: "ProId");

            migrationBuilder.CreateIndex(
                name: "IX_MasKecamatan_KabId",
                table: "MasKecamatan",
                column: "KabId");

            migrationBuilder.CreateIndex(
                name: "IX_MasKelurahan_KecId",
                table: "MasKelurahan",
                column: "KecId");

            migrationBuilder.CreateIndex(
                name: "IX_MasPricing_TenantId",
                table: "MasPricing",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MasPricingAdditional_PriceId",
                table: "MasPricingAdditional",
                column: "PriceId");

            migrationBuilder.CreateIndex(
                name: "IX_MasProductData_CategoryId",
                table: "MasProductData",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MasProductData_PackagingId",
                table: "MasProductData",
                column: "PackagingId");

            migrationBuilder.CreateIndex(
                name: "IX_MasProductData_SizeCode",
                table: "MasProductData",
                column: "SizeCode");

            migrationBuilder.CreateIndex(
                name: "IX_MasProductData_TenantId",
                table: "MasProductData",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MasProductData_ZoneCode",
                table: "MasProductData",
                column: "ZoneCode");

            migrationBuilder.CreateIndex(
                name: "IX_MasProvinsi_RegionId",
                table: "MasProvinsi",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_MasServiceCategory_ServiceId",
                table: "MasServiceCategory",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_MasSupplierData_IndustryId",
                table: "MasSupplierData",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_MasSupplierData_KelId",
                table: "MasSupplierData",
                column: "KelId");

            migrationBuilder.CreateIndex(
                name: "IX_MasSupplierData_SupplierServiceId",
                table: "MasSupplierData",
                column: "SupplierServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_MasSupplierData_SupplierTypeId",
                table: "MasSupplierData",
                column: "SupplierTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MasSupplierData_TenantId",
                table: "MasSupplierData",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrder_HouseCode",
                table: "OutSalesOrder",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrder_OrdPlatform",
                table: "OutSalesOrder",
                column: "OrdPlatform");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrder_OrdSalesType",
                table: "OutSalesOrder",
                column: "OrdSalesType");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrder_TenantId",
                table: "OutSalesOrder",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderConsignee_KelId",
                table: "OutSalesOrderConsignee",
                column: "KelId");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderCustomer_KelId",
                table: "OutSalesOrderCustomer",
                column: "KelId");

            migrationBuilder.CreateIndex(
                name: "IX_OutsalesOrderDelivery_OrdCourier",
                table: "OutsalesOrderDelivery",
                column: "OrdCourier");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderPack_PackTypeId",
                table: "OutSalesOrderPack",
                column: "PackTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderPick_IKU",
                table: "OutSalesOrderPick",
                column: "IKU");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderPick_OrdProductId",
                table: "OutSalesOrderPick",
                column: "OrdProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderProduct_OrderId",
                table: "OutSalesOrderProduct",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderProduct_ProductId",
                table: "OutSalesOrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SecAuditTrail_UserId",
                table: "SecAuditTrail",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SecProfileMenu_MenuId",
                table: "SecProfileMenu",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_SecProfileMenu_ProfileId",
                table: "SecProfileMenu",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_SecUser_HouseCode",
                table: "SecUser",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_SecUser_JobPosId",
                table: "SecUser",
                column: "JobPosId");

            migrationBuilder.CreateIndex(
                name: "IX_SecUser_ProfileId",
                table: "SecUser",
                column: "ProfileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncQualityCheck");

            migrationBuilder.DropTable(
                name: "IncRequestPurchaseProduct");

            migrationBuilder.DropTable(
                name: "InvProductHistory");

            migrationBuilder.DropTable(
                name: "InvProductPutaway");

            migrationBuilder.DropTable(
                name: "InvProductStock");

            migrationBuilder.DropTable(
                name: "MasBrand");

            migrationBuilder.DropTable(
                name: "MasCheckpoint");

            migrationBuilder.DropTable(
                name: "MasCustomerData");

            migrationBuilder.DropTable(
                name: "MasPricingAdditional");

            migrationBuilder.DropTable(
                name: "MasProductPriority");

            migrationBuilder.DropTable(
                name: "MasProductTypeOfRepack");

            migrationBuilder.DropTable(
                name: "MasServiceCategory");

            migrationBuilder.DropTable(
                name: "MasUnit");

            migrationBuilder.DropTable(
                name: "OutSalesOrderConsignee");

            migrationBuilder.DropTable(
                name: "OutSalesOrderCustomer");

            migrationBuilder.DropTable(
                name: "OutsalesOrderDelivery");

            migrationBuilder.DropTable(
                name: "OutSalesOrderPack");

            migrationBuilder.DropTable(
                name: "SecAuditTrail");

            migrationBuilder.DropTable(
                name: "SecProfileMenu");

            migrationBuilder.DropTable(
                name: "IncDeliveryOrderArrival");

            migrationBuilder.DropTable(
                name: "MasCustomerType");

            migrationBuilder.DropTable(
                name: "MasPricing");

            migrationBuilder.DropTable(
                name: "MasService");

            migrationBuilder.DropTable(
                name: "MasSalesCourier");

            migrationBuilder.DropTable(
                name: "MasPackingType");

            migrationBuilder.DropTable(
                name: "OutSalesOrderPick");

            migrationBuilder.DropTable(
                name: "SecUser");

            migrationBuilder.DropTable(
                name: "SecMenu");

            migrationBuilder.DropTable(
                name: "IncItemProduct");

            migrationBuilder.DropTable(
                name: "OutSalesOrderProduct");

            migrationBuilder.DropTable(
                name: "MasJabatan");

            migrationBuilder.DropTable(
                name: "SecProfile");

            migrationBuilder.DropTable(
                name: "IncDeliveryOrderProduct");

            migrationBuilder.DropTable(
                name: "InvStorageCode");

            migrationBuilder.DropTable(
                name: "OutSalesOrder");

            migrationBuilder.DropTable(
                name: "MasDivision");

            migrationBuilder.DropTable(
                name: "IncDeliveryOrder");

            migrationBuilder.DropTable(
                name: "IncPurchaseOrderProduct");

            migrationBuilder.DropTable(
                name: "InvStorageBin");

            migrationBuilder.DropTable(
                name: "MasSalesPlatform");

            migrationBuilder.DropTable(
                name: "MasSalesType");

            migrationBuilder.DropTable(
                name: "MasDirectorate");

            migrationBuilder.DropTable(
                name: "MasDeliveryOrderCourier");

            migrationBuilder.DropTable(
                name: "IncPurchaseOrder");

            migrationBuilder.DropTable(
                name: "MasProductData");

            migrationBuilder.DropTable(
                name: "InvStorageLevel");

            migrationBuilder.DropTable(
                name: "IncRequestPurchase");

            migrationBuilder.DropTable(
                name: "MasSupplierData");

            migrationBuilder.DropTable(
                name: "InvStorageCategory");

            migrationBuilder.DropTable(
                name: "InvStorageSize");

            migrationBuilder.DropTable(
                name: "MasProductPackaging");

            migrationBuilder.DropTable(
                name: "InvStorageColumn");

            migrationBuilder.DropTable(
                name: "InvStorageSection");

            migrationBuilder.DropTable(
                name: "MasDataTenantDivision");

            migrationBuilder.DropTable(
                name: "MasDataTenantWarehouse");

            migrationBuilder.DropTable(
                name: "MasIndustry");

            migrationBuilder.DropTable(
                name: "MasSupplierService");

            migrationBuilder.DropTable(
                name: "MasSupplierType");

            migrationBuilder.DropTable(
                name: "InvStorageBesaran");

            migrationBuilder.DropTable(
                name: "InvStorageTebal");

            migrationBuilder.DropTable(
                name: "InvStorageRow");

            migrationBuilder.DropTable(
                name: "MasDataTenant");

            migrationBuilder.DropTable(
                name: "InvStorageZone");

            migrationBuilder.DropTable(
                name: "MasHouseCode");

            migrationBuilder.DropTable(
                name: "MasKelurahan");

            migrationBuilder.DropTable(
                name: "MasKecamatan");

            migrationBuilder.DropTable(
                name: "MasKabupaten");

            migrationBuilder.DropTable(
                name: "MasProvinsi");

            migrationBuilder.DropTable(
                name: "MasRegional");
        }
    }
}
