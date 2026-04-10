using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class Parametros2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Adiciona a coluna permitindo nulo temporariamente para não travar nos dados existentes
            migrationBuilder.AddColumn<int>(
                name: "TipoMovimentoEntradaBonificacaoId",
                table: "Parametros",
                type: "int",
                nullable: true); // Mudamos para true aqui primeiro

            // 2. Cria o Índice e a FK
            migrationBuilder.CreateIndex(
                name: "IX_Parametros_TipoMovimentoEntradaBonificacaoId",
                table: "Parametros",
                column: "TipoMovimentoEntradaBonificacaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parametros_TiposMovimento_TipoMovimentoEntradaBonificacaoId",
                table: "Parametros",
                column: "TipoMovimentoEntradaBonificacaoId",
                principalTable: "TiposMovimento",
                principalColumn: "IdTipoMovimento",
                onDelete: ReferentialAction.Restrict); // Use Restrict para evitar deleções em cascata em parâmetros

            // 3. SEED: Atualiza o registro ID 1 com o valor correto (No seu caso, o ID 2 que é 'Entrada Bonif')
            // Use o nome exato que aparece no seu banco de dados
            migrationBuilder.Sql("UPDATE Parametros SET TipoMovimentoEntradaBonificacaoId = 2 WHERE IdParametroMovimento = 1");

            // 4. Agora que o valor está lá, alteramos a coluna para NOT NULL
            migrationBuilder.AlterColumn<int>(
                name: "TipoMovimentoEntradaBonificacaoId",
                table: "Parametros",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parametros_TiposMovimento_TipoMovimentoEntradaBonificacaoId",
                table: "Parametros");

            migrationBuilder.DropIndex(
                name: "IX_Parametros_TipoMovimentoEntradaBonificacaoId",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "TipoMovimentoEntradaBonificacaoId",
                table: "Parametros");
        }
    }
}
