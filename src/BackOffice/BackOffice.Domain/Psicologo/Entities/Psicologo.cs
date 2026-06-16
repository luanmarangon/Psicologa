using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Psicologo.Entities
{
    public class Psicologo: EntityBase
    {
        public int Id { get; set; }
        public int PessoaId { get; set; }
        public string Crp { get; set; }
        public string CrpUf { get; set; }
        public DateTime DataEmissaoCrp { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
