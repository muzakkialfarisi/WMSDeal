using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS.DataAccess.Migrations
{
    public partial class alter_arrival : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NoteNotArrived",
                table: "IncDeliveryOrderArrival",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QtyNotArrived",
                table: "IncDeliveryOrderArrival",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoteNotArrived",
                table: "IncDeliveryOrderArrival");

            migrationBuilder.DropColumn(
                name: "QtyNotArrived",
                table: "IncDeliveryOrderArrival");
        }
    }
}
