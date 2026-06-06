import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class CadastroProntuario extends Component {

    constructor(props) {

        super(props);

        this.state = {

            iniciando: true,
            aguarde: false,

            trocarResponsavel: false,
            pacienteNome: '',
            pacienteMatricula: '',
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
            }
        };
    }

    componentDidMount = () => {

        let promiseEdicao = null;
        let promisePaciente = null;
        if (!isEmpty(this.props.idEdicao)) {

            promiseEdicao = this.obter(this.props.idEdicao);
            promisePaciente = this.obterPaciente(this.props.idEdicao);
        }

        Promise.all([promiseEdicao, promisePaciente]).then(() => {

            this.setState({
                iniciando: false
            }, () => {

                // this.inicializarSelect2Responsavel();

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
        let p = HTTPClient.get("Administrativo/Prontuario/Obter?id=" + id)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    dados: {
                        ...this.state.dados,
                        ...r.data
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

    obterPaciente = (id) => {
        console.log(id);
        let p = HTTPClient.get("Administrativo/Paciente/Obter?id=" + id)
            .then(r => r.json())
            .then(r => {
                this.setState({
                    pacienteNome: r.data.pessoaNome,
                    pacienteMatricula: r.data.matricula
                });
                console.log(r.data.pessoaNome);
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

        HTTPClient.post("Administrativo/Prontuario/Salvar", this.state.dados)

            .then(r => r.json())

            // .then(r => {

            //     if (r.success) {

            //         this.props.onFechar(r.data);
            //         showToastr(r.messages);

            //     } else {

            //         showToastr(r.messages);

            //     }

            // })

            // .catch(() => {

            //     showToastr({
            //         type: "error",
            //         text: "Um erro ocorreu."
            //     });

            // })
            .then(r => {

    console.log("SUCCESS:", r.success);

    if (r.success) {

        console.log("ANTES onFechar");

        this.props.onFechar(r.data);

        console.log("DEPOIS onFechar");

        showToastr(r.messages);
    }
})
.catch(e => {

    console.error("ERRO:", e);

});

            // .finally(() => {

            //     this.setState({
            //         aguarde: false
            //     });

            // });

    }

    encerrar = () => {
    }

    render() {

        let form =

            <form role="form">

                {/* PACIENTE */}
                <div className="paciente-box mb-4">
                    <div className="d-flex justify-content-between align-items-center">
                        <div>
                            <div className="paciente-subtitulo">Paciente</div>
                            <div className="paciente-nome">{this.state.pacienteNome}</div>
                        </div>
                        <div>
                            {this.state.dados.ativo ?
                                <span className="badge badge-success p-2"><i className="fas fa-check-circle mr-1"></i>Ativo</span>
                                :
                                <span className="badge badge-danger p-2"><i className="fas fa-ban mr-1"></i>Inativo</span>
                            }
                        </div>
                    </div>
                </div>

                {/* DADOS GERAIS */}
                <div className="card card-modern">
                    <div className="card-body">
                        {/* QUEIXA INICIAL */}
                        <div className="form-group mb-0">

                            <label htmlFor="txtqueixaPrincipal">Queixa Inicial</label>
                            <textarea className="form-control form-control-modern" rows={2} id="txtqueixaPrincipal"
                                placeholder="Digite a queixa inicial do paciente..." value={this.state.dados.queixaPrincipal || ''}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, queixaPrincipal: e.target.value } })} />
                        </div>

                        {/* OBJETIVO TRATAMENTO */}
                        <div className="form-group mb-0">

                            <label htmlFor="txtObjetivoTratamento">Objetivo do Tratamento</label>
                            <textarea className="form-control form-control-modern" rows={2} id="txtObjetivoTratamento"
                                placeholder="Digite o objetivo do tratamento do paciente..." value={this.state.dados.objetivoTratamento || ''}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, objetivoTratamento: e.target.value } })} />
                        </div>

                        {/* OBSERVACOES INICIAIS */}
                        <div className="form-group mb-0">
                            <label htmlFor="txtObservacoesIniciais">Observações Iniciais</label>
                            <textarea className="form-control form-control-modern" rows={2} id="txtObservacoesIniciais"
                                placeholder="Digite as observações iniciais do paciente..." value={this.state.dados.observacoesIniciais || ''}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, observacoesIniciais: e.target.value } })} />
                        </div>

                        {/* HISTÓRICO FAMILIAR */}
                        <div className="form-group mb-0">
                            <label htmlFor="txtHistoricoFamiliar">Histórico Familiar</label>
                            <textarea className="form-control form-control-modern" rows={2} id="txtHistoricoFamiliar"
                                placeholder="Digite o histórico familiar do paciente..." value={this.state.dados.historicoFamiliar || ''}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, historicoFamiliar: e.target.value } })} />
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
                                <h4 className="modal-title d-flex align-items-center">
                                    {/* <span className="bg-primary text-white rounded-circle p-2 mr-2"> */}
                                    <i className="fas fa-brain mr-2 text-primary"></i>
                                    {/* </span> */}
                                    <span>Prontuário do Paciente</span>
                                </h4>
                                {/* <small className="text-muted">Gerencie os dados do paciente</small> */}
                            </div>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>

                        <div className="modal-body bg-light">
                            {!this.state.iniciando ? form : <LoadingIndicator />}
                        </div>

                        <div className={"modal-footer bg-white border-top-0 " + (this.state.aguarde ? "site-disabled" : "")}>

                            <button type="button" className="btn btn-warning btn-modern mr-auto" onClick={this.encerrar}>Encerrar</button>
                            <button type="button" className="btn btn-light btn-modern" data-dismiss="modal">Cancelar</button>
                            <button type="button" className="btn btn-primary btn-modern px-4" onClick={this.salvar}>
                                {!this.state.aguarde
                                    ?
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