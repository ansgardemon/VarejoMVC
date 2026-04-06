using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VarejoSHARED.DTO;
using VarejoSHARED.DTO.Relatorios;

namespace VarejoAPI.Services
{
    public class RelatorioExportService
    {
        #region RELATÓRIO 101 - PRODUTOS
        public byte[] GerarPdfRelatorio101(List<ProdutoDTO> produtos)
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

        #region RELATÓRIO 102 - PRECIFICAÇÃO E MARGENS DE LUCRO
        public byte[] GerarPdfRelatorio102(List<Relatorio102DTO> produtos)
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

                    page.Header().Element(c => GerarCabecalho(c, "PRECIFICAÇÃO E MARGENS DE LUCRO", "#102"));

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

        #region RELATÓRIO 104 - CURVA ABC DE PRODUTOS
        public byte[] GerarPdfRelatorio104(List<Relatorio104DTO> dados)
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

                    page.Header().Element(c => GerarCabecalho(c, "CURVA ABC DE PRODUTOS", "#104"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Produto
                            columns.RelativeColumn(2);   // Categoria
                            columns.RelativeColumn(1);   // Qtd
                            columns.RelativeColumn(2);   // Faturamento
                            columns.RelativeColumn(1);   // % Acumulado
                            columns.ConstantColumn(40);  // Curva
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("CATEGORIA");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("QTD");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("FATURAMENTO");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("% ACUM.");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("CURVA");
                        });

                        foreach (var item in dados)
                        {
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "-");
                            table.Cell().Element(RowStyle).Text(item.Categoria ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.QuantidadeVendida.ToString("N2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Faturamento.ToString("C2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.PercentualAcumulado.ToString("N2") + "%");

                            // Coluna da Letra A, B ou C (Centralizada e em Negrito)
                            table.Cell().Element(RowStyle).AlignCenter().Text(item.ClasseABC).SemiBold();
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, dados.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion

        #region RELATÓRIO 105 - PRODUTOS SEM GIRO
        public byte[] GerarPdfRelatorio105(List<Relatorio105DTO> dados, int diasCorte)
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

                    page.Header().Element(c => GerarCabecalho(c, $"PRODUTOS SEM GIRO (HÁ {diasCorte}+ DIAS)", "#105"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Produto
                            columns.RelativeColumn(2);   // Categoria
                            columns.ConstantColumn(60);  // Estoque
                            columns.ConstantColumn(80);  // Última Venda
                            columns.ConstantColumn(70);  // Dias Parado
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("CATEGORIA");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("ESTOQUE");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("ÚLT. VENDA");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("DIAS");
                        });

                        foreach (var item in dados)
                        {
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "-");
                            table.Cell().Element(RowStyle).Text(item.Categoria ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.EstoqueAtual.ToString("N2"));
                            table.Cell().Element(RowStyle).AlignCenter().Text(item.UltimaVenda.HasValue ? item.UltimaVenda.Value.ToString("dd/MM/yyyy") : "Nunca");

                            // Destaca em vermelho se estiver parado há mais de 90 dias
                            var styleDias = item.DiasParado > 90 ? Colors.Red.Medium : Colors.Black;
                            table.Cell().Element(RowStyle).AlignCenter().Text(item.DiasParado == 999 ? "S/ Reg." : $"{item.DiasParado} d").FontColor(styleDias).Bold();
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, dados.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion

        #region RELATÓRIO 106 - RANKING DE VENDAS
        public byte[] GerarPdfRelatorio106(List<Relatorio106DTO> dados, string titulo)
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

                    page.Header().Element(c => GerarCabecalho(c, titulo, "#106"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // POS
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Produto
                            columns.RelativeColumn(2);   // Categoria
                            columns.RelativeColumn(1);   // Qtd
                            columns.RelativeColumn(1);   // Faturamento
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("POS");
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("CATEGORIA");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("QTD");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("FATURAMENTO");
                        });

                        int posicao = 1;
                        foreach (var item in dados)
                        {
                            table.Cell().Element(RowStyle).AlignCenter().Text(posicao.ToString() + "º").SemiBold();
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "-");
                            table.Cell().Element(RowStyle).Text(item.Categoria ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.QuantidadeVendida.ToString("N2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.Faturamento.ToString("C2"));
                            posicao++;
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, dados.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion

        #region RELATÓRIO 107 - HISTÓRICO DE ALTERAÇÃO DE PREÇOS
        public byte[] GerarPdfRelatorio107(List<Relatorio107DTO> dados)
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

                    page.Header().Element(c => GerarCabecalho(c, "HISTÓRICO DE ALTERAÇÃO DE PREÇOS", "#107"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);  // Data
                            columns.RelativeColumn(3);   // Produto
                            columns.RelativeColumn(1);   // Antigo
                            columns.RelativeColumn(1);   // Novo
                            columns.ConstantColumn(50);  // % Variação
                            columns.RelativeColumn(1);   // Usuario
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("DATA");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("PREÇO ANT.");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("PREÇO NOVO");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("VAR. %");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("USUÁRIO");
                        });

                        foreach (var item in dados)
                        {
                            table.Cell().Element(RowStyle).Text(item.DataAlteracao.ToString("dd/MM/yy HH:mm"));
                            table.Cell().Element(RowStyle).Text(item.NomeProduto ?? "-");
                            table.Cell().Element(RowStyle).AlignRight().Text(item.PrecoAnterior.ToString("C2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.PrecoNovo.ToString("C2")).SemiBold();

                            // Lógica de Cor (Verde para queda, Vermelho para aumento)
                            var corVariacao = item.VariacaoPercentual > 0 ? Colors.Red.Medium : (item.VariacaoPercentual < 0 ? Colors.Green.Medium : Colors.Black);
                            var sinal = item.VariacaoPercentual > 0 ? "+" : "";

                            table.Cell().Element(RowStyle).AlignRight().Text($"{sinal}{item.VariacaoPercentual:N2}%").FontColor(corVariacao).Bold();
                            table.Cell().Element(RowStyle).AlignCenter().Text(item.Usuario);
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, dados.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion

        #region RELATÓRIO 201 - POSIÇÃO ATUAL DE ESTOQUE
        public byte[] GerarPdfRelatorio201(List<Relatorio201DTO> dados)
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

                    page.Header().Element(c => GerarCabecalho(c, "POSIÇÃO ATUAL DE ESTOQUE", "#201"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Produto
                            columns.RelativeColumn(2);   // Categoria
                            columns.RelativeColumn(1);   // Estoque
                            columns.RelativeColumn(1);   // Custo Un.
                            columns.RelativeColumn(1);   // Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("CATEGORIA");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("ESTOQUE");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("CUSTO UN.");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("TOTAL");
                        });

                        foreach (var item in dados)
                        {
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto);
                            table.Cell().Element(RowStyle).Text(item.Categoria);

                            var corEstoque = item.EstoqueAtual < 0 ? Colors.Red.Medium : Colors.Black;
                            table.Cell().Element(RowStyle).AlignRight().Text(item.EstoqueAtual.ToString("N2")).FontColor(corEstoque).SemiBold();

                            table.Cell().Element(RowStyle).AlignRight().Text(item.CustoMedio.ToString("C2"));
                            table.Cell().Element(RowStyle).AlignRight().Text(item.ValorTotalCusto.ToString("C2")).SemiBold();
                        }

                        // Linha de Totalização Patrimonial
                        table.Cell().ColumnSpan(5).PaddingVertical(5).PaddingHorizontal(5).AlignRight().Text("VALOR TOTAL EM ESTOQUE:").SemiBold().FontSize(10);
                        table.Cell().PaddingVertical(5).PaddingHorizontal(5).AlignRight().Text(dados.Sum(x => x.ValorTotalCusto).ToString("C2")).SemiBold().FontSize(10).FontColor("#163889");
                    });

                    page.Footer().Element(c => GerarRodape(c, dados.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion

        #region RELATÓRIO 202 - LOTES E VALIDADES
        public byte[] GerarPdfRelatorio202(List<Relatorio202DTO> dados)
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

                    page.Header().Element(c => GerarCabecalho(c, "CONTROLE DE VALIDADES", "#202"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(3);   // Produto
                            columns.RelativeColumn(2);   // Categoria
                            columns.ConstantColumn(80);  // Data Validade
                            columns.ConstantColumn(100); // Status / Dias
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("ID");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).Text("CATEGORIA");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("VALIDADE");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("SITUAÇÃO");
                        });

                        foreach (var item in dados)
                        {
                            table.Cell().Element(RowStyle).Text(item.IdProduto.ToString());
                            table.Cell().Element(RowStyle).Text(item.NomeProduto);
                            table.Cell().Element(RowStyle).Text(item.Categoria);

                            table.Cell().Element(RowStyle).AlignCenter().Text(item.DataValidade.ToString("dd/MM/yyyy"));

                            // Lógica de Status
                            string statusTexto;
                            string corStatus = Colors.Black;

                            if (item.DiasParaVencer < 0)
                            {
                                statusTexto = $"Vencido há {Math.Abs(item.DiasParaVencer)} d";
                                corStatus = Colors.Red.Medium;
                            }
                            else if (item.DiasParaVencer == 0)
                            {
                                statusTexto = "Vence Hoje!";
                                corStatus = Colors.Red.Medium;
                            }
                            else if (item.DiasParaVencer <= 30)
                            {
                                statusTexto = $"Vence em {item.DiasParaVencer} d";
                                corStatus = Colors.Orange.Medium;
                            }
                            else
                            {
                                statusTexto = "No Prazo";
                                corStatus = Colors.Green.Darken2;
                            }

                            table.Cell().Element(RowStyle).AlignCenter().Text(statusTexto).FontColor(corStatus).SemiBold();
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, dados.Count));
                });
            });

            return documento.GeneratePdf();
        }
        #endregion

        #region RELATÓRIO 203 - MOVIMENTAÇÃO DE ESTOQUE GERAL
        public byte[] ExportarPdfRelatorio203(List<Relatorio203DTO> dados)
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

                    page.Header().Element(c => GerarCabecalho(c, "AUDITORIA DE MOVIMENTAÇÃO DE ESTOQUE", "#203"));

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(70);  // Data
                            columns.ConstantColumn(40);  // Nº
                            columns.RelativeColumn(2);   // Tipo Mov.
                            columns.RelativeColumn(2);   // Pessoa
                            columns.RelativeColumn(3);   // Produto
                            columns.RelativeColumn(1);   // Qtd
                            columns.RelativeColumn(1);   // V. Un
                            columns.RelativeColumn(1);   // V. Total
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("DATA");
                            header.Cell().Element(HeaderStyle).Text("Nº");
                            header.Cell().Element(HeaderStyle).Text("OPERAÇÃO");
                            header.Cell().Element(HeaderStyle).Text("PESSOA");
                            header.Cell().Element(HeaderStyle).Text("PRODUTO");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("QTD");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("V. UN.");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("TOTAL");
                        });

                        foreach (var item in dados)
                        {
                            table.Cell().Element(RowStyle).Text(item.DataMovimento.ToString("dd/MM/yyyy"));
                            table.Cell().Element(RowStyle).Text(item.IdMovimento.ToString());
                            table.Cell().Element(RowStyle).Text(item.TipoMovimento);
                            table.Cell().Element(RowStyle).Text(item.Pessoa);
                            table.Cell().Element(RowStyle).Text(item.NomeProduto);

                            var corQtd = item.IsEntrada ? Colors.Green.Darken2 : Colors.Red.Medium;
                            var sinal = item.IsEntrada ? "+" : "";

                            table.Cell().Element(RowStyle).AlignRight().Text($"{sinal}{item.Quantidade:N2}").FontColor(corQtd).SemiBold();
                            table.Cell().Element(RowStyle).AlignRight().Text(item.ValorUnitario.ToString("C2")).FontColor(Colors.Grey.Darken2);
                            table.Cell().Element(RowStyle).AlignRight().Text(item.ValorTotal.ToString("C2")).SemiBold();
                        }
                    });

                    page.Footer().Element(c => GerarRodape(c, dados.Count));
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