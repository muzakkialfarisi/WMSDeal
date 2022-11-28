using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialUpdate1105 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutSalesDispatchtoCourier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrdCourier = table.Column<int>(type: "int", nullable: false),
                    CourierName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PhotoHandOver = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DatedHandOvered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandoveredBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesDispatchtoCourier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutSalesDispatchtoCourier_OutSalesOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutSalesOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesDispatchtoCourier_OrderId",
                table: "OutSalesDispatchtoCourier",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutSalesDispatchtoCourier");
        }
    }
}
