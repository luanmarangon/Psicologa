using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Psicologo.Interfaces.Repositories
{
    public interface IPsicologoRepository: IRepositoryBase<Entities.Psicologo>
    {
        bool Salvar(Entities.Psicologo psicologo);
        public Entities.Psicologo Obter(int id);
        public Entities.Psicologo ObterPorPessoaId(int pessoaId);
    }
}
