import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Cadastro extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dados: {
                id: this.props.usuarioId,
                nome: "",
                icon: "",
                destaqueHome: false,
                ativo: true,
            },
            iniciando: true,
            obtendo: false,
            aguarde: false,
        };


    }

    componentDidMount = () => {
        let promiseEdicao = null;

        if (!isEmpty(this.props.idEdicao)) {
            promiseEdicao = this.obter(this.props.idEdicao);
        }

        Promise.all([promiseEdicao]).then(() => {

            this.setState({
                iniciando: false,
            });

        });
        $("#cadastroModal").modal('show');

        $('#cadastroModal').on('hidden.bs.modal', () => {
            this.props.onFechar();
        });
    }

    componentDidUpdate = () => {

    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');
    }

    obter = (id) => {
        // if (isEmpty(this.state.dados.id) || this.state.dados.id == "0") {
        //     this.podeTrocarSenha = true;
        //     return Promise.resolve(true);
        // }
        let p = HTTPClient.get("Administrativo/Convenio/Obter?id=" + id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    ...this.dados,
                    dados: {
                        ...this.state.dados,
                        nome: r.data.nome,
                        icon: r.data.icon,
                        destaqueHome: r.data.destaqueHome,
                        ativo: r.data.ativo,
                    }
                });

            })
            .catch((e) => {
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


        HTTPClient.post("Administrativo/Convenio/Salvar", this.state.dados)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (r.success) {
                    this.props.onFechar(r.data);
                    showToastr(r.messages);
                }
                else showToastr(r.messages);

            })
            .catch((e) => {
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
                <div className="row">
                    <div className="col form-group">
                        <label htmlFor="txtNome">Nome do Convênio*</label>
                        <input type="text" className="form-control" id="txtNome" autoComplete="off" maxLength="100"
                            value={this.state.dados.nome}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, nome: e.target.value } })} />
                    </div>
                </div>

                <div className="row">
                    <div className="col form-group">
                        <label htmlFor="Icone">Ícone</label>
                        <div className="input-group">
                            {/* Preview */}
                            <div className="input-group-prepend">
                                <span className="input-group-text">
                                    <i className={this.state.dados.icon || "fas fa-icons"}></i>
                                </span>
                            </div>

                            {/* Select */}
                            <select className="form-control" id="Icone" value={this.state.dados.icon || ""}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, icon: e.target.value } })}>
                                <option value="">Selecione um ícone</option>
                                <option value="fas fa-heart">❤️ Coração</option>
                                <option value="fas fa-brain">🧠 Psicologia</option>
                                <option value="fas fa-heartbeat">💓 Saúde</option>
                                <option value="fas fa-notes-medical">📋 Atendimento</option>
                                <option value="fas fa-user-md">👨‍⚕️ Profissional</option>
                                <option value="fas fa-hospital">🏥 Clínica</option>
                                <option value="fas fa-stethoscope">🩺 Consulta</option>
                                <option value="fas fa-star">⭐ Destaque</option>
                            </select>
                        </div>

                    </div>
                    <div className="col-2 form-group">
                        <label htmlFor="Ativo">Ativo</label>
                        <select className="form-control" id="Ativo"
                            value={this.state.dados.ativo ? "1" : "0"}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, ativo: e.target.value === "1" } })}>
                            <option value="1">Sim</option>
                            <option value="0">Não</option>
                        </select>
                    </div>
                    <div className="form-check form-check-inline">
                        <input className="form-check-input" type="checkbox" id="txtDestaque" checked={this.state.dados.destaqueHome}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, destaqueHome: e.target.checked } })} />
                        <label className="form-check-label" htmlFor="txtDestaque">Destaque Home</label>
                    </div>
                </div>
            </form>

        let modal =
            <div className="modal fade" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-lg modal-dialog-scrollable">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title">Dados do Convênio</h4>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">

                            {!this.state.iniciando ? form : <LoadingIndicator />}

                        </div>
                        <div className={"modal-footer " + (this.state.aguarde ? "site-disabled" : "")}>
                            <button type="button" className="btn btn-primary" onClick={this.salvar}>Salvar</button>

                        </div>
                    </div>
                </div>
            </div>

        return (modal);
    }
}

