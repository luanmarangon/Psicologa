import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Cadastro extends Component {

    constructor(props) {
        super(props);
        this.state = {
            dados: {
                id: this.props.idEdicao,
                nome: "",
                descricaoCurta: "",
                descricao: "",
                tempoSessaoMinutos: 50,
                valorSessao: 0,
                imagemCapa: "",
                online: false,
                presencial: false,
                destaqueHome: false,
                ordemExibicao: 0,
                ativo: true,
            },
            previewImagem: null, // 👈 NOVO
            iniciando: true,
            obtendo: false,
            aguarde: false,
        };
    }

    // componentDidMount = () => {

    //     let promiseEdicao = null;
    //     if (!isEmpty(this.props.idEdicao)) {
    //         promiseEdicao = this.obter(this.props.idEdicao);
    //         promiseEdicao.then(() => {
    //             // promiseObterCidades = this.obterCidades(this.state.endereco.uf);
    //         });
    //     }
    //     else {
    //         // promiseObterCidades = this.obterCidades(this.state.endereco.uf);
    //     }

    //     Promise.all([promiseEdicao]).then(() => {
    //         this.setState({
    //             iniciando: false,
    //         });
    //     });


    //     $("#cadastroModal").modal('show');

    //     let that = this;

    //     $('#cadastroModal').on('hidden.bs.modal', function (e) {
    //         that.props.onFechar();
    //     });


    // }
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

        // Máscara valor sessão
        $("#txtValorSessao").inputmask('currency', {
            prefix: 'R$ ',
            groupSeparator: '.',
            radixPoint: ',',
            autoGroup: true,
            digits: 2,
            digitsOptional: false,
            rightAlign: false,
            removeMaskOnSubmit: true
        });

        $("#txtValorSessao").on('input', (e) => {

            this.setState({
                dados: {
                    ...this.state.dados,
                    valorSessao:
                        parseFloat(
                            e.target.inputmask.unmaskedvalue()
                        ) / 100
                }
            });

        });

    }

    componentDidUpdate = () => {
    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');
    }

    handleImagem = (e) => {
        const file = e.target.files[0];

        if (file) {

            // 🔥 preview (continua usando blob)
            const preview = URL.createObjectURL(file);

            // 🔥 conversão para base64 (ENVIO)
            const reader = new FileReader();

            reader.onload = () => {
                this.setState({
                    previewImagem: preview,
                    dados: {
                        ...this.state.dados,
                        imagemCapa: reader.result // ✅ BASE64
                    }
                });
            };

            reader.readAsDataURL(file);
        }
    }


    obter = (id) => {

        let p = HTTPClient.get("Administrativo/Servico/Obter?id=" + id)
            .then(r => {
                return r.json();
            })
            .then(r => {

                this.setState({
                    ...this.dados,
                    dados: {
                        ...this.state.dados,
                        nome: r.data.nome,
                        descricaoCurta: r.data.descricaoCurta,
                        descricao: r.data.descricao,
                        tempoSessaoMinutos: r.data.tempoSessaoMinutos,
                        valorSessao: r.data.valorSessao,
                        imagemCapa: r.data.imagemCapa,
                        online: r.data.online,
                        presencial: r.data.presencial,
                        destaqueHome: r.data.destaqueHome,
                        ordemExibicao: r.data.ordemExibicao,
                        ativo: r.data.ativo,

                    },
                    previewImagem: r.data.imagemCapa


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

        HTTPClient.post("Administrativo/Servico/Salvar", this.state.dados)
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
                        <label htmlFor="txtNome">Nome do Serviço*</label>
                        <input type="text" className="form-control" id="txtNome"
                            value={this.state.dados.nome} maxLength="70" autoComplete="off"
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, nome: e.target.value } })} />
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
                </div>
                <div className="row">
                    <div className="col form-group">
                        <label htmlFor="txtDescricaoCurta">
                            Descrição Curta*
                        </label>

                        <textarea className="form-control" id="txtDescricaoCurta" autoComplete="off" maxLength="100" rows="2"
                            value={this.state.dados.descricaoCurta}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, descricaoCurta: e.target.value } })} />

                        <div className="text-right mt-1">
                            <small className="text-muted">
                                {this.state.dados.descricaoCurta?.length || 0}/100
                            </small>
                        </div>
                    </div>
                </div>

                <div className="row">
                    <div className="col form-group">
                        <label htmlFor="txtDescricao">
                            Descrição*
                        </label>

                        <textarea className="form-control" id="txtDescricao" autoComplete="off" maxLength="500" rows="7"
                            value={this.state.dados.descricao}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, descricao: e.target.value } })} />

                        <div className="text-right mt-1">
                            <small className="text-muted">
                                {this.state.dados.descricao?.length || 0}/500
                            </small>
                        </div>
                    </div>
                </div>

                <div className="row">
                    <div className="col form-group">
                        <label htmlFor="txtOrdemExibicao">Ordem de Exibição*</label>
                        <input type="number" className="form-control" id="txtOrdemExibicao"
                            value={this.state.dados.ordemExibicao} autoComplete="off"
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, ordemExibicao: e.target.value } })} />
                    </div>
                    <div className="col form-group">
                        <label htmlFor="txtTempoSessaoMinutos">Tempo de Sessão (minutos)*</label>
                        <input type="number" className="form-control" id="txtTempoSessaoMinutos"
                            value={this.state.dados.tempoSessaoMinutos} autoComplete="off"
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, tempoSessaoMinutos: e.target.value } })} />
                    </div>
                    <div className="col form-group">
                        <label htmlFor="txtValorSessao">Valor da Sessão (R$)*</label>
                        <input type="text" className="form-control" id="txtValorSessao"
                            value={this.state.dados.valorSessao} autoComplete="off" step="0.01"
                            // defaultValue={this.state.dados.valorSessao}
                            onChange={(e) => this.setState({ dados: { ...this.state.dados, valorSessao: e.target.value } })} />

                        {/* <input
                            type="text"
                            className="form-control"
                            id="txtValorSessao"
                            defaultValue={this.state.dados.valorSessao || ""}
                        /> */}

                    </div>
                </div>

                <div className="row">

                    <div className="col-md-6">
                        <div className="form-group">

                            <label className="d-block mb-2">
                                Modalidade
                            </label>

                            <div className="form-check form-check-inline">
                                <input className="form-check-input" type="checkbox" id="txtOnline" checked={this.state.dados.online}
                                    onChange={(e) => this.setState({ dados: { ...this.state.dados, online: e.target.checked } })} />
                                <label className="form-check-label" htmlFor="txtOnline">Online</label>
                            </div>

                            <div className="form-check form-check-inline">
                                <input className="form-check-input" type="checkbox" id="txtPresencial" checked={this.state.dados.presencial}
                                    onChange={(e) => this.setState({ dados: { ...this.state.dados, presencial: e.target.checked } })} />
                                <label className="form-check-label" htmlFor="txtPresencial">Presencial</label>
                            </div>
                            <div className="form-check form-check-inline">
                                <input className="form-check-input" type="checkbox" id="txtDestaque" checked={this.state.dados.destaqueHome}
                                    onChange={(e) => this.setState({ dados: { ...this.state.dados, destaqueHome: e.target.checked } })} />
                                <label className="form-check-label" htmlFor="txtDestaque">Destaque Home</label>
                            </div>

                        </div>
                    </div>

                </div>

                <div className="row mt-4">
                    <div className="col-lg-12">
                        <div className="card border-0 shadow-sm" style={{ borderRadius: "16px", overflow: "hidden" }}>
                            <div className="card-body">
                                <div className="d-flex justify-content-between align-items-center mb-3">
                                    <div>
                                        <h5 className="mb-1 font-weight-bold">Imagem de Capa</h5>
                                        <small className="text-muted">Escolha uma imagem para representar o serviço</small>
                                    </div>
                                    {this.state.previewImagem && (<span className="badge badge-success px-3 py-2"><i className="fas fa-check-circle mr-1"></i>Imagem carregada</span>)}
                                </div>

                                {/* Upload */}
                                <div className="border rounded p-4 text-center" style={{ background: "#f8fafc", borderStyle: "dashed", cursor: "pointer" }}>
                                    <input className="d-none" type="file" id="uploadImagem" accept="image/*" onChange={this.handleImagem} />
                                    <label htmlFor="uploadImagem" className="mb-0 w-100" style={{ cursor: "pointer" }}>
                                        {this.state.previewImagem ? (<img src={this.state.previewImagem} alt="Preview" className="img-fluid rounded shadow-sm"
                                            style={{ maxHeight: "260px", objectFit: "cover" }} />
                                        ) : (
                                            <div className="py-4">
                                                <div className="mb-3" style={{ fontSize: "42px", color: "#94a3b8" }}><i className="fas fa-cloud-upload-alt"></i></div>
                                                <h6 className="font-weight-bold mb-2">Clique para enviar uma imagem</h6>
                                                <p className="text-muted mb-0" style={{ fontSize: "14px" }}>PNG, JPG ou WEBP</p>
                                            </div>
                                        )
                                        }
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </form>

        let modal =
            <div className="modal fade" id="cadastroModal" data-keyboard="true" tabIndex="-1">
                <div className="modal-dialog modal-xl modal-dialog-scrollable">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h4 className="modal-title">Dados do Serviço</h4>
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

