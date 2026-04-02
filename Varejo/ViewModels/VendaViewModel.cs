using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class VendaViewModel
    {
        public int IdVenda { get; set; } // 0 para nova venda

        [Required(ErrorMessage = "Selecione o cliente")]
        public int PessoaId { get; set; }

        [Required(ErrorMessage = "Selecione a forma de pagamento")]
        public int FormaPagamentoId { get; set; }

        public int? PrazoPagamentoId { get; set; }

        [Display(Name = "Observação")]
        public string? Observacao { get; set; }

        // Totais calculados (Geralmente via JavaScript na tela e conferidos no C#)
        public decimal ValorSubtotal { get; set; }
        public decimal DescontoTotal { get; set; }
        public decimal ValorFinal => ValorSubtotal - DescontoTotal;

        // Lista de itens que virá do formulário
        public List<VendaItemViewModel> Itens { get; set; } = new List<VendaItemViewModel>();
    }

    public class VendaItemViewModel
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } // Para exibição na tabela

        public int ProdutoEmbalagemId { get; set; }
        public string NomeEmbalagem { get; set; } // Ex: "Caixa c/ 12"

        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; } // Preço de tabela da embalagem
        public decimal DescontoUnitario { get; set; } // Desconto por item

        public decimal Subtotal => Quantidade * (ValorUnitario - DescontoUnitario);
    }
}