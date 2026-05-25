import React from "react";

function CardServico(props) {

    const portalEducativoSubcategoriaAbrir = (chaveUrl) => {
        if (!isEmpty(chaveUrl)) {
            goTo(`/a/${chaveUrl}#modulos`);
        }
    }

    const formatarTempoExecucao = (tempoExecucaoMinutos) => {
        if (!tempoExecucaoMinutos) return null;
        if (tempoExecucaoMinutos < 60) {
            return `${tempoExecucaoMinutos} min`;
        }
        const horas = Math.floor(tempoExecucaoMinutos / 60);
        const minutos = tempoExecucaoMinutos % 60;
        return minutos > 0 ? `${horas}h ${minutos}min` : `${horas}h`;
    }

    const formatarPreco = (preco) => {
        const valor = Number(preco);
        if (isNaN(valor)) return null;
        return new Intl.NumberFormat('pt-BR', {
            style: 'currency',
            currency: 'BRL'
        }).format(valor);
    }

    const tempo = formatarTempoExecucao(props.tempoExecucaoMinutos);
    const precoFormatado = formatarPreco(props.preco);

    const gerarCorFundo = (texto) => {
        const cores = ['#4f46e5','#0891b2','#059669','#d97706','#dc2626','#7c3aed','#db2777','#0284c7'];
        let hash = 0;
        for (let i = 0; i < texto.length; i++) hash = texto.charCodeAt(i) + ((hash << 5) - hash);
        return cores[Math.abs(hash) % cores.length];
    }

    const gerarIniciais = (nome) => {
        if (!nome) return '?';
        const partes = nome.trim().split(' ').filter(Boolean);
        if (partes.length === 1) return partes[0].charAt(0).toUpperCase();
        return (partes[0].charAt(0) + partes[partes.length - 1].charAt(0)).toUpperCase();
    }

    const corFundo = gerarCorFundo(props.nome || '');
    const iniciais = gerarIniciais(props.nome);

    return (
        <div
            className="col mb-3 px-2"
            style={{ cursor: "pointer" }}
            onClick={() => portalEducativoSubcategoriaAbrir(props.chaveUrl)}
        >
            <div style={{
                borderRadius: 10,
                overflow: "hidden",
                border: "1px solid #e5e7eb",
                backgroundColor: "#fff",
                transition: "box-shadow 0.2s ease, transform 0.2s ease",
                height: "100%",
                display: "flex",
                flexDirection: "column",
            }}
                onMouseEnter={e => {
                    e.currentTarget.style.boxShadow = "0 8px 24px rgba(0,0,0,0.10)";
                    e.currentTarget.style.transform = "translateY(-3px)";
                }}
                onMouseLeave={e => {
                    e.currentTarget.style.boxShadow = "none";
                    e.currentTarget.style.transform = "translateY(0)";
                }}
            >
                {/* Imagem */}
                <div style={{ position: "relative", overflow: "hidden", height: 160 }}>
                    {props.capa ? (
                        <img
                            src={props.capa}
                            alt={props.nome}
                            style={{
                                width: "100%",
                                height: "100%",
                                objectFit: "cover",
                                display: "block",
                            }}
                            onError={e => { e.currentTarget.style.display = 'none'; e.currentTarget.nextSibling.style.display = 'flex'; }}
                        />
                    ) : null}
                    <div style={{
                        display: props.capa ? "none" : "flex",
                        width: "100%",
                        height: "100%",
                        backgroundColor: corFundo,
                        alignItems: "center",
                        justifyContent: "center",
                        flexDirection: "column",
                        gap: 6,
                    }}>
                        <span style={{ fontSize: 36, fontWeight: 700, color: "rgba(255,255,255,0.95)", letterSpacing: 2 }}>
                            {iniciais}
                        </span>
                        <span style={{ fontSize: 11, color: "rgba(255,255,255,0.65)", fontWeight: 500, textTransform: "uppercase", letterSpacing: "0.1em", maxWidth: "80%", textAlign: "center", overflow: "hidden", whiteSpace: "nowrap", textOverflow: "ellipsis" }}>
                            {props.nome}
                        </span>
                    </div>
                    {/* Badge tempo */}
                    {tempo && (
                        <span style={{
                            position: "absolute",
                            bottom: 8,
                            right: 8,
                            backgroundColor: "rgba(0,0,0,0.65)",
                            color: "#fff",
                            fontSize: 11,
                            fontWeight: 500,
                            padding: "3px 8px",
                            borderRadius: 20,
                            backdropFilter: "blur(4px)",
                        }}>
                            {tempo}
                        </span>
                    )}
                </div>

                {/* Corpo */}
                <div style={{
                    padding: "14px 16px 16px",
                    display: "flex",
                    flexDirection: "column",
                    flex: 1,
                    gap: 6,
                }}>
                    <h6 style={{
                        margin: 0,
                        fontSize: "0.92rem",
                        fontWeight: 600,
                        color: "#111827",
                        lineHeight: 1.4,
                        display: "-webkit-box",
                        WebkitLineClamp: 2,
                        WebkitBoxOrient: "vertical",
                        overflow: "hidden",
                    }}>
                        {props.nome}
                    </h6>

                    {props.descricao && (
                        <p style={{
                            margin: 0,
                            fontSize: "0.8rem",
                            color: "#6b7280",
                            lineHeight: 1.5,
                            display: "-webkit-box",
                            WebkitLineClamp: 2,
                            WebkitBoxOrient: "vertical",
                            overflow: "hidden",
                            flex: 1,
                        }}>
                            {props.descricao}
                        </p>
                    )}

                    {/* Rodapé: preço */}
                    {precoFormatado && (
                        <div style={{
                            marginTop: 8,
                            paddingTop: 10,
                            borderTop: "1px solid #f3f4f6",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                        }}>
                            <span style={{
                                fontSize: "1rem",
                                fontWeight: 700,
                                color: "#2563eb",
                            }}>
                                {precoFormatado}
                            </span>
                            <span style={{
                                fontSize: 11,
                                color: "#9ca3af",
                                fontWeight: 400,
                            }}>
                                Ver detalhes →
                            </span>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}

export default CardServico;