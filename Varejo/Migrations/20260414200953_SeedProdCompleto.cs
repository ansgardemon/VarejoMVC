using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Varejo.Migrations
{
    public partial class SeedProdCompleto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // 1. FORMAS E PRAZOS DE PAGAMENTO
            // =========================================================
            migrationBuilder.InsertData(
                table: "FormasPagamento",
                columns: new[] { "IdFormaPagamento", "DescricaoFormaPagamento" },
                values: new object[,]
                {
                    { 1, "Dinheiro" },
                    { 2, "PIX" },
                    { 3, "Cartão de Débito" },
                    { 4, "Cartão de Crédito" },
                    { 5, "Vale Alimentação" }
                });

            migrationBuilder.InsertData(
                table: "PrazosPagamento",
                columns: new[] { "IdPrazoPagamento", "Descricao", "IntervaloDias", "NumeroParcelas" },
                values: new object[,]
                {
                    { 1, "À Vista", 0, 1 },
                    { 2, "15 Dias", 15, 1 },
                    { 3, "30 Dias", 30, 1 },
                    { 4, "7/14/21 Dias", 7, 3 },
                    { 5, "30/60 Dias", 30, 2 }
                });


            // =========================================================
            // 2. HIERARQUIA BÁSICA (MARCAS E CATEGORIAS)
            // =========================================================
            migrationBuilder.InsertData(
                table: "Marcas",
                columns: new[] { "IdMarca", "NomeMarca" },
                values: new object[,]
                {
                    { 1, "Ambev" },
                    { 2, "Heineken" },
                    { 3, "Diageo" },
                    { 4, "Concha y Toro" },
                    { 5, "Coca-Cola Co." },
                    { 6, "PepsiCo (Elma Chips)" },
                    { 7, "Nestlé" },
                    { 8, "Ferrero" }
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "IdCategoria", "DescricaoCategoria" },
                values: new object[,]
                {
                    { 1, "Cervejas" },
                    { 2, "Destilados" },
                    { 3, "Vinhos" },
                    { 4, "Refrigerantes" },
                    { 5, "Snacks e Salgadinhos" },
                    { 6, "Doces e Chocolates" }
                });


            // =========================================================
            // 3. FAMÍLIAS (Ligando Marca à Categoria)
            // =========================================================
            migrationBuilder.InsertData(
                table: "Familias",
                columns: new[] { "IdFamilia", "Ativo", "CategoriaId", "MarcaId", "NomeFamilia" },
                values: new object[,]
                {
                    { 1, true, 1, 1, "Cerveja Pilsen" },
                    { 2, true, 1, 2, "Cerveja Puro Malte" },
                    { 3, true, 2, 3, "Whisky Escocês" },
                    { 4, true, 3, 4, "Vinho Tinto Reservado" },
                    { 5, true, 4, 5, "Refrigerante de Cola" },
                    { 6, true, 5, 6, "Salgadinho de Milho" },
                    { 7, true, 5, 6, "Batata Chips" },
                    { 8, true, 6, 7, "Chocolate em Barra" },
                    { 9, true, 6, 8, "Bombons Premium" }
                });


            // =========================================================
            // 4. PRODUTOS E VARIAÇÕES
            // =========================================================
            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "IdProduto", "Ativo", "Complemento", "CustoMedio", "EstoqueAtual", "EstoqueInicial", "FamiliaId", "NomeProduto", "UrlImagem" },
                values: new object[,]
                {
                    // Cervejas (Variação de Tamanho)
                    { 1, true, "Lata 350ml", 2.30m, 300m, 300m, 1, "Cerveja Brahma Chopp Lata", "https://dummyimage.com/400x400/e63946/fff.png&text=Brahma+Lata" },
                    { 2, true, "Garrafa 600ml", 5.10m, 120m, 120m, 1, "Cerveja Brahma Chopp Garrafa", "https://dummyimage.com/400x400/e63946/fff.png&text=Brahma+600ml" },
                    { 3, true, "Long Neck 330ml", 4.50m, 200m, 200m, 2, "Cerveja Heineken LN", "https://dummyimage.com/400x400/2a9d8f/fff.png&text=Heineken+LN" },
                    { 4, true, "Lata 350ml", 3.80m, 400m, 400m, 2, "Cerveja Heineken Lata", "https://dummyimage.com/400x400/2a9d8f/fff.png&text=Heineken+Lata" },

                    // Destilados e Vinhos (Variação de Rótulo/Tipo)
                    { 5, true, "Garrafa 1 Litro", 65.00m, 24m, 24m, 3, "Whisky Johnnie Walker Red Label", "https://dummyimage.com/400x400/d62828/fff.png&text=JW+Red+Label" },
                    { 6, true, "Garrafa 1 Litro", 110.00m, 12m, 12m, 3, "Whisky Johnnie Walker Black Label", "https://dummyimage.com/400x400/111111/fff.png&text=JW+Black+Label" },
                    { 7, true, "Garrafa 750ml", 45.00m, 36m, 36m, 4, "Vinho Casillero del Diablo Cabernet", "https://dummyimage.com/400x400/780000/fff.png&text=Casillero+Cabernet" },

                    // Refrigerantes (Variação de Tamanho e Açúcar)
                    { 8, true, "Lata 350ml", 2.80m, 150m, 150m, 5, "Coca-Cola Original Lata", "https://dummyimage.com/400x400/c1121f/fff.png&text=Coca+Lata" },
                    { 9, true, "Lata 350ml", 2.80m, 100m, 100m, 5, "Coca-Cola Zero Lata", "https://dummyimage.com/400x400/000000/fff.png&text=Coca+Zero" },
                    { 10, true, "Pet 2 Litros", 7.20m, 80m, 80m, 5, "Coca-Cola Original 2L", "https://dummyimage.com/400x400/c1121f/fff.png&text=Coca+2L" },

                    // Snacks e Salgadinhos (Variação de Gramatura)
                    { 11, true, "Pacote 140g", 9.50m, 45m, 45m, 6, "Doritos Queijo Nacho 140g", "https://dummyimage.com/400x400/f77f00/fff.png&text=Doritos+140g" },
                    { 12, true, "Pacote 45g", 3.50m, 80m, 80m, 6, "Doritos Queijo Nacho 45g", "https://dummyimage.com/400x400/f77f00/fff.png&text=Doritos+45g" },
                    { 13, true, "Tubo 114g", 11.90m, 30m, 30m, 7, "Batata Pringles Original", "https://dummyimage.com/400x400/d62828/fff.png&text=Pringles+114g" },

                    // Doces e Chocolates
                    { 14, true, "Unidade 41.5g", 2.50m, 100m, 100m, 8, "Chocolate Kit Kat Ao Leite", "https://dummyimage.com/400x400/d90429/fff.png&text=KitKat" },
                    { 15, true, "Caixa T8 100g", 18.00m, 20m, 20m, 9, "Bombom Ferrero Rocher", "https://dummyimage.com/400x400/cc8b00/fff.png&text=Ferrero+Rocher" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // O Down apaga exatamente os IDs que foram inseridos, na ordem inversa (para não violar a chave estrangeira)

            migrationBuilder.DeleteData(table: "Produtos", keyColumn: "IdProduto", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
            migrationBuilder.DeleteData(table: "Familias", keyColumn: "IdFamilia", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            migrationBuilder.DeleteData(table: "Categorias", keyColumn: "IdCategoria", keyValues: new object[] { 1, 2, 3, 4, 5, 6 });
            migrationBuilder.DeleteData(table: "Marcas", keyColumn: "IdMarca", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8 });

            migrationBuilder.DeleteData(table: "PrazosPagamento", keyColumn: "IdPrazoPagamento", keyValues: new object[] { 1, 2, 3, 4, 5 });
            migrationBuilder.DeleteData(table: "FormasPagamento", keyColumn: "IdFormaPagamento", keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}