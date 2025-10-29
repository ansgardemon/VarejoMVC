using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedEspecieMovimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
         table: "EspeciesMovimento",
         columns: new[] { "DescricaoEspecieMovimento" },
         values: new object[,]
         {
                    {"Entrada"},
                    {"Saída"}

         }
     );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
