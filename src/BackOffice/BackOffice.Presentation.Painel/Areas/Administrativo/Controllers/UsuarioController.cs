using Microsoft.AspNetCore.Mvc;
using Psicologa.Application;
using Psicologa.Application.Usuario.Services;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Presentation.Painel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.JSONConverter;
using Shared.Infra.CrossCutting.ValidationResult;

namespace Psicologa.Areas.Administrativo.Presentation.Painel.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.GerenciarUsuarios)]
    [Area("Administrativo")]
    public class UsuarioController : BaseController
    {
        ApplicationUsuarioService _uService;
        ApplicationPerfilUsuarioService _puService;
        private readonly IAppSettings _appSettings;

        public UsuarioController(ApplicationUsuarioService uService, ApplicationPerfilUsuarioService puService, IAppSettings appSettings)
        {
            _uService = uService;
            _puService = puService;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            UsuarioViewModel uVM = null;
            try
            {
                uVM = dados.Deserialize<UsuarioViewModel>();

                (operacao, vr) = _uService.Salvar(uVM);

                if (!operacao)
                {
                    AddUserMessage(vr);
                }

            }
            catch (Exception ex)
            {
                AddUserMessageError("Um erro ocorreu. Tente novamente");
            }

            object ucVM = null;
            if (operacao)
            {
                ucVM = _uService.Obter(uVM.Id);
                AddUserMessageSuccess("Usuário salvo com sucesso.");
            }

            return DefaultJSONResponse(operacao, ucVM);
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            UsuarioConsultaViewModel uVM = _uService.Obter(idLimpo);
            return DefaultJSONResponse(uVM != null, uVM);
        }

        [HttpDelete]
        public IActionResult Excluir(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            bool operacao;

            operacao = _uService.Excluir(idLimpo);

            if (operacao)
            {
                AddUserMessageSuccess("Usuário excluído com sucesso.");
            }
            else
            {
                AddUserMessageError("Erro ao excluir usuário");
            }

            return DefaultJSONResponse(operacao);
        }

        [HttpGet]
        public IActionResult Pesquisar(string q)
        {
            IEnumerable<UsuarioConsultaViewModel> ucs = new List<UsuarioConsultaViewModel>();

            if (!string.IsNullOrEmpty(q))
            {
                if (q.ToLower() == "ultimos")
                {
                    ucs = _uService.ObterUltimos(30);
                }
                else
                {
                    ucs = _uService.Consultar(q);
                }
            }

            return DefaultJSONResponse(true, ucs);
        }

        [HttpGet]
        public IActionResult ObterPerfis()
        {
            return DefaultJSONResponse(true, _puService.ObterTodos());
        }
    }
}
