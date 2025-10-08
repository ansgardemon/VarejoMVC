using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedProdutoEmbalagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO ProdutoEmbalagem (Preco, Ean, ProdutoId, TipoEmbalagemId) VALUES(49.90, '7891234500012', 1, 1),(27.50, '7891234500029', 2, 2),(16.00, '7891234500036', 3, 3);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
