using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Psicologa.Application.Usuario.ViewsModel;


namespace Psicologa.Presentation.Painel
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class Permissao : AuthorizeAttribute, IAuthorizationFilter
    {
        public PerfilUsuarioViewModel.TpPermissao Tipo { get; set; }

        public PerfilUsuarioViewModel.TpPermissao[] Tipos { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var ua = (UsuarioAutenticado)context.HttpContext.RequestServices.GetService(typeof(UsuarioAutenticado));
            IEnumerable<PerfilUsuarioViewModel.TpPermissao> permissoes = ua.Permissoes;

            var isAuthorized = false;

            if (permissoes != null)
            {
                if (Tipo != PerfilUsuarioViewModel.TpPermissao.Indefinido)
                    isAuthorized = permissoes.Contains(Tipo);

                if (Tipos != null)
                {
                    isAuthorized = permissoes.Where(p => Tipos.Contains(p)).Count() > 0;
                }
            }

            if (!isAuthorized)
            {
                context.Result = new StatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
                return;
            }

        }
    }
}
