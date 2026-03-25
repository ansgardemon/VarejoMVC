using System.ComponentModel.DataAnnotations;

namespace Varejo.Models
{
    public class EspecieTitulo
    {

        [Key]
        public int IdEspecieTitulo { get; set; }

        public string Descricao { get; set; }

        //     RELACIONAMENTOS

         public ICollection<TituloFinanceiro> Titulos { get; set; }
    }
}
