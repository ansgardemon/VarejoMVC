namespace Varejo.ViewModels
{
    public class EstoqueViewModel
    {
        public List<EstoqueItemViewModel> Itens { get; set; }

        public EstoqueFiltroViewModel Filtro { get; set; }
    }
}