using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Produtos (Complemento, NomeProduto, Ativo, UrlImagem, CustoMedio, FamiliaId) VALUES ('Safra 2018', 'Vinho Merlot', 1, 'imagem01', 45.50, 1),('Maracujá', 'Licor', 1, 'imagem02', 25.00, 2),('Tabaco Especial', 'Charuto', 0, 'imagem03', 15.75, 3);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
