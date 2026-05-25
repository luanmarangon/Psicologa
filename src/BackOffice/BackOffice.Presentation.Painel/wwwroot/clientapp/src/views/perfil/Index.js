import React, { Component } from 'react';
import { createRoot } from 'react-dom/client';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            aguarde: false,
            perfil: null,
        };
    }

    componentDidMount = () => {
        this.obter();
    }

    obter = () => {
        this.setState({ aguarde: true });

        HTTPClient.get("Administrativo/Perfil/ObterPerfil", false)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                this.setState({ perfil: r.data });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Um erro ocorreu ao obter o Perfil." });
            })
            .finally(() => {
                this.setState({ aguarde: false });
            });
    }

    obterSexo = (sexo) => {
        switch (sexo) {
            case 1: return "Masculino";
            case 2: return "Feminino";
            default: return "Não informado";
        }
    }

    renderContato = (contato, index) => {
        return (
            <div key={index} className="info-item">
                <div className="info-icon">
                    <i className={
                        contato.tipo === 4
                            ? "fas fa-envelope"
                            : contato.tipo === 1 || contato.tipo === 6
                                ? "fas fa-mobile-alt"
                                : "fas fa-phone"
                    }></i>
                </div>
                <div>
                    <div className="info-label">{contato.tipoNome}</div>
                    <div className="info-value">{contato.contato}</div>
                </div>
            </div>
        );
    }

    render() {

        const { aguarde, perfil } = this.state;

        if (aguarde) {
            return (
                <div className="mt-5 mb-5">
                    <LoadingIndicator />
                </div>
            );
        }

        if (!perfil) return null;

        const { dados, endereco, contatos, tipos } = perfil;

        return (
            <div className="perfil-wrapper">

                <div className="perfil-header">
                    <div className="perfil-avatar">
                        {dados.nome?.charAt(0)}
                    </div>
                    <div>
                        <h2 className="perfil-nome">{dados.nome}</h2>
                        <div className="perfil-tipos">
                            {tipos?.map((t, index) => (
                                <span key={index} className="perfil-badge">{t.tipoNome}</span>
                            ))}
                        </div>
                    </div>
                </div>

                <div className="row">

                    {/* Dados pessoais */}
                    <div className="col-lg-6 mb-4">
                        <div className="modern-card">
                            <h4 className="card-title"><i className="fas fa-user mr-2"></i>Dados pessoais</h4>
                            <div className="info-grid">
                                <div className="info-item">
                                    <div className="info-label">CPF</div>
                                    <div className="info-value">{dados.docIdNro}</div>
                                </div>
                                <div className="info-item">
                                    <div className="info-label">Nascimento</div>
                                    <div className="info-value">{dados.dataNascimento}</div>
                                </div>
                                <div className="info-item">
                                    <div className="info-label">Sexo</div>
                                    <div className="info-value">{this.obterSexo(dados.sexo)}</div>
                                </div>
                                <div className="info-item">
                                    <div className="info-label">Status</div>
                                    <div className="info-value">{dados.ativo ? "Ativo" : "Inativo"}</div>
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Endereço */}
                    <div className="col-lg-6 mb-4">
                        <div className="modern-card">
                            <h4 className="card-title"><i className="fas fa-map-marker-alt mr-2"></i>Endereço</h4>
                            <div className="info-grid">
                                <div className="info-item">
                                    <div className="info-label">Logradouro</div>
                                    <div className="info-value">{endereco.logradouro}, {endereco.numero}</div>
                                </div>
                                <div className="info-item">
                                    <div className="info-label">Bairro</div>
                                    <div className="info-value">{endereco.bairro}</div>
                                </div>
                                <div className="info-item">
                                    <div className="info-label">Cidade</div>
                                    <div className="info-value">{endereco.cidade} - {endereco.uf}</div>
                                </div>
                                <div className="info-item">
                                    <div className="info-label">CEP</div>
                                    <div className="info-value">{endereco.cep}</div>
                                </div>
                                {endereco.complemento && (
                                    <div className="info-item">
                                        <div className="info-label">Complemento</div>
                                        <div className="info-value">{endereco.complemento}</div>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>

                    {/* Contatos */}
                    <div className="col-lg-12">
                        <div className="modern-card">
                            <h4 className="card-title"><i className="fas fa-address-book mr-2"></i>Contatos</h4>
                            <div className="row">
                                {contatos?.map((contato, index) => (
                                    <div className="col-md-6 col-lg-4 mb-3" key={index}>
                                        {this.renderContato(contato, index)}
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>

                    {/* Segurança */}
                    <div className="col-lg-12 mt-4">
                        <div className="modern-card">
                            <div className="d-flex flex-wrap align-items-center justify-content-between">
                                <div>
                                    <h4 className="card-title mb-2"><i className="fas fa-shield-alt mr-2"></i>Segurança da conta</h4>
                                    <p className="card-subtitle mb-0">Atualize sua senha periodicamente para manter sua conta protegida.</p>
                                </div>
                                <a href="/Login/RecuperarSenha" className="btn-alterar-senha">
                                    <i className="fas fa-lock mr-2"></i>Alterar senha
                                </a>
                            </div>
                        </div>
                    </div>

                </div>

            </div> // ← essa div fechando a perfil-wrapper estava faltando
        );
    }
}

createRoot(document.getElementById('root')).render(
    <React.StrictMode>
        <Index />
    </React.StrictMode>
);