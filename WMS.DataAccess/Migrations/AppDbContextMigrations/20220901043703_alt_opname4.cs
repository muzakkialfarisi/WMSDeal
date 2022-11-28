using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alt_opname4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProductStorage_InvStockOpnameProduct_ProductId",
                table: "InvStockOpnameProductStorage");

            migrationBuilder.DropIndex(
                name: "IX_InvStockOpnameProductStorage_ProductId",
                table: "InvStockOpnameProductStorage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "InvStockOpnameProductStorage");

            migrationBuilder.AddColumn<string>(
                name: "OpnameProductId",
                table: "InvStockOpnameProductStorage",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpnameProductId",
                table: "InvStockOpnameProduct",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct",
                column: "OpnameProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProductStorage_OpnameProductId",
                table: "InvStockOpnameProductStorage",
                column: "OpnameProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProduct_ProductId",
                table: "InvStockOpnameProduct",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProductStorage_InvStockOpnameProduct_OpnameProductId",
                table: "InvStockOpnameProductStorage",
                column: "OpnameProductId",
                principalTable: "InvStockOpnameProduct",
                principalColumn: "OpnameProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvStockOpnameProductStorage_InvStockOpnameProduct_OpnameProductId",
                table: "InvStockOpnameProductStorage");

            migrationBuilder.DropIndex(
                name: "IX_InvStockOpnameProductStorage_OpnameProductId",
                table: "InvStockOpnameProductStorage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropIndex(
                name: "IX_InvStockOpnameProduct_ProductId",
                table: "InvStockOpnameProduct");

            migrationBuilder.DropColumn(
                name: "OpnameProductId",
                table: "InvStockOpnameProductStorage");

            migrationBuilder.DropColumn(
                name: "OpnameProductId",
                table: "InvStockOpnameProduct");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "InvStockOpnameProductStorage",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvStockOpnameProduct",
                table: "InvStockOpnameProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStockOpnameProductStorage_ProductId",
                table: "InvStockOpnameProductStorage",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvStockOpnameProductStorage_InvStockOpnameProduct_ProductId",
                table: "InvStockOpnameProductStorage",
                column: "ProductId",
                principalTable: "InvStockOpnameProduct",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
