namespace VarejoSHARED.DTO.Relatorios
{
    #region MÓDULO 300 - MOVIMENTAÇÕES
    public class RelatorioFiltro301DTO : RelatorioFiltroBaseDTO
    {
        public List<int>? TiposMovimentoIds { get; set; }
        public List<int>? PessoasIds { get; set; }
        public List<int>? ProdutosIds { get; set; }
    }

    public class Relatorio301DTO
    {
        public int IdMovimento { get; set; }
        public DateTime DataMovimento { get; set; }
        public string TipoMovimento { get; set; } = string.Empty;
        public string Pessoa { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal => Math.Abs(Quantidade) * ValorUnitario;
        public string Observacao { get; set; } = string.Empty;

        // Usado para formatar cores na tela (Verde para entrada, Vermelho para saída
        public bool IsEntrada => Quantidade > 0;
    }
    #endregion
}