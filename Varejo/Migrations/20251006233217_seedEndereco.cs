using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql("INSERT INTO Enderecos (Logradouro, Cep, Bairro, Cidade, Uf, Complemento, Numero, PessoaId) VALUES ('Av. Paulista','00000-001','Bela Vista','São Paulo','SP','Apto 001','01',1),('Av. Carioca','00000-002','Péssima Vista','Rio de Janeiro','RJ','Apto 002','02',2),('Av. Mineira','00000-003','Belo Pão de Queijo','Minas Gerais','MG','Apto 003','03',3)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
