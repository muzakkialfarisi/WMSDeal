using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class altertablereturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HouseCode",
                table: "InvReturn",
                type: "nvarchar(25)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_InvReturn_HouseCode",
                table: "InvReturn",
                column: "HouseCode");

            migrationBuilder.AddForeignKey(
                name: "FK_InvReturn_MasHouseCode_HouseCode",
                table: "InvReturn",
                column: "HouseCode",
                principalTable: "MasHouseCode",
                principalColumn: "HouseCode",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvReturn_MasHouseCode_HouseCode",
                table: "InvReturn");

            migrationBuilder.DropIndex(
                name: "IX_InvReturn_HouseCode",
                table: "InvReturn");

            migrationBuilder.DropColumn(
                name: "HouseCode",
                table: "InvReturn");
        }
    }
}
