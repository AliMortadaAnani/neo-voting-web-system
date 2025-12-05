using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovernmentSystem.API.Migrations
{
    public partial class v5_DropPagedProcedures : Migration
    {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // (optional) Re-create if you want, or leave empty
        }
    }
}