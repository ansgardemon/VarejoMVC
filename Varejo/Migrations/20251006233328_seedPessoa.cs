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
            migrationBuilder.Sql("INSERT INTO Pessoas (NomeRazao, TratamentoFantasia, CpfCnpj, Ddd, Telefone, Email, EhJuridico,EhUsuario, EhCliente, EhFornecedor, Ativo) VALUES ('Filipe', 'Filipe Master', '123.123.123-12', '11', '(11)91234-5678', 'filipe@mail.com', 1,1,1,1,1),('Matheus', 'Matheus Master', '321.321.321-32', '11', '(11)98765-4321', 'Matheus@mail.com', 1,1,1,1,1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
