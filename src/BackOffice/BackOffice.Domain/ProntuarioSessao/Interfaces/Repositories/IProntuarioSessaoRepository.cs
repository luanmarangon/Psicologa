using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.ProntuarioSessao.Interfaces.Repositories
{
    public interface IProntuarioSessaoRepository : IRepositoryBase<Entities.ProntuarioSessao>
    {
        bool EvoluirSessao(Entities.ProntuarioSessao sessao);

        IEnumerable<Domain.ProntuarioSessao.Entities.ProntuarioSessao> Consultar(string termo, int protocoloId, int filtroTipoAtendimento, PaginacaoDados paginacao);

        Entities.ProntuarioSessao ObterSessao(int id);

        bool ExcluirSessao(int id);
    }
}