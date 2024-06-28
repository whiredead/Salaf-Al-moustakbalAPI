using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalafAlmoustakbalAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class dossiercredit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "credit",
                table: "Dossiers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "credit",
                table: "Dossiers",
                type: "float",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
