namespace VarejoSHARED.DTO
{
    public class EnderecoOutputDTO
    {
        public int IdEndereco { get; set; }
        public string Logradouro { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Complemento { get; set; }
        public string Numero { get; set; }

        public int PessoaId { get; set; }

    }
}
