import React, { Component } from 'react';
import { createRoot } from 'react-dom/client';
import LoadingIndicator from '../../components/LoadingIndicator';

// ─── Página principal ─────────────────────────────────────────────────────────
export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            aguarde: true,
            enviandoEmail: false,
            blogs: [],           // ← inicializado corretamente
            servicos: [],
            convenios: [],       // ← inicializado corretamente
            nome: "",
            email: "",
            assunto: "",
            mensagem: "",
        };
    }

    componentDidMount = () => {
        const hash = window.location.hash;

        if (hash) {

            setTimeout(() => {

                const elemento = document.querySelector(hash);

                if (elemento) {
                    elemento.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }

            }, 200);
        }
        this.obterBlogs().finally(() => this.setState({ aguarde: false }));
        this.obterServicos().finally(() => this.setState({ aguarde: false }));
        this.obterConvenios().finally(() => this.setState({ aguarde: false }));
    }

    componentDidUpdate = () => { }

    obterBlogs = () => {
        return HTTPClient.get(`Blog/ObterUltimos`)
            .then(r => r.json())
            .then(r => { if (r.success) this.setState({ blogs: r.data }); })
            .catch(() => showToastr({ type: "error", text: "Um erro ocorreu ao obter os blogs." }));
    }

    obterServicos = () => {
        return HTTPClient.get(`Servico/ObterDestaquesHome`)
            .then(r => r.json())
            .then(r => { if (r.success) this.setState({ servicos: r.data }); })
            .catch(() => showToastr({ type: "error", text: "Um erro ocorreu ao obter os serviços." }));
    }

    obterConvenios = () => {
        return HTTPClient.get(`Home/ObterConvenioDestaques`)
            .then(r => r.json())
            .then(r => { if (r.success) this.setState({ convenios: r.data }); })
            .catch(() => showToastr({ type: "error", text: "Um erro ocorreu ao obter os convênios." }));
    }


    enviarEmail = () => {
        this.setState({ enviandoEmail: true });
        const { nome, email, assunto, mensagem } = this.state;
        if (!isEmpty(nome) && !isEmpty(email) && !isEmpty(assunto) && !isEmpty(mensagem) && email.includes("@")) {
            HTTPClient.post("Home/EnviarEmailContato", { nome, email, assunto, mensagem })
                .then(r => r.json())
                .then(r => { this.setState({ nome: "", email: "", assunto: "", mensagem: "" }); showToastr(r.messages); })
                .catch(() => showToastr({ type: "error", text: "Um erro ocorreu." }))
                .finally(() => this.setState({ enviandoEmail: false }));
        } else {
            showToastr({ type: "error", text: "Preencha todos os campos." });
            this.setState({ enviandoEmail: false });
        }
    }


    renderSobre = () => (
        <section className="ps-section bg-white">
            <div className="container">

                <div className="row align-items-center">

                    <div className="col-lg-6 mb-5 mb-lg-0">

                        <div className="ps-image-wrapper">
                            <img
                                src="/img/psicologa-sobre.jpg"
                                alt="Psicóloga"
                                className="img-fluid"
                            />
                        </div>

                    </div>

                    <div className="col-lg-6">

                        <span className="ps-tag">
                            Sobre nós
                        </span>

                        <h2 className="ps-title">
                            Um espaço seguro para cuidar da sua saúde emocional
                        </h2>

                        <p className="ps-text">
                            Nosso objetivo é proporcionar acolhimento,
                            escuta qualificada e acompanhamento psicológico
                            para auxiliar no desenvolvimento emocional,
                            autoconhecimento e qualidade de vida.
                        </p>

                        <div className="ps-benefits">

                            <div className="ps-benefit-item">
                                <div className="ps-icon">🌿</div>
                                <div>
                                    <strong>Acolhimento Humanizado</strong>
                                    <p>Atendimento com empatia, ética e cuidado.</p>
                                </div>
                            </div>

                            <div className="ps-benefit-item">
                                <div className="ps-icon">💬</div>
                                <div>
                                    <strong>Escuta Especializada</strong>
                                    <p>Um ambiente seguro para compartilhar emoções.</p>
                                </div>
                            </div>

                            <div className="ps-benefit-item">
                                <div className="ps-icon">✨</div>
                                <div>
                                    <strong>Desenvolvimento Pessoal</strong>
                                    <p>Auxílio na construção do equilíbrio emocional.</p>
                                </div>
                            </div>

                        </div>

                    </div>

                </div>

            </div>
        </section>
    );

    renderServicos = (servicos) => {

        const servicosHome = servicos.slice(0, 6);

        return (
            <section className="ps-section bg-light">
                <div className="container">

                    {/* Cabeçalho */}
                    <div className="text-center mb-5">

                        <span className="ps-tag">
                            Serviços
                        </span>

                        <h2 className="ps-title-center">
                            Como posso te ajudar
                        </h2>

                        <p className="ps-text-center mx-auto" style={{ maxWidth: "700px" }}>
                            Atendimentos pensados para promover acolhimento,
                            equilíbrio emocional e qualidade de vida.
                        </p>

                    </div>

                    {/* Cards */}
                    <div className="row">

                        {servicosHome.length === 0 ? (
                            <div className="col-12 text-center">
                                <p className="text-muted">Nenhum serviço disponível no momento.</p>
                            </div>
                        ) : (

                            servicosHome.map(item => (

                                <div className="col-xl-4 col-md-6 mb-4" key={item.id}>
                                    <div className="ps-service-card h-100">
                                        {/* Imagem */}
                                        <div className="ps-service-image position-relative">
                                            <img src={item.imagemCapa} alt={item.nome} className="img-fluid w-100" />

                                            {/* Modalidades */}
                                            <div className="ps-service-badges">
                                                {item.online && (
                                                    <span className="badge badge-primary mr-2"> <i className="fas fa-video mr-1"></i> Online </span>
                                                )}

                                                {item.presencial && (
                                                    <span className="badge badge-success"><i className="fas fa-map-marker-alt mr-1"></i>Presencial</span>
                                                )}
                                            </div>
                                        </div>

                                        {/* Conteúdo */}
                                        <div className="ps-service-content d-flex flex-column">
                                            <h4 className="ps-service-title">{item.nome}</h4>
                                            <p className="ps-service-description flex-grow-1">{item.descricaoCurta}</p>
                                            {/* Rodapé */}
                                            <div className="d-flex justify-content-between align-items-center mt-3">
                                                <span className="text-muted small"><i className="far fa-clock mr-1"></i>{item.tempoSessaoMinutos} min</span>
                                                {/* <a href={`/servico/${item.url}`} className="ps-link">Ver detalhes</a> */}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>

                    {/* Botão */}
                    {/* {servicos.length > 3 && ( */}
                    <div className="text-center mt-4">
                        <a href="/Servico" className="ps-button" > Ver todos os serviços </a>
                    </div>
                    {/* )} */}

                </div>
            </section>
        );
    }

    renderConvenios = () => {
        const { aguarde } = this.state;

        return (
            <section class="ps-convenios">
                <div class="container text-center">

                    <span class="ps-tag">
                        Convênios
                    </span>

                    <h2 class="ps-title-center">
                        Atendimento por convênios
                    </h2>

                    <p class="ps-text-center">
                        Consulte disponibilidade para atendimento através dos convênios abaixo.
                    </p>

                    <div class="convenios-lista">

                        {aguarde ?
                            <div className="text-center w-100">
                                <LoadingIndicator />
                            </div>
                            :
                            this.state.convenios.length == 0 ? <div className="text-center w-100">
                                <p className="ps-empty-convenio">
                                    Nenhum convênio disponível no momento.
                                </p>
                            </div>
                                :
                                this.state.convenios.map(item => {
                                    return (
                                        <span class="convenio-item" key={item.id}>
                                            <i className={item.icon || "fas fa-check-circle"}></i>
                                            {item.nome}
                                        </span>
                                    );
                                })}

                    </div>
                </div>
            </section>
        )
    };

    renderBlogs = (blogs) => {
        const { aguarde } = this.state;

        return (
            <section className="ps-section ps-blog-section">
                <div className="container">
                    <div className="text-center mb-5">
                        <span className="ps-tag">Blog</span>
                        <h2 className="ps-title-center">
                            Conteúdos sobre saúde emocional
                        </h2>
                        <p className="ps-text-center">
                            Artigos e conteúdos para ajudar no seu processo de
                            autoconhecimento e bem-estar.
                        </p>
                    </div>

                    <div className="row">
                        {aguarde ? (
                            <div className="text-center w-100">
                                <LoadingIndicator />
                            </div>
                        ) : blogs.length === 0 ? (
                            <div className="text-center w-100">
                                <p className="ps-empty-blog">
                                    Nenhum artigo publicado no momento.
                                </p>
                            </div>
                        ) : (
                            blogs.map((item) => (
                                <div className="col-lg-4 mb-4" key={item.id}>
                                    <div className="ps-blog-card">
                                        <div className="ps-blog-image">
                                            <img
                                                src={item.imagemCapa}
                                                className="img-fluid"
                                                alt={item.titulo}
                                            />
                                        </div>

                                        <div className="ps-blog-content">
                                            <h4>{item.titulo}</h4>
                                            <p>{item.resumo}</p>

                                            <div className="ps-blog-footer">
                                                <span>{item.autor}</span>
                                                <a href={`/Blog/Post/${item.url}`}>
                                                    Ler artigo →
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            ))
                        )}
                    </div>

                    {!aguarde && blogs.length > 0 && (
                        // <div className="text-center mt-5">
                        //     <a
                        //         href="/blog"
                        //         className="ps-button-outline"
                        //     >
                        //         Ver todos os artigos
                        //     </a>
                        // </div>
                        <div className="text-center mt-4">
                            <a href="/blog" className="ps-button" > Ver todos os artigos </a>
                        </div>
                    )}
                </div>
            </section>
        );
    };

    renderContato = () => {
        const {
            nome,
            email,
            assunto,
            mensagem,
            enviandoEmail
        } = this.state;

        return (
            <section id="emailContato" className="ps-section bg-white">
                <div className="container">
                    <div className="row justify-content-center">
                        <div className="col-lg-8">
                            <div className="text-center mb-5">
                                <span className="ps-tag">Contato</span>
                                <h2 className="ps-title-center">
                                    Entre em contato
                                </h2>
                                <p className="ps-text-center">
                                    Será um prazer conversar com você.
                                </p>
                            </div>

                            <div className="ps-contact-box">
                                <div className="row">
                                    <div className="col-md-6 form-group">
                                        <input
                                            type="text"
                                            className="form-control ps-input"
                                            placeholder="Seu nome"
                                            value={nome}
                                            onChange={(e) =>
                                                this.setState({
                                                    nome: e.target.value
                                                })
                                            }
                                        />
                                    </div>

                                    <div className="col-md-6 form-group">
                                        <input
                                            type="email"
                                            className="form-control ps-input"
                                            placeholder="Seu e-mail"
                                            value={email}
                                            onChange={(e) =>
                                                this.setState({
                                                    email: e.target.value
                                                })
                                            }
                                        />
                                    </div>
                                </div>

                                <div className="form-group">
                                    <input
                                        type="text"
                                        className="form-control ps-input"
                                        placeholder="Assunto"
                                        value={assunto}
                                        onChange={(e) =>
                                            this.setState({
                                                assunto: e.target.value
                                            })
                                        }
                                    />
                                </div>

                                <div className="form-group">
                                    <textarea
                                        rows="6"
                                        className="form-control ps-input"
                                        placeholder="Escreva sua mensagem..."
                                        value={mensagem}
                                        onChange={(e) =>
                                            this.setState({
                                                mensagem: e.target.value
                                            })
                                        }
                                    />
                                </div>

                                <div className="text-center mt-4">
                                    {enviandoEmail ? (
                                        <LoadingIndicator />
                                    ) : (
                                        <button
                                            className="ps-button"
                                            onClick={this.enviarEmail}
                                        >
                                            Enviar mensagem
                                        </button>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        );
    };

    render() {
        const { aguarde, blogs, servicos, convenios } = this.state;
        return (
            <div>
                {/* ───────────────── SOBRE ───────────────── */}
                {this.renderSobre()}

                {/* ───────────────── SERVIÇOS ───────────────── */}
                {servicos.length > 0 && this.renderServicos(servicos)}

                {/* ───────────────── CONVÊNIOS ───────────────── */}
                {convenios.length > 0 && this.renderConvenios()}

                {/* ───────────────── BLOG ───────────────── */}
                {this.renderBlogs(blogs)}

                {/* ───────────────── CONTATO ───────────────── */}
                {this.renderContato()}
            </div>
        );
    }
}

createRoot(document.getElementById('root')).render(<React.StrictMode><Index /></React.StrictMode>);