import React, { Component } from 'react';
import { createRoot } from 'react-dom/client';

import Agendamento from './components/Agendamento';
import LoadingIndicator from '../../components/LoadingIndicator';

const estilos = `
.sg { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 16px; padding: 8px 0; }
.sc { background: #fff; border: 1px solid #e5e7eb; border-radius: 12px; overflow: hidden; display: flex; flex-direction: column; transition: box-shadow .2s, transform .2s; }
.sc:hover { box-shadow: 0 4px 18px rgba(0,0,0,.08); transform: translateY(-2px); }
.si { height: 160px; overflow: hidden; background: #f1f3f5; position: relative; display: flex; align-items: center; justify-content: center; }
.si img { width: 100%; height: 100%; object-fit: cover; display: block; }
.si-ph { display: flex; flex-direction: column; align-items: center; justify-content: center; gap: 8px; width: 100%; height: 100%; }
.si-ph-icon { width: 52px; height: 52px; border-radius: 50%; background: #fff; border: 1px solid #e5e7eb; display: flex; align-items: center; justify-content: center; }
.si-ph-txt { font-size: 11px; color: #9ca3af; letter-spacing: .04em; }
.si-badge { position: absolute; bottom: 8px; right: 8px; background: rgba(0,0,0,.55); color: #fff; font-size: 11px; padding: 2px 8px; border-radius: 99px; }
.sb { padding: 14px 16px; display: flex; flex-direction: column; gap: 6px; flex: 1; }
.sn { font-size: 14px; font-weight: 600; color: #111827; margin: 0; line-height: 1.35; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.sd { font-size: 12px; color: #6b7280; margin: 0; line-height: 1.5; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.sm { display: flex; align-items: center; gap: 5px; font-size: 12px; color: #9ca3af; }
.sf { margin-top: auto; padding-top: 10px; border-top: 1px solid #f3f4f6; display: flex; align-items: center; justify-content: space-between; }
.sp { font-size: 15px; font-weight: 700; color: #2563eb; }
.sa { background: transparent; color: #2563eb; border: 1px solid #2563eb; padding: 5px 14px; border-radius: 8px; font-size: 12px; font-weight: 500; cursor: pointer; transition: all .15s; }
.sa:hover { background: #2563eb; color: #fff; }
`;

const IconPin = () => (
    <svg width="12" height="12" viewBox="0 0 16 16" fill="none">
        <path d="M8 2C5.24 2 3 4.24 3 7c0 4 5 9 5 9s5-5 5-9c0-2.76-2.24-5-5-5z" stroke="currentColor" strokeWidth="1.5" fill="none" />
    </svg>
);

const IconUser = () => (
    <svg width="12" height="12" viewBox="0 0 16 16" fill="none">
        <circle cx="8" cy="6" r="3" stroke="currentColor" strokeWidth="1.5" />
        <path d="M3 14c0-2.76 2.24-5 5-5s5 2.24 5 5" stroke="currentColor" strokeWidth="1.5" fill="none" />
    </svg>
);

const IconImg = () => (
    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="#9ca3af" strokeWidth="1.5">
        <rect x="3" y="3" width="18" height="18" rx="2" />
        <path d="M3 15l5-5 4 4 3-3 4 4" />
        <circle cx="8.5" cy="8.5" r="1.5" />
    </svg>
);

const formatarTempo = (min) => {
    if (!min) return null;
    if (min < 60) return `${min} min`;
    const h = Math.floor(min / 60);
    const m = min % 60;
    return m > 0 ? `${h}h ${m}min` : `${h}h`;
};

export default class Index extends Component {

    constructor(props) {
        super(props);
        this.state = {
            pesquisar: "",
            iniciando: true,
            aguarde: false,
            resultadoPesquisa: [],
            cadastroModal: false,
            servicoIdSelecionado: ""
        };
    }

    componentDidMount = () => {
        this.pesquisar().finally(() => this.setState({ iniciando: false }));
    }

    pesquisar = () => {
        this.setState({ aguarde: true });
        return HTTPClient.get("Administrativo/Cliente/ObterServicos")
            .then(r => r.json())
            .then(r => { this.setState({ resultadoPesquisa: r.data || [] }); })
            .catch(() => showToastr({ type: "error", text: "Erro ao carregar serviços." }))
            .finally(() => this.setState({ aguarde: false }));
    }

    cadastroModalAbrir = (item) => {
        this.setState({ cadastroModal: true, servicoIdSelecionado: item.id });
    }

    cadastroModalFechar = () => {
        this.setState({ cadastroModal: false, servicoIdSelecionado: "" });
    }

    render() {
        // const filtrados = this.state.resultadoPesquisa.filter(item =>
        //     item.nome?.toLowerCase().includes(this.state.pesquisar.toLowerCase())
        // );
        const filtrados = this.state.resultadoPesquisa.filter(item => {
            const termo = this.state.pesquisar.toLowerCase();
            return (
                item.nome?.toLowerCase().includes(termo) ||
                item.dados?.nome?.toLowerCase().includes(termo) ||
                item.endereco?.cidade?.toLowerCase().includes(termo) ||
                item.descricao?.toLowerCase().includes(termo)
            );
        });

        return (
            <div style={{ padding: '0 4px' }}>
                <style>{estilos}</style>

                {/* Busca */}
                <div style={{ display: 'flex', justifyContent: 'flex-end', marginBottom: 16 }}>
                    <div style={{ display: 'flex', gap: 0, width: 300 }}>
                        <input
                            type="text"
                            className="form-control"
                            placeholder="Buscar serviço..."
                            style={{ borderRadius: '8px 0 0 8px' }}
                            onChange={e => this.setState({ pesquisar: e.target.value })}
                        />
                        <button
                            className="btn btn-primary"
                            style={{ borderRadius: '0 8px 8px 0', padding: '0 14px' }}
                        >
                            <i className="fas fa-search"></i>
                        </button>
                    </div>
                </div>

                {/* Contador */}
                {!this.state.aguarde && filtrados.length > 0 && (
                    <p style={{ fontSize: 13, color: '#9ca3af', marginBottom: 4 }}>
                        {filtrados.length} serviço{filtrados.length !== 1 ? 's' : ''} encontrado{filtrados.length !== 1 ? 's' : ''}
                    </p>
                )}

                {/* Lista */}
                {this.state.aguarde ? (
                    <div style={{ textAlign: 'center', padding: '40px 0' }}>
                        <LoadingIndicator timeWait={500} />
                    </div>
                ) : filtrados.length === 0 ? (
                    <div style={{ textAlign: 'center', padding: '48px 0', color: '#9ca3af', fontSize: 14 }}>
                        Nenhum serviço encontrado.
                    </div>
                ) : (
                    <div className="sg">
                        {filtrados.map(item => {
                            const imgObj = item.imagem?.find(i => i.principal === true && i.nome === "CAPA");
                            const imagemSrc = imgObj ? `data:image/jpeg;base64,${imgObj.imagem}` : null;
                            const tempo = formatarTempo(item.tempoExecucaoMinutos);

                            return (
                                <div className="sc" key={item.id}>

                                    {/* Imagem */}
                                    <div className="si">
                                        {imagemSrc ? (
                                            <img
                                                src={imagemSrc}
                                                alt={item.nome}
                                                onError={e => {
                                                    e.currentTarget.style.display = 'none';
                                                    e.currentTarget.nextSibling.style.display = 'flex';
                                                }}
                                            />
                                        ) : null}
                                        <div className="si-ph" style={{ display: imagemSrc ? 'none' : 'flex' }}>
                                            <div className="si-ph-icon"><IconImg /></div>
                                            <span className="si-ph-txt">sem imagem</span>
                                        </div>
                                        {tempo && <span className="si-badge">{tempo}</span>}
                                    </div>

                                    {/* Corpo */}
                                    <div className="sb">
                                        <p className="sn">{item.nome}</p>

                                        {item.descricao && (
                                            <p className="sd">{item.descricao}</p>
                                        )}

                                        <div className="sm">
                                            <IconUser />
                                            <span style={{ overflow: 'hidden', whiteSpace: 'nowrap', textOverflow: 'ellipsis' }}>
                                                {item.dados?.nome}
                                            </span>
                                        </div>

                                        {item.endereco?.cidade && (
                                            <div className="sm">
                                                <IconPin />
                                                {item.endereco.cidade} - {item.endereco.uf}
                                            </div>
                                        )}

                                        {/* Rodapé */}
                                        <div className="sf">
                                            <div>
                                                <span className="sp">
                                                    {Number(item.preco).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })}
                                                </span>
                                            </div>
                                            <button className="sa" onClick={() => this.cadastroModalAbrir(item)}>
                                                Agendar
                                            </button>
                                        </div>
                                    </div>

                                </div>
                            );
                        })}
                    </div>
                )}

                {/* Modal */}
                {this.state.cadastroModal && (
                    <Agendamento
                        onFechar={this.cadastroModalFechar}
                        idEdicao={this.state.servicoIdSelecionado}
                    />
                )}
            </div>
        );
    }
}

createRoot(document.getElementById('root')).render(
    <React.StrictMode><Index /></React.StrictMode>
);