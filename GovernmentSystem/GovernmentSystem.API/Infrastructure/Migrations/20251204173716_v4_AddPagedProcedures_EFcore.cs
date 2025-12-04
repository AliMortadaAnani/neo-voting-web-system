using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovernmentSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class v4_AddPagedProcedures_EFcore : Migration
    {
        /// <inheritdoc />
        // In the Migration file
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Note: We MUST select all columns that EF Core maps to the Voter entity.
            migrationBuilder.Sql(@"
        CREATE OR ALTER PROCEDURE GetPagedVoters
            @Skip INT,
            @Take INT
        AS
        BEGIN
            SET NOCOUNT ON;
            SELECT Id, NationalId, VotingToken, GovernorateId, FirstName, LastName,
                   DateOfBirth, Gender, EligibleForElection, ValidToken, IsRegistered, Voted
            FROM Voters
            ORDER BY LastName, FirstName
            OFFSET @Skip ROWS
            FETCH NEXT @Take ROWS ONLY;
        END
    ");

            migrationBuilder.Sql(@"
        CREATE OR ALTER PROCEDURE GetPagedCandidates
            @Skip INT,
            @Take INT
        AS
        BEGIN
            SET NOCOUNT ON;
            SELECT Id, NationalId, NominationToken, GovernorateId, FirstName, LastName,
                   DateOfBirth, Gender, EligibleForElection, ValidToken, IsRegistered
            FROM Candidates
            ORDER BY LastName, FirstName
            OFFSET @Skip ROWS
            FETCH NEXT @Take ROWS ONLY;
        END
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}