
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psicologa.Application.Dashboard.Services;
using Psicologa.Application.Usuario.ViewsModel;
using Psicologa.Domain.Dashboard.Services;
using Psicologa.Domain.Usuario.Entities;
using Psicologa.Presentation.Painel;
using Psicologa.Presentation.Painel.Controllers;
using MySqlX.XDevAPI;
using Shared.Infra.CrossCutting;
using System.Net;
using static Psicologa.Domain.Usuario.Entities.PerfilUsuario;

namespace Psicologa.Areas.Administrativo.Presentation.Painel.Controllers
{
    [Permissao(Tipo = PerfilUsuarioViewModel.TpPermissao.Backoffice)]
    [Area("Administrativo")]

    public class DashboardController : BaseController
    {
        private readonly Application.Dashboard.Services.ApplicationIndicadorService _indicadorService;
        private readonly UsuarioAutenticado _ua;

        public DashboardController(ApplicationIndicadorService indicadorService, UsuarioAutenticado ua)
        {
            _indicadorService = indicadorService;
            _ua = ua;

        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["statusCode"] = HttpContext.Response.StatusCode + " - " + ((HttpStatusCode)HttpContext.Response.StatusCode).ToString();
            return View();
        }


        #region Dashboard
        //[AllowAnonymous]
        [HttpGet]
        //public IActionResult ObterIndicador()
        //{
        //    var perfil = _ua.PerfilNome;
        //    int usuarioId = Convert.ToInt32(_ua.Id);
        //    var teste;

        //    if (perfil == "Master")
        //    {

        //        teste = _indicadorService.ObterIndicadoresAdmin();
        //        teste.Perfil = perfil;
        //    }

        //    if (perfil == "Prestador")
        //    {
        //         teste = _indicadorService.ObterIndicadoresPrestador(usuarioId);
        //        teste.Perfil = perfil;

        //    }

        //    if(perfil == "Cliente")
        //    {
        //        teste = _indicadorService.ObterIndicadoresCliente(usuarioId);
        //        teste.Perfil = perfil;
        //    }
        //    return DefaultJSONResponse(true, teste);
        //}
        public IActionResult ObterIndicador()
        {
            var perfil = _ua.PerfilNome;
            int usuarioId = Convert.ToInt32(_ua.Id);

            if (perfil == "MASTER")
            {
                var teste = _indicadorService.ObterIndicadoresMaster();
                teste.Perfil = perfil;
                return DefaultJSONResponse(true, teste);
            }
            else if (perfil == "ADMINISTRATIVO")
            {
                var teste = _indicadorService.ObterIndicadoresAdministrativo();
                //teste.Perfil = perfil;
                return DefaultJSONResponse(true, teste);
            }
            else if (perfil == "PSICOLOGO")
            {
                var teste = _indicadorService.ObterIndicadoresPsicologos(_ua.PessoaId);
                //teste.Perfil = perfil;
                return DefaultJSONResponse(true, teste);
            }
            else
            {
                var teste = _indicadorService.ObterIndicador();   
                teste.Perfil = perfil;
                return DefaultJSONResponse(true, teste);
            }
        }
        #endregion

    }
}
