import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Cadastro from './components/Cadastro';
import LoadingIndicator from '../../components/LoadingIndicator';
import ApresentarImagem from '../../components/ApresentarImagem';
import Paginacao from '../../components/Paginacao';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            legendaResultadoPesquisa: "",
            cadastroModal: false,
            blogIdSelecionado: "",
            paginacao: { totalPaginas: 0 }
        };
    }

    componentDidMount = () => {
        this.pesquisar(true)
            .finally(() => this.setState({ iniciando: false }));
    }

    pesquisar = (pagina = -1) => {

        let uri = `Administrativo/BlogPost/Pesquisar?q=${encodeURIComponent(this.state.pesquisar)}&pagina=${pagina}`;

        this.setState({ aguarde: true });

        return HTTPClient.get(uri)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    resultadoPesquisa: r.data.blogs,
                    paginacao: r.data.paginacao,
                    legendaResultadoPesquisa: `${r.data.paginacao.totalItens} post(s)`
                });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao buscar posts." });
            })
            .finally(() => this.setState({ aguarde: false }));
    }

    novoPost = () => {
        window.location.href = `/Administrativo/BlogPost/NovoPost`;
    }

    // editar = (item) => {
    //     this.setState({
    //         blogIdSelecionado: item.id
    //     });

    //     alert("Em breve..." + item.id);
    // }

    editar = (item) => {
        window.location.href = `/Administrativo/BlogPost/NovoPost?id=${item.id}`;
    }

    renderCard = (post) => {
    return (
        <div key={post.id} className="col-xl-4 col-lg-6 col-md-6 mb-4">
            <div className="card h-100 border-0 shadow-sm  blog-card">

                {/* Imagem */}
                <div className="position-relative">
                    <ApresentarImagem
                        src={post.imagemCapa}
                        width="100%"
                        height="220px"
                        alt={post.titulo}
                    />

                    <span
                        className={`badge position-absolute m-3 p-2 ${
                            post.ativo
                                ? "badge-success"
                                : "badge-danger"
                        }`}
                        style={{ top: 0, right: 0 }}
                    >
                        <i
                            className={`fas ${
                                post.ativo
                                    ? "fa-check-circle"
                                    : "fa-times-circle"
                            } mr-1`}
                        ></i>

                        {post.ativo ? "Ativo" : "Desativado"}
                    </span>
                </div>

                {/* Conteúdo */}
                <div className="card-body d-flex flex-column">

                    {/* Título */}
                    <h5
                        className="card-title font-weight-bold mb-2"
                        style={{
                            minHeight: "48px",
                            lineHeight: "1.4"
                        }}
                    >
                        {post.titulo}
                    </h5>

                    {/* Resumo */}
                    <p
                        className="card-text text-muted flex-grow-1"
                        style={{
                            minHeight: "70px",
                            lineHeight: "1.5"
                        }}
                    >
                        {post.resumo || "Sem descrição..."}
                    </p>

                    {/* Informações */}
                    <div className="border-top pt-3 mt-2">

                        <div className="d-flex justify-content-between flex-wrap">

                            <small className="text-muted mb-2">
                                <i className="fas fa-user mr-1"></i>
                                <strong>{post.autor}</strong>
                            </small>

                            <small className="text-muted mb-2">
                                <i className="far fa-calendar-alt mr-1"></i>
                                {formatarDataInputDateToPtBr(post.dataPublicacao)}
                            </small>

                        </div>

                        {/* Ações */}
                        <div className="d-flex justify-content-between align-items-center mt-3">

                            <button
                                className="btn btn-sm btn-primary px-3"
                                onClick={() => this.editar(post)}
                            >
                                <i className="fas fa-edit mr-1"></i>
                                Editar
                            </button>

                            <div className="dropdown">
                                <a
                                    className="btn btn-light btn-sm"
                                    href="#"
                                    role="button"
                                    data-toggle="dropdown"
                                >
                                    <i className="fas fa-ellipsis-v"></i>
                                </a>

                                <div className="dropdown-menu dropdown-menu-right shadow border-0">

                                    <a
                                        className="dropdown-item"
                                        href="#!"
                                        onClick={() => this.visualizar(post)}
                                    >
                                        <i className="fas fa-eye mr-2 text-info"></i>
                                        Visualizar
                                    </a>

                                    <a
                                        className="dropdown-item"
                                        href="#!"
                                        onClick={() => this.alterarStatus(post)}
                                    >
                                        <i
                                            className={`fas mr-2 ${
                                                post.ativo
                                                    ? "fa-toggle-on text-success"
                                                    : "fa-toggle-off text-secondary"
                                            }`}
                                        ></i>

                                        {post.ativo
                                            ? "Desativar"
                                            : "Ativar"}
                                    </a>

                                    <div className="dropdown-divider"></div>

                                    <a
                                        className="dropdown-item text-danger"
                                        href="#!"
                                        onClick={() => this.excluir(post)}
                                    >
                                        <i className="far fa-trash-alt mr-2"></i>
                                        Excluir
                                    </a>

                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

    render() {

        return (
            <div className="container-fluid">

                <div className="row align-items-center">
                    <div className="col">
                        <div className="input-group">
                            <input
                                type="text"
                                className="form-control"
                                placeholder="Pesquisar posts..."
                                onChange={(e) => this.setState({ pesquisar: e.target.value })}
                            />
                            <button
                                className="btn btn-outline-secondary"
                                onClick={() => this.pesquisar()}
                            >
                                Buscar
                            </button>
                        </div>
                    </div>

                    <div className="col-auto">
                        <button
                            className="btn btn-primary"
                            onClick={this.novoPost}
                        >
                            + Novo Post
                        </button>
                    </div>
                </div>

                <div className="row align-items-center mt-3 mb-2">
                    <div className="col text-muted">
                        {this.state.legendaResultadoPesquisa}
                    </div>
                </div>

                <div className="row">
                    {/* LISTAGEM */}
                    <div className="row">

                        {this.state.aguarde ?
                            <LoadingIndicator />
                            :
                            this.state.resultadoPesquisa.length === 0 ?
                                <div className="col-12 text-center text-muted">
                                    Nenhum post encontrado
                                </div>
                                :
                                this.state.resultadoPesquisa.map(this.renderCard)
                        }

                    </div>

                    {/* MODAL */}
                    {this.state.cadastroModal &&
                        <Cadastro
                            onFechar={() => this.setState({ cadastroModal: false })}
                            idEdicao={this.state.blogIdSelecionado}
                        />
                    }
                </div>

                <Paginacao
                    paginacao={this.state.paginacao}
                    onPageChange={this.pesquisar}
                />

            </div>
        );
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));