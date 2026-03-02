using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Projects_ProjectsId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "HeroImagePath",
                table: "AboutMe");

            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "AboutMe");

            migrationBuilder.RenameColumn(
                name: "ProjectsId",
                table: "Files",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_ProjectsId",
                table: "Files",
                newName: "IX_Files_ProjectId");

            migrationBuilder.AddColumn<int>(
                name: "SiteImageType",
                table: "Files",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Projects_ProjectId",
                table: "Files",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Projects_ProjectId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "SiteImageType",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Files",
                newName: "ProjectsId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_ProjectId",
                table: "Files",
                newName: "IX_Files_ProjectsId");

            migrationBuilder.AddColumn<string>(
                name: "HeroImagePath",
                table: "AboutMe",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "AboutMe",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Projects_ProjectsId",
                table: "Files",
                column: "ProjectsId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
