using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class MovimentoViewModel
    {
        // Movimento
        public int? Documento { get; set; }
        public string Observacao { get; set; }
        public int TipoMovimentoId { get; set; }
        public int PessoaId { get; set; }

        // Produtos do movimento
        public List<ProdutoMovimentoItem> Produtos { get; set; } = new List<ProdutoMovimentoItem>();
    }

    public class ProdutoMovimentoItem
    {
        [Required]
        public int ProdutoId { get; set; }
        [Required]
        public int ProdutoEmbalagemId { get; set; }
        [Required]
        public decimal Quantidade { get; set; }
    }
}
