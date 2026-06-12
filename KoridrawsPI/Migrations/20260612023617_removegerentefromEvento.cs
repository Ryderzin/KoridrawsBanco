using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KoridrawsPI.Migrations
{
    /// <inheritdoc />
    public partial class removegerentefromEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Gerentes_GerenteId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_GerenteId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "GerenteId",
                table: "Eventos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GerenteId",
                table: "Eventos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_GerenteId",
                table: "Eventos",
                column: "GerenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Gerentes_GerenteId",
                table: "Eventos",
                column: "GerenteId",
                principalTable: "Gerentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
