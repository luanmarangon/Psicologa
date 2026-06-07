using Psicologa.Application.LogAplicacao.ViewsModel;
using Psicologa.Application.ProntuarioSessao.ViewsModel;
using Psicologa.Domain.ProntuarioSessao.Entities;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Application.LogAplicacao.Services
{
    public class ApplicationLogAplicacaoService : IDisposable
    {
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _service;
        private readonly IAppSettings _appSettings;

        public ApplicationLogAplicacaoService(Domain.LogAplicacao.Services.LogAplicacaoService service, IAppSettings appSettings)
        {
            _service = service;
            _appSettings = appSettings;
        }

        public LogAplicacaoViewModel Obter(int id)
        {
            return FormatarRetornoConsulta(_service.Obter(id));
        }

        public IEnumerable<ViewsModel.LogAplicacaoViewModel> ObterUltimos(int top)
       {
            var logs = _service.ObterUltimos(top);
            List<LogAplicacaoViewModel> ret = new List<LogAplicacaoViewModel>();
            foreach (var log in logs)
            {
                ret.Add(FormatarRetornoConsulta(log));
            }

            return ret;
        }

        public IEnumerable<LogAplicacaoViewModel> Consultar(string termo, PaginacaoDados paginacao)
        {
            List<LogAplicacaoViewModel> retorno = new List<LogAplicacaoViewModel>();
            var logs = _service.Consultar(termo, paginacao);
            foreach (var log in logs)
            {
                retorno.Add(FormatarRetornoConsulta(log));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == PaginacaoDados.TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.DataCriacao).ToList();
            }

            return retorno;

        }

        internal LogAplicacaoViewModel FormatarRetornoConsulta(Domain.LogAplicacao.Entities.LogAplicacao log)
        {
            if (log == null)
                return null;

            LogAplicacaoViewModel ret = new LogAplicacaoViewModel();

            ret.Id = log.Id;
            ret.DataCriacao = log.DataCriacao;
            ret.UsuarioId = log.UsuarioId;
            ret.UsuarioNome = log.UsuarioNome;
            ret.Dispositivo = log.Dispositivo;
            ret.IP = log.IP;
            ret.UserAgent = log.UserAgent;
            ret.Entidade = log.Entidade;
            ret.EntidadeId = log.EntidadeId;
            ret.Operacao = log.Operacao;
            ret.Aplicacao = log.Aplicacao;
            ret.Metodo = log.Metodo;
            ret.DadosAntes = log.DadosAntes;
            ret.DadosDepois = log.DadosDepois;
            ret.DadosAlterados = log.DadosAlterados;

            return ret;
        }

        public void Dispose()
        {
        }
    }
}