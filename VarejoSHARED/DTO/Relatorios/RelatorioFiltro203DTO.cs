namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltro203DTO : RelatorioFiltroProdutosDTO
    {
        // "TODOS", "ENTRADAS", "SAIDAS"
        public string TipoOperacao { get; set; } = "TODOS";
    }

    public class Relatorio203DTO
    {
        public int IdMovimento { get; set; }
        public DateTime DataMovimento { get; set; }
        public string TipoMovimento { get; set; } = string.Empty;
        public string Pessoa { get; set; } = string.Empty;
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }

        public decimal ValorTotal => Math.Abs(Quantidade) * ValorUnitario;
        public bool IsEntrada => Quantidade > 0;
    }
}