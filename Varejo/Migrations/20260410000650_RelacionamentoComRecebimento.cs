using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class RelacionamentoComRecebimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecebimentoId",
                table: "TitulosFinanceiro",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecebimentoId",
                table: "Movimentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TitulosFinanceiro_RecebimentoId",
                table: "TitulosFinanceiro",
                column: "RecebimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_RecebimentoId",
                table: "Movimentos",
                column: "RecebimentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentos_Recebimentos_RecebimentoId",
                table: "Movimentos",
                column: "RecebimentoId",
                principalTable: "Recebimentos",
                principalColumn: "IdRecebimento");

            migrationBuilder.AddForeignKey(
                name: "FK_TitulosFinanceiro_Recebimentos_RecebimentoId",
                table: "TitulosFinanceiro",
                column: "RecebimentoId",
                principalTable: "Recebimentos",
                principalColumn: "IdRecebimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimentos_Recebimentos_RecebimentoId",
                table: "Movimentos");

            migrationBuilder.DropForeignKey(
                name: "FK_TitulosFinanceiro_Recebimentos_RecebimentoId",
                table: "TitulosFinanceiro");

            migrationBuilder.DropIndex(
                name: "IX_TitulosFinanceiro_RecebimentoId",
                table: "TitulosFinanceiro");

            migrationBuilder.DropIndex(
                name: "IX_Movimentos_RecebimentoId",
                table: "Movimentos");

            migrationBuilder.DropColumn(
                name: "RecebimentoId",
                table: "TitulosFinanceiro");

            migrationBuilder.DropColumn(
                name: "RecebimentoId",
                table: "Movimentos");
        }
    }
}
