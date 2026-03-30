using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class EspecieMovimentoAjuste : Migration
    {
        /// <inheritdoc />
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
         table: "EspeciesMovimento",
         columns: new[] { "DescricaoEspecieMovimento" },
         values: new object[,]
         {
                    {"Ajuste"}

         }


     );

            migrationBuilder.InsertData(
                    table: "TiposMovimento",
                    columns: new[] { "IdTipoMovimento", "DescricaoTipoMovimento", "EspecieMovimentoId" },
                    values: new object[,]
                    {
                    { 7, "Ajuste", 3 }

                    });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
