using System;

namespace Shared.Domain.Cidade.Entities
{
    public class CidadeUF : EntityBase
    {
        public string Nome { get; set; }
        public string Sigla { get; set; }

        public CidadeUF() 
        {

        }

        public override bool Validar()
        {
            return true;
        }
    }
}
