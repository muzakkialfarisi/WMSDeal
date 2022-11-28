using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class AlterOutSalesOrderStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderPick_PickId",
                table: "OutSalesOrderPack");

            migrationBuilder.DropTable(
                name: "OutSalesOrderPick");

            migrationBuilder.RenameColumn(
                name: "PickId",
                table: "OutSalesOrderPack",
                newName: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "DatePicked",
                table: "OutSalesOrderStorage",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateQualityChecked",
                table: "OutSalesOrderStorage",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickedBy",
                table: "OutSalesOrderStorage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickedStatus",
                table: "OutSalesOrderStorage",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QualityCheckedBy",
                table: "OutSalesOrderStorage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QualityCheckedRemark",
                table: "OutSalesOrderStorage",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QualityCheckedStatus",
                table: "OutSalesOrderStorage",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderStorage_Id",
                table: "OutSalesOrderPack",
                column: "Id",
                principalTable: "OutSalesOrderStorage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderStorage_Id",
                table: "OutSalesOrderPack");

            migrationBuilder.DropColumn(
                name: "DatePicked",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropColumn(
                name: "DateQualityChecked",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropColumn(
                name: "PickedBy",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropColumn(
                name: "PickedStatus",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropColumn(
                name: "QualityCheckedBy",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropColumn(
                name: "QualityCheckedRemark",
                table: "OutSalesOrderStorage");

            migrationBuilder.DropColumn(
                name: "QualityCheckedStatus",
                table: "OutSalesOrderStorage");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OutSalesOrderPack",
                newName: "PickId");

            migrationBuilder.CreateTable(
                name: "OutSalesOrderPick",
                columns: table => new
                {
                    PickId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrdProductId = table.Column<int>(type: "int", nullable: true),
                    CheckedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateChecked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatePicked = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrdQty = table.Column<int>(type: "int", nullable: false),
                    PickedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quality = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderPick", x => x.PickId);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderPick_IncItemProduct_IKU",
                        column: x => x.IKU,
                        principalTable: "IncItemProduct",
                        principalColumn: "IKU");
                    table.ForeignKey(
                        name: "FK_OutSalesOrderPick_OutSalesOrderProduct_OrdProductId",
                        column: x => x.OrdProductId,
                        principalTable: "OutSalesOrderProduct",
                        principalColumn: "OrdProductId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderPick_IKU",
                table: "OutSalesOrderPick",
                column: "IKU");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderPick_OrdProductId",
                table: "OutSalesOrderPick",
                column: "OrdProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrderPack_OutSalesOrderPick_PickId",
                table: "OutSalesOrderPack",
                column: "PickId",
                principalTable: "OutSalesOrderPick",
                principalColumn: "PickId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
