using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class Financeiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EspeciesTitulo",
                columns: table => new
                {
                    IdEspecieTitulo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspeciesTitulo", x => x.IdEspecieTitulo);
                });

            migrationBuilder.InsertData(
    table: "EspeciesTitulo",
    columns: new[] { "IdEspecieTitulo", "Descricao" },
    values: new object[,]
    {
        { 1, "Obrigação" },
        { 2, "Direito" }
    });

            migrationBuilder.CreateTable(
                name: "FormasPagamento",
                columns: table => new
                {
                    IdFormaPagamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescricaoFormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPagamento", x => x.IdFormaPagamento);
                });

            migrationBuilder.CreateTable(
                name: "PrazosPagamento",
                columns: table => new
                {
                    IdPrazoPagamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroParcelas = table.Column<int>(type: "int", nullable: false),
                    IntervaloDias = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrazosPagamento", x => x.IdPrazoPagamento);
                });

            migrationBuilder.CreateTable(
                name: "TitulosFinanceiro",
                columns: table => new
                {
                    IdTituloFinanceiro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Documento = table.Column<int>(type: "int", nullable: false),
                    Parcela = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ValorAberto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Quitado = table.Column<bool>(type: "bit", nullable: false),
                    EspecieTituloId = table.Column<int>(type: "int", nullable: false),
                    FormaPagamentoId = table.Column<int>(type: "int", nullable: true),
                    PrazoPagamentoId = table.Column<int>(type: "int", nullable: true),
                    PessoaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitulosFinanceiro", x => x.IdTituloFinanceiro);
                    table.ForeignKey(
                        name: "FK_TitulosFinanceiro_EspeciesTitulo_EspecieTituloId",
                        column: x => x.EspecieTituloId,
                        principalTable: "EspeciesTitulo",
                        principalColumn: "IdEspecieTitulo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TitulosFinanceiro_FormasPagamento_FormaPagamentoId",
                        column: x => x.FormaPagamentoId,
                        principalTable: "FormasPagamento",
                        principalColumn: "IdFormaPagamento");
                    table.ForeignKey(
                        name: "FK_TitulosFinanceiro_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "IdPessoa");
                    table.ForeignKey(
                        name: "FK_TitulosFinanceiro_PrazosPagamento_PrazoPagamentoId",
                        column: x => x.PrazoPagamentoId,
                        principalTable: "PrazosPagamento",
                        principalColumn: "IdPrazoPagamento");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TitulosFinanceiro_EspecieTituloId",
                table: "TitulosFinanceiro",
                column: "EspecieTituloId");

            migrationBuilder.CreateIndex(
                name: "IX_TitulosFinanceiro_FormaPagamentoId",
                table: "TitulosFinanceiro",
                column: "FormaPagamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TitulosFinanceiro_PessoaId",
                table: "TitulosFinanceiro",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_TitulosFinanceiro_PrazoPagamentoId",
                table: "TitulosFinanceiro",
                column: "PrazoPagamentoId");



        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TitulosFinanceiro");

            migrationBuilder.DropTable(
                name: "EspeciesTitulo");

            migrationBuilder.DropTable(
                name: "FormasPagamento");

            migrationBuilder.DropTable(
                name: "PrazosPagamento");
        }
    }
}
