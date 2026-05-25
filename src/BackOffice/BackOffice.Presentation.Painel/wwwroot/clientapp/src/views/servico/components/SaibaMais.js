import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class SaibaMais extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dados: {
                servicoId: this.props.idEdicao,
                nome: "",
                contato: "",
                email: "",
                mensagem: ""
            },
            iniciando: true,
            obtendo: false,
            aguarde: false,
        };
    }

    componentDidMount = () => {

        // let promiseEdicao = null;

        // if (!isEmpty(this.props.idEdicao)) {
        //     promiseEdicao = this.obter(this.props.idEdicao);

        // }

        // Promise.all([promiseEdicao]).then(() => {

        this.setState({
            iniciando: false,
        });

        // });

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

    salvar = () => {

        this.setState({
            aguarde: true
        });

        HTTPClient.post("Home/SaibaMais", this.state.dados)
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

                {/* Nome + WhatsApp */}
                <div className="row">
                    <div className="col-md-6 mb-4">
                        <label className="modern-label">Nome</label>
                        <div className="modern-input-group">
                            <i className="fas fa-user modern-input-icon"></i>
                            <input type="text" className="modern-input" value={this.state.dados.nome} maxLength="70"
                                autoComplete="off" placeholder="Digite seu nome" onChange={(e) => this.setState({ dados: { ...this.state.dados, nome: e.target.value } })} />
                        </div>
                    </div>
                    <div className="col-md-6 mb-4">
                        <label className="modern-label">WhatsApp</label>
                        <div className="modern-input-group">
                            <i className="fab fa-whatsapp modern-input-icon"></i>
                            {/* <input type="text" id="txtContato" className="modern-input" value={this.state.dados.contato} maxLength="20"
                                autoComplete="off" placeholder="(00) 00000-0000" onChange={(e) => this.setState({ dados: { ...this.state.dados, contato: e.target.value } })} /> */}
                            <input
                                type="text"
                                className="modern-input"
                                value={this.state.dados.contato}
                                maxLength="15"
                                autoComplete="off"
                                placeholder="(00) 00000-0000"
                                onChange={(e) => {

                                    let value = e.target.value.replace(/\D/g, '');

                                    // limita a 11 números
                                    value = value.substring(0, 11);

                                    // aplica máscara
                                    value = value.replace(/^(\d{2})(\d)/g, '($1) $2');
                                    value = value.replace(/(\d{5})(\d)/, '$1-$2');

                                    this.setState({
                                        dados: {
                                            ...this.state.dados,
                                            contato: value
                                        }
                                    });

                                }}
                            />
                        </div>
                    </div>
                </div>

                {/* Email */}
                <div className="mb-4">
                    <label className="modern-label">Email</label>
                    <div className="modern-input-group">
                        <i className="fas fa-envelope modern-input-icon"></i>
                        <input type="email" className="modern-input" value={this.state.dados.email} maxLength="120"
                            autoComplete="off" placeholder="Digite seu email" onChange={(e) => this.setState({ dados: { ...this.state.dados, email: e.target.value } })} />
                    </div>
                </div>

                {/* Mensagem */}
                <div className="mb-2">
                    <label className="modern-label">Mensagem</label>
                    <div className="modern-textarea-group">
                        <textarea className="modern-textarea" rows="5" maxLength="500" placeholder="Conte um pouco sobre o que você procura..."
                            value={this.state.dados.mensagem} onChange={(e) => this.setState({ dados: { ...this.state.dados, mensagem: e.target.value } })} />
                    </div>
                </div>
            </form>;

        return (
            <div className="modal fade" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-lg modal-dialog-centered">
                    <div className="modal-content modern-modal">
                        {/* Header */}
                        <div className="modal-header modern-modal-header border-0">
                            <div>
                                <span className="ps-tag mb-3 d-inline-block">Atendimento</span>
                                <h3 className="font-weight-bold mb-2">Quero saber mais</h3>
                                <p className="text-muted mb-0">Preencha os dados abaixo e entrarei em contato com você.</p>
                            </div>

                            <button type="button" className="close" data-dismiss="modal">
                                <span>&times;</span>
                            </button>

                        </div>

                        {/* Body */}
                        <div className="modal-body px-4 pb-2">
                            {!this.state.iniciando ? form : <LoadingIndicator />}
                        </div>

                        {/* Footer */}
                        <div className="modal-footer border-0 px-4 pb-4">
                            <button type="button" className="btn btn-light px-4" data-dismiss="modal" >Fechar</button>
                            <button type="button" className="ps-button border-0" onClick={this.salvar} >Enviar mensagem</button>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

