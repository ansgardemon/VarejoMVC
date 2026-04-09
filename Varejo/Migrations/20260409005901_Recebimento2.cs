using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class Recebimento2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecebimentosItem_Produtos_ProdutoId",
                table: "RecebimentosItem");

            migrationBuilder.DropColumn(
                name: "FatorConversao",
                table: "RecebimentosItem");

            migrationBuilder.DropColumn(
                name: "GerouCusto",
                table: "RecebimentosItem");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoProdutoFornecedor",
                table: "RecebimentosItem",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoEmbalagemId",
                table: "RecebimentosItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormaPagamentoId",
                table: "Recebimentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrazoPagamentoId",
                table: "Recebimentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecebimentosItem_ProdutoEmbalagemId",
                table: "RecebimentosItem",
                column: "ProdutoEmbalagemId");

            migrationBuilder.CreateIndex(
                name: "IX_Recebimentos_FormaPagamentoId",
                table: "Recebimentos",
                column: "FormaPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Recebimentos_PrazoPagamentoId",
                table: "Recebimentos",
                column: "PrazoPagamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recebimentos_FormasPagamento_FormaPagamentoId",
                table: "Recebimentos",
                column: "FormaPagamentoId",
                principalTable: "FormasPagamento",
                principalColumn: "IdFormaPagamento");

            migrationBuilder.AddForeignKey(
                name: "FK_Recebimentos_PrazosPagamento_PrazoPagamentoId",
                table: "Recebimentos",
                column: "PrazoPagamentoId",
                principalTable: "PrazosPagamento",
                principalColumn: "IdPrazoPagamento");

            migrationBuilder.AddForeignKey(
                name: "FK_RecebimentosItem_ProdutosEmbalagem_ProdutoEmbalagemId",
                table: "RecebimentosItem",
                column: "ProdutoEmbalagemId",
                principalTable: "ProdutosEmbalagem",
                principalColumn: "IdProdutoEmbalagem",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecebimentosItem_Produtos_ProdutoId",
                table: "RecebimentosItem",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "IdProduto",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recebimentos_FormasPagamento_FormaPagamentoId",
                table: "Recebimentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Recebimentos_PrazosPagamento_PrazoPagamentoId",
                table: "Recebimentos");

            migrationBuilder.DropForeignKey(
                name: "FK_RecebimentosItem_ProdutosEmbalagem_ProdutoEmbalagemId",
                table: "RecebimentosItem");

            migrationBuilder.DropForeignKey(
                name: "FK_RecebimentosItem_Produtos_ProdutoId",
                table: "RecebimentosItem");

            migrationBuilder.DropIndex(
                name: "IX_RecebimentosItem_ProdutoEmbalagemId",
                table: "RecebimentosItem");

            migrationBuilder.DropIndex(
                name: "IX_Recebimentos_FormaPagamentoId",
                table: "Recebimentos");

            migrationBuilder.DropIndex(
                name: "IX_Recebimentos_PrazoPagamentoId",
                table: "Recebimentos");

            migrationBuilder.DropColumn(
                name: "ProdutoEmbalagemId",
                table: "RecebimentosItem");

            migrationBuilder.DropColumn(
                name: "FormaPagamentoId",
                table: "Recebimentos");

            migrationBuilder.DropColumn(
                name: "PrazoPagamentoId",
                table: "Recebimentos");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoProdutoFornecedor",
                table: "RecebimentosItem",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "FatorConversao",
                table: "RecebimentosItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "GerouCusto",
                table: "RecebimentosItem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_RecebimentosItem_Produtos_ProdutoId",
                table: "RecebimentosItem",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "IdProduto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
