namespace VarejoAPI.DTO
{
    public class PessoaInputDTO
    {
        public string NomeRazao { get; set; } = string.Empty;
        public string TratamentoFantasia { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Ddd { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public bool EhJuridico { get; set; }
        public bool EhUsuario { get; set; }
        public bool EhCliente { get; set; }
        public bool EhFornecedor { get; set; }

    }
}
