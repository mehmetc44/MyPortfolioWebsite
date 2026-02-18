using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portfolio.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percent",
                table: "Languages");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Languages",
                newName: "Name_Tr");

            migrationBuilder.AddColumn<string>(
                name: "Name_De",
                table: "Languages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name_En",
                table: "Languages",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name_De",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "Name_En",
                table: "Languages");

            migrationBuilder.RenameColumn(
                name: "Name_Tr",
                table: "Languages",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "Percent",
                table: "Languages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
