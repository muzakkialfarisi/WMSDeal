using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class newserialnumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IncSerialNumber",
                columns: table => new
                {
                    SerialId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DOProductId = table.Column<int>(type: "int", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncSerialNumber", x => x.SerialId);
                    table.ForeignKey(
                        name: "FK_IncSerialNumber_IncDeliveryOrderProduct_DOProductId",
                        column: x => x.DOProductId,
                        principalTable: "IncDeliveryOrderProduct",
                        principalColumn: "DOProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncSerialNumber_DOProductId",
                table: "IncSerialNumber",
                column: "DOProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncSerialNumber");
        }
    }
}
