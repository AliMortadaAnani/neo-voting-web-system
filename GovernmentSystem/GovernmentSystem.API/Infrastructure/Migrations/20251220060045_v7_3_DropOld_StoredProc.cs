using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovernmentSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class v7_3_DropOld_StoredProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the procedures if they exist
            migrationBuilder.Sql(@"
                IF OBJECT_ID('GetPagedVoters', 'P') IS NOT NULL
                    DROP PROCEDURE GetPagedVoters;
            ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('GetPagedCandidates', 'P') IS NOT NULL
                    DROP PROCEDURE GetPagedCandidates;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
