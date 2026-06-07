import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Cadastro from './components/Cadastro';
import LoadingIndicator from '../../components/LoadingIndicator';
import Paginacao from '../../components/Paginacao';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            logSelecionado: [],
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            legendaResultadoPesquisa: "Últimos registros",
            cadastroModal: false,
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


    // pesquisar = (pagina = -1) => {

    //     let uri = ""

    //     // if (ultimos === true || this.state.pesquisar == "")
    //     // {
    //     //     uri = "Administrativo/LogAplicacao/Pesquisar?q=ultimos";
    //     // }
    //     // else
    //     // {
    //     //     if (this.state.pesquisar == "")
    //     //     {
    //     //         return;
    //     //     }

    //     uri = "Administrativo/LogAplicacao/Pesquisar?q=" + encodeURIComponent(this.state.pesquisar) +
    //         "&pagina=" + pagina +
    //         "&ordenacao=" + this.state.paginacao.ordenacao;
    //     // }

    //     this.setState({
    //         aguarde: true
    //     });

    //     let p = HTTPClient.get(uri)
    //         .then(r => {
    //             return r.json();
    //         })
    //         .then(r => {
    //             this.setState({
    //                 resultadoPesquisa: r.data.ucs,
    //                 legendaResultadoPesquisa: (ultimos ? "Últimos registros" : r.data.length + " registro(s) ")
    //             });
    //         })
    //         .catch((e) => {
    //             showToastr({
    //                 type: "error",
    //                 text: "Um erro ocorreu."
    //             });
    //         })
    //         .finally(() => {

    //             this.setState({
    //                 aguarde: false
    //             });

    //         });

    //     return p;
    // }

    pesquisar = (pagina = -1) => {

        let uri = "Administrativo/LogAplicacao/Pesquisar?q=" +
            encodeURIComponent(this.state.pesquisar) +
            "&pagina=" + pagina +
            "&ordenacao=" + this.state.paginacao.ordenacao;

        this.setState({
            aguarde: true
        });

        return HTTPClient.get(uri)
            .then(r => r.json())
            .then(r => {

                this.setState({
                    paginacao: r.data.paginacao,
                    resultadoPesquisa: r.data.logs,
                    legendaResultadoPesquisa:
                        r.data.paginacao.totalItens + " registro(s)"
                });

            })
            .catch(() => {

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
    }

    // obter = (id) => {

    //     return HTTPClient.get(`Administrativo/Prontuario/ObterProntuarioPorPacienteId?pacienteId=${id}`)
    //         .then(r => r.json())
    //         .then(r => {

    //             const prontuarioId = r.data.id;

    //             this.setState({
    //                 paciente: {
    //                     id: r.data.pessoaId,
    //                     nome: r.data.paciente.pessoa.nome || "",
    //                     matricula: r.data.paciente.matricula || "",
    //                     ativo: r.data.ativo,
    //                     prontuarioId: prontuarioId
    //                 }
    //             });

    //             return prontuarioId;
    //         })
    //         .catch(() => {

    //             showToastr({
    //                 type: "error",
    //                 text: "Um erro ocorreu."
    //             });

    //             return null;
    //         });
    // }

    // obterSessao = (pagina = -1) => {
    //     let uri =
    //         "Administrativo/ProntuarioAnexo/Pesquisar?q=" +
    //         encodeURIComponent(this.state.pesquisar) +
    //         "&pagina=" + pagina +
    //         "&ordenacao=" + this.state.paginacao.ordenacao;

    //     return HTTPClient.get(uri)
    //         .then(r => r.json())
    //         .then(r => {

    //             this.setState({
    //                 resultadoPesquisa: r.data.sessoes || [],
    //                 paginacao: r.data.paginacao || { totalPaginas: 0 }
    //             });

    //         })
    //         .catch(() => {
    //             showToastr({
    //                 type: "error",
    //                 text: "Um erro ocorreu."
    //             });
    //         });
    // }

    cadastroModalAbrir = () => {
        this.setState({
            cadastroModal: true,
            logSelecionado: ""
        });
    }

    cadastroModalFechar = (pessoa) => {
        this.setState({
            cadastroModal: false,
            logSelecionado: ""
        });
    }

    editar = (itemEditar) => {
        this.setState({
            cadastroModal: true,
            logSelecionado: itemEditar.id
        });
    }





    render() {

        let saida =
            <div className="row card card-secondary card-outline">
                <div className="col-12 p-3 mb-3">

                    <div className="row align-items-end mb-3">

                        {/* <div className="col-md-4">
                            <label>Paciente</label>
                            <input type="text" className="form-control" disabled value={this.state.paciente.nome} />
                        </div>

                        <div className="col-md-2">
                            <label>Matricula</label>
                            <input type="text" className="form-control" disabled value={this.state.paciente.matricula} />
                        </div> */}
                        {/* <div className="col-md-2">
                            <label>Prontuario</label>
                            <input type="text" className="form-control" disabled value={this.state.paciente.prontuarioId} />
                        </div> */}

                        <div className="col-md-4 ml-auto">
                            <label>Pesquisar Sessão</label>
                            <div className="input-group">
                                <input type="text" className="form-control" placeholder="Pesquisar" onChange={(e) => this.setState({ pesquisar: e.target.value })} />
                                <div className="input-group-append">
                                    {/* <button type="button" className="btn btn-default" data-toggle="dropdown">
                                        <i className="fas fa-filter"></i>
                                    </button> */}
                                    {/* <div className="dropdown-menu" onClick={(e) => e.stopPropagation()}>
                                        <div className="px-4 py-3 small">
                                            <div className="form-group">
                                                <label>Tipo de Atendimento</label>
                                                <select className="form-control" value={this.state.filtroTipoAtendimento}
                                                    onChange={(e) => this.setState({ ...this.state, filtroTipoAtendimento: e.target.value })}>
                                                    <option value="0"></option>
                                                    <option value="1">Documento Pessoal</option>
                                                    <option value="2">Convênio</option>
                                                    <option value="3">Termo de Consentimento</option>
                                                    <option value="4">Contrato</option>
                                                    <option value="5">Avaliação Psicológica</option>
                                                    <option value="6">Encaminhamento Médico</option>
                                                    <option value="7">Receita Médica</option>
                                                    <option value="8">Exame</option>
                                                    <option value="9">Relatório</option>
                                                    <option value="99">Outro</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div> */}

                                    <button type="button" className="btn btn-default" onClick={() => this.pesquisar()}>
                                        <i className="fas fa-search"></i>
                                    </button>
                                    {/* <button type="button" className="btn btn-primary" onClick={this.cadastroModalAbrir}>Novo</button> */}
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
                                                <th style={{ width: "20%" }}>Data Log</th>
                                                <th>Usuário</th>
                                                <th>Entidade</th>
                                                <th>Método</th>
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
                                                            <td colSpan="6" className="no-item" > Nenhum anexo foi encontrado </td>
                                                        </tr>
                                                        :
                                                        this.state.resultadoPesquisa.map(item => {
                                                            return (
                                                                <tr key={item.id} >
                                                                    <td>{formatarDataInputDateToPtBr(item.dataCriacao) || "-"}</td>
                                                                    <td>{item.usuarioNome || "-"}</td>
                                                                    <td>{item.entidade || "-"}</td>
                                                                    <td>{item.metodo || "-"}</td>
                                                                    {/* AÇÕES */}
                                                                    <td>
                                                                        <div>
                                                                            <a className="btn table-action" href="#!" role="button" data-toggle="dropdown">
                                                                                <i className="action-icon fas fa-ellipsis-v"></i>
                                                                            </a>
                                                                            <div className="dropdown-menu">
                                                                                <a className="dropdown-item" href="#!" onClick={() => this.editar(item)}><i className="fas fa-edit"></i>Editar</a>
                                                                                {/* <a className="dropdown-item" href="#!" onClick={() => this.excluir(item)}><i className="fas fa-trash"></i>Excluir</a> */}
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
                                <Paginacao paginacao={this.state.paginacao} onPageChange={this.pesquisar} />
                            </div>
                        </div>
                    </div>

                    {this.state.cadastroModal ? <Cadastro onFechar={this.cadastroModalFechar} logSelecionado={this.state.logSelecionado} sessaoSelecionada={this.state.sessaoSelecionada} /> : null}
                </div>
            </div>
        return (saida);
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));