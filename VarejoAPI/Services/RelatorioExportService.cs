using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VarejoSHARED.DTO;

namespace VarejoAPI.Services
{
    public class RelatorioExportService
    {
        #region RELATÓRIO 101 - PRODUTOS
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

                    page.Header().Element(c => GerarCabecalho(c, "RELATÓRIO DE ESTOQUE", "#101"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(60);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("CATEGORIA");
                            header.Cell().Element(HeaderStyle).Text("MARCA");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("ESTOQUE");
                        });

                        foreach (var item in produtos)
                        {
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "-");
                            table.Cell().Element(RowStyle).Text(item.DescricaoCategoria ?? "-");
                            table.Cell().Element(RowStyle).Text(item.NomeMarca ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.EstoqueAtual.ToString("N2"));
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, produtos.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion


        #region RELATÓRIO 102 - PRODUTOS POR VALORES
        public byte[] GerarPdfProdutosValores(List<Relatorio102DTO> produtos)
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

                    page.Header().Element(c => GerarCabecalho(c, "PRODUTOS POR VALORES", "#102"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.ConstantColumn(50);
                            columns.ConstantColumn(50);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("EMB.");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("CUSTO");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("PREÇO");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("MARGEM");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("MARKUP");
                        });

                        foreach (var item in produtos)
                        {
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "-");
                            table.Cell().Element(RowStyle).Text(item.Embalagem ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.CustoMedio.ToString("C2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.PrecoVenda.ToString("C2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.MargemBruta.ToString("N2") + "%");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Markup.ToString("N2") + "%");
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, produtos.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion


        #region RELATÓRIO 103 - MOVIMENTO DE ESTOQUE
        public byte[] GerarPdfRelatorio103(List<Relatorio103DTO> movimentos)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily(Fonts.Verdana));

                    page.Header().Element(c => GerarCabecalho(c, "MOVIMENTO DE ESTOQUE POR PRODUTO", "#103"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(50);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(3);
                            columns.ConstantColumn(40);
                            columns.ConstantColumn(60);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("DATA");
                            header.Cell().Element(HeaderStyle).Text("Nº MOV");
                            header.Cell().Element(HeaderStyle).Text("TIPO MOVIMENTO");
                            header.Cell().Element(HeaderStyle).Text("PESSOA");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("UNID");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("QTD");
                        });

                        foreach (var item in movimentos)
                        {
                            table.Cell().Element(RowStyle).Text(item.DataMovimento.ToString("dd/MM/yyyy"));
                            table.Cell().Element(RowStyle).Text(item.IdMovimento.ToString());
                            table.Cell().Element(RowStyle).Text(item.TipoMovimento ?? "-");
                            table.Cell().Element(RowStyle).Text(item.Pessoa ?? "-");
                            table.Cell().Element(RowStyle).Text(item.ProdutoNome ?? "-");
                            table.Cell().Element(RowStyle).Text(item.Embalagem ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Quantidade.ToString("N2"));
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, movimentos.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion


        #region MÉTODOS AUXILIARES

        private void GerarCabecalho(IContainer container, string titulo, string codigoRelatorio)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(titulo).FontSize(22).ExtraBold().FontColor("#163889");
                    col.Item().Text(text =>
                    {
                        text.Span("Data de emissão: ").SemiBold();
                        text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });

                row.ConstantItem(100).AlignRight().Column(col =>
                {
                    col.Item().Text(codigoRelatorio).FontSize(14).Bold().FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void GerarRodape(IContainer container, int totalRegistros)
        {
            container.PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Text(x =>
                {
                    x.Span("Total de registros: ").SemiBold();
                    x.Span(totalRegistros.ToString());
                });

                row.RelativeItem().AlignRight().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        }

        private static IContainer HeaderStyle(IContainer container) =>
            container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                     .PaddingVertical(5)
                     .PaddingHorizontal(5)
                     .Background("#163889");

        private static IContainer RowStyle(IContainer container) =>
            container.PaddingVertical(5)
                     .PaddingHorizontal(5)
                     .BorderBottom(1)
                     .BorderColor(Colors.Grey.Lighten3);

        #endregion
    }
}