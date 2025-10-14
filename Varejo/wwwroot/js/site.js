$(document).ready(function () {

    /* ==========================
       MÁSCARA CPF / CNPJ
    =========================== */
    const cpfCnpjMask = function (val) {
        const num = val.replace(/\D/g, '');
        return num.length <= 11 ? '000.000.000-009' : '00.000.000/0000-00';
    };

    const cpfCnpjOptions = {
        onKeyPress: function (val, e, field, options) {
            field.mask(cpfCnpjMask.apply({}, arguments), options);
        }
    };

    // funciona para qualquer input que tenha id ou name CpfCnpj
    $('input[id*="CpfCnpj"], input[name*="CpfCnpj"]').mask(cpfCnpjMask, cpfCnpjOptions);


    /* ==========================
       MÁSCARA CEP (00000-000)
    =========================== */
    $('input[id*="Cep"], input[name*="Cep"]').mask('00000-000');


    /* ==========================
       MÁSCARA TELEFONE (sem DDD)
       Ex: 9 9999-9999 ou 9999-9999
    =========================== */
    const phoneMask = function (val) {
        const num = val.replace(/\D/g, '');
        return num.length > 8 ? '00000-0000' : '0000-00009';
    };

    const phoneOptions = {
        onKeyPress: function (val, e, field, options) {
            field.mask(phoneMask.apply({}, arguments), options);
        }
    };

    // aplica a todos os inputs com nome ou id contendo "Telefone"
    $('input[id*="Telefone"], input[name*="Telefone"]').mask(phoneMask, phoneOptions);


    ///* ==========================
    //   MÁSCARA DDD
    //   Ex: 00
    //=========================== */
    //$('input[id*="Ddd"], input[name*="Ddd"]').mask('(00)');

});

//switch

const check = document.querySelector('.check');

// Carrega tema salvo
const savedTheme = localStorage.getItem('theme');
if (savedTheme === 'dark') {
    document.body.classList.add('dark');
    check.checked = true;
} else {
    document.body.classList.remove('dark');
    check.checked = false;
}

// Toggle tema
check.addEventListener('change', () => {
    if (check.checked) {
        document.body.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    } else {
        document.body.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    }
});
