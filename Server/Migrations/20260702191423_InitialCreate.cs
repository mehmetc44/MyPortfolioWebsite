using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title_TR = table.Column<string>(type: "TEXT", nullable: false),
                    Title_EN = table.Column<string>(type: "TEXT", nullable: false),
                    Title_DE = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    ReadTime = table.Column<string>(type: "TEXT", nullable: false),
                    SubTag_TR = table.Column<string>(type: "TEXT", nullable: false),
                    SubTag_EN = table.Column<string>(type: "TEXT", nullable: false),
                    SubTag_DE = table.Column<string>(type: "TEXT", nullable: false),
                    Excerpt_TR = table.Column<string>(type: "TEXT", nullable: false),
                    Excerpt_EN = table.Column<string>(type: "TEXT", nullable: false),
                    Excerpt_DE = table.Column<string>(type: "TEXT", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", nullable: false),
                    DetailText_TR = table.Column<string>(type: "TEXT", nullable: false),
                    DetailText_EN = table.Column<string>(type: "TEXT", nullable: false),
                    DetailText_DE = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Tag_TR = table.Column<string>(type: "TEXT", nullable: false),
                    Tag_EN = table.Column<string>(type: "TEXT", nullable: false),
                    Tag_DE = table.Column<string>(type: "TEXT", nullable: false),
                    Title_TR = table.Column<string>(type: "TEXT", nullable: false),
                    Title_EN = table.Column<string>(type: "TEXT", nullable: false),
                    Title_DE = table.Column<string>(type: "TEXT", nullable: false),
                    Bio_TR = table.Column<string>(type: "TEXT", nullable: false),
                    Bio_EN = table.Column<string>(type: "TEXT", nullable: false),
                    Bio_DE = table.Column<string>(type: "TEXT", nullable: false),
                    AvatarUrl = table.Column<string>(type: "TEXT", nullable: false),
                    Repos = table.Column<int>(type: "INTEGER", nullable: false),
                    Pubs = table.Column<int>(type: "INTEGER", nullable: false),
                    Github = table.Column<string>(type: "TEXT", nullable: false),
                    Linkedin = table.Column<string>(type: "TEXT", nullable: false),
                    Twitter = table.Column<string>(type: "TEXT", nullable: false),
                    Medium = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title_TR = table.Column<string>(type: "TEXT", nullable: false),
                    Title_EN = table.Column<string>(type: "TEXT", nullable: false),
                    Title_DE = table.Column<string>(type: "TEXT", nullable: false),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<string>(type: "TEXT", nullable: false),
                    Client = table.Column<string>(type: "TEXT", nullable: false),
                    SubTag_TR = table.Column<string>(type: "TEXT", nullable: false),
                    SubTag_EN = table.Column<string>(type: "TEXT", nullable: false),
                    SubTag_DE = table.Column<string>(type: "TEXT", nullable: false),
                    Description_TR = table.Column<string>(type: "TEXT", nullable: false),
                    Description_EN = table.Column<string>(type: "TEXT", nullable: false),
                    Description_DE = table.Column<string>(type: "TEXT", nullable: false),
                    Tech = table.Column<string>(type: "TEXT", nullable: false),
                    RepoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    DemoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    ImagesJson = table.Column<string>(type: "TEXT", nullable: false),
                    DetailText_TR = table.Column<string>(type: "TEXT", nullable: false),
                    DetailText_EN = table.Column<string>(type: "TEXT", nullable: false),
                    DetailText_DE = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
