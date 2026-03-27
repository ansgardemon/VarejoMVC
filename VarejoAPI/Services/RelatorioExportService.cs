using NuGet.Packaging;
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
            // QuestPDF precisa de uma licença definida (Community é grátis para uso pessoal/estudos)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // CABEÇALHO
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("RELATÓRIO DE ESTOQUE").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });

                    // CONTEÚDO (TABELA)
                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50); // ID
                            columns.RelativeColumn();   // Produto
                            columns.RelativeColumn();   // Categoria
                            columns.ConstantColumn(80); // Estoque
                        });

                        // Header da Tabela
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID");
                            header.Cell().Element(CellStyle).Text("Produto");
                            header.Cell().Element(CellStyle).Text("Categoria");
                            header.Cell().Element(CellStyle).Text("Estoque");

                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        });

                        // Linhas da Tabela
                        // Linhas da Tabela
                        foreach (var item in produtos)
                        {
                            // ID (Sempre tem valor pois é int, mas convertemos com segurança)
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());

                            // Produto (Se for nulo no banco, vira string vazia "")
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "");

                            // Categoria (Se for nulo, vira string vazia)
                            table.Cell().Element(RowStyle).Text(item.DescricaoCategoria ?? "");

                            // Marca (Caso você tenha adicionado essa coluna)
                            table.Cell().Element(RowStyle).Text(item.NomeMarca ?? "Sem Marca");

                            // Estoque (Decimal não é nulo, mas garantimos a formatação)
                            table.Cell().Element(RowStyle).Text(item.EstoqueAtual.ToString("N2"));

                            static IContainer RowStyle(IContainer container) =>
                                container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return documento.GeneratePdf();
        }
    }
}