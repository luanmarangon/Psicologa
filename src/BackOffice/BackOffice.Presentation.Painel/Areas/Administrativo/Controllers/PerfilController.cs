using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Pessoa.Services;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Domain.BlogPost.Entities;
using Psicologa.Presentation.Painel.Controllers;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.Backoffice)]
    [Area("Administrativo")]
    public class PerfilController : BaseController
    {
        private ApplicationPessoaService _pessoaService;
        private readonly UsuarioAutenticado _ua;

        public PerfilController(UsuarioAutenticado ua, ApplicationPessoaService pessoaService)
        {
            _ua = ua;
            _pessoaService = pessoaService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ObterPerfil()
        {
            var pessoa = _pessoaService.Obter(_ua.PessoaId);
            return DefaultJSONResponse(pessoa != null, pessoa);
        }
    
    
    
    
    
    
    
    }
}