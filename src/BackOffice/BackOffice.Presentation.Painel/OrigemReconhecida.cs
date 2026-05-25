using Psicologa.Application;
using Shared.Infra.CrossCutting;

namespace Psicologa.Presentation.Painel
{
    public class OrigemReconhecida
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly string _cookieNome = "origem-reconhecida";

        private readonly IAppSettings _appSettings;
        private readonly ILogger<OrigemReconhecida> _logger;    

        public OrigemReconhecida(IHttpContextAccessor accessor, IAppSettings appSettings, ILogger<OrigemReconhecida> logger)
        {
            _accessor = accessor;
            _appSettings = appSettings;
            _logger = logger;

            var ck = _accessor.HttpContext.Request.Cookies[_cookieNome];

            if (!string.IsNullOrEmpty(ck))
            {

                try
                {

                    string cookieValor = Criptografia.Descriptografar(ck);


                    if (_appSettings.OrigensReconhecidas.Any(origem => cookieValor.Contains(origem)))
                    {
                        Reconhecida = true;
                        ColaboradorEGestora = cookieValor.Contains("www.vartechs.com.br");
                        CriarCookie(cookieValor);
                    }
                    else Desprezar();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro na obtenção da cookie das Origens Reconhecidas.");


                }
            }
            else
            {

                try
                {
                    string referer = "";

                    if (_accessor.HttpContext.Request.Headers.Referer.Any())
                    {
                        referer = _accessor.HttpContext.Request.Headers.Referer.ToString();
                    }

#if (DEBUG)
                    referer = "localhost";
#endif


                    if (_appSettings.OrigensReconhecidas.Any(origem => referer.Contains(origem)))
                    {
                        CriarCookie(referer);
                    }
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Erro na Identificação das Origens Reconhecidas.");
                }
            }
        }

        private void CriarCookie(string origem)
        {
            string cookieValor = Criptografia.Criptografar(origem);

            _accessor.HttpContext.Response.Cookies.Append(_cookieNome, cookieValor, new CookieOptions()
            {
                Expires = DateTime.Now.Date.AddDays(365),
                HttpOnly = true
            });
        }

        public void Desprezar()
        {
            Reconhecida = false;
            ColaboradorEGestora = false;
            _accessor.HttpContext.Response.Cookies.Delete(_cookieNome);
        }


        public bool Reconhecida { get; set; }
        public bool ColaboradorEGestora { get; set; }


    }
}
