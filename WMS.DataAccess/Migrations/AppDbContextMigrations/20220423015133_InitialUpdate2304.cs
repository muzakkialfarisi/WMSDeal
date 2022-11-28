using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialUpdate2304 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvPickingRouteColumn_InvStorageColumn_ColumnCode",
                table: "InvPickingRouteColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_InvProductPutaway_IncDeliveryOrderProduct_DOProductId",
                table: "InvProductPutaway");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "OutSalesOrderProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Flag",
                table: "OutSalesOrderProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "OutsalesOrderDelivery",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "OutSalesOrderCustomer",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "OutSalesOrderConsignee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "OrderId",
                table: "OutSalesOrder",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "FlagApi",
                table: "OutSalesOrder",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QtyOrder",
                table: "InvProductStock",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "RouteCode",
                table: "InvPickingRouteColumn",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "ColumnCode",
                table: "InvPickingRouteColumn",
                type: "nvarchar(20)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RouteCode",
                table: "InvPickingRoute",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(5)",
                oldMaxLength: 5);

            migrationBuilder.AddForeignKey(
                name: "FK_InvPickingRouteColumn_InvStorageColumn_ColumnCode",
                table: "InvPickingRouteColumn",
                column: "ColumnCode",
                principalTable: "InvStorageColumn",
                principalColumn: "ColumnCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InvProductPutaway_IncDeliveryOrderArrival_DOProductId",
                table: "InvProductPutaway",
                column: "DOProductId",
                principalTable: "IncDeliveryOrderArrival",
                principalColumn: "DOProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderProduct_OutSalesOrder_OrderId",
                table: "OutSalesOrderProduct",
                column: "OrderId",
                principalTable: "OutSalesOrder",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvPickingRouteColumn_InvStorageColumn_ColumnCode",
                table: "InvPickingRouteColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_InvProductPutaway_IncDeliveryOrderArrival_DOProductId",
                table: "InvProductPutaway");

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "OutSalesOrderProduct");

            migrationBuilder.DropColumn(
                name: "FlagApi",
                table: "OutSalesOrder");

            migrationBuilder.DropColumn(
                name: "QtyOrder",
                table: "InvProductStock");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "OutSalesOrderProduct",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "OutsalesOrderDelivery",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "OutSalesOrderCustomer",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "OutSalesOrderConsignee",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "OutSalesOrder",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "RouteCode",
                table: "InvPickingRouteColumn",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ColumnCode",
                table: "InvPickingRouteColumn",
                type: "nvarchar(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)");

            migrationBuilder.AlterColumn<string>(
                name: "RouteCode",
                table: "InvPickingRoute",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_InvPickingRouteColumn_InvStorageColumn_ColumnCode",
                table: "InvPickingRouteColumn",
                column: "ColumnCode",
                principalTable: "InvStorageColumn",
                principalColumn: "ColumnCode");

            migrationBuilder.AddForeignKey(
                name: "FK_InvProductPutaway_IncDeliveryOrderProduct_DOProductId",
                table: "InvProductPutaway",
                column: "DOProductId",
                principalTable: "IncDeliveryOrderProduct",
                principalColumn: "DOProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderProduct_OutSalesOrder_OrderId",
                table: "OutSalesOrderProduct",
                column: "OrderId",
                principalTable: "OutSalesOrder",
                principalColumn: "OrderId");
        }
    }
}
