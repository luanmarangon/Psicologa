import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { createRoot } from 'react-dom/client';

import Cadastro from './components/Cadastro';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            legendaResultadoPesquisa: "Últimos serviços",
            cadastroModal: false,
            servicoIdSelecionado: "",
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

        let uri = `Administrativo/Servico/Pesquisar?q=${encodeURIComponent(this.state.pesquisar)}&filtro=${this.state.filtro}&pagina=${pagina}`;

        this.setState({ aguarde: true });

        return HTTPClient.get(uri)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    resultadoPesquisa: r.data.servicos,
                    paginacao: r.data.paginacao,
                    legendaResultadoPesquisa: `${r.data.paginacao.totalItens} serviço(s)`
                });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao buscar serviços." });
            })
            .finally(() => this.setState({ aguarde: false }));
    }

    cadastroModalAbrir = (item) => {

        this.setState({
            cadastroModal: true,
        });
    }

    cadastroModalFechar = (servico) => {

        if (this.state.servicoIdSelecionado != "" && servico != null) {
            let i = this.state.resultadoPesquisa.findIndex(item => {

                return item.id == this.state.servicoIdSelecionado;
            });

            if (i > -1) {

                this.state.resultadoPesquisa[i] = servico;
                this.setState({
                    resultadoPesquisa: this.state.resultadoPesquisa
                });
            }
        }

        this.setState({
            cadastroModal: false,
            servicoIdSelecionado: "",
        });
    }

    editar = (itemEditar) => {

        this.setState({
            cadastroModal: true,
            servicoIdSelecionado: itemEditar.id
        });

    }


    excluir = (itemExcluir) => {

        if (!confirm(`Confirma a exclusão de "${itemExcluir.nome}"?`)) {
            return false;
        }

        HTTPClient.delete("Administrativo/Servico/Excluir?id=" + itemExcluir.id)
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
                                                <select className="form-control" value={this.state.filtro}
                                                    onChange={(e) => this.setState({ ...this.state, filtro: e.target.value })}>
                                                    <option value="0"></option>
                                                    <option value="1">Destaques</option>
                                                    <option value="2">Presencial</option>
                                                    <option value="3">Online</option>
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
                                    <table className="table table-hover table-striped table-selectable">
                                        <thead>
                                            <tr>
                                                <th>
                                                    <i className="fas fa-check text-success"></i>
                                                    /
                                                    <i className="fas fa-times text-danger"></i>

                                                </th>
                                                <th style={{ width: "40%" }}>Serviço</th>
                                                <th>Valor Sessão</th>
                                                <th>Tempo Sessão</th>
                                                <th>Online</th>
                                                <th>Presencial</th>
                                                <th style={{ width: "50px" }}></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {this.state.aguarde ?
                                                <tr>
                                                    <td colSpan="7">
                                                        <LoadingIndicator timeWait={500} />
                                                    </td>
                                                </tr>
                                                :
                                                this.state.resultadoPesquisa.length == 0 ?
                                                    <tr>
                                                        <td colSpan="7" className="no-item text-center">
                                                            Nenhum serviço foi encontrado.
                                                        </td>
                                                    </tr>
                                                    :
                                                    this.state.resultadoPesquisa.map(item => {
                                                        return (
                                                            <tr key={item.id}>
                                                                {/* <td>{item.ativo ? "Sim" : "Não"}</td> */}
                                                                <td>
                                                                    {item.ativo ? (
                                                                        <i className="fas fa-check text-success"></i>
                                                                    ) : (
                                                                        <i className="fas fa-times text-danger"></i>
                                                                    )}
                                                                </td>
                                                                <td>{item.nome}</td>
                                                                <td>{item.valorSessao}</td>
                                                                <td>{item.tempoSessaoMinutos}</td>
                                                                <td>{item.online ? "Sim" : "Não"}</td>
                                                                <td>{item.presencial ? "Sim" : "Não"}</td>
                                                                <td>
                                                                    <div>
                                                                        <a className="btn table-action" href="#" role="button" data-toggle="dropdown">
                                                                            <i className="action-icon fas fa-ellipsis-v"></i>
                                                                        </a>
                                                                        <div className="dropdown-menu">
                                                                            <a className="dropdown-item" href="#" onClick={(e) => this.editar(item)}><i className="fas fa-edit"></i>Editar</a>
                                                                            <a className="dropdown-item" href="#" onClick={(e) => this.excluir(item)}><i className="far fa-trash-alt"></i>Excluir</a>
                                                                        </div>

                                                                    </div>
                                                                </td>
                                                            </tr>);
                                                    })
                                            }
                                        </tbody>
                                    </table>
                                </div>

                            </div>
                        </div>
                    </div>

                    {this.state.cadastroModal ? <Cadastro onFechar={this.cadastroModalFechar} idEdicao={this.state.servicoIdSelecionado} /> : null}

                </div>
            </div>
        return (saida);
    }
}

createRoot(document.getElementById('root')).render(<React.StrictMode> <Index /> </React.StrictMode>);