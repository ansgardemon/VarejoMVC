namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro202DTO : RelatorioFiltroProdutosDTO
    {
        // "TODOS", "VENCIDOS", "VENCENDO_30", "NO_PRAZO"
        public string StatusValidade { get; set; } = "TODOS";
    }

    public class Relatorio202DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime DataValidade { get; set; }

        // Calcula a diferença de dias entre hoje e a validade
        public int DiasParaVencer => (DataValidade.Date - DateTime.Now.Date).Days;
    }
}