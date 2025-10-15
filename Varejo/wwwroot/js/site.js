$(document).ready(function () {

    // =========== CPF - CNPJ ===========

    const cpfCnpjMask = function (val) {
        const num = val.replace(/\D/g, '');
        return num.length <= 11 ? '000.000.000-009' : '00.000.000/0000-00';
    };

    const cpfCnpjOptions = {
        onKeyPress: function (val, e, field, options) {
            field.mask(cpfCnpjMask.apply({}, arguments), options);
        }
    };

    $('input[id*="CpfCnpj"], input[name*="CpfCnpj"]').mask(cpfCnpjMask, cpfCnpjOptions);

    const $cpfCnpj = $('#CpfCnpj');
    const $juridico = $('#EhJuridico');

    $cpfCnpj.on('input', function () {
        const valor = $(this).val().replace(/\D/g, ''); // remove tudo que não é número

        if (valor.length > 11) {
            $juridico.prop('checked', true); // ativa o checkbox
        } else {
            $juridico.prop('checked', false); // desativa
        }
    });

    // =========== CEP ===========
    $('input[id*="Cep"], input[name*="Cep"]').mask('00000-000');

    // =========== TELEFONE ===========
    const phoneMask = function (val) {
        const num = val.replace(/\D/g, '');
        return num.length > 8 ? '00000-0000' : '0000-00009';
    };

    const phoneOptions = {
        onKeyPress: function (val, e, field, options) {
            field.mask(phoneMask.apply({}, arguments), options);
        }
    };

    $('input[id*="Telefone"], input[name*="Telefone"]').mask(phoneMask, phoneOptions);

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
    $('input[name="DescricaoCategoria"]').focus();
    $('input[name="NomeFamilia"]').focus();
    $('input[name="NomeMarca"]').focus();
    $('input[name="NomeRazao"]').focus();
    $('input[name="Preco"]').focus();
    $('input[name="Complemento"]').focus();
    $('input[name="DescricaoTipoEmbalagem"]').focus();
    $('input[name="nomeUsuario"]').focus();
});