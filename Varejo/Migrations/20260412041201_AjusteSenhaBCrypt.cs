using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class AjusteSenhaBCrypt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Aumenta a coluna
            migrationBuilder.Sql("ALTER TABLE Usuarios ALTER COLUMN Senha VARCHAR(255);");

            // Atualiza a senha do ADMIN para 123456 criptografada
            migrationBuilder.Sql("UPDATE Usuarios SET Senha = '$2a$10$XEisFLAKtWNuXT9gcL80nOdJ4egAfdJjnxG7T5amKXV2djq1wMYcG' WHERE nomeUsuario = 'admin';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
