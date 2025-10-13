using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedPessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Pessoas",
               columns: new[]
               {
                    "NomeRazao",
                    "TratamentoFantasia",
                    "CpfCnpj",
                    "Ddd",
                    "Telefone",
                    "Email",
                    "EhJuridico",
                    "EhUsuario",
                    "EhCliente",
                    "EhFornecedor",
                    "Ativo"
               },
               values: new object[,]
               {
                    { "Administrador do Sistema", "Admin", "00000000000", "11", "90000-0001", "admin@varejo.com", false, true, false, false, true },
                    { "Gerente de Vendas", "Gerente", "11111111111", "11", "90000-0002", "gerente@varejo.com", false, true, false, false, true },
                    { "João da Silva", "João", "22222222222", "11", "90000-0003", "joao@varejo.com", false, true, false, false, true },
                    { "Maria Oliveira", "Maria", "33333333333", "11", "90000-0004", "maria@varejo.com", false, true, false, false, true },
                    { "Funcionário de Teste", "Func", "44444444444", "11", "90000-0005", "func@varejo.com", false, true, false, false, true }
               }
           );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
