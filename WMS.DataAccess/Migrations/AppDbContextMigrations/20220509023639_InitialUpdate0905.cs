using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialUpdate0905 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvRelabeling",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DateRelabelled = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RelabelledBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvRelabeling", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvRelabeling_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvRepacking",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    DateRepacked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RepackedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvRepacking", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvRepacking_MasProductData_ProductId",
                        column: x => x.ProductId,
                        principalTable: "MasProductData",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvRelabeling_ProductId",
                table: "InvRelabeling",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvRepacking_ProductId",
                table: "InvRepacking",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvRelabeling");

            migrationBuilder.DropTable(
                name: "InvRepacking");
        }
    }
}
