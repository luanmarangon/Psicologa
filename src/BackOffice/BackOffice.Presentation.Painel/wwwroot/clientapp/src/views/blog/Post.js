import React, { Component } from 'react';
import ReactDOM from 'react-dom';
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
        const url = window.location.pathname;
        const partes = url.split('/');

        const blogUrl = partes[partes.length - 1];

        if (blogUrl) {
            this.obter(blogUrl);
        }
        console.log(blogUrl);
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

    obter = (url) => {
        this.setState({ aguarde: true });

        HTTPClient.get("Blog/ObterPorUrl?url=" + url)
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


    render() {

        const { aguarde, dados } = this.state;

        if (aguarde) {
            return (
                <div className="container py-5 text-center">
                    <LoadingIndicator />
                </div>
            );
        }

        return (

            <div className="ps-post-page">

                {/* Hero */}
                <section className="ps-post-hero">

                    <div className="container">

                        <div className="ps-post-header">

                            <a href="/Blog">                        <span className="ps-post-badge">
                                Blog • Psicologia
                            </span>
                            </a>
                            <h1 className="ps-post-title">
                                {dados.titulo}
                            </h1>

                            <div className="ps-post-meta">

                                <span>
                                    <i className="fas fa-user mr-2"></i>
                                    {dados.autor}
                                </span>

                                <span>
                                    <i className="far fa-calendar-alt mr-2"></i>

                                    {formatarDataInputDateToPtBr(dados.dataPublicacao)}
                                </span>

                            </div>

                        </div>

                    </div>

                </section>

                {/* Imagem */}
                {
                    dados.imagemCapa && (
                        <div className="container">

                            <div className="ps-post-cover">

                                <img
                                    src={dados.imagemCapa}
                                    alt={dados.titulo}
                                />

                            </div>

                        </div>
                    )
                }

                {/* Conteúdo */}
                <section className="ps-post-content-area">

                    <div className="container">

                        <div className="row justify-content-center">

                            <div className="col-lg-9">

                                <article
                                    className="ps-post-content"
                                    dangerouslySetInnerHTML={{
                                        __html: dados.conteudo
                                    }}
                                />

                            </div>

                        </div>

                    </div>

                </section>

            </div>

        );
    }
}

ReactDOM.render(<Index />, document.getElementById('root'));