

self.addEventListener('install', event => {
    console.log('Service Worker: Instalando...');
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    console.log('Service Worker: Ativado e pronto para gerenciar o PWA.');
});

// Interceptador de requisições padrão (exigência para ser um PWA instalável)
self.addEventListener('fetch', event => {
    // Por enquanto, apenas deixa a requisição passar normalmente (Network first)
    // Mais tarde, quando formos fazer cache offline, mexeremos aqui.
    return;
});