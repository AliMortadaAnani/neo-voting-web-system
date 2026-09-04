using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class edits2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Election_NominationDates",
                table: "Elections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Election_VotingAfterNomination",
                table: "Elections");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Election_VotingDates",
                table: "Elections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Election_NominationDates",
                table: "Elections",
                sql: "[NominationEndDate] > [NominationStartDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Election_VotingAfterNomination",
                table: "Elections",
                sql: "[VotingStartDate] >= [NominationEndDate]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Election_VotingDates",
                table: "Elections",
                sql: "[VotingEndDate] > [VotingStartDate]");
        }
    }
}
