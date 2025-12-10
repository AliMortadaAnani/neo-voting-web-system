using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adding_election_property_to_SystemAuditLogs_And_SystemAuditLogPagedProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ElectionId",
                table: "SystemAuditLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemAuditLogs_ElectionId",
                table: "SystemAuditLogs",
                column: "ElectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAuditLogs_Elections_ElectionId",
                table: "SystemAuditLogs",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemAuditLogs_Elections_ElectionId",
                table: "SystemAuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_SystemAuditLogs_ElectionId",
                table: "SystemAuditLogs");

            migrationBuilder.DropColumn(
                name: "ElectionId",
                table: "SystemAuditLogs");
        }
    }
}
