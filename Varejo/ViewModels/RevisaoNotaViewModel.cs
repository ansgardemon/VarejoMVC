namespace Varejo.ViewModels
{
    public class RevisaoNotaViewModel
    {
        // Dados da Nota (ReadOnly na tela)
        public string NumeroNota { get; set; }
        public string ChaveAcesso { get; set; }
        public int PessoaId { get; set; }
        public string NomeFornecedor { get; set; }

        // Campos que o usuário vai preencher
        public int? PrazoPagamentoId { get; set; }
        public int? FormaPagamentoId { get; set; }
        public DateTime DataEntrada { get; set; } = DateTime.Now;

        // A lista de itens que você já tem, mas com o ID da Embalagem
        public List<ItemRevisaoViewModel> Itens { get; set; } = new();
    }
}
