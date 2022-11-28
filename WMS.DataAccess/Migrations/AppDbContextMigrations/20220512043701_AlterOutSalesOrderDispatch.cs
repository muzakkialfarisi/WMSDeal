using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class AlterOutSalesOrderDispatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OutSalesDispatchtoCourier_OrdCourier",
                table: "OutSalesDispatchtoCourier",
                column: "OrdCourier");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesDispatchtoCourier_MasSalesCourier_OrdCourier",
                table: "OutSalesDispatchtoCourier",
                column: "OrdCourier",
                principalTable: "MasSalesCourier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesDispatchtoCourier_MasSalesCourier_OrdCourier",
                table: "OutSalesDispatchtoCourier");

            migrationBuilder.DropIndex(
                name: "IX_OutSalesDispatchtoCourier_OrdCourier",
                table: "OutSalesDispatchtoCourier");
        }
    }
}
