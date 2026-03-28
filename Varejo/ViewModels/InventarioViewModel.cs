namespace Varejo.ViewModels
{
    public class InventarioViewModel
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }

        public string? Observacao { get; set; }

        public bool Finalizado { get; set; }

        public List<InventarioItemViewModel> Itens { get; set; }
    }
}
