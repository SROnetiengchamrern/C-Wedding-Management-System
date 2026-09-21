using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class InviteThemeMediaMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "WebsiteSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InviteMessage",
                table: "WebsiteSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapSearch",
                table: "WebsiteSettings",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MusicUrl",
                table: "WebsiteSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowMap",
                table: "WebsiteSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ThemeLayout",
                table: "WebsiteSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WebsiteBankQrs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebsiteSettingsId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteBankQrs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteBankQrs_WebsiteSettings_WebsiteSettingsId",
                        column: x => x.WebsiteSettingsId,
                        principalTable: "WebsiteSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebsiteGalleryPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WebsiteSettingsId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebsiteGalleryPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebsiteGalleryPhotos_WebsiteSettings_WebsiteSettingsId",
                        column: x => x.WebsiteSettingsId,
                        principalTable: "WebsiteSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteBankQrs_WebsiteSettingsId",
                table: "WebsiteBankQrs",
                column: "WebsiteSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_WebsiteGalleryPhotos_WebsiteSettingsId",
                table: "WebsiteGalleryPhotos",
                column: "WebsiteSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebsiteBankQrs");

            migrationBuilder.DropTable(
                name: "WebsiteGalleryPhotos");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "InviteMessage",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "MapSearch",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "MusicUrl",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "ShowMap",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "ThemeLayout",
                table: "WebsiteSettings");
        }
    }
}
