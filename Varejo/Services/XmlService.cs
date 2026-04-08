using System.Xml.Linq;
using Varejo.Interfaces;
using Varejo.ViewModels;

namespace Varejo.Services
{
    public class XmlService : IXmlService
    {
        public RevisaoNotaViewModel CarregarDadosDoXml(IFormFile arquivo)
        {
            using var stream = arquivo.OpenReadStream();
            var xml = XDocument.Load(stream);
            XNamespace ns = "http://www.portalfiscal.inf.br/nfe";

            // 1. Cabeçalho da Nota
            var infNFe = xml.Descendants(ns + "infNFe").FirstOrDefault();
            var ide = xml.Descendants(ns + "ide").FirstOrDefault();
            var emit = xml.Descendants(ns + "emit").FirstOrDefault();

            var model = new RevisaoNotaViewModel
            {
                ChaveAcesso = infNFe?.Attribute("Id")?.Value.Replace("NFe", "") ?? "",
                NumeroNota = ide?.Element(ns + "nNF")?.Value ?? "",
                CnpjFornecedor = emit?.Element(ns + "CNPJ")?.Value ?? emit?.Element(ns + "CPF")?.Value ?? "",
                NomeFornecedor = emit?.Element(ns + "xNome")?.Value ?? "Fornecedor Desconhecido",
                Itens = new List<ItemRevisaoViewModel>()
            };

            // 2. Itens da Nota (<det>)
            var produtosXml = xml.Descendants(ns + "det");

            foreach (var det in produtosXml)
            {
                var prod = det.Element(ns + "prod");

                model.Itens.Add(new ItemRevisaoViewModel
                {
                    CodigoFornecedor = prod?.Element(ns + "cProd")?.Value ?? "",
                    DescricaoXml = prod?.Element(ns + "xProd")?.Value ?? "",
                    Quantidade = decimal.Parse(prod?.Element(ns + "qCom")?.Value.Replace(".", ",") ?? "0"),
                    ValorUnitario = decimal.Parse(prod?.Element(ns + "vUnCom")?.Value.Replace(".", ",") ?? "0")
                });
            }

            return model;
        }
    }
}