using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalafAlmoustakbalAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatetableClient1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_StatutOccupationLogement_StatutOccupationLogementId",
                table: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StatutOccupationLogement",
                table: "StatutOccupationLogement");

            migrationBuilder.RenameTable(
                name: "StatutOccupationLogement",
                newName: "statut");

            migrationBuilder.AddPrimaryKey(
                name: "PK_statut",
                table: "statut",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_statut_StatutOccupationLogementId",
                table: "Clients",
                column: "StatutOccupationLogementId",
                principalTable: "statut",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_statut_StatutOccupationLogementId",
                table: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_statut",
                table: "statut");

            migrationBuilder.RenameTable(
                name: "statut",
                newName: "StatutOccupationLogement");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StatutOccupationLogement",
                table: "StatutOccupationLogement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_StatutOccupationLogement_StatutOccupationLogementId",
                table: "Clients",
                column: "StatutOccupationLogementId",
                principalTable: "StatutOccupationLogement",
                principalColumn: "Id");
        }
    }
}
