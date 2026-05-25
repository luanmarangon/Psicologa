using System;

namespace Shared.Domain.Cidade.Entities
{
    public class Cidade : EntityBase
    {
    
        public string Nome { get; set; }
        public int IBGEMunicipio { get; set; }
        public int IBGEMunicipioCompleto { get; set; }
        public CidadeUF UF { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public Cidade()
        {
            UF = new CidadeUF();
        }

        public override bool Validar()
        {
            return true;
        }
    }
}
