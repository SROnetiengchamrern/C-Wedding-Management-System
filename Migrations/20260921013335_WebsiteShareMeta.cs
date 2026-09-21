using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class WebsiteShareMeta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShareDescription",
                table: "WebsiteSettings",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShareImageUrl",
                table: "WebsiteSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShareDescription",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "ShareImageUrl",
                table: "WebsiteSettings");
        }
    }
}
