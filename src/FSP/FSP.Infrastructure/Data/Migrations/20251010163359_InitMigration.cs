using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FSP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeagueName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.LeagueId);
                });

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    ClubId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClubName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LeagueId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.ClubId);
                    table.ForeignKey(
                        name: "LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "LeagueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Age = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MatchPlayed = table.Column<int>(type: "integer", nullable: false),
                    Starts = table.Column<int>(type: "integer", nullable: false),
                    Minutes = table.Column<int>(type: "integer", nullable: false),
                    NineteenMinutes = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Goals = table.Column<int>(type: "integer", nullable: false),
                    Assists = table.Column<int>(type: "integer", nullable: false),
                    GoalsAssists = table.Column<int>(type: "integer", nullable: false),
                    NonPenaltyGoals = table.Column<int>(type: "integer", nullable: false),
                    PenaltyKicksMade = table.Column<int>(type: "integer", nullable: false),
                    PenaltyKickAttempted = table.Column<int>(type: "integer", nullable: false),
                    YellowCards = table.Column<int>(type: "integer", nullable: false),
                    RedCards = table.Column<int>(type: "integer", nullable: false),
                    GoalsPer90s = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AssistsPer90s = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GoalsAssistsPer90s = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NonPenaltyGoalsPer90s = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NonPenaltyGoalsAssistsPer90s = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PlayerRefId = table.Column<string>(type: "text", nullable: false),
                    Season = table.Column<string>(type: "text", nullable: false),
                    ClubId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_Player_Club",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "ClubId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goalkeepings",
                columns: table => new
                {
                    GoalkeepingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Age = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MatchPlayed = table.Column<int>(type: "integer", nullable: false),
                    Starts = table.Column<int>(type: "integer", nullable: false),
                    Minutes = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NineteenMinutes = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GoalsAgainst = table.Column<int>(type: "integer", nullable: false),
                    GoalsAssistsPer90s = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ShotsOnTargetAgainst = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Saves = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    SavePercentage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Wins = table.Column<int>(type: "integer", nullable: false),
                    Draws = table.Column<int>(type: "integer", nullable: false),
                    Losses = table.Column<int>(type: "integer", nullable: false),
                    CleanSheets = table.Column<int>(type: "integer", nullable: false),
                    CleanSheetsPercentage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PenaltyKicksAttempted = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PenaltyKicksAllowed = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PenaltyKicksSaved = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PenaltyKicksMissed = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PenaltyKicksSavedPercentage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Season = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    PlayerRefId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goalkeepings", x => x.GoalkeepingId);
                    table.ForeignKey(
                        name: "FK_Goalkeeping_Player",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDetails",
                columns: table => new
                {
                    PlayerDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OriginalName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Born = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Citizenship = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Club = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PlayerRefId = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDetails", x => x.PlayerDetailId);
                    table.ForeignKey(
                        name: "FK_PlayerDetails_Player",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shootings",
                columns: table => new
                {
                    ShootingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Age = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NineteenMinutes = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Goals = table.Column<int>(type: "integer", nullable: false),
                    ShotsTotal = table.Column<int>(type: "integer", nullable: false),
                    ShotsOnTarget = table.Column<int>(type: "integer", nullable: false),
                    ShotsOnTargetPercentage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ShotsTotalPer90 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ShotsOnTargetPer90 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GoalsShots = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    GoalsShotsOnTarget = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    AverageShotDistance = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PenaltyKicksMade = table.Column<int>(type: "integer", nullable: false),
                    PenaltyKicksAttempted = table.Column<int>(type: "integer", nullable: false),
                    Season = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<int>(type: "integer", nullable: false),
                    PlayerRefId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shootings", x => x.ShootingId);
                    table.ForeignKey(
                        name: "FK_Shooting_Player",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clubs_LeagueId",
                table: "Clubs",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_Goalkeepings_PlayerId",
                table: "Goalkeepings",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDetails_PlayerId",
                table: "PlayerDetails",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerName_ClubId",
                table: "Players",
                columns: new[] { "PlayerName", "ClubId" });

            migrationBuilder.CreateIndex(
                name: "IX_Shootings_PlayerId",
                table: "Shootings",
                column: "PlayerId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goalkeepings");

            migrationBuilder.DropTable(
                name: "PlayerDetails");

            migrationBuilder.DropTable(
                name: "Shootings");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Leagues");
        }
    }
}
