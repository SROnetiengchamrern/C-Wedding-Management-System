using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupleProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Partner1Bio",
                table: "WebsiteSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partner1BioKh",
                table: "WebsiteSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Partner1BirthDate",
                table: "WebsiteSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partner1PhotoUrl",
                table: "WebsiteSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partner1Title",
                table: "WebsiteSettings",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partner2Bio",
                table: "WebsiteSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partner2BioKh",
                table: "WebsiteSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Partner2BirthDate",
                table: "WebsiteSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partner2PhotoUrl",
                table: "WebsiteSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Partner2Title",
                table: "WebsiteSettings",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Partner1Bio",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner1BioKh",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner1BirthDate",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner1PhotoUrl",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner1Title",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner2Bio",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner2BioKh",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner2BirthDate",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner2PhotoUrl",
                table: "WebsiteSettings");

            migrationBuilder.DropColumn(
                name: "Partner2Title",
                table: "WebsiteSettings");
        }
    }
}
