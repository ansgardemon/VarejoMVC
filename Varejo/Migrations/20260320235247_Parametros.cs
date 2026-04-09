using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class Parametros : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Parametros",
                columns: table => new
                {
                    IdParametroMovimento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoMovimentoVendaId = table.Column<int>(type: "int", nullable: false),
                    TipoMovimentoCompraId = table.Column<int>(type: "int", nullable: false),
                    TipoMovimentoAvariaId = table.Column<int>(type: "int", nullable: false),
               
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.IdParametroMovimento);
                    table.ForeignKey(
                        name: "FK_Parametros_TiposMovimento_TipoMovimentoAvariaId",
                        column: x => x.TipoMovimentoAvariaId,
                        principalTable: "TiposMovimento",
                        principalColumn: "IdTipoMovimento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parametros_TiposMovimento_TipoMovimentoCompraId",
                        column: x => x.TipoMovimentoCompraId,
                        principalTable: "TiposMovimento",
                        principalColumn: "IdTipoMovimento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Parametros_TiposMovimento_TipoMovimentoVendaId",
                        column: x => x.TipoMovimentoVendaId,
                        principalTable: "TiposMovimento",
                        principalColumn: "IdTipoMovimento",
                        onDelete: ReferentialAction.Restrict);
                
         
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_TipoMovimentoAvariaId",
                table: "Parametros",
                column: "TipoMovimentoAvariaId");

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_TipoMovimentoCompraId",
                table: "Parametros",
                column: "TipoMovimentoCompraId");

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_TipoMovimentoVendaId",
                table: "Parametros",
                column: "TipoMovimentoVendaId");
           
     


            // SEED AQUI
            migrationBuilder.InsertData(
                table: "Parametros",
                columns: new[]
                {
            "IdParametroMovimento",
            "TipoMovimentoVendaId",
            "TipoMovimentoCompraId",
            "TipoMovimentoAvariaId"
                },
                values: new object[]
                {
            1, // ID fixo do parâmetro
            4, // Venda
            1, // Compra
            6
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Parametros",
                keyColumn: "IdParametroMovimento",
                keyValue: 1
            );

            migrationBuilder.DropTable(
                name: "Parametros");
        }
    }
}
