using Microsoft.AspNetCore.Mvc;

namespace Psicologa.Presentation.Painel.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Erro/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewData["statusCode"] = "404";
                    ViewData["titulo"] = "Página não encontrada";
                    ViewData["descricao"] = "A página que você tentou acessar não existe ou foi removida.";
                    break;
                case 403:
                    ViewData["statusCode"] = "403";
                    ViewData["titulo"] = "Acesso negado";
                    ViewData["descricao"] = "Você não tem permissão para acessar esta página.";
                    break;
                case 401:
                    ViewData["statusCode"] = "401";
                    ViewData["titulo"] = "Não autenticado";
                    ViewData["descricao"] = "Você precisa estar logado para acessar esta página.";
                    break;
                default:
                    ViewData["statusCode"] = statusCode.ToString();
                    ViewData["titulo"] = "Ocorreu um erro";
                    ViewData["descricao"] = "Ocorreu um erro inesperado. Tente novamente ou entre em contato com o suporte.";
                    break;
            }

            return View("Erro");
        }

        [Route("Erro")]
        public IActionResult Erro()
        {
            ViewData["statusCode"] = "500";
            ViewData["titulo"] = "Erro interno";
            ViewData["descricao"] = "Ocorreu um erro inesperado ao processar sua solicitação. Nossa equipe já foi notificada.";
            return View("Erro");
        }
    }
}