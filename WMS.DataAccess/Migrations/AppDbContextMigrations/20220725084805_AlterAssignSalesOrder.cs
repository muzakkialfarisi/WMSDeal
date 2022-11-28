using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class AlterAssignSalesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PickAssignId",
                table: "OutSalesOrderAssign",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasInvoicing_TenantId",
                table: "MasInvoicing",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_MasInvoicing_MasDataTenant_TenantId",
                table: "MasInvoicing",
                column: "TenantId",
                principalTable: "MasDataTenant",
                principalColumn: "TenantId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasInvoicing_MasDataTenant_TenantId",
                table: "MasInvoicing");

            migrationBuilder.DropIndex(
                name: "IX_MasInvoicing_TenantId",
                table: "MasInvoicing");

            migrationBuilder.DropColumn(
                name: "PickAssignId",
                table: "OutSalesOrderAssign");
        }
    }
}
