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
            //nav.addClass('sticky-nav');
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

    // #region 3. NAVEGAÇÃO COM ENTER (Formulários)
    $('form').on('keydown', 'input, select', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault(); // Impede o envio acidental antes da hora
            const form = $(this).closest('form');

            // Mapeia todos os inputs visíveis, habilitados e o botão de submit final
            const campos = form.find('input, select, textarea, button[type="submit"]').filter(':visible:not([disabled]):not([readonly])');
            const indexAtual = campos.index(this);

            // Se houver um próximo campo, foca nele. Se for o último, submete.
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

    // #region 11. ENGINE DE ORDENAÇÃO DE TABELAS UNIVERSAL
    $(document).on('click', 'th[data-sortable="true"]', function () {
        const th = $(this);
        const table = th.closest('table');
        const tbody = table.find('tbody');
        const rows = tbody.find('tr').toArray();

        // Ignora se a tabela estiver vazia (linha de "Nenhum registro")
        if (rows.length === 1 && $(rows[0]).find('td').attr('colspan')) return;

        const columnIndex = th.index();
        const type = th.data('sort-type') || 'string'; // 'number' ou 'string'

        let isAsc = table.attr('data-sort-dir') === 'asc';
        const currentSortCol = table.attr('data-sort-col');

        if (currentSortCol == columnIndex) {
            isAsc = !isAsc; // Inverte direção
        } else {
            isAsc = true; // Zera para A-Z
        }

        table.attr('data-sort-dir', isAsc ? 'asc' : 'desc');
        table.attr('data-sort-col', columnIndex);

        // Reseta todos os ícones desta tabela específica
        table.find('th i.fa-sort, th i.fa-sort-up, th i.fa-sort-down')
            .attr('class', 'fa-solid fa-sort opacity-50 ms-2');

        // Destaca o ícone da coluna clicada
        const activeIcon = th.find('i');
        if (activeIcon.length) {
            activeIcon.attr('class', isAsc ? 'fa-solid fa-sort-up text-primary opacity-100 ms-2' : 'fa-solid fa-sort-down text-primary opacity-100 ms-2');
        }

        // Processamento da ordenação
        rows.sort(function (a, b) {
            let valA = $(a).find('td').eq(columnIndex).text().trim();
            let valB = $(b).find('td').eq(columnIndex).text().trim();

            if (type === 'number') {
                // Remove tudo que não for número, ponto ou sinal negativo (para moedas e IDs)
                valA = parseFloat(valA.replace(/[^\d.-]/g, '')) || 0;
                valB = parseFloat(valB.replace(/[^\d.-]/g, '')) || 0;
                return isAsc ? valA - valB : valB - valA;
            }

            return isAsc ? valA.localeCompare(valB) : valB.localeCompare(valA);
        });

        // Reinsere as linhas na tabela
        $.each(rows, function (index, row) {
            tbody.append(row);
        });
    });
    // #endregion

    // #region 12. UTILITÁRIOS: MÁSCARAS, VALIDAÇÕES E VIACEP

    // --- MÁSCARAS INTELIGENTES ---
    $(document).on('input', '.cpf-cnpj-mask', function () {
        let v = $(this).val().replace(/\D/g, "");
        if (v.length <= 11) {
            v = v.replace(/(\d{3})(\d)/, "$1.$2");
            v = v.replace(/(\d{3})(\d)/, "$1.$2");
            v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
        } else {
            v = v.replace(/^(\d{2})(\d)/, "$1.$2");
            v = v.replace(/^(\d{2})\.(\d{3})(\d)/, "$1.$2.$3");
            v = v.replace(/\.(\d{3})(\d)/, ".$1/$2");
            v = v.replace(/(\d{4})(\d)/, "$1-$2");
        }
        $(this).val(v.length > 18 ? v.substring(0, 18) : v);
    });

    $(document).on('input', '.cep-mask', function () {
        let v = $(this).val().replace(/\D/g, "");
        v = v.replace(/(\d{5})(\d)/, "$1-$2");
        $(this).val(v.length > 9 ? v.substring(0, 9) : v);
    });

    $(document).on('input', '.tel-mask', function () {
        let v = $(this).val().replace(/\D/g, "");
        v = v.replace(/(\d{5})(\d)/, "$1-$2");
        $(this).val(v.length > 10 ? v.substring(0, 10) : v);
    });

    // --- BUSCA VIACEP (DINÂMICA PARA MÚLTIPLOS ENDEREÇOS) ---
    $(document).on('blur', '.cep-mask', function () {
        const cep = $(this).val().replace(/\D/g, '');
        const container = $(this).closest('.endereco-item'); // Encontra o card do endereço atual

        if (cep !== "" && /^[0-9]{8}$/.test(cep)) {
            $(this).addClass('is-loading');

            $.getJSON(`https://viacep.com.br/ws/${cep}/json/?callback=?`, function (dados) {
                if (!("erro" in dados)) {
                    container.find('input[name*="Logradouro"]').val(dados.logradouro);
                    container.find('input[name*="Bairro"]').val(dados.bairro);
                    container.find('input[name*="Cidade"]').val(dados.localidade);
                    container.find('input[name*="Uf"]').val(dados.uf);
                    container.find('input[name*="Numero"]').focus();
                } else {
                    alert("CEP não encontrado.");
                }
            }).always(function () {
                $('.cep-mask').removeClass('is-loading');
            });
        }
    });

    // --- VALIDAÇÃO REAL-TIME CPF/CNPJ E AUTO-CHECK JURÍDICO ---
    $(document).on('blur', '.cpf-cnpj-mask', function () {
        const valor = $(this).val().replace(/\D/g, '');
        const input = $(this);

        if (valor === "") {
            input.removeClass('is-invalid is-valid');
            input.next('.invalid-feedback').remove();
            return;
        }

        let valido = false;
        let isCnpj = false;

        if (valor.length === 11) {
            valido = validarCPF(valor);
        } else if (valor.length === 14) {
            valido = validarCNPJ(valor);
            isCnpj = true;
        }

        if (!valido) {
            input.addClass('is-invalid').removeClass('is-valid');
            if (!input.next('.invalid-feedback').length) {
                input.after('<div class="invalid-feedback fw-bold">Documento inválido. Verifique os números.</div>');
            }
        } else {
            input.removeClass('is-invalid').addClass('is-valid');
            input.next('.invalid-feedback').remove();

            // MÁGICA: Se for um CNPJ válido, marca a flag EhJuridico automaticamente
            if (isCnpj) {
                const checkJuridico = $('#EhJuridico');
                // Ativa a flag se ela já não estiver ativa
                if (checkJuridico.length && !checkJuridico.prop('checked')) {
                    checkJuridico.prop('checked', true).trigger('change');
                }
            }
        }
    });

    // --- FUNÇÕES MATEMÁTICAS REAIS DE VALIDAÇÃO ---
    function validarCPF(cpf) {
        if (cpf.length !== 11 || !!cpf.match(/(\d)\1{10}/)) return false;
        cpf = cpf.split('').map(el => +el);
        const rest = (count) => (cpf.slice(0, count - 12).reduce((soma, el, i) => soma + el * (count - i), 0) * 10) % 11 % 10;
        return rest(10) === cpf[9] && rest(11) === cpf[10];
    }

    function validarCNPJ(cnpj) {
        if (cnpj.length !== 14 || !!cnpj.match(/(\d)\1{13}/)) return false;
        let tamanho = cnpj.length - 2;
        let numeros = cnpj.substring(0, tamanho);
        let digitos = cnpj.substring(tamanho);
        let soma = 0;
        let pos = tamanho - 7;
        for (let i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2) pos = 9;
        }
        let resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
        if (resultado != digitos.charAt(0)) return false;

        tamanho = tamanho + 1;
        numeros = cnpj.substring(0, tamanho);
        soma = 0;
        pos = tamanho - 7;
        for (let i = tamanho; i >= 1; i--) {
            soma += numeros.charAt(tamanho - i) * pos--;
            if (pos < 2) pos = 9;
        }
        resultado = soma % 11 < 2 ? 0 : 11 - (soma % 11);
        return resultado == digitos.charAt(1);
    }
    // #endregion

});