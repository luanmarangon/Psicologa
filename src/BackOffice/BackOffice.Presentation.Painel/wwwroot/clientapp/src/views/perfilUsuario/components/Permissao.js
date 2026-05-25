import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Permissao extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dados: {
                senhaConfirmacao: "",
                id: this.props.perfilId,
                nome: this.props.perfilNome,
                permissoes: []
            },
            permissoes: [],
            iniciando: true,
            obtendo: false,
            aguarde: false,  
        };

    }

    componentDidMount = () => {

        let that = this; 

        this.obterPermissoes().finally(() => {
            this.obter().finally(() => {

                this.setState({
                    iniciando: false
                });

            });

        });

        $("#permissoesModal").modal('show');

        $('#permissoesModal').on('hidden.bs.modal', function (e) {
            that.props.onFechar();
        });
    }

    componentDidUpdate = () => {

    }

    componentWillUnmount = () => {
        
        $('#permissoesModal').modal('hide');
    }


    obterPermissoes = () => {

        let p = HTTPClient.get("Administrativo/PerfilUsuario/ObterPermissoes")
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    ...this.state,
                    permissoes: r.data
                });

            })
            .catch((e) => {
                showToastr({
                    type: "error",
                    text: "Um erro ocorreu."
                });
            })
            .finally(() =>{
            });

        return p;
    }


    obter = () =>{
        
  
        let p = HTTPClient.get("Administrativo/PerfilUsuario/Obter?id=" + this.state.dados.id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    ...this.dados,
                    dados: {
                        ...this.state.dados,
                        permissoes: r.data.permissoes
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


        HTTPClient.post("Administrativo/PerfilUsuario/Salvar", this.state.dados)
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
            .finally(() =>{
                this.setState({
                    aguarde: false
                });
            });
          
    }


    selecionarPermissao = (e, item) => {
        let permissoes = this.state.dados.permissoes;

        if (e.target.checked){
            permissoes = [...permissoes, item.id]
        }
        else
        {            
            let index = permissoes.findIndex((s) => s == item.id);
            if (index > -1)
            permissoes.splice(index, 1);
        }

        this.setState({
            ...this.state,
            dados: {
                ...this.state.dados,
                permissoes: permissoes
            }
        });
    }


    render() {
        let form  = 
            <form role="form">
                <div className="form-group">
                    <label htmlFor="txtPerfil">Perfil</label>
                    <input type="text" className="form-control" id="txtPerfil" disabled value={this.state.dados.nome} />
                </div>

                {this.state.obtendo || this.state.iniciando ?
                    <div>
                        <LoadingIndicator timeWait={100} />
                    </div>
                :

                    <div className="form-group">
                        <label htmlFor="txtPerfil">Permissões*</label>
                        <div className="border p-2 rounded-sm">
                            {this.state.permissoes.map(item => {

                                let isChecked = false;
                                if (this.state.dados.permissoes.length > 0){
                                    isChecked = this.state.dados.permissoes.findIndex((s) => s == item.id) > -1;
                                }

                                let menu = 
                                    <div className="form-check" key={item.id}>
                                        <input className="form-check-input" type="checkbox" 
                                            defaultChecked={isChecked}
                                            id={`cbServ${item.id}`} onClick={evt => this.selecionarPermissao(evt, item)} />
                                        <label className="form-check-label" htmlFor={`cbServ${item.id}`}>
                                            {item.nome}
                                            <br />
                                        </label>
                                    </div>
                                
                                return menu;
                                
                            })}
                        </div>
                    </div>  
                }
            
            </form>

        let modal = 
            <div className="modal fade" id="permissoesModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-lg modal-dialog-scrollable">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title">Permissões do Perfil</h4>
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
 
