namespace Varejo.ViewModels
{
    public class PessoaViewModel
    {
        public int IdPessoa { get; set; }
        public string NomeRazao { get; set; }
        public string TratamentoFantasia { get; set; }
        public string CpfCnpj { get; set; }
        public string Ddd { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public bool EhJuridico { get; set; }
        public bool EhUsuario { get; set; }
        public bool EhCliente { get; set; }
        public bool EhFornecedor { get; set; }
        public bool Ativo { get; set; }
    }
}
