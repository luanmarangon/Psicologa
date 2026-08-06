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
            legendaResultadoPesquisa: "Últimos lançamentos",
            cadastroModal: false,
            lancamentoIdSelecionado: "",
            filtro: "0", // 0=Todos, 1=Despesas, 2=Receitas, 3=Pendentes
            resumo: {
                totalReceitas: 0,
                totalDespesas: 0,
                saldo: 0,
            },
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

        let uri = `Administrativo/Financeiro/Pesquisar?q=${encodeURIComponent(this.state.pesquisar)}&filtro=${this.state.filtro}&pagina=${pagina}`;

        this.setState({ aguarde: true });

        return HTTPClient.get(uri)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    resultadoPesquisa: r.data.lancamentos,
                    paginacao: r.data.paginacao,
                    resumo: r.data.resumo || { totalReceitas: 0, totalDespesas: 0, saldo: 0 },
                    legendaResultadoPesquisa: `${r.data.paginacao.totalItens} lançamento(s)`
                });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao buscar lançamentos." });
            })
            .finally(() => this.setState({ aguarde: false }));
    }

    cadastroModalAbrir = (item) => {

        this.setState({
            cadastroModal: true,
        });
    }

    cadastroModalFechar = (lancamento) => {

        if (this.state.lancamentoIdSelecionado != "" && lancamento != null) {
            let i = this.state.resultadoPesquisa.findIndex(item => {

                return item.id == this.state.lancamentoIdSelecionado;
            });

            if (i > -1) {

                this.state.resultadoPesquisa[i] = lancamento;
                this.setState({
                    resultadoPesquisa: this.state.resultadoPesquisa
                });
            }
        } else if (lancamento != null) {
            // Novo lançamento — mais simples recarregar a pesquisa pra atualizar resumo/paginação
            this.pesquisar();
        }

        this.setState({
            cadastroModal: false,
            lancamentoIdSelecionado: "",
        });
    }

    editar = (itemEditar) => {

        this.setState({
            cadastroModal: true,
            lancamentoIdSelecionado: itemEditar.id
        });

    }


    excluir = (itemExcluir) => {

        if (!confirm(`Confirma a exclusão deste lançamento?`)) {
            return false;
        }

        HTTPClient.delete("Administrativo/Financeiro/Excluir?id=" + itemExcluir.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (r.success) {
                    this.pesquisar();
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

    formatarMoeda = (valor) => {
        return (Number(valor) || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
    }

    formatarData = (data) => {
        if (!data) return '';
        const [ano, mes, dia] = data.split('T')[0].split('-');
        return `${dia}/${mes}/${ano}`;
    }


    render() {

        const resumo = this.state.resumo || { totalReceitas: 0, totalDespesas: 0, saldo: 0 };

        let saida =
            <div className="row card card-secondary card-outline">
                <div className="col-12 p-3 mb-3">

                    {/* Resumo */}
                    <div className="row mb-3">
                        <div className="col-md-4 mb-2 mb-md-0">
                            <div className="card">
                                <div className="card-body py-2">
                                    <div className="small text-muted">Receitas</div>
                                    <div className="h5 mb-0 text-success">{this.formatarMoeda(resumo.totalReceitas)}</div>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4 mb-2 mb-md-0">
                            <div className="card">
                                <div className="card-body py-2">
                                    <div className="small text-muted">Despesas</div>
                                    <div className="h5 mb-0 text-danger">{this.formatarMoeda(resumo.totalDespesas)}</div>
                                </div>
                            </div>
                        </div>
                        <div className="col-md-4">
                            <div className="card">
                                <div className="card-body py-2">
                                    <div className="small text-muted">Saldo</div>
                                    <div className={`h5 mb-0 ${resumo.saldo >= 0 ? 'text-success' : 'text-danger'}`}>
                                        {this.formatarMoeda(resumo.saldo)}
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

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
                                                <label>Tipo</label>
                                                <select className="form-control" value={this.state.filtro}
                                                    onChange={(e) => this.setState({ ...this.state, filtro: e.target.value })}>
                                                    <option value="0">Todos</option>
                                                    <option value="1">Despesas</option>
                                                    <option value="2">Receitas</option>
                                                    <option value="3">Pendentes</option>
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
                                                <th style={{ width: "10%" }}>Tipo</th>
                                                <th>Categoria</th>
                                                <th>Data</th>
                                                <th>Descrição</th>
                                                <th className="text-right">Valor</th>
                                                <th>Status</th>
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
                                                            Nenhum lançamento foi encontrado.
                                                        </td>
                                                    </tr>
                                                    :
                                                    this.state.resultadoPesquisa.map(item => {
                                                        return (
                                                            <tr key={item.id}>
                                                                <td>
                                                                    {item.tipo === "receita"
                                                                        ? <span className="badge badge-success">Receita</span>
                                                                        : <span className="badge badge-danger">Despesa</span>
                                                                    }
                                                                </td>
                                                                <td>{item.categoriaNome}</td>
                                                                <td>{this.formatarData(item.dataLancamento)}</td>
                                                                <td>{item.descricao}</td>
                                                                <td className="text-right font-weight-bold">{this.formatarMoeda(item.valor)}</td>
                                                                <td>
                                                                    {item.quitado
                                                                        ? <span className="badge badge-light border">{item.tipo === "receita" ? "Recebido" : "Pago"}</span>
                                                                        : <span className="badge badge-warning">{item.tipo === "receita" ? "A receber" : "Pendente"}</span>
                                                                    }
                                                                </td>
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

                    {this.state.cadastroModal ? <Cadastro onFechar={this.cadastroModalFechar} idEdicao={this.state.lancamentoIdSelecionado} /> : null}

                </div>
            </div>
        return (saida);
    }
}

createRoot(document.getElementById('root')).render(<React.StrictMode> <Index /> </React.StrictMode>);