using VarejoSHARED.DTO.Relatorios;

namespace VarejoSHARED.DTO
{
    public class RelatorioFiltro106DTO : RelatorioFiltroProdutosDTO
    {
        public int QuantidadeRegistros { get; set; } = 50; // Padrão: mostrar os Top 50
        public string TipoOrdem { get; set; } = "MAIS_VENDIDOS"; // "MAIS_VENDIDOS" ou "MENOS_VENDIDOS"
    }

    public class Relatorio106DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public decimal QuantidadeVendida { get; set; }
        public decimal Faturamento { get; set; }
    }
}