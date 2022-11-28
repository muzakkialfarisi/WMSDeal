using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class Nullablesalesorderstorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderStorage_InvStorageCode_StorageCode",
                table: "OutSalesOrderStorage");

            migrationBuilder.AlterColumn<Guid>(
                name: "StorageCode",
                table: "OutSalesOrderStorage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderStorage_InvStorageCode_StorageCode",
                table: "OutSalesOrderStorage",
                column: "StorageCode",
                principalTable: "InvStorageCode",
                principalColumn: "StorageCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderStorage_InvStorageCode_StorageCode",
                table: "OutSalesOrderStorage");

            migrationBuilder.AlterColumn<Guid>(
                name: "StorageCode",
                table: "OutSalesOrderStorage",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderStorage_InvStorageCode_StorageCode",
                table: "OutSalesOrderStorage",
                column: "StorageCode",
                principalTable: "InvStorageCode",
                principalColumn: "StorageCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
