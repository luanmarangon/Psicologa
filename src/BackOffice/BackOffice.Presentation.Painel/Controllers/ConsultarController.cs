using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psicologa.Application;
using Psicologa.Application.BlogPost.Services;
using Psicologa.Application.Consultar.Services;
using Shared.Infra.CrossCutting;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Presentation.Painel.Controllers
{
    [AllowAnonymous]
    public class ConsultarController : BaseController
    {
        private readonly IAppSettings _appSettings;

        private readonly ApplicationBlogPostService _blog;

        private readonly ApplicationConsultarService _consultar;

        public ConsultarController(IAppSettings appSettings, ApplicationBlogPostService blog, ApplicationConsultarService consultar)
        {
            _appSettings = appSettings;
            _blog = blog;
            _consultar = consultar;
        }

        //[HttpGet]
        //public IActionResult Index(string q)
        //{
        //    PaginacaoDados paginacao = new PaginacaoDados(0, 12, (PaginacaoDados.TpOrdenacao)1); //12
        //    var dados = _consultar.Consultar(q, paginacao);

        //    ViewBag.Dados = dados;

        //    return View();
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public IActionResult Index(string q)
        {
           if(!string.IsNullOrEmpty(q))
            {
                PaginacaoDados paginacao = new PaginacaoDados(
                0,
                12,
                (PaginacaoDados.TpOrdenacao)1
            );

                var dados = _consultar.Consultar(q, paginacao);

                ViewBag.Dados = dados;
                ViewBag.Pesquisa = q;
            }

            return View();
        }

    }
}