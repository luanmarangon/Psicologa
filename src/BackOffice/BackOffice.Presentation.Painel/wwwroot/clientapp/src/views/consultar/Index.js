import React, { Component } from 'react';
import ReactDOM from 'react-dom';

export default class Index extends Component {

    constructor(props) {
        super(props);

        this.state = {
            pesquisa: window.textoPesquisa || "",
            resultados: window.dadosConsulta || []
        };
    }

    removerHtml = (texto) => {

        if (!texto)
            return "";

        return texto.replace(/<[^>]*>?/gm, '');

    }

    renderResultado = (item) => {

        const isBlog = item.Tipo === "Blog";

        return (

            <div
                className="col-lg-6 mb-4"
                key={`${item.Url}-${item.Id}`}
            >

                <a
                    href={item.Url}
                    className="text-decoration-none"
                >

                    <div className="card border-0 shadow-sm h-100 resultado-card overflow-hidden">

                        {/* Imagem */}
                        {item.ImagemCapa && (

                            <img
                                src={item.ImagemCapa}
                                alt={item.Titulo}
                                className="w-100"
                                style={{
                                    height: "220px",
                                    objectFit: "cover"
                                }}
                            />

                        )}

                        <div className="card-body">

                            {/* Tipo */}
                            <div className="mb-3">

                                <span className={`badge px-3 py-2 ${isBlog
                                    ? "badge-info"
                                    : "badge-success"
                                    }`}>

                                    <i className={`mr-2 fas ${isBlog
                                        ? "fa-newspaper"
                                        : "fa-heart"
                                        }`}>
                                    </i>

                                    {item.Tipo}
               

                                </span>

                            </div>

                            {/* Título */}
                            <h4 className="font-weight-bold mb-3 text-dark">

                                {item.Titulo}

                            </h4>

                            {/* Resumo */}
                            <p
                                className="text-muted mb-0"
                                style={{
                                    lineHeight: "1.8"
                                }}
                            >

                                {item.Resumo
                                    ? item.Resumo
                                    : this.removerHtml(item.Conteudo).substring(0, 220)}

                            </p>

                        </div>

                    </div>

                </a>

            </div>

        );

    }

    render() {

        return (

            <section className="ps-section bg-light">

                <div className="container">

                    {/* Sem resultados */}
                    {this.state.resultados.length === 0 ? (

                        <div className="text-center py-5">

                            <h3 className="mb-3">
                                Nenhum resultado encontrado
                            </h3>

                            <p className="text-muted mb-0">

                                {this.state.pesquisa.length > 0 ? (
                                    <>
                                        Não encontramos conteúdos relacionados à pesquisa
                                        <strong> "{this.state.pesquisa}"</strong>.
                                    </>
                                ) : (
                                    "Nenhum resultado encontrado."
                                )}

                            </p>

                        </div>

                    ) : (

                        <>

                            {/* Quantidade */}
                            <div className="mb-4">

                                <h5 className="text-muted">

                                    {this.state.resultados.length} resultado(s)
                                    encontrado(s) para
                                    <strong> "{this.state.pesquisa}"</strong>

                                </h5>

                            </div>

                            {/* Resultados */}
                            <div className="row">

                                {this.state.resultados.map(item =>
                                    this.renderResultado(item)
                                )}

                            </div>

                        </>

                    )}

                </div>

            </section>

        );

    }

}

ReactDOM.render(
    <Index />,
    document.getElementById('root')
);