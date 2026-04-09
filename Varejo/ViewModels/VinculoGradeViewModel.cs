namespace Varejo.ViewModels
{
    public class VinculoGradeViewModel
    {
        public int PessoaId { get; set; }
        public string NomeFornecedor { get; set; }

        public int FamiliaId { get; set; }
        public string NomeFamilia { get; set; }

        public List<ItemVinculoViewModel> Itens { get; set; } = new List<ItemVinculoViewModel>();
    }
}
