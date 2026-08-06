using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Financeiro.Entities
{
    public class Financeiro : EntityBase
    {
        public enum TpTipoLancamento
        {
            [Description("Despesa")]
            Despesa = 1,
            [Description("Receita")]
            Receita = 2,
        }

        public int Id { get; set; }
        public TpTipoLancamento Tipo { get; set; }
        public string Descricao { get; set; }
        public Domain.Financeiro.Entities.FinanceiroCategoria Categoria { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataLancamento { get; set; }
        public string Observacao { get; set; }
        public bool Ativo { get; set; }
        public bool Quitado { get; set; }
        public DateTime DataQuitacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }



        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
