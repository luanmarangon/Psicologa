import React, { Component } from 'react';
import ReactDOM from 'react-dom';
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

        let uri = `Blog/ObterTodosPublicados`;

        this.setState({ aguarde: true });

        return HTTPClient.get(uri)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    resultadoPesquisa: r.data

                });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao buscar posts." });
            })
            .finally(() => this.setState({ aguarde: false }));
    }



    editar = (item) => {
        window.location.href = `/Blog/Post/${item.url}`;
    }

    renderCard = (post) => {

        return (

            <div key={post.id} className="col-xl-4 col-lg-6 col-md-6 mb-4">
                <div className="ps-blog-card h-100">
                    {/* Imagem */}
                    <div className="ps-blog-image position-relative">
                        <ApresentarImagem
                            src={post.imagemCapa}
                            width="100%"
                            height="240px"
                            alt={post.titulo}
                        />

                        {/* <span className={`badge position-absolute m-3 px-3 py-2 ${post.ativo ? "badge-success": "badge-secondary"}`}
                            style={{
                                top: 0,
                                right: 0,
                                borderRadius: 30,
                                fontWeight: 500
                            }}
                        >
                            <i
                                className={`fas mr-1 ${post.ativo
                                    ? "fa-check-circle"
                                    : "fa-times-circle"
                                    }`}
                            />

                            {post.ativo
                                ? "Publicado"
                                : "Rascunho"}
                        </span> */}

                    </div>

                    {/* Conteúdo */}
                    <div className="ps-blog-content d-flex flex-column">
                        {/* Título */}
                        <h4 style={{ minHeight: 58, lineHeight: 1.4 }}>{post.titulo}</h4>

                        {/* Resumo */}
                        <p style={{ minHeight: 85 }}>{post.resumo || "Sem resumo disponível."}</p>

                        {/* Footer */}
                        <div className="ps-blog-footer mt-auto">
                            <div className="d-flex justify-content-between align-items-center flex-wrap mb-3">
                                <span style={{ fontSize: 13, color: '#64748b' }} className="mr-2"><i className="fas fa-user mr-2"></i>{post.autor}</span>
                                <span style={{ fontSize: 13, color: '#64748b' }}><i className="far fa-calendar-alt mr-2"></i>{formatarDataInputDateToPtBr(post.dataPublicacao)}</span>
                            </div>

                            {/* Ações */}
                            <div className="d-flex justify-content-between align-items-center">
                                <button className="btn btn-primary px-4" style={{ borderRadius: 10, fontWeight: 600 }} onClick={() => this.editar(post)}>
                                    <i className="fas fa-eye mr-2 text-ligth"></i> Visualizar
                                </button>

                                {/* <div className="dropdown">

                                <button
                                    className="btn btn-light"
                                    data-toggle="dropdown"
                                    style={{
                                        width: 42,
                                        height: 42,
                                        borderRadius: 10
                                    }}
                                >
                                    <i className="fas fa-ellipsis-h"></i>
                                </button>

                                <div className="dropdown-menu dropdown-menu-right border-0 shadow">

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
                                            className={`fas mr-2 ${post.ativo
                                                ? "fa-toggle-on text-success"
                                                : "fa-toggle-off text-secondary"
                                                }`}
                                        />

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

                            </div> */}

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

                {/* <div className="row align-items-center">
                    <div className="col">
                        <div className="input-group">
                            <input type="text" className="form-control" placeholder="Pesquisar posts..." onChange={(e) => this.setState({ pesquisar: e.target.value })} />
                            <button className="btn btn-outline-secondary" onClick={() => this.pesquisar()}>Buscar</button>
                        </div>
                    </div>
                </div> */}

                <div className="row">
                    {/* LISTAGEM */}
                    <div className="row">
                        {this.state.aguarde ? <LoadingIndicator /> :
                            this.state.resultadoPesquisa.length === 0 ?
                                <div className="col-12 text-center text-muted">Nenhum post encontrado</div> :
                                this.state.resultadoPesquisa.map(this.renderCard)
                        }

                    </div>


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