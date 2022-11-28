using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class productbundling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MasProductBundling",
                columns: table => new
                {
                    BundlingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateUdated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasProductBundling", x => x.BundlingId);
                    table.ForeignKey(
                        name: "FK_MasProductBundling_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasProductBundlingData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    BundlingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasProductBundlingData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MasProductBundlingData_MasProductBundling_BundlingId",
                        column: x => x.BundlingId,
                        principalTable: "MasProductBundling",
                        principalColumn: "BundlingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MasProductBundlingData_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MasProductBundling_TenantId",
                table: "MasProductBundling",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MasProductBundlingData_BundlingId",
                table: "MasProductBundlingData",
                column: "BundlingId");

            migrationBuilder.CreateIndex(
                name: "IX_MasProductBundlingData_ProductId",
                table: "MasProductBundlingData",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasProductBundlingData");

            migrationBuilder.DropTable(
                name: "MasProductBundling");
        }
    }
}
