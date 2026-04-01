using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using VarejoSHARED.DTO;

namespace VarejoAPI.Services
{
    public class RelatorioExportService
    {
        // --- RELATÓRIO 101 (PRODUTOS) ---
        public byte[] GerarPdfProdutos(List<ProdutoDTO> produtos)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurarLayoutBase(page, "RELAÇÃO GERAL DE PRODUTOS");

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Nome
                            columns.RelativeColumn(2);   // Categoria
                            columns.ConstantColumn(60);  // Estoque
                            columns.ConstantColumn(80);  // Custo
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(EstiloHeader).Text("ID");
                            header.Cell().Element(EstiloHeader).Text("Produto");
                            header.Cell().Element(EstiloHeader).Text("Categoria");
                            header.Cell().Element(EstiloHeader).Text("Estoque");
                            header.Cell().Element(EstiloHeader).Text("Custo");
                        });

                        foreach (var p in produtos)
                        {
                            table.Cell().Element(EstiloLinha).Text(p.IdProduto.ToString());
                            table.Cell().Element(EstiloLinha).Text(p.NomeProduto);
                            table.Cell().Element(EstiloLinha).Text(p.DescricaoCategoria);
                            table.Cell().Element(EstiloLinha).AlignRight().Text(p.EstoqueAtual.ToString("N2"));
                            table.Cell().Element(EstiloLinha).AlignRight().Text(p.CustoMedio.ToString("C2"));
                        }
                    });
                });
            }).GeneratePdf();
        }

        // --- RELATÓRIO 103 (MOVIMENTAÇÃO POR PRODUTO) ---
        public byte[] GerarPdfMovimentacaoPorProduto(List<Relatorio103DTO> dados)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurarLayoutBase(page, "MOVIMENTAÇÃO DE ESTOQUE POR PRODUTO");

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(75);  // Data
                            columns.RelativeColumn(3);   // Produto
                            columns.ConstantColumn(55);  // Qtd
                            columns.RelativeColumn(2);   // Tipo
                            columns.RelativeColumn(2);   // Pessoa
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(EstiloHeader).Text("Data");
                            header.Cell().Element(EstiloHeader).Text("Produto");
                            header.Cell().Element(EstiloHeader).Text("Qtd");
                            header.Cell().Element(EstiloHeader).Text("Tipo");
                            header.Cell().Element(EstiloHeader).Text("Pessoa");
                        });

                        foreach (var m in dados)
                        {
                            table.Cell().Element(EstiloLinha).Text(m.DataMovimento.ToString("dd/MM/yyyy"));
                            table.Cell().Element(EstiloLinha).Text(m.ProdutoNome);
                            table.Cell().Element(EstiloLinha).AlignRight().Text(m.Quantidade.ToString("N2"));
                            table.Cell().Element(EstiloLinha).Text(m.TipoMovimento);
                            table.Cell().Element(EstiloLinha).Text(m.Pessoa);
                        }
                    });
                });
            }).GeneratePdf();
        }

        // --- AUXILIARES DE ESTILO ---
        private void ConfigurarLayoutBase(PageDescriptor page, string titulo)
        {
            page.Margin(30);
            page.Header().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(titulo).FontSize(16).SemiBold().FontColor("#1a237e");
                    col.Item().Text($"Emissão: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(9).Italic();
                });
            });

            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
            });
        }

        static IContainer EstiloHeader(IContainer container) =>
            container.BorderBottom(1).PaddingVertical(5).DefaultTextStyle(x => x.SemiBold().FontSize(10));

        static IContainer EstiloLinha(IContainer container) =>
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingVertical(5).DefaultTextStyle(x => x.FontSize(9));
    }
}