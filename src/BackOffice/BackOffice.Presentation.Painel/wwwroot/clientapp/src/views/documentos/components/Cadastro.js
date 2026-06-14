import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

import EditorTexto from '../../../components/EditorTexto';
// import { CKEditor } from '@ckeditor/ckeditor5-react';
// import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
// import { Alignment } from '@ckeditor/ckeditor5-alignment';
// import { Font } from '@ckeditor/ckeditor5-font';
// import { Underline, Strikethrough } from '@ckeditor/ckeditor5-basic-styles';

export default class Cadastro extends Component {

    constructor(props) {
        super(props);

        this.editor = null;

        this.state = {
            dados: {
                id: this.props.idEdicao,
                nome: "",
                categoria: "Declaracao",
                ativo: true,
                conteudo: ""
            },

            variaveis: [
                "{{PacienteNome}}",
                "{{PacienteCpf}}",
                "{{PacienteNascimento}}",
                "{{PacienteIdade}}",
                "{{PacienteTelefone}}",
                "{{PacienteEmail}}",
                "{{ResponsavelNome}}",
                "{{ResponsavelCpf}}",
                "{{PsicologoNome}}",
                "{{PsicologoCRP}}",
                "{{DataSessao}}",
                "{{QuantidadeSessoes}}",
                "{{DataAtual}}"
            ],

            iniciando: true,
            obtendo: false,
            aguarde: false
        };
    }

    componentDidMount = () => {

        let promiseEdicao = null;

        if (!isEmpty(this.props.idEdicao)) {
            promiseEdicao = this.obter(this.props.idEdicao);
        }

        Promise.all([promiseEdicao]).then(() => {

            this.setState({
                iniciando: false
            });

        });

        $("#cadastroModal").modal('show');

        $('#cadastroModal').on('hidden.bs.modal', () => {
            this.props.onFechar();
        });

    }

    componentWillUnmount = () => {

        $('#cadastroModal').modal('hide');

    }

    obter = (id) => {

        let p = HTTPClient.get("Administrativo/Documentos/Obter?id=" + id)
            .then(r => r.json())
            .then(r => {

                this.setState({
                    dados: {
                        ...this.state.dados,
                        ...r.data
                    }
                });

            })
            .catch(() => {

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

        HTTPClient.post(
            "Administrativo/Documentos/Salvar",
            this.state.dados
        )
            .then(r => r.json())
            .then(r => {

                if (r.success) {

                    this.props.onFechar(r.data);

                    showToastr(r.messages);

                } else {

                    showToastr(r.messages);

                }

            })
            .catch(() => {

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

    inserirVariavel = (variavel) => {

        if (!this.editor)
            return;

        this.editor.model.change(writer => {

            this.editor.model.insertContent(
                writer.createText(variavel)
            );

        });

    }

    gerarPreview = () => {

        let html = this.state.dados.conteudo || "";

        const valores = {
            "{{PacienteNome}}": "João da Silva",
            "{{PacienteCpf}}": "123.456.789-00",
            "{{PacienteNascimento}}": "15/03/1990",
            "{{PacienteIdade}}": "36",
            "{{PacienteTelefone}}": "(18) 99999-9999",
            "{{PacienteEmail}}": "joao@email.com",
            "{{ResponsavelNome}}": "Maria da Silva",
            "{{ResponsavelCpf}}": "111.222.333-44",
            "{{PsicologoNome}}": "Dra. Ana Oliveira",
            "{{PsicologoCRP}}": "06/123456",
            "{{DataSessao}}": "10/06/2026",
            "{{QuantidadeSessoes}}": "12",
            "{{DataAtual}}": new Date().toLocaleDateString('pt-BR')
        };

        Object.keys(valores).forEach(chave => {

            html = html.replaceAll(
                chave,
                valores[chave]
            );

        });

        return html;

    }

    render() {

        const form =
            <form role="form">

                <div className="row">

                    <div className="col-md-7 form-group">
                        <label>Nome do Modelo *</label>

                        <input
                            type="text"
                            className="form-control"
                            maxLength="100"
                            autoComplete="off"
                            value={this.state.dados.nome}
                            onChange={(e) =>
                                this.setState({
                                    dados: {
                                        ...this.state.dados,
                                        nome: e.target.value
                                    }
                                })
                            }
                        />
                    </div>

                    <div className="col-md-3 form-group">
                        <label>Categoria</label>

                        <select
                            className="form-control"
                            value={this.state.dados.categoria}
                            onChange={(e) =>
                                this.setState({
                                    dados: {
                                        ...this.state.dados,
                                        categoria: e.target.value
                                    }
                                })
                            }
                        >
                            <option value="Declaracao">Declaração</option>
                            <option value="Atestado">Atestado</option>
                            <option value="Relatorio">Relatório</option>
                            <option value="Laudo">Laudo</option>
                            <option value="Parecer">Parecer</option>
                            <option value="Termo">Termo</option>
                            <option value="Encaminhamento">Encaminhamento</option>
                            <option value="Outro">Outro</option>
                        </select>
                    </div>

                    <div className="col-md-2 form-group">
                        <label>Status</label>

                        <select
                            className="form-control"
                            value={this.state.dados.ativo ? "1" : "0"}
                            onChange={(e) =>
                                this.setState({
                                    dados: {
                                        ...this.state.dados,
                                        ativo: e.target.value === "1"
                                    }
                                })
                            }
                        >
                            <option value="1">Ativo</option>
                            <option value="0">Inativo</option>
                        </select>
                    </div>

                </div>

                <div className="card mb-3">

                    <div className="card-header">
                        Variáveis Disponíveis
                    </div>

                    <div className="card-body">

                        {
                            this.state.variaveis.map((item, index) => (

                                <button
                                    key={index}
                                    type="button"
                                    className="btn btn-sm btn-outline-primary mr-1 mb-1"
                                    onClick={() => this.inserirVariavel(item)}
                                >
                                    {item}
                                </button>

                            ))
                        }

                    </div>

                </div>

                <div className="row">

                    <div className="col-md-8">

                        <label>Conteúdo</label>

                        {/* <CKEditor
                            editor={ClassicEditor}
                            data={this.state.dados.conteudo || ''}
                            config={{
                                extraPlugins: [Alignment, Font, Underline, Strikethrough],
                                toolbar: [
                                    'heading', '|',
                                    'bold', 'italic', 'underline', 'strikethrough',
                                    'fontSize', 'fontColor', 'fontBackgroundColor',
                                    '|',
                                    'alignment',
                                    '|',
                                    'link',
                                    'bulletedList', 'numberedList',
                                    'indent', 'outdent',
                                    '|',
                                    'blockQuote',
                                    '|',
                                    'uploadImage',
                                    'imageStyle:inline', 'imageStyle:block', 'imageStyle:side',
                                    '|',
                                    'insertTable',
                                    'tableColumn', 'tableRow', 'mergeTableCells',
                                    '|',
                                    'undo', 'redo'
                                ]
                            }}
                            onReady={(editor) => {

                                this.editor = editor;
                                console.log('Plugins disponíveis:', [...editor.ui.componentFactory.names()]);
                            }}
                            onChange={(event, editor) => {

                                const data = editor.getData();

                                this.setState({
                                    dados: {
                                        ...this.state.dados,
                                        conteudo: data
                                    }
                                });

                            }}
                        /> */}
                        <EditorTexto
                            value={this.state.dados.conteudo || ''}
                            onChange={(html) => this.setState({
                                dados: { ...this.state.dados, conteudo: html }
                            })}
                        />
                    </div>

                    <div className="col-md-4">

                        <div className="card">

                            <div className="card-header">
                                Visualização
                            </div>

                            <div
                                className="card-body"
                                style={{
                                    minHeight: '650px',
                                    maxHeight: '650px',
                                    overflowY: 'auto',
                                    backgroundColor: '#fff',
                                    border: '1px solid #dee2e6'
                                }}
                                dangerouslySetInnerHTML={{
                                    __html: this.gerarPreview()
                                }}
                            />

                        </div>

                    </div>

                </div>

            </form>;

        return (

            <div
                className="modal fade"
                id="cadastroModal"
                data-keyboard="true"
                tabIndex="-1"
            >

                <div
                    className="modal-dialog modal-dialog-scrollable"
                    style={{
                        maxWidth: "95%",
                        width: "95%"
                    }}
                >

                    <div className="modal-content">

                        <div className="modal-header">

                            <h4 className="modal-title">
                                Modelo de Documento
                            </h4>

                            <button
                                type="button"
                                className="close"
                                data-dismiss="modal"
                                aria-label="Close"
                            >
                                <span aria-hidden="true">
                                    &times;
                                </span>
                            </button>

                        </div>

                        <div className="modal-body">

                            {
                                !this.state.iniciando
                                    ? form
                                    : <LoadingIndicator />
                            }

                        </div>

                        <div className={
                            "modal-footer " +
                            (this.state.aguarde ? "site-disabled" : "")
                        }>

                            <button
                                type="button"
                                className="btn btn-secondary"
                            >
                                Visualizar PDF
                            </button>

                            <button
                                type="button"
                                className="btn btn-primary"
                                onClick={this.salvar}
                            >
                                Salvar
                            </button>

                        </div>

                    </div>

                </div>

            </div>

        );
    }
}