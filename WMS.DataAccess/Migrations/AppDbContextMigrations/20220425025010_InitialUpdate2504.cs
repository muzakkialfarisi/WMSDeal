using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class InitialUpdate2504 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutSalesOrderStorage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdProductId = table.Column<int>(type: "int", nullable: false),
                    IKU = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    IdPutAway = table.Column<int>(type: "int", nullable: true),
                    QtyPick = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutSalesOrderStorage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutSalesOrderStorage_IncItemProduct_IKU",
                        column: x => x.IKU,
                        principalTable: "IncItemProduct",
                        principalColumn: "IKU");
                    table.ForeignKey(
                        name: "FK_OutSalesOrderStorage_InvProductPutaway_IdPutAway",
                        column: x => x.IdPutAway,
                        principalTable: "InvProductPutaway",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutSalesOrderStorage_OutSalesOrderProduct_OrdProductId",
                        column: x => x.OrdProductId,
                        principalTable: "OutSalesOrderProduct",
                        principalColumn: "OrdProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderStorage_IdPutAway",
                table: "OutSalesOrderStorage",
                column: "IdPutAway");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderStorage_IKU",
                table: "OutSalesOrderStorage",
                column: "IKU");

            migrationBuilder.CreateIndex(
                name: "IX_OutSalesOrderStorage_OrdProductId",
                table: "OutSalesOrderStorage",
                column: "OrdProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutSalesOrderStorage");
        }
    }
}
