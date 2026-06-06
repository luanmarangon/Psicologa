import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import CadastroSessao from './components/CadastroSessao';
import LoadingIndicator from '../../components/LoadingIndicator';
import Paginacao from '../../components/Paginacao';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            paciente: {
                id: 0,
                nome: "",
                matricula: "",
                ativo: false,
                prontuarioId: 0
            },
            pacienteSelecionado: [],
            prontuarioId: 0,
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            legendaResultadoPesquisa: "Últimos registros",
            cadastroModal: false,
            sessaoIdSelecionada: [],
            paginacao: { totalPaginas: 0 }
        };

    }

    componentDidMount = () => {
        const params = new URLSearchParams(window.location.search);
        const pacienteId = params.get("id");

        if (pacienteId) {
            this.obter(pacienteId)
                .then(prontuarioId => {
                    this.setState({ prontuarioId: prontuarioId });
                    this.obterSessao();
                });
        }
    }

    componentDidUpdate = () => {
        tableSelectable();
    }

    obter = (id) => {

        return HTTPClient.get(`Administrativo/Prontuario/ObterProntuarioPorPacienteId?pacienteId=${id}`)
            .then(r => r.json())
            .then(r => {

                const prontuarioId = r.data.id;

                this.setState({
                    paciente: {
                        id: r.data.pessoaId,
                        nome: r.data.paciente.pessoa.nome || "",
                        matricula: r.data.paciente.matricula || "",
                        ativo: r.data.ativo,
                        prontuarioId: prontuarioId
                    }
                });

                return prontuarioId;
            })
            .catch(() => {

                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });

                return null;
            });
    }

    obterSessao = (pagina = -1) => {
        let uri =
            "Administrativo/Prontuario/PesquisarSessao?q=" +
            encodeURIComponent(this.state.pesquisar) +
            "&protocoloId=" + this.state.paciente.prontuarioId +
            "&filtroTipoAtendimento=" + this.state.filtroTipoAtendimento +
            "&pagina=" + pagina +
            "&ordenacao=" + this.state.paginacao.ordenacao;

        return HTTPClient.get(uri)
            .then(r => r.json())
            .then(r => {

                this.setState({
                    resultadoPesquisa: r.data.sessoes || [],
                    paginacao: r.data.paginacao || { totalPaginas: 0 }
                });

            })
            .catch(() => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });
            });
    }

    cadastroModalAbrir = () => {
        this.setState({
            cadastroModal: true,
            sessaoSelecionada: null,
            sessaoIdSelecionada: ""
        });
    }

    cadastroModalFechar = (pessoa) => {
        this.setState({
            cadastroModal: false,
            sessaoIdSelecionada: ""
        });

        this.obterSessao();
    }

    editar = (itemEditar) => {
        this.setState({
            cadastroModal: true,
            sessaoSelecionada: itemEditar,
            pacienteSelecionado: this.state.paciente
        });
    }

    excluir = (itemExcluir) => {

        if (!confirm(`Confirma a exclusão da sessão de "${formatarDataInputDateToPtBr(itemExcluir.dataSessao)} das ${itemExcluir.horaInicio}"?`)) {
            return false;
        }

        HTTPClient.delete("Administrativo/Prontuario/ExcluirSessao?id=" + itemExcluir.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (r.success) {
                    this.setState({
                        resultadoPesquisa: this.state.resultadoPesquisa.filter(item => { return item.id !== itemExcluir.id }),
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
                        {/* <div className="col-md-2">
                            <label>Prontuario</label>
                            <input type="text" className="form-control" disabled value={this.state.paciente.prontuarioId} />
                        </div> */}

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
                                                <label>Tipo de Atendimento</label>
                                                <select className="form-control" value={this.state.filtroTipoAtendimento}
                                                    onChange={(e) => this.setState({ ...this.state, filtroTipoAtendimento: e.target.value })}>
                                                    <option value="0"></option>
                                                    <option value="1">Presencial</option>
                                                    <option value="2">Online</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>

                                    <button type="button" className="btn btn-default" onClick={() => this.obterSessao()}>
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
                                                <th>Atendimento</th>
                                                <th>Hora Inicio</th>
                                                <th>Hora Fim</th>
                                                <th>Psicologo</th>
                                                <th style={{ width: "50px" }}></th>
                                            </tr>
                                        </thead>

                                        <tbody>

                                            {
                                                this.state.aguarde ?
                                                    <tr> <td colSpan="6"> <LoadingIndicator timeWait={500} /> </td> </tr>
                                                    :
                                                    this.state.resultadoPesquisa.length === 0 ?
                                                        <tr>
                                                            <td colSpan="6" className="no-item" > Nenhum paciente foi encontrado </td>
                                                        </tr>
                                                        :
                                                        this.state.resultadoPesquisa.map(item => {
                                                            return (
                                                                <tr key={item.id} >
                                                                    <td>{formatarDataInputDateToPtBr(item.dataSessao) || "-"}</td>
                                                                    <td>{item.tipoAtendimentoNome || "-"}</td>
                                                                    <td>{item.horaInicio || "-"}</td>
                                                                    <td>{item.horaFim || "-"}</td>
                                                                    <td>{item.psicologaNome || "-"}</td>
                                                                    {/* AÇÕES */}
                                                                    <td>
                                                                        <div>
                                                                            <a className="btn table-action" href="#!" role="button" data-toggle="dropdown">
                                                                                <i className="action-icon fas fa-ellipsis-v"></i>
                                                                            </a>
                                                                            <div className="dropdown-menu">
                                                                                <a className="dropdown-item" href="#!" onClick={() => this.editar(item)}><i className="fas fa-edit"></i>Editar</a>
                                                                                <a className="dropdown-item" href="#!" onClick={() => this.excluir(item)}><i className="fas fa-trash"></i>Excluir</a>
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
                                <Paginacao paginacao={this.state.paginacao} onPageChange={this.obterSessao} />
                            </div>
                        </div>
                    </div>

                    {this.state.cadastroModal ? <CadastroSessao onFechar={this.cadastroModalFechar} pacienteSelecionado={this.state.paciente} sessaoSelecionada={this.state.sessaoSelecionada} /> : null}
                </div>
            </div>
        return (saida);
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));