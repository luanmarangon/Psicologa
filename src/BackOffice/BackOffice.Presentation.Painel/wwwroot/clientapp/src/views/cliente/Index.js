import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            legendaResultadoPesquisa: "Últimos registros",
            cadastroModal: false,
            pessoaIdSelecionado: "",
            paginacao: { totalPaginas: 0 }
        };

    }

    componentDidMount = () => {
        this.pesquisar(true)
            .finally(() => {
                this.setState({
                    iniciando: false
                });
            });
    }

    componentDidUpdate = () => {
        tableSelectable();
    }

    pesquisar = (pagina = -1) => {

        // let uri = "Administrativo/Pessoa/Pesquisar?q=" + encodeURIComponent(this.state.pesquisar) + "&tipoPessoa=" + this.state.filtroTipoPessoa
        //     + "&pagina=" + pagina + "&ordenacao=" + this.state.paginacao.ordenacao;

        let uri = "Administrativo/Cliente/Pesquisar";
        this.setState({
            aguarde: true
        });

        let p = HTTPClient.get(uri)
            .then(r => {
                return r.json();
            })
            .then(r => {
                this.setState({
                    resultadoPesquisa: [r.data],
                });
            })
            .catch((e) => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });
            })
            .finally(() => {

                this.setState({
                    aguarde: false
                });

            });

        return p;
    }

    cadastroModalAbrir = () => {

        this.setState({
            cadastroModal: true
        });
    }

    cadastroModalFechar = (pessoa) => {

        if (this.state.pessoaIdSelecionado != "" && pessoa != null) {
            let i = this.state.resultadoPesquisa.findIndex(item => {

                return item.dados.id == this.state.pessoaIdSelecionado;
            });

            if (i > -1) {

                this.state.resultadoPesquisa[i] = pessoa;
                this.setState({
                    resultadoPesquisa: this.state.resultadoPesquisa
                });
            }
        }

        this.setState({
            cadastroModal: false,
            pessoaIdSelecionado: ""
        });
    }

    excluir = (itemExcluir) => {

        if (!confirm(`Confirma a exclusão de "${itemExcluir.dados.nome}"?`)) {
            return false;
        }

        HTTPClient.delete("Administrativo/Pessoa/Excluir?id=" + itemExcluir.dados.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (r.success) {
                    this.setState({
                        resultadoPesquisa: this.state.resultadoPesquisa.filter(item => { return item.dados.id !== itemExcluir.dados.id }),
                        legendaResultadoPesquisa: (this.state.resultadoPesquisa.length - 1) + " registro(s)"
                    });
                }
                else {
                    showToastr(r.messages);
                }

            })
            .catch((e) => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });
            });
    }

    editar = (itemEditar) => {

        this.setState({
            cadastroModal: true,
            pessoaIdSelecionado: itemEditar.dados.id
        });
    }

    listarContatos = (contatos) => {
        let aux = [];

        contatos.forEach(item => {
            aux.push(<div key={item.id}>{item.contato} <span className="ml-1 small">({item.tipoNome})</span></div>);
        });

        return aux;
    }

    fraselizarPessoaTipos = (tipos) => {

        let aux = [];

        tipos.forEach(item => {
            aux.push(item.tipoNome);
        });

        return aux.join(", ");
    }

    montarPaginacao = () => {
        let totalPaginas = this.state.paginacao.totalPaginas;

        if (totalPaginas <= 1)
            return null

        let paginas = [];

        let firstDisabled = "";
        if (this.state.paginacao.paginaAtual == 0)
            firstDisabled = "disabled";
        let ant = this.state.paginacao.paginaAtual - 1;

        let lastDisabled = "";
        if (this.state.paginacao.paginaAtual == totalPaginas - 1)
            lastDisabled = "disabled";
        let prox = this.state.paginacao.paginaAtual + 1;


        let inicio = 0;
        let fim = totalPaginas;
        if (totalPaginas > 3) {
            inicio = this.state.paginacao.paginaAtual - 2;
            if (inicio < 0)
                inicio = 0;
            else if (this.state.paginacao.paginaAtual >= 3)
                inicio = this.state.paginacao.paginaAtual - 2;
            else if (this.state.paginacao.paginaAtual - inicio < 3)
                inicio = 0;

            fim = inicio + 3;
            if (fim > totalPaginas)
                fim = totalPaginas;
        }

        for (let i = inicio; i < fim; i++) {
            let active = "";

            if (this.state.paginacao.paginaAtual == i)
                active = "active";

            paginas.push(<li key={"page" + i} className={"page-item " + active}><a className="page-link" href="#" onClick={() => this.pesquisar(i)}>{i + 1}</a></li>)
        }

        let saida =

            <div className="card-footer ">

                <nav>
                    <ul className="pagination justify-content-end">

                        <li className={"page-item " + firstDisabled}>
                            {ant < 0 ?
                                <span className="page-link"><i className="fas fa-step-backward"></i></span> :
                                <a className="page-link" href="#" onClick={() => this.pesquisar(0)}><i className="fas fa-step-backward"></i></a>
                            }
                        </li>

                        <li className={"page-item " + firstDisabled}>
                            {ant < 0 ?
                                <span className="page-link"><i className="fas fa-caret-left"></i></span> :
                                <a className="page-link" href="#" onClick={() => this.pesquisar(ant)}><i className="fas fa-caret-left"></i></a>
                            }
                        </li>

                        {paginas}

                        <li className={"page-item " + lastDisabled}>
                            <a className="page-link" href="#" onClick={() => this.pesquisar(prox)}><i className="fas fa-caret-right"></i></a>
                        </li>
                        <li className={"page-item " + lastDisabled}>
                            <a className="page-link" href="#" onClick={() => this.pesquisar(this.state.paginacao.totalPaginas - 1)}><i className="fas fa-step-forward"></i></a>
                        </li>
                    </ul>
                </nav>
            </div>


        return saida;

    }

    render() {

        let saida =
            <div className="row card card-secondary card-outline">
                <div className="col-12 p-3 mb-3">

                    <div className="row justify-content-end mb-3">

                        <div className="form-group">
                            <div className="input-group col">
                                <input type="text" className="form-control float-right" placeholder="Pesquisar" onChange={(e) => this.setState({ pesquisar: e.target.value })} />


                                <div className="input-group-append">

                                    <button type="button" className="btn btn-default" role="button" data-toggle="dropdown">
                                        <i className="action-icon fas fa-filter"></i>
                                    </button>
                                    <div className="dropdown-menu" onClick={(e) => e.stopPropagation()}>
                                        <div className="px-4 py-3 small">

                                            <div className="form-group">
                                                <label>Tipo da Pessoa</label>
                                                <select className="form-control" value={this.state.filtroTipoPessoa}
                                                    onChange={(e) => this.setState({ ...this.state, filtroTipoPessoa: e.target.value })}>
                                                    <option value="0"></option>
                                                    <option value="1">Cliente</option>
                                                    <option value="2">Colaborador</option>
                                                    <option value="3">Prestador</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>

                                    <button type="button" className="btn btn-default" onClick={() => this.pesquisar()}><i className="fas fa-search"></i></button>
                                </div>
                            </div>
                        </div>

                        <div className="form-group">
                            <button type="button" className="btn btn-primary" onClick={this.cadastroModalAbrir}>Novo</button>
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-12">
                            <div className="mb-1 text-right">
                                {/* {this.state.legendaResultadoPesquisa} */}
                            </div>

                            <div className="card">
                                <div className="card-body table-responsive">
                                    <table className="table table-sm table-hover table-striped table-selectable">
                                        <thead>
                                            <tr>
                                                <th style={{ width: "40%" }}>Pessoa</th>
                                                <th>Doc. Id.</th>
                                                <th>Formas de Contato</th>
                                                <th className="text-center" style={{ width: "100px" }}>Ativo</th>
                                                <th style={{ width: "50px" }}></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {this.state.aguarde ?
                                                <tr>
                                                    <td colSpan="5">
                                                        <LoadingIndicator timeWait={500} />
                                                    </td>
                                                </tr>
                                                :
                                              
                                                    this.state.resultadoPesquisa.map(item => {
                                                        return (
                                                            <tr key={item.dados.id} className={!item.dados.ativo ? "inactive" : ""}>

                                                                <td><span className="text-uppercase">{item.dados.nome}</span>
                                                                    <div className="small text-bold">{this.fraselizarPessoaTipos(item.tipos)}</div>
                                                                </td>
                                                                <td>{item.dados.docIdNro}
                                                                    <div className="small">{item.dados.docIdTipoNome}</div>
                                                                </td>
                                                                <td>{this.listarContatos(item.contatos)}</td>
                                                                <td className="text-center">
                                                                    {item.dados.ativo ?
                                                                        <span className="text-success" title="Ativo"><i className="fas fa-check"></i></span>
                                                                        :
                                                                        <span className="text-danger" title="Inativo"><i className="fas fa-ban"></i></span>
                                                                    }
                                                                </td>
                                                                <td>
                                                                    <div>
                                                                        <a className="btn table-action" href="#" role="button" data-toggle="dropdown">
                                                                            <i className="action-icon fas fa-ellipsis-v"></i>
                                                                        </a>
                                                                        <div className="dropdown-menu">
                                                                            <a className="dropdown-item" href="#!" onClick={(e) => this.editar(item)}><i className="fas fa-edit"></i>Editar</a>
                                                                            {/* <a className="dropdown-item" href="#!" onClick={(e) => this.excluir(item)}><i className="far fa-trash-alt"></i>Excluir</a> */}
                                                                        </div>
                                                                    </div>
                                                                </td>
                                                            </tr>);
                                                    })
                                            }
                                        </tbody>
                                    </table>
                                </div>
                                {!this.state.aguarde && this.montarPaginacao()}
                            </div>
                        </div>
                    </div>

                    {this.state.cadastroModal ? <Cadastro onFechar={this.cadastroModalFechar} idEdicao={this.state.pessoaIdSelecionado} /> : null}

                </div>
            </div>
        return (saida);
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));