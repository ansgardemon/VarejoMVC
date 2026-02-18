using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }

        [DisplayName("Observação")]
        public string ObservacaoPedido { get; set; }

        [DisplayName("Data do Pedido")]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        [DisplayName("Data de Entrega")]
        public DateTime DataEntrega { get; set; }

        [DisplayName("Data de Faturamento")]
        public DateTime DataFaturamento { get; set; }

        [DisplayName("Data de Cancelamento")]
        public DateTime DataCancelamento { get; set; }

        [DisplayName("Valor Produtos")]
        public decimal ValorProdutos { get; set; }

        [DisplayName("Valor Desconto")]
        public decimal ValorDesconto { get; set; }

        [DisplayName("Valor Frete")]
        public decimal ValorFrete { get; set; }

        [DisplayName("Valor Total")]
        public decimal ValorTotal { get; set; }





        // Relacionamento com TipoMovimento

        [DisplayName("Tipo de Movimento")]
        public int TipoMovimentoId { get; set; }
        public TipoMovimento TipoMovimento { get; set; }

        // Relacionamento com Pessoa


        [DisplayName("Cliente")]
        public int PessoaId { get; set; }
        public Pessoa Pessoa { get; set; }


        // Relacionamento com Endereço

        [DisplayName("Endereço de Entrega")]

        public int EnderecoId { get; set; }

        public Endereco Endereco { get; set; }


        // Relacionamento com FormaPagamento

        [DisplayName("Forma de Pagamento")]
        public int FormaPagamentoId { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        // Relacionamento com Prazo

        [DisplayName("Prazo de Pagamento")]
        public int PrazoId { get; set; }

        public Prazo Prazo { get; set; }


        // Relacionamento com StatusPedido

        [DisplayName("Status do Pedido")]

        public int StatusPedidoId { get; set; }


        // Relacionamento com Usuario

        [DisplayName("Usuário")]
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }



        //RELACIONAMENTO UM PARA MUITOS

        public virtual ICollection<ProdutoPedido>? ProdutosPedido{ get; set; }

        public virtual ICollection<Titulo>? Titulos { get; set; }


    }
}
