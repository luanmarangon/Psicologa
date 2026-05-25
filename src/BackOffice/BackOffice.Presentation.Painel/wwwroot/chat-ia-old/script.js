const chatInput = document.querySelector("#chat-input");
const sendButton = document.querySelector("#send-btn");
const sendImagemButton = document.querySelector("#send-image-btn");
const chatContainer = document.querySelector(".chat-container");
const typingContainer = document.querySelector(".typing-container");
const menuAssuntosButton = document.querySelector("#menu-assuntos-btn");
const themeButton = document.querySelector("#theme-btn");
const deleteButton = document.querySelector("#delete-btn");

let userText = null;

let introduzido = false;
let clienteIdentificado = false;
let exibirMenuAssuntos = false;
let assuntoEscolhido = "";
let cnpj = "";
let chatGPTAssistenteId = "";
let threadId = ""; //Não deve ser armazenado no Storage. A cada nova entrada no Chat ou troca de tema, um novo deverá ser gerado.
let imagensEnviadas = [];

const init = () => {
    let themeColor = localStorage.getItem("themeColor");

    if (themeColor == null)
    {
        themeColor = "light-mode";
        localStorage.setItem("themeColor", "light-mode");
    }

    document.body.classList.add(themeColor);

    if (themeColor == "dark-mode")
        document.body.setAttribute("data-bs-theme", "dark");
    else document.body.setAttribute("data-bs-theme", "");

    themeButton.innerText = themeColor == "light-mode" ? "dark_mode" : "light_mode";



    showTypingAnimation();

}

const createChatElement = (content, className) => {
    
    const chatDiv = document.createElement("div");
    chatDiv.classList.add("chat", className);
    chatDiv.innerHTML = content;
    return chatDiv; 
}

const getChatResponse = async (incomingChatDiv) => {

    let bodyRequest = {
        prompt: userText,
        cnpj: cnpj,
        acao: "responder-duvida",
        chatGPTAssistenteId: chatGPTAssistenteId,
        threadId: threadId, 
        assunto: assuntoEscolhido,
        imagensFileIds: []
    }

    if (!introduzido) {
        bodyRequest.acao = "introduzir";
    }
    else if (!clienteIdentificado) {
        bodyRequest.acao = "validar-cliente";
    }
    else if (exibirMenuAssuntos) {
        bodyRequest.acao = "gerar-menu-assuntos";
    }
    else if (assuntoEscolhido == "") {
        bodyRequest.acao = "definir-assunto";
    }

    const pElement = document.createElement("p");
    pElement.classList.add("response");

 

    try {

        typingContainer.classList.add("disabled");

        if (bodyRequest.acao == "responder-duvida") {
            bodyRequest.imagensFileIds = await enviarImagensAsync();
        }

        const requestOptions = {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(bodyRequest)
        }


        const response = await (await fetch("/chatia/ProcessarPrompt", requestOptions)).json();
        let resposta = response.resposta;

        switch (bodyRequest.acao) {

            case "introduzir":
                {
                    if (response.sucesso) {
                        introduzido = true;
                    }
                    break;
                }
            case "validar-cliente":
                {

                    clienteIdentificado = false;
                    exibirMenuAssuntos = false;

                    if (response.sucesso) {
                        clienteIdentificado = true;
                        exibirMenuAssuntos = true;
                        cnpj = userText;
                    }
                    break;
                }
            case "gerar-menu-assuntos":
                {
                    if (response.sucesso) {
                         
                        exibirMenuAssuntos = false;
                        assuntoEscolhido = "";
                    }
                    break;
                }
            case "definir-assunto":
                {
                    if (response.sucesso) {
                        assuntoEscolhido = userText;
                        chatGPTAssistenteId = response.chatGPTAssistenteId;
                    }
                    else { 
                          assuntoEscolhido = "";
                    }

                    break;
                }
            case "responder-duvida":
                {
                   
                    if (response.sucesso) {

                        if (response.threadId)
                            threadId = response.threadId;

                        imagensEnviadas = [];
                        exibirImagens();
                    }
                    break;
                }

        }

        if (response.botoes) {
            let htmlBotes = "";
            response.botoes.forEach(btn => {
                htmlBotes += `<button type="button" onclick="handleChatButtons(this, '${btn.nome}')">${btn.nome}</button>`;
            });

            resposta += `<span class="chat-buttons">${htmlBotes}</span>`;
        }

        pElement.innerHTML = resposta;

    }
    catch (error)
    { 
        pElement.classList.add("error");
        pElement.textContent = "Ops! Algo deu errado ao recuperar a resposta. Por favor, tente novamente.";
    }
    finally {

        typingContainer.classList.remove("disabled");
    }

    // Remove a animação de digitação, adicionar a nova resposta e arquivo os chats no local storage
    incomingChatDiv.querySelector(".typing-animation").remove();
    incomingChatDiv.querySelector(".chat-details").appendChild(pElement);
    //localStorage.setItem("all-chats", chatContainer.innerHTML);
    chatContainer.scrollTo(0, chatContainer.scrollHeight);

    if (exibirMenuAssuntos) {
        showTypingAnimation();
    }

    setTimeout(() => {
        resolverLinkTargetBlank();
        hljs.highlightAll();
    }, 100);
}

const copyResponse = (copyBtn) => {
    const reponseTextElement = copyBtn.parentElement.querySelector("p");
    navigator.clipboard.writeText(reponseTextElement.textContent);
    copyBtn.textContent = "done";
    setTimeout(() => {
        copyBtn.textContent = "content_copy";
        copyBtn.className = "material-symbols-outlined";
    }, 1000);
}

const showTypingAnimation = () => {

    const html = `<div class="chat-content">
                    <div class="chat-details">
                        <img src="/chat-ia/images/chatbot.png" alt="chatbot-img" class="img-user">
                        <div class="typing-animation">
                            <div class="typing-dot" style="--delay: 0.2s"></div>
                            <div class="typing-dot" style="--delay: 0.3s"></div>
                            <div class="typing-dot" style="--delay: 0.4s"></div>
                        </div>
                    </div>
                    <span onclick="copyResponse(this)" class="btn-copy material-symbols-outlined">content_copy</span>
                </div>`;
    
    const incomingChatDiv = createChatElement(html, "incoming");
    chatContainer.appendChild(incomingChatDiv);
    chatContainer.scrollTo(0, chatContainer.scrollHeight);
    getChatResponse(incomingChatDiv);
}

const handleOutgoingChat = (prompt = "") => {

    if (prompt != "")
        userText = prompt;
    else userText = chatInput.value.trim(); 

    if (!userText) return; 

    
    chatInput.value = "";
    chatInput.style.height = `${initialInputHeight}px`;

    let userTextPrepered = userText.replace(/\n/g, "<br>");

    const html = `<div class="chat-content">
                    <div class="chat-details">
                        <img src="/chat-ia/images/user.jpg" alt="user-img" class="img-user">
                        <p class="response">${userTextPrepered}</p>
                    </div>
                </div>`;

    //Crie uma div de chat de saída com a mensagem do usuário e anexa-a ao contêiner do chat
    const outgoingChatDiv = createChatElement(html, "outgoing");
    chatContainer.querySelector(".default-text")?.remove();
    chatContainer.appendChild(outgoingChatDiv);
    chatContainer.scrollTo(0, chatContainer.scrollHeight);
    setTimeout(showTypingAnimation, 500);
    
}

const handleChatButtons = (button, prompt) => {

    button.parentElement.classList.add("disabled");
    handleOutgoingChat(prompt);
}

deleteButton.addEventListener("click", () => {

      
    if (confirm("Você tem certeza de que deseja excluir todos os chats?")) {

        introduzido = false;
        clienteIdentificado = false;
        exibirMenuAssuntos = false;
        assuntoEscolhido = "";
        cnpj = "";
        chatGPTAssistenteId = "";
        threadId = "";
        chatContainer.innerHTML = "";

        imagensEnviadas = [];
        exibirImagens();

        init();
            
    }
    
});

sendImagemButton.addEventListener("click", () => {

    if (assuntoEscolhido == "") {

        alert("Primeiro selecione um assunto");
        return;
    }

    if (imagensEnviadas.length >= 3) {

        alert("Limite de 3 imagens atingido.");
        return;
    }

    let inputFile = document.getElementById("inputImage");

    if (!inputFile) {
        
        inputFile = document.createElement('input');
        inputFile.id = "inputImage";
        inputFile.type = 'file';
        inputFile.accept = '.jpg, .jpeg, .png';
        inputFile.style.display = "none";
        document.body.appendChild(inputFile);

        inputFile.addEventListener('change', (event) => {
            const files = event.target.files; // Arquivos selecionados
            debugger
            if (files.length > 0) {
                const file = files[0];
                const fileSize = file.size; 

                if (inputFile.accept.indexOf(obterExtensaoArquivo(file.name)) == -1)
                    alert('Formato não suportado. Suporto apenas .jpg, .jpeg, .png.');
                else if (fileSize > 300 * 1024) {
                    alert('O arquivo é muito grande. O tamanho máximo permitido é 300 KB.');
                } else {
                    console.log(`Você selecionou o arquivo: ${file.name}`);

                    let item = {
                        file,
                        id: createGUID()
                    }

                    imagensEnviadas.push(item);
                    exibirImagens();
                }
            }

            event.target.value = "";
        });

    }
    
    inputFile.click();

});

const enviarImagensAsync = async () => {

    if (imagensEnviadas.length == 0)
        return [];

    const formData = new FormData();

    for (let i = 0; i < imagensEnviadas.length; i++) {
        formData.append('files', imagensEnviadas[i].file); // "files" será o nome do campo no servidor
    }

    try
    {
        const response = await fetch('/chatia/ReceberImagens', {
            method: 'POST',
            body: formData,  
        });

        
        if (!response.ok) {
            alert("Falha ao enviar os arquivos das imagens");
            throw 'Não foi possível enviar o(s) arquivo(s). Tente novamente.'; 

        }
        else {
            const dados = await response.json();

            if (dados.success)
                return dados.fileIds
            else {
                alert(dados.message);
            }
        }
       
    }
    catch (error) {
        throw 'Não foi possível enviar o(s) arquivo(s). Tente novamente. ' + error; 
    }

    return [];
}


const exibirImagens = () => {
    let fileList = document.getElementById("fileList");

    if (!fileList) {
        fileList = document.createElement('ul');
        fileList.id = "fileList";
        document.querySelector(".typing-content-part-1").appendChild(fileList);
    }

    fileList.innerHTML = '';

    imagensEnviadas.forEach((item) => {
        let li = document.createElement('li');
        li.title = "Remover";
        li.setAttribute("data-file-id", item.id);

        // Cria o img e spanRemove antes do uso
        const img = document.createElement('img');
        const spanRemove = document.createElement('span');
        spanRemove.className = "material-symbols-outlined remove-file";
        spanRemove.innerHTML = "cancel";

        // Adiciona img e spanRemove ao li
        li.appendChild(img);
        li.appendChild(spanRemove);

        // Evento de remoção
        li.addEventListener("click", () => {
            let id = li.getAttribute("data-file-id");
            li.parentNode.removeChild(li);
            imagensEnviadas = imagensEnviadas.filter(a => a.id != id);
        });

        fileList.appendChild(li);

        // Leitura do arquivo como Data URL para o img.src
        const reader = new FileReader();
        reader.onload = (e) => {
            img.src = e.target.result; // Define o src do img após a leitura
        };

        reader.readAsDataURL(item.file);
    });
};


themeButton.addEventListener("click", () => {
    
    let themeColor = localStorage.getItem("themeColor");

    document.body.classList.remove(themeColor);
    if (themeColor == "light-mode") {
        themeColor = "dark-mode";
        document.body.setAttribute("data-bs-theme", "dark")
    }
    else {
        themeColor = "light-mode";
        document.body.setAttribute("data-bs-theme", "")
    }
    
    document.body.classList.add(themeColor);
    localStorage.setItem("themeColor", themeColor);
    
    themeButton.innerText = themeColor == "light-mode" ? "dark_mode" : "light_mode"; 
});

menuAssuntosButton.addEventListener("click", () => {

    exibirMenuAssuntos = true;
    assuntoEscolhido = "";
    chatGPTAssistenteId = "";
    threadId = "";

    imagensEnviadas = [];
    exibirImagens();
    showTypingAnimation();

});


const initialInputHeight = chatInput.scrollHeight;

chatInput.addEventListener("input", () => {   
    
    chatInput.style.height =  `${initialInputHeight}px`;
    chatInput.style.height = `${chatInput.scrollHeight}px`;
});

chatInput.addEventListener("keydown", (e) => {

    if (e.key === "Enter" && !e.shiftKey && window.innerWidth > 800) {
        e.preventDefault();
        handleOutgoingChat();
    }
});

init();
sendButton.addEventListener("click", () => { handleOutgoingChat() });


const resolverLinkTargetBlank = () => {

    let links = document.querySelectorAll(".chat .chat-details a")
    undefined
    links.forEach(link => {
        link.target = "_blank";
    });
}

