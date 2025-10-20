using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedTipoMovimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {// Entradas
            migrationBuilder.InsertData(
                table: "TiposMovimento",
                columns: new[] { "IdTipoMovimento", "DescricaoTipoMovimento", "EspecieMovimentoId" },
                values: new object[,]
                {
                    { 1, "Compra", 1 },
                    { 2, "Entrada Bonif", 1 },
                    { 3, "Devolução de Cliente", 1 }
                });

            // Saídas
            migrationBuilder.InsertData(
                table: "TiposMovimento",
                columns: new[] { "IdTipoMovimento", "DescricaoTipoMovimento", "EspecieMovimentoId" },
                values: new object[,]
                {
                    { 4, "Venda", 2 },
                    { 5, "Saída Bonif", 2 },
                    { 6, "Avaria", 2 }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
