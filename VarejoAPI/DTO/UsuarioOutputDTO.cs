using System.ComponentModel.DataAnnotations;
using Varejo.Models;

namespace VarejoAPI.DTO
{
    public class UsuarioOutputDTO
    {

            public int IdUsuario { get; set; }
            public string nomeUsuario { get; set; }      
            public string Senha { get; set; }
            public bool Ativo { get; set; } = true;
            //RELACIONAMENTO COM PESSOA
            public int PessoaId { get; set; }
            public virtual Pessoa? Pessoa { get; set; }
            //RELACIONAMENTO COM PESSOA
            public int TipoUsuarioId { get; set; }

            public virtual TipoUsuario? TipoUsuario { get; set; }


        
    }
}





