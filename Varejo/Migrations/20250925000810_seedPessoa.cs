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
            migrationBuilder.Sql(@"
INSERT INTO Pessoa 
(NomeRazao, TratamentoFantasia, CpfCnpj, Ddd, Telefone, Email, EhJuridico, EhUsuario, EhCliente, EhFornecedor, Ativo) 
VALUES
('Filipe Alexandre', 'Sr. Filipe', '12345678901', '11', '999988877', 'filipe@example.com', 0, 1, 0, 0, 1)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
