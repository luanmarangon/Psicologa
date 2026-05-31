import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { createRoot } from 'react-dom/client';

import Cadastro from './components/Cadastro';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            aguarde: false,
            pesquisar: '',
            filtro: '0',
            legendaResultadoPesquisa: '0 agendamento(s)',
            cadastroModal: false,
            agendamentoIdSelecionado: null,
            resultadoPesquisa: []
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

        let uri = `Administrativo/Agendamento/Pesquisar?q=${encodeURIComponent(this.state.pesquisar)}&filtro=${this.state.filtro}&pagina=${pagina}`;

        this.setState({ aguarde: true });

        return HTTPClient.get(uri)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    resultadoPesquisa: r.data.agendamentos,
                    paginacao: r.data.paginacao,
                    legendaResultadoPesquisa: `${r.data.paginacao.totalItens} agendamento(s)`
                });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao buscar agendamentos." });
            })
            .finally(() => this.setState({ aguarde: false }));
    }

    cadastroModalAbrir = (item) => {

        this.setState({
            cadastroModal: true,
        });
    }

    cadastroModalFechar = (lead) => {
        this.pesquisar();
        this.setState({
            cadastroModal: false,
            agendamentoIdSelecionado: "",
        });
    }

    editar = (itemEditar) => {

        this.setState({
            cadastroModal: true,
            agendamentoIdSelecionado: itemEditar.id
        });
    }

    excluir = (itemExcluir) => {

        if (!confirm(`Confirma a exclusão de "${itemExcluir.nome}"?`)) {
            return false;
        }

        HTTPClient.delete("Administrativo/Agendamento/Excluir?id=" + itemExcluir.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (r.success) {
                    this.setState({
                        resultadoPesquisa: this.state.resultadoPesquisa.filter(item => { return item.id !== itemExcluir.id }),
                        legendaResultadoPesquisa: (this.state.resultadoPesquisa.length - 1) + " agendamento(s)"
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

    renderAgendamento = (item) => {
        return (
            <div className="col-md-6 col-lg-4 mb-4" key={item.id}>
                <div className="card h-100 shadow-sm">

                    {/* Header */}
                    <div className={`card-header d-flex justify-content-between align-items-center ${item.online ? 'bg-primary' : 'bg-success'} text-white`}>

                        <span>
                            <i className={`fas ${item.online ? 'fa-video' : 'fa-user'} mr-2`}></i>
                            {item.online ? 'Online' : 'Presencial'}
                        </span>

                        <span className={`badge ${item.ativo ? 'badge-light' : 'badge-secondary'} ml-auto`}>
                            {item.ativo ? '✔ Ativo' : 'Inativo'}
                        </span>

                    </div>

                    <div className="card-body">

                        <h5 className="card-title font-weight-bold mb-1">{item.pacienteNome}</h5>
                        <p className="text-muted small mb-3">
                            <i className="fas fa-id-card mr-1"></i>{item.documento}
                        </p>

                        <hr />

                        <div className="d-flex flex-column" style={{ gap: '8px' }}>

                            <div className="d-flex align-items-center">
                                <i className="fas fa-calendar-alt text-primary mr-2" style={{ width: '16px' }}></i>
                                <span className="small">
                                    <strong>Consulta:</strong> {item.dataConsulta} às {item.horaInicio}
                                </span>
                            </div>

                            <div className="d-flex align-items-center">
                                <i className="fas fa-clock text-warning mr-2" style={{ width: '16px' }}></i>
                                <span className="small">
                                    <strong>Duração:</strong> {item.tempoSessao} min
                                </span>
                            </div>

                            <div className="d-flex align-items-center">
                                <i className="fas fa-briefcase-medical text-info mr-2" style={{ width: '16px' }}></i>
                                <span className="small">
                                    <strong>Serviço:</strong> {item.servicoNome}
                                </span>
                            </div>
                            <div className="d-flex align-items-center">
                                <i className="fas fa-user-md text-success mr-2" style={{ width: '16px' }}></i>
                                <span className="small">
                                    <strong>Psicólogo(a):</strong> {item.psicologoNome}
                                </span>
                            </div>

                            <div className="d-flex align-items-center">
                                <i className="fas fa-calendar-check text-secondary mr-2" style={{ width: '16px' }}></i>
                                <span className="small">
                                    <strong>Agendado em:</strong> {formatarDataInputDateToPtBr(item.dataCriacao)}
                                    <span className="mr-2"></span>
                                    <strong>Atualizado em:</strong> {formatarDataInputDateToPtBr(item.dataAtualizacao)}
                                </span>
                            </div>
                            <div className="d-flex align-items-center">
                                <i
                                    className={`fas 
            ${item.statusAgendamentoDescricao === 'Agendado'
                                            ? 'fa-calendar-check text-primary'
                                            : item.statusAgendamentoDescricao === 'Cancelado'
                                                ? 'fa-times-circle text-danger'
                                                : item.statusAgendamentoDescricao === 'Realizado'
                                                    ? 'fa-check-circle text-success'
                                                    : item.statusAgendamentoDescricao === 'Faltou'
                                                        ? 'fa-user-times text-warning'
                                                        : item.statusAgendamentoDescricao === 'Remarcado'
                                                            ? 'fa-sync-alt text-info'
                                                            : 'fa-question-circle text-secondary'
                                        } 
            mr-2`}
                                    style={{ width: '16px' }}
                                ></i>

                                <span className="small">
                                    <strong>Status do Agendamento:</strong> {item.statusAgendamentoDescricao}
                                </span>
                            </div>

                            <div className="d-flex align-items-center">
                                <i
                                    className={`fas 
            ${item.tipoAgendamentoDescricao === 'Consulta'
                                            ? 'fa-stethoscope text-primary'
                                            : item.tipoAgendamentoDescricao === 'Retorno'
                                                ? 'fa-undo-alt text-success'
                                                : item.tipoAgendamentoDescricao === 'Avaliação'
                                                    ? 'fa-clipboard-check text-warning'
                                                    : 'fa-question-circle text-secondary'
                                        } 
            mr-2`}
                                    style={{ width: '16px' }}
                                ></i>

                                <span className="small">
                                    <strong>Tipo de Agendamento:</strong> {item.tipoAgendamentoDescricao}
                                </span>
                            </div>

                        </div>

                    </div>

                    {/* Footer */}
                    <div className="card-footer bg-white d-flex justify-content-between align-items-center">

                        <button
                            className="btn btn-sm btn-primary"
                            onClick={() => this.setState({ cadastroModal: true, agendamentoIdSelecionado: item.id })}
                        >
                            <i className="fas fa-edit mr-1"></i> Editar
                        </button>

                        {/* Menu no canto direito */}
                        {/* <div className="dropdown">
                            <a className="btn btn-sm btn-light" href="#" role="button" data-toggle="dropdown">
                                <i className="fas fa-ellipsis-v"></i>
                            </a>
                            <div className="dropdown-menu dropdown-menu-right">
                                <a className="dropdown-item text-danger" href="#" onClick={(e) => { e.preventDefault(); this.excluir(item); }}>
                                    <i className="far fa-trash-alt mr-2"></i>Excluir
                                </a>
                            </div>
                        </div> */}

                    </div>

                </div>
            </div>
        );
    }

    render() {

        let saida =
            <div className="row">

                {/* Barra de pesquisa */}
                <div className="col-12 mb-4">
                    <div className="d-flex align-items-center justify-content-between" style={{ gap: '12px' }}>

                        {/* Input + Filtro + Busca */}
                        <div className="d-flex align-items-center flex-grow-1" style={{
                            background: '#fff',
                            borderRadius: '10px',
                            boxShadow: '0 2px 8px rgba(0,0,0,0.08)',
                            padding: '6px 12px',
                            gap: '8px'
                        }}>

                            <i className="fas fa-search text-muted"></i>

                            <input
                                type="text"
                                className="border-0 flex-grow-1"
                                placeholder="Pesquisar agendamento..."
                                style={{ outline: 'none', fontSize: '14px', background: 'transparent' }}
                                onChange={(e) => this.setState({ pesquisar: e.target.value })}
                            />

                            {/* Separador */}
                            <div style={{ width: '1px', height: '24px', background: '#dee2e6' }}></div>

                            {/* Filtro dropdown */}
                            <div className="dropdown">
                                <button
                                    type="button"
                                    className="btn btn-sm btn-light border-0"
                                    data-toggle="dropdown"
                                    title="Filtros"
                                    style={{ borderRadius: '8px' }}
                                >
                                    <i className="fas fa-filter text-muted mr-1"></i>
                                    <span className="small text-muted">Filtrar</span>
                                </button>
                                <div className="dropdown-menu p-3" style={{ minWidth: '220px' }} onClick={(e) => e.stopPropagation()}>
                                    <p className="text-muted small font-weight-bold mb-2">Modalidade</p>
                                    <select
                                        className="form-control form-control-sm"
                                        value={this.state.filtro}
                                        onChange={(e) => this.setState({ filtro: e.target.value })}
                                    >
                                        <option value="0">Todos</option>
                                        <option value="1">Presencial</option>
                                        <option value="2">Online</option>
                                    </select>
                                </div>
                            </div>

                            {/* Botão buscar */}
                            <button
                                type="button"
                                className="btn btn-sm btn-primary"
                                onClick={() => this.pesquisar()}
                                style={{ borderRadius: '8px' }}
                            >
                                Buscar
                            </button>

                        </div>

                        {/* Botão Novo */}
                        <button
                            type="button"
                            className="btn btn-primary"
                            onClick={this.cadastroModalAbrir}
                            style={{ borderRadius: '10px', whiteSpace: 'nowrap', padding: '10px 20px' }}
                        >
                            <i className="fas fa-plus mr-2"></i>Novo
                        </button>

                    </div>

                    {/* Legenda resultado */}
                    {this.state.legendaResultadoPesquisa && (
                        <div className="text-muted small mt-2 ml-1">
                            {this.state.legendaResultadoPesquisa}
                        </div>
                    )}
                </div>

                {/* Cards */}
                {this.state.aguarde ? (
                    <div className="col-12">
                        <LoadingIndicator timeWait={500} />
                    </div>
                ) : this.state.resultadoPesquisa.length === 0 ? (
                    <div className="col-12 text-center text-muted py-5">
                        <i className="fas fa-calendar-times fa-2x mb-3 d-block"></i>
                        Nenhum agendamento foi encontrado.
                    </div>
                ) : (
                    this.state.resultadoPesquisa.map(item => this.renderAgendamento(item))
                )}

                {this.state.cadastroModal
                    ? <Cadastro onFechar={this.cadastroModalFechar} idEdicao={this.state.agendamentoIdSelecionado} />
                    : null
                }

            </div>

        return (saida);
    }
}

createRoot(document.getElementById('root')).render(<React.StrictMode> <Index /> </React.StrictMode>);