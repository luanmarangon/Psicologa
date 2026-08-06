using Microsoft.AspNetCore.Mvc;
using Psicologa.Presentation.Painel.Controllers;

namespace Psicologa.Presentation.Painel.Areas.Administrativo.Controllers
{
    public class AdministrativoController : BaseController
    {
        public AdministrativoController()
        {
        }

        public IActionResult Index()
        {
            return Redirect("/Administrativo/Dashboard");
        }
    }
}
