using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MisteryApp.Repository.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeIngestSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngestRejectedFiles_IngestRuns_RunId",
                table: "IngestRejectedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MappedRecords_IngestRuns_RunId",
                table: "MappedRecords");

            migrationBuilder.DropIndex(
                name: "IX_IngestRejectedFiles_RunId",
                table: "IngestRejectedFiles");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "MappedRecords");

            migrationBuilder.DropColumn(
                name: "ProcessedFiles",
                table: "IngestRuns");

            migrationBuilder.DropColumn(
                name: "RejectedFiles",
                table: "IngestRuns");

            migrationBuilder.DropColumn(
                name: "TotalFiles",
                table: "IngestRuns");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "IngestRejectedFiles");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "IngestRejectedFiles");

            migrationBuilder.RenameColumn(
                name: "RunId",
                table: "MappedRecords",
                newName: "FileId");

            migrationBuilder.RenameIndex(
                name: "IX_MappedRecords_RunId",
                table: "MappedRecords",
                newName: "IX_MappedRecords_FileId");

            migrationBuilder.RenameColumn(
                name: "RunId",
                table: "IngestRejectedFiles",
                newName: "FileId");

            migrationBuilder.CreateTable(
                name: "IngestFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RunId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProcessedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngestFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngestFiles_IngestRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "IngestRuns",
                        principalColumn: "RunId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngestRejectedFiles_FileId",
                table: "IngestRejectedFiles",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngestFiles_RunId",
                table: "IngestFiles",
                column: "RunId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngestRejectedFiles_IngestFiles_FileId",
                table: "IngestRejectedFiles",
                column: "FileId",
                principalTable: "IngestFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MappedRecords_IngestFiles_FileId",
                table: "MappedRecords",
                column: "FileId",
                principalTable: "IngestFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngestRejectedFiles_IngestFiles_FileId",
                table: "IngestRejectedFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MappedRecords_IngestFiles_FileId",
                table: "MappedRecords");

            migrationBuilder.DropTable(
                name: "IngestFiles");

            migrationBuilder.DropIndex(
                name: "IX_IngestRejectedFiles_FileId",
                table: "IngestRejectedFiles");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "MappedRecords",
                newName: "RunId");

            migrationBuilder.RenameIndex(
                name: "IX_MappedRecords_FileId",
                table: "MappedRecords",
                newName: "IX_MappedRecords_RunId");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "IngestRejectedFiles",
                newName: "RunId");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "MappedRecords",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProcessedFiles",
                table: "IngestRuns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RejectedFiles",
                table: "IngestRuns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalFiles",
                table: "IngestRuns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "IngestRejectedFiles",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RejectedAt",
                table: "IngestRejectedFiles",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_IngestRejectedFiles_RunId",
                table: "IngestRejectedFiles",
                column: "RunId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngestRejectedFiles_IngestRuns_RunId",
                table: "IngestRejectedFiles",
                column: "RunId",
                principalTable: "IngestRuns",
                principalColumn: "RunId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MappedRecords_IngestRuns_RunId",
                table: "MappedRecords",
                column: "RunId",
                principalTable: "IngestRuns",
                principalColumn: "RunId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
