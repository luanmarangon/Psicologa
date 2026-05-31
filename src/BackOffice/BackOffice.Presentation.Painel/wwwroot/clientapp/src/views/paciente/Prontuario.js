import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import CadastroSessao from './components/CadastroSessao';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            paciente: {
                id: 0,
                nome: "",
                matricula: "",
            },
            protocoloId: "",
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            legendaResultadoPesquisa: "Últimos registros",
            cadastroModal: false,
            pacienteIdSelecionado: "",
            paginacao: { totalPaginas: 0 }
        };

    }

    componentDidMount = () => {
        const params = new URLSearchParams(window.location.search);
        const id = params.get("id");
        this.state.protocoloId = id;
        if (id) {
            this.obter(id);
        }
        console.log(id);
    }

    componentDidUpdate = () => {
        tableSelectable();
    }

    obter = (id) => {
        return HTTPClient.get(`Administrativo/Paciente/ObterProntuario?id=${id}`)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    paciente: {
                        id: r.data.pessoaId,
                        nome: r.data.pessoaNome,
                        matricula: r.data.matricula || ""
                    }
                });
            })
            .catch(() => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });
            });
    }

    obterSessao = (id) => {
        let p = HTTPClient.get("Administrativo/Paciente/ObterSessoes?id=" + id)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    dados: {
                        ...this.state.dados,
                        ...r.data
                    }
                });
            })
            .catch(() => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });
            });
        return p;
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
            cadastroModal: true,
            pacienteIdSelecionado: this.state.protocoloId

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





    editar = (itemEditar) => {
        this.setState({
            cadastroModal: true,
            pacienteIdSelecionado: itemEditar.id
        });
    }


    render() {

        let saida =
            <div className="row card card-secondary card-outline">
                <div className="col-12 p-3 mb-3">

                    <div className="row align-items-end mb-3">

                        <div className="col-md-4">
                            <label>Paciente</label>
                            <input type="text" className="form-control" disabled value={this.state.paciente.nome} />
                        </div>

                        <div className="col-md-2">
                            <label>Matricula</label>
                            <input type="text" className="form-control" disabled value={this.state.paciente.matricula} />
                        </div>

                        <div className="col-md-4 ml-auto">
                            <label>Pesquisar Sessão</label>
                            <div className="input-group">
                                <input type="text" className="form-control" placeholder="Pesquisar" onChange={(e) => this.setState({ pesquisar: e.target.value })} />
                                <div className="input-group-append">
                                    <button type="button" className="btn btn-default" data-toggle="dropdown">
                                        <i className="fas fa-filter"></i>
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

                                    <button type="button" className="btn btn-default" onClick={() => this.pesquisar()}>
                                        <i className="fas fa-search"></i>
                                    </button>
                                    <button type="button" className="btn btn-primary" onClick={this.cadastroModalAbrir}>Novo</button>
                                </div>
                            </div>

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
                                                <th style={{ width: "20%" }}>Data Sessão</th>
                                                <th>Situação</th>
                                                <th>Psicologo</th>
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
                                {/* {!this.state.aguarde && this.montarPaginacao()} */}

                            </div>
                        </div>
                    </div>

                    {this.state.cadastroModal ? <CadastroSessao onFechar={this.cadastroModalFechar} idEdicao={this.state.pacienteIdSelecionado} /> : null}
                </div>
            </div>
        return (saida);
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));