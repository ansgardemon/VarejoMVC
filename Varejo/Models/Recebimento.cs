using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Recebimento
    {
        [Key]
        public int IdRecebimento { get; set; }

        [Display(Name = "Fornecedor")]
        public int PessoaId { get; set; } // FK para Pessoa onde EhFornecedor == true
        public virtual Pessoa Fornecedor { get; set; }

        [Display(Name = "Número NF")]
        public string NumeroNota { get; set; }

        [Display(Name = "Chave de Acesso (XML)")]
        public string ChaveAcesso { get; set; } // As 44 posições da NF-e

        public DateTime DataEntrada { get; set; } = DateTime.Now;

        // Relacionamento com itens e financeiro
        public virtual ICollection<RecebimentoItem> Itens { get; set; }
    }
}
