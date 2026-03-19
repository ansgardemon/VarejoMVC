namespace VarejoAPI.DTO
{
    public class PessoaOutputDTO
    {
        public int IdPessoa { get; set; }
        public string NomeRazao { get; set; }
        public string CpfCnpj { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }

        public List<EnderecoDTO>? Enderecos { get; set; }
    }
}
