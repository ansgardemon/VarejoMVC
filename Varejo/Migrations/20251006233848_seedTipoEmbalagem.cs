using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedTipoEmbalagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO TipoEmbalagem (DescricaoTipoEmbalagem, Multiplicador) VALUES ('Garrafa 750ml', 1), ('Garrafa 500ml', 1), ('Caixa 10 unidades', 10);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
