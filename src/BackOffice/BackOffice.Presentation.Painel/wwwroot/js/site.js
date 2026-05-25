const baseURL = obterBaseURL();

function obterBaseURL() {

    //if (window.location.href.indexOf("localhost") > -1)
    //    return "http://localhost:63304/"
    //else return BASE_URL;

    return BASE_URL;
}

function obterImagemProdutoURL(id) {

    return BANCO_IMAGENS_URL + id + ".jpg";
}


function goTo(route, forceReload = false) {

    if (route[0] == "/")
        route = route.slice(1);

    window.location.href = BASE_URL + route;

    if (forceReload)
        window.location.reload();

}

function resolveClientURL(route) {

    if (route[0] == "/")
        route = route.slice(1);

    return BASE_URL + route;
}


function noImageSrc(route) {

    return resolveClientURL('/img/no-image.png');
}


function showToastr(messages) {

    if (!isEmpty(messages)) {

        if (!Array.isArray(messages)) {
            messages = [messages];
        }

        messages.forEach(msg => {

            let config =
            {
                position: 'top-end',
                icon: 'success',
                title: msg.text,
                showCloseButton: true,
                showConfirmButton: false,
                timer: 8000,
                backdrop: false,
                customClass: {
                    title: "sweetalert2-title",
                    icon: "sweetalert2-icon"
                }
            }


            if (msg.type === 1 || msg.type === "success") {
                config.icon = "success"
            }
            else if (msg.type === 2 || msg.type === "invalidField") {
                config.icon = "warning"
            }
            else if (msg.type === 3 || msg.type === "error") {
                config.icon = "error"
            }
            else if (msg.type === 4 || msg.type === "information") {
                config.icon = "info"
            }
            else if (msg.type === 5 || msg.type === "notification") {
                config.icon = "info"
            }
            else if (msg.type === 6 || msg.type === "alert") {
                config.icon = "warning"
            }

            Swal.fire(config)

        });
    }
}


function formatarDataPtBrToInputDate(dataPtBr) {

    if (isEmpty(dataPtBr)) {
        return "";
    }

    let aux = dataPtBr.split("/");
    if (aux.length == 3)
        return aux[2] + "-" + aux[1] + "-" + aux[0];
    return dataPtBr;
}

function formatarDataHoraPtBrToInputDate(dataHoraPtBr, mostrarSegundos = false) {

    if (isEmpty(dataHoraPtBr)) {
        return "";
    }

    let aux = dataHoraPtBr.split("/");
    if (aux.length == 3) {

        let anoSemHora = aux[2].split(" ")[0];
        let hora = "";
        if (!isEmpty(aux[2].split(" ")[1])) {

            if (mostrarSegundos)
                hora = "T" + aux[2].split(" ")[1];
            else {

                let horaAux = aux[2].split(" ");
                hora = "T" + horaAux[1].split(":")[0] + ":" + horaAux[1].split(":")[1];
            }
        }

        return anoSemHora + "-" + aux[1] + "-" + aux[0] + hora;
    }
    return dataHoraPtBr;
}

function formatarDataInputDateToPtBr(dataInputDate, mostrarSegundos = false) {

    if (isEmpty(dataInputDate)) {
        return "";
    }

    if (dataInputDate == "0001-01-01T00:00:00")
        return "";

    let aux = "";
    if (mostrarSegundos)
        aux = dataInputDate.split("-");
    else {

        aux = dataInputDate.split("T")[0].split("-");
    }

    return aux[2] + "/" + aux[1] + "/" + aux[0];
}


function formatarDataHoraToInputDate(dataHora, mostrarSegundos = false) {

    if (isEmpty(dataHora)) {
        return "";
    }

    if (dataHora == "0001-01-01T00:00:00")
        return "";

    debugger
    let aux = dataHora;

    if (dataHora instanceof Date)
        aux = dataHora.toISOString();

    if (mostrarSegundos)
        return aux;
    else return aux.split("T")[0];

}



function formatarDataISOToPtBr(dataIso, mostrarSegundos = true) {

    if (dataIso == "0001-01-01T00:00:00")
        return "";


    // Criar um objeto Date a partir do formato ISO 8601
    const data = new Date(dataIso);

    // Obter os componentes da data
    let dia = data.getDate().toString();;
    let mes = (data.getMonth() + 1).toString();; // Mês é base 0, então adicionamos 1
    let ano = data.getFullYear().toString();;

    let hora = data.getHours().toString();
    let minutos = data.getMinutes().toString();
    let segundos = data.getSeconds().toString();

    dia = dia.padStart(2, "0");
    mes = mes.padStart(2, "0");
    hora = hora.padStart(2, "0");
    minutos = minutos.padStart(2, "0");
    segundos = segundos.padStart(2, "0");

    // Formatar a data e hora para o padrão brasileiro
    let dataFormatada = `${dia}/${mes}/${ano}`;

    if (mostrarSegundos)
        dataFormatada += ` ${hora}:${minutos}:${segundos}`;

    return dataFormatada;
}

function isEmpty(value) {
    return (
        // null or undefined
        (value == null) ||

        // has length and it's zero
        (value.hasOwnProperty('length') && value.length === 0) ||

        // is an Object and has no keys
        (value.constructor === Object && Object.keys(value).length === 0)
    )
}

function tryParseInt(value, defaultOut = "") {

    let x = parseInt(value);

    if (!isNaN(x))
        return parseInt(value);
    else return defaultOut;
}

function mostrarCarregando(mostrar) {

    var $divCarregando = $(".divCarregandoAjaxGlobalSistema_");
    if ($divCarregando.length === 0) {

        var html = ['<div class="loader-container">',
            '<div class="line"></div>',
            '<div class="subline inc"></div>',
            '<div class="subline dec"></div>',
            '</div>'];

        $divCarregando = $("<div class='no-print' />").addClass("divCarregandoAjaxGlobalSistema_").html(html.join(""));
        $divCarregando.appendTo($("body"));
        $divCarregando.data("fila", 0);
    }

    var fila = +$divCarregando.data("fila");
    if (mostrar) {
        fila++;
        $divCarregando.show();
    } else {
        fila--;
        if (fila <= 0) {
            $divCarregando.fadeOut();
        }
    }
    $divCarregando.data("fila", fila);
}


function obterDataAtual() {
    let today = new Date();
    let dd = String(today.getDate()).padStart(2, '0');
    let mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    let yyyy = today.getFullYear();

    return (yyyy + "-" + mm + "-" + dd)
}

function obterHoraAtual() {
    let today = new Date();
    let hh = String(today.getHours()).padStart(2, '0');
    let mm = String(today.getMinutes()).padStart(2, '0');
    return (hh + ":" + mm);
}


function irTopoModal() {
    document.querySelector(".modal-body").scrollTo({ top: 0, behavior: 'smooth' });
}


function queryString(name) {
    var url = rl = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}


function htmlToTextPlain(str) {
    return str.replace(/<br ?\/?>/g, "\n");
}

function textPlainToHTML(str) {
    if (isEmpty(str))
        return "";
    return str.toString().replace(/\n/g, "<br />").replace("\r\n", "<br />");
}

function floatToPTBRString(floatValue) {
    return floatValue.toLocaleString('pt-br', { currency: 'BRL', minimumFractionDigits: 2 });
}

function stringPTBRToFloat(strValue) {
    return parseFloat(strValue.replace(".", "").replace(",", "."));
}


function createGUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function tableSelectable() {

    $(".table-selectable tbody tr").unbind("click");
    $(".table-selectable tbody tr").on("click", function (e) {
        $(this).addClass("table-active").siblings().removeClass("table-active");
    });

}

function obterExtensaoArquivo(nomeArquivo) {
    const partes = nomeArquivo.split('.'); 
    return partes.length > 1 ? partes.pop().toLowerCase() : '';
}