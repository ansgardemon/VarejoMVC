using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class HistoricoProdutoMudanca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoMovimentoId",
                table: "HistoricosProduto",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosProduto_EspecieMovimentoId",
                table: "HistoricosProduto",
                column: "EspecieMovimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosProduto_MovimentoId",
                table: "HistoricosProduto",
                column: "MovimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosProduto_TipoMovimentoId",
                table: "HistoricosProduto",
                column: "TipoMovimentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosProduto_EspeciesMovimento_EspecieMovimentoId",
                table: "HistoricosProduto",
                column: "EspecieMovimentoId",
                principalTable: "EspeciesMovimento",
                principalColumn: "IdEspecieMovimento",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosProduto_Movimentos_MovimentoId",
                table: "HistoricosProduto",
                column: "MovimentoId",
                principalTable: "Movimentos",
                principalColumn: "IdMovimento",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosProduto_TiposMovimento_TipoMovimentoId",
                table: "HistoricosProduto",
                column: "TipoMovimentoId",
                principalTable: "TiposMovimento",
                principalColumn: "IdTipoMovimento",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosProduto_EspeciesMovimento_EspecieMovimentoId",
                table: "HistoricosProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosProduto_Movimentos_MovimentoId",
                table: "HistoricosProduto");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosProduto_TiposMovimento_TipoMovimentoId",
                table: "HistoricosProduto");

            migrationBuilder.DropIndex(
                name: "IX_HistoricosProduto_EspecieMovimentoId",
                table: "HistoricosProduto");

            migrationBuilder.DropIndex(
                name: "IX_HistoricosProduto_MovimentoId",
                table: "HistoricosProduto");

            migrationBuilder.DropIndex(
                name: "IX_HistoricosProduto_TipoMovimentoId",
                table: "HistoricosProduto");

            migrationBuilder.DropColumn(
                name: "TipoMovimentoId",
                table: "HistoricosProduto");
        }
    }
}
