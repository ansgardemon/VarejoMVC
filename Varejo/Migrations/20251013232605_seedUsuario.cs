using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "nomeUsuario", "Senha", "Ativo", "PessoaId", "TipoUsuarioId" },
                values: new object[,]
                {
                    { "admin", "1234", true, 1, 1 },
                    { "gerente01", "1234", true, 2, 2 },
                    { "joao", "senha123", true, 3, 3 },
                    { "maria", "abc123", true, 4, 3 },
                    { "funcionario01", "teste", true, 5, 3 }
                }
            );

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
