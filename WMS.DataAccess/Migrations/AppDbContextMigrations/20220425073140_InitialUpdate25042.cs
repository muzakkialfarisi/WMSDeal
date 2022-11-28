using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialUpdate25042 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StorageCode",
                table: "OutSalesOrderStorage",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderStorage_StorageCode",
                table: "OutSalesOrderStorage",
                column: "StorageCode");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderStorage_InvStorageCode_StorageCode",
                table: "OutSalesOrderStorage",
                column: "StorageCode",
                principalTable: "InvStorageCode",
                principalColumn: "StorageCode",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderStorage_InvStorageCode_StorageCode",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropIndex(
                name: "IX_OutSalesOrderStorage_StorageCode",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropColumn(
                name: "StorageCode",
                table: "OutSalesOrderStorage");
        }
    }
}
