import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { createRoot } from 'react-dom/client';

import Permissao from './components/Permissao';
import Cadastro from './components/Cadastro';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            permissoesModal: false,
            perfilIdSelecionado: "",
            perfilNomeSelecionado: "",
            cadastroModal: false,
        };
    }

    componentDidMount = () => {
        this.obterPerfis()
            .finally(() => {
                this.setState({
                    iniciando: false
                });
            });
    }

    componentDidUpdate = () => {
        tableSelectable();
    }

    obterPerfis = () => {

        let uri = "Administrativo/PerfilUsuario/ObterPerfis";

        this.setState({
            aguarde: true
        });

        let p = HTTPClient.get(uri)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    resultadoPesquisa: r.data
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

    permissoesModalAbrir = (item) => {

        this.setState({
            permissoesModal: true,
            perfilIdSelecionado: item.id,
            perfilNomeSelecionado: item.nome
        });
    }

    permissoesModalFechar = (perfil) => {

        if (this.state.perfilIdSelecionado != "" && perfil != null) {
            let i = this.state.resultadoPesquisa.findIndex(item => {
                return item.id == this.state.perfilIdSelecionado;
            });
            if (i > -1) {
                this.state.resultadoPesquisa[i] = perfil;
                this.setState({
                    resultadoPesquisa: this.state.resultadoPesquisa
                });
            }
        }
        this.setState({
            permissoesModal: false,
            perfilIdSelecionado: "",
            perfilNomeSelecionado: ""
        });
    }

    cadastroModalAbrir = (item) => {
        this.setState({
            cadastroModal: true,
            perfilIdSelecionado: item.id,
            perfilNomeSelecionado: item.nome
        });
    }
    cadastroModalFechar = () => {
        this.obterPerfis();
        this.setState({
            cadastroModal: false,
            perfilIdSelecionado: "",
            perfilNomeSelecionado: ""
        });
    }

    fraselizarPermissoes = (permissoes) => {

        if (isEmpty(permissoes))
            return "";

        let aux = [];

        permissoes.forEach(item => {
            aux.push(item);
        });

        return aux.join(", ");
    }



    render() {

        let saida =
            <div className="row card card-secondary card-outline">
                <div className="col-12 p-3 mb-3">
                    {/* <div className="row">
                        <div className="col text-right mb-3">
                            <button className="btn btn-primary" onClick={this.cadastroModalAbrir}><i className="fas fa-plus"></i> Cadastrar Perfil</button>
                        </div>
                    </div> */}
                    <div className="row">
                        <div className="col-12">

                            <div className="card">
                                <div className="card-body table-responsive">
                                    <table className="table table-hover table-striped table-selectable">
                                        <thead>
                                            <tr>
                                                <th>Perfil</th>
                                                <th>Permissões</th>
                                                <th style={{ width: "50px" }}></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {this.state.aguarde ?
                                                <tr>
                                                    <td colSpan="3">
                                                        <LoadingIndicator timeWait={500} />
                                                    </td>
                                                </tr>
                                                :
                                                this.state.resultadoPesquisa.length == 0 ?
                                                    <tr>
                                                        <td colSpan="3" className="no-item">
                                                            Nenhum item encontrado.
                                                        </td>
                                                    </tr>
                                                    :
                                                    this.state.resultadoPesquisa.map(item => {
                                                        return (
                                                            <tr key={item.id}>
                                                                <td>{item.nome}</td>
                                                                <td>{this.fraselizarPermissoes(item.permissoesNomes)}</td>
                                                                <td>
                                                                    <div>
                                                                        <a className="btn table-action" href="#" role="button" data-toggle="dropdown">
                                                                            <i className="action-icon fas fa-ellipsis-v"></i>
                                                                        </a>
                                                                        <div className="dropdown-menu">
                                                                            <a className="dropdown-item" href="#!" onClick={(e) => this.permissoesModalAbrir(item)}><i className="fas fa-list"></i>Permissões</a>
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

                    {this.state.permissoesModal ? <Permissao onFechar={this.permissoesModalFechar}
                        perfilId={this.state.perfilIdSelecionado}
                        perfilNome={this.state.perfilNomeSelecionado} /> : null}

                    {this.state.cadastroModal ? <Cadastro onFechar={this.cadastroModalFechar}
                        perfilId={this.state.perfilIdSelecionado}
                        perfilNome={this.state.perfilNomeSelecionado} /> : null}

                </div>
            </div>
        return (saida);
    }
}

createRoot(document.getElementById('root')).render(<React.StrictMode> <Index /> </React.StrictMode>);
