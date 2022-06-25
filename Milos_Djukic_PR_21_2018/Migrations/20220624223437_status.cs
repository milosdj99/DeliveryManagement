using Microsoft.EntityFrameworkCore.Migrations;

namespace Milos_Djukic_PR_21_2018.Migrations
{
    public partial class status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Deliverers");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Deliverers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Deliverers");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Deliverers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
