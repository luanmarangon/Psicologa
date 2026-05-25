using Shared.Infra.CrossCutting;
using System.Security.Claims;

namespace Psicologa.Presentation.Painel
{
    public class RequisicaoAtual
    {
        private readonly IHttpContextAccessor _accessor;

        public RequisicaoAtual(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            var context = _accessor.HttpContext;

            if (context != null)
            {
                try
                {
                    Ip = context.Connection.RemoteIpAddress?.ToString();

                    IpReal = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                             ?? Ip;

                    UserAgent = context.Request.Headers["User-Agent"].ToString();

                    var isMobile = UserAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase);
                    var isTablet = UserAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase);

                    Dispositivo = isMobile ? "Mobile" : isTablet ? "Tablet" : "Desktop";
                }
                catch
                {
                    // opcional: log
                }
            }
        }

        public string Ip { get; set; }
        public string IpReal { get; set; }
        public string UserAgent { get; set; }
        public string Dispositivo { get; set; }

        //public string[] ToArray(string usuarioNome, string usuarioId)
        //{
        //    return new[] { IpReal, UserAgent, Dispositivo, usuarioNome, usuarioId };
        //}
        public string[] ToArray(UsuarioAutenticado ua)
        {
            return new[] { IpReal, UserAgent, Dispositivo, ua.Nome, ua.Id.ToString() };
        }
    }
}
