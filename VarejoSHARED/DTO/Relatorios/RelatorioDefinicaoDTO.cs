namespace VarejoSHARED.DTO.Relatorios
{
    public class RelatorioDefinicaoDTO
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty; // Ex: Estoque, Financeiro
        public bool IsFavorito { get; set; }
    }
}