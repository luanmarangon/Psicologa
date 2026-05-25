using System;
using System.ComponentModel;

namespace Psicologa.Domain.Pessoa.Entities
{
    public class PessoaContato : EntityBase
    {
        public enum TpContato
        {

            [Description("Indefinido")]
            Indefinido = 0,
    
            [Description("Celular")]
            Celular = 1,

            [Description("Telefone")]
            Telefone = 2,
      
            [Description("WhatsApp")]
            WhatsApp = 3,
            
            [Description("e-mail")]
            Email = 4,

            [Description("Outro")]
            Outro = 5,
          
            [Description("Celular + WhatsApp")]
            CelularWhatsApp = 6
        }

 
        public string Contato { get; set; }
        public TpContato Tipo { get; set; }
        public string Observacao { get; set; }

        public PessoaContato():base()
        {
            Contato = "";
            Tipo = TpContato.Indefinido;
            Observacao = "";
        }

        public override bool Validar()
        {
            throw new NotImplementedException();
        }
    }
}
