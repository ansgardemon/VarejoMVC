$(document).ready(function () {
    // =========== SWITCH ===========
    const check = document.querySelector('.check');

    const savedTheme = localStorage.getItem('theme');
    if (savedTheme === 'dark') {
        document.body.classList.add('dark');
        check.checked = true;
    } else {
        document.body.classList.remove('dark');
        check.checked = false;
    }

    check.addEventListener('change', () => {
        if (check.checked) {
            document.body.classList.add('dark');
            localStorage.setItem('theme', 'dark');
        } else {
            document.body.classList.remove('dark');
            localStorage.setItem('theme', 'light');
        }
    });

    // =========== FOCUS ===========
    const campos = [
        "DescricaoCategoria",
        "NomeFamilia",
        "NomeMarca",
        "NomeRazao",
        "Preco",
        "Complemento",
        "DescricaoTipoEmbalagem",
        "nomeUsuario"
    ];

    $(`input[name="${campos.join('"], input[name="')}"]`).filter(':visible:first').focus();



    // =========== ENTER NOS FORMS ===========

    $('form').on('keydown', 'input, select, textarea', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();

            const form = $(this).closest('form');
            const campos = form.find('input, select, textarea, button')
                .filter(':visible:not([disabled])');
            const indexAtual = campos.index(this);

            // se houver próximo campo, foca nele
            if (indexAtual > -1 && indexAtual + 1 < campos.length) {
                campos.eq(indexAtual + 1).focus();
            }
            // se for o último campo, envia o formulário
            else {
                form.submit();
            }
        }
    });

});
