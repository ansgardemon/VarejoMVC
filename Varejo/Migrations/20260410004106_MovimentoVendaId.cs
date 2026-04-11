using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class MovimentoVendaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VendaId",
                table: "Movimentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movimentos_VendaId",
                table: "Movimentos",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentos_Vendas_VendaId",
                table: "Movimentos",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "IdVenda");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimentos_Vendas_VendaId",
                table: "Movimentos");

            migrationBuilder.DropIndex(
                name: "IX_Movimentos_VendaId",
                table: "Movimentos");

            migrationBuilder.DropColumn(
                name: "VendaId",
                table: "Movimentos");
        }
    }
}
