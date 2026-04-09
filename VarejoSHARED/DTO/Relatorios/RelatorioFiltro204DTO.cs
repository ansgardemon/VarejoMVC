namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro204DTO : RelatorioFiltroProdutosDTO
    {
        // Analisa o histórico de vendas dos últimos 30 dias por padrão
        public int DiasAnaliseGiro { get; set; } = 30;

        // Quantos dias de estoque a loja deseja ter garantido (ex: quer estoque para 15 dias)
        public int DiasCoberturaDesejada { get; set; } = 15;
    }

    public class Relatorio204DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }
        public decimal VendaTotalPeriodo { get; set; } // O quanto vendeu no período analisado
        public int DiasAnalise { get; set; }
        public int DiasDesejados { get; set; }

        // MÁGICA DA MATEMÁTICA: Propriedades calculadas em tempo real
        public decimal VendaMediaDiaria => DiasAnalise > 0 ? VendaTotalPeriodo / DiasAnalise : 0;

        // 999 = Estoque infinito (não vende nada)
        public decimal DiasCoberturaAtual => VendaMediaDiaria > 0 ? EstoqueAtual / VendaMediaDiaria : 999;

        // Sugere a compra apenas se a cobertura desejada for maior que o estoque que já temos
        public decimal SugestaoCompra => Math.Max(0, (VendaMediaDiaria * DiasDesejados) - EstoqueAtual);
    }
}