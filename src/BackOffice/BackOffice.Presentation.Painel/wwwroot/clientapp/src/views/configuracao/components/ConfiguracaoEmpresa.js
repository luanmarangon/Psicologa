import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class ConfiguracaoEmpresa extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dados: {
                id: "",
                nome: "",
                cep: "",
                endereco: "",
                numero: "",
                complemento: "",
                bairro: "",
                cidade: "",
                estado: "",
                whatsapp: "",
                email: "",
                facebook: "",
                instagram: "",
                linkedin: "",
                slogan: "",
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

        $('#cadastroModal').on('hidden.bs.modal', function (e) {
            that.props.onFechar();
        });
    }

    componentDidUpdate = () => {

        let that = this;

        //máscara para CEP
        $("#txtCEP").inputmask("99999-999");
        $("#txtCEP").off('change');
        $("#txtCEP").on('change', function (e) {

            that.setState({
                dados: {
                    ...that.state.dados,
                    cep: e.target.value
                }
            });

            that.preencherEndereco(e.target.value);
        });

        $("#txtWhatsApp").inputmask("(99) 99999-9999");
        $("#txtWhatsApp").off('change');
        $("#txtWhatsApp").on('change', function (e) {

            that.setState({
                dados: {
                    ...that.state.dados,
                    whatsapp: e.target.value
                }
            });

        });

    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');
    }

    obter = () => {

        let p = HTTPClient.get("Administrativo/Configuracao/ObterConfiguracao")
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    ...this.dados,
                    dados: {
                        ...this.state.dados,
                        id: r.data.id,
                        nome: r.data.nome,
                        cep: r.data.cep,
                        endereco: r.data.endereco,
                        numero: r.data.numero,
                        complemento: r.data.complemento,
                        bairro: r.data.bairro,
                        cidade: r.data.cidade,
                        estado: r.data.estado,
                        whatsapp: r.data.whatsapp,
                        email: r.data.email,
                        facebook: r.data.facebook,
                        instagram: r.data.instagram,
                        linkedin: r.data.linkedin,
                        slogan: r.data.slogan,
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

    // obterPerfis = () => {

    //     let p = HTTPClient.get("Administrativo/Usuario/ObterPerfis?id=" + this.state.dados.id)
    //         .then(r => {
    //             return r.json();
    //         })
    //         .then(r => {

    //             this.setState({
    //                 ...this.dados,
    //                 perfis: r.data,
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


    salvar = () => {

        this.setState({
            aguarde: true
        });


        HTTPClient.post("Administrativo/Configuracao/Salvar", this.state.dados)
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

    preencherEndereco = (cep) => {

        if (cep == "")
            return;

        fetch("//viacep.com.br/ws/" + cep + "/json/")
            .then((r) => {
                return r.json();
            })
            .then((r) => {

                r.localidade

                this.setState({
                    dados: {
                        ...this.state.dados,
                        endereco: r.logradouro,
                        bairro: r.bairro,
                        complemento: r.complemento,
                        estado: r.uf,
                        cidade: r.localidade
                    }
                });

                //this.obterCidades(r.uf, r.ibge);

            })
            .catch((e) => {
                console.log("Erro ao obter endereço", e);
            });

    }

    render() {
        let form =
            <form role="form">
                <div className="form-group">
                    <label htmlFor="txtNome">Nome</label>
                    <input type="text" className="form-control" id="txtNome" value={this.state.dados.nome}
                        onChange={(e) => this.setState({ dados: { ...this.state.dados, nome: e.target.value } })} />
                </div>

                <label>Endereço</label>
                <div className="border rounded p-2 mb-2 small ">
                    <div className="row">
                        <div className="col-2 form-group">
                            <label htmlFor="txtCEP">CEP</label>
                            <input type="text" className="form-control" id="txtCEP" autoComplete="off" maxLength="10"
                                value={this.state.dados.cep}
                                onChange={(e) => {
                                    this.setState({ dados: { ...this.state.dados, cep: e.target.value } })
                                    this.preencherEndereco(e.target.value);
                                }} />
                        </div>
                        <div className="col form-group">
                            <label htmlFor="txtEndereco">Endereço</label>
                            <input type="text" className="form-control" id="txtEndereco" autoComplete="off" maxLength="150"
                                value={this.state.dados.endereco} onChange={(e) => this.setState({ dados: { ...this.state.dados, endereco: e.target.value } })} />
                        </div>
                        <div className="col-2 form-group">
                            <label htmlFor="txtNumero">Número</label>
                            <input type="text" className="form-control" id="txtNumero" autoComplete="off" maxLength="20"
                                value={this.state.dados.numero} onChange={(e) => this.setState({ dados: { ...this.state.dados, numero: e.target.value } })} />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col form-group">
                            <label htmlFor="txtComplemento">Complemento</label>
                            <input type="text" className="form-control" id="txtComplemento" autoComplete="off" maxLength="150"
                                value={this.state.dados.complemento} onChange={(e) => this.setState({ dados: { ...this.state.dados, complemento: e.target.value } })} />
                        </div>
                        <div className="col form-group">
                            <label htmlFor="txtBairro">Bairro</label>
                            <input type="text" className="form-control" id="txtBairro" autoComplete="off" maxLength="150"
                                value={this.state.dados.bairro} onChange={(e) => this.setState({ dados: { ...this.state.dados, bairro: e.target.value } })} />
                        </div>
                        <div className="col form-group">
                            <label htmlFor="txtCidade">Cidade</label>
                            <input type="text" className="form-control" id="txtCidade" autoComplete="off" maxLength="150"
                                value={this.state.dados.cidade} onChange={(e) => this.setState({ dados: { ...this.state.dados, cidade: e.target.value } })} />
                        </div>
                        <div className="col-1 form-group">
                            <label htmlFor="txtEstado">Estado</label>
                            <input type="text" className="form-control" id="txtEstado" autoComplete="off" maxLength="150"
                                value={this.state.dados.estado} onChange={(e) => this.setState({ dados: { ...this.state.dados, estado: e.target.value } })} />
                        </div>
                    </div>
                </div>

                <label>Contatos</label>
                <div className="border rounded p-2 mb-2 small ">
                    <div className="row">
                        <div className="col-4 form-group">
                            <label htmlFor="txtWhatsApp">WhatsApp</label>
                            <div className="input-group">
                                <div className="input-group-prepend">
                                    <span className="input-group-text" style={{ background: '#25d366', color: '#fff', border: 'none' }}                                >
                                        <i className="fab fa-whatsapp"></i>
                                    </span>
                                </div>
                                <input type="text" className="form-control" id="txtWhatsApp" autoComplete="off" maxLength="150" placeholder="Número do WhatsApp"
                                    value={this.state.dados.whatsapp} onChange={(e) => this.setState({ dados: { ...this.state.dados, whatsapp: e.target.value } })} />
                            </div>
                        </div>

                        <div className="col form-group">
                            <label htmlFor="txtEmail">E-mail</label>
                            <div className="input-group">
                                <div className="input-group-prepend">
                                    <span className="input-group-text" style={{ background: '#007bff', color: '#fff', border: 'none' }}                                >
                                        <i className="fas fa-envelope"></i>
                                    </span>
                                </div>
                                <input type="email" className="form-control" id="txtEmail" autoComplete="off" maxLength="150" placeholder="E-mail"
                                    value={this.state.dados.email} onChange={(e) => this.setState({ dados: { ...this.state.dados, email: e.target.value } })} />
                            </div>
                        </div>
                    </div>

                    <div className="row">
                        <div className="col form-group">
                            <label htmlFor="txtFacebook">Facebook</label>
                            <div className="input-group">
                                <div className="input-group-prepend">
                                    <span className="input-group-text" style={{ background: '#1877f2', color: '#fff', border: 'none' }}                                >
                                        <i className="fab fa-facebook-f"></i>
                                    </span>
                                </div>
                                <input type="text" className="form-control" id="txtFacebook" autoComplete="off" maxLength="150" placeholder="Link do Facebook"
                                    value={this.state.dados.facebook} onChange={(e) => this.setState({ dados: { ...this.state.dados, facebook: e.target.value } })} />
                            </div>
                        </div>
                        <div className="col form-group">
                            <label htmlFor="txtInstagram">Instagram</label>
                            <div className="input-group">
                                <div className="input-group-prepend">
                                    <span className="input-group-text" style={{ background: '#e1306c', color: '#fff', border: 'none' }}                                >
                                        <i className="fab fa-instagram"></i>
                                    </span>
                                </div>
                                <input type="text" className="form-control" id="txtInstagram" autoComplete="off" maxLength="150" placeholder="Link do Instagram"
                                    value={this.state.dados.instagram} onChange={(e) => this.setState({ dados: { ...this.state.dados, instagram: e.target.value } })} />
                            </div>
                        </div>
                        <div className="col form-group">
                            <label htmlFor="txtLinkedIn">LinkedIn</label>
                            <div className="input-group">
                                <div className="input-group-prepend">
                                    <span className="input-group-text" style={{ background: '#0077b5', color: '#fff', border: 'none' }}                                >
                                        <i className="fab fa-linkedin-in"></i>
                                    </span>
                                </div>
                                <input type="text" className="form-control" id="txtLinkedIn" autoComplete="off" maxLength="150" placeholder="Link do LinkedIn"
                                    value={this.state.dados.linkedin} onChange={(e) => this.setState({ dados: { ...this.state.dados, linkedin: e.target.value } })} />
                            </div>
                        </div>
                    </div>
                </div>

                <label>Site</label>
                <div className="border rounded p-2 mb-2 small ">
                    <div className="row">
                        <div className="col form-group">
                            <label htmlFor="txtSlogan">Slogan</label>
                               <textarea className="form-control" id="txtSlogan" autoComplete="off" maxLength="200" rows="3" 
                                    value={this.state.dados.slogan} onChange={(e) => this.setState({ dados: { ...this.state.dados, slogan: e.target.value } })} />
                           
                           
{/*                            
                            <input type="text" className="form-control" id="txtSlogan" autoComplete="off" maxLength="150"
                                value={this.state.dados.slogan} onChange={(e) => this.setState({ dados: { ...this.state.dados, slogan: e.target.value } })} /> */}
                        </div>
                    </div>
                </div>
            </form>

        let modal =
            <div className="modal fade" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-xl modal-dialog-scrollable">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title">Configurações da Empresa</h4>
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

