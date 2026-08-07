using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Financeiro.Entities
{
    public class FinanceiroCategoria : EntityBase
    {

        public enum TipoCategoria
        {
            [Description("Receita")]
            Receita = 1,
            [Description("Despesa")]
            Despesa = 2
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public TipoCategoria Tipo { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
