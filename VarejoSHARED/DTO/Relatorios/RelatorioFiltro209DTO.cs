namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro209DTO : RelatorioFiltroProdutosDTO
    {
        // O usuário vai selecionar qual TipoMovimento representa "Ajuste de Estoque"
        public int? IdTipoMovimentoAjuste { get; set; }
    }
    public class Relatorio209DTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public DateTime DataAjuste { get; set; }
        public decimal QuantidadeAjustada { get; set; } // Negativo é perda, positivo é sobra
        public decimal CustoUnitario { get; set; }
        public decimal ImpactoFinanceiro => QuantidadeAjustada * CustoUnitario;
        public string Observacao { get; set; } = string.Empty;
    }
}
