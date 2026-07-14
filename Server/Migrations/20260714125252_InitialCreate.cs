using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title_TR = table.Column<string>(type: "text", nullable: false),
                    Title_EN = table.Column<string>(type: "text", nullable: false),
                    Title_DE = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<string>(type: "text", nullable: false),
                    ReadTime = table.Column<string>(type: "text", nullable: false),
                    SubTag_TR = table.Column<string>(type: "text", nullable: false),
                    SubTag_EN = table.Column<string>(type: "text", nullable: false),
                    SubTag_DE = table.Column<string>(type: "text", nullable: false),
                    Excerpt_TR = table.Column<string>(type: "text", nullable: false),
                    Excerpt_EN = table.Column<string>(type: "text", nullable: false),
                    Excerpt_DE = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    DetailText_TR = table.Column<string>(type: "text", nullable: false),
                    DetailText_EN = table.Column<string>(type: "text", nullable: false),
                    DetailText_DE = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Tag_TR = table.Column<string>(type: "text", nullable: true),
                    Tag_EN = table.Column<string>(type: "text", nullable: true),
                    Tag_DE = table.Column<string>(type: "text", nullable: true),
                    Title_TR = table.Column<string>(type: "text", nullable: true),
                    Title_EN = table.Column<string>(type: "text", nullable: true),
                    Title_DE = table.Column<string>(type: "text", nullable: true),
                    Bio_TR = table.Column<string>(type: "text", nullable: true),
                    Bio_EN = table.Column<string>(type: "text", nullable: true),
                    Bio_DE = table.Column<string>(type: "text", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    Repos = table.Column<int>(type: "integer", nullable: false),
                    Pubs = table.Column<int>(type: "integer", nullable: false),
                    Github = table.Column<string>(type: "text", nullable: true),
                    Linkedin = table.Column<string>(type: "text", nullable: true),
                    Instagram = table.Column<string>(type: "text", nullable: true),
                    Medium = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title_TR = table.Column<string>(type: "text", nullable: false),
                    Title_EN = table.Column<string>(type: "text", nullable: false),
                    Title_DE = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<string>(type: "text", nullable: false),
                    Client = table.Column<string>(type: "text", nullable: false),
                    SubTag_TR = table.Column<string>(type: "text", nullable: false),
                    SubTag_EN = table.Column<string>(type: "text", nullable: false),
                    SubTag_DE = table.Column<string>(type: "text", nullable: false),
                    Description_TR = table.Column<string>(type: "text", nullable: false),
                    Description_EN = table.Column<string>(type: "text", nullable: false),
                    Description_DE = table.Column<string>(type: "text", nullable: false),
                    Tech = table.Column<string>(type: "text", nullable: false),
                    RepoUrl = table.Column<string>(type: "text", nullable: false),
                    DemoUrl = table.Column<string>(type: "text", nullable: false),
                    ImagesJson = table.Column<string>(type: "text", nullable: false),
                    DetailText_TR = table.Column<string>(type: "text", nullable: false),
                    DetailText_EN = table.Column<string>(type: "text", nullable: false),
                    DetailText_DE = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
