using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adding_SystemAuditLogPagedProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetPagedSystemAuditLogs
                    @Skip INT,
                    @Take INT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT Id, TimestampUTC, ActionType, Details, UserId, ElectionId
                    FROM SystemAuditLogs
                    ORDER BY TimestampUTC DESC
                    OFFSET @Skip ROWS
                    FETCH NEXT @Take ROWS ONLY;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF OBJECT_ID('GetPagedSystemAuditLogs', 'P') IS NOT NULL
                    DROP PROCEDURE GetPagedSystemAuditLogs;
            ");
        }
    }
}