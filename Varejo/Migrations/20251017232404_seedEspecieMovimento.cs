using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Varejo.Models;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedEspecieMovimento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EstoqueAtual",
                table: "Produtos",
                type: "decimal(18,4)",
                nullable: false,
            defaultValue: 0m);

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
            migrationBuilder.DropColumn(
                name: "EstoqueAtual",
                table: "Produtos");
        }
    }
}
