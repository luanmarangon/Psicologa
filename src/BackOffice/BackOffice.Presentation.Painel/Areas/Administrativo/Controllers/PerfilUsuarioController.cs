using Psicologa.Application.Usuario.Services;
using Psicologa.Application.Usuario.ViewsModel;
using Microsoft.AspNetCore.Mvc;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using Psicologa.Presentation.Painel;
using Psicologa.Presentation.Painel.Controllers;
using Shared.Infra.CrossCutting.JSONConverter;

namespace Psicologa.Areas.Administrativo.Presentation.Painel.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.GerenciarPermissoes)]
    [Area("Administrativo")]
    public class PerfilUsuarioController : BaseController
    {
        private readonly ApplicationPerfilUsuarioService _puService;

        public PerfilUsuarioController(ApplicationPerfilUsuarioService puService)
        {
            _puService = puService;
        }

        [HttpPost]
        public IActionResult Salvar([FromBody] System.Text.Json.JsonElement dados)
        {
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            PerfilUsuarioViewModel puVM = null;
            try
            {
                puVM = new PerfilUsuarioViewModel();
                puVM.Id = Convert.ToInt32(Criptografia.Descriptografar(dados.GetProperty("id").ToString()));
                puVM.Nome = dados.GetProperty("nome").ToString();
                var permissoes = dados.GetProperty("permissoes");

                puVM.Permissoes = new List<PerfilUsuarioViewModel.TpPermissao>();
                foreach (var p in permissoes.EnumerateArray())
                {
                    int id = Convert.ToInt32(p.ToString());
                    puVM.Permissoes.Add((PerfilUsuarioViewModel.TpPermissao)id);
                }

                //puVM = dados.Deserialize<PerfilUsuarioViewModel>();
                
                (operacao, vr) = _puService.Salvar(puVM);

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
                ucVM = _puService.Obter(puVM.Id);
                AddUserMessageSuccess("Permissão salva com sucesso.");
            }

            return DefaultJSONResponse(operacao, ucVM);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Obter(string id)
        {
            int idLimpo = Convert.ToInt32(Criptografia.Descriptografar(id));
            PerfilUsuarioViewModel uVM = _puService.Obter(idLimpo);
            return DefaultJSONResponse(uVM != null, uVM);
        }

        [HttpGet]
        public IActionResult ObterPerfis()
        {
            return DefaultJSONResponse(true, _puService.ObterTodos());
        }

        [HttpGet]
        public IActionResult ObterPermissoes()
        {
            List<object> retorno = new List<object>();

            foreach (var item in _puService.ObterPermissoes())
            {
                retorno.Add(new
                {
                    id = (int)item,
                    nome = Utils.ObterDescricaoEnum(item)
                });
            }

            return DefaultJSONResponse(true, retorno);
        }

        [HttpPost]
        public IActionResult SalvarPerfil([FromBody] System.Text.Json.JsonElement dados)
        {
            bool operacao = false;
            ValidationResult vr = new ValidationResult();

            PerfilUsuarioCadastroViewModel puVM = null;
            try
            {
                puVM = dados.Deserialize<PerfilUsuarioCadastroViewModel>();
             
                (operacao, vr) = _puService.SalvarPerfil(puVM);

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
                ucVM = _puService.Obter(puVM.Id);
                AddUserMessageSuccess("Permissão salva com sucesso.");
            }

            return DefaultJSONResponse(operacao, ucVM);
        }
    }
}
