using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alt_opnamepro1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.RenameColumn(
                name: "InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct",
                newName: "OpnameId");

            migrationBuilder.RenameIndex(
                name: "IX_InvStockOpnameProduct_InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct",
                newName: "IX_InvStockOpnameProduct_OpnameId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_OpnameId",
                table: "InvStockOpnameProduct",
                column: "OpnameId",
                principalTable: "InvStockOpname",
                principalColumn: "OpnameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_OpnameId",
                table: "InvStockOpnameProduct");

            migrationBuilder.RenameColumn(
                name: "OpnameId",
                table: "InvStockOpnameProduct",
                newName: "InvStockOpnameOpnameId");

            migrationBuilder.RenameIndex(
                name: "IX_InvStockOpnameProduct_OpnameId",
                table: "InvStockOpnameProduct",
                newName: "IX_InvStockOpnameProduct_InvStockOpnameOpnameId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProduct_InvStockOpname_InvStockOpnameOpnameId",
                table: "InvStockOpnameProduct",
                column: "InvStockOpnameOpnameId",
                principalTable: "InvStockOpname",
                principalColumn: "OpnameId");
        }
    }
}
