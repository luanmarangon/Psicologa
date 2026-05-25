using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Configuracao.Entities
{
    public class ConfiguracaoFuncionamentoPeriodo : EntityBase
    {
        public string HoraInicio { get; set; }

        public string HoraFim { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}