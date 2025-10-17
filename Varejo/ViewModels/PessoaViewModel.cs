using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class PessoaViewModel
    {
        public int IdPessoa { get; set; }


        [Display(Name = "Nome / Razão")]
        public string NomeRazao { get; set; }

        [Display(Name = "Apelido / Fantasia")]
        public string TratamentoFantasia { get; set; }

        [Display(Name = "CPF / CNPJ")]
        public string CpfCnpj { get; set; }


        [Display(Name = "DDD")]
        public string Ddd { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        [Display(Name = "Jurídico")]
        public bool EhJuridico { get; set; }

        [Display(Name = "Usuário")]
        public bool EhUsuario { get; set; }

        [Display(Name = "Cliente")]
        public bool EhCliente { get; set; }


        [Display(Name = "Fornecedor")]
        public bool EhFornecedor { get; set; }
        public bool Ativo { get; set; }


        public List<EnderecoViewModel> Enderecos { get; set; } = new List<EnderecoViewModel>();

    }
}
