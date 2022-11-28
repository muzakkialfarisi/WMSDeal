using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialUpdate1404 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DOProductCode",
                table: "IncDeliveryOrderProduct",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "InvPickingRoute",
                columns: table => new
                {
                    RouteCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Log = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPickingRoute", x => x.RouteCode);
                    table.ForeignKey(
                        name: "FK_InvPickingRoute_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvPickingRouteColumn",
                columns: table => new
                {
                    RouteColumn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    ColumnCode = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPickingRouteColumn", x => x.RouteColumn);
                    table.ForeignKey(
                        name: "FK_InvPickingRouteColumn_InvPickingRoute_RouteCode",
                        column: x => x.RouteCode,
                        principalTable: "InvPickingRoute",
                        principalColumn: "RouteCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvPickingRouteColumn_InvStorageColumn_ColumnCode",
                        column: x => x.ColumnCode,
                        principalTable: "InvStorageColumn",
                        principalColumn: "ColumnCode");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvPickingRoute_HouseCode",
                table: "InvPickingRoute",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvPickingRouteColumn_ColumnCode",
                table: "InvPickingRouteColumn",
                column: "ColumnCode");

            migrationBuilder.CreateIndex(
                name: "IX_InvPickingRouteColumn_RouteCode",
                table: "InvPickingRouteColumn",
                column: "RouteCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvPickingRouteColumn");

            migrationBuilder.DropTable(
                name: "InvPickingRoute");

            migrationBuilder.DropColumn(
                name: "DOProductCode",
                table: "IncDeliveryOrderProduct");
        }
    }
}
