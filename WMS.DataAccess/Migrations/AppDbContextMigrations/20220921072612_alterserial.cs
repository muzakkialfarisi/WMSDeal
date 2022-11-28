using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alterserial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncSerialNumber_IncDeliveryOrderProduct_DOProductId",
                table: "IncSerialNumber");

            migrationBuilder.AlterColumn<int>(
                name: "DOProductId",
                table: "IncSerialNumber",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "OrdProductId",
                table: "IncSerialNumber",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "IncSerialNumber",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncSerialNumber_OrdProductId",
                table: "IncSerialNumber",
                column: "OrdProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncSerialNumber_ProductId",
                table: "IncSerialNumber",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncSerialNumber_IncDeliveryOrderProduct_DOProductId",
                table: "IncSerialNumber",
                column: "DOProductId",
                principalTable: "IncDeliveryOrderProduct",
                principalColumn: "DOProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncSerialNumber_MasProductData_ProductId",
                table: "IncSerialNumber",
                column: "ProductId",
                principalTable: "MasProductData",
                principalColumn: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncSerialNumber_OutSalesOrderProduct_OrdProductId",
                table: "IncSerialNumber",
                column: "OrdProductId",
                principalTable: "OutSalesOrderProduct",
                principalColumn: "OrdProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncSerialNumber_IncDeliveryOrderProduct_DOProductId",
                table: "IncSerialNumber");

            migrationBuilder.DropForeignKey(
                name: "FK_IncSerialNumber_MasProductData_ProductId",
                table: "IncSerialNumber");

            migrationBuilder.DropForeignKey(
                name: "FK_IncSerialNumber_OutSalesOrderProduct_OrdProductId",
                table: "IncSerialNumber");

            migrationBuilder.DropIndex(
                name: "IX_IncSerialNumber_OrdProductId",
                table: "IncSerialNumber");

            migrationBuilder.DropIndex(
                name: "IX_IncSerialNumber_ProductId",
                table: "IncSerialNumber");

            migrationBuilder.DropColumn(
                name: "OrdProductId",
                table: "IncSerialNumber");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "IncSerialNumber");

            migrationBuilder.AlterColumn<int>(
                name: "DOProductId",
                table: "IncSerialNumber",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IncSerialNumber_IncDeliveryOrderProduct_DOProductId",
                table: "IncSerialNumber",
                column: "DOProductId",
                principalTable: "IncDeliveryOrderProduct",
                principalColumn: "DOProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
