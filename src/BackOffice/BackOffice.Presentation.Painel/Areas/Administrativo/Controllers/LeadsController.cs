using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Servico.ViewsModel;
using Psicologa.Application.ServicoContato.Services;
using Psicologa.Application.ServicoContato.ViewsModel;
using Psicologa.Domain.Convenio.Services;
using Psicologa.Domain.Servico.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarLeads)]
    [Area("Administrativo")]
    public class LeadsController : BaseController
    {
        UsuarioAutenticado _ua;
        RequisicaoAtual _req;
        private readonly ApplicationServicoContatoService _leadsService;

        public LeadsController(UsuarioAutenticado ua, RequisicaoAtual req, ApplicationServicoContatoService leadsService)
        {
            _ua = ua;
            _req = req;
            _leadsService = leadsService;
        }

        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        public IActionResult Pesquisar(string q, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<object> leads = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12

            leads = _leadsService.Consultar(q, paginacao);
            var retorno = new
            {
                leads,
                paginacao
            };
            return DefaultJSONResponse(true, retorno);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var servico = _leadsService.Obter(idLimpo);
            return DefaultJSONResponse(true, servico);
        }



        [HttpGet]
        public IActionResult ObterParametros()
        {
            var retorno = _leadsService.ObterParametros();

            return DefaultJSONResponse(true, retorno);
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ServicoContatoViewModel servicoVM = null;
            try
            {
                servicoVM = dados.Deserialize<ServicoContatoViewModel>();
                (operacao, vr) = _leadsService.Salvar(servicoVM, requisicao);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }
            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }
            object retorno = null;
            if (operacao)
            {
                retorno = _leadsService.Obter(servicoVM.Id);
                AddUserMessageSuccess("Serviço salvo com sucesso");
            }
            return DefaultJSONResponse(operacao, retorno);
        }

    }
}