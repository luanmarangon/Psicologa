using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Agendamento.Entities
{
    public class Disponibilidade : EntityBase
    {
        public Domain.Pessoa.Entities.Pessoa Psicologo { get; set; }
        public DateTime DataConsulta { get; set; }
        public List<HorarioAgendado> HorariosAgendados { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
