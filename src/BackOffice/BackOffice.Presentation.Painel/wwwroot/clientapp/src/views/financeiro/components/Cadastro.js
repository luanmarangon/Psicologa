import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Cadastro extends Component {

    constructor(props) {
        super(props);

        this.state = {
            aguarde: false,
            aguardeCategorias: false,
            aguardeSalvar: false,
            aguardeExcluir: false,
            categorias: [],
            dados: {
                id: 0,
                tipo: 1, // "despesa" | "receita"
                categoria: 0,
                categoriaNome: "",
                descricao: "",
                dataLancamento: "",
                valor: "",
                observacao: "",
                quitado: false, // "pago" para despesa, "recebido" para receita — mesmo campo
                dataQuitacao: "",
                ativo: true,
            },
        };
    }

    componentDidMount = () => {
        if (this.props.idEdicao) {
            // Ao editar, primeiro garante que sabe o tipo do lançamento antes de buscar
            // as categorias corretas (senão busca categorias de despesa por padrão)
            this.obter(this.props.idEdicao).then(() => {
                this.obterCategorias(this.state.dados.tipo);
            });
        } else {
            this.obterCategorias(this.state.dados.tipo);
        }
    }

    // ─── HTTP ────────────────────────────────────────────────────────────────

    obterCategorias = (tipo) => {
        this.setState({ aguardeCategorias: true });

        return HTTPClient.get(`Administrativo/Financeiro/ObterCategorias?tipo=${tipo}`)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                this.setState({ categorias: r.data });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao obter as categorias." });
            })
            .finally(() => {
                this.setState({ aguardeCategorias: false });
            });
    }

    obter = (id) => {
        this.setState({ aguarde: true });

        return HTTPClient.get(`Administrativo/Financeiro/Obter?id=${id}`)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                this.setState({ dados: { ...this.state.dados, ...r.data } });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao obter o lançamento." });
            })
            .finally(() => {
                this.setState({ aguarde: false });
            });
    }

    salvar = () => {
        const { dados: form } = this.state;

        if (!form.descricao) return showToastr({ type: "warning", text: "Informe a descrição." });
        if (!form.categoria) return showToastr({ type: "warning", text: "Selecione a categoria." });
        if (!form.dataLancamento) return showToastr({ type: "warning", text: "Informe a data." });
        if (!form.valor || Number(form.valor) <= 0) return showToastr({ type: "warning", text: "Informe um valor válido." });

        this.setState({ aguardeSalvar: true });

        const payload = {
            ...form,
            valor: parseFloat(form.valor), // garante decimal, evita truncar casas ao enviar como string
        };

        HTTPClient.post("Administrativo/Financeiro/Salvar", payload)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                showToastr({ type: "success", text: "Lançamento salvo com sucesso." });
                this.props.onFechar(r.data);
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao salvar o lançamento." });
            })
            .finally(() => {
                this.setState({ aguardeSalvar: false });
            });
    }

    // ─── Handlers ────────────────────────────────────────────────────────────

    handleChange = (campo, valor) => {
        this.setState(prev => ({
            dados: { ...prev.dados, [campo]: valor }
        }));
    }

    handleTipoChange = (tipo) => {
        // tipo: 1 = despesa, 2 = receita
        this.setState(prev => ({
            dados: { ...prev.dados, tipo, categoria: "" }
        }), () => {
            this.obterCategorias(tipo);
        });
    }
    // ─── Render ──────────────────────────────────────────────────────────────

    render() {
        const { aguarde, aguardeCategorias, aguardeSalvar, categorias, dados: form } = this.state;
        const edicao = !!this.props.idEdicao;
        const ehReceita = form.tipo === 2; // 2 = receita

        let saida =
            <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                <div className="modal-dialog">
                    <div className="modal-content">

                        <div className="modal-header">
                            <h5 className="modal-title">
                                <i className={`fas ${ehReceita ? 'fa-arrow-circle-up text-success' : 'fa-arrow-circle-down text-danger'} mr-2`}></i>
                                {edicao ? "Editar Lançamento" : "Novo Lançamento"}
                            </h5>
                            <button type="button" className="close" onClick={() => this.props.onFechar(null)}>
                                <span>&times;</span>
                            </button>
                        </div>

                        <div className="modal-body">
                            {aguarde ?
                                <LoadingIndicator />
                                :
                                <div className="row">

                                    {/* TIPO — Despesa / Receita */}
                                    {!edicao &&
                                        <div className="col-12 form-group">
                                            <div className="btn-group btn-block" role="group">
                                                <button
                                                    type="button"
                                                    className={`btn ${!ehReceita ? 'btn-danger' : 'btn-outline-danger'}`}
                                                    onClick={() => this.handleTipoChange(1)}
                                                >
                                                    <i className="fas fa-arrow-circle-down mr-1"></i>
                                                    Despesa
                                                </button>
                                                <button
                                                    type="button"
                                                    className={`btn ${ehReceita ? 'btn-success' : 'btn-outline-success'}`}
                                                    onClick={() => this.handleTipoChange(2)}
                                                >
                                                    <i className="fas fa-arrow-circle-up mr-1"></i>
                                                    Receita
                                                </button>
                                            </div>
                                        </div>
                                    }

                                    {/* DESCRIÇÃO */}
                                    <div className="col-12 form-group">
                                        <label>Descrição <span className="text-danger">*</span></label>
                                        <input
                                            type="text"
                                            className="form-control"
                                            placeholder={ehReceita ? "Ex: Sessão Paciente 1" : "Ex: Aluguel sala 2 - Agosto"}
                                            value={form.descricao || ''}
                                            onChange={(e) => this.handleChange('descricao', e.target.value)}
                                        />
                                    </div>

                                    {/* CATEGORIA */}
                                    <div className="col-12 form-group">
                                        <label>Categoria <span className="text-danger">*</span></label>
                                        <select
                                            className="form-control"
                                            value={form.categoria || ''}
                                            disabled={aguardeCategorias}
                                            onChange={(e) => this.handleChange('categoria', e.target.value)}
                                        >
                                            <option value="">{aguardeCategorias ? "Carregando..." : "Selecione..."}</option>
                                            {categorias.map(c => (
                                                <option key={c.id} value={c.id}>{c.nome}</option>
                                            ))}
                                        </select>
                                        {!aguardeCategorias && categorias.length === 0 &&
                                            <small className="text-muted d-block mt-1">
                                                Nenhuma categoria cadastrada. Cadastre em Configurações.
                                            </small>
                                        }
                                    </div>

                                    {/* DATA */}
                                    <div className="col-6 form-group">
                                        <label>Data <span className="text-danger">*</span></label>
                                        <input
                                            type="date"
                                            className="form-control"
                                            value={formatarDataPtBrToInputDate(form.dataLancamento)}
                                            onChange={(e) => this.handleChange('dataLancamento', e.target.value)}
                                        />
                                        {/* <input type="date" className="form-control" id="txtDataNascimento" value={formatarDataPtBrToInputDate(form.dataLancamento)}
                                            onChange={(e) => this.setState({ dados: { ...this.state.dados, dataLancamento: e.target.value } })} /> */}
                                    </div>

                                    {/* VALOR */}
                                    <div className="col-6 form-group">
                                        <label>Valor <span className="text-danger">*</span></label>
                                        <div className="input-group">
                                            <div className="input-group-prepend">
                                                <span className="input-group-text">R$</span>
                                            </div>
                                            {/* <input
                                                type="number"
                                                className="form-control"
                                                min={0}
                                                step="0.01"
                                                value={(form.valor)}
                                                onChange={(e) => this.handleChange('valor', e.target.value)}
                                            /> */}
                                            {/* <input
                                                type="text"
                                                inputMode="numeric"
                                                className="form-control"
                                                value={floatToPTBRString(form.valor || 0)}
                                                // onChange={this.handleValorChange}
                                                onChange={(e) => this.setState({ dados: { ...this.state.dados, valor: e.target.value } })} 
                                            /> */}
                                            <input
                                                type="text"
                                                inputMode="decimal"
                                                className="form-control"
                                                value={form.valor}
                                                onChange={(e) => {
                                                    const valor = e.target.value.replace(',', '.');
                                                    this.setState({ dados: { ...this.state.dados, valor } });
                                                }}
                                            />
                                        </div>
                                    </div>

                                    {/* OBSERVAÇÃO */}
                                    <div className="col-12 form-group">
                                        <label>Observação</label>
                                        <textarea
                                            className="form-control"
                                            rows={2}
                                            value={form.observacao}
                                            onChange={(e) => this.handleChange('observacao', e.target.value)}
                                        />
                                    </div>

                                    {/* QUITADO */}
                                    <div className="col-6 form-group">
                                        <div className="custom-control custom-switch mt-1">
                                            <input
                                                type="checkbox"
                                                className="custom-control-input"
                                                id="quitado"
                                                checked={form.quitado || false}
                                                onChange={(e) => {
                                                    const quitado = e.target.checked;
                                                    this.setState(prev => ({
                                                        dados: {
                                                            ...prev.dados,
                                                            quitado,
                                                            dataQuitacao: quitado
                                                                ? (prev.dados.dataQuitacao || new Date().toISOString().slice(0, 10))
                                                                : '',
                                                        }
                                                    }));
                                                }}
                                            />
                                            <label className="custom-control-label" htmlFor="quitado">
                                                {form.quitado
                                                    ? (ehReceita ? "Recebido" : "Pago")
                                                    : (ehReceita ? "A receber" : "Pendente")
                                                }
                                            </label>
                                        </div>
                                    </div>

                                    {form.quitado &&
                                        <div className="col-6 form-group">
                                            <label>Data {ehReceita ? "do Recebimento" : "do Pagamento"}</label>
                                            <input
                                                type="date"
                                                className="form-control"
                                                value={formatarDataPtBrToInputDate(form.dataQuitacao)}
                                                onChange={(e) => this.handleChange('dataQuitacao', e.target.value)}
                                            />
                                        </div>
                                    }

                                </div>
                            }
                        </div>

                        <div className="modal-footer">
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={() => this.props.onFechar(null)}
                                disabled={aguardeSalvar}
                            >
                                Cancelar
                            </button>

                            <button
                                type="button"
                                className="btn btn-primary"
                                onClick={this.salvar}
                                disabled={aguardeSalvar}
                            >
                                {aguardeSalvar
                                    ? <><i className="fas fa-spinner fa-spin mr-1"></i>Salvando...</>
                                    : <><i className="fas fa-save mr-1"></i>Salvar</>
                                }
                            </button>
                        </div>

                    </div>
                </div>
            </div>

        return (saida);
    }
}