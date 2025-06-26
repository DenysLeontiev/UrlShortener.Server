using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtentedUrlTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Urls",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Urls");
        }
    }
}
