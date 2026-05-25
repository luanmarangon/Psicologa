import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';
import CadastroTipo from '../components/CadastroTipo';

export default class Agendamento extends Component {

    constructor(props) {
        super(props);
        this.state = {

            dados: {
                id: "0",
                docIdTipo: "1",
                docIdNro: "",
                nome: "",
                nomeReduzido: "",
                dataNascimento: "",
                razaoSocial: "",
                sexo: "0",
                ativo: "true"
            },
            endereco: {
                id: "0",
                logradouro: "",
                numero: "",
                bairro: "",
                cep: "",
                complemento: "",
                uf: "SP",
                cidadeId: "",
                pontoReferencia: "",
                latitude: "",
                longitude: ""
            },
            contato: {
                id: "0",
                tipo: "1",
                tipoNome: "",
                contato: "",
                observacao: ""
            },
            contatos: [],
            tipos: [],
            cidadesOptions: [],
            carregandoCidades: true,
            iniciando: true,
            aguarde: false
        };

    }

    componentDidMount = () => {


        // let promiseObterCidades = null;
        let promiseEdicao = null;
        if (!isEmpty(this.props.idEdicao)) {
            promiseEdicao = this.obter(this.props.idEdicao);
            promiseEdicao.then(() => {
                // promiseObterCidades = this.obterCidades(this.state.endereco.uf);
            });
        }
        else {
            // promiseObterCidades = this.obterCidades(this.state.endereco.uf);
        }

        Promise.all([promiseEdicao]).then(() => {
            this.setState({
                iniciando: false,
                carregandoCidades: false,
            });
        });


        $("#cadastroModal").modal('show');

        let that = this;

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
                endereco: {
                    ...that.state.endereco,
                    cep: e.target.value
                }
            });

            that.preencherEndereco(e.target.value);
        });


        //máscara para Telefone
        $("#txtContato").off('change');
        if (this.state.contato.tipo == "6" || this.state.contato.tipo == "1") {
            $("#txtContato").inputmask("(99) 99999-9999");
            $("#txtContato").on('change', function (e) {

                that.setState({
                    contato: {
                        ...that.state.contato,
                        contato: e.target.value
                    }
                });
            });
        }
        else if (this.state.contato.tipo == "2") {
            $("#txtContato").inputmask("(99) 9999-9999");
            $("#txtContato").on('change', function (e) {

                that.setState({
                    contato: {
                        ...that.state.contato,
                        contato: e.target.value
                    }
                });
            });
        }
        else {
            $("#txtContato").inputmask("");
            $("#txtContato").on('change', function (e) {

                that.setState({
                    contato: {
                        ...that.state.contato,
                        contato: e.target.value
                    }
                });
            });


        }

        //máscara para CPF ou CNPJ

        $("#txtDocIdNro").off('change');
        if (this.state.dados.docIdTipo == "1" || this.state.dados.docIdTipo == "3") //CPF ou CNPJ
        {
            $("#txtDocIdNro").on('change', function (e) {

                that.setState({
                    dados: {
                        ...that.state.dados,
                        docIdNro: e.target.value
                    }
                });
            });

            if (this.state.dados.docIdTipo == "1") {
                $("#txtDocIdNro").inputmask("999.999.999-99");
            }
            else if (this.state.dados.docIdTipo == "3") {
                $("#txtDocIdNro").inputmask("**.***.***/****-**");
            }
        }
        else {
            $("#txtDocIdNro").inputmask("");

            $("#txtDocIdNro").on('change', function (e) {

                that.setState({
                    dados: {
                        ...that.state.dados,
                        docIdNro: e.target.value
                    }
                });
            });
        }

        tableSelectable();
    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');

    }

    obter = (id) => {

        let p = HTTPClient.get("Administrativo/Cliente/ObterCliente?id=" + id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    dados: r.data.dados,
                    endereco: r.data.endereco,
                    contatos: r.data.contatos,
                    tipos: r.data.tipos
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

        var pessoaDados = {
            dados: this.state.dados,
            endereco: this.state.endereco,
            contatos: this.state.contatos,
            tipos: this.state.tipos
        }

        HTTPClient.post("Administrativo/Cliente/SalvarCliente", pessoaDados)
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

    // obterCidades = (uf, ibge) => {

    //     this.setState({
    //         ...this.state,
    //         endereco: { ...this.state.endereco, uf: uf },
    //         carregandoCidades: true
    //     });

    //     let cidadeId = "0";
    //     let p = HTTPClient.get("Administrativo/Prestador/ObterCidades?uf=" + uf)
    //         .then(r => {
    //             return r.json();
    //         })
    //         .then(r => {

    //             let cidadesOptions = r.data.map(c => {

    //                 if (ibge != undefined && ibge == c.ibge) {
    //                     cidadeId = c.id;
    //                 }

    //                 return <option key={c.id} value={c.id}>{c.nome}</option>
    //             });

    //             if (cidadeId == "0") {
    //                 cidadeId = this.state.endereco.cidadeId;
    //             }

    //             this.setState({
    //                 ...this.state,
    //                 endereco: {
    //                     ...this.state.endereco,
    //                     cidadeId: cidadeId
    //                 },
    //                 cidadesOptions: cidadesOptions,
    //                 carregandoCidades: false
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



    adicionarContato = () => {

        if (this.state.contato.contato.trim() != "") {
            let contatos = this.state.contatos;
            let item = null;

            if (this.state.contato.id != "0") {
                //edição
                item = contatos.find((item) => { return item.id === this.state.contato.id });

                if (item != null) {
                    item.tipo = this.state.contato.tipo;
                    item.tipoNome = this.refs.selTpContato[this.refs.selTpContato.selectedIndex].innerHTML;
                    item.contato = this.state.contato.contato;
                    item.observacao = this.state.contato.observacao;
                }
            }

            if (item == null) {
                //novo    
                contatos.push({
                    id: new Date().getTime() * -1,
                    tipo: this.state.contato.tipo,
                    tipoNome: this.refs.selTpContato[this.refs.selTpContato.selectedIndex].innerHTML,
                    contato: this.state.contato.contato,
                    observacao: this.state.contato.observacao
                });
            }

            this.setState({
                contato: {
                    ...this.state.contato,
                    id: 0,
                    contato: "",
                    observacao: "",
                    contatos: contatos
                }
            });
        }
    }


    excluirContato = (itemExcluir) => {

        if (!confirm("Confirma a exclusão?")) {
            return false;
        }

        this.setState({
            contatos: this.state.contatos.filter(item => { return item.id !== itemExcluir.id }),
        });

    }


    editarContato = (itemEditar) => {

        $("#txtContato").inputmask("");
        this.setState({
            contato: {
                ...this.state.contato,
                id: itemEditar.id,
                tipo: itemEditar.tipo,
                contato: itemEditar.contato,
                observacao: itemEditar.observacao
            }
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
                    endereco: {
                        ...this.state.endereco,
                        logradouro: r.logradouro,
                        bairro: r.bairro,
                        complemento: r.complemento,
                        uf: r.uf,
                        cidade: r.localidade
                    }
                });

                // this.obterCidades(r.uf, r.ibge);

            })
            .catch((e) => {
                // console.log("Erro ao obter endereço", e);
            });

    }

    setStateTipos = (tipos) => {

        this.setState({
            tipos: tipos
        });
    }



    render() {

        let form =
            <form role="form">

                <div className="row">
                    <div className="col-sm-3">
                        <div className="form-group">
                            <label htmlFor="selDocIdTipo">Doc. de Identificação*</label>
                            <select id="selDocIdTipo" className="form-control" value={this.state.dados.docIdTipo} onChange={(e) => this.setState({ dados: { ...this.state.dados, docIdTipo: e.target.value } })}>
                                <option value="1">CPF</option>
                                {/*<option value="2">Passaporte</option>*/}
                                <option value="3">CNPJ</option>
                                {/*<option value="4">Cadastro Único (PF)</option>*/}
                            </select>
                        </div>
                    </div>
                    <div className="col-sm-5">
                        <div className="form-group">
                            <label htmlFor="txtDocIdNro">Documento*</label>
                            <input type="text" className="form-control" id="txtDocIdNro" autoComplete="off" placeholder="Número do Doc. de Identificação" maxLength="20" value={this.state.dados.docIdNro}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, docIdNro: e.target.value } })} />
                        </div>
                    </div>

                    <div className="col-sm-2">
                        <div className="form-group">
                            <label>Ativo*</label>
                            <select className="form-control"
                                value={this.state.dados.ativo}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, ativo: e.target.value } })} >
                                <option value="true">Sim</option>
                                <option value="false">Não</option>
                            </select>
                        </div>
                    </div>
                </div>

                <div className="row" style={this.state.dados.docIdTipo == "3" ? { display: "none" } : null}>
                    <div className="col-sm-4">
                        <div className="form-group">
                            <label htmlFor="txtDataNascimento">Data de Nascimento*</label>
                            <input type="date" className="form-control" id="txtDataNascimento" value={formatarDataPtBrToInputDate(this.state.dados.dataNascimento)}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, dataNascimento: e.target.value } })} />
                        </div>
                    </div>

                    <div className="col-sm-4">
                        <div className="form-group">
                            <label htmlFor="selSexo">Sexo</label>
                            <select id="selSexo" className="form-control"
                                value={this.state.dados.sexo}
                                onChange={(e) => { this.setState({ dados: { ...this.state.dados, sexo: e.target.value } }) }}>
                                <option value="0"></option>
                                <option value="1">Masculino</option>
                                <option value="2">Feminino</option>
                            </select>
                        </div>
                    </div>

                </div>
            </form>


        let modal =
            <div className="modal fade" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-lg modal-dialog-scrollable">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title">Agendamento</h4>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">
                            {!this.state.iniciando ? form : <LoadingIndicator />}
                        </div>

                        <div className={"modal-footer " + (this.state.aguarde ? "site-disabled" : "")}>
                            <button type="button" className="btn btn-primary" onClick={this.salvar}>
                                {!this.state.aguarde ? "Salvar" : <span><i className="fas fa-circle-notch fa-spin mr-1"></i>Salvando</span>}
                            </button>
                        </div>

                    </div>
                </div>
            </div>




        return (modal);
    }
}