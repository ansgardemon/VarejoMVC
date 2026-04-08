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
        <div class="card h-100 shadow border-0 text-center">
            <div class="card-body d-flex flex-column justify-content-between">
                <h5 class="card-title text-uppercase fw-bold">${categoria.descricaoCategoria}</h5>
                
                <p class="card-text text-light-muted mb-4">
                    Explore nossa seleção de ${categoria.descricaoCategoria.toLowerCase()}.
                </p>
                
                <a href="/Landing/Produtos?categoriaId=${categoria.idCategoria}" 
                   class="btn btn-outline-gold mt-auto">
                    Ver mais
                </a>
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
                autoplay: true,
                autoplaySpeed: 1000,
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
        <div class="card h-100 shadow">
            <div class="position-absolute top-0 end-0 m-2">
                <span class="badge-destaque">TOP!</span>
            </div>
            <img src="${produto.urlImagem || '...'}" class="card-img-top"; padding:15px;">
            <div class="card-body text-center">
                <h5 class="card-title">${produto.nomeProduto}</h5>
                <span class="price-tag">${precoFormatado}</span>
                <a href="${linkWhatsApp}" target="_blank" class="btn btn-success w-100 fw-bold py-2">
                    <i class="fab fa-whatsapp me-1"></i> ENVIAR PEDIDO
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

    const animaScroll = () => {
        const target = $('.fade-in-up');
        const windowHeight = $(window).height() * 0.85;
        const scrollTop = $(window).scrollTop();

        target.each(function () {
            const itemTop = $(this).offset().top;
            if (scrollTop > itemTop - windowHeight) {
                $(this).css('opacity', 1); // Garante que fique visível se JS travar
                $(this).addClass('active'); // Você pode criar a classe .active { opacity:1; transform:none; } no CSS
            }
        });
    }

    // Dispara no carregamento e na rolagem
    $(window).on('scroll', animaScroll);
    animaScroll(); // Gatilho inicial

  

});
