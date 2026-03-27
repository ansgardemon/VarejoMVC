namespace VarejoSHARED.DTO
{
    public class RelatorioFiltroProdutosDTO : RelatorioFiltroBaseDTO
    {
        public int? IdCategoria { get; set; }
        public int? IdMarca { get; set; }
        public bool? Ativo { get; set; } = true;
    }
}