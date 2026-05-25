using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Configuracao.Entities
{
    public class ConfiguracaoFuncionamento : EntityBase
    {
        public int Id { get; set; }

        public List<ConfiguracaoFuncionamentoDia> Funcionamento { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}