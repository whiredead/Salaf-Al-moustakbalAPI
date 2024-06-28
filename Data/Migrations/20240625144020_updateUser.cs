using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalafAlmoustakbalAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Dossiers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Dossiers_UserId",
                table: "Dossiers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dossiers_AspNetUsers_UserId",
                table: "Dossiers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dossiers_AspNetUsers_UserId",
                table: "Dossiers");

            migrationBuilder.DropIndex(
                name: "IX_Dossiers_UserId",
                table: "Dossiers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Dossiers");
        }
    }
}
