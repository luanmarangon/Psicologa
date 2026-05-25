using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Servico.Services;
using Psicologa.Application.Servico.ViewsModel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarServicos)]
    [Area("Administrativo")]
    public class ServicoController : BaseController
    {
        private ApplicationServicoService _servicoService;
        private UsuarioAutenticado _ua;
        private RequisicaoAtual _req;

        public ServicoController(ApplicationServicoService servicoService, UsuarioAutenticado ua, RequisicaoAtual req)
        {
            _servicoService = servicoService;
            _ua = ua;
            _req = req;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pesquisar(string q, int filtro, int pagina = 0, int ordenacao = 1)
        {
            IEnumerable<object> servicos = new List<object>();
            PaginacaoDados paginacao = new PaginacaoDados(pagina, 12, (PaginacaoDados.TpOrdenacao)ordenacao); //12
            
            
            var tp = (Domain.Servico.Entities.Servico.TpFiltroServico)Convert.ToInt32(filtro);
            servicos = _servicoService.Consultar(q, tp, paginacao);
            var retorno = new
            {
                servicos,
                paginacao
            };
            return DefaultJSONResponse(true, retorno);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            var servico = _servicoService.Obter(idLimpo);
            return DefaultJSONResponse(true, servico);
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            var requisicao = _req.ToArray(_ua);
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            ServicoViewModel servicoVM = null;
            try
            {
                servicoVM = dados.Deserialize<ServicoViewModel>();
                (operacao, vr) = _servicoService.Salvar(servicoVM, requisicao);

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
                retorno = _servicoService.Obter(servicoVM.Id);
                AddUserMessageSuccess("Serviço salvo com sucesso");
            }
            return DefaultJSONResponse(operacao, retorno);
        }

        [HttpDelete]
        public IActionResult Excluir(string id)
        {
            var requisicao = _req.ToArray(_ua);
            bool operacao = false;

            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            operacao = _servicoService.Excluir(idLimpo, requisicao);
            if (operacao)
            {
                AddUserMessageSuccess("Serviço excluído com sucesso.");
            }
            else
            {
                AddUserMessageError("Erro ao excluir serviço!");
            }
            return DefaultJSONResponse(operacao, null);
        }

        [HttpGet]
        public IActionResult ObterTodos()
        {
            return DefaultJSONResponse(true, _servicoService.ObterTodos());
        }
        [HttpGet]
        public IActionResult ObterTodosAtivos()
        {
            return DefaultJSONResponse(true, _servicoService.ObterTodosAtivos());
        }

        //[HttpGet]
        //public IActionResult ObterDestaquesHome()
        //{
        //    return DefaultJSONResponse(true, _servicoService.ObterDestaquesHome(3));
        //}
    }
}