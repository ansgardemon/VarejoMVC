using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Varejo.Models;

namespace Varejo.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        public string nomeUsuario { get; set; }

        public string Senha { get; set; }

        public bool Ativo { get; set; } = true;


        //RELACIONAMENTO COM PESSOA
        public int PessoaId { get; set; }

        public virtual Pessoa? Pessoa { get; set; }

        public int TipoUsuarioId { get; set; }

        public virtual TipoUsuario? TipoUsuario { get; set; }

        public IEnumerable<SelectListItem>? Pessoas { get; set; }

        public IEnumerable<SelectListItem>? TipoUsuarios { get; set; }


    }
}
