using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarejoSHARED.DTO
{
    public class VendaOutputDTO
    {
        public int IdVenda { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorSubtotal { get; set; }
        public decimal DescontoTotal { get; set; }
        public decimal ValorFinal { get; set; }
        public string? Observacao { get; set; }
        public bool Finalizada { get; set; }

        public int PessoaId { get; set; }
        public string? NomeCliente { get; set; }

        public int FormaPagamentoId { get; set; }
        public string? FormaPagamento { get; set; }

        public int? PrazoPagamentoId { get; set; }
        public string? PrazoPagamento { get; set; }

        public List<VendaItemOutputDTO> Itens { get; set; } = new();


    }
}
