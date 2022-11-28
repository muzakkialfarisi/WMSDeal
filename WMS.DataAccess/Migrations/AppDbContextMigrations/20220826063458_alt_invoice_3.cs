using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alt_invoice_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatePutedAway",
                table: "IncItemProduct",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MasInvoicingDetail_InvoiceNumber",
                table: "MasInvoicingDetail",
                column: "InvoiceNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_MasInvoicingDetail_MasInvoicing_InvoiceNumber",
                table: "MasInvoicingDetail",
                column: "InvoiceNumber",
                principalTable: "MasInvoicing",
                principalColumn: "InvoiceNumber",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasInvoicingDetail_MasInvoicing_InvoiceNumber",
                table: "MasInvoicingDetail");

            migrationBuilder.DropIndex(
                name: "IX_MasInvoicingDetail_InvoiceNumber",
                table: "MasInvoicingDetail");

            migrationBuilder.AddColumn<string>(
                name: "MasInvoicingInvoiceNumber",
                table: "MasInvoicingDetail",
                type: "nvarchar(25)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DatePutedAway",
                table: "IncItemProduct",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

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
    }
}
