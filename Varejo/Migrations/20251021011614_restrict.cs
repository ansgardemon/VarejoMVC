using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class restrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Familias_Marcas_MarcaId",
                table: "Familias");

            migrationBuilder.AddForeignKey(
                name: "FK_Familias_Marcas_MarcaId",
                table: "Familias",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "IdMarca",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Familias_Marcas_MarcaId",
                table: "Familias");

            migrationBuilder.AddForeignKey(
                name: "FK_Familias_Marcas_MarcaId",
                table: "Familias",
                column: "MarcaId",
                principalTable: "Marcas",
                principalColumn: "IdMarca");
        }
    }
}
