using System;
using System.ComponentModel;

namespace Psicologa.Domain.Pessoa.Entities
{
    public class PessoaTipo : EntityBase
    {
        public enum TpPessoa
        {
            [Description("Indefinido")]
            Indefinido = 0,

            [Description("Cliente")]
            Cliente = 1,
            [Description("Colaborador")]
            Colaborador = 2,
            [Description("Psicologo")]
            Psicologo = 3,
        }
        
        public TpPessoa Tipo { get; set; }

        public PessoaTipo():base()
        {
            Tipo = TpPessoa.Indefinido;
        }

        public override bool Validar()
        {
            throw new NotImplementedException();
        }
    }
}
