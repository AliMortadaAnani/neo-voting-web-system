using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovernmentSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class v7_1_add_RegisteredUsername_StringFieldNullable_For_Voters_Candidates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegisteredUsername",
                table: "Voters",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredUsername",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredUsername",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "RegisteredUsername",
                table: "Candidates");
        }
    }
}
