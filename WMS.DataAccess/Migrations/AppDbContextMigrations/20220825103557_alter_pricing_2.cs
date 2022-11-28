using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alter_pricing_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "StorageMin",
                table: "MasPricing",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "ReceivingFeeMin",
                table: "MasPricing",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<float>(
                name: "PPh",
                table: "MasPricing",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "ManagementFee",
                table: "MasPricing",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "CostTotal",
                table: "MasInvoicingDetail",
                type: "real",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<string>(
                name: "MasInvoicingInvoiceNumber",
                table: "MasInvoicingDetail",
                type: "nvarchar(25)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasInvoicingDetail_MasInvoicingInvoiceNumber",
                table: "MasInvoicingDetail",
                column: "MasInvoicingInvoiceNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_MasInvoicingDetail_MasInvoicing_MasInvoicingInvoiceNumber",
                table: "MasInvoicingDetail",
                column: "MasInvoicingInvoiceNumber",
                principalTable: "MasInvoicing",
                principalColumn: "InvoiceNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasInvoicingDetail_MasInvoicing_MasInvoicingInvoiceNumber",
                table: "MasInvoicingDetail");

            migrationBuilder.DropIndex(
                name: "IX_MasInvoicingDetail_MasInvoicingInvoiceNumber",
                table: "MasInvoicingDetail");

            migrationBuilder.DropColumn(
                name: "MasInvoicingInvoiceNumber",
                table: "MasInvoicingDetail");

            migrationBuilder.AlterColumn<int>(
                name: "StorageMin",
                table: "MasPricing",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "ReceivingFeeMin",
                table: "MasPricing",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "PPh",
                table: "MasPricing",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "ManagementFee",
                table: "MasPricing",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<string>(
                name: "CostTotal",
                table: "MasInvoicingDetail",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real",
                oldMaxLength: 150);
        }
    }
}
