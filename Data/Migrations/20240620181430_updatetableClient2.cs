using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalafAlmoustakbalAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatetableClient2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "statut_des",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "statut_des",
                table: "Clients");
        }
    }
}
