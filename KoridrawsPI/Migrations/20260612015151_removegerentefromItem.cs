using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KoridrawsPI.Migrations
{
    /// <inheritdoc />
    public partial class removegerentefromItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Itens_Gerentes_GerenteId",
                table: "Itens");

            migrationBuilder.DropIndex(
                name: "IX_Itens_GerenteId",
                table: "Itens");

            migrationBuilder.DropColumn(
                name: "GerenteId",
                table: "Itens");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GerenteId",
                table: "Itens",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Itens_GerenteId",
                table: "Itens",
                column: "GerenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Itens_Gerentes_GerenteId",
                table: "Itens",
                column: "GerenteId",
                principalTable: "Gerentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
