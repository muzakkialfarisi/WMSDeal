using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialUpdate23042 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QtyOrder",
                table: "InvStorageCode",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QtyStock",
                table: "InvProductPutaway",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InvSalesOrderPick",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdProductId = table.Column<int>(type: "int", nullable: false),
                    StorageCode = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QtyPick = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvSalesOrderPick", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvSalesOrderPick_InvStorageCode_StorageCode",
                        column: x => x.StorageCode,
                        principalTable: "InvStorageCode",
                        principalColumn: "StorageCode",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_InvSalesOrderPick_OutSalesOrderProduct_OrdProductId",
                        column: x => x.OrdProductId,
                        principalTable: "OutSalesOrderProduct",
                        principalColumn: "OrdProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvSalesOrderPick_OrdProductId",
                table: "InvSalesOrderPick",
                column: "OrdProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSalesOrderPick_StorageCode",
                table: "InvSalesOrderPick",
                column: "StorageCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvSalesOrderPick");

            migrationBuilder.DropColumn(
                name: "QtyOrder",
                table: "InvStorageCode");

            migrationBuilder.DropColumn(
                name: "QtyStock",
                table: "InvProductPutaway");
        }
    }
}
