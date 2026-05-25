using System;

namespace Psicologa.Domain.Pessoa.Entities
{
    public class Endereco : EntityBase
    {
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string CEP { get; set; }
        public string Complemento { get; set; }
        //public Shared.Domain.Cidade.Entities.Cidade Cidade { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string PontoReferencia { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public Endereco() 
        {
            //Cidade = new Shared.Domain.Cidade.Entities.Cidade();
        }

        public override bool Validar()
        {
            throw new NotImplementedException();
        }
    }
}
