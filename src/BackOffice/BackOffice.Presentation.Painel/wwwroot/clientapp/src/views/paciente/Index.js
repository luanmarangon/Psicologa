import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import CadastroPaciente from './components/CadastroPaciente';
import CadastroProntuario from './components/CadastroProntuario';
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
            prontuarioModal: false,
            pacienteIdSelecionado: "",
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

        let uri = "Administrativo/Paciente/Pesquisar?q=" + encodeURIComponent(this.state.pesquisar) + "&tipoPessoa=" + this.state.filtroTipoPessoa
            + "&pagina=" + pagina + "&ordenacao=" + this.state.paginacao.ordenacao;


        this.setState({
            aguarde: true
        });

        let p = HTTPClient.get(uri)
            .then(r => {
                return r.json();
            })
            .then(r => {
                this.setState({
                    paginacao: r.data.paginacao,
                    resultadoPesquisa: r.data.pessoas,
                    legendaResultadoPesquisa: r.data.paginacao.totalItens + " registro(s) "
                });
                console.log(r);
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

        if (this.state.pacienteIdSelecionado != "" && pessoa != null) {
            let i = this.state.resultadoPesquisa.findIndex(item => {

                return item.dados.id == this.state.pacienteIdSelecionado;
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
            pacienteIdSelecionado: ""
        });
    }

    prontuarioModalAbrir = () => {

        this.setState({
            prontuarioModal: true
        });
    }
    prontuarioModalFechar = (pessoa) => {

        if (this.state.pacienteIdSelecionado != "" && pessoa != null) {
            let i = this.state.resultadoPesquisa.findIndex(item => {

                return item.dados.id == this.state.pacienteIdSelecionado;
            });

            if (i > -1) {

                this.state.resultadoPesquisa[i] = pessoa;
                this.setState({
                    resultadoPesquisa: this.state.resultadoPesquisa
                });
            }
        }

        this.setState({
            prontuarioModal: false,
            pacienteIdSelecionado: ""
        });
    }



    editar = (itemEditar) => {
        this.setState({
            cadastroModal: true,
            pacienteIdSelecionado: itemEditar.id
        });
    }

    editarProntuario = (itemEditar) => {
        this.setState({
            prontuarioModal: true,
            pacienteIdSelecionado: itemEditar.id
        });
    }


    sessao = (item) => {
        window.location.href = `/Administrativo/Paciente/Prontuario?id=${item.id}`;
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
                                                    <option value="3">Psicologo</option>
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
                                {this.state.legendaResultadoPesquisa}
                            </div>

                            <div className="card">
                                <div className="card-body table-responsive">

                                    <table className="table table-sm table-hover table-striped table-selectable">

                                        <thead>
                                            <tr>
                                                <th style={{ width: "40%" }}>Pessoa</th>
                                                <th>Matricula</th>
                                                <th>Primeira Sessão</th>
                                                <th>Responsável</th>
                                                <th className="text-center" style={{ width: "100px" }}>Ativo</th>
                                                <th style={{ width: "50px" }}></th>
                                            </tr>
                                        </thead>

                                        <tbody>

                                            {
                                                this.state.aguarde ?
                                                    <tr> <td colSpan="5"> <LoadingIndicator timeWait={500} /> </td> </tr>
                                                    :
                                                    this.state.resultadoPesquisa.length === 0 ?
                                                        <tr>
                                                            <td colSpan="5" className="no-item" > Nenhum paciente foi encontrado </td>
                                                        </tr>
                                                        :
                                                        this.state.resultadoPesquisa.map(item => {
                                                            return (
                                                                <tr key={item.id} className={!item.ativo ? "inactive" : ""} >
                                                                    {/* PACIENTE */}
                                                                    <td><span className="text-uppercase">{item.pessoaNome}</span></td>
                                                                    {/* MATRICULA */}
                                                                    <td><span className="text-uppercase">{item.matricula}</span></td>
                                                                    {/* PRIMEIRA SESSÃO */}
                                                                    <td>{formatarDataInputDateToPtBr(item.dataPrimeiraSessao) || "-"}</td>
                                                                    {/* RESPONSÁVEL */}
                                                                    <td>{item.responsavelNome || "-"}</td>
                                                                    {/* ATIVO */}
                                                                    <td className="text-center">
                                                                        {item.ativo ?
                                                                            <span className="text-success" title="Ativo" ><i className="fas fa-check"></i></span>
                                                                            :
                                                                            <span className="text-danger" title="Inativo" ><i className="fas fa-ban"></i></span>
                                                                        }
                                                                    </td>
                                                                    {/* AÇÕES */}
                                                                    <td>
                                                                        <div>
                                                                            <a className="btn table-action" href="#!" role="button" data-toggle="dropdown">
                                                                                <i className="action-icon fas fa-ellipsis-v"></i>
                                                                            </a>
                                                                            <div className="dropdown-menu">
                                                                                <a className="dropdown-item" href="#!" onClick={() => this.editar(item)}><i className="fas fa-edit"></i>Editar</a>
                                                                                <a className="dropdown-item" href="#!" onClick={() => this.editarProntuario(item)}><i className="far fa-file-alt"></i>Prontuário</a>
                                                                                <a className="dropdown-item" href="#!" onClick={() => this.sessao(item)}><i className="far fa-file-alt"></i>Sessões</a>
                                                                            </div>
                                                                        </div>
                                                                    </td>
                                                                </tr>
                                                            );
                                                        })
                                            }
                                        </tbody>
                                    </table>
                                </div>
                                {!this.state.aguarde && this.montarPaginacao()}

                            </div>
                        </div>
                    </div>

                    {this.state.cadastroModal ? <CadastroPaciente onFechar={this.cadastroModalFechar} idEdicao={this.state.pacienteIdSelecionado} /> : null}
                    {this.state.prontuarioModal ? <CadastroProntuario onFechar={this.prontuarioModalFechar} idEdicao={this.state.pacienteIdSelecionado} /> : null}
                </div>
            </div>
        return (saida);
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));