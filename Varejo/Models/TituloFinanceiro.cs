using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Varejo.Models
{
    public class TituloFinanceiro
    {
        [Key]
        public int IdTituloFinanceiro { get; set; }

        [Required]
        public int Documento { get; set; }

        [Required]
        public int Parcela { get; set; }

        [StringLength(100)]
        public string? Observacao { get; set; }

        [Required]
        public decimal Valor { get; set; }

        public decimal ValorAberto { get; set; }

        [Required]
        public DateTime DataEmissao { get; set; }

        [Required]
        public DateTime DataVencimento { get; set; }

        public DateTime? DataPagamento { get; set; }

        public bool Quitado { get; set; }

        // =========================
        // RELACIONAMENTOS
        // =========================

        [Required]
        public int EspecieTituloId { get; set; }
        public EspecieTitulo? EspecieTitulo { get; set; }

        public int? FormaPagamentoId { get; set; }
        public FormaPagamento? FormaPagamento { get; set; }

        public int? PrazoPagamentoId { get; set; }
        public PrazoPagamento? PrazoPagamento { get; set; }

        public int? PessoaId { get; set; }
        public Pessoa? Pessoa { get; set; }

        // histórico de pagamentos
        public ICollection<PagamentoTitulo> Pagamentos { get; set; } = new List<PagamentoTitulo>();

        // =========================
        // REGRAS DE NEGÓCIO
        // =========================
        public void AtualizarValores()
        {
            var totalPago = Pagamentos != null
                ? Pagamentos.Sum(p => p.ValorPago)
                : 0;

            ValorAberto = Valor - totalPago;
            Quitado = ValorAberto <= 0;

            // opcional: define DataPagamento automaticamente
            if (Quitado && Pagamentos.Any())
            {
                DataPagamento = Pagamentos.Max(p => p.DataPagamento);
            }
            else
            {
                DataPagamento = null;
            }
        }
    }
}