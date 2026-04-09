const API_BASE = "http://localhost:5018/api";

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const categoriaIdInicial = urlParams.get("categoriaId");

    // Encadeamento correto para não atropelar as chamadas
    carregarCategorias().then(() => {
        if (categoriaIdInicial) {
            // Remove active de todos e coloca no que veio da URL
            $("#categoriaPills .btn-filter").removeClass("active");
            $(`#categoriaPills .btn-filter[data-value='${categoriaIdInicial}']`).addClass("active");
        }
        return carregarMarcas();
    }).then(() => {
        carregarProdutos();
    });
});

function carregarCategorias() {
    return new Promise((resolve) => {
        $.get(`${API_BASE}/Categoria`, function (data) {
            const container = $("#categoriaPills");
            data.forEach(cat => {
                // Forçamos o ID para String para evitar erro de seletor
                const id = String(cat.idCategoria || cat.id);
                container.append(`<button class="btn btn-filter" data-value="${id}">${cat.descricaoCategoria}</button>`);
            });
            resolve();
        });
    });
}

function carregarMarcas() {
    return new Promise((resolve) => {
        $.get(`${API_BASE}/Marca`, function (data) {
            const container = $("#marcaPills");
            data.forEach(marca => {
                const id = String(marca.idMarca || marca.id);
                container.append(`<button class="btn btn-filter" data-value="${id}">${marca.nomeMarca}</button>`);
            });
            resolve();
        });
    });
}

$(document).on("click", ".btn-filter", function () {
    $(this).siblings().removeClass("active");
    $(this).addClass("active");
    carregarProdutos();
});

function carregarProdutos() {
    // Usamos .filter('.active') que é mais preciso que .find() em alguns casos
    const categoriaId = $("#categoriaPills .btn-filter.active").attr("data-value") || "";
    const marcaId = $("#marcaPills .btn-filter.active").attr("data-value") || "";

    const params = [];
    if (categoriaId) params.push(`IdCategoria=${categoriaId}`);
    if (marcaId) params.push(`IdMarca=${marcaId}`);

    const url = `${API_BASE}/Produto/catalogo${params.length ? '?' + params.join("&") : ""}`;

    console.log("[DEBUG] Chamando API:", url);

    $.ajax({
        url: url,
        type: "GET",
        success: function (data) {
            const container = $("#listaProdutos");
            container.empty();

            if (!data || data.length === 0) {
                container.append('<div class="col-12 text-center py-5"><h3 class="text-gold">Nenhum produto encontrado.</h3><p class="text-light-muted">Tente mudar o filtro!</p></div>');
                return;
            }

            data.forEach(prod => {
                const nome = prod.nomeProduto || "Produto";
                const urlImg = prod.urlImagem || "/images/produto-padrao.png";
                const preco = prod.preco || 0;
                const precoFormatado = preco > 0 ? preco.toLocaleString("pt-BR", { style: "currency", currency: "BRL" }) : "Sob consulta";
                const linkZap = `https://wa.me/5511985967421?text=Salve! Tenho interesse no ${nome}`;

                const card = `
                    <div class="col-12 col-md-6 col-lg-3">
                        <div class="card h-100 border-0 shadow-sm" style="background: transparent;">
                            <div class="d-flex align-items-center justify-content-center p-3" style="height: 220px;">
                                <img src="${urlImg}" class="img-fluid" style="max-height: 100%; object-fit: contain;">
                            </div>
                            <div class="card-body text-center">
                                <h5 class="card-title fw-bold text-uppercase" style="color: var(--brand-accent); font-family: var(--font-head);">${nome}</h5>
                                <div class="mb-3">
                                    <span class="price-tag" style="font-size: 1.5rem; color: #fff; font-weight: 800;">${precoFormatado}</span>
                                </div>
                                <a href="${linkZap}" target="_blank" class="btn btn-success w-100 fw-bold py-2 shadow-sm">
                                    <i class="fab fa-whatsapp me-1"></i> PEDIR AGORA
                                </a>
                            </div>
                        </div>
                    </div>`;
                container.append(card);
            });
        },
        error: function (err) {
            console.error("Erro na API:", err);
            $("#listaProdutos").html('<p class="text-center text-danger">Erro ao carregar catálogo.</p>');
        }
    });
}