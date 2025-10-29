using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    /// <inheritdoc />
    public partial class seedTipoEmbalagems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
table: "TiposEmbalagem",
columns: new[] { "DescricaoTipoEmbalagem", "Multiplicador" },
values: new object[,]
{
                    {"Unidade", 1 },
                    {"Caixa C/6", 6 },
                    {"Caixa C/12", 12 },
                     { "Caixa C/24", 24 },
                    {"Fardo C/6", 6 },
                    {"Fardo C/12", 12 },
                    {"Fardo C/24", 24 },
                    {"Pacote C/10", 10 },
                    {"Pacote C/20", 20 },
                    {"Pacote C/50", 50 },
                    {"Lote C/100", 100 }
}
);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
