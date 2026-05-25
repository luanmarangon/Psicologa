using Psicologa.Domain.Usuario.Interfaces.Repositories;
using System.Collections.Generic;

namespace Psicologa.Domain.Usuario.Services
{
    public class PerfilUsuarioService : ServiceBase<Domain.Usuario.Entities.PerfilUsuario>, IServiceBase<Domain.Usuario.Entities.PerfilUsuario>
    {
        public readonly Interfaces.Repositories.IPerfilUsuarioRepository _repository;


        public PerfilUsuarioService(IPerfilUsuarioRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public new bool Salvar(Domain.Usuario.Entities.PerfilUsuario pu)
        {
            bool operacao = false;
            if (pu.Validar())
            {
                operacao = _repository.Salvar(pu);
            }

            return operacao;
        }

        public Domain.Usuario.Entities.PerfilUsuario Obter(int id)
        {
            return _repository.Obter(id);
        }
        public IEnumerable<Domain.Usuario.Entities.PerfilUsuario> ObterTodos()
        {
            return _repository.ObterTodos();
        }

        public bool SalvarPerfil(Domain.Usuario.Entities.PerfilUsuarioCadastro pu)
        {
            bool operacao = false;
            if (pu.Validar())
            {
                operacao = _repository.SalvarPerfil(pu);
            }
            return operacao;
        }
    }
}
