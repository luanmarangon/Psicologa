using Microsoft.AspNetCore.Mvc;
using Psicologa.Presentation.Painel.Controllers;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    [Permissao(Tipo = Application.Usuario.ViewsModel.PerfilUsuarioViewModel.TpPermissao.GerenciarDocumentos)]
    [Area("Administrativo")]
    public class DocumentosController : BaseController
    {
        private UsuarioAutenticado _ua;
        private RequisicaoAtual _req;

        public DocumentosController(UsuarioAutenticado ua, RequisicaoAtual req)
        {
            _ua = ua;
            _req = req;
        }

        public ActionResult Index()
        {
            return View();
        }
    
    
    
    
    
    
    
    
    }
}