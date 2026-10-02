using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryTaxonomy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropColumn(
                name: "Category_DE",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Category_EN",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Category_TR",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SubTag_DE",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SubTag_EN",
                table: "Articles");
            */

            migrationBuilder.RenameColumn(
                name: "SubTag_TR",
                table: "Articles",
                newName: "CategoryId");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name_TR = table.Column<string>(type: "text", nullable: false),
                    Name_EN = table.Column<string>(type: "text", nullable: false),
                    Name_DE = table.Column<string>(type: "text", nullable: false),
                    IsSubCategory = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Articles",
                newName: "SubTag_TR");

            migrationBuilder.AddColumn<string>(
                name: "Category_DE",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category_EN",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category_TR",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubTag_DE",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubTag_EN",
                table: "Articles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
