using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KoridrawsPI.Migrations
{
    /// <inheritdoc />
    public partial class ResetSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResetSenhaExpiracao",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResetSenhaToken",
                table: "Usuarios",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetSenhaExpiracao",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ResetSenhaToken",
                table: "Usuarios");
        }
    }
}
