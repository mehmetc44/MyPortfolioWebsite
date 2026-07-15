using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddSkillsAndProfileDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Education_DE",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education_EN",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education_TR",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpenToOffers",
                table: "Profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Job_DE",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Job_EN",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Job_TR",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motto_DE",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motto_EN",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motto_TR",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechTags",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Percentage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropColumn(
                name: "Education_DE",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Education_EN",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Education_TR",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "IsOpenToOffers",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Job_DE",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Job_EN",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Job_TR",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Motto_DE",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Motto_EN",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Motto_TR",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TechTags",
                table: "Profiles");
        }
    }
}
