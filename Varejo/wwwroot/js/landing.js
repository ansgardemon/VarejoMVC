$(document).ready(function () {
    const urlBase = "http://localhost:5018/api/Categoria"; "

    $ajax({
        url: urlBase,
        type: "GET",
        contentType: "application/json",
        success: function (data) {
            let categorias = $('#categorias');
            categorias.empty(); // Limpa o conteúdo existente
            data.forEach(function (categoria) {
                let categoriaItem = `
                    <div class="categoria-item">
                        <h3>${categoria.nome}</h3>
                        <p>${categoria.descricao}</p>
                    </div>
                `;
                categorias.append(categoriaItem);
            });
        },
        error: function (error) {
            console.error("Erro ao carregar categorias:", error);
        }
    })
});