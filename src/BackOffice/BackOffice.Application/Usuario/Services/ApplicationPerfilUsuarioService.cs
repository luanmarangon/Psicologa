using Psicologa.Application.Usuario.ViewsModel;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using Shared.Infra.CrossCutting;

namespace Psicologa.Application.Usuario.Services
{
    public class ApplicationPerfilUsuarioService : IDisposable
    {
        private readonly Domain.Usuario.Services.PerfilUsuarioService _puService;

        public ApplicationPerfilUsuarioService(Domain.Usuario.Services.PerfilUsuarioService puService)
        {
            _puService = puService;
        }

        public (bool, ValidationResult) Salvar(PerfilUsuarioViewModel puVM)
        {
            bool operacao = false;

            Domain.Usuario.Entities.PerfilUsuario pu = new Domain.Usuario.Entities.PerfilUsuario();
            pu.Id = puVM.Id;
            pu.Perfil = (Domain.Usuario.Entities.PerfilUsuario.TpPerfil)pu.Id;
            //pu.Perfil = puVM.Nome;
            pu.Permissoes = new List<Domain.Usuario.Entities.PerfilUsuario.TpPermissao>();
            foreach (var item in puVM.Permissoes)
            {
                pu.Permissoes.Add((Domain.Usuario.Entities.PerfilUsuario.TpPermissao)(int)item);
            }

            operacao = _puService.Salvar(pu);

            if (operacao)
                puVM.Id = pu.Id;


            return (operacao, pu.ValidationResult);
        }


        public IEnumerable<PerfilUsuarioViewModel> ObterTodos()
        {
            List<PerfilUsuarioViewModel> retorno = new List<PerfilUsuarioViewModel>();
            var ps = _puService.ObterTodos();

            foreach (var p in ps)
            {
                retorno.Add(FormatarRetornoConsulta(p));
            }
           
            return retorno;
        }

        public List<Domain.Usuario.Entities.PerfilUsuario.TpPermissao> ObterPermissoes()
        {
            List<Domain.Usuario.Entities.PerfilUsuario.TpPermissao> ps = new List<Domain.Usuario.Entities.PerfilUsuario.TpPermissao>();
            foreach (Domain.Usuario.Entities.PerfilUsuario.TpPermissao e in Enum.GetValues(typeof(Domain.Usuario.Entities.PerfilUsuario.TpPermissao)))
            {
                if (e != Domain.Usuario.Entities.PerfilUsuario.TpPermissao.Indefinido)
                    ps.Add(e);
            }

            return ps;
        }


        public PerfilUsuarioViewModel Obter(int id)
        {
            var u = _puService.Obter(id);

            return FormatarRetornoConsulta(u);
        }

        internal PerfilUsuarioViewModel FormatarRetornoConsulta(Domain.Usuario.Entities.PerfilUsuario pu)
        {
            if (pu == null)
                return null;

            var puVM = new PerfilUsuarioViewModel()
            {
                Id = pu.Id,
                Nome = Utils.ObterDescricaoEnum(pu.Perfil)
            };

            puVM.Permissoes = new List<PerfilUsuarioViewModel.TpPermissao>();
            puVM.PermissoesNomes = new List<string>();
            foreach (var perm in pu.Permissoes)
            {
                puVM.Permissoes.Add((PerfilUsuarioViewModel.TpPermissao)(int)perm);
                puVM.PermissoesNomes.Add(Utils.ObterDescricaoEnum(perm));
            }

            return puVM;
        }

        public (bool, ValidationResult) SalvarPerfil(PerfilUsuarioCadastroViewModel puVM)
        {
            bool operacao = false;
            Domain.Usuario.Entities.PerfilUsuarioCadastro pu = new Domain.Usuario.Entities.PerfilUsuarioCadastro();
            pu.Id = puVM.Id;
            pu.Nome = puVM.Nome;
            operacao = _puService.SalvarPerfil(pu);
            if (operacao)
                puVM.Id = pu.Id;

            return (operacao, pu.ValidationResult);


        }
        public void Dispose()
        {

        }
    }

}
