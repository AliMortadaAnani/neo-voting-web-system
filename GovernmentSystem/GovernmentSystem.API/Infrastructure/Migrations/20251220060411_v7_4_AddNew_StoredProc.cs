using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovernmentSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class v7_4_AddNew_StoredProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update GetPagedVoters to return RegisteredUsername
            migrationBuilder.Sql(@"
        CREATE OR ALTER PROCEDURE GetPagedVoters
            @Skip INT,
            @Take INT
        AS
        BEGIN
            SET NOCOUNT ON;
            SELECT Id, NationalId, VotingToken, GovernorateId, FirstName, LastName,
                DateOfBirth, Gender, EligibleForElection, ValidToken, IsRegistered, Voted,
                RegisteredUsername  -- <--- NEW COLUMN ADDED HERE
            FROM Voters
            ORDER BY LastName, FirstName
            OFFSET @Skip ROWS
            FETCH NEXT @Take ROWS ONLY;
        END
    ");

            // Update GetPagedCandidates to return RegisteredUsername
            migrationBuilder.Sql(@"
        CREATE OR ALTER PROCEDURE GetPagedCandidates
            @Skip INT,
            @Take INT
        AS
        BEGIN
            SET NOCOUNT ON;
            SELECT Id, NationalId, NominationToken, GovernorateId, FirstName, LastName,
                DateOfBirth, Gender, EligibleForElection, ValidToken, IsRegistered,
                RegisteredUsername -- <--- NEW COLUMN ADDED HERE
            FROM Candidates
            ORDER BY LastName, FirstName
            OFFSET @Skip ROWS
            FETCH NEXT @Take ROWS ONLY;
        END
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Implementation depends on if you want to drop them or revert them.
            // Usually fine to just drop them in Down() if this was a create migration.
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPagedVoters");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS GetPagedCandidates");
        }
    }
}
