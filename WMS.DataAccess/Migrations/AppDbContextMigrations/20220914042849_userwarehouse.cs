using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class userwarehouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecUserWarehouse",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HouseCode = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecUserWarehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecUserWarehouse_MasHouseCode_HouseCode",
                        column: x => x.HouseCode,
                        principalTable: "MasHouseCode",
                        principalColumn: "HouseCode");
                    table.ForeignKey(
                        name: "FK_SecUserWarehouse_SecUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SecUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecUserWarehouse_HouseCode",
                table: "SecUserWarehouse",
                column: "HouseCode");

            migrationBuilder.CreateIndex(
                name: "IX_SecUserWarehouse_UserId",
                table: "SecUserWarehouse",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecUserWarehouse");
        }
    }
}
