using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Movimento
    {
        [Key]
        public int IdMovimento { get; set; }

        public int? Documento { get; set; }

        public string Observacao { get; set; }

        public DateTime DataMovimento { get; set; } = DateTime.Now;

        // Relacionamento com TipoMovimento

        [DisplayName("Tipo de Movimento")]
        public int TipoMovimentoId { get; set; }
        public TipoMovimento TipoMovimento { get; set; }

        // Relacionamento com Pessoa


        [DisplayName("Cliente/Fornecedor")]
        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; }



        //RELACIONAMENTO UM PARA MUITOS

        public virtual ICollection<ProdutoMovimento>? ProdutosMovimento { get; set; }


    }
}
