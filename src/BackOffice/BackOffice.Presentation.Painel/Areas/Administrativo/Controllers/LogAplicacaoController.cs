using Microsoft.AspNetCore.Mvc;
using Oci.Common;
using Psicologa.Application.LogAplicacao.Services;
using Psicologa.Application.LogAplicacao.ViewsModel;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarLogsAplicacao)]
    [Area("Administrativo")]
    public class LogAplicacaoController : BaseController
    {
        private UsuarioAutenticado _ua;
        private RequisicaoAtual _req;
        private ApplicationLogAplicacaoService _logs;

        public LogAplicacaoController(UsuarioAutenticado ua, RequisicaoAtual req, ApplicationLogAplicacaoService logs)
        {
            _ua = ua;
            _req = req;
            _logs = logs;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<LogAplicacaoViewModel> logs = new List<LogAplicacaoViewModel>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 10, (PaginacaoDados.TpOrdenacao)ordenacao);

            //if (!string.IsNullOrEmpty(q))
            //{
            //    if (q.ToLower() == "ultimos")
            //    {
            //        ucs = _logs.ObterUltimos(30);
            //    }
            //    else
            //    {
            logs = _logs.Consultar(q, paginacao);
            //}
            //}

            var retorno = new
            {
                logs,
                paginacao
            };

            return DefaultJSONResponse(true, retorno);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(id);
            var log = _logs.Obter(idLimpo);
            return DefaultJSONResponse(true, log);
        }
    }
}