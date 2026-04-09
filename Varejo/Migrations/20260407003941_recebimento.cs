using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class recebimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracoesEmpresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazaoSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Cnpj = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    NomeFantasia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Logotipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesEmpresa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recebimentos",
                columns: table => new
                {
                    IdRecebimento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PessoaId = table.Column<int>(type: "int", nullable: false),
                    NumeroNota = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChaveAcesso = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recebimentos", x => x.IdRecebimento);
                    table.ForeignKey(
                        name: "FK_Recebimentos_Pessoas_PessoaId",
                        column: x => x.PessoaId,
                        principalTable: "Pessoas",
                        principalColumn: "IdPessoa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutosCusto",
                columns: table => new
                {
                    IdProdutoCusto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    RecebimentoId = table.Column<int>(type: "int", nullable: false),
                    ValorCustoUnitario = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EhCustoAtual = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutosCusto", x => x.IdProdutoCusto);
                    table.ForeignKey(
                        name: "FK_ProdutosCusto_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "IdProduto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutosCusto_Recebimentos_RecebimentoId",
                        column: x => x.RecebimentoId,
                        principalTable: "Recebimentos",
                        principalColumn: "IdRecebimento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecebimentosItem",
                columns: table => new
                {
                    IdRecebimentoItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecebimentoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    CodigoProdutoFornecedor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    EhBonificacao = table.Column<bool>(type: "bit", nullable: false),
                    FatorConversao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GerouCusto = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecebimentosItem", x => x.IdRecebimentoItem);
                    table.ForeignKey(
                        name: "FK_RecebimentosItem_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "IdProduto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecebimentosItem_Recebimentos_RecebimentoId",
                        column: x => x.RecebimentoId,
                        principalTable: "Recebimentos",
                        principalColumn: "IdRecebimento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecebimentosXmlLog",
                columns: table => new
                {
                    IdXmlLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChaveAcesso = table.Column<string>(type: "nvarchar(44)", maxLength: 44, nullable: false),
                    DataImportacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecebimentoId = table.Column<int>(type: "int", nullable: false),
                    XmlCaminho = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecebimentosXmlLog", x => x.IdXmlLog);
                    table.ForeignKey(
                        name: "FK_RecebimentosXmlLog_Recebimentos_RecebimentoId",
                        column: x => x.RecebimentoId,
                        principalTable: "Recebimentos",
                        principalColumn: "IdRecebimento",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosCusto_ProdutoId",
                table: "ProdutosCusto",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutosCusto_RecebimentoId",
                table: "ProdutosCusto",
                column: "RecebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Recebimentos_PessoaId",
                table: "Recebimentos",
                column: "PessoaId");

            migrationBuilder.CreateIndex(
                name: "IX_RecebimentosItem_ProdutoId",
                table: "RecebimentosItem",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecebimentosItem_RecebimentoId",
                table: "RecebimentosItem",
                column: "RecebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecebimentosXmlLog_RecebimentoId",
                table: "RecebimentosXmlLog",
                column: "RecebimentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracoesEmpresa");

            migrationBuilder.DropTable(
                name: "ProdutosCusto");

            migrationBuilder.DropTable(
                name: "RecebimentosItem");

            migrationBuilder.DropTable(
                name: "RecebimentosXmlLog");

            migrationBuilder.DropTable(
                name: "Recebimentos");
        }
    }
}
