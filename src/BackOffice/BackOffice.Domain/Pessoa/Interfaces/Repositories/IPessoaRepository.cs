using Psicologa.Domain.Pessoa.Entities;
using Shared.Infra.CrossCutting;
using System.Collections.Generic;

namespace Psicologa.Domain.Pessoa.Interfaces.Repositories
{
    public interface IPessoaRepository : IRepositoryBase<Entities.Pessoa>
    {
        bool Salvar(Entities.Pessoa pessoa);
        bool SalvarPorIntegracao(Entities.Pessoa pessoa);
        Entities.Pessoa Obter(int id);
        bool Excluir(int id);
        Entities.Pessoa ObterPorDocumentoIdentificacao(string documento);
        Entities.Pessoa ObterPorFormaContato(string contato);
        IEnumerable<Entities.Pessoa> Consultar(string nome, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido);
        IEnumerable<Entities.Pessoa> ObterUltimos(int top, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido);
        IEnumerable<Entities.Pessoa> ObterTodosClientesAtivos();
        IEnumerable<Entities.Pessoa> ObterUsuariosColaboradores(int clienteIdVinculo, string termo);

    }
}
