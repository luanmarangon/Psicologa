using Shared.Domain;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Documentos.Interfaces.Repositories
{
    public interface IDocumentosRepository: IRepositoryBase<Entities.Documentos>
    {
        bool Salvar(Psicologa.Domain.Documentos.Entities.Documentos documento);
        Domain.Documentos.Entities.Documentos Obter(int id);
        IEnumerable<Domain.Documentos.Entities.Documentos> Consultar(string termo, int tp, PaginacaoDados paginacao);
    }
}
