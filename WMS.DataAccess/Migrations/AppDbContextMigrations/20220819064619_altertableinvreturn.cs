using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class altertableinvreturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "InvReturn",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_TenantId",
                table: "InvReturn",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_MasDataTenant_TenantId",
                table: "InvReturn",
                column: "TenantId",
                principalTable: "MasDataTenant",
                principalColumn: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_MasDataTenant_TenantId",
                table: "InvReturn");

            migrationBuilder.DropIndex(
                name: "IX_InvReturn_TenantId",
                table: "InvReturn");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "InvReturn");
        }
    }
}
