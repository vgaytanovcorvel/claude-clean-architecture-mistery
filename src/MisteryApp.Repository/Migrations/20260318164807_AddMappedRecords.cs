using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisteryApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddMappedRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MappedRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IngestedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MappedRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MappedRecords_IngestRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "IngestRuns",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MappedRecords_IngestedAt",
                table: "MappedRecords",
                column: "IngestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MappedRecords_RunId",
                table: "MappedRecords",
                column: "RunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MappedRecords");
        }
    }
}
