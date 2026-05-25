import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { createRoot } from 'react-dom/client';

import Cadastro from './components/Cadastro';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            pesquisar:"",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            legendaResultadoPesquisa: "Últimos usuários",
            cadastroModal: false,
            usuarioIdSelecionado: "",
            pessoaIdSelecionado: "",
            pessoaSelecionado: ""
        };
    }

    componentDidMount = () => {
        this.pesquisar(true)
        .finally(() =>{
            this.setState({
                iniciando: false
            });
        });
    }

    componentDidUpdate = () => {
        tableSelectable();
    }

    pesquisar = (ultimos) =>{
        
        let uri = ""

        if (ultimos === true || this.state.pesquisar == "")
        {
            uri = "Administrativo/Usuario/Pesquisar?q=ultimos";
        }
        else
        {
            if (this.state.pesquisar == "")
            {
                return;
            }

            uri = "Administrativo/Usuario/Pesquisar?q=" + encodeURIComponent(this.state.pesquisar);
        }

        this.setState({
            aguarde:true
        });

        let p = HTTPClient.get(uri)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    resultadoPesquisa: r.data,
                    legendaResultadoPesquisa: (ultimos ? "Últimos registros" : r.data.length + " registro(s) ")
                });

            })
            .catch((e) => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });
            })
            .finally(() =>{

                this.setState({
                    aguarde:false
                });
                
            });

        return p;
    }

    cadastroModalAbrir = (item) => {

        this.setState({
            cadastroModal: true,
            usuarioIdSelecionado: item.id,
            pessoaIdSelecionado: item.pessoaId,
            pessoaNomeSelecionado: item.pessoaNome
        });
    }

    cadastroModalFechar = (usuario) => {

        if (this.state.usuarioIdSelecionado != "" && usuario != null)
        {
            let i = this.state.resultadoPesquisa.findIndex(item => {

                return item.id == this.state.usuarioIdSelecionado;
            });

            if (i > -1)
            {
                
                this.state.resultadoPesquisa[i] = usuario;
                this.setState({
                    resultadoPesquisa: this.state.resultadoPesquisa
                });
            }
        }

        this.setState({
            cadastroModal: false,
            usuarioIdSelecionado: "",
            pessoaIdSelecionado: "",
            pessoaNomeSelecionado: ""
        });
    }

    excluir = (itemExcluir) => {
       
        if (!confirm(`Confirma a exclusão de "${itemExcluir.nome}"?`))
        {
            return false;
        }

        HTTPClient.delete("Administrativo/Usuario/Excluir?id=" + itemExcluir.id)
        .then(r => {
            return r.json();
        })
        .then(r => {

            if (r.success)
            {
                this.setState({
                    resultadoPesquisa : this.state.resultadoPesquisa.filter(item => {return item.id !== itemExcluir.id}),
                    legendaResultadoPesquisa: (this.state.resultadoPesquisa.length - 1)+ " registro(s)"
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
                                <input type="text" name="table_search" className="form-control float-right" placeholder="Pesquisar por Colaboradores" onChange={(e) => this.setState({pesquisar: e.target.value}) }  />
                                <div className="input-group-append">
                                    <button type="submit" className="btn btn-default" onClick={() => this.pesquisar(false)}><i className="fas fa-search"></i></button>
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
                                    <table className="table table-hover table-striped table-selectable">
                                        <thead>
                                            <tr>
                                                <th style={{width:"40%"}}>Colaborador</th>
                                                <th>Nome de Usuário</th>
                                                <th>Perfil</th>
                                                <th style={{width:"50px"}}></th>
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
                                                    <td colSpan="6" className="no-item">
                                                        Nenhum colaborador/usuário foi encontrado.
                                                    </td>
                                                </tr>
                                                :
                                                this.state.resultadoPesquisa.map(item =>{
                                                    return (
                                                    <tr key={item.id}>
                                                        <td>{item.pessoaNome}</td>
                                                        <td>{item.nome}
                                                            {isEmpty(item.nome) ? <span className="small text-bold">(não é usuário)</span> : ""}
                                                        </td>
                                                        <td>{item.perfilNome}</td>
                                                        <td>
                                                            <div>
                                                                <a className="btn table-action" href="#" role="button" data-toggle="dropdown">
                                                                    <i className="action-icon fas fa-ellipsis-v"></i>
                                                                </a>
                                                                <div className="dropdown-menu">
                                                                    <a className="dropdown-item" href="#" onClick={(e) => this.cadastroModalAbrir(item)}><i className="fas fa-user"></i>Dados de Usuário</a>
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
                                            
                    {this.state.cadastroModal ? <Cadastro onFechar={this.cadastroModalFechar} 
                                                          usuarioId = {this.state.usuarioIdSelecionado}
                                                          pessoaId={this.state.pessoaIdSelecionado} 
                                                          pessoaNome={this.state.pessoaNomeSelecionado} /> : null}

                 </div>
            </div>
        return (saida);
    }
}
        
createRoot(document.getElementById('root')).render(<React.StrictMode> <Index/> </React.StrictMode>);