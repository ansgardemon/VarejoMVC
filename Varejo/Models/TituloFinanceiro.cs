using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varejo.Models
{
    public class TituloFinanceiro
    {
        [Key]
        public int IdTituloFinanceiro { get; set; }

        public int Documento { get; set; }

        public int Parcela { get; set; }


        [StringLength(100)]
        public string Observacao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        public decimal? ValorPago { get; set; }

        public decimal ValorAberto { get; set; }

        [Required]
        public DateTime DataEmissao { get; set; }

        [Required]
        public DateTime DataVencimento { get; set; }

        public DateTime? DataPagamento { get; set; }

       public bool Quitado { get; set; }

        // RELACIONAMENTOS

        [Required]
        public int EspecieTituloId { get; set; }
        public EspecieTitulo EspecieTitulo { get; set; }

        public int? FormaPagamentoId { get; set; }
        public FormaPagamento FormaPagamento { get; set; }

        public int? PrazoPagamentoId { get; set; }
        public PrazoPagamento PrazoPagamento { get; set; }

        // Pessoa (cliente ou fornecedor)
        public int? PessoaId { get; set; }
        public Pessoa Pessoa { get; set; }


        public void AtualizarValores()
        {
            ValorAberto = Valor - (ValorPago ?? 0);
            Quitado = ValorAberto <= 0;
        }
    }
}