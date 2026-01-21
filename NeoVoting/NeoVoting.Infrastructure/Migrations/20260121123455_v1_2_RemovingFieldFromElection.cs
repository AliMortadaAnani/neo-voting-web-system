using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1_2_RemovingFieldFromElection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalNumberOfRegisteredVoters",
                table: "Elections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinalNumberOfRegisteredVoters",
                table: "Elections",
                type: "int",
                nullable: true);
        }
    }
}
