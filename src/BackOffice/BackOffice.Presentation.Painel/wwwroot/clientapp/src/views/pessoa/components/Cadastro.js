import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';
import CadastroTipo from '../components/CadastroTipo';

export default class Cadastro extends Component {

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
                cidade: "",
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
            // carregandoCidades: true,
            iniciando: true,
            aguarde: false
        };

    }

    componentDidMount = () => {


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

        let p = HTTPClient.get("Administrativo/Pessoa/Obter?id=" + id)
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

        HTTPClient.post("Administrativo/Pessoa/Salvar", pessoaDados)
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

    // obterCidades = (uf, ibge) => {

    //     this.setState({
    //         ...this.state,
    //         endereco: { ...this.state.endereco, uf: uf },
    //         carregandoCidades: true
    //     });

    //     let cidadeId = "0";
    //     let p = HTTPClient.get("Administrativo/Pessoa/ObterCidades?uf=" + uf)
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
    //                 cidadeId = this.state.endereco.cidade;
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

                //this.obterCidades(r.uf, r.ibge);

            })
            .catch((e) => {
                console.log("Erro ao obter endereço", e);
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

                <div className="form-group">
                    <label htmlFor="txtNome">Nome*</label>
                    <input type="text" className="form-control" id="txtNome" autoComplete="off" maxLength="150" value={this.state.dados.nome}
                        onChange={(e) => this.setState({ dados: { ...this.state.dados, nome: e.target.value } })} />
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

                <div className="row" style={this.state.dados.docIdTipo != "3" ? { display: "none" } : null}>
                    <div className="form-group col-sm-10">
                        <label htmlFor="txtRazaoSocial">Razão Social*</label>
                        <input type="text" className="form-control" id="txtRazaoSocial" autoComplete="off" maxLength="200" value={this.state.dados.razaoSocial}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, razaoSocial: e.target.value } })} />
                    </div>

                    <div className="form-group col-sm-2">
                        <label htmlFor="txtNomeReduzido">Nome Reduzido</label>
                        <input type="text" className="form-control" id="txtNomeReduzido" autoComplete="off" maxLength="10" value={this.state.dados.nomeReduzido}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, nomeReduzido: e.target.value } })} />
                    </div>
                </div>



                <div className="card">
                    <div className="card-header">
                        <h3 className="card-title">Endereço</h3>
                    </div>
                    <div className="card-body">
                        <div className="row">

                            <div className="col-sm-3">
                                <div className="form-group">
                                    <label htmlFor="txtCEP">CEP*</label>
                                    <input type="text" className="form-control" id="txtCEP" autoComplete="off" maxLength="10" value={this.state.endereco.cep}
                                        onChange={(e) => {
                                            this.setState({ endereco: { ...this.state.endereco, cep: e.target.value } })
                                            this.preencherEndereco(e.target.value);
                                        }} />
                                </div>
                            </div>

                            <div className="col-sm-7">
                                <div className="form-group">
                                    <label htmlFor="txtLogradouro">Logradouro*</label>
                                    <input type="text" className="form-control" id="txtLogradouro" autoComplete="off" maxLength="200" value={this.state.endereco.logradouro}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, logradouro: e.target.value } })} />

                                </div>
                            </div>
                            <div className="col-sm-2">
                                <div className="form-group">
                                    <label htmlFor="txtNumero">Número*</label>
                                    <input type="text" className="form-control" id="txtNumero" autoComplete="off" maxLength="14" value={this.state.endereco.numero}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, numero: e.target.value } })} />
                                </div>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col-sm-4">
                                <div className="form-group">
                                    <label htmlFor="txtBairro">Bairro*</label>
                                    <input type="text" className="form-control" id="txtBairro" autoComplete="off" maxLength="50" value={this.state.endereco.bairro}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, bairro: e.target.value } })} />

                                </div>
                            </div>

                            <div className="col-sm-4">
                                <div className="form-group">
                                    <label htmlFor="txtComplemento">Complemento</label>
                                    <input type="text" className="form-control" id="txtComplemento" autoComplete="off" maxLength="200" value={this.state.endereco.complemento}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, complemento: e.target.value } })} />
                                </div>
                            </div>

                            <div className="col-sm-4">
                                <div className="form-group">
                                    <label htmlFor="txtPontoReferencia">Ponto de Referência</label>
                                    <input type="text" className="form-control" id="txtPontoReferencia" autoComplete="off" maxLength="50" value={this.state.endereco.pontoReferencia}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, pontoReferencia: e.target.value } })} />
                                </div>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col-sm-2">
                                <div className="form-group">
                                    <label htmlFor="txtLatitude">Latitude</label>
                                    <input type="text" className="form-control" id="txtLatitude" autoComplete="off" maxLength="11" value={this.state.endereco.latitude}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, latitude: e.target.value } })} />
                                </div>
                            </div>

                            <div className="col-sm-2">
                                <div className="form-group">
                                    <label htmlFor="txtLongitude">Longitude</label>
                                    <input type="text" className="form-control" id="txtLongitude" autoComplete="off" maxLength="11" value={this.state.endereco.longitude}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, longitude: e.target.value } })} />
                                </div>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col-sm-2">
                                <div className="form-group">
                                    <label htmlFor="selUF">UF*</label>
                                    <select id="selUF" className="form-control"
                                        // value={this.state.endereco.uf}
                                        // onChange={(e) => {
                                        //     this.obterCidades(e.target.value);
                                        // }}

                                        value={this.state.endereco.uf}
                                        onChange={(e) => { this.setState({ endereco: { ...this.state.endereco, uf: e.target.value } }) }}

                                    >
                                        <option value="0"></option>
                                        <option value="AC">AC</option>
                                        <option value="AL">AL</option>
                                        <option value="AM">AM</option>
                                        <option value="AP">AP</option>
                                        <option value="BA">BA</option>
                                        <option value="CE">CE</option>
                                        <option value="DF">DF</option>
                                        <option value="ES">ES</option>
                                        <option value="GO">GO</option>
                                        <option value="MA">MA</option>
                                        <option value="MG">MG</option>
                                        <option value="MS">MS</option>
                                        <option value="MT">MT</option>
                                        <option value="PA">PA</option>
                                        <option value="PB">PB</option>
                                        <option value="PE">PE</option>
                                        <option value="PI">PI</option>
                                        <option value="PR">PR</option>
                                        <option value="RJ">RJ</option>
                                        <option value="RN">RN</option>
                                        <option value="RO">RO</option>
                                        <option value="RR">RR</option>
                                        <option value="RS">RS</option>
                                        <option value="SC">SC</option>
                                        <option value="SE">SE</option>
                                        <option value="SP">SP</option>
                                        <option value="TO">TO</option>

                                    </select>
                                </div>
                            </div>
                            <div className="col-sm-6">
                                <div className="form-group">
                                    <label htmlFor="selCidade">Cidade*</label>
                                    {/* <select id="selCidade" className="form-control" value={this.state.endereco.cidade} onChange={(e) => this.setState({ endereco: { ...this.state.endereco, cidadeId: e.target.value } })}>
                                        <option value="0">{this.state.carregandoCidades ? "carregando..." : ""}</option>
                                        {!this.state.carregandoCidades ? this.state.cidadesOptions : null}
                                    </select> */}

                                    <input type="text" className="form-control" id="txtCidade" autoComplete="off" maxLength="11" value={this.state.endereco.cidade}
                                        onChange={(e) => this.setState({ endereco: { ...this.state.endereco, cidade: e.target.value } })} />


                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <div className="card">
                    <div className="card-header">
                        <h3 className="card-title">Formas de Contato</h3>
                    </div>
                    <div className="card-body">

                        <div className="form-group text-info">
                            Adicione várias formas de contactar essa pessoa clicando em "+".
                        </div>

                        <div className="row">
                            <div className="col-sm-3">
                                <div className="form-group">
                                    <label htmlFor="selTpContato">Tipo</label>
                                    <select id="selTpContato" className="form-control"
                                        ref="selTpContato"
                                        value={this.state.contato.tipo}
                                        onChange={(e) => this.setState({ contato: { ...this.state.contato, tipo: e.target.value } })}>
                                        <option value="6">Celular + WhatsApp</option>
                                        <option value="1">Celular</option>
                                        <option value="2">Telefone</option>
                                        <option value="3">WhatsApp</option>
                                        <option value="4">E-mail</option>
                                        <option value="5">Outro</option>
                                    </select>
                                </div>
                            </div>

                            <div className="col-sm-4">
                                <div className="form-group">
                                    <label htmlFor="txtContato">Contato</label>
                                    <input type="text" className="form-control" id="txtContato" autoComplete="off" maxLength="80" value={this.state.contato.contato}
                                        onChange={(e) => this.setState({ contato: { ...this.state.contato, contato: e.target.value } })} />
                                </div>
                            </div>
                            <div className="col-sm-5">
                                <div className="form-group">
                                    <label htmlFor="txtContatoObservacao">Observação</label>
                                    <div className="input-group">
                                        <input type="text" className="form-control" id="txtContatoObservacao" autoComplete="off" maxLength="200" value={this.state.contato.observacao}
                                            onChange={(e) => this.setState({ contato: { ...this.state.contato, observacao: e.target.value } })} />

                                        <span className="ml-3">
                                            <button type="button" className="btn btn-primary" onClick={this.adicionarContato}><i className="fas fa-plus"></i></button>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div className="card">
                            <div className="card-body table-responsive p-0">
                                <table className="table table-hover table-sm table-selectable">
                                    <tbody>
                                        {this.state.contatos.map(item => {
                                            return (
                                                <tr key={item.id}>
                                                    <td><span className="font-weight-bold">{item.contato}</span> ({item.tipoNome})
                                                        {item.observacao != "" ? <div className="text-sm-left">{item.observacao}</div> : null}
                                                    </td>
                                                    <td style={{ width: "50px" }}>
                                                        <div>
                                                            <a className="btn table-action" href="#" role="button" data-toggle="dropdown">
                                                                <i className="action-icon fas fa-ellipsis-v"></i>
                                                            </a>
                                                            <div className="dropdown-menu">
                                                                <a className="dropdown-item" href="#" onClick={(e) => this.editarContato(item)}><i className="fas fa-edit"></i>Editar</a>
                                                                <a className="dropdown-item" href="#" onClick={(e) => this.excluirContato(item)}><i className="far fa-trash-alt"></i>Excluir</a>
                                                            </div>
                                                        </div>
                                                    </td>

                                                </tr>);
                                        })}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>

                </div>


                <CadastroTipo atualizarState={this.setStateTipos} tipos={this.state.tipos} />

            </form>


        let modal =
            <div className="modal fade" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-xl modal-dialog-scrollable">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title">Pessoa</h4>
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