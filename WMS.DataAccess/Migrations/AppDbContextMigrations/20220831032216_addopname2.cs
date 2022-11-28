using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class addopname2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvStockOpnameProductStorage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_InvStockOpnameProductStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStockOpnameProductStorage_InvStockOpnameProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "InvStockOpnameProduct",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStockOpnameProductStorage_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProductStorage_ProductId",
                table: "InvStockOpnameProductStorage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProductStorage_StorageCode",
                table: "InvStockOpnameProductStorage",
                column: "StorageCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvStockOpnameProductStorage");
        }
    }
}
