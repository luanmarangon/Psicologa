using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psicologa.Application;
using Psicologa.Application.Usuario.Services;
using Psicologa.Application.Usuario.ViewsModel;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System.Security.Claims;

namespace Psicologa.Presentation.Painel.Controllers
{

    public class LoginController : BaseController
    {
        private readonly ApplicationUsuarioService _uService;
        private readonly UsuarioAutenticado _usuarioAutenticado;
        private readonly IAppSettings _appSettings;

        public LoginController(ApplicationUsuarioService uService, UsuarioAutenticado usuarioAutenticado, IAppSettings appSettings)
        {
            _uService = uService;
            _usuarioAutenticado = usuarioAutenticado;
            _appSettings = appSettings;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (_usuarioAutenticado.Autenticado)
                return Redirect(_appSettings.BaseURL + "Home");

            return View();
        }

        [AllowAnonymous]
        public IActionResult Sair()
        {
            //Removendo a cookie de autenticação
            Microsoft.AspNetCore.Authentication
                .AuthenticationHttpContextExtensions
                .SignOutAsync(HttpContext);

            return Redirect("/Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Validar(string nome, string senha, string autenticado)
        {
            string urlRedirect = "";
            UsuarioConsultaViewModel usuario = null;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(senha))
            {
                AddUserMessageInvalidField("Informe o nome de usuário e senha.");
            }
            else
            {
                usuario = _uService.Obter(nome, senha);
            }

            if (usuario == null)
                AddUserMessageError("Dados inválidos.");
            else
            {
                //urlRedirect = _appSettings.BaseURL + "Home";
                urlRedirect = _appSettings.BaseURL + "Administrativo/dashboard";

                #region Criando ao cookie de autenticação
                var userClaims = new List<Claim>()
                {
                    new Claim("usuarioId", Criptografia.Criptografar(usuario.Id.ToString(), true)),
                    new Claim("pessoaId", Criptografia.Criptografar(usuario.PessoaId.ToString(), true)),
                    new Claim("nome", usuario.PessoaNome),
                    new Claim("perfilNome", usuario.PerfilNome),
                    new Claim("perfilId", Criptografia.Criptografar(usuario.PerfilId.ToString(), true)),
                    new Claim(ClaimTypes.Role, usuario.PerfilNome)
                };

                var identity = new ClaimsIdentity(userClaims, "Identificação do usuário - Portal Academy");
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);


                //Gerando a cookie e informando que deve ser persistente, isto é, mantém o usuário autenticado.
                AuthenticationHttpContextExtensions.SignInAsync(HttpContext, principal, new AuthenticationProperties
                {
                    IsPersistent = !string.IsNullOrEmpty(autenticado) && autenticado == "on",
                    ExpiresUtc = DateTime.UtcNow.AddDays(_appSettings.CookieAuthDaysLife)
                });


                #endregion
            }

            return DefaultJSONResponse(usuario != null, new
            {
                urlRedirect
            });

           
        }

        [AllowAnonymous]
        public IActionResult RecuperarSenha()
        {

            if (_usuarioAutenticado != null && _usuarioAutenticado.Id > 0)
            {
                ViewBag.Logado = true;
            }

            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult EnviarEmailRecuperarSenha(string nome)
        {
            bool operacao = false;

            if (string.IsNullOrEmpty(nome))
            {
                AddUserMessageInvalidField("Forneça o nome de usuário.");
            }
            else
            {
                try
                {
                    ValidationResult vr = new ValidationResult();
                    
                    (operacao, vr) = _uService.EnviarEmailRecuperarSenha(nome);
                    AddUserMessage(vr);
                }
                catch  
                {
                    AddUserMessageInvalidField("Um erro ocorreu.");

                }
            }

            return DefaultJSONResponse(operacao);

        }


        [AllowAnonymous]
        [HttpPost]
        public IActionResult AlterarSenha(string nome, string codigoSeguranca, string senha, string senhaConfirmar)
        {
            bool operacao = false;

            if (string.IsNullOrEmpty(nome))
            {
                AddUserMessageInvalidField("Forneça o nome de usuário.");
            }
            else
            {
                try
                {
                    ValidationResult vr = new ValidationResult();
                    (operacao, vr) = _uService.AlterarSenha(nome, codigoSeguranca, senha, senhaConfirmar);
                    AddUserMessage(vr);
                }
                catch
                {
                    AddUserMessageError("Um erro ocorreu.");
                }
            }

            return DefaultJSONResponse(operacao);
        }



    }

}
