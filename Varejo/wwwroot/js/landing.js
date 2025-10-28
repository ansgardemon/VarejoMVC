$(document).ready(function () {
    const urlCategorias = "http://localhost:5018/api/Categoria";
    const urlMarcas = "http://localhost:5018/api/Marca";
    const categoriasContainer = $("#categoriasContainer");
    const marcasContainer = $("#marcasContainer");

    console.log("Página carregada. Iniciando requisição de categorias...");

    // CATEGORIAS
    $.ajax({
        url: urlCategorias,
        type: "GET",
        contentType: "application/json",
        success: function (dados) {
            console.log("Dados recebidos da API:", dados);

            categoriasContainer.empty();

            dados.forEach(categoria => {
                console.log("Processando categoria:", categoria);

                const card = `
                    <div class="col-12 col-md-6 col-lg-3 mx-auto">
                        <div class="card border-0 shadow-sm text-center h-100 category-card">
                            <div class="card-body d-flex flex-column justify-content-between">
                                <h5 class="card-title">${categoria.descricaoCategoria}</h5>
                                <p class="card-text text-muted">Explore nossa seleção de ${categoria.descricaoCategoria.toLowerCase()}.</p>
                                <a href="/Landing/Produtos?categoriaId=${categoria.idCategoria}" class="btn btn-category">Ver mais</a>
                            </div>
                        </div>
                    </div>
                `;
                categoriasContainer.append(card);
            });

            console.log("Todas as categorias foram inseridas no container.");
        },
        error: function (erro) {
            console.error("Erro ao carregar categorias:", erro);
            categoriasContainer.append('<p class="text-danger">Não foi possível carregar as categorias.</p>');
        }
    });

    // MARCAS
    $.ajax({
        url: urlMarcas,
        type: "GET",
        contentType: "application/json",
        success: function (dados) {
            const $carousel = $("#marcasContainer");
            $carousel.empty();

            dados.forEach(marca => {
                const card = `
                <div class="brand-card">
                    <h5>${marca.nomeMarca}</h5>
                </div>
            `;
                $carousel.append(card);
            });

            // Inicializa o Slick APÓS adicionar os cards
            $carousel.slick({
                slidesToShow: 4,
                slidesToScroll: 1,
                infinite: true,
                dots: false,
                arrows: true,
                responsive: [
                    { breakpoint: 992, settings: { slidesToShow: 3 } },
                    { breakpoint: 768, settings: { slidesToShow: 2 } },
                    { breakpoint: 480, settings: { slidesToShow: 1 } }
                ]
            });
        },
        error: function (erro) {
            console.error("Erro ao carregar marcas:", erro);
        }
    });

    // PRODUTOS
    const urlProdutos = "http://localhost:5018/api/Produto/destaques";
    const produtosContainer = $("#produtosContainer");

    console.log("Iniciando requisição de produtos em destaque...");

    $.ajax({
        url: urlProdutos,
        type: "GET",
        contentType: "application/json",
        success: function (dados) {
            console.log("Produtos recebidos:", dados);
            produtosContainer.empty();

            if (!dados || dados.length === 0) {
                produtosContainer.append('<p class="text-muted text-center">Nenhum produto encontrado.</p>');
                return;
            }

            dados.forEach(produto => {
                const precoFormatado = produto.preco
                    ? `A partir de ${produto.preco.toLocaleString("pt-BR", { style: "currency", currency: "BRL" })}`
                    : "Preço indisponível";

                const mensagem = encodeURIComponent(`Olá, estou interessado no ${produto.nomeProduto}`);
                const linkWhatsApp = `https://wa.me/5511985967421?text=${mensagem}`;

                const card = `
                <div class="col-12 col-md-6 col-lg-3">
                    <div class="card h-100 shadow-sm border-0" style="height: 200px;">
                        <img src="${produto.urlImagem || 'https://placehold.co/400x300?text=Sem+Imagem'}" 
                                class="card-img-top" 
                                alt="${produto.nomeProduto}"/>
                        <div class="card-body text-center d-flex flex-column justify-content-between">
                            <div>
                                <h5 class="card-title">${produto.nomeProduto}</h5>
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
                produtosContainer.append(card);
            });

            console.log("Todos os produtos foram carregados com sucesso!");
        },
        error: function (erro) {
            console.error("Erro ao carregar produtos:", erro);
            produtosContainer.append('<p class="text-danger">Não foi possível carregar os produtos em destaque.</p>');
        }
    });


    // API DO PEXELS PUXANDO IMAGEM DO DIA NO BANNER

    const heroBg = $(".hero-bg");
    const pexelsApiKey = "6jmswSf5W8nsVmOsVYwMSgzA4VzSNZaPSmfhDNcs0lb7G9x8JEWeYAUv";

    // Queries possíveis
    const queries = ["beer", "wine", "drink"];
    // Escolhe uma query aleatória
    const query = queries[Math.floor(Math.random() * queries.length)];

    // Puxa várias imagens (15) para ter mais opções
    const urlPexels = `https://api.pexels.com/v1/search?query=${query}&orientation=landscape&per_page=15`;

    $.ajax({
        url: urlPexels,
        type: "GET",
        headers: { Authorization: pexelsApiKey },
        success: function (response) {
            if (response.photos && response.photos.length > 0) {
                const imagens = response.photos;
                // Escolhe a imagem baseada no dia do mês
                const hoje = new Date().getDate(); // 1-31
                const index = hoje % imagens.length;
                const imgUrl = imagens[index].src.landscape;

                heroBg.css("background-image", `url(${imgUrl})`);
                heroBg.css("opacity", 0);
                heroBg.animate({ opacity: 1 }, 1000);

                console.log("Imagem do dia carregada:", imgUrl, "(Query:", query + ")");
            }
        },
        error: function (err) {
            console.error("Erro ao buscar imagem do Pexels:", err);
        }
    });

});
