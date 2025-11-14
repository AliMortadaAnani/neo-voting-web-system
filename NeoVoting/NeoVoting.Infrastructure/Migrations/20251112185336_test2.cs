using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateID",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Elections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NominationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NominationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VotingStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VotingEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ElectionStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elections", x => x.Id);
                    table.CheckConstraint("CK_Election_ElectionStatusID", "[ElectionStatusId] BETWEEN 1 and 4");
                    table.CheckConstraint("CK_Election_Name", "LEN([Name]) > 0");
                    table.CheckConstraint("CK_Election_NominationDates", "[NominationEndDate] > [NominationStartDate]");
                    table.CheckConstraint("CK_Election_VotingAfterNomination", "[VotingStartDate] >= [NominationEndDate]");
                    table.CheckConstraint("CK_Election_VotingDates", "[VotingEndDate] > [VotingStartDate]");
                    table.ForeignKey(
                        name: "FK_Elections_ElectionStatuses_ElectionStatusId",
                        column: x => x.ElectionStatusId,
                        principalTable: "ElectionStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SystemAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimestampUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAuditLogs", x => x.Id);
                    table.CheckConstraint("CK_SystemAuditLog_ActionType", "[ActionType] IN ('VOTER_REGISTERED','CANDIDATE_REGISTERED','CANDIDATE_PROFILE_CREATED')");
                    table.ForeignKey(
                        name: "FK_SystemAuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Goals = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NominationReasons = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ProfilePhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProfiles", x => x.Id);
                    table.CheckConstraint("CK_CandidateProfile_Goals", "LEN([Goals]) > 0");
                    table.CheckConstraint("CK_CandidateProfile_NominationReasons", "LEN([NominationReasons]) > 0");
                    table.ForeignKey(
                        name: "FK_CandidateProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CandidateProfiles_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoterAge = table.Column<int>(type: "int", nullable: false),
                    VoterGender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    TimestampUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ElectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.CheckConstraint("CK_Vote_GovernorateId", "[GovernorateId] BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Vote_VoterAge", "[VoterAge] >= 18");
                    table.CheckConstraint("CK_Vote_VoterGender", "[VoterGender] IN ('M','F')");
                    table.ForeignKey(
                        name: "FK_Votes_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votes_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElectionWinners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoteCount = table.Column<int>(type: "int", nullable: true),
                    ElectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionWinners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionWinners_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElectionWinners_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PublicVoteLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimestampUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ElectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GovernorateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicVoteLogs", x => x.Id);
                    table.CheckConstraint("CK_YourEntity_GovernorateId", "[GovernorateId] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_PublicVoteLogs_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicVoteLogs_Governorates_GovernorateId",
                        column: x => x.GovernorateId,
                        principalTable: "Governorates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicVoteLogs_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoteChoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteChoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteChoices_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoteChoices_Votes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Votes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_RefreshToken",
                table: "AspNetUsers",
                column: "RefreshToken",
                unique: true,
                filter: "[RefreshToken] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Gender",
                table: "AspNetUsers",
                sql: "[Gender] IN ('M', 'F') OR [Gender] IS NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_GovernorateID",
                table: "AspNetUsers",
                sql: "[GovernorateID] BETWEEN 1 AND 5 OR [GovernorateID] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ElectionId",
                table: "CandidateProfiles",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_UserId_ElectionId",
                table: "CandidateProfiles",
                columns: new[] { "UserId", "ElectionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Elections_ElectionStatusId",
                table: "Elections",
                column: "ElectionStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionWinners_CandidateProfileId",
                table: "ElectionWinners",
                column: "CandidateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionWinners_ElectionId_CandidateProfileId",
                table: "ElectionWinners",
                columns: new[] { "ElectionId", "CandidateProfileId" },
                unique: true);

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
                name: "IX_SystemAuditLogs_UserId",
                table: "SystemAuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteChoices_CandidateProfileId",
                table: "VoteChoices",
                column: "CandidateProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteChoices_VoteId_CandidateProfileId",
                table: "VoteChoices",
                columns: new[] { "VoteId", "CandidateProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ElectionId",
                table: "Votes",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_GovernorateId",
                table: "Votes",
                column: "GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateID",
                table: "AspNetUsers",
                column: "GovernorateID",
                principalTable: "Governorates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ElectionWinners");

            migrationBuilder.DropTable(
                name: "PublicVoteLogs");

            migrationBuilder.DropTable(
                name: "SystemAuditLogs");

            migrationBuilder.DropTable(
                name: "VoteChoices");

            migrationBuilder.DropTable(
                name: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Elections");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Gender",
                table: "AspNetUsers");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_GovernorateID",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Governorates_GovernorateID",
                table: "AspNetUsers",
                column: "GovernorateID",
                principalTable: "Governorates",
                principalColumn: "Id");
        }
    }
}
