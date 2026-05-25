import React from 'react';

const Paginacao = ({ paginacao, onPageChange }) => {
    const { totalPaginas, paginaAtual } = paginacao;

    if (totalPaginas <= 1) return null;

    let paginas = [];
    let firstDisabled = paginaAtual === 0 ? "disabled" : "";
    let lastDisabled = paginaAtual === totalPaginas - 1 ? "disabled" : "";
    let ant = paginaAtual - 1;
    let prox = paginaAtual + 1;

    // Definindo os limites da paginação
    let inicio = 0;
    let fim = totalPaginas;

    if (totalPaginas > 3) {
        inicio = paginaAtual - 2;
        if (inicio < 0) inicio = 0;
        fim = inicio + 3;
        if (fim > totalPaginas) fim = totalPaginas;
    }

    // Criando os botões de número de página
    for (let i = inicio; i < fim; i++) {
        let active = paginaAtual === i ? "active" : "";
        paginas.push(
            <li key={`page-${i}`} className={`page-item ${active}`}>
                <button className="page-link" onClick={() => onPageChange(i)}>
                    {i + 1}
                </button>
            </li>
        );
    }

    // Componente de saída
    return (
        // <div className="card-footer">
        <div className="mt-4">
            <nav>
                <ul className="pagination justify-content-end">
                    {/* Primeiro botão (voltar para primeira página) */}
                    <li className={`page-item ${firstDisabled}`}>
                        <button
                            className="page-link"
                            onClick={() => onPageChange(0)}
                            disabled={ant < 0}
                        >
                            <i className="fas fa-step-backward"></i>
                        </button>
                    </li>

                    {/* Botão anterior */}
                    <li className={`page-item ${firstDisabled}`}>
                        <button
                            className="page-link"
                            onClick={() => onPageChange(ant)}
                            disabled={ant < 0}
                        >
                            <i className="fas fa-caret-left"></i>
                        </button>
                    </li>

                    {/* Páginas numeradas */}
                    {paginas}

                    {/* Botão próximo */}
                    <li className={`page-item ${lastDisabled}`}>
                        <button
                            className="page-link"
                            onClick={() => onPageChange(prox)}
                            disabled={prox >= totalPaginas}
                        >
                            <i className="fas fa-caret-right"></i>
                        </button>
                    </li>

                    {/* Último botão (ir para última página) */}
                    <li className={`page-item ${lastDisabled}`}>
                        <button
                            className="page-link"
                            onClick={() => onPageChange(totalPaginas - 1)}
                            disabled={prox >= totalPaginas}
                        >
                            <i className="fas fa-step-forward"></i>
                        </button>
                    </li>
                </ul>
            </nav>
        </div>
    );
};

export default Paginacao;
