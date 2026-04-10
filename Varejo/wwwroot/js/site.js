$(document).ready(function () {

    // #region 1. NAVBAR, THEME & CLICK-DROPDOWN
    const nav = $('.navbar-varejo');
    const themeCheckbox = $('#theme-checkbox, #theme-switch');
    const htmlElement = $('html');

    // Funcionalidade de Clique no Dropdown (Persistente)
    $('.nav-drop').on('click', function (e) {
        e.stopPropagation();
        const currentDrop = $(this);
        $('.nav-drop').not(currentDrop).removeClass('active'); // Fecha outros
        currentDrop.toggleClass('active'); // Alterna o atual
    });

    // Fechar ao clicar fora
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.nav-drop').length) {
            $('.nav-drop').removeClass('active');
        }
    });

    // Controle de Scroll (Para fazer a Home transitar do centro para o topo)
    const path = window.location.pathname.toLowerCase();
    const isHome = path === '/' || path === '' || path.includes('/home/index') || path.endsWith('/home');

    function handleScroll() {
        if (isHome) return; // Se for a Home, o JS não empurra o menu!

        if ($(window).scrollTop() > 50) {
            nav.addClass('sticky-nav');
        } else {
            nav.removeClass('sticky-nav');
        }
    }

    // Aplicação de Tema
    function applyTheme(theme) {
        htmlElement.attr('data-theme', theme);
        $('body').toggleClass('dark', theme === 'dark');
        localStorage.setItem('varejo-theme', theme);
        themeCheckbox.prop('checked', theme === 'dark');
    }

    $(window).on('scroll', handleScroll);
    handleScroll();

    themeCheckbox.on('change', function () {
        applyTheme(this.checked ? 'dark' : 'light');
    });

    const savedTheme = localStorage.getItem('varejo-theme') || 'light';
    applyTheme(savedTheme);
    // #endregion

    // #region 2. FORM AUTO-FOCUS
    const camposFocus = ["DescricaoCategoria", "NomeFamilia", "NomeMarca", "NomeRazao", "Preco", "Complemento", "DescricaoTipoEmbalagem", "nomeUsuario"];
    const seletores = camposFocus.map(c => `input[name="${c}"]`).join(', ');
    $(seletores).filter(':visible:first').focus();
    // #endregion

    // #region 3. NAVEGAÇÃO COM ENTER
    $('form').on('keydown', 'input, select, textarea', function (e) {
        if (e.key === 'Enter') {
            if (this.tagName.toLowerCase() === 'textarea') return;
            e.preventDefault();
            const form = $(this).closest('form');
            const campos = form.find('input, select, textarea, button:not([type="button"])').filter(':visible:not([disabled]):not([readonly])');
            const indexAtual = campos.index(this);
            if (indexAtual > -1 && indexAtual + 1 < campos.length) {
                campos.eq(indexAtual + 1).focus();
            } else {
                form.submit();
            }
        }
    });
    // #endregion

    // #region 4. NAVBAR MOBILE (OUTSIDE CLICK)
    $(document).on('click', function (e) {
        const navbar = $('.navbar-collapse');
        const toggler = $('.navbar-toggler');
        if (navbar.hasClass('show') && !navbar.is(e.target) && navbar.has(e.target).length === 0 && !toggler.is(e.target) && toggler.has(e.target).length === 0) {
            navbar.collapse('hide');
        }
    });
    // #endregion

    // #region 5. BOTÃO VOLTAR AO TOPO
    const btnTop = $('.btn-top');
    const footer = $('footer');
    if (btnTop.length && footer.length) {
        $(window).on('scroll', function () {
            const scrollBottom = $(window).scrollTop() + $(window).height();
            const footerTop = footer.offset().top;
            if (scrollBottom >= footerTop) {
                const overlap = scrollBottom - footerTop;
                btnTop.css('bottom', (20 + overlap) + 'px');
            } else {
                btnTop.css('bottom', '20px');
            }
        });
    }
    // #endregion

    // #region 6. MOBILE DRILL-DOWN MENU LOGIC
    // Usar $(document).on garante que o evento funcione mesmo se o Bootstrap recriar o elemento
    $(document).on('click', '.mobile-drill-trigger', function (e) {
        e.preventDefault();
        const targetId = $(this).data('target');
        const title = $(this).data('title');

        $('#mobileMainView').removeClass('active').addClass('main-pushed');
        $(targetId).addClass('active');

        $('#mobileBackTitle').text(title);
        $('#mobileBackBtn').removeClass('d-none');
    });

    $(document).on('click', '#mobileBackBtn', function (e) {
        e.preventDefault();
        $('.mobile-view-container').removeClass('active');
        $('#mobileMainView').removeClass('main-pushed').addClass('active');
        $(this).addClass('d-none');
    });

    // Reset nativo do Bootstrap (sem jQuery para evitar o erro de backdrop)
    const offcanvasEl = document.getElementById('mobileMenuOffcanvas');
    if (offcanvasEl) {
        offcanvasEl.addEventListener('hidden.bs.offcanvas', function () {
            $('.mobile-view-container').removeClass('active main-pushed');
            $('#mobileMainView').addClass('active');
            $('#mobileBackBtn').addClass('d-none');
        });
    }
    $('#theme-checkbox-mobile').on('change', function () {
        $('#theme-checkbox').prop('checked', this.checked).trigger('change');
    });

    // #endregion

});