using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class addtablearrivalproduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncDeliveryOrderArrivalProduct",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DOProductId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncDeliveryOrderArrivalProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrderArrivalProduct_IncDeliveryOrderArrival_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderArrival",
                        principalColumn: "DOProductId");
                    table.ForeignKey(
                        name: "FK_IncDeliveryOrderArrivalProduct_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrderArrivalProduct_DOProductId",
                table: "IncDeliveryOrderArrivalProduct",
                column: "DOProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncDeliveryOrderArrivalProduct_ProductId",
                table: "IncDeliveryOrderArrivalProduct",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncDeliveryOrderArrivalProduct");
        }
    }
}
