// produtos.js
const API_BASE = "http://localhost:5018/api";

$(document).ready(function () {
    console.log("[produtos.js] DOM carregado. Iniciando...");
    carregarCategorias();
    carregarMarcas();
    carregarProdutos();

    // Atualiza produtos ao trocar filtros
    $("#categoriaSelect, #marcaSelect").on("change", function () {
        console.log("[produtos.js] Filtro alterado. categoria=", $("#categoriaSelect").val(), " marca=", $("#marcaSelect").val());
        carregarProdutos();
    });
}

);

function carregarCategorias() {
    const url = `${API_BASE}/Categoria`;
    console.log("[carregarCategorias] GET", url);
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (data) {
            console.log("[carregarCategorias] sucesso:", data);
            const select = $("#categoriaSelect");
            select.empty().append('<option value="">Todas as Categorias</option>');
            if (!data || data.length === 0) {
                console.warn("[carregarCategorias] API retornou lista vazia");
                return;
            }
            data.forEach(cat => {
                // Use propriedade segura (case-insensitive) caso o DTO venha com camelCase
                const id = cat.idCategoria ?? cat.IdCategoria ?? cat.id ?? cat.Id;
                const descricao = cat.descricaoCategoria ?? cat.DescricaoCategoria ?? cat.nome ?? cat.Nome;
                select.append(`<option value="${id}">${descricao}</option>`);
            });
        },
        error: function (xhr, status, err) {
            console.error("[carregarCategorias] erro", status, err, xhr);
            $("#categoriaSelect").append('<option value="">Erro carregando categorias</option>');
        }
    });
}

function carregarMarcas() {
    const url = `${API_BASE}/Marca`;
    console.log("[carregarMarcas] GET", url);
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (data) {
            console.log("[carregarMarcas] sucesso:", data);
            const select = $("#marcaSelect");
            select.empty().append('<option value="">Todas as Marcas</option>');
            if (!data || data.length === 0) {
                console.warn("[carregarMarcas] API retornou lista vazia");
                return;
            }
            data.forEach(marca => {
                const id = marca.idMarca ?? marca.IdMarca ?? marca.id ?? marca.Id;
                const nome = marca.nomeMarca ?? marca.NomeMarca ?? marca.nome ?? marca.Nome;
                select.append(`<option value="${id}">${nome}</option>`);
            });
        },
        error: function (xhr, status, err) {
            console.error("[carregarMarcas] erro", status, err, xhr);
            $("#marcaSelect").append('<option value="">Erro carregando marcas</option>');
        }
    });
}

function carregarProdutos() {
    const categoriaId = $("#categoriaSelect").val();
    const marcaId = $("#marcaSelect").val();

    // Monta query apenas com parâmetros válidos (omitindo vazios)
    const params = [];
    if (categoriaId && categoriaId !== "") params.push(`IdCategoria=${encodeURIComponent(categoriaId)}`);
    if (marcaId && marcaId !== "") params.push(`IdMarca=${encodeURIComponent(marcaId)}`);
    const qs = params.length ? `?${params.join("&")}` : "";
    const url = `${API_BASE}/Produto/catalogo${qs}`;

    console.log("[carregarProdutos] GET", url);
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json",
        cache: false,
        success: function (data) {
            console.log("[carregarProdutos] sucesso:", data);
            const container = $("#listaProdutos");
            container.empty();

            if (!data || data.length === 0) {
                container.append('<p class="text-center text-muted">Nenhum produto encontrado.</p>');
                return;
            }

            data.forEach(prod => {
                // obter propriedades com fallback
                const id = prod.idProduto ?? prod.IdProduto ?? prod.id ?? prod.Id;
                const nome = prod.nomeProduto ?? prod.NomeProduto ?? prod.nome ?? prod.Nome ?? "Produto";
                const urlImg = prod.urlImagem ?? prod.UrlImagem ?? prod.url ?? prod.Url ?? "/images/produto-padrao.png";
                const preco = (prod.preco ?? prod.Preco ?? 0);
                const mensagem = encodeURIComponent(`Olá, estou interessado no ${nome}`);
                const linkWhatsApp = `https://wa.me/5511985967421?text=${mensagem}`;


                const precoFormatado = preco > 0
                    ? `A partir de ${preco.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}`
                    : "Preço indisponível";

                const card = `
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="card h-100 shadow-sm border-0">
                            <img src="${urlImg}" class="card-img-top" alt="${nome}">
                            <div class="card-body text-center d-flex flex-column justify-content-between">
                                <div>
                                    <h5 class="card-title">${nome}</h5>
                                    <p class="text-muted mb-3">${precoFormatado}</p>
                                </div>
                                <a href="${linkWhatsApp}" 
                               target="_blank" 
                               class="btn btn-success w-100 mt-auto">
                                Pedir pelo WhatsApp
                            </a>
                            </div>
                        </div>
                    </div>
                `;
                container.append(card);
            });
        },
        error: function (xhr, status, err) {
            console.error("[carregarProdutos] erro", status, err, xhr);
            $("#listaProdutos").html('<p class="text-center text-danger">Erro ao carregar produtos.</p>');
        }
    });
}
