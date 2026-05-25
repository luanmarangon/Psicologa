import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Cadastro extends Component {

    constructor(props) {
        super(props);

        this.state = {
            dados: {
                id: this.props.idEdicao || 0,

                servicoId: 0,
                servicoNome: "",

                nome: "",
                contato: "",
                email: "",
                mensagem: "",

                statusContato: 0,
                entrouContato: false,

                dataContato: "",
                dataRetorno: "",

                observacaoInterna: "",
                origem: "",

                virouPaciente: false,

                prioridade: 0,
                preferenciaContato: 0,

                ip: "",
                userAgent: ""
            },

            parametros: {
                statusContato: [],
                prioridade: [],
                preferenciaContato: []
            },

            servicos: [],

            iniciando: true,
            aguarde: false
        };
    }

    componentDidMount = () => {
        const promises = [
            this.obterParametros()
        ];

        // Carrega serviços apenas quando não tiver serviço fixo
        if (!this.props.servicoNome) {
            promises.push(this.obterServicos());
        }

        // Edição
        if (this.props.idEdicao) {
            promises.push(this.obter(this.props.idEdicao));
        }

        Promise.all(promises)
            .finally(() => {

                this.setState({
                    iniciando: false
                });

            });

        $("#cadastroModal").modal('show');

        $('#cadastroModal').on('hidden.bs.modal', () => {
            this.props.onFechar();
        });

    }

    componentWillUnmount = () => {

        $('#cadastroModal').off('hidden.bs.modal');
        $('#cadastroModal').modal('hide');

    }

    setCampo = (campo, valor) => {

        this.setState(prev => ({
            dados: {
                ...prev.dados,
                [campo]: valor
            }
        }));

    }

    obter = (id) => {

        return HTTPClient.get("Administrativo/Leads/Obter?id=" + id)
            .then(r => r.json())
            .then(r => {

                if (r.success) {

                    this.setState({
                        dados: {

                            ...this.state.dados,

                            id: r.data.id,

                            servicoId: r.data.servicoId || 0,
                            servicoNome: r.data.servicoNome || "",

                            nome: r.data.nome || "",
                            contato: r.data.contato || "",
                            email: r.data.email || "",
                            mensagem: r.data.mensagem || "",

                            statusContato: r.data.statusContato || 0,
                            entrouContato: r.data.entrouContato || false,

                            dataContato: r.data.dataContato
                                ? new Date(r.data.dataContato).toISOString().split('T')[0]
                                : "",

                            dataRetorno: r.data.dataRetorno
                                ? new Date(r.data.dataRetorno).toISOString().split('T')[0]
                                : "",

                            observacaoInterna: r.data.observacaoInterna || "",
                            origem: r.data.origem || "",

                            virouPaciente: r.data.virouPaciente || false,

                            prioridade: r.data.prioridade || 0,
                            preferenciaContato: r.data.preferenciaContato || 0,

                            ip: r.data.ip || "",
                            userAgent: r.data.userAgent || ""
                        }
                    });

                }

            })
            .catch(() => {

                showToastr({
                    type: "error",
                    text: "Erro ao obter lead."
                });

            });

    }

    obterParametros = () => {

        return HTTPClient.get("Administrativo/Leads/ObterParametros")
            .then(r => r.json())
            .then(r => {

                if (r.success) {

                    this.setState({
                        parametros: r.data
                    });

                }

            })
            .catch(() => {

                showToastr({
                    type: "error",
                    text: "Erro ao obter parâmetros."
                });

            });

    }

    obterServicos = () => {

        return HTTPClient.get("Administrativo/Servico/ObterTodos")
            .then(r => r.json())
            .then(r => {

                if (r.success) {

                    this.setState({
                        servicos: r.data
                    });

                }

            })
            .catch(() => {

                showToastr({
                    type: "error",
                    text: "Erro ao obter serviços."
                });

            });

    }

    salvar = () => {

        this.setState({
            aguarde: true
        });

        HTTPClient.post("Administrativo/Leads/Salvar", this.state.dados)
            .then(r => r.json())
            .then(r => {

                if (r.success) {

                    showToastr(r.messages);

                    $("#cadastroModal").modal('hide');

                    this.props.onFechar(r.data);
                    showToastr(r.messages);
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

        const {
            dados,
            parametros,
            servicos,
            iniciando,
            aguarde
        } = this.state;

        let form =

            <form role="form">

                {/* DADOS CONTATO */}
                <div className="card border-0 shadow-sm mb-4">

                    <div className="card-header bg-white">

                        <h5 className="mb-0 font-weight-bold">
                            <i className="fas fa-user-circle mr-2 text-primary"></i>
                            Dados do contato
                        </h5>

                    </div>

                    <div className="card-body">

                        <div className="row">

                            <div className="col-md-6 form-group">

                                <label>Nome</label>

                                <input
                                    type="text"
                                    className="form-control"
                                    value={dados.nome}
                                    disabled={dados.nome !== ""}
                                    onChange={(e) =>
                                        this.setCampo("nome", e.target.value)
                                    }
                                />

                            </div>

                            <div className="col-md-6 form-group">

                                <label>Serviço</label>

                                {dados.servicoNome ? (

                                    <input
                                        type="text"
                                        className="form-control"
                                        disabled
                                        value={dados.servicoNome}
                                    />

                                ) : (

                                    <select
                                        className="form-control"
                                        value={dados.servicoId}
                                        onChange={(e) =>
                                            this.setCampo(
                                                "servicoId",
                                                parseInt(e.target.value, 10)
                                            )
                                        }
                                    >

                                        <option value={0}>
                                            Selecione um serviço
                                        </option>

                                        {servicos.map(item => (

                                            <option
                                                key={`servico-${item.id}`}
                                                value={item.id}
                                            >
                                                {item.nome}
                                            </option>

                                        ))}

                                    </select>

                                )}

                            </div>

                        </div>

                        <div className="row">

                            <div className="col-md-6 form-group">

                                <label>Contato</label>

                                <input
                                    type="text"
                                    className="form-control"
                                    value={dados.contato}
                                    disabled={dados.nome !== ""}
                                    onChange={(e) =>
                                        this.setCampo("contato", e.target.value)
                                    }
                                />

                            </div>

                            <div className="col-md-6 form-group">

                                <label>E-mail</label>

                                <input
                                    type="email"
                                    className="form-control"
                                    value={dados.email}
                                    disabled={dados.nome !== ""}
                                    onChange={(e) =>
                                        this.setCampo("email", e.target.value)
                                    }
                                />

                            </div>

                        </div>

                        <div className="form-group">

                            <label>Mensagem</label>

                            <textarea
                                className="form-control"
                                rows="5"
                                style={{ resize: "none" }}
                                value={dados.mensagem}
                                onChange={(e) =>
                                    this.setCampo("mensagem", e.target.value)
                                }
                                disabled={dados.nome !== ""}
                            />

                        </div>

                    </div>

                </div>

                {/* ACOMPANHAMENTO */}
                <div className="card border-0 shadow-sm mb-4">

                    <div className="card-header bg-white">

                        <h5 className="mb-0 font-weight-bold">
                            <i className="fas fa-clipboard-check mr-2 text-success"></i>
                            Acompanhamento
                        </h5>

                    </div>

                    <div className="card-body">

                        <div className="row">

                            <div className="col-md-4 form-group">

                                <label>Status</label>

                                <select
                                    className="form-control"
                                    value={String(dados.statusContato ?? "0")}
                                    onChange={(e) =>
                                        this.setCampo(
                                            "statusContato",
                                            Number(e.target.value)
                                        )
                                    }
                                >

                                    {parametros.statusContato?.map(item => (

                                        <option
                                            key={`status-${item.enum}`}
                                            value={item.valor}
                                        >
                                            {item.descricao}
                                        </option>

                                    ))}

                                </select>

                            </div>

                            <div className="col-md-4 form-group">

                                <label>Prioridade</label>

                                <select
                                    className="form-control"
                                    value={dados.prioridade}
                                    onChange={(e) =>
                                        this.setCampo(
                                            "prioridade",
                                            parseInt(e.target.value, 10)
                                        )
                                    }
                                >

                                    {parametros.prioridade?.map(item => (

                                        <option
                                            key={`prioridade-${item.id}`}
                                            value={item.valor}
                                        >
                                            {item.nome}
                                        </option>

                                    ))}

                                </select>

                            </div>

                            <div className="col-md-4 form-group">

                                <label>Preferência de contato</label>

                                <select
                                    className="form-control"
                                    value={dados.preferenciaContato}
                                    onChange={(e) =>
                                        this.setCampo(
                                            "preferenciaContato",
                                            parseInt(e.target.value, 10)
                                        )
                                    }
                                >

                                    {parametros.preferenciaContato?.map(item => (

                                        <option
                                            key={`preferencia-${item.id}`}
                                            value={item.valor}
                                        >
                                            {item.nome}
                                        </option>

                                    ))}

                                </select>

                            </div>

                        </div>

                        <div className="row">

                            <div className="col-md-3 form-group">

                                <div className="custom-control custom-switch mt-4">

                                    <input
                                        type="checkbox"
                                        className="custom-control-input"
                                        id="chkEntrouContato"
                                        checked={dados.entrouContato}
                                        onChange={(e) =>
                                            this.setCampo(
                                                "entrouContato",
                                                e.target.checked
                                            )
                                        }
                                    />

                                    <label
                                        className="custom-control-label"
                                        htmlFor="chkEntrouContato"
                                    >
                                        Entrou em contato
                                    </label>

                                </div>

                            </div>

                            <div className="col-md-3 form-group">

                                <div className="custom-control custom-switch mt-4">

                                    <input
                                        type="checkbox"
                                        className="custom-control-input"
                                        id="chkVirouPaciente"
                                        checked={dados.virouPaciente}
                                        onChange={(e) =>
                                            this.setCampo(
                                                "virouPaciente",
                                                e.target.checked
                                            )
                                        }
                                    />

                                    <label
                                        className="custom-control-label"
                                        htmlFor="chkVirouPaciente"
                                    >
                                        Virou paciente
                                    </label>

                                </div>

                            </div>

                            <div className="col-md-3 form-group">

                                <label>Data contato</label>

                                <input
                                    type="date"
                                    className="form-control"
                                    value={dados.dataContato}
                                    onChange={(e) =>
                                        this.setCampo(
                                            "dataContato",
                                            e.target.value
                                        )
                                    }
                                />

                            </div>

                            <div className="col-md-3 form-group">

                                <label>Data retorno</label>

                                <input
                                    type="date"
                                    className="form-control"
                                    value={dados.dataRetorno}
                                    onChange={(e) =>
                                        this.setCampo(
                                            "dataRetorno",
                                            e.target.value
                                        )
                                    }
                                />

                            </div>

                        </div>

                        <div className="form-group">

                            <label>Observação interna</label>

                            <textarea
                                className="form-control"
                                rows="4"
                                style={{ resize: "none" }}
                                value={dados.observacaoInterna}
                                onChange={(e) =>
                                    this.setCampo(
                                        "observacaoInterna",
                                        e.target.value
                                    )
                                }
                            />

                        </div>

                    </div>

                </div>

                {/* INFORMAÇÕES TÉCNICAS */}
                <div className="card border-0 shadow-sm">

                    <div className="card-header bg-white">

                        <h5 className="mb-0 font-weight-bold">
                            <i className="fas fa-shield-alt mr-2 text-secondary"></i>
                            Informações técnicas
                        </h5>

                    </div>

                    <div className="card-body">

                        <div className="row">

                            <div className="col-md-4 form-group">

                                <label>Origem</label>

                                <input
                                    type="text"
                                    className="form-control bg-light"
                                    maxLength={50}
                                   disabled={dados.nome !== ""}
                                    value={dados.origem}
                                     onChange={(e) =>
                                        this.setCampo("origem", e.target.value)
                                    }
                                />

                            </div>

                            <div className="col-md-4 form-group">

                                <label>IP</label>

                                <input
                                    type="text"
                                    className="form-control bg-light"
                                    disabled
                                    value={dados.ip}
                                />

                            </div>

                            <div className="col-md-4 form-group">

                                <label>User Agent</label>

                                <textarea
                                    className="form-control bg-light"
                                    disabled
                                    rows="3"
                                    style={{ resize: "none" }}
                                    value={dados.userAgent}
                                />

                            </div>

                        </div>

                    </div>

                </div>

            </form>;

        return (

            <div
                className="modal fade"
                id="cadastroModal"
                data-keyboard="true"
                tabIndex="-1"
            >

                <div className="modal-dialog modal-xl modal-dialog-scrollable">

                    <div className="modal-content border-0 shadow-lg rounded-lg">

                        <div className="modal-header bg-white">

                            <h4 className="modal-title font-weight-bold">
                                <i className="fas fa-headset text-primary mr-2"></i>
                                Atendimento do Lead
                            </h4>

                            <button
                                type="button"
                                className="close"
                                data-dismiss="modal"
                                aria-label="Close"
                            >
                                <span aria-hidden="true">
                                    &times;
                                </span>
                            </button>

                        </div>

                        <div className="modal-body bg-light">

                            {!iniciando
                                ? form
                                : <LoadingIndicator />
                            }

                        </div>

                        <div className={`modal-footer ${aguarde ? "site-disabled" : ""}`}>

                            <button
                                type="button"
                                className="btn btn-light"
                                data-dismiss="modal"
                            >
                                Fechar
                            </button>

                            <button
                                type="button"
                                className="btn btn-primary"
                                onClick={this.salvar}
                                disabled={aguarde}
                            >

                                {aguarde ? (
                                    <>
                                        <span className="spinner-border spinner-border-sm mr-2"></span>
                                        Salvando...
                                    </>
                                ) : (
                                    <>
                                        <i className="fas fa-save mr-2"></i>
                                        Salvar
                                    </>
                                )}

                            </button>

                        </div>

                    </div>

                </div>

            </div>

        );

    }

}