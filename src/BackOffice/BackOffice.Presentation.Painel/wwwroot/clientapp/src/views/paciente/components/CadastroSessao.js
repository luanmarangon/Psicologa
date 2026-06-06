import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class CadastroSessao extends Component {

    constructor(props) {

        super(props);

        this.state = {
            iniciando: true,
            aguarde: false,

            pacienteId: 0,
            pacienteNome: '',
            pacienteMatricula: '',
            prontuarioAtivo: false,
            pacienteProntuarioId: 0,
            dados: {
                id: 0,
                dataSessao: null,
                horaInicio: null,
                horaFim: null,
                tipoAtendimento: 1,
                evolucao: '',
                prontuarioId: 0,
                // tecnicas: '',
                // proximosPassos: '',
            },
        };
    }

    componentDidMount = () => {

        let promiseEdicao = null;




        if (this.props.pacienteSelecionado) {

            this.setState({
                pacienteId: this.props.pacienteSelecionado.id,
                pacienteNome: this.props.pacienteSelecionado.nome,
                pacienteMatricula: this.props.pacienteSelecionado.matricula,
                prontuarioAtivo: this.props.pacienteSelecionado.ativo,
                pacienteProntuarioId: this.props.pacienteSelecionado.prontuarioId

            });
            console.log(this.props.pacienteSelecionado);

        }
        if (!isEmpty(this.props.sessaoSelecionada)) {
            console.log(this.props.sessaoSelecionada);

            promiseEdicao = this.obter(this.props.sessaoSelecionada.id);

        }

        Promise.all([promiseEdicao]).then(() => {

            this.setState({
                iniciando: false
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
        let p = HTTPClient.get("Administrativo/Prontuario/ObterSessao?id=" + id)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    dados: r.data
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

    // inicializarSelect2Responsavel = () => {

    //     if (!$("#selResponsavel").length) return;

    //     if ($("#selResponsavel").hasClass("select2-hidden-accessible")) {

    //         $("#selResponsavel").select2("destroy");

    //     }

    //     $("#selResponsavel").select2({

    //         language: "pt-BR",

    //         placeholder: "Digite para buscar o responsável...",

    //         minimumInputLength: 2,

    //         ajax: {

    //             url: (params) =>
    //                 resolveClientURL(
    //                     "Administrativo/Pessoa/ConsultarPessoaAutoComplete?q=" +
    //                     encodeURIComponent(params.term)
    //                 ),

    //             dataType: 'json',

    //             delay: 300,

    //             processResults: (data) => ({

    //                 results: data.map(item => ({

    //                     id: item.dados.id,
    //                     text: item.dados.nome.toUpperCase(),
    //                     nome: item.dados.nome.toUpperCase(),
    //                     documento: item.dados.docIdNro || '',
    //                     cidade: item.dados.cidade || ''

    //                 }))

    //             })

    //         },

    //         templateResult: (data) => {

    //             if (!data.id) return data.text;

    //             return $(

    //                 '<div>' +
    //                 '<div><strong>' + data.nome + '</strong></div>' +
    //                 (data.documento
    //                     ? '<div class="small ">' + data.documento + '</div>'
    //                     : '') +
    //                 // (data.cidade
    //                 //     ? '<div class="small text-muted">' + data.cidade + '</div>'
    //                 //     : '') +
    //                 '</div>'

    //             );

    //         },

    //         templateSelection: (data) => data.nome || data.text

    //     });

    //     $("#selResponsavel")
    //         .off("select2:select")
    //         .on("select2:select", (e) => {

    //             const item = e.params.data;

    //             this.setState(prev => ({

    //                 trocarResponsavel: false,

    //                 dados: {

    //                     ...prev.dados,

    //                     responsavelId: item.id,
    //                     responsavelNome: item.nome

    //                 }

    //             }));

    //         });

    //     if (
    //         this.state.dados.responsavelId > 0 &&
    //         this.state.dados.responsavelNome
    //     ) {

    //         const option = new Option(
    //             this.state.dados.responsavelNome,
    //             this.state.dados.responsavelId,
    //             true,
    //             true
    //         );

    //         $("#selResponsavel")
    //             .append(option)
    //             .trigger("change");

    //         $("#selResponsavel")
    //             .next(".select2-container")
    //             .hide();
    //     }
    // }

    // trocarResponsavelHandler = () => {

    //     $("#selResponsavel")
    //         .val(null)
    //         .trigger("change");

    //     $("#selResponsavel")
    //         .next(".select2-container")
    //         .show();

    //     this.setState(prev => ({

    //         trocarResponsavel: true,

    //         dados: {

    //             ...prev.dados,

    //             responsavelId: 0,
    //             responsavelNome: ''

    //         }

    //     }));

    // }
    salvar = () => {
        this.setState({
            aguarde: true
        });

        this.state.dados.prontuarioId = this.state.pacienteProntuarioId;



        HTTPClient.post("Administrativo/Prontuario/EvoluirSessao", this.state.dados)

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
                                {this.state.pacienteNome || '-'}
                            </div>

                        </div>

                        <div>

                            {
                                this.state.prontuarioAtivo
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

                            {/* PRIMEIRA SESSÃO */}
                            <div className="col-md-3">
                                <div className="form-group">
                                    <label htmlFor="txtDataSessao">Data da Sessão</label>
                                    <input type="date" className="form-control form-control-modern" id="txtDataSessao"
                                        value={this.state.dados.dataSessao ? this.state.dados.dataSessao.split("T")[0] : ""}
                                        onChange={(e) => this.setState({ dados: { ...this.state.dados, dataSessao: e.target.value } })} />
                                </div>
                            </div>

                            {/*INICIO SESSAO */}
                            <div className="col-md-3">
                                <div className="form-group">
                                    <label htmlFor="txtInicioSessao">Início da Sessão</label>
                                    <input type="time" className="form-control form-control-modern" id="txtInicioSessao"
                                        value={this.state.dados.horaInicio ? this.state.dados.horaInicio : ""}
                                        onChange={(e) => this.setState({ dados: { ...this.state.dados, horaInicio: e.target.value } })} />
                                </div>
                            </div>

                            {/*FIM SESSAO */}
                            <div className="col-md-3">
                                <div className="form-group">
                                    <label htmlFor="txtFimSessao">Fim da Sessão</label>
                                    <input type="time" className="form-control form-control-modern" id="txtFimSessao"
                                        value={this.state.dados.horaFim ? this.state.dados.horaFim : ""}
                                        onChange={(e) => this.setState({ dados: { ...this.state.dados, horaFim: e.target.value } })} />
                                </div>
                            </div>

                            {/* TIPO DE ATENDIMENTO */}
                            <div className="col-md-3">
                                <div className="form-group">
                                    <label>Tipo de Atendimento</label>
                                    <select
                                        className="form-control form-control-modern"
                                        value={this.state.dados.tipoAtendimento}
                                        onChange={(e) =>
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    tipoAtendimento: parseInt(e.target.value)
                                                }
                                            })
                                        }
                                    >
                                        <option value="1">Presencial</option>
                                        <option value="2">Online</option>
                                    </select>
                                </div>
                            </div>



                        </div>

                        {/* EVOLUÇÃO */}
                        <div className="form-group mb-0">
                            <label htmlFor="txtEvolucao">Evolução e Observações</label>
                            <textarea
                                className="form-control form-control-modern"
                                rows={5}
                                id="txtEvolucao"
                                placeholder="Digite a evolução e observações importantes do paciente..."
                                value={this.state.dados.evolucao || ''}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, evolucao: e.target.value } })} />
                        </div>

                        {/*TECNICAS APLICADAS*/}
                        {/* <div className="form-group mb-0">
                            <label htmlFor="txtTecnicas">Técnicas Aplicadas</label>
                            <textarea
                                className="form-control form-control-modern"
                                rows={2}
                                id="txtTecnicas"
                                placeholder="Digite as técnicas aplicadas durante a sessão..."
                                value={this.state.dados.tecnicas || ''}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, tecnicas: e.target.value } })} />
                        </div> */}

                        {/*PROXIMOS PASSOS*/}
                        {/* <div className="form-group mb-0">
                            <label htmlFor="txtProximosPassos">Próximos Passos</label>
                            <textarea
                                className="form-control form-control-modern"
                                rows={2}
                                id="txtProximosPassos"
                                placeholder="Digite os próximos passos para o paciente..."
                                value={this.state.dados.proximosPassos || ''}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, proximosPassos: e.target.value } })} />
                        </div> */}


                    </div>

                </div>



            </form>;

        let modal =

            <div className="modal fade modal-modern" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-xl modal-dialog-scrollable">
                    <div className="modal-content border-0 shadow-lg">
                        <div className="modal-header bg-white border-bottom-0 pb-0">
                            <div>
                                <h4 className="modal-title font-weight-bold">Sessão</h4>
                                <small className="text-muted">Gerencie os dados da sessão do paciente.</small>
                            </div>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body bg-light">
                            {!this.state.iniciando ? form : <LoadingIndicator />}
                        </div>
                        <div className={"modal-footer bg-white border-top-0 " + (this.state.aguarde ? "site-disabled" : "")}>

                            <button type="button" className="btn btn-light btn-modern" data-dismiss="modal">Cancelar</button>
                            <button type="button" className="btn btn-primary btn-modern px-4" onClick={this.salvar}>
                                {!this.state.aguarde ?
                                    <span><i className="fas fa-save mr-2"></i>Salvar</span>
                                    :
                                    <span><i className="fas fa-circle-notch fa-spin mr-2"></i>Salvando</span>
                                }
                            </button>
                        </div>
                    </div>
                </div>
            </div>;

        return modal;
    }
}