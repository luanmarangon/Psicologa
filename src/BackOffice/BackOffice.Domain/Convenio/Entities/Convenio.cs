using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Convenio.Entities
{
    public class Convenio : EntityBase
    {
        public string Nome{ get; set; }
        public string Icon { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public bool Ativo { get; set; }
        public bool DestaqueHome { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
