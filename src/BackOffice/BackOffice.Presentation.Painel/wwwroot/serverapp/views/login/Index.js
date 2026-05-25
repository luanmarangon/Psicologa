var index = {

    init: function () {
        sessionStorage.removeItem("notificacoes");
    },

    entrar: function (evt) {

        evt.preventDefault();

        var formData = new FormData(document.forms[0]);
        
        HTTPClient.postFormData("Login/Validar", formData)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (!r.success) {
                    showToastr(r.messages);
                }
                else {
                    window.location.href = r.data.urlRedirect;
                }
            })
            .catch(e => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu. Tente novamente."
                });
            });

    },


    enviarEmailRecuperarSenha: function (evt) {

        evt.preventDefault();

        var formData = new FormData(document.forms[0]);

        HTTPClient.postFormData("Login/EnviarEmailRecuperarSenha", formData)
            .then(r => {
                return r.json();
            })
            .then(r => {

                showToastr(r.messages);

                if (r.success) {
    
                    document.getElementById("nome").disabled = "disabled";
                    document.getElementById("divCodigoSeguranca").classList.remove("d-none");
                    document.getElementById("btnTrocar").classList.remove("d-none");
                    document.getElementById("btnEnviarEmail").classList.add("d-none");

                    setTimeout(function () {
                        document.getElementById("divCodigoSeguranca").focus();
                    }, 100);
                }
            })
            .catch(e => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu. Tente novamente."
                });

            });

    },

    trocarSenha: function (evt) {

        evt.preventDefault();

        var formData = new FormData(document.forms[0]);
        formData.append("nome", document.getElementById("nome").value); //está readonly, então é preciso forçar o envio.

        HTTPClient.postFormData("Login/AlterarSenha", formData)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (!r.success) {
                    showToastr(r.messages);
                }
                else {

                    window.location.href = "Index";

                }
            })
            .catch(e => {

                showToastr({
                    type: "error",
                    text: "Um erro ocorreu. Tente novamente."
                });

            });

    }
}

index.init();