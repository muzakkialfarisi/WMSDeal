using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alt_opnamepro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_OpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProduct_InvStorageCode_StorageCode",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProduct_MasProductData_ProductId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropIndex(
                name: "IX_InvStockOpnameProduct_OpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropIndex(
                name: "IX_InvStockOpnameProduct_ProductId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropIndex(
                name: "IX_InvStockOpnameProduct_StorageCode",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "BrokenDescription",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "BrokenQty",
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
                name: "OpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "StockQty",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "StorageCode",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "SystemQty",
                table: "InvStockOpnameProduct");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProduct_InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct",
                column: "InvStockOpnameOpnameId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct",
                column: "InvStockOpnameOpnameId",
                principalTable: "InvStockOpname",
                principalColumn: "OpnameId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProduct_MasProductData_ProductId",
                table: "InvStockOpnameProduct",
                column: "ProductId",
                principalTable: "MasProductData",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProduct_MasProductData_ProductId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropIndex(
                name: "IX_InvStockOpnameProduct_InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

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
                name: "OpnameId",
                table: "InvStockOpnameProduct",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StockQty",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "StorageCode",
                table: "InvStockOpnameProduct",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "SystemQty",
                table: "InvStockOpnameProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_OpnameId",
                table: "InvStockOpnameProduct",
                column: "OpnameId",
                principalTable: "InvStockOpname",
                principalColumn: "OpnameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProduct_InvStorageCode_StorageCode",
                table: "InvStockOpnameProduct",
                column: "StorageCode",
                principalTable: "InvStorageCode",
                principalColumn: "StorageCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProduct_MasProductData_ProductId",
                table: "InvStockOpnameProduct",
                column: "ProductId",
                principalTable: "MasProductData",
                principalColumn: "ProductId");
        }
    }
}
