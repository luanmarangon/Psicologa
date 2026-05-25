using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Psicologa.Application.Convenio.Services;
using Psicologa.Application.Convenio.ViewsModel;
using Psicologa.Application.Servico.ViewsModel;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Domain.Servico.Services;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.GerenciarConvenios)]
    [Area("Administrativo")]
    public class ConvenioController: BaseController
    {
        private readonly UsuarioAutenticado _ua;
        private readonly RequisicaoAtual _req;

        private readonly ApplicationConvenioService _convenioService;

        public ConvenioController(UsuarioAutenticado ua, RequisicaoAtual req, ApplicationConvenioService convenioService)
        {
            _ua = ua;
            _req = req;
            _convenioService = convenioService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int filtro, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<object> convenios = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12


            var tp = (Domain.Servico.Entities.Servico.TpFiltroServico)Convert.ToInt32(filtro);
            convenios = _convenioService.Consultar(q, paginacao);
            var retorno = new
            {
                convenios,
                paginacao
            };
            return DefaultJSONResponse(true, retorno);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var convenio = _convenioService.Obter(idLimpo);
            return DefaultJSONResponse(true, convenio);
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ConvenioViewModel convenioVM = null;
            try
            {
                convenioVM = dados.Deserialize<ConvenioViewModel>();
                (operacao, vr) = _convenioService.Salvar(convenioVM, requisicao);

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
                retorno = _convenioService.Obter(convenioVM.Id);
                AddUserMessageSuccess("Convênio salvo com sucesso");
            }
            return DefaultJSONResponse(operacao, retorno);
        }

        [HttpDelete]
        public IActionResult Excluir(string id)
        {
            var requisicao = _req.ToArray(_ua);
            bool operacao = false;

            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            operacao = _convenioService.Excluir(idLimpo, requisicao);
            if (operacao)
            {
                AddUserMessageSuccess("Convênio excluído com sucesso.");
            }
            else
            {
                AddUserMessageError("Erro ao excluir convênio!");
            }
            return DefaultJSONResponse(operacao, null);
        }




    }
}
