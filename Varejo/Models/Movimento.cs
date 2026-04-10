using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varejo.Models
{
    public class Movimento
    {
        [Key]
        public int IdMovimento { get; set; }

        [DisplayName("Número do Documento")]
        public int? Documento { get; set; }

        [DisplayName("Observação")]
        public string Observacao { get; set; }

        [DisplayName("Data do Movimento")]
        public DateTime DataMovimento { get; set; } = DateTime.Now;

        // Relacionamento com TipoMovimento

        [DisplayName("Tipo de Movimento")]
        public int TipoMovimentoId { get; set; }
        public TipoMovimento TipoMovimento { get; set; }

        // Relacionamento com Pessoa


        [DisplayName("Cliente/Fornecedor")]
        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; }

        //Relacionamento com Recebimento

        [DisplayName("Origem Recebimento")]
        public int? RecebimentoId { get; set; } // O '?' torna nulo para lançamentos manuais

        [ForeignKey("RecebimentoId")]
        public virtual Recebimento? Recebimento { get; set; }



        //RELACIONAMENTO UM PARA MUITOS

        public virtual ICollection<ProdutoMovimento>? ProdutosMovimento { get; set; }


    }
}
