using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInviteDressScheduleBilingual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DressCode",
                table: "WebsiteSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DressCodeKh",
                table: "WebsiteSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InviteMessageKh",
                table: "WebsiteSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduleText",
                table: "WebsiteSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScheduleTextKh",
                table: "WebsiteSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WelcomeMessageKh",
                table: "WebsiteSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DressCode",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "DressCodeKh",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "InviteMessageKh",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "ScheduleText",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "ScheduleTextKh",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "WelcomeMessageKh",
                table: "WebsiteSettings");
        }
    }
}
