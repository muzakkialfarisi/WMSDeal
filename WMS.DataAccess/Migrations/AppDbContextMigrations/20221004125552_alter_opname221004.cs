using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alter_opname221004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvStockOpnameProductStorage");

            migrationBuilder.AddColumn<string>(
                name: "BrokenDescription",
                table: "InvStockOpnameProduct",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BrokenQty",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InvStockOpnameProduct",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "InvStockOpnameProduct",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOpname",
                table: "InvStockOpnameProduct",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DiscrepancyQty",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpiredQty",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "InvStockOpnameProduct",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Log",
                table: "InvStockOpnameProduct",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StockQty",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SystemQty",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrokenDescription",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "BrokenQty",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "DateOpname",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "DiscrepancyQty",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "ExpiredQty",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "Log",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "StockQty",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "SystemQty",
                table: "InvStockOpnameProduct");

            migrationBuilder.CreateTable(
                name: "InvStockOpnameProductStorage",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OpnameProductId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BrokenDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrokenQty = table.Column<int>(type: "int", nullable: false),
                    DateOpname = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscrepancyQty = table.Column<int>(type: "int", nullable: false),
                    ExpiredQty = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StockQty = table.Column<int>(type: "int", nullable: false),
                    SystemQty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStockOpnameProductStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStockOpnameProductStorage_InvStockOpnameProduct_OpnameProductId",
                        column: x => x.OpnameProductId,
                        principalTable: "InvStockOpnameProduct",
                        principalColumn: "OpnameProductId");
                    table.ForeignKey(
                        name: "FK_InvStockOpnameProductStorage_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProductStorage_OpnameProductId",
                table: "InvStockOpnameProductStorage",
                column: "OpnameProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProductStorage_StorageCode",
                table: "InvStockOpnameProductStorage",
                column: "StorageCode");
        }
    }
}
