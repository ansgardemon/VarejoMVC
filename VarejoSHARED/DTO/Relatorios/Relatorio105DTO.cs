using VarejoSHARED.DTO.Relatorios;

namespace VarejoSHARED.DTO
{
    public class RelatorioFiltro105DTO : RelatorioFiltroProdutosDTO
    {
        public int DiasSemGiro { get; set; } = 30;
    }

    public class Relatorio105DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal EstoqueAtual { get; set; }
        public DateTime? UltimaVenda { get; set; }
        public int DiasParado { get; set; } // Dias calculados desde a última venda
    }
}