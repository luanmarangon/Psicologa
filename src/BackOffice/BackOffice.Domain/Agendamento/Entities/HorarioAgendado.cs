using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Agendamento.Entities
{
    public class HorarioAgendado :EntityBase
    {
        public string HoraInicio { get; set; }
        public string HoraFim { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }

    }
}
