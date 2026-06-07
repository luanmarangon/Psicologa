import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Visualizar extends Component {

    constructor(props) {

        super(props);

        this.state = {
            iniciando: true,
        };
    }

    componentDidMount = () => {

        Promise.all([]).then(() => {
            this.setState({ iniciando: false });
        });

        $("#cadastroModal").modal('show');

        let that = this;

        $('#cadastroModal').on('hidden.bs.modal', function () {
            that.props.onFechar();
        });

    }

    componentWillUnmount = () => {
        $('#cadastroModal').modal('hide');
    }

    getDataUrl = () => {
        const { sessaoSelecionada } = this.props;
        if (!sessaoSelecionada || !sessaoSelecionada.arquivo) return null;
        return `data:${sessaoSelecionada.mimeType};base64,${sessaoSelecionada.arquivo}`;
    }

    isImagem = () => {
        const { sessaoSelecionada } = this.props;
        return sessaoSelecionada &&
            sessaoSelecionada.mimeType &&
            sessaoSelecionada.mimeType.startsWith('image/');
    }

    isPdf = () => {
        const { sessaoSelecionada } = this.props;
        return sessaoSelecionada &&
            sessaoSelecionada.mimeType === 'application/pdf';
    }

    formatarTamanho = (bytes) => {
        if (!bytes) return '';
        if (bytes < 1024) return `${bytes} B`;
        if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
        return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    }



    render() {

        const { sessaoSelecionada, pacienteSelecionado } = this.props;
        const dataUrl = this.getDataUrl();

        let form =

            <form role="form">

                {/* PACIENTE */}
                <div className="paciente-box mb-4">
                    <div className="d-flex justify-content-between align-items-center">
                        <div>
                            <div className="paciente-subtitulo">Paciente</div>
                            <div className="paciente-nome">
                                {pacienteSelecionado ? pacienteSelecionado.nome : '-'}
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

                        <div className="row align-items-end">

                            {/* TIPO */}
                            <div className="col-md-4">
                                <div className="form-group">
                                    <label>Tipo do Documento</label>
                                    <input
                                        type="text"
                                        className="form-control form-control-modern"
                                        value={sessaoSelecionada ? sessaoSelecionada.tipoAnexoDescricao : '-'}
                                        disabled={true}
                                        readOnly
                                    />
                                </div>
                            </div>

                            {/* NOME */}
                            <div className="col-md-4">
                                <div className="form-group">
                                    <label>Nome do Documento</label>
                                    <input
                                        type="text"
                                        className="form-control form-control-modern"
                                        value={sessaoSelecionada ? sessaoSelecionada.nome : '-'}
                                        disabled={true}
                                        readOnly
                                    />
                                </div>
                            </div>

                            {/* ARQUIVO + DOWNLOAD */}
                            {dataUrl && !this.isPdf() && (
                                <div className="col-md-4">
                                    <div className="form-group">
                                        <label></label>
                                        <div>
                                            <a href={dataUrl} download={sessaoSelecionada.nomeArquivo} className="btn btn-outline-primary btn-modern" style={{ display: 'inline-flex', alignItems: 'center', gap: '8px' }}>
                                                <i className="fas fa-download" style={{ lineHeight: 1 }}></i>Baixar
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            )}

                        </div>

                        {/* PREVIEW IMAGEM */}
                        {dataUrl && this.isImagem() && (
                            <div className="form-group">
                                <label>Visualização</label>
                                <div className="border rounded p-2 text-center bg-light">
                                    <img
                                        src={dataUrl}
                                        alt={sessaoSelecionada.nomeArquivo}
                                        style={{ maxWidth: '100%', maxHeight: '500px', objectFit: 'contain' }}
                                    />
                                </div>
                            </div>
                        )}

                        {/* PREVIEW PDF */}
                        {dataUrl && this.isPdf() && (
                            <div className="form-group">
                                <label>Visualização</label>
                                <iframe
                                    src={dataUrl}
                                    width="100%"
                                    height="600px"
                                    title={sessaoSelecionada.nomeArquivo}
                                    style={{ border: '1px solid #dee2e6', borderRadius: '4px' }}
                                />
                            </div>
                        )}

                        {/* ARQUIVO SEM PREVIEW */}
                        {dataUrl && !this.isImagem() && !this.isPdf() && (
                            <div className="form-group">
                                <div className="alert alert-info">
                                    <i className="fas fa-info-circle mr-2"></i>
                                    Visualização não disponível para este tipo de arquivo. Utilize o botão acima para baixar.
                                </div>
                            </div>
                        )}

                        {/* SEM ARQUIVO */}
                        {!dataUrl && (
                            <div className="form-group">
                                <div className="alert alert-warning">
                                    <i className="fas fa-exclamation-triangle mr-2"></i>
                                    Nenhum arquivo disponível.
                                </div>
                            </div>
                        )}

                    </div>

                </div>

            </form>;

        let modal =

            <div className="modal fade modal-modern" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-xl modal-dialog-scrollable">
                    <div className="modal-content border-0 shadow-lg">
                        <div className="modal-header bg-white border-bottom-0 pb-0">
                            <div>
                                <h4 className="modal-title font-weight-bold">Visualizar Anexo</h4>
                                <small className="text-muted">Detalhes do anexo do paciente.</small>
                            </div>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body bg-light">
                            {!this.state.iniciando ? form : <LoadingIndicator />}
                        </div>
                        <div className="modal-footer bg-white border-top-0">
                            <button type="button" className="btn btn-light btn-modern" data-dismiss="modal">Fechar</button>
                        </div>
                    </div>
                </div>
            </div>;

        return modal;
    }
}