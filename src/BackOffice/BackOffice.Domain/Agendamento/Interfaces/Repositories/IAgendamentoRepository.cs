using Psicologa.Domain.Agendamento.Entities;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Agendamento.Interfaces.Repositories
{
    public interface IAgendamentoRepository: IRepositoryBase<Entities.Agendamento>
    {
        IEnumerable<Disponibilidade> ObterDisponibilidade(int psicologoId, DateTime dataConsulta);
        IEnumerable<Domain.Agendamento.Entities.Agendamento> Consultar(string termo, Domain.Agendamento.Entities.Agendamento.tpFiltro filtro, PaginacaoDados paginacao);
        bool Salvar(Entities.Agendamento agendamento);
        Domain.Agendamento.Entities.Agendamento ObterPorId(int id);
        Domain.Agendamento.Entities.Agendamento ObterAgendamentoPorPaciente(int pacienteId, int psicologoId, DateTime data);
        bool Excluir(int id);
    }
}
