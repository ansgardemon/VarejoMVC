namespace Varejo.ViewModels
{
    public class EstoqueViewModel
    {
        public List<EstoqueListViewModel> Itens { get; set; }

        public EstoqueFiltroViewModel Filtro { get; set; }
    }
}