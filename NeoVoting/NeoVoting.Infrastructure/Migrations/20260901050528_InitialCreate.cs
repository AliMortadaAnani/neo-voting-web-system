using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoVoting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpirationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Elections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NominationStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NominationEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VotingStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VotingEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elections", x => x.Id);
                    table.CheckConstraint("CK_Election_NominationDates", "[NominationEndDate] > [NominationStartDate]");
                    table.CheckConstraint("CK_Election_Status", "([Status] IN (1, 2, 3) )");
                    table.CheckConstraint("CK_Election_VotingAfterNomination", "[VotingStartDate] >= [NominationEndDate]");
                    table.CheckConstraint("CK_Election_VotingDates", "[VotingEndDate] > [VotingStartDate]");
                });

            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Question = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.Id);
                    table.CheckConstraint("CK_Poll_StartEndDates", "[EndDate] > [StartDate]");
                    table.CheckConstraint("CK_Poll_Status", "([Status] IN (1, 2, 3) )");
                });

            migrationBuilder.CreateTable(
                name: "SystemAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimestampUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAuditLogs", x => x.Id);
                    table.CheckConstraint("CK_SystemAuditLog_ActionType", "[ActionType] IN ('ADMIN_CREATED_ELECTION', 'ADMIN_STARTED_VOTING_PHASE', 'ADMIN_ENDED_VOTING_PHASE', 'ADMIN_CREATED_POLL', 'ADMIN_STARTED_POLL', 'ADMIN_ENDED_POLL', 'ADMIN_BANNED_USER_ACCOUNT', 'ADMIN_RESET_USER_PASSWORD')");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Governorate = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    VerificationHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                    table.CheckConstraint("CK_Candidate_Gender", "[Gender] IN ('M', 'F')");
                    table.CheckConstraint("CK_Candidate_Governorate", "([Governorate] IN (1, 2, 3, 4, 5) )");
                    table.ForeignKey(
                        name: "FK_Candidates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Voters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Governorate = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    VerificationHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voters", x => x.Id);
                    table.CheckConstraint("CK_Voter_Gender", "[Gender] IN ('M', 'F')");
                    table.CheckConstraint("CK_Voter_Governorate", "([Governorate] IN (1, 2, 3, 4, 5) )");
                    table.ForeignKey(
                        name: "FK_Voters_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Election_Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElectionId = table.Column<int>(type: "int", nullable: false),
                    Governorate = table.Column<int>(type: "int", nullable: true),
                    ParliamentStartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ParliamentEndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalRegisteredVotersCount = table.Column<int>(type: "int", nullable: true),
                    RegisteredMaleVotersCount = table.Column<int>(type: "int", nullable: true),
                    RegisteredFemaleVotersCount = table.Column<int>(type: "int", nullable: true),
                    RegisteredVotersAged18To29Count = table.Column<int>(type: "int", nullable: true),
                    RegisteredVotersAged30To45Count = table.Column<int>(type: "int", nullable: true),
                    RegisteredVotersAged46To64Count = table.Column<int>(type: "int", nullable: true),
                    RegisteredVotersAged65AndOverCount = table.Column<int>(type: "int", nullable: true),
                    TotalActualVotersCount = table.Column<int>(type: "int", nullable: true),
                    ActualMaleVotersCount = table.Column<int>(type: "int", nullable: true),
                    ActualFemaleVotersCount = table.Column<int>(type: "int", nullable: true),
                    ActualVotersAged18To29Count = table.Column<int>(type: "int", nullable: true),
                    ActualVotersAged30To45Count = table.Column<int>(type: "int", nullable: true),
                    ActualVotersAged46To64Count = table.Column<int>(type: "int", nullable: true),
                    ActualVotersAged65AndOverCount = table.Column<int>(type: "int", nullable: true),
                    TotalNominatedCandidatesCount = table.Column<int>(type: "int", nullable: true),
                    NominatedMaleCandidatesCount = table.Column<int>(type: "int", nullable: true),
                    NominatedFemaleCandidatesCount = table.Column<int>(type: "int", nullable: true),
                    NominatedCandidatesAged18To29Count = table.Column<int>(type: "int", nullable: true),
                    NominatedCandidatesAged30To45Count = table.Column<int>(type: "int", nullable: true),
                    NominatedCandidatesAged46To64Count = table.Column<int>(type: "int", nullable: true),
                    NominatedCandidatesAged65AndOverCount = table.Column<int>(type: "int", nullable: true),
                    TotalWinningCandidatesCount = table.Column<int>(type: "int", nullable: true),
                    WinningMaleCandidatesCount = table.Column<int>(type: "int", nullable: true),
                    WinningFemaleCandidatesCount = table.Column<int>(type: "int", nullable: true),
                    WinningCandidatesAged18To29Count = table.Column<int>(type: "int", nullable: true),
                    WinningCandidatesAged30To45Count = table.Column<int>(type: "int", nullable: true),
                    WinningCandidatesAged46To64Count = table.Column<int>(type: "int", nullable: true),
                    WinningCandidatesAged65AndOverCount = table.Column<int>(type: "int", nullable: true),
                    PercentageOfTotalRegisteredVotersWhoAreActualVoters = table.Column<double>(type: "float", nullable: true),
                    PercentageOfRegisteredMaleVotersWhoAreActualMaleVoters = table.Column<double>(type: "float", nullable: true),
                    PercentageOfRegisteredFemaleVotersWhoAreActualFemaleVoters = table.Column<double>(type: "float", nullable: true),
                    PercentageOfRegisteredVotersAged18To29WhoAreActualVoters = table.Column<double>(type: "float", nullable: true),
                    PercentageOfRegisteredVotersAged30To45WhoAreActualVoters = table.Column<double>(type: "float", nullable: true),
                    PercentageOfRegisteredVotersAged46To64WhoAreActualVoters = table.Column<double>(type: "float", nullable: true),
                    PercentageOfRegisteredVotersAged65AndOverWhoAreActualVoters = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalActualVotersWhoAreMale = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalActualVotersWhoAreFemale = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalActualVotersWhoAreAged18To29 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalActualVotersWhoAreAged30To45 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalActualVotersWhoAreAged46To64 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalActualVotersWhoAreAged65AndOver = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalNominatedCandidatesWhoAreMale = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalNominatedCandidatesWhoAreFemale = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalNominatedCandidatesWhoAreAged18To29 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalNominatedCandidatesWhoAreAged30To45 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalNominatedCandidatesWhoAreAged46To64 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalNominatedCandidatesWhoAreAged65AndOver = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalWinningCandidatesWhoAreMale = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalWinningCandidatesWhoAreFemale = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalWinningCandidatesWhoAreAged18To29 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalWinningCandidatesWhoAreAged30To45 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalWinningCandidatesWhoAreAged46To64 = table.Column<double>(type: "float", nullable: true),
                    PercentageOfTotalWinningCandidatesWhoAreAged65AndOver = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenMaleAndFemaleActualVoterSharePercentage = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenMaleAndFemaleNominatedCandidateSharePercentage = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenMaleAndFemaleWinningCandidateSharePercentage = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenWinningMaleShareAndActualMaleVoterShare = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenWinningShareAndActualVoterShareForAged18To29 = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenWinningShareAndActualVoterShareForAged30To45 = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenWinningShareAndActualVoterShareForAged46To64 = table.Column<double>(type: "float", nullable: true),
                    DifferenceBetweenWinningShareAndActualVoterShareForAged65AndOver = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Election_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Election_Statistics_Elections_ElectionId",
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
                    Governorate = table.Column<int>(type: "int", nullable: false),
                    TimestampUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ElectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.CheckConstraint("CK_Vote_Governorate", "([Governorate] IN (1, 2, 3, 4, 5) )");
                    table.ForeignKey(
                        name: "FK_Votes_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Poll_Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PollId = table.Column<int>(type: "int", nullable: false),
                    RegisteredVotersCount = table.Column<int>(type: "int", nullable: true),
                    ActualVotersCount = table.Column<int>(type: "int", nullable: true),
                    ParticipationPercentage = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poll_Statistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Poll_Statistics_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PollAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Answer = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PollId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollAnswers_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CandidateProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Goals = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    NominationReasons = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProfilePhotoFilename = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    ElectionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateProfiles_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
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
                name: "EventParticipations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoterId = table.Column<int>(type: "int", nullable: false),
                    ElectionId = table.Column<int>(type: "int", nullable: false),
                    PollId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParticipations_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventParticipations_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventParticipations_Voters_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Voters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PollVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimestampUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PollId = table.Column<int>(type: "int", nullable: false),
                    PollAnswerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollVotes_PollAnswers_PollAnswerId",
                        column: x => x.PollAnswerId,
                        principalTable: "PollAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PollVotes_Polls_PollId",
                        column: x => x.PollId,
                        principalTable: "Polls",
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
                    CandidateProfileId = table.Column<int>(type: "int", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "VoteChoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CandidateProfileId = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_CandidateId",
                table: "CandidateProfiles",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ElectionId_CandidateId",
                table: "CandidateProfiles",
                columns: new[] { "ElectionId", "CandidateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_UserId",
                table: "Candidates",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Election_Statistics_ElectionId",
                table: "Election_Statistics",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Elections_Name",
                table: "Elections",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectionWinners_CandidateProfileId",
                table: "ElectionWinners",
                column: "CandidateProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipations_ElectionId",
                table: "EventParticipations",
                column: "ElectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipations_PollId",
                table: "EventParticipations",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipations_VoterId_ElectionId",
                table: "EventParticipations",
                columns: new[] { "VoterId", "ElectionId" },
                unique: true,
                filter: "[ElectionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipations_VoterId_PollId",
                table: "EventParticipations",
                columns: new[] { "VoterId", "PollId" },
                unique: true,
                filter: "[PollId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Poll_Statistics_PollId",
                table: "Poll_Statistics",
                column: "PollId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollAnswers_PollId",
                table: "PollAnswers",
                column: "PollId");

            migrationBuilder.CreateIndex(
                name: "IX_Polls_Name",
                table: "Polls",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_Id_PollAnswerId",
                table: "PollVotes",
                columns: new[] { "Id", "PollAnswerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_PollAnswerId",
                table: "PollVotes",
                column: "PollAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_PollVotes_PollId",
                table: "PollVotes",
                column: "PollId");

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
                name: "IX_Voters_UserId",
                table: "Voters",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ElectionId",
                table: "Votes",
                column: "ElectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Election_Statistics");

            migrationBuilder.DropTable(
                name: "ElectionWinners");

            migrationBuilder.DropTable(
                name: "EventParticipations");

            migrationBuilder.DropTable(
                name: "Poll_Statistics");

            migrationBuilder.DropTable(
                name: "PollVotes");

            migrationBuilder.DropTable(
                name: "SystemAuditLogs");

            migrationBuilder.DropTable(
                name: "VoteChoices");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Voters");

            migrationBuilder.DropTable(
                name: "PollAnswers");

            migrationBuilder.DropTable(
                name: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Polls");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "Elections");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
