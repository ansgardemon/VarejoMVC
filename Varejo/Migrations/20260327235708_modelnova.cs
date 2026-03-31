using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class modelnova : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProdutoEmbalagemId",
                table: "InventariosItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InventariosItem_ProdutoEmbalagemId",
                table: "InventariosItem",
                column: "ProdutoEmbalagemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventariosItem_ProdutosEmbalagem_ProdutoEmbalagemId",
                table: "InventariosItem",
                column: "ProdutoEmbalagemId",
                principalTable: "ProdutosEmbalagem",
                principalColumn: "IdProdutoEmbalagem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventariosItem_ProdutosEmbalagem_ProdutoEmbalagemId",
                table: "InventariosItem");

            migrationBuilder.DropIndex(
                name: "IX_InventariosItem_ProdutoEmbalagemId",
                table: "InventariosItem");

            migrationBuilder.DropColumn(
                name: "ProdutoEmbalagemId",
                table: "InventariosItem");
        }
    }
}
