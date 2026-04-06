using VarejoSHARED.DTO.Relatorios;

namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro207DTO : RelatorioFiltroProdutosDTO { } // Usa DataInicio e DataFim do Base

    public class Relatorio207DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }
        public decimal TotalSaidas { get; set; }
        public int DiasAnalisados { get; set; }
        public decimal SaidaMediaDiaria => DiasAnalisados > 0 ? TotalSaidas / DiasAnalisados : 0;
    }
}