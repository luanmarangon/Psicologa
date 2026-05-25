import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Cadastro extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dados: {
                id: this.props.usuarioId,
                nome: "",
                senha: "",
                senhaConfirmacao: "",
                pessoaId: this.props.pessoaId,
                pessoaNome: this.props.pessoaNome,
                perfilId: "0"
            },
            perfis: [],
            iniciando: true,
            obtendo: false,
            aguarde: false,
        };

        this.podeTrocarSenha = false;

    }

    componentDidMount = () => {

        let that = this;

        this.obterPerfis().finally(() => {
            this.obter().finally(() => {

                this.setState({
                    iniciando: false
                });

            });

        });

        $("#cadastroModal").modal('show');

        $('#cadastroModal').on('hidden.bs.modal', function (e) {
            that.props.onFechar();
        });
    }

    componentDidUpdate = () => {

    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');
    }

    obter = () => {

        if (isEmpty(this.state.dados.id) || this.state.dados.id == "0") {
            this.podeTrocarSenha = true;
            return Promise.resolve(true);
        }
        let p = HTTPClient.get("Administrativo/Usuario/Obter?id=" + this.state.dados.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    ...this.dados,
                    dados: {
                        ...this.state.dados,
                        nome: r.data.nome,
                        perfilId: r.data.perfilId
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

    obterPerfis = () => {

        let p = HTTPClient.get("Administrativo/Usuario/ObterPerfis?id=" + this.state.dados.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    ...this.dados,
                    perfis: r.data,
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


        HTTPClient.post("Administrativo/Usuario/Salvar", this.state.dados)
            .then(r => {
                return r.json();
            })
            .then(r => {

                if (r.success) {
                    this.props.onFechar(r.data);
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
                <div className="form-group">
                    <label htmlFor="txtColaborador">Colaborador</label>
                    <input type="text" className="form-control" id="txtColaborador" disabled value={this.state.dados.pessoaNome} />
                </div>

                <div className="row">
                    <div className="col form-group">
                        <label htmlFor="txtNome">Nome de Usuário*</label>
                        <input type="text" className="form-control" id="txtNome" autoComplete="off" maxLength="50"
                            value={this.state.dados.nome}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, nome: e.target.value } })} />
                    </div>

                    <div className="col form-group">
                        <label htmlFor="selOptPerfil">Perfil*</label>
                        <select id="selOptPerfil" className="form-control"
                            defaultValue={this.state.dados.perfilId}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, perfilId: e.target.value } })} >
                            <option value="0"></option>
                            {this.state.perfis.map((item) => <option key={item.id} value={item.id}>{item.nome}</option>)}

                        </select>
                    </div>
                </div>


                <div className="row">

                    <div className="col form-group">
                        <label htmlFor="txtSenha">Senha*</label>
                        <input type="password" className="form-control" id="txtSenha" autoComplete="off" maxLength="15" value={this.state.senha}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, senha: e.target.value } })} disabled={this.podeTrocarSenha ? "" : "disabled"} />
                    </div>

                    <div className="col form-group">
                        <label htmlFor="txtSenhaConfirmacao">Confirmação da Senha*</label>
                        <input type="password" className="form-control" id="txtSenhaConfirmacao" autoComplete="off" maxLength="15" value={this.state.senhaConfirmacao}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, senhaConfirmacao: e.target.value } })} disabled={this.podeTrocarSenha ? "" : "disabled"} />
                    </div>

                </div>


                <div className="form-group text-info">
                    (A senha deve ser composta de no mínimo, 6 caracteres)
                </div>



            </form>

        let modal =
            <div className="modal fade" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-lg modal-dialog-scrollable">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title">Dados de Usuário do Sistema</h4>
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

