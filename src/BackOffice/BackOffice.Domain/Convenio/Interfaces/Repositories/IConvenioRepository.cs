using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Convenio.Interfaces.Repositories
{
    public interface IConvenioRepository: IRepositoryBase<Entities.Convenio>
    {
        bool Salvar(Entities.Convenio convenio);
        IEnumerable<Domain.Convenio.Entities.Convenio> Consultar(string termo, PaginacaoDados paginacao);
        IEnumerable<Domain.Convenio.Entities.Convenio> ConsultarUltimos(int quantidade);
        IEnumerable<Domain.Convenio.Entities.Convenio> ObterDestaquesHome();
        Entities.Convenio Obter(int convenioId);
        bool Excluir(int convenioId);
    }
}
