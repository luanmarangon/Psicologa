using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Psicologa.Application;
using Psicologa.Application.BlogPost.Services;
using Psicologa.Application.BlogPost.ViewsModel;
using Psicologa.Application.Servico.Services;
using Psicologa.Domain.BlogPost.Entities;
using Shared.Infra.CrossCutting;

namespace Psicologa.Presentation.Painel.Controllers
{
    [AllowAnonymous]
    public class ServicoController : BaseController
    {
        private readonly ApplicationServicoService _servicoService;
        private readonly IAppSettings _appSettings;

        public ServicoController(ApplicationServicoService servicoService, IAppSettings appSettings)
        {
            _servicoService = servicoService;
            _appSettings = appSettings;
        }

        public IActionResult Index()
        {
            //no Front end o arquivo vai ser a Servico.js.
            return View();
        }

        [HttpGet]
        public IActionResult ObterDestaquesHome()
        {
            return DefaultJSONResponse(true, _servicoService.ObterDestaquesHome(3));
        }

        [HttpGet]
        public IActionResult ObterTodos()
        {
            IEnumerable<object> servicos = new List<object>();
            servicos = _servicoService.ObterTodos();
            return DefaultJSONResponse(true, servicos);
        }

    }
}