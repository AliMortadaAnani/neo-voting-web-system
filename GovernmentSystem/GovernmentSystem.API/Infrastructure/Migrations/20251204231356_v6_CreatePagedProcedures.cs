using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GovernmentSystem.API.Migrations
{
    public partial class v6_CreatePagedProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetPagedVoters
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
                CREATE PROCEDURE GetPagedCandidates
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('GetPagedVoters', 'P') IS NOT NULL
                    DROP PROCEDURE GetPagedVoters;
            ");

            migrationBuilder.Sql(@"
                IF OBJECT_ID('GetPagedCandidates', 'P') IS NOT NULL
                    DROP PROCEDURE GetPagedCandidates;
            ");
        }
    }
}