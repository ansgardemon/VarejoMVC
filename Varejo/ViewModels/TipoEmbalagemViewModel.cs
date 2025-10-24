using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Varejo.ViewModels
{
    public class TipoEmbalagemViewModel
    {
        [DisplayName("ID Tipo Embalagem")]
        public int IdTipoEmbalagem { get; set; }

        [DisplayName("Descrição Tipo Embalagem")]
        public string DescricaoTipoEmbalagem { get; set; }

        [DisplayName("Multiplicador")]
        public int Multiplicador { get; set; }



    }
}
