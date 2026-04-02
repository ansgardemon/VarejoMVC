using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class Vendas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    IdVenda = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataVenda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorSubtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DescontoTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Finalizada = table.Column<bool>(type: "bit", nullable: false),
                    PessoaId = table.Column<int>(type: "int", nullable: false),
                    FormaPagamentoId = table.Column<int>(type: "int", nullable: false),
                    PrazoPagamentoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.IdVenda);
                    table.ForeignKey(
                        name: "FK_Vendas_FormasPagamento_FormaPagamentoId",
                        column: x => x.FormaPagamentoId,
                        principalTable: "FormasPagamento",
                        principalColumn: "IdFormaPagamento",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vendas_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "IdPessoa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vendas_PrazosPagamento_PrazoPagamentoId",
                        column: x => x.PrazoPagamentoId,
                        principalTable: "PrazosPagamento",
                        principalColumn: "IdPrazoPagamento");
                });

            migrationBuilder.CreateTable(
                name: "VendasItem",
                columns: table => new
                {
                    IdVendaItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoEmbalagemId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DescontoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendasItem", x => x.IdVendaItem);
                    table.ForeignKey(
                        name: "FK_VendasItem_ProdutosEmbalagem_ProdutoEmbalagemId",
                        column: x => x.ProdutoEmbalagemId,
                        principalTable: "ProdutosEmbalagem",
                        principalColumn: "IdProdutoEmbalagem",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendasItem_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "IdProduto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendasItem_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "IdVenda",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_FormaPagamentoId",
                table: "Vendas",
                column: "FormaPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_PessoaId",
                table: "Vendas",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_PrazoPagamentoId",
                table: "Vendas",
                column: "PrazoPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_VendasItem_ProdutoEmbalagemId",
                table: "VendasItem",
                column: "ProdutoEmbalagemId");

            migrationBuilder.CreateIndex(
                name: "IX_VendasItem_ProdutoId",
                table: "VendasItem",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_VendasItem_VendaId",
                table: "VendasItem",
                column: "VendaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendasItem");

            migrationBuilder.DropTable(
                name: "Vendas");
        }
    }
}
