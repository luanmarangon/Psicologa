using Psicologa.Application.Usuario.Services;
using Psicologa.Application.Usuario.ViewsModel;
using Shared.Infra.CrossCutting;
using System.Security.Claims;

namespace Psicologa.Presentation.Painel
{
    public class UsuarioAutenticado
    {
        private readonly IHttpContextAccessor _accessor;

        public UsuarioAutenticado(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

            if (_accessor.HttpContext.User.Claims.Count() > 0)
            {
                try
                {
                    Id = Convert.ToInt32(Criptografia.Descriptografar(_accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "usuarioId").Value));
                    IdCriptografado = _accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "usuarioId").Value;
                    PessoaId = Convert.ToInt32(Criptografia.Descriptografar(_accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "pessoaId").Value));
                    PessoaIdCriptografado = _accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "pessoaId").Value;
                    PessoaJuridicaEmpresaId = Convert.ToInt32(Criptografia.Descriptografar(_accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "pessoaId").Value));
                    Nome = _accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "nome").Value;
                    PerfilNome = _accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "perfilNome").Value;
                    PerfilId = Convert.ToInt32(Criptografia.Descriptografar(_accessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "perfilId").Value));
                    NomeAbreviado = Nome;
                    var aux = NomeAbreviado.Split(' ');
                    if (aux.Length > 2)
                    {
                        NomeAbreviado = aux[0] + " " + aux[aux.Length - 1];
                    }

                    var pus = (ApplicationPerfilUsuarioService)_accessor.HttpContext.RequestServices.GetService(typeof(ApplicationPerfilUsuarioService));
                    Permissoes = pus.Obter(PerfilId).Permissoes;

                    Autenticado = true;
                }
                catch { }
            }
        }

        public long Id { get; set; }
        public string IdCriptografado { get; set; }
        public int PessoaId { get; set; }
        public string PessoaIdCriptografado { get; set; }
        public string Nome { get; set; }
        public string NomeAbreviado { get; set; }
        public string PerfilNome { get; set; }
        public int PerfilId { get; set; }
        public IEnumerable<PerfilUsuarioViewModel.TpPermissao> Permissoes { get; set; }
        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }

        /// <summary>
        /// Id da Empresa (pessoaId) associado ao usuário autenticado. Se a empresa e e a pessoa do usuário foram os mesmos, os ids serão iguais.
        /// </summary>
        public int PessoaJuridicaEmpresaId { get; set; }
        public bool Autenticado { get; set; }
        
    

    }
}
