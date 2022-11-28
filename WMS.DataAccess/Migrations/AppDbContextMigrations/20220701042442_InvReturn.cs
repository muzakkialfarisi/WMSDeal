using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InvReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QtyReturn",
                table: "InvProductStock",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InvReturn",
                columns: table => new
                {
                    ReturnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateInReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeautyPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloseUpPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOutReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvReturn", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_InvReturn_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvReturn_MasProductData_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_StorageCode",
                table: "InvReturn",
                column: "StorageCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_TenantId",
                table: "InvReturn",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvReturn");

            migrationBuilder.DropColumn(
                name: "QtyReturn",
                table: "InvProductStock");
        }
    }
}
