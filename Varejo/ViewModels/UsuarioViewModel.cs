using Microsoft.AspNetCore.Mvc.Rendering;

namespace Varejo.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }
        public string nomeUsuario { get; set; }
        public string Senha { get; set; }
        public int TipoUsuarioId { get; set; }
        public int PessoaId { get; set; }

        //popular o dropdown
        public IEnumerable<SelectListItem>? TiposUsuario { get; set; }
    }
}
