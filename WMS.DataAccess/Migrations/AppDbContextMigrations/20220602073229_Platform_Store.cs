using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class Platform_Store : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "OutSalesOrderAssign",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "MasPlatform",
                columns: table => new
                {
                    PlatformId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasPlatform", x => x.PlatformId);
                });

            migrationBuilder.CreateTable(
                name: "MasStore",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PlatformId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    KelId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    KodePos = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasStore", x => x.StoreId);
                    table.ForeignKey(
                        name: "FK_MasStore_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasStore_MasKelurahan_KelId",
                        column: x => x.KelId,
                        principalTable: "MasKelurahan",
                        principalColumn: "KelId");
                    table.ForeignKey(
                        name: "FK_MasStore_MasPlatform_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "MasPlatform",
                        principalColumn: "PlatformId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderAssign_UserId",
                table: "OutSalesOrderAssign",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MasStore_KelId",
                table: "MasStore",
                column: "KelId");

            migrationBuilder.CreateIndex(
                name: "IX_MasStore_PlatformId",
                table: "MasStore",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_MasStore_TenantId",
                table: "MasStore",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderAssign_SecUser_UserId",
                table: "OutSalesOrderAssign",
                column: "UserId",
                principalTable: "SecUser",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderAssign_SecUser_UserId",
                table: "OutSalesOrderAssign");

            migrationBuilder.DropTable(
                name: "MasStore");

            migrationBuilder.DropTable(
                name: "MasPlatform");

            migrationBuilder.DropIndex(
                name: "IX_OutSalesOrderAssign_UserId",
                table: "OutSalesOrderAssign");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "OutSalesOrderAssign",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
