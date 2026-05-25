using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Configuracao.Entities
{
    public class ConfiguracaoFuncionamentoDia : EntityBase
    {
        public int DiaSemana { get; set; }

        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public List<ConfiguracaoFuncionamentoPeriodo> Periodos { get; set; }

        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}