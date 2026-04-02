namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioFiltroProdutosDTO : RelatorioFiltroBaseDTO
    {
        // Atualizado para suportar a regra de Multi-Select (Checkbox)
        public List<int> CategoriasIds { get; set; } = new();
        public List<int> MarcasIds { get; set; } = new();
        public List<int> FamiliasIds { get; set; } = new();
        public List<int>? ProdutosIds { get; set; } = new();

        public bool? Ativo { get; set; } // Mantemos true/false/null

    }
}