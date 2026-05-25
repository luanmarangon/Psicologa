import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { createRoot } from 'react-dom/client';

import ConfiguracaoEmpresa from './components/ConfiguracaoEmpresa';
import ConfiguracaoFuncionamento from './components/ConfiguracaoFuncionamento';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            // resultadoPesquisa: [],
            // legendaResultadoPesquisa: "Últimos usuários",
            confEmpresaModal: false,
            confFuncionamentoModal: false,
            // usuarioIdSelecionado: "",
            // pessoaIdSelecionado: "",
            // pessoaSelecionado: ""
        };
    }

    componentDidMount = () => {

    }

    componentDidUpdate = () => {
        tableSelectable();
    }



    confEmpresaModalAbrir = (item) => {
        this.setState({
            confEmpresaModal: true,
        });
    }

    confEmpresaModalFechar = (usuario) => {
        this.setState({
            confEmpresaModal: false,
        });
    }
    confFuncionamentoModalAbrir = (item) => {
        this.setState({
            confFuncionamentoModal: true,
        });
    }

    confFuncionamentoModalFechar = (usuario) => {
        this.setState({
            confFuncionamentoModal: false,
        });
    }

    renderItem = (label, textoBotao, icone, onClick) => {
        return (
            <div
                className="p-4"
                style={{
                    background: 'linear-gradient(135deg, #f8fafc 0%, #eef2ff 100%)',
                    borderBottom: '1px solid #e5e7eb'
                }}
            >
                <label className="mb-2 d-block font-weight-bold">{label}</label>
                <div>
                    <button
                        className="btn btn-primary px-4 py-2"
                        style={{ borderRadius: 10, fontWeight: 600, boxShadow: '0 10px 25px rgba(59,130,246,0.18)' }}
                        onClick={onClick}
                    >
                        <i className={`fas fa-${icone} mr-2`}></i>
                        {textoBotao}
                    </button>
                </div>
            </div>
        );
    }


    render() {
        const { confEmpresaModal: confEmpresa, confFuncionamentoModal: confFuncionamento } = this.state;

        return (
            <div className="card border-0 shadow-sm rounded-lg overflow-hidden">

                {this.renderItem(
                    'Configuração',
                    'Configuração',
                    'cog',
                    () => this.confEmpresaModalAbrir({ id: "", pessoaId: "", pessoaNome: "" })
                )}
                {this.renderItem(
                    'Horário de Funcionamento',
                    'Funcionamento',
                    'clock',
                    () => this.confFuncionamentoModalAbrir({ id: "", pessoaId: "", pessoaNome: "" })
                )}

                {/* futuros itens */}
                {/* {this.renderItem('Outro item', 'Acessar', 'user', () => this.outroModal())} */}

                {confEmpresa && (<ConfiguracaoEmpresa onFechar={this.confEmpresaModalFechar} />)}
                {confFuncionamento && (<ConfiguracaoFuncionamento onFechar={this.confFuncionamentoModalFechar} />)}
            </div>
        );
    }
}

createRoot(document.getElementById('root')).render(<React.StrictMode> <Index /> </React.StrictMode>);