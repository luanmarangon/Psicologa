using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Agendamento.ViewsModel;
using Psicologa.Application.Financeiro.Services;
using Psicologa.Application.Financeiro.ViewsModel;
using Psicologa.Domain.Agendamento.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarFinanceiro)]
    [Area("Administrativo")]
    public class FinanceiroController : BaseController
    {
        UsuarioAutenticado _ua;
        RequisicaoAtual _req;
        ApplicationFinanceiroService _applicationFinanceiro;
        public FinanceiroController(UsuarioAutenticado ua, RequisicaoAtual req, ApplicationFinanceiroService applicationFinanceiro)
        {
            _ua = ua;
            _req = req;
            _applicationFinanceiro = applicationFinanceiro;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int filtro, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<FinanceiroViewModel> agendamentos = new List<FinanceiroViewModel>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12

            var filtroFinal = (Domain.Agendamento.Entities.Agendamento.tpFiltro)filtro;
            //agendamentos = _applicationFinanceiro.Consultar(q, filtroFinal, paginacao);
            var retorno = new
            {
                agendamentos,
                paginacao
            };
            return DefaultJSONResponse(true, retorno);
        }



    }
}
