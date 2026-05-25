using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Servico.Interfaces.Repositories
{
    public interface IServicoContatoRepository: IRepositoryBase<Entities.Servico>
    {
        bool Salvar(Entities.Servico servico);
        Entities.Servico Obter(int servicoId);
        IEnumerable<Entities.Servico> Obter(string[] filtro);
        IEnumerable<Domain.Servico.Entities.Servico> ObterTodos();
        IEnumerable<Domain.Servico.Entities.Servico> ObterTodosAtivos();
        bool Excluir (int servicoId);
        IEnumerable<Domain.Servico.Entities.Servico> ObterDestaquesHome(int limite);
        IEnumerable<Domain.Servico.Entities.Servico> Consultar(string termo, Domain.Servico.Entities.Servico.TpFiltroServico filtro, PaginacaoDados paginacao);
        Entities.Servico ObterPorUrl(string url);
        Entities.Servico ObterPorNome(string nome);
    }
}
