using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedTipoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
table: "TiposUsuario",
columns: new[] { "DescricaoTipoUsuario" },
values: new object[,]
{
                    {"Administrador"},
                    {"Gerente"},
                    {"Usuário"},
                    {"Cliente"}
}
);




        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
