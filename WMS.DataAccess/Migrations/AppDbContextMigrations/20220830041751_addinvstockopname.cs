using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class addinvstockopname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvStockOpname",
                columns: table => new
                {
                    OpnameId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    DateOpname = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStockOpname", x => x.OpnameId);
                    table.ForeignKey(
                        name: "FK_InvStockOpname_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStockOpname_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStockOpnameProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpnameId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOpname = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SystemQty = table.Column<int>(type: "int", nullable: false),
                    ExpiredQty = table.Column<int>(type: "int", nullable: false),
                    BrokenQty = table.Column<int>(type: "int", nullable: false),
                    BrokenDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StockQty = table.Column<int>(type: "int", nullable: false),
                    DiscrepancyQty = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStockOpnameProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStockOpnameProduct_InvStockOpname_OpnameId",
                        column: x => x.OpnameId,
                        principalTable: "InvStockOpname",
                        principalColumn: "OpnameId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStockOpnameProduct_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStockOpnameProduct_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpname_HouseCode",
                table: "InvStockOpname",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpname_TenantId",
                table: "InvStockOpname",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProduct_OpnameId",
                table: "InvStockOpnameProduct",
                column: "OpnameId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProduct_ProductId",
                table: "InvStockOpnameProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProduct_StorageCode",
                table: "InvStockOpnameProduct",
                column: "StorageCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvStockOpnameProduct");

            migrationBuilder.DropTable(
                name: "InvStockOpname");
        }
    }
}
