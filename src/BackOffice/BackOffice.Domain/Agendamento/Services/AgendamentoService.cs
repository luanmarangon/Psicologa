using Psicologa.Domain.Agendamento.Entities;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Agendamento.Services
{
    public class AgendamentoService : ServiceBase<Entities.Agendamento>, IServiceBase<Entities.Agendamento>
    {
        public readonly Interfaces.Repositories.IAgendamentoRepository _repository;

        public AgendamentoService(Interfaces.Repositories.IAgendamentoRepository repository)
            : base(repository)
        {
            _repository = repository;
        }
        
        public bool Salvar(Entities.Agendamento agendamento)
        {
            if (agendamento.Id == 0)
            {
                agendamento.DataCriacao = DateTime.Now;
                agendamento.DataAtualizacao = DateTime.Now;
            }
            else
            {
                agendamento.DataAtualizacao = DateTime.Now;
            }
            return _repository.Salvar(agendamento);
        }
        public IEnumerable<Disponibilidade> ObterDisponibilidade(int psicologoId, DateTime dataConsulta)
        {
            return _repository.ObterDisponibilidade(psicologoId, dataConsulta);
        }
        public Domain.Agendamento.Entities.Agendamento ObterPorId(int id)
        {
            return _repository.ObterPorId(id);
        }

        public Domain.Agendamento.Entities.Agendamento ObterAgendamentoPorPaciente(int pacienteId, int psicologoId, DateTime data)
        {
            return _repository.ObterAgendamentoPorPaciente(pacienteId, psicologoId, data);
        }
        public bool Excluir(int id)
        {
            return _repository.Excluir(id);
        }
        public IEnumerable<Domain.Agendamento.Entities.Agendamento> Consultar(string termo, Domain.Agendamento.Entities.Agendamento.tpFiltro filtro, PaginacaoDados paginacao)
        {
            if (string.IsNullOrEmpty(termo))
                termo = "";
            else
                termo = termo.Replace("%", "").Replace("_", "");

            return _repository.Consultar(termo, filtro, paginacao);
        }
    }
}