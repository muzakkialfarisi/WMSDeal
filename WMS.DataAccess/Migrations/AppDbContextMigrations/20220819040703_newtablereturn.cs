using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class newtablereturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvReturn",
                columns: table => new
                {
                    ReturnNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReturnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BeautyPicture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvReturn", x => x.ReturnNumber);
                });

            migrationBuilder.CreateTable(
                name: "InvReturnProduct",
                columns: table => new
                {
                    ReturnProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrdProductId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    SKU = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfExpired = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<float>(type: "real", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CloseUpPicture = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvReturnProduct", x => x.ReturnProductId);
                    table.ForeignKey(
                        name: "FK_InvReturnProduct_InvReturn_ReturnNumber",
                        column: x => x.ReturnNumber,
                        principalTable: "InvReturn",
                        principalColumn: "ReturnNumber",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvReturnProduct_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_InvReturnProduct_OutSalesOrderProduct_OrdProductId",
                        column: x => x.OrdProductId,
                        principalTable: "OutSalesOrderProduct",
                        principalColumn: "OrdProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvReturnProduct_OrdProductId",
                table: "InvReturnProduct",
                column: "OrdProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturnProduct_ProductId",
                table: "InvReturnProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturnProduct_ReturnNumber",
                table: "InvReturnProduct",
                column: "ReturnNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvReturnProduct");

            migrationBuilder.DropTable(
                name: "InvReturn");
        }
    }
}
