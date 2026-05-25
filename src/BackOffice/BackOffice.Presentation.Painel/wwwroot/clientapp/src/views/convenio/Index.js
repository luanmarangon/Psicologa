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
            legendaResultadoPesquisa: "Últimos Convênios",
            cadastroModal: false,
            convenioIdSelecionado: "",
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

    pesquisar = (ultimos) => {

        let uri = ""

        uri = "Administrativo/Convenio/Pesquisar?q=" + encodeURIComponent(this.state.pesquisar);

        this.setState({
            aguarde: true
        });

        let p = HTTPClient.get(uri)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    resultadoPesquisa: r.data.convenios,
                    legendaResultadoPesquisa: (ultimos ? "Últimos registros" : r.data.length + " registro(s) ")
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

    editar = (itemEditar) => {

        this.setState({
            cadastroModal: true,
            convenioIdSelecionado: itemEditar.id
        });

    }

    cadastroModalAbrir = (item) => {

        this.setState({
            cadastroModal: true,
        });
    }

    cadastroModalFechar = (convenio) => {

        let resultadoPesquisa = [...this.state.resultadoPesquisa];

        if (convenio) {

            const index = resultadoPesquisa.findIndex(item =>
                item.id === convenio.id
            );

            // edição
            if (index > -1) {

                resultadoPesquisa[index] = convenio;

            }

            // novo
            else {

                resultadoPesquisa.unshift(convenio);

            }

        }

        this.setState({
            resultadoPesquisa,
            cadastroModal: false,
            convenioIdSelecionado: ""
        });

    }

    excluir = (itemExcluir) => {

        if (!confirm(`Confirma a exclusão de "${itemExcluir.nome}"?`)) {
            return false;
        }

        HTTPClient.delete("Administrativo/Convenio/Excluir?id=" + itemExcluir.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (r.success) {
                    this.setState({
                        resultadoPesquisa: this.state.resultadoPesquisa.filter(item => { return item.id !== itemExcluir.id }),
                        legendaResultadoPesquisa: (this.state.resultadoPesquisa.length - 1) + " registro(s)"
                    });
                    showToastr(r.messages);
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
                                <input type="text" name="table_search" className="form-control float-right" placeholder="Pesquisar por Colaboradores" onChange={(e) => this.setState({ pesquisar: e.target.value })} />
                                <div className="input-group-append">
                                    <button type="submit" className="btn btn-default" onClick={() => this.pesquisar(false)}><i className="fas fa-search"></i></button>
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
                                                <th style={{ width: "70%" }}>Nome</th>
                                                <th style={{ width: "10%" }}>Icone</th>
                                                <th style={{ width: "10%" }}>Destaque Home</th>
                                                <th style={{ width: "10%" }}>Ativo</th>
                                                <th style={{ width: "50px" }}></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {this.state.aguarde ?
                                                <tr>
                                                    <td colSpan="4">
                                                        <LoadingIndicator timeWait={500} />
                                                    </td>
                                                </tr>
                                                :
                                                this.state.resultadoPesquisa.length == 0 ?
                                                    <tr>
                                                        <td colSpan="6" className="no-item text-center">
                                                            Nenhum convênio foi encontrado.
                                                        </td>
                                                    </tr>
                                                    :
                                                    this.state.resultadoPesquisa.map(item => {
                                                        return (
                                                            <tr key={item.id}>
                                                                <td>{item.nome}</td>
                                                                <td><i className={item.icon || "fas fa-icons"}></i></td>
                                                                <td>{item.destaqueHome ? "Sim" : "Não"}</td>
                                                                <td>{item.ativo ? "Sim" : "Não"}</td>
                                                                <td>
                                                                    <div>
                                                                        <a className="btn table-action" href="#" role="button" data-toggle="dropdown">
                                                                            <i className="action-icon fas fa-ellipsis-v"></i>
                                                                        </a>
                                                                        <div className="dropdown-menu">
                                                                            <a className="dropdown-item" href="#" onClick={(e) => this.editar(item)}><i className="fas fa-user"></i>Dados de Usuário</a>
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

                    {this.state.cadastroModal ? <Cadastro onFechar={this.cadastroModalFechar} idEdicao={this.state.convenioIdSelecionado} /> : null}

                </div>
            </div>
        return (saida);
    }
}

createRoot(document.getElementById('root')).render(<React.StrictMode> <Index /> </React.StrictMode>);