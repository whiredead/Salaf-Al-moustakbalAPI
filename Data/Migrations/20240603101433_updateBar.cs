using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalafAlmoustakbalAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateBar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "hasChild",
                table: "Bars",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hasChild",
                table: "Bars");
        }
    }
}
