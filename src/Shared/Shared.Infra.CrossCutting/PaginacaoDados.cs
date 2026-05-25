using System;
using System.ComponentModel;

namespace Shared.Infra.CrossCutting
{
    public class PaginacaoDados
    {
        public enum TpOrdenacao
        {
            [Description("Indefinido")]
            Indefinido = 0,
            [Description("Nome (A - Z)")]
            Nome = 1,
        }

        /// Total de itens da consulta.
        public int TotalItens { get; set; }
        /// Quantidade de itens por página de dados.
        public int TamanhoPagina { get; set; }
        /// Número da página atual (iniciando em zero).
        public int PaginaAtual { get; set; }
        /// Tipo de ordenação de dados aplicada à consulta.
        public TpOrdenacao Ordenacao { get; set; }
        /// Nome da ordenação de dados aplicada à consulta.
        public string OrdenacaoNome { get; set; }
        /// Total de páginas de dados da consulta.
        public decimal TotalPaginas
        {
            get
            {
                return Math.Ceiling(TotalItens / (decimal)TamanhoPagina);
            }
        }

        public PaginacaoDados(int paginaAtual, int tamanhoPagina, TpOrdenacao ordenacao)
        {
            PaginaAtual = paginaAtual;
            TamanhoPagina = tamanhoPagina;
            Ordenacao = ordenacao;
            OrdenacaoNome = Utils.ObterDescricaoEnum(ordenacao);
        }

        public PaginacaoDados(int paginaAtual, int tamanhoPagina)
        {
            PaginaAtual = paginaAtual;
            TamanhoPagina = tamanhoPagina;
            Ordenacao = TpOrdenacao.Indefinido;
            OrdenacaoNome = Utils.ObterDescricaoEnum(TpOrdenacao.Indefinido);
        }

        public PaginacaoDados()
        {

        }

    }
}
