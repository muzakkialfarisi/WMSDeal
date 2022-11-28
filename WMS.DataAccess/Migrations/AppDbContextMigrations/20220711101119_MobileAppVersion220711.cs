using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class MobileAppVersion220711 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncDOProductSerial");

            migrationBuilder.DropTable(
                name: "InvReturn");

            migrationBuilder.DropColumn(
                name: "QtyReturn",
                table: "InvProductStock");

            migrationBuilder.AlterColumn<string>(
                name: "HistoryType",
                table: "InvProductHistory",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QtyReturn",
                table: "InvProductStock",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "HistoryType",
                table: "InvProductHistory",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "IncDOProductSerial",
                columns: table => new
                {
                    SerialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DOProductId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncDOProductSerial", x => x.SerialId);
                    table.ForeignKey(
                        name: "FK_IncDOProductSerial_IncDeliveryOrderProduct_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderProduct",
                        principalColumn: "DOProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvReturn",
                columns: table => new
                {
                    ReturnId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DOProductId = table.Column<int>(type: "int", nullable: true),
                    IKU = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    OrdProductId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BeautyPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CloseUpPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateInReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOutReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpnameProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReturnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvReturn", x => x.ReturnId);
                    table.ForeignKey(
                        name: "FK_InvReturn_IncDeliveryOrderProduct_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderProduct",
                        principalColumn: "DOProductId");
                    table.ForeignKey(
                        name: "FK_InvReturn_IncItemProduct_IKU",
                        column: x => x.IKU,
                        principalTable: "IncItemProduct",
                        principalColumn: "IKU");
                    table.ForeignKey(
                        name: "FK_InvReturn_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode");
                    table.ForeignKey(
                        name: "FK_InvReturn_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvReturn_OutSalesOrderProduct_OrdProductId",
                        column: x => x.OrdProductId,
                        principalTable: "OutSalesOrderProduct",
                        principalColumn: "OrdProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncDOProductSerial_DOProductId",
                table: "IncDOProductSerial",
                column: "DOProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_DOProductId",
                table: "InvReturn",
                column: "DOProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_IKU",
                table: "InvReturn",
                column: "IKU");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_OrdProductId",
                table: "InvReturn",
                column: "OrdProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_ProductId",
                table: "InvReturn",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_StorageCode",
                table: "InvReturn",
                column: "StorageCode");
        }
    }
}
