using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alterOutSOPack220613 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderStorage_Id",
                table: "OutSalesOrderPack");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OutSalesOrderPack",
                newName: "OrdProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderProduct_OrdProductId",
                table: "OutSalesOrderPack",
                column: "OrdProductId",
                principalTable: "OutSalesOrderProduct",
                principalColumn: "OrdProductId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderProduct_OrdProductId",
                table: "OutSalesOrderPack");

            migrationBuilder.RenameColumn(
                name: "OrdProductId",
                table: "OutSalesOrderPack",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderStorage_Id",
                table: "OutSalesOrderPack",
                column: "Id",
                principalTable: "OutSalesOrderStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
