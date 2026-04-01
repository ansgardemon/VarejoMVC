using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VarejoSHARED.DTO;

namespace VarejoAPI.Services
{
    public class RelatorioExportService
    {
        public byte[] GerarPdfProdutos(List<ProdutoDTO> produtos)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Verdana));

                    // CABEÇALHO PROFISSIONAL
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("RELATÓRIO DE ESTOQUE").FontSize(22).ExtraBold().FontColor("#163889");
                            col.Item().Text(text =>
                            {
                                text.Span("Data de emissão: ").SemiBold();
                                text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                            });
                        });

                        row.ConstantItem(100).AlignRight().Column(col => {
                            col.Item().Text("#101").FontSize(14).Bold().FontColor(Colors.Grey.Medium);
                        });
                    });

                    // CONTEÚDO DA TABELA
                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        // DEFINIÇÃO DAS 5 COLUNAS (Ajustado para não sobrar/faltar)
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Produto (Mais largo)
                            columns.RelativeColumn(2);   // Categoria
                            columns.RelativeColumn(2);   // Marca
                            columns.ConstantColumn(60);  // Estoque
                        });

                        // HEADER DA TABELA (Estilizado)
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("CATEGORIA");
                            header.Cell().Element(HeaderStyle).Text("MARCA");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("ESTOQUE");

                            static IContainer HeaderStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                                         .PaddingVertical(5)
                                         .PaddingHorizontal(5)
                                         .Background("#163889"); // Azul do seu sistema
                        });

                        // LINHAS (Dados)
                        foreach (var item in produtos)
                        {
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "-");
                            table.Cell().Element(RowStyle).Text(item.DescricaoCategoria ?? "-");
                            table.Cell().Element(RowStyle).Text(item.NomeMarca ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.EstoqueAtual.ToString("N2"));

                            static IContainer RowStyle(IContainer container) =>
                                container.PaddingVertical(5)
                                         .PaddingHorizontal(5)
                                         .BorderBottom(1)
                                         .BorderColor(Colors.Grey.Lighten3);
                        }
                    });

                    // RODAPÉ
                    page.Footer().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text(x =>
                        {
                            x.Span("Total de registros: ").SemiBold();
                            x.Span(produtos.Count.ToString());
                        });

                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" de ");
                            x.TotalPages();
                        });
                    });
                });
            });

            return documento.GeneratePdf();
        }
    }
}