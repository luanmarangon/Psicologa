using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.ProntuarioSessao.Entities
{
    public class ProntuarioSessao : EntityBase
    {
        public int Id { get; set; }
        public Domain.Prontuario.Entities.Prontuario Prontuario { get; set; }
        public Domain.Agendamento.Entities.Agendamento? Agendamento { get; set; }
        public DateTime DataSessao { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public Domain.Pessoa.Entities.Pessoa? Psicologa { get; set; }
        public Domain.Agendamento.Entities.Agendamento.tpFiltro TipoAtendimento { get; set; }
        public string Evolucao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        //public string HumorPaciente { get; set; }
        //public string TecnicasAplicadas { get; set; }
        //public string ProximosPassos { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
