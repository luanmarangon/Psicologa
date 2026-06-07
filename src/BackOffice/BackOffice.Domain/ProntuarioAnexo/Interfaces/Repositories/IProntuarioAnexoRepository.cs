using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.ProntuarioAnexo.Interfaces.Repositories
{
    public interface IProntuarioAnexoRepository:IRepositoryBase<Entities.ProntuarioAnexo>
    {
        bool Salvar(Entities.ProntuarioAnexo anexo);
        Entities.ProntuarioAnexo Obter(int id);
        IEnumerable<Entities.ProntuarioAnexo> Consultar(string termo, int protocoloId, Domain.ProntuarioAnexo.Entities.ProntuarioAnexo.tpTipoAnexo tpAnexo, PaginacaoDados paginacao);
        bool Excluir(int id);
    }
}
