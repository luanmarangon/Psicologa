using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Prontuario.Interfaces.Repositories
{
    public interface IProntuarioRepository:IRepositoryBase<Entities.Prontuario>
    {
        Domain.Prontuario.Entities.Prontuario Obter(int prontuarioId);
        Domain.Prontuario.Entities.Prontuario ObterProntuarioPorPacienteId(int pacienteId);
        bool Salvar(Domain.Prontuario.Entities.Prontuario prontuario);
    }
}
