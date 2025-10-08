using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedFamilia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Familias (NomeFamilia, Ativo, MarcaId, CategoriaId) VALUES ('Vinho Tinto, 1, 1, 1'),('Licor', 1, 2, 2),('Tabaco', 0, 3, 3)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
