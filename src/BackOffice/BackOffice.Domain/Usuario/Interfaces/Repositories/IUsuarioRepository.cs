using System.Collections.Generic;

namespace Psicologa.Domain.Usuario.Interfaces.Repositories
{
    public interface IUsuarioRepository : IRepositoryBase<Entities.Usuario>
    {
        bool Salvar(Entities.Usuario perfilUsuario);
        Entities.Usuario Obter(int id);
        bool Excluir(int id);
        bool ExcluirPorPessoa(int pessoaId);

        Entities.Usuario Obter(string usuarioNome, string senha);
        IEnumerable<Entities.Usuario> Consultar(string nome);
        IEnumerable<Entities.Usuario> ObterUltimos(int top);
        Entities.Usuario ObterPorUsuarioNome(string usuarioNome);
        Entities.Usuario ObterPorPessoa(int pessoaId);
        void AtualizarCodigoSeguranca(int id, string codigoSeguranca);
        bool AlterarSenha(int id, string senha);
        int ObterQuantidadeUsuariosCadastrados();

    }
}
