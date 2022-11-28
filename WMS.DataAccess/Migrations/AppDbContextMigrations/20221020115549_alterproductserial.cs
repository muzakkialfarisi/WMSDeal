using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alterproductserial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvStorageCode_BinCode",
                table: "InvStorageCode");

            migrationBuilder.RenameColumn(
                name: "ModifiedBy",
                table: "MasProductData",
                newName: "SerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageCode_BinCode",
                table: "InvStorageCode",
                column: "BinCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvStorageCode_BinCode",
                table: "InvStorageCode");

            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "MasProductData",
                newName: "ModifiedBy");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorageCode_BinCode",
                table: "InvStorageCode",
                column: "BinCode");
        }
    }
}
