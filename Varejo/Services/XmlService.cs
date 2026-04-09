using System.Globalization;
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
                CnpjCpfFornecedorXml = emit?.Element(ns + "CNPJ")?.Value ?? emit?.Element(ns + "CPF")?.Value ?? "",
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
                    // Puxa o código <cProd> do XML
                    CodigoFornecedor = prod?.Element(ns + "cProd")?.Value ?? string.Empty,

                    // Puxa o nome <xProd> do XML
                    NomeProdutoXml = prod?.Element(ns + "xProd")?.Value ?? "Produto sem descrição",

                    // Converte a quantidade <qCom> tratando o ponto decimal corretamente
                    Quantidade = decimal.Parse(prod?.Element(ns + "qCom")?.Value ?? "0", CultureInfo.InvariantCulture),

                    // Converte o valor unitário <vUnCom> tratando o ponto decimal corretamente
                    ValorUnitario = decimal.Parse(prod?.Element(ns + "vUnCom")?.Value ?? "0", CultureInfo.InvariantCulture),

                    // Campos de vínculo começam nulos para o Controller preencher via banco de dados
                    ProdutoIdInterno = null,
                    ProdutoEmbalagemId = null
                });
            }

            return model;
        }
    }
}