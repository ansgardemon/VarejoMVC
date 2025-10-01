 using System.ComponentModel.DataAnnotations;
using Varejo.Models;

namespace Varejo.ViewModels
{
    public class EnderecoViewModel
    {

        public int IdEndereco { get; set; }
        public string Logradouro { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Uf { get; set; }
        public string Complemento { get; set; }
        public string Numero { get; set; }

        //RELACIONAMENTO COM PESSOA
        public int PessoaId { get; set; }

        public virtual Pessoa? Pessoa { get; set; }
    }
}
