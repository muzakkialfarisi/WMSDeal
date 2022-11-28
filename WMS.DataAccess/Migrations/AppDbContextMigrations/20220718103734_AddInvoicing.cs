using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class AddInvoicing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InvPeriod",
                table: "MasInvoicing",
                newName: "InvoicePeriod");

            migrationBuilder.RenameColumn(
                name: "InvNumber",
                table: "MasInvoicing",
                newName: "InvoiceNumber");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "MasInvoicing",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MasInvoicingDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    CostName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CostItem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CostTotal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CostAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostGrandAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasInvoicingDetail", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasInvoicingDetail");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "MasInvoicing");

            migrationBuilder.RenameColumn(
                name: "InvoicePeriod",
                table: "MasInvoicing",
                newName: "InvPeriod");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "MasInvoicing",
                newName: "InvNumber");
        }
    }
}
