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


    // =========== VALIDAÇÃO CPF/CNPJ ===========
    // =========== CPF ===========
    function validarCPF(cpf) {
        cpf = cpf.replace(/\D/g, ""); // remove tudo que não é número
        if (cpf.length !== 11 || /^(\d)\1+$/.test(cpf)) return false;

        let soma = 0;
        for (let i = 0; i < 9; i++) soma += parseInt(cpf[i]) * (10 - i);
        let resto = (soma * 10) % 11;
        if (resto === 10 || resto === 11) resto = 0;
        if (resto !== parseInt(cpf[9])) return false;

        soma = 0;
        for (let i = 0; i < 10; i++) soma += parseInt(cpf[i]) * (11 - i);
        resto = (soma * 10) % 11;
        if (resto === 10 || resto === 11) resto = 0;
        return resto === parseInt(cpf[10]);
    }
    // =========== CNPJ ===========
    function validarCNPJ(cnpj) {
        cnpj = cnpj.replace(/\D/g, "");
        if (cnpj.length !== 14 || /^(\d)\1+$/.test(cnpj)) return false;

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

        tamanho++;
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
    // =========== VALIDADOR CPF/CNPJ ===========
    $('#CpfCnpj').on('blur', function () {
        var valor = $(this).val().replace(/\D/g, '');

        if (valor.length === 11) {
            // CPF
            if (!validarCPF(valor)) {
                alert('CPF inválido!');
                $(this).val('').focus();
            }
        } else if (valor.length === 14) {
            // CNPJ
            if (!validarCNPJ(valor)) {
                alert('CNPJ inválido!');
                $(this).val('').focus();
            }
        } else if (valor.length > 0) {
            alert('CPF ou CNPJ com quantidade incorreta de dígitos!');
            $(this).val('').focus();
        }
    });

    // =========== CEP ===========
    function limpa_formulario_cep(container) {
        container.find("[name$='.Logradouro']").val('');
        container.find("[name$='.Bairro']").val('');
        container.find("[name$='.Cidade']").val('');
        container.find("[name$='.Uf']").val('');
    }

    function consulta_cep(inputCep) {
        var container = $(inputCep).closest('.endereco-item');
        var cep = $(inputCep).val().replace(/\D/g, '');

        if (cep !== '') {
            var validacep = /^[0-9]{8}$/;

            if (validacep.test(cep)) {
                container.find("[name$='.Logradouro']").val('...');
                container.find("[name$='.Bairro']").val('...');
                container.find("[name$='.Cidade']").val('...');
                container.find("[name$='.Uf']").val('...');

                $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {
                    if (!("erro" in dados)) {
                        container.find("[name$='.Logradouro']").val(dados.logradouro);
                        container.find("[name$='.Bairro']").val(dados.bairro);
                        container.find("[name$='.Cidade']").val(dados.localidade);
                        container.find("[name$='.Uf']").val(dados.uf);
                    } else {
                        limpa_formulario_cep(container);
                        alert("CEP não encontrado.");
                    }
                });
            } else {
                limpa_formulario_cep(container);
                alert("Formato de CEP inválido.");
            }
        } else {
            limpa_formulario_cep(container);
        }
    }

    $(document).on('blur', "[name$='.Cep']", function () {
        consulta_cep(this);
    });


    // =========== ADICIONAR ENDEREÇO ===========
    $('#adicionar-endereco').on('click', function () {
        var container = $('#enderecos-container');
        var index = container.children().length;
        var html = `
		<div class="endereco-item mb-3 border p-2">
			<div class="mb-3">
				<label for="Enderecos_${index}__Logradouro" class="form-label">Logradouro</label>
				<input name="Enderecos[${index}].Logradouro" class="form-control" />
			</div>

			<div class="mb-3">
				<label for="Enderecos_${index}__Numero" class="form-label">Número</label>
				<input name="Enderecos[${index}].Numero" class="form-control" />
			</div>

			<div class="mb-3">
				<label for="Enderecos_${index}__Cep" class="form-label">CEP</label>
				<input name="Enderecos[${index}].Cep" class="form-control" />
			</div>

			<div class="mb-3">
				<label for="Enderecos_${index}__Bairro" class="form-label">Bairro</label>
				<input name="Enderecos[${index}].Bairro" class="form-control" />
			</div>

			<div class="mb-3">
				<label for="Enderecos_${index}__Cidade" class="form-label">Cidade</label>
				<input name="Enderecos[${index}].Cidade" class="form-control" />
			</div>

			<div class="mb-3">
				<label for="Enderecos_${index}__Uf" class="form-label">UF</label>
				<input name="Enderecos[${index}].Uf" class="form-control" />
			</div>

			<div class="mb-3">
				<label for="Enderecos_${index}__Complemento" class="form-label">Complemento</label>
				<input name="Enderecos[${index}].Complemento" class="form-control" />
			</div>

			<div class="mb-3 d-flex justify-content-end">
				<button type="button" class="btn btn-danger mt-2 remover-endereco">Remover</button>
			</div>
		</div>

		`;
        container.append(html);
        atualizarTransformEndereco();
    });

    // =========== REMOVER ENDEREÇO ===========
    $(document).on('click', '.remover-endereco', function () {
        $(this).closest('.endereco-item').remove();

        atualizarTransformEndereco();
    });

    // =========== AJUSTE DE ALTURA DO BOTÃO ===========
    function atualizarTransformEndereco() {
        const quantidade = $('#enderecos-container .endereco-item').length;
        const altura = 98; // altura fixa que você quer que o botão suba
        const translate = quantidade > 0 ? -altura : 0; // sobe apenas 1 vez
        $('.btns-endereco').css('transform', `translateY(${translate}px)`);
    }
});
