using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class secusertenant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SecUserTenant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Flag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecUserTenant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecUserTenant_MasDataTenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "MasDataTenant",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecUserTenant_SecUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SecUser",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SecUserTenant_TenantId",
                table: "SecUserTenant",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_SecUserTenant_UserId",
                table: "SecUserTenant",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SecUserTenant");
        }
    }
}
