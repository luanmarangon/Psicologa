import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Cadastro extends Component {

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
                tipoAnexo: 1,
                nome: "",
                arquivo: null,
                nomeArquivo: "",
                tamanhoArquivo: 0,
                observacao: "",
                prontuarioId: 0,
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
        let p = HTTPClient.get("Administrativo/ProntuarioAnexo/Obter?id=" + id)
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

    salvar = async () => {
        this.setState({ aguarde: true });

        this.state.dados.prontuarioId = this.state.pacienteProntuarioId;

        let dados = { ...this.state.dados };

        // Se arquivo for um objeto File, converte para base64
        if (dados.arquivo instanceof File) {
            try {
                dados.arquivo = await this.fileToBase64(dados.arquivo);
            } catch {
                showToastr({ type: "error", text: "Erro ao processar o arquivo." });
                this.setState({ aguarde: false });
                return;
            }
        }

        HTTPClient.post("Administrativo/ProntuarioAnexo/Salvar", dados)
            .then(r => r.json())
            .then(r => {
                if (r.success) {
                    this.props.onFechar(r.data);
                } else {
                    showToastr(r.messages);
                }
            })
            .catch(() => {
                showToastr({ type: "error", text: "Um erro ocorreu." });
            })
            .finally(() => {
                this.setState({ aguarde: false });
            });
    }

    fileToBase64 = (file) => new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(',')[1]);
        reader.onerror = reject;
        reader.readAsDataURL(file);
    });

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

                    </div>

                </div>

                <div className="card card-modern">

                    <div className="card-header border-0 bg-white">
                        <h3 className="card-title">
                            <i className="fas fa-paperclip mr-2 text-primary"></i>
                            Anexo
                        </h3>
                    </div>

                    <div className="card-body">

                        <div className="row">

                            {/* TIPO */}
                            <div className="col-md-4">
                                <div className="form-group">
                                    <label>Tipo do Documento</label>
                                    <select
                                        className="form-control form-control-modern"
                                        value={this.state.dados.tipoAnexo}
                                        onChange={(e) =>
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    tipoAnexo: parseInt(e.target.value)
                                                }
                                            })
                                        }
                                    >
                                        <option value="1">Documento Pessoal</option>
                                        <option value="2">Convênio</option>
                                        <option value="3">Termo de Consentimento</option>
                                        <option value="4">Contrato</option>
                                        <option value="5">Avaliação Psicológica</option>
                                        <option value="6">Encaminhamento Médico</option>
                                        <option value="7">Receita Médica</option>
                                        <option value="8">Exame</option>
                                        <option value="9">Relatório</option>
                                        <option value="99">Outro</option>
                                    </select>
                                </div>
                            </div>

                            {/* NOME */}
                            <div className="col-md-4">
                                <div className="form-group">
                                    <label>Nome do Documento</label>
                                    <input
                                        type="text"
                                        className="form-control form-control-modern"
                                        value={this.state.dados.nome || ""}
                                        onChange={(e) =>
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    nome: e.target.value
                                                }
                                            })
                                        }
                                    />
                                </div>
                            </div>

                            {/* ARQUIVO */}
                            <div className="col-md-4">
                                <div className="form-group">
                                    <label>Arquivo</label>
                                    <input
                                        type="file"
                                        className="form-control form-control-modern"
                                        accept=".pdf,.jpg,.jpeg,.png,.doc,.docx"
                                        onChange={(e) => {
                                            const file = e.target.files[0];
                                            if (!file) return;
                                            this.setState({
                                                dados: {
                                                    ...this.state.dados,
                                                    arquivo: file,
                                                    nomeArquivo: file.name,
                                                    tamanhoArquivo: file.size
                                                }
                                            });
                                        }}
                                    />
                                </div>
                            </div>

                        </div>

                        {/* OBSERVAÇÃO */}
                        <div className="form-group mb-0">
                            <label>Observações</label>
                            <textarea
                                className="form-control form-control-modern"
                                rows={4}
                                placeholder="Informações complementares..."
                                value={this.state.dados.observacao || ""}
                                onChange={(e) =>
                                    this.setState({
                                        dados: {
                                            ...this.state.dados,
                                            observacao: e.target.value
                                        }
                                    })
                                }
                            />
                        </div>

                    </div>

                </div>

            </form>;

        let modal =

            <div className="modal fade modal-modern" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-xl modal-dialog-scrollable">
                    <div className="modal-content border-0 shadow-lg">
                        <div className="modal-header bg-white border-bottom-0 pb-0">
                            <div>
                                <h4 className="modal-title font-weight-bold">Anexos do Paciente</h4>
                                <small className="text-muted">Gerencie os anexos do paciente.</small>
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