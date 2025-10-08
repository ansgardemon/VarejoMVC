using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedProdutoMovimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO ProdutoMovimento (Quantidade, ProdutoId, ProdutoEmbalagemId, MovimentoId) VALUES (10, 1, 1, 1), (5, 2, 2, 1), (2, 3, 3, 2);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
