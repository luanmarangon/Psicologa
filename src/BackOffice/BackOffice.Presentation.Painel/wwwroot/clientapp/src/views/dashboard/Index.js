import React, { Component } from 'react';
import { createRoot } from 'react-dom/client';
import LoadingIndicator from '../../components/LoadingIndicator';

export default class Index extends Component {

    constructor(props) {

        super(props);

        this.state = {
            aguarde: false,
            indicadores: {},
        };

    }

    componentDidMount = () => {

        this.obter();

    }

    obter = () => {

        this.setState({
            aguarde: true
        });

        HTTPClient.get("Administrativo/Dashboard/ObterIndicador", false)
            .then(r => r.json())
            .then(r => {

                if (!r.success) {

                    showToastr(r.messages);
                    return;

                }

                this.setState({
                    indicadores: r.data
                });

                console.log("Dashboard data:", r.data);

            })
            .catch(() => {

                showToastr({
                    type: "error",
                    text: "Um erro ocorreu ao obter o Dashboard."
                });

            })
            .finally(() => {

                this.setState({
                    aguarde: false
                });

            });

    }

    formatNumber = (num) => {

        if (!num && num !== 0)
            return '—';

        if (num >= 1000)
            return (num / 1000).toFixed(1) + 'K';

        return num.toString();

    }

    obterCards = () => {

        const { indicadores } = this.state;

        // procura automaticamente qual perfil possui array
        const chavePerfil = Object.keys(indicadores)
            .find(k => Array.isArray(indicadores[k]));

        if (!chavePerfil)
            return [];

        return indicadores[chavePerfil];

    }

    renderCard = (config, index) => {

        const {
            label,
            gradient,
            value,
            icon,
            link
        } = config;

        const displayValue =
            typeof value === 'number'
                ? this.formatNumber(value)
                : (value || '—');

        const card = (

            <div
                className="professional-card"
                style={{ background: gradient }}
            >

                <div className="card-pattern" />

                <div className="icon-container">
                    <i className={icon} />
                </div>

                <div className="value-section">
                    <div className="main-value">
                        {displayValue}
                    </div>
                </div>

                <div className="card-label">
                    {label}
                </div>

                <div className="progress-line" />

            </div>

        );

        return link
            ? (
                <a
                    href={link}
                    className="card-link"
                    key={index}
                >
                    {card}
                </a>
            )
            : (
                <div key={index}>
                    {card}
                </div>
            );

    }

    render() {

        const { aguarde, indicadores } = this.state;

        const config = this.obterCards();

        const {
            titulo,
            subTitulo
        } = indicadores.saudacao || {
            titulo: 'Dashboard',
            subTitulo: ''
        };

        return (

            <div>

                <div className="dashboard-wrapper">

                    <div className="element-content">

                        {/* Cabeçalho */}
                        {!aguarde && (

                            <div style={{ marginBottom: 24 }}>

                                <h4
                                    style={{
                                        margin: 0,
                                        fontWeight: 700,
                                        color: '#1e293b'
                                    }}
                                >
                                    {titulo}
                                </h4>

                                {subTitulo && (

                                    <p
                                        style={{
                                            margin: '4px 0 0',
                                            color: '#64748b',
                                            fontSize: 14
                                        }}
                                    >
                                        {subTitulo}
                                    </p>

                                )}

                            </div>

                        )}

                        {aguarde ? (

                            <div className="mt-5 mb-5">
                                <LoadingIndicator />
                            </div>

                        ) : (

                            <div className="professional-grid">

                                {config.map((c, i) =>
                                    this.renderCard(c, i)
                                )}

                            </div>

                        )}

                    </div>

                </div>

                <style jsx>{`

                    .dashboard-wrapper {
                        width: 100%;
                    }

                    .professional-grid {
                        display: grid;
                        grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
                        gap: 1.5rem;
                        padding: 1.5rem 0;
                    }

                    .card-link {
                        text-decoration: none;
                        color: inherit;
                    }

                    .professional-card {
                        position: relative;
                        border-radius: 12px;
                        padding: 1.75rem;
                        height: 160px;
                        overflow: hidden;
                        transition: all 0.3s ease;
                        box-shadow:
                            0 4px 20px rgba(0,0,0,0.08),
                            0 1px 3px rgba(0,0,0,0.05);
                        border: 1px solid rgba(255,255,255,0.8);
                        display: flex;
                        flex-direction: column;
                        cursor: default;
                    }

                    .professional-card:hover {
                        transform: translateY(-3px);
                        box-shadow:
                            0 8px 30px rgba(0,0,0,0.12);
                    }

                    .card-pattern {
                        position: absolute;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        opacity: 0.05;

                        background-image:
                            radial-gradient(circle at 25% 25%, white 2px, transparent 2px),
                            radial-gradient(circle at 75% 75%, white 1px, transparent 1px);

                        background-size:
                            40px 40px,
                            20px 20px;
                    }

                    .icon-container {
                        width: 44px;
                        height: 44px;
                        border-radius: 10px;
                        background: rgba(255,255,255,0.15);
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        margin-bottom: 1rem;
                    }

                    .icon-container i {
                        font-size: 1.3rem;
                        color: white;
                    }

                    .value-section {
                        flex-grow: 1;
                        display: flex;
                        align-items: center;
                        margin-bottom: 0.5rem;
                    }

                    .main-value {
                        font-size: 2.4rem;
                        font-weight: 700;
                        color: white;
                        line-height: 1;
                        text-shadow: 0 1px 3px rgba(0,0,0,0.2);
                    }

                    .card-label {
                        color: rgba(255,255,255,0.95);
                        font-size: 0.88rem;
                        font-weight: 500;
                        line-height: 1.2;
                        margin-bottom: 0.5rem;
                    }

                    .progress-line {
                        position: absolute;
                        bottom: 0;
                        left: 0;
                        right: 0;
                        height: 3px;
                        background: rgba(255,255,255,0.15);
                    }

                    .progress-line::after {
                        content: '';
                        position: absolute;
                        top: 0;
                        left: 0;
                        bottom: 0;
                        width: 70%;
                        background: rgba(255,255,255,0.35);
                    }

                    @media (max-width: 768px) {

                        .professional-grid {
                            grid-template-columns: 1fr;
                            gap: 1rem;
                        }

                        .professional-card {
                            height: 140px;
                            padding: 1.25rem;
                        }

                        .main-value {
                            font-size: 2rem;
                        }

                    }

                    @media (min-width: 769px) and (max-width: 1024px) {

                        .professional-grid {
                            grid-template-columns: repeat(2, 1fr);
                        }

                    }

                    @media (min-width: 1025px) and (max-width: 1400px) {

                        .professional-grid {
                            grid-template-columns: repeat(3, 1fr);
                        }

                    }

                    @media (min-width: 1401px) {

                        .professional-grid {
                            grid-template-columns: repeat(4, 1fr);
                        }

                    }

                `}</style>

            </div>

        );

    }

}

createRoot(document.getElementById('root')).render(
    <React.StrictMode>
        <Index />
    </React.StrictMode>
);