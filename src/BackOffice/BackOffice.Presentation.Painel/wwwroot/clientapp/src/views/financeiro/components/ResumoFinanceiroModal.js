import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class ResumoFinanceiroModal extends Component {

    constructor(props) {
        super(props);

        const hoje = new Date();
        const primeiroDiaMes = new Date(hoje.getFullYear(), hoje.getMonth(), 1);

        this.state = {
            aguarde: false,
            dataInicio: this.toInputDate(primeiroDiaMes),
            dataFim: this.toInputDate(hoje),
            resumo: {
                totalReceita: 0,
                totalDespesa: 0,
                saldo: 0,
            },
        };
    }

    componentDidMount = () => {
        this.obterResumo();
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    toInputDate = (date) => {
        const yyyy = date.getFullYear();
        const mm = String(date.getMonth() + 1).padStart(2, '0');
        const dd = String(date.getDate()).padStart(2, '0');
        return `${yyyy}-${mm}-${dd}`;
    }

    // ─── HTTP ────────────────────────────────────────────────────────────────

    obterResumo = () => {
        const { dataInicio, dataFim } = this.state;

        this.setState({ aguarde: true });

        return HTTPClient.get(`Administrativo/Financeiro/ObterResumo?dataInicio=${dataInicio}&dataFim=${dataFim}`)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                this.setState({ resumo: r.data });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao obter o resumo financeiro.Aqui" });
            })
            .finally(() => {
                this.setState({ aguarde: false });
            });
    }

    // ─── Atalhos de período ─────────────────────────────────────────────────

    aplicarPeriodo = (tipo) => {
        const hoje = new Date();
        let inicio, fim = hoje;

        if (tipo === 'mesAtual') {
            inicio = new Date(hoje.getFullYear(), hoje.getMonth(), 1);
        } else if (tipo === 'mesAnterior') {
            inicio = new Date(hoje.getFullYear(), hoje.getMonth() - 1, 1);
            fim = new Date(hoje.getFullYear(), hoje.getMonth(), 0);
        } else if (tipo === 'ano') {
            inicio = new Date(hoje.getFullYear(), 0, 1);
        }

        this.setState({
            dataInicio: this.toInputDate(inicio),
            dataFim: this.toInputDate(fim),
        }, () => this.obterResumo());
    }

    // ─── Render ──────────────────────────────────────────────────────────────

    render() {
        const { aguarde, dataInicio, dataFim, resumo } = this.state;
        const totalReceita = Number(resumo.totalReceitas) || 0;
        const totalDespesa = Number(resumo.totalDespesas) || 0;
        const saldo = Number(resumo.saldo) || 0;

        // percentuais para o gráfico de barras (base: o maior dos dois valores)
        const maiorValor = Math.max(totalReceita, totalDespesa, 1);
        const pctReceita = (totalReceita / maiorValor) * 100;
        const pctDespesa = (totalDespesa / maiorValor) * 100;

        let saida =
            <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                <div className="modal-dialog modal-lg">
                    <div className="modal-content">

                        <div className="modal-header">
                            <h5 className="modal-title">
                                <i className="fas fa-chart-pie mr-2 text-primary"></i>
                                Resumo Financeiro
                            </h5>
                            <button type="button" className="close" onClick={() => this.props.onFechar()}>
                                <span>&times;</span>
                            </button>
                        </div>

                        <div className="modal-body">

                            {/* FILTRO DE DATAS */}
                            <div className="row align-items-end mb-4">
                                <div className="col-md-4 form-group mb-2">
                                    <label className="small text-muted mb-1">Data início</label>
                                    <input
                                        type="date"
                                        className="form-control"
                                        value={dataInicio}
                                        onChange={(e) => this.setState({ dataInicio: e.target.value })}
                                    />
                                </div>
                                <div className="col-md-4 form-group mb-2">
                                    <label className="small text-muted mb-1">Data fim</label>
                                    <input
                                        type="date"
                                        className="form-control"
                                        value={dataFim}
                                        onChange={(e) => this.setState({ dataFim: e.target.value })}
                                    />
                                </div>
                                <div className="col-md-4 form-group mb-2">
                                    <button
                                        type="button"
                                        className="btn btn-primary btn-block"
                                        onClick={this.obterResumo}
                                        disabled={aguarde}
                                    >
                                        {aguarde
                                            ? <><i className="fas fa-spinner fa-spin mr-1"></i>Buscando...</>
                                            : <><i className="fas fa-filter mr-1"></i>Filtrar</>
                                        }
                                    </button>
                                </div>
                            </div>

                            {/* ATALHOS DE PERÍODO */}
                            <div className="mb-4">
                                <button type="button" className="btn btn-sm btn-outline-secondary mr-2" onClick={() => this.aplicarPeriodo('mesAtual')}>
                                    Mês atual
                                </button>
                                <button type="button" className="btn btn-sm btn-outline-secondary mr-2" onClick={() => this.aplicarPeriodo('mesAnterior')}>
                                    Mês anterior
                                </button>
                                <button type="button" className="btn btn-sm btn-outline-secondary" onClick={() => this.aplicarPeriodo('ano')}>
                                    Este ano
                                </button>
                            </div>

                            {aguarde ?
                                <LoadingIndicator />
                                :
                                <>
                                    {/* CARDS */}
                                    <div className="row mb-4">
                                        <div className="col-md-4 mb-2 mb-md-0">
                                            <div className="card border-left-success shadow-sm h-100">
                                                <div className="card-body py-3">
                                                    <div className="small text-muted text-uppercase">Receitas</div>
                                                    <div className="h4 mb-0 text-success font-weight-bold">
                                                        {formatarMoeda(totalReceita)}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="col-md-4 mb-2 mb-md-0">
                                            <div className="card border-left-danger shadow-sm h-100">
                                                <div className="card-body py-3">
                                                    <div className="small text-muted text-uppercase">Despesas</div>
                                                    <div className="h4 mb-0 text-danger font-weight-bold">
                                                        {formatarMoeda(totalDespesa)}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div className="col-md-4">
                                            <div className={`card shadow-sm h-100 ${saldo >= 0 ? 'border-left-success' : 'border-left-danger'}`}>
                                                <div className="card-body py-3">
                                                    <div className="small text-muted text-uppercase">Saldo</div>
                                                    <div className={`h4 mb-0 font-weight-bold ${saldo >= 0 ? 'text-success' : 'text-danger'}`}>
                                                        {formatarMoeda(saldo)}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    {/* GRÁFICO DE BARRAS (RECEITA X DESPESA) */}
                                    <div className="card shadow-sm">
                                        <div className="card-body">
                                            <div className="small text-muted text-uppercase mb-3">Receita x Despesa</div>

                                            <div className="mb-3">
                                                <div className="d-flex justify-content-between mb-1">
                                                    <span className="small font-weight-bold text-success">Receitas</span>
                                                    <span className="small font-weight-bold text-success">{formatarMoeda(totalReceita)}</span>
                                                </div>
                                                <div style={{ height: 10, borderRadius: 6, backgroundColor: '#eef0f2', overflow: 'hidden' }}>
                                                    <div style={{
                                                        height: '100%',
                                                        width: `${pctReceita}%`,
                                                        backgroundColor: '#28a745',
                                                        borderRadius: 6,
                                                        transition: 'width 0.4s ease',
                                                    }} />
                                                </div>
                                            </div>

                                            <div>
                                                <div className="d-flex justify-content-between mb-1">
                                                    <span className="small font-weight-bold text-danger">Despesas</span>
                                                    <span className="small font-weight-bold text-danger">{formatarMoeda(totalDespesa)}</span>
                                                </div>
                                                <div style={{ height: 10, borderRadius: 6, backgroundColor: '#eef0f2', overflow: 'hidden' }}>
                                                    <div style={{
                                                        height: '100%',
                                                        width: `${pctDespesa}%`,
                                                        backgroundColor: '#dc3545',
                                                        borderRadius: 6,
                                                        transition: 'width 0.4s ease',
                                                    }} />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </>
                            }
                        </div>

                        <div className="modal-footer">
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={() => this.props.onFechar()}
                            >
                                Fechar
                            </button>
                        </div>

                    </div>
                </div>
            </div>

        return (saida);
    }
}