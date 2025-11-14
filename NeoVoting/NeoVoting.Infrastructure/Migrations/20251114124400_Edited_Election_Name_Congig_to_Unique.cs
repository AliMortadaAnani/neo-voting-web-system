using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Edited_Election_Name_Congig_to_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Elections_Name",
                table: "Elections",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Elections_Name",
                table: "Elections");
        }
    }
}
