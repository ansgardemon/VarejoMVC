namespace VarejoSHARED.DTO
{
    public class DashboardDTO
    {
        public int TotalProdutos { get; set; }
        public int TotalCategorias { get; set; }
        public int TotalFamilias { get; set; }
        public int TotalUsuarios { get; set; }
        public int TotalMarcas { get; set; }
        public int TotalMovimentos { get; set; }
        public int TotalProdutosMovimentados { get; set; }

        public List<ProdutoOutputDTO> UltimosProdutos { get; set; } = new();
        public List<CategoriaCountDTO> ProdutosPorCategoria { get; set; } = new();
        public List<MovimentoItemDTO> UltimosMovimentos { get; set; } = new();
    }

    public class CategoriaCountDTO
    {
        public int IdCategoria { get; set; }
        public string Descricao { get; set; } = "";
        public int QtdeProdutos { get; set; }
    }

    public class MovimentoItemDTO
    {
        public int IdMovimento { get; set; }
        public string Pessoa { get; set; } = "";
        public string TipoMovimento { get; set; } = "";
        public DateTime Data { get; set; }
        public int QtdeProdutos { get; set; }
    }
}