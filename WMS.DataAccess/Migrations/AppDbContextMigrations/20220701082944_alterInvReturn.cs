using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alterInvReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_InvStorageCode_StorageCode",
                table: "InvReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_MasProductData_TenantId",
                table: "InvReturn");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "InvReturn",
                newName: "OrdProductId");

            migrationBuilder.RenameIndex(
                name: "IX_InvReturn_TenantId",
                table: "InvReturn",
                newName: "IX_InvReturn_OrdProductId");

            migrationBuilder.AlterColumn<Guid>(
                name: "StorageCode",
                table: "InvReturn",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "DOProductId",
                table: "InvReturn",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IKU",
                table: "InvReturn",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OpnameProductId",
                table: "InvReturn",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_DOProductId",
                table: "InvReturn",
                column: "DOProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_IKU",
                table: "InvReturn",
                column: "IKU");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_ProductId",
                table: "InvReturn",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_IncDeliveryOrderProduct_DOProductId",
                table: "InvReturn",
                column: "DOProductId",
                principalTable: "IncDeliveryOrderProduct",
                principalColumn: "DOProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_IncItemProduct_IKU",
                table: "InvReturn",
                column: "IKU",
                principalTable: "IncItemProduct",
                principalColumn: "IKU");

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_InvStorageCode_StorageCode",
                table: "InvReturn",
                column: "StorageCode",
                principalTable: "InvStorageCode",
                principalColumn: "StorageCode");

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_MasProductData_ProductId",
                table: "InvReturn",
                column: "ProductId",
                principalTable: "MasProductData",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_OutSalesOrderProduct_OrdProductId",
                table: "InvReturn",
                column: "OrdProductId",
                principalTable: "OutSalesOrderProduct",
                principalColumn: "OrdProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_IncDeliveryOrderProduct_DOProductId",
                table: "InvReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_IncItemProduct_IKU",
                table: "InvReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_InvStorageCode_StorageCode",
                table: "InvReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_MasProductData_ProductId",
                table: "InvReturn");

            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_OutSalesOrderProduct_OrdProductId",
                table: "InvReturn");

            migrationBuilder.DropIndex(
                name: "IX_InvReturn_DOProductId",
                table: "InvReturn");

            migrationBuilder.DropIndex(
                name: "IX_InvReturn_IKU",
                table: "InvReturn");

            migrationBuilder.DropIndex(
                name: "IX_InvReturn_ProductId",
                table: "InvReturn");

            migrationBuilder.DropColumn(
                name: "DOProductId",
                table: "InvReturn");

            migrationBuilder.DropColumn(
                name: "IKU",
                table: "InvReturn");

            migrationBuilder.DropColumn(
                name: "OpnameProductId",
                table: "InvReturn");

            migrationBuilder.RenameColumn(
                name: "OrdProductId",
                table: "InvReturn",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_InvReturn_OrdProductId",
                table: "InvReturn",
                newName: "IX_InvReturn_TenantId");

            migrationBuilder.AlterColumn<Guid>(
                name: "StorageCode",
                table: "InvReturn",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_InvStorageCode_StorageCode",
                table: "InvReturn",
                column: "StorageCode",
                principalTable: "InvStorageCode",
                principalColumn: "StorageCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_MasProductData_TenantId",
                table: "InvReturn",
                column: "TenantId",
                principalTable: "MasProductData",
                principalColumn: "ProductId");
        }
    }
}
