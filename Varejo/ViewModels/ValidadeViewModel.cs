using Varejo.Models;

namespace Varejo.ViewModels
{
    public class ValidadeViewModel
    {
        public int IdValidade { get; set; }

        public DateTime DataValidade { get; set; }

        public bool EmEstoque { get; set; } = true;


        public int ProdutoId { get; set; }

        public string? ProdutoNome { get; set; }

    }
}
