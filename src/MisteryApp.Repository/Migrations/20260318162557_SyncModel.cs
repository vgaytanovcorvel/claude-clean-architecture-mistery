using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisteryApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_IngestRuns_StartedAt",
                table: "IngestRuns",
                column: "StartedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IngestRuns_StartedAt",
                table: "IngestRuns");
        }
    }
}
