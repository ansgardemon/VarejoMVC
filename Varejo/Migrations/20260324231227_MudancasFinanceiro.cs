using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class MudancasFinanceiro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorPago",
                table: "TitulosFinanceiro");

            migrationBuilder.CreateTable(
                name: "PagamentosTitulo",
                columns: table => new
                {
                    IdPagamento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TituloFinanceiroId = table.Column<int>(type: "int", nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosTitulo", x => x.IdPagamento);
                    table.ForeignKey(
                        name: "FK_PagamentosTitulo_TitulosFinanceiro_TituloFinanceiroId",
                        column: x => x.TituloFinanceiroId,
                        principalTable: "TitulosFinanceiro",
                        principalColumn: "IdTituloFinanceiro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosTitulo_TituloFinanceiroId",
                table: "PagamentosTitulo",
                column: "TituloFinanceiroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagamentosTitulo");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPago",
                table: "TitulosFinanceiro",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
