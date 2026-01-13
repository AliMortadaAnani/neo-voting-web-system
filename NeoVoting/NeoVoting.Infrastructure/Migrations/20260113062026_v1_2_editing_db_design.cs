using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v1_2_editing_db_design : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateID",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ElectionWinners_Elections_ElectionId",
                table: "ElectionWinners");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicVoteLogs_Elections_ElectionId",
                table: "PublicVoteLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicVoteLogs_Governorates_GovernorateId",
                table: "PublicVoteLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemAuditLogs_Elections_ElectionId",
                table: "SystemAuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_PublicVoteLogs_ElectionId_VoteId",
                table: "PublicVoteLogs");

            migrationBuilder.DropIndex(
                name: "IX_PublicVoteLogs_GovernorateId",
                table: "PublicVoteLogs");

            migrationBuilder.DropIndex(
                name: "IX_PublicVoteLogs_VoteId",
                table: "PublicVoteLogs");

            migrationBuilder.DropIndex(
                name: "IX_ElectionWinners_CandidateProfileId",
                table: "ElectionWinners");

            migrationBuilder.DropIndex(
                name: "IX_ElectionWinners_ElectionId_CandidateProfileId",
                table: "ElectionWinners");

            migrationBuilder.DropColumn(
                name: "ElectionId",
                table: "PublicVoteLogs");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "PublicVoteLogs");

            migrationBuilder.DropColumn(
                name: "ElectionId",
                table: "ElectionWinners");

            migrationBuilder.RenameColumn(
                name: "ElectionId",
                table: "SystemAuditLogs",
                newName: "CandidateProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemAuditLogs_ElectionId",
                table: "SystemAuditLogs",
                newName: "IX_SystemAuditLogs_CandidateProfileId");

            migrationBuilder.RenameColumn(
                name: "GovernorateID",
                table: "AspNetUsers",
                newName: "GovernorateId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_GovernorateID",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_GovernorateId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "SystemAuditLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicVoteLogs_VoteId",
                table: "PublicVoteLogs",
                column: "VoteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectionWinners_CandidateProfileId",
                table: "ElectionWinners",
                column: "CandidateProfileId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateId",
                table: "AspNetUsers",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAuditLogs_CandidateProfiles_CandidateProfileId",
                table: "SystemAuditLogs",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_SystemAuditLogs_CandidateProfiles_CandidateProfileId",
                table: "SystemAuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_PublicVoteLogs_VoteId",
                table: "PublicVoteLogs");

            migrationBuilder.DropIndex(
                name: "IX_ElectionWinners_CandidateProfileId",
                table: "ElectionWinners");

            migrationBuilder.RenameColumn(
                name: "CandidateProfileId",
                table: "SystemAuditLogs",
                newName: "ElectionId");

            migrationBuilder.RenameIndex(
                name: "IX_SystemAuditLogs_CandidateProfileId",
                table: "SystemAuditLogs",
                newName: "IX_SystemAuditLogs_ElectionId");

            migrationBuilder.RenameColumn(
                name: "GovernorateId",
                table: "AspNetUsers",
                newName: "GovernorateID");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_GovernorateId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_GovernorateID");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "SystemAuditLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ElectionId",
                table: "PublicVoteLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "GovernorateId",
                table: "PublicVoteLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ElectionId",
                table: "ElectionWinners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PublicVoteLogs_ElectionId_VoteId",
                table: "PublicVoteLogs",
                columns: new[] { "ElectionId", "VoteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicVoteLogs_GovernorateId",
                table: "PublicVoteLogs",
                column: "GovernorateId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicVoteLogs_VoteId",
                table: "PublicVoteLogs",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionWinners_CandidateProfileId",
                table: "ElectionWinners",
                column: "CandidateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionWinners_ElectionId_CandidateProfileId",
                table: "ElectionWinners",
                columns: new[] { "ElectionId", "CandidateProfileId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateID",
                table: "AspNetUsers",
                column: "GovernorateID",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionWinners_Elections_ElectionId",
                table: "ElectionWinners",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicVoteLogs_Elections_ElectionId",
                table: "PublicVoteLogs",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PublicVoteLogs_Governorates_GovernorateId",
                table: "PublicVoteLogs",
                column: "GovernorateId",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SystemAuditLogs_Elections_ElectionId",
                table: "SystemAuditLogs",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
