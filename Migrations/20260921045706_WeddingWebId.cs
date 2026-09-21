using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class WeddingWebId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WebId",
                table: "Weddings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                WITH numbered AS (
                    SELECT Id, ROW_NUMBER() OVER (ORDER BY Partner1Name, Partner2Name, Id) AS rn
                    FROM Weddings
                )
                UPDATE w
                SET WebId = 2025 + n.rn
                FROM Weddings w
                INNER JOIN numbered n ON n.Id = w.Id;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Weddings_WebId",
                table: "Weddings",
                column: "WebId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Weddings_WebId",
                table: "Weddings");

            migrationBuilder.DropColumn(
                name: "WebId",
                table: "Weddings");
        }
    }
}
