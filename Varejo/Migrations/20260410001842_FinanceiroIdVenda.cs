using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class FinanceiroIdVenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendaId",
                table: "TitulosFinanceiro",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TitulosFinanceiro_VendaId",
                table: "TitulosFinanceiro",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TitulosFinanceiro_Vendas_VendaId",
                table: "TitulosFinanceiro",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "IdVenda");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TitulosFinanceiro_Vendas_VendaId",
                table: "TitulosFinanceiro");

            migrationBuilder.DropIndex(
                name: "IX_TitulosFinanceiro_VendaId",
                table: "TitulosFinanceiro");

            migrationBuilder.DropColumn(
                name: "VendaId",
                table: "TitulosFinanceiro");
        }
    }
}
