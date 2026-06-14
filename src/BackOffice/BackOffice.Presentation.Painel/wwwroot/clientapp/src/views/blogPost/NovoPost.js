import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Cadastro from './components/Cadastro';
import LoadingIndicator from '../../components/LoadingIndicator';

import { CKEditor } from '@ckeditor/ckeditor5-react';
import ClassicEditor from '@ckeditor/ckeditor5-build-classic';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            previewImagem: null, // 👈 NOVO
            dados: {
                blogId: props.idEdicao || null,
                titulo: "",
                conteudo: "",
                imagemCapa: null,
                autor: "",
                dataRevogacao: null,
                dataPublicacao: null,
                ativo: false,
            },
        };
    }

    componentDidMount = () => {
        const params = new URLSearchParams(window.location.search);
        const id = params.get("id");

        if (id) {
            this.obter(id);
        }
        console.log(id);
    }

    componentDidUpdate = () => {
        tableSelectable();
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

    // 🔥 UPLOAD CKEDITOR
    uploadAdapter = (loader) => {
        return {
            upload: () => {
                return loader.file.then(file => {

                    const data = new FormData();
                    data.append('file', file);

                    return fetch('/api/upload', {
                        method: 'POST',
                        body: data
                    })
                        .then(res => res.json())
                        .then(result => {
                            return {
                                default: result.url
                            };
                        });
                });
            }
        };
    }

    uploadPlugin = (editor) => {
        editor.plugins.get('FileRepository').createUploadAdapter = (loader) => {
            return this.uploadAdapter(loader);
        };
    }

    obter = (id) => {
        this.setState({ aguarde: true });

        HTTPClient.get("Administrativo/BlogPost/Obter?id=" + id)
            .then(r => r.json())
            .then(r => {
                if (r.success) {
                    this.setState({
                        dados: r.data,
                        previewImagem: r.data.imagemCapa
                    });
                }
            })
            .finally(() => {
                this.setState({ aguarde: false });
            });
    }

    salvar = () => {

        this.setState({
            aguarde: true
        });


        HTTPClient.post("Administrativo/BlogPost/Salvar", this.state.dados)
            // .then(r => {
            //     return r.json();
            // })
            // .then(r => {

            //     if (r.success) {
            //         this.props.onFechar(r.data);
            //         showToastr(r.messages);
            //     }
            //     else showToastr(r.messages);

            // })
            .then(r => {
                console.log("STATUS:", r.status);
                return r.text(); // 👈 ao invés de json()
            })
            .then(text => {
                console.log("RESPOSTA BRUTA:", text);

                const r = JSON.parse(text); // pode quebrar aqui e mostrar o erro real

                if (r.success) {
                    // this.props.onFechar(r.data);
                    showToastr(r.messages);
                } else {
                    showToastr(r.messages);
                }
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

        let saida =
            <div className="card card-secondary card-outline">
                <div className="card-body">

                    {/* Linha 1 */}
                    <div className="row g-3">
                        <div className="col-md-8">
                            <label className="form-label">Título *</label>
                            <input type="text" className="form-control"
                                value={this.state.dados.titulo}
                                onChange={(e) => this.setState({ dados: { ...this.state.dados, titulo: e.target.value } })} />
                        </div>

                        <div className="col-md-2">
                            <label className="form-label">Data Publicação</label>
                            <input type="date" className="form-control" value={this.state.dados.dataPublicacao} onChange={(e) => this.setState({ dados: { ...this.state.dados, dataPublicacao: e.target.value } })} />
                        </div>

                        <div className="col-md-2">
                            <label className="form-label">Ativo?</label>
                            <select className="form-control" value={this.state.dados.ativo} onChange={(e) => this.setState({ dados: { ...this.state.dados, ativo: e.target.value } })}>
                                <option value={true}>Sim</option>
                                <option value={false}>Não</option>
                            </select>
                        </div>
                    </div>

                    {/* Imagem */}
                    <div className="row mt-3">
                        <div className="col-md-12">
                            <label className="form-label">Imagem de Capa</label>
                            <input
                                className="form-control"
                                type="file"
                                accept="image/*"
                                onChange={this.handleImagem}
                            />
                        </div>
                    </div>

                    {/* Preview imagem */}
                    {this.state.previewImagem && (
                        <div className="row mt-3">
                            <div className="col-md-12 text-center">
                                <img
                                    src={this.state.previewImagem}
                                    alt="Preview"
                                    className="img-fluid rounded shadow"
                                    style={{ maxHeight: '200px' }}
                                />
                            </div>
                        </div>
                    )}

                    {/* 🚀 CKEditor COM UPLOAD */}
                    <div className="row mt-3">
                        <div className="col-md-12">
                            <label className="form-label">Conteúdo</label>

                            <CKEditor
                                editor={ClassicEditor}
                                data={this.state.dados.conteudo || ''}
                                config={{
                                    extraPlugins: [this.uploadPlugin],
                                    toolbar: [
                                        'heading',
                                        '|',
                                        'bold', 'italic', 'underline', 'strikethrough',
                                        'fontSize', 'fontColor', 'fontBackgroundColor',
                                        '|',
                                        'alignment',
                                        '|',
                                        'link',
                                        'bulletedList', 'numberedList',
                                        '|',
                                        'blockQuote',
                                        'code', 'codeBlock',
                                        '|',
                                        'imageUpload',
                                        'imageStyle:inline', 'imageStyle:block', 'imageStyle:side',
                                        '|',
                                        'insertTable',
                                        'tableColumn', 'tableRow', 'mergeTableCells',
                                        '|',
                                        'htmlEmbed',
                                        'sourceEditing',
                                        '|',
                                        'undo', 'redo'
                                    ]
                                }}
                                onChange={(event, editor) => {
                                    const data = editor.getData();
                                    this.setState({ dados: { ...this.state.dados, conteudo: data } });
                                }}
                            />
                        </div>
                    </div>

                    {/* Linha final */}
                    <div className="row g-3 mt-3">
                        <div className="col-md-8">
                            <label className="form-label">Autor</label>
                            <input type="text" className="form-control" value={this.state.dados.autor} onChange={(e) => this.setState({ dados: { ...this.state.dados, autor: e.target.value } })} />
                        </div>

                        <div className="col-md-2">
                            <label className="form-label">Data Revogação</label>
                            <input type="date" className="form-control" value={this.state.dados.dataRevogacao} onChange={(e) => this.setState({ dados: { ...this.state.dados, dataRevogacao: e.target.value } })} />
                        </div>

                        <div className="col-md-2 d-flex align-items-end gap-2">
                            <button className="btn btn-success w-100 mr-2" onClick={this.salvar}>Salvar</button>
                            <button className="btn btn-secondary w-100">Cancelar</button>
                        </div>
                    </div>

                </div>
            </div>;

        return (saida);
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));