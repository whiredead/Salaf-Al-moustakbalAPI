using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalafAlmoustakbalAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class dossiercredittest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "test",
                table: "Dossiers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "test",
                table: "Dossiers");
        }
    }
}
