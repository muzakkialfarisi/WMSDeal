using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class OutSalesOrderAssign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutSalesOrderAssign",
                columns: table => new
                {
                    OrderId = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateStaged = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false),
                    ImageStaged = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderAssign", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderAssign_OutSalesOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutSalesOrder",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutSalesOrderAssign");
        }
    }
}
