using System.Collections.Generic;

namespace Psicologa.Domain.Usuario.Interfaces.Repositories
{
    public interface IPerfilUsuarioRepository : IRepositoryBase<Usuario.Entities.PerfilUsuario>
    {
        bool Salvar(Entities.PerfilUsuario perfilUsuario);
        bool SalvarPerfil(Entities.PerfilUsuarioCadastro perfilUsuarioCadastro);
        Entities.PerfilUsuario Obter(int id);
        IEnumerable<Usuario.Entities.PerfilUsuario> ObterTodos();
    }
}
