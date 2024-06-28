using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalafAlmoustakbalAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class dossiercredittest1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "test",
                table: "Dossiers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "test",
                table: "Dossiers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
