using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class IncDOProductSerial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_IncDOProductSerial_DOProductId",
                table: "IncDOProductSerial",
                column: "DOProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncDOProductSerial");
        }
    }
}
