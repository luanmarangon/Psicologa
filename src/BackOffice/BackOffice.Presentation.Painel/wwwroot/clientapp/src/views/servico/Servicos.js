import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import LoadingIndicator from '../../components/LoadingIndicator';

import SaibaMais from './components/SaibaMais';

export default class Index extends Component {

    constructor(props) {
        super(props);

        this.state = {
            iniciando: true,
            aguarde: false,
            saibaMaisModal: false,
            servicoIdSelecionado: null,
            servicos: []
        };
    }

    componentDidMount = () => {

        this.obter()
            .finally(() => {

                this.setState({
                    iniciando: false
                });

            });

    }

    obter = () => {

        this.setState({
            aguarde: true
        });

        return HTTPClient.get("Servico/ObterTodos")
            .then(r => r.json())
            .then(r => {

                if (r.success) {

                    this.setState({
                        servicos: r.data
                    });

                }

            })
            .catch(() => {

                showToastr({
                    type: "error",
                    text: "Erro ao obter serviços."
                });

            })
            .finally(() => {

                this.setState({
                    aguarde: false
                });

            });

    }

    saibaMais = (item) => {

        this.setState({
            saibaMaisModal: true,
            servicoIdSelecionado: item.id
        });

    }

    saibaMaisModalFechar = () => {

        this.setState({
            saibaMaisModal: false,
            servicoIdSelecionado: null
        });

    }

    // renderCard = (item) => {

    //     return (

    //         <div
    //             className="col-xl-4 col-lg-6 col-md-6 mb-4"
    //             key={item.id}
    //         >

    //             <div className="card border-0 shadow-sm h-100 servico-card">

    //                 {/* Imagem */}
    //                 <div className="position-relative">

    //                     <img
    //                         src={item.imagemCapa}
    //                         alt={item.nome}
    //                         className="img-fluid w-100"
    //                         style={{
    //                             height: "240px",
    //                             objectFit: "cover"
    //                         }}
    //                     />

    //                     {/* Modalidades */}
    //                     <div
    //                         className="position-absolute"
    //                         style={{
    //                             left: "15px",
    //                             bottom: "15px"
    //                         }}
    //                     >

    //                         {item.online && (
    //                             <span className="badge badge-primary mr-2 px-3 py-2">
    //                                 <i className="fas fa-video mr-1"></i>
    //                                 Online
    //                             </span>
    //                         )}

    //                         {item.presencial && (
    //                             <span className="badge badge-success px-3 py-2">
    //                                 <i className="fas fa-map-marker-alt mr-1"></i>
    //                                 Presencial
    //                             </span>
    //                         )}

    //                     </div>

    //                 </div>

    //                 {/* Conteúdo */}
    //                 <div className="card-body d-flex flex-column">

    //                     {/* Nome */}
    //                     <h4
    //                         className="font-weight-bold mb-3"
    //                         style={{
    //                             minHeight: "60px"
    //                         }}
    //                     >
    //                         {item.nome}
    //                     </h4>

    //                     {/* Descrição */}
    //                     <p
    //                         className="text-muted flex-grow-1"
    //                         style={{
    //                             lineHeight: "1.7"
    //                         }}
    //                     >
    //                         {item.descricaoCurta}
    //                     </p>

    //                     {/* Infos */}
    //                     <div className="border-top pt-3 mt-3">

    //                         <div className="d-flex justify-content-between align-items-center">

    //                             <span className="text-muted">
    //                                 <i className="far fa-clock mr-2"></i>
    //                                 {item.tempoSessaoMinutos} min
    //                             </span>

    //                             {/* 
    //                             <span className="font-weight-bold">
    //                                 R$ {parseFloat(item.valorSessao).toFixed(2)}
    //                             </span> 
    //                             */}

    //                         </div>

    //                         {/* Botão */}
    //                         <div className="mt-4">

    //                             <a
    //                                 href={`/Servico/${item.url}`}
    //                                 className="btn btn-outline-primary btn-block"
    //                             >
    //                                 Ver detalhes
    //                             </a>

    //                         </div>

    //                     </div>

    //                 </div>

    //             </div>

    //         </div>

    //     );

    // }


    renderServico = (item, index) => {

        const invertido = index % 2 === 1;

        return (

            <div
                className="row align-items-center mb-5 py-4"
                key={item.id}
            >

                {/* Imagem */}
                <div className={`col-lg-6 mb-4 mb-lg-0 ${invertido ? 'order-lg-2' : ''}`}>

                    <div className="servico-imagem-wrapper">

                        <img
                            src={item.imagemCapa}
                            alt={item.nome}
                            className="img-fluid w-100 rounded shadow-sm"
                            style={{
                                height: "420px",
                                objectFit: "cover"
                            }}
                        />

                    </div>

                </div>

                {/* Conteúdo */}
                <div className={`col-lg-6 ${invertido ? 'order-lg-1' : ''}`}>

                    <span className="ps-tag mb-3 d-inline-block">
                        Serviço
                    </span>

                    <h2 className="mb-4 font-weight-bold">
                        {item.nome}
                    </h2>

                    <p
                        className="text-muted mb-4"
                        style={{
                            lineHeight: "1.9",
                            fontSize: "17px"
                        }}
                    >
                        {item.descricao}
                    </p>

                    {/* Infos */}
                    <div className="d-flex flex-wrap mb-4">

                        <div className="mr-4 mb-2">
                            <i className="far fa-clock mr-2 text-primary"></i>
                            {item.tempoSessaoMinutos} minutos
                        </div>

                        {item.online && (
                            <div className="mr-4 mb-2">
                                <i className="fas fa-video mr-2 text-primary"></i>
                                Online
                            </div>
                        )}

                        {item.presencial && (
                            <div className="mb-2">
                                <i className="fas fa-map-marker-alt mr-2 text-primary"></i>
                                Presencial
                            </div>
                        )}

                    </div>

                    {/* Botão */}
                    {/* <a
                        href="#" onClick={(e) => this.saibaMais(item)}
                        className="ps-button"
                    >
                        Quero saber mais
                    </a> */}
                    <button
                        type="button"
                        onClick={() => this.saibaMais(item)}
                        className="ps-button border-0"
                    >
                        Quero saber mais
                    </button>

                </div>

            </div>

        );

    }

    render() {

        return (

            <>

                <div className="container">

                    {this.state.servicos.map((item, index) =>
                        this.renderServico(item, index)
                    )}

                </div>

                {this.state.saibaMaisModal && (

                    <SaibaMais
                        onFechar={this.saibaMaisModalFechar}
                        idEdicao={this.state.servicoIdSelecionado}
                    />

                )}

            </>

        );

    }
}

ReactDOM.render(
    <Index />,
    document.getElementById('root')
);