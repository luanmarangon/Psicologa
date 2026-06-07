import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';
import JsonViewer from '../../../components/JsonViewer';

export default class Cadastro extends Component {

    constructor(props) {

        super(props);

        this.state = {
            iniciando: true,
            dados: {},
        };
    }

    componentDidMount = () => {

        let promiseEdicao = null;

        if (!isEmpty(this.props.logSelecionado)) {
            promiseEdicao = this.obter(this.props.logSelecionado);
        }

        Promise.all([promiseEdicao]).then(() => {
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

    obter = (id) => {
        let p = HTTPClient.get("Administrativo/LogAplicacao/Obter?id=" + id)
            .then(r => r.json())
            .then(r => {
                this.setState({ dados: r.data });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Um erro ocorreu." });
            });
        return p;
    }

    renderCampo = (label, valor, colSize = 'col-md-4') => (
        <div className={colSize}>
            <div className="form-group mb-3">
                <label style={{ fontSize: '11px', textTransform: 'uppercase', letterSpacing: '0.5px', color: '#6c757d', marginBottom: '4px' }}>
                    {label}
                </label>
                <input
                    type="text"
                    className="form-control form-control-modern"
                    value={valor || '-'}
                    disabled
                    readOnly
                />
            </div>
        </div>
    )

    renderJson = (label, valor) => {
        let conteudo = '-';
        if (valor) {
            try {
                conteudo = JSON.stringify(JSON.parse(valor), null, 2);
            } catch {
                conteudo = valor;
            }
        }
        return (
            <div className="col-md-12">
                <div className="form-group mb-3">
                    <label style={{ fontSize: '11px', textTransform: 'uppercase', letterSpacing: '0.5px', color: '#6c757d', marginBottom: '4px' }}>
                        {label}
                    </label>
                    <pre
                        className="form-control form-control-modern"
                        style={{
                            height: 'auto',
                            minHeight: '80px',
                            maxHeight: '200px',
                            overflowY: 'auto',
                            fontSize: '12px',
                            fontFamily: 'monospace',
                            whiteSpace: 'pre-wrap',
                            wordBreak: 'break-all',
                            backgroundColor: '#f8f9fa',
                            margin: 0,
                        }}
                    >
                        {conteudo}
                    </pre>
                </div>
            </div>
        );
    }

    getOperacaoBadge = (operacao) => {
        const map = {
            'INSERT': 'success',
            'UPDATE': 'warning',
            'DELETE': 'danger',
        };
        const upper = (operacao || '').toUpperCase();
        const color = map[upper] || 'secondary';
        return (
            <span className={`badge badge-${color}`} style={{ fontSize: '13px', padding: '6px 12px' }}>
                {operacao || '-'}
            </span>
        );
    }

    render() {

        const { dados } = this.state;

        let form =

            <form role="form">

                {/* ENTIDADE */}
                <div className="paciente-box mb-4">
                    <div className="d-flex align-items-center" style={{ gap: '16px' }}>
                        <div>
                            <div className="paciente-subtitulo">Entidade</div>
                            <div className="paciente-nome">{dados.entidade || '-'}</div>
                        </div>
                        {dados.entidadeId && (
                            <div style={{ borderLeft: '1px solid #dee2e6', paddingLeft: '16px' }}>
                                <div className="paciente-subtitulo">ID</div>
                                <div className="paciente-nome">{dados.entidadeId}</div>
                            </div>
                        )}
                        {dados.operacao && (
                            <div style={{ marginLeft: 'auto' }}>
                                {this.getOperacaoBadge(dados.operacao)}
                            </div>
                        )}
                    </div>
                </div>

                <div className="card card-modern">
                    <div className="card-body">

                        {/* LINHA 1: data, dispositivo, método, IP */}
                        <div className="row">
                            {this.renderCampo('Data do Log', formatarDataInputDateToPtBr(dados.dataCriacao), 'col-md-4')}
                            {this.renderCampo('Dispositivo', dados.dispositivo, 'col-md-4')}
                            {this.renderCampo('Método', dados.metodo, 'col-md-4')}
                            {this.renderCampo('IP', dados.ip, 'col-md-4')}
                            {this.renderCampo('Aplicação', dados.aplicacao, 'col-md-4')}
                            {this.renderCampo('Usuário', dados.usuarioNome, 'col-md-4')}
                        </div>

                        {/* USER AGENT */}
                        <div className="row">
                            <div className="col-md-12">
                                <div className="form-group mb-3">
                                    <label style={{ fontSize: '11px', textTransform: 'uppercase', letterSpacing: '0.5px', color: '#6c757d', marginBottom: '4px' }}>
                                        User Agent
                                    </label>
                                    <input
                                        type="text"
                                        className="form-control form-control-modern"
                                        value={dados.userAgent || '-'}
                                        disabled
                                        readOnly
                                    />
                                </div>
                            </div>
                        </div>

                        {/* DADOS ANTES / DEPOIS / ALTERADOS */}
                        {(dados.dadosAntes || dados.dadosDepois || dados.dadosAlterados) && (
                            <>
                                <hr style={{ borderColor: '#dee2e6', margin: '8px 0 16px' }} />
                                <p style={{ fontSize: '11px', textTransform: 'uppercase', letterSpacing: '0.5px', color: '#6c757d', marginBottom: '12px' }}>
                                    Dados da Operação
                                </p>
                                <div className="row">
                                    <JsonViewer label="Dados Antes" dados={this.state.dados.dadosAntes} />
                                    <JsonViewer label="Dados Depois" dados={this.state.dados.dadosDepois} />
                                    <JsonViewer label="Dados Alterados" dados={this.state.dados.dadosAlterados} />
                                </div>
                            </>
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
                                <h4 className="modal-title font-weight-bold">Log do Sistema</h4>
                                <small className="text-muted">Detalhes do registro de log.</small>
                            </div>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body bg-light">
                            {!this.state.iniciando ? form : <LoadingIndicator />}
                        </div>
                        <div className="modal-footer bg-white border-top-0">
                            <button type="button" className="btn btn-light btn-modern" data-dismiss="modal">
                                Fechar
                            </button>
                        </div>
                    </div>
                </div>
            </div>;

        return modal;
    }
}