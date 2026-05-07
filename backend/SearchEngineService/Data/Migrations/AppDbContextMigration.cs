using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SearchEngineService.Data.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Contents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                Views = table.Column<double>(type: "double precision", nullable: false),
                Likes = table.Column<double>(type: "double precision", nullable: false),
                Reactions = table.Column<double>(type: "double precision", nullable: false),
                ReadingTime = table.Column<double>(type: "double precision", nullable: false),
                PopularityScore = table.Column<double>(type: "double precision", nullable: false),
                RelevanceScore = table.Column<double>(type: "double precision", nullable: false),
                FinalScore = table.Column<double>(type: "double precision", nullable: false),
                PublishedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                LastSyncedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                RowVersion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, rowVersion: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Contents", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Contents_FinalScore",
            table: "Contents",
            column: "FinalScore");

        migrationBuilder.CreateIndex(
            name: "IX_Contents_PopularityScore",
            table: "Contents",
            column: "PopularityScore");

        migrationBuilder.CreateIndex(
            name: "IX_Contents_PublishedAtUtc",
            table: "Contents",
            column: "PublishedAtUtc");

        migrationBuilder.CreateIndex(
            name: "IX_Contents_Provider_ExternalId",
            table: "Contents",
            columns: new[] { "Provider", "ExternalId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Contents_Title",
            table: "Contents",
            column: "Title");

        migrationBuilder.CreateIndex(
            name: "IX_Contents_Type",
            table: "Contents",
            column: "Type");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Contents");
    }
}
