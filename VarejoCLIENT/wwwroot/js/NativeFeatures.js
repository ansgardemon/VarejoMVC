// Arquivo: wwwroot/js/nativeFeatures.js

window.nativeFeatures = {
    // Função para vibrar o celular
    vibrar: function (padrao) {
        // Verifica se o navegador/dispositivo tem suporte ao motor de vibração
        if ("vibrate" in navigator) {
            // padrao pode ser um número (ex: 200) ou um array (ex: [100, 50, 100])
            navigator.vibrate(padrao);
        } else {
            console.warn("Este dispositivo não suporta a API de vibração nativa.");
        }
    }
};