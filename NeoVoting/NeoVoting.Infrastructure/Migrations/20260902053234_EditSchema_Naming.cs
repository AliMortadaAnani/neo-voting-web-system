using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditSchema_Naming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SystemAuditLog_ActionType",
                table: "SystemAuditLogs");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SystemAuditLog_ActionType",
                table: "SystemAuditLogs",
                sql: "[ActionType] IN ('ADMIN_CREATED_ELECTION', 'ADMIN_STARTED_ELECTION_VOTING_PHASE', 'ADMIN_ENDED_ELECTION_VOTING_PHASE', 'ADMIN_CREATED_POLL', 'ADMIN_STARTED_POLL', 'ADMIN_ENDED_POLL', 'ADMIN_BANNED_USER_ACCOUNT', 'ADMIN_RESET_USER_PASSWORD')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SystemAuditLog_ActionType",
                table: "SystemAuditLogs");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SystemAuditLog_ActionType",
                table: "SystemAuditLogs",
                sql: "[ActionType] IN ('ADMIN_CREATED_ELECTION', 'ADMIN_STARTED_VOTING_PHASE', 'ADMIN_ENDED_VOTING_PHASE', 'ADMIN_CREATED_POLL', 'ADMIN_STARTED_POLL', 'ADMIN_ENDED_POLL', 'ADMIN_BANNED_USER_ACCOUNT', 'ADMIN_RESET_USER_PASSWORD')");
        }
    }
}
