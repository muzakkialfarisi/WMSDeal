using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dualcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvSalesOrderPick_OutSalesOrderProduct_OrdProductId",
                table: "InvSalesOrderPick");

            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderProduct_OutSalesOrder_OrderId",
                table: "OutSalesOrderProduct");

            migrationBuilder.DropIndex(
                name: "IX_SecUserTenant_UserId",
                table: "SecUserTenant");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "OutSalesOrderProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "OrdProductId",
                table: "InvSalesOrderPick",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_SecUserTenant_UserId",
                table: "SecUserTenant",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvSalesOrderPick_OutSalesOrderProduct_OrdProductId",
                table: "InvSalesOrderPick",
                column: "OrdProductId",
                principalTable: "OutSalesOrderProduct",
                principalColumn: "OrdProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderProduct_OutSalesOrder_OrderId",
                table: "OutSalesOrderProduct",
                column: "OrderId",
                principalTable: "OutSalesOrder",
                principalColumn: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvSalesOrderPick_OutSalesOrderProduct_OrdProductId",
                table: "InvSalesOrderPick");

            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderProduct_OutSalesOrder_OrderId",
                table: "OutSalesOrderProduct");

            migrationBuilder.DropIndex(
                name: "IX_SecUserTenant_UserId",
                table: "SecUserTenant");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "OutSalesOrderProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OrdProductId",
                table: "InvSalesOrderPick",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecUserTenant_UserId",
                table: "SecUserTenant",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvSalesOrderPick_OutSalesOrderProduct_OrdProductId",
                table: "InvSalesOrderPick",
                column: "OrdProductId",
                principalTable: "OutSalesOrderProduct",
                principalColumn: "OrdProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderProduct_OutSalesOrder_OrderId",
                table: "OutSalesOrderProduct",
                column: "OrderId",
                principalTable: "OutSalesOrder",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
