namespace Varejo.ViewModels
{
    public class ExibirItensXmlViewModel
    {
        public string CodigoFornecedor { get; set; }
        public string NomeProduto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorTotal { get; set; }
        // Este campo é importante para o "De-Para" (vincular ao seu produto interno)
        public int? ProdutoIdInterno { get; set; }
    }
}
