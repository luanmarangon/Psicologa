import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class ConfiguracaoFuncionamento extends Component {

    constructor(props) {

        super(props);

        this.state = {

            dados: {

                id: "",

                funcionamento: [
                    {
                        diaSemana: 2,
                        nome: "Segunda-feira",
                        ativo: true,
                        periodos: []
                    },
                    {
                        diaSemana: 3,
                        nome: "Terça-feira",
                        ativo: true,
                        periodos: []
                    },
                    {
                        diaSemana: 4,
                        nome: "Quarta-feira",
                        ativo: true,
                        periodos: []
                    },
                    {
                        diaSemana: 5,
                        nome: "Quinta-feira",
                        ativo: true,
                        periodos: []
                    },
                    {
                        diaSemana: 6,
                        nome: "Sexta-feira",
                        ativo: true,
                        periodos: []
                    },
                    {
                        diaSemana: 7,
                        nome: "Sábado",
                        ativo: false,
                        periodos: []
                    },
                    {
                        diaSemana: 1,
                        nome: "Domingo",
                        ativo: false,
                        periodos: []
                    }
                ]

            },

            iniciando: true,
            obtendo: false,
            aguarde: false,

        };

    }

    componentDidMount = () => {

        let that = this;

        this.obter().finally(() => {
            this.setState({
                iniciando: false
            });
        });

        $("#cadastroModal").modal('show');

        $('#cadastroModal').on('hidden.bs.modal', function () {

            that.props.onFechar();

        });

        this.setState({
            iniciando: false
        });

    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');

    }

    alterarDia = (index, campo, valor) => {

        const funcionamento = [...this.state.dados.funcionamento];

        funcionamento[index][campo] = valor;

        this.setState({
            dados: {
                ...this.state.dados,
                funcionamento
            }
        });

    }

    alterarPeriodo = (diaIndex, periodoIndex, campo, valor) => {

        const funcionamento = [...this.state.dados.funcionamento];

        funcionamento[diaIndex]
            .periodos[periodoIndex][campo] = valor;

        this.setState({
            dados: {
                ...this.state.dados,
                funcionamento
            }
        });

    }

    adicionarPeriodo = (diaIndex) => {

        const funcionamento = [...this.state.dados.funcionamento];

        funcionamento[diaIndex]
            .periodos
            .push({
                horaInicio: "",
                horaFim: ""
            });

        this.setState({
            dados: {
                ...this.state.dados,
                funcionamento
            }
        });

    }

    removerPeriodo = (diaIndex, periodoIndex) => {

        const funcionamento = [...this.state.dados.funcionamento];

        funcionamento[diaIndex]
            .periodos
            .splice(periodoIndex, 1);

        this.setState({
            dados: {
                ...this.state.dados,
                funcionamento
            }
        });

    }

    
    // obter = () => {

    //     let p = HTTPClient.get("Administrativo/Configuracao/ObterConfiguracaoFuncionamento")
    //         .then(r => {
    //             return r.json();
    //         })
    //         .then(r => {

    //             this.setState({
    //                 ...this.dados,
    //                 dados: {
    //                     ...this.state.dados,

    //                 }
    //             });

    //         })
    //         .catch((e) => {
    //             showToastr({
    //                 type: "error",
    //                 text: "Um erro ocorreu."
    //             });
    //         });

    //     return p;
    // }
    obter = () => {

        let p = HTTPClient
            .get("Administrativo/Configuracao/ObterConfiguracaoFuncionamento")
            .then(r => r.json())
            .then(r => {

                if (!r.success) {

                    showToastr(r.messages);

                    return;
                }

                const funcionamentoApi =
                    r.data.funcionamento || [];

                // mescla com estrutura padrão
                const funcionamento =
                    this.state.dados.funcionamento.map(diaPadrao => {

                        const diaApi =
                            funcionamentoApi.find(x =>
                                x.diaSemana === diaPadrao.diaSemana);

                        // se existir na API usa os dados
                        if (diaApi) {

                            return {
                                ...diaPadrao,
                                ...diaApi
                            };
                        }

                        // mantém padrão vazio
                        return diaPadrao;
                    });

                this.setState({
                    dados: {
                        ...this.state.dados,

                        id: r.data.id,

                        funcionamento
                    }
                });

            })
            .catch(() => {

                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });

            });

        return p;
    }
    salvar = () => {

        this.setState({
            aguarde: true
        });

        HTTPClient.post(
            "Administrativo/Configuracao/SalvarFuncionamento",
            this.state.dados
        )
            .then(r => r.json())
            .then(r => {

                if (r.success) {

                    showToastr(r.messages);

                    this.props.onFechar(r.data);

                }
                else {

                    showToastr(r.messages);

                }

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

    render() {

        let form =

            <div className="row">

                {
                    this.state.dados.funcionamento.map((dia, diaIndex) => (

                        <div
                            className="col-lg-6 mb-4"
                            key={dia.diaSemana}
                        >

                            <div
                                className="card border-0 shadow-sm"
                                style={{
                                    borderRadius: 22,
                                    overflow: "hidden"
                                }}
                            >

                                {/* HEADER */}
                                <div
                                    className="d-flex align-items-center justify-content-between"
                                    style={{
                                        padding: "20px 22px",
                                        borderBottom: "1px solid #eef2f7",
                                        background: "#ffffff"
                                    }}
                                >

                                    <div>

                                        <div
                                            style={{
                                                fontWeight: 700,
                                                fontSize: 17,
                                                color: "#1e293b"
                                            }}
                                        >
                                            {dia.nome}
                                        </div>

                                        <div
                                            style={{
                                                fontSize: 13,
                                                color: "#64748b",
                                                marginTop: 4
                                            }}
                                        >

                                            {
                                                dia.ativo
                                                    ? "Atendimento disponível"
                                                    : "Sem atendimento"
                                            }

                                        </div>

                                    </div>

                                    <div className="custom-control custom-switch">

                                        <input
                                            type="checkbox"
                                            className="custom-control-input"
                                            id={`dia-${diaIndex}`}
                                            checked={dia.ativo}
                                            onChange={(e) =>
                                                this.alterarDia(
                                                    diaIndex,
                                                    "ativo",
                                                    e.target.checked
                                                )
                                            }
                                        />

                                        <label
                                            className="custom-control-label"
                                            htmlFor={`dia-${diaIndex}`}
                                        />

                                    </div>

                                </div>

                                {/* BODY */}
                                <div
                                    style={{
                                        padding: 22,
                                        background: "#f8fafc"
                                    }}
                                >

                                    {
                                        !dia.ativo && (

                                            <div
                                                className="text-center"
                                                style={{
                                                    padding: "18px 10px",
                                                    borderRadius: 18,
                                                    background: "#ffffff",
                                                    color: "#94a3b8",
                                                    fontSize: 14
                                                }}
                                            >

                                                Não haverá atendimento neste dia.

                                            </div>

                                        )
                                    }

                                    {
                                        dia.ativo && (

                                            <>

                                                {
                                                    dia.periodos.map((periodo, periodoIndex) => (

                                                        <div
                                                            key={periodoIndex}
                                                            className="d-flex align-items-center mb-3"
                                                            style={{
                                                                gap: 12
                                                            }}
                                                        >

                                                            <div style={{ flex: 1 }}>

                                                                <label
                                                                    className="small text-muted mb-1"
                                                                >
                                                                    Início
                                                                </label>

                                                                <input
                                                                    type="time"
                                                                    className="form-control"
                                                                    value={periodo.horaInicio}
                                                                    onChange={(e) =>
                                                                        this.alterarPeriodo(
                                                                            diaIndex,
                                                                            periodoIndex,
                                                                            "horaInicio",
                                                                            e.target.value
                                                                        )
                                                                    }
                                                                    style={{
                                                                        height: 48,
                                                                        borderRadius: 14,
                                                                        border: "1px solid #dbe2ea"
                                                                    }}
                                                                />

                                                            </div>

                                                            <div style={{ flex: 1 }}>

                                                                <label
                                                                    className="small text-muted mb-1"
                                                                >
                                                                    Fim
                                                                </label>

                                                                <input
                                                                    type="time"
                                                                    className="form-control"
                                                                    value={periodo.horaFim}
                                                                    onChange={(e) =>
                                                                        this.alterarPeriodo(
                                                                            diaIndex,
                                                                            periodoIndex,
                                                                            "horaFim",
                                                                            e.target.value
                                                                        )
                                                                    }
                                                                    style={{
                                                                        height: 48,
                                                                        borderRadius: 14,
                                                                        border: "1px solid #dbe2ea"
                                                                    }}
                                                                />

                                                            </div>

                                                            <button
                                                                type="button"
                                                                className="btn btn-light"
                                                                onClick={() =>
                                                                    this.removerPeriodo(
                                                                        diaIndex,
                                                                        periodoIndex
                                                                    )
                                                                }
                                                                style={{
                                                                    width: 46,
                                                                    height: 46,
                                                                    borderRadius: 14,
                                                                    marginTop: 24
                                                                }}
                                                            >

                                                                <i className="fas fa-trash text-danger"></i>

                                                            </button>

                                                        </div>

                                                    ))
                                                }

                                                <button
                                                    type="button"
                                                    className="btn btn-light btn-block"
                                                    onClick={() =>
                                                        this.adicionarPeriodo(diaIndex)
                                                    }
                                                    style={{
                                                        height: 50,
                                                        borderRadius: 16,
                                                        border: "1px dashed #cbd5e1",
                                                        background: "#ffffff",
                                                        color: "#475569",
                                                        fontWeight: 600
                                                    }}
                                                >

                                                    <i className="fas fa-plus mr-2"></i>

                                                    Adicionar período

                                                </button>

                                            </>

                                        )
                                    }

                                </div>

                            </div>

                        </div>

                    ))
                }

            </div>;

        let modal =

            <div
                className="modal fade"
                id="cadastroModal"
                data-keyboard="true"
                tabIndex="-1"
            >

                <div className="modal-dialog modal-xl modal-dialog-scrollable">

                    <div
                        className="modal-content border-0"
                        style={{
                            borderRadius: 24,
                            overflow: "hidden"
                        }}
                    >

                        {/* HEADER */}
                        <div
                            className="modal-header border-0"
                            style={{
                                background: "linear-gradient(135deg,#7c3aed 0%,#6d28d9 100%)",
                                padding: "24px 28px"
                            }}
                        >

                            <div>

                                <h4
                                    className="modal-title text-white mb-1"
                                    style={{
                                        fontWeight: 700
                                    }}
                                >
                                    Configuração de Funcionamento
                                </h4>

                                <div
                                    style={{
                                        color: "rgba(255,255,255,.75)",
                                        fontSize: 14
                                    }}
                                >
                                    Configure os horários disponíveis para atendimento.
                                </div>

                            </div>

                            <button
                                type="button"
                                className="close text-white"
                                data-dismiss="modal"
                            >

                                <span>&times;</span>

                            </button>

                        </div>

                        {/* BODY */}
                        <div
                            className="modal-body"
                            style={{
                                background: "#f8fafc",
                                padding: 28
                            }}
                        >

                            {
                                !this.state.iniciando
                                    ? form
                                    : <LoadingIndicator />
                            }

                        </div>

                        {/* FOOTER */}
                        <div
                            className={
                                "modal-footer border-0 " +
                                (this.state.aguarde ? "site-disabled" : "")
                            }
                            style={{
                                padding: 22,
                                background: "#ffffff"
                            }}
                        >

                            <button
                                type="button"
                                className="btn btn-light"
                                data-dismiss="modal"
                                style={{
                                    height: 48,
                                    borderRadius: 14,
                                    padding: "0 22px"
                                }}
                            >

                                Cancelar

                            </button>

                            <button
                                type="button"
                                className="btn text-white"
                                onClick={this.salvar}
                                style={{
                                    height: 48,
                                    borderRadius: 14,
                                    padding: "0 26px",
                                    background: "linear-gradient(135deg,#7c3aed 0%,#6d28d9 100%)",
                                    fontWeight: 600
                                }}
                            >

                                <i className="fas fa-save mr-2"></i>

                                Salvar funcionamento

                            </button>

                        </div>

                    </div>

                </div>

            </div>;

        return modal;

    }

}