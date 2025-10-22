$(document).ready(function () {
    const urlBase = "http://localhost:5018/api/Categoria";
    const container = $("#categoriasContainer");

    console.log("Página carregada. Iniciando requisição de categorias...");

    // Consumir API de categorias
    $.ajax({
        url: urlBase,
        type: "GET",
        contentType: "application/json",
        success: function (dados) {
            console.log("Dados recebidos da API:", dados);

            container.empty(); // limpa qualquer conteúdo antigo

            dados.forEach(categoria => {
                console.log("Processando categoria:", categoria);

                const card = `
                    <div class="col-md-4">
                        <div class="card border-0 shadow-sm text-center h-100 category-card">
                            <div class="card-body d-flex flex-column justify-content-between">
                                <h5 class="card-title">${categoria.descricaoCategoria}</h5>
                                <p class="card-text text-muted">Explore nossa seleção de ${categoria.descricaoCategoria.toLowerCase()}.</p>
                                <a href="/Landing/Produtos?categoriaId=${categoria.idCategoria}" class="btn btn-outline-wine mt-auto">Ver mais</a>
                            </div>
                        </div>
                    </div>
                `;
                container.append(card);
            });

            console.log("Todas as categorias foram inseridas no container.");
        },
        error: function (erro) {
            console.error("Erro ao carregar categorias:", erro);
            container.append('<p class="text-danger">Não foi possível carregar as categorias.</p>');
        }
    });
});
