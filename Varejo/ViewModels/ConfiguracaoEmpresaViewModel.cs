namespace Varejo.ViewModels
{
    public class ConfiguracaoEmpresaViewModel
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public string? UrlLogotipo { get; set; } // O caminho que vai para o banco

        // Campo para o upload
        public IFormFile? LogotipoUpload { get; set; }
    }
}
