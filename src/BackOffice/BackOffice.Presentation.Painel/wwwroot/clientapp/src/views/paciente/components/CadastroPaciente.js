import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class CadastroPaciente extends Component {

    constructor(props) {

        super(props);

        this.state = {

            iniciando: true,
            aguarde: false,

            trocarResponsavel: false,

            dados: {

                id: 0,
                pessoaId: 0,
                pessoaNome: '',

                dataPrimeiraSessao: '',

                ativo: true,

                observacoes: '',

                contatoEmergenciaNome: '',
                contatoEmergenciaTelefone: '',

                responsavelId: null,
                responsavelNome: '',

                dataCriacao: null,
                dataAtualizacao: null
            },

            endereco: {},

            contato: {
                tipo: '',
                contato: '',
                observacao: ''
            },

            contatos: [],
            tipos: []

        };
    }

    componentDidMount = () => {

        let promiseEdicao = null;

        if (!isEmpty(this.props.idEdicao)) {

            promiseEdicao = this.obter(this.props.idEdicao);

        }

        Promise.all([promiseEdicao]).then(() => {

            this.setState({
                iniciando: false
            }, () => {

                this.inicializarSelect2Responsavel();

            });

        });

        $("#cadastroModal").modal('show');

        let that = this;

        $('#cadastroModal').on('hidden.bs.modal', function () {

            that.props.onFechar();

        });

    }

    componentDidUpdate = () => {

        let that = this;

        $("#txtContatoEmergenciaTelefone").inputmask("(99) 99999-9999");

    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');

    }

    obter = (id) => {
        let p = HTTPClient.get("Administrativo/Paciente/Obter?id=" + id)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    dados: { ...this.state.dados, ...r.data }
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

    inicializarSelect2Responsavel = () => {

        if (!$("#selResponsavel").length) return;

        if ($("#selResponsavel").hasClass("select2-hidden-accessible")) {

            $("#selResponsavel").select2("destroy");

        }

        $("#selResponsavel").select2({

            language: "pt-BR",

            placeholder: "Digite para buscar o responsável...",

            minimumInputLength: 2,

            ajax: {

                url: (params) =>
                    resolveClientURL(
                        "Administrativo/Pessoa/ConsultarPessoaAutoComplete?q=" +
                        encodeURIComponent(params.term)
                    ),

                dataType: 'json',

                delay: 300,

                processResults: (data) => ({

                    results: data.map(item => ({

                        id: item.dados.id,
                        text: item.dados.nome.toUpperCase(),
                        nome: item.dados.nome.toUpperCase(),
                        documento: item.dados.docIdNro || '',
                        cidade: item.dados.cidade || ''

                    }))

                })

            },

            templateResult: (data) => {

                if (!data.id) return data.text;

                return $(

                    '<div>' +
                    '<div><strong>' + data.nome + '</strong></div>' +
                    (data.documento
                        ? '<div class="small ">' + data.documento + '</div>'
                        : '') +
                    // (data.cidade
                    //     ? '<div class="small text-muted">' + data.cidade + '</div>'
                    //     : '') +
                    '</div>'

                );

            },

            templateSelection: (data) => data.nome || data.text

        });

        $("#selResponsavel")
            .off("select2:select")
            .on("select2:select", (e) => {

                const item = e.params.data;

                this.setState(prev => ({

                    trocarResponsavel: false,

                    dados: {

                        ...prev.dados,

                        responsavelId: item.id,
                        responsavelNome: item.nome

                    }

                }));

            });

        if (
            this.state.dados.responsavelId > 0 &&
            this.state.dados.responsavelNome
        ) {

            const option = new Option(
                this.state.dados.responsavelNome,
                this.state.dados.responsavelId,
                true,
                true
            );

            $("#selResponsavel")
                .append(option)
                .trigger("change");

            $("#selResponsavel")
                .next(".select2-container")
                .hide();
        }
    }

    trocarResponsavelHandler = () => {

        $("#selResponsavel")
            .val(null)
            .trigger("change");

        $("#selResponsavel")
            .next(".select2-container")
            .show();

        this.setState(prev => ({

            trocarResponsavel: true,

            dados: {

                ...prev.dados,

                responsavelId: 0,
                responsavelNome: ''

            }

        }));

    }

    // salvar = () => {

    //     this.setState({
    //         aguarde: true
    //     });

    //     var pessoaDados = {

    //         dados: this.state.dados,
    //         endereco: this.state.endereco,
    //         contatos: this.state.contatos,
    //         tipos: this.state.tipos

    //     };

    //     HTTPClient.post("Administrativo/Paciente/Salvar", pessoaDados)

    //         .then(r => r.json())

    //         .then(r => {

    //             if (r.success) {

    //                 this.props.onFechar(r.data);

    //             }
    //             else {

    //                 showToastr(r.messages);

    //             }

    //         })

    //         .catch(() => {

    //             showToastr({
    //                 type: "error",
    //                 text: "Um erro ocorreu."
    //             });

    //         })

    //         .finally(() => {

    //             this.setState({
    //                 aguarde: false
    //             });

    //         });

    // }

    salvar = () => {

        this.setState({
            aguarde: true
        });

        HTTPClient.post("Administrativo/Paciente/Salvar", this.state.dados)

            .then(r => r.json())

            .then(r => {

                if (r.success) {

                    this.props.onFechar(r.data);

                } else {

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

            <form role="form">

                {/* PACIENTE */}
                <div className="paciente-box mb-4">

                    <div className="d-flex justify-content-between align-items-center">

                        <div>

                            <div className="paciente-subtitulo">
                                Paciente
                            </div>

                            <div className="paciente-nome">
                                {this.state.dados.pessoaNome || '-'}
                            </div>

                        </div>

                        <div>

                            {
                                this.state.dados.ativo
                                    ?
                                    <span className="badge badge-success p-2">
                                        <i className="fas fa-check-circle mr-1"></i>
                                        Ativo
                                    </span>
                                    :
                                    <span className="badge badge-danger p-2">
                                        <i className="fas fa-ban mr-1"></i>
                                        Inativo
                                    </span>
                            }

                        </div>

                    </div>

                </div>

                {/* DADOS GERAIS */}
                <div className="card card-modern">

                    <div className="card-header border-0 bg-white">

                        <h3 className="card-title">
                            <i className="fas fa-user mr-2 text-primary"></i>
                            Dados Gerais
                        </h3>

                    </div>

                    <div className="card-body">

                        <div className="row">

                            {/* MATRICULA*/}
                            <div className="col-md-3">
                                <div className="form-group">
                                    <label>Matrícula</label>
                                    <input type="text" className="form-control form-control-modern" value={this.state.dados.matricula && this.state.dados.matricula.length > 0 ? this.state.dados.matricula : 'Nova matrícula'} disabled />
                                </div>
                            </div>

                            {/* PRIMEIRA SESSÃO */}
                            <div className="col-md-3">

                                <div className="form-group">

                                    <label htmlFor="txtDataPrimeiraSessao">
                                        Primeira Sessão
                                    </label>

                                    <input
                                        type="date"
                                        className="form-control form-control-modern"
                                        id="txtDataPrimeiraSessao"
                                        // value={formatarDataPtBrToInputDate(this.state.dados.dataPrimeiraSessao)}
                                        value={this.state.dados.dataPrimeiraSessao
                                            ? this.state.dados.dataPrimeiraSessao.split("T")[0]
                                            : ""}
                                        onChange={(e) =>
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    dataPrimeiraSessao: e.target.value
                                                }
                                            })
                                        }
                                    />

                                </div>

                            </div>

                            {/* ATIVO */}
                            <div className="col-md-3">
                                <div className="form-group">
                                    <label>Status</label>
                                    <select
                                        className="form-control form-control-modern"
                                        value={this.state.dados.ativo}
                                        onChange={(e) =>
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    ativo: e.target.value === "true"
                                                }
                                            })
                                        }
                                    >
                                        <option value={true}>Ativo</option>
                                        <option value={false}>Inativo</option>
                                    </select>
                                </div>
                            </div>



                        </div>

                        {/* OBSERVAÇÕES */}
                        <div className="form-group mb-0">

                            <label htmlFor="txtObservacoes">
                                Observações
                            </label>

                            <textarea
                                className="form-control form-control-modern"
                                rows={5}
                                id="txtObservacoes"
                                placeholder="Digite observações importantes do paciente..."
                                value={this.state.dados.observacoes || ''}
                                onChange={(e) =>
                                    this.setState({
                                        dados: {
                                            ...this.state.dados,
                                            observacoes: e.target.value
                                        }
                                    })
                                }
                            />

                        </div>

                    </div>

                </div>

                {/* CONTATO EMERGÊNCIA */}
                <div className="card card-modern mt-4">

                    <div className="card-header border-0 bg-white">

                        <h3 className="card-title">

                            <i className="fas fa-phone-alt mr-2 text-danger"></i>
                            Contato de Emergência

                        </h3>

                    </div>

                    <div className="card-body">

                        <div className="row">

                            {/* NOME */}
                            <div className="col-md-8">

                                <div className="form-group">

                                    <label htmlFor="txtContatoEmergenciaNome">
                                        Nome do Contato
                                    </label>

                                    <input
                                        type="text"
                                        className="form-control form-control-modern"
                                        id="txtContatoEmergenciaNome"
                                        autoComplete="off"
                                        placeholder="Nome completo"
                                        value={this.state.dados.contatoEmergenciaNome || ''}
                                        onChange={(e) =>
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    contatoEmergenciaNome: e.target.value
                                                }
                                            })
                                        }
                                    />

                                </div>

                            </div>

                            {/* TELEFONE */}
                            <div className="col-md-4">

                                <div className="form-group">

                                    <label htmlFor="txtContatoEmergenciaTelefone">
                                        Telefone
                                    </label>

                                    <input
                                        type="text"
                                        className="form-control form-control-modern"
                                        id="txtContatoEmergenciaTelefone"
                                        autoComplete="off"
                                        placeholder="(00) 00000-0000"
                                        value={this.state.dados.contatoEmergenciaTelefone || ''}
                                        onChange={(e) =>
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    contatoEmergenciaTelefone: e.target.value
                                                }
                                            })
                                        }
                                    />

                                </div>

                            </div>

                        </div>

                    </div>

                </div>

                {/* RESPONSÁVEL */}
                <div className="card card-modern mt-4">

                    <div className="card-header border-0 bg-white">

                        <h3 className="card-title">

                            <i className="fas fa-user-shield mr-2 text-info"></i>
                            Responsável

                        </h3>

                    </div>

                    <div className="card-body">

                        <div className="form-group mb-0">

                            <label htmlFor="selResponsavel">
                                Responsável pelo paciente
                            </label>

                            {
                                this.state.dados.responsavelId > 0 &&
                                    !this.state.trocarResponsavel
                                    ? (

                                        <div className="responsavel-box">

                                            <div className="d-flex justify-content-between align-items-center">

                                                <div>

                                                    <div className="responsavel-label">
                                                        Responsável selecionado
                                                    </div>

                                                    <div className="responsavel-nome">
                                                        {this.state.dados.responsavelNome}
                                                    </div>

                                                </div>

                                                <button
                                                    type="button"
                                                    className="btn btn-outline-primary btn-modern"
                                                    onClick={this.trocarResponsavelHandler}
                                                >

                                                    <i className="fas fa-sync-alt mr-1"></i>
                                                    Trocar

                                                </button>

                                            </div>

                                        </div>

                                    )
                                    :
                                    (
                                        <select
                                            id="selResponsavel"
                                            className="form-control"
                                            style={{ width: '100%' }}
                                        />
                                    )
                            }

                        </div>

                    </div>

                </div>

            </form>;

        let modal =

            <div
                className="modal fade modal-modern"
                id="cadastroModal"
                data-keyboard="true"
                tabIndex="-1"
            >

                <div className="modal-dialog modal-xl modal-dialog-scrollable">

                    <div className="modal-content border-0 shadow-lg">

                        <div className="modal-header bg-white border-bottom-0 pb-0">

                            <div>

                                <h4 className="modal-title font-weight-bold">

                                    Cadastro de Paciente

                                </h4>

                                <small className="text-muted">
                                    Gerencie os dados do paciente
                                </small>

                            </div>

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

                            {
                                !this.state.iniciando
                                    ? form
                                    : <LoadingIndicator />
                            }

                        </div>

                        <div className={
                            "modal-footer bg-white border-top-0 " +
                            (this.state.aguarde ? "site-disabled" : "")
                        }>

                            <button
                                type="button"
                                className="btn btn-light btn-modern"
                                data-dismiss="modal"
                            >
                                Cancelar
                            </button>

                            <button
                                type="button"
                                className="btn btn-primary btn-modern px-4"
                                onClick={this.salvar}
                            >

                                {
                                    !this.state.aguarde
                                        ?
                                        <span>

                                            <i className="fas fa-save mr-2"></i>
                                            Salvar

                                        </span>
                                        :
                                        <span>

                                            <i className="fas fa-circle-notch fa-spin mr-2"></i>
                                            Salvando

                                        </span>
                                }

                            </button>

                        </div>

                    </div>

                </div>

            </div>;

        return modal;
    }
}