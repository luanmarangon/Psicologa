using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Financeiro.Entities
{
    public class ResumoFinanceiro : EntityBase
    {
        public decimal TotalReceita { get; set; }
        public decimal TotalDespesa { get; set; }
        public decimal Saldo { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
