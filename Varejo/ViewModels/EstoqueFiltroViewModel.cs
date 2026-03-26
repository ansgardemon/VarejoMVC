namespace Varejo.ViewModels
{
    public class EstoqueFiltroViewModel
    {
        public string? NomeProduto { get; set; }
        public int? MarcaId { get; set; }
        public int? CategoriaId { get; set; }

        // status de estoque
        public bool AbaixoMinimo { get; set; }
        public bool AcimaMaximo { get; set; }
        public bool EstoqueZerado { get; set; }
        public bool EstoqueNegativo { get; set; }

        // intervalo de quantidade
        public decimal? EstoqueMin { get; set; }
        public decimal? EstoqueMax { get; set; }

        // movimentação recente
        public int? DiasSemMovimento { get; set; }
        public int? DiasComMovimento { get; set; }
    }
}
