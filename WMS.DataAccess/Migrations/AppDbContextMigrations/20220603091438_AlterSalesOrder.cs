using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class AlterSalesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrder_MasSalesPlatform_OrdPlatform",
                table: "OutSalesOrder");

            migrationBuilder.DropTable(
                name: "MasSalesPlatform");

            migrationBuilder.RenameColumn(
                name: "OrdPlatform",
                table: "OutSalesOrder",
                newName: "PlatformId");

            migrationBuilder.RenameIndex(
                name: "IX_OutSalesOrder_OrdPlatform",
                table: "OutSalesOrder",
                newName: "IX_OutSalesOrder_PlatformId");

            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "OutSalesOrder",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StoreName",
                table: "OutSalesOrder",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrder_StoreId",
                table: "OutSalesOrder",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrder_MasPlatform_PlatformId",
                table: "OutSalesOrder",
                column: "PlatformId",
                principalTable: "MasPlatform",
                principalColumn: "PlatformId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrder_MasStore_StoreId",
                table: "OutSalesOrder",
                column: "StoreId",
                principalTable: "MasStore",
                principalColumn: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrder_MasPlatform_PlatformId",
                table: "OutSalesOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_OutSalesOrder_MasStore_StoreId",
                table: "OutSalesOrder");

            migrationBuilder.DropIndex(
                name: "IX_OutSalesOrder_StoreId",
                table: "OutSalesOrder");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "OutSalesOrder");

            migrationBuilder.DropColumn(
                name: "StoreName",
                table: "OutSalesOrder");

            migrationBuilder.RenameColumn(
                name: "PlatformId",
                table: "OutSalesOrder",
                newName: "OrdPlatform");

            migrationBuilder.RenameIndex(
                name: "IX_OutSalesOrder_PlatformId",
                table: "OutSalesOrder",
                newName: "IX_OutSalesOrder_OrdPlatform");

            migrationBuilder.CreateTable(
                name: "MasSalesPlatform",
                columns: table => new
                {
                    SplId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SplCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    SplName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasSalesPlatform", x => x.SplId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_OutSalesOrder_MasSalesPlatform_OrdPlatform",
                table: "OutSalesOrder",
                column: "OrdPlatform",
                principalTable: "MasSalesPlatform",
                principalColumn: "SplId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
