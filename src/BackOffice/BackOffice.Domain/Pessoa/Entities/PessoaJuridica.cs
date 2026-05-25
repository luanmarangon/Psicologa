using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Psicologa.Domain.Pessoa.Entities
{
    public class PessoaJuridica : Pessoa
    {

        public string RazaoSocial { get; set; }

        public PessoaJuridica() : base()
        {
            RazaoSocial = "";
        }

        public PessoaJuridica(Pessoa pessoa) : base()
        {
            RazaoSocial = "";

            if(pessoa != null)
            {
                // Pega lista de PropertyInfos
                List<PropertyInfo> filhaInfos = this.GetType().GetProperties().ToList();
                List<PropertyInfo> paiInfos = pessoa.GetType().GetProperties().ToList();

                // Para cada PropertyInfo da classe pai
                foreach (PropertyInfo itemPai in paiInfos)
                {
                    // Para cada PropertyInfo da classe filha
                    foreach (PropertyInfo itemFilha in filhaInfos)
                    {
                        // Verifica se é o mesmo atributo
                        if (itemFilha.Name == itemPai.Name)
                        {
                            // Atribue o valor para a classe atual
                            base.GetType().GetProperty(itemPai.Name).SetValue(this, itemPai.GetValue(pessoa, null));
                            // Remove o atributo da lista da classe filha
                            filhaInfos.Remove(itemFilha);
                            break;
                        }
                    }
                }
            }
        }

        public override bool Validar()
        {
            return base.Validar();
        }
    }
}
