using Shared.Infra.CrossCutting.ValidationResult;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Psicologa.Domain.Pessoa.Entities
{
    public class Pessoa : EntityBase
    {
        public enum TpDoc
        {
       
            [Description("Indefinido")]
            Indefinido = 0,
            
            [Description("CPF")]
            CPF = 1,
             
            [Description("Passaporte")]
            Passaporte = 2,
             
            [Description("CNPJ")]
            CNPJ = 3,
             
            [Description("Cadastro Único Brasileiro (PF)")]
            CadastroUnicoPF = 4
        }
        
        public string Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string DocIdNro { get; set; }
        public TpDoc DocIdTipo { get; set; }
        public Endereco Endereco { get; set; }
        public List<PessoaContato> Contatos { get; set; }
        public List<PessoaTipo> Tipos { get; set; }
        public bool Ativo { get; set; }

        public Pessoa()
        {
            DocIdTipo = TpDoc.Indefinido;
            Nome = "";
            DataCadastro = DateTime.MinValue;
            Endereco = new Endereco();
            Contatos = new List<PessoaContato>();
            Tipos = new List<PessoaTipo>();
            Ativo = true;
        }

        public override bool Validar()
        {

            DocIdNro = DocIdNro.Trim().Replace(".", "").Replace("-", "").Replace("/", "");

            if (DocIdTipo == Pessoa.TpDoc.CPF)
            {
                DocIdNro = DocIdNro.PadLeft(11, '0');
            }

            if(DocIdTipo == Pessoa.TpDoc.CNPJ)
            {
                DocIdNro = DocIdNro.PadLeft(14, '0');
            }

            if (Nome.Trim() == "")
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Nome é obrigatório.");
            }
            else if (Nome.Split(" ").Length < 2)
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Nome inválido.");
            }
            else if (DocIdNro == "")
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Documento de Identificação é necessário.");
            }
            else if (DocIdTipo == Pessoa.TpDoc.Indefinido)
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Tipo do Documento de Identificação é necessário.");
            }

            if(this.Tipos.Count(c => c.Tipo != PessoaTipo.TpPessoa.Psicologo) > 0)
            {
                if (base.ValidationResult.Messages.Length == 0)
                {
                    if (this.GetType() == typeof(PessoaJuridica))
                    {
                        PessoaJuridica pj = (PessoaJuridica)this;
                        pj.RazaoSocial = pj.RazaoSocial.Trim();

                        if (pj.RazaoSocial == "")
                        {
                            base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Razão Social é obrigatório.");
                        }
                        else if (pj.DocIdNro != "" && !PessoaJuridicaUtils.ValidarCNPJ(pj.DocIdNro))
                        {
                            base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O CNPJ é inválido.");

                        }
                    }
                    else
                    {
                        PessoaFisica pf = (PessoaFisica)this;

                        if (pf.DataNascimento == DateTime.MinValue)
                        {
                            base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Data de Nascimento é obrigatório.");
                        }
                        else if (pf.DocIdTipo == Pessoa.TpDoc.CPF && pf.DocIdNro != "" && !PessoaFisicaUtils.ValidarCPF(pf.DocIdNro))
                        {
                            base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O CPF é inválido.");
                        }
                    }
                }

                if (Endereco != null && base.ValidationResult.Messages.Length == 0)
                {
                    Endereco.Logradouro = Endereco.Logradouro.Trim();
                    Endereco.Numero = Endereco.Numero.Trim();
                    Endereco.Bairro = Endereco.Bairro.Trim();
                    Endereco.CEP = Endereco.CEP.Trim();

                    if (Endereco.Logradouro == "" || Endereco.Numero == "" || Endereco.Bairro == "" || Endereco.CEP == "" || Endereco.Cidade == "")
                    {
                        base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Forneça o endereço.");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Endereco.Latitude) && Endereco.Latitude.Length > 11)
                        {
                            base.ValidationResult.Add(Message.TypeMessage.InvalidField, "A Latitude tem muitos caracteres. Limite em 11.");
                        }
                        else if (!string.IsNullOrEmpty(Endereco.Longitude) && Endereco.Longitude.Length > 11)
                        {
                            base.ValidationResult.Add(Message.TypeMessage.InvalidField, "A Longitude tem muitos caracteres. Limite em 11.");
                        }
                    }
                }
            }

            if (base.ValidationResult.Messages.Length == 0)
            {
                foreach (var c in Contatos)
                {
                    if (c.Contato.Trim() == "")
                    {
                        base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Toda forma de contato precisa de um contato.");
                        break;
                    }
                }
            }


            if (base.ValidationResult.Messages.Length == 0 && Tipos.Count == 0)
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "O Tipo da Pessoa precisa ser definido.");
            }

            return base.ValidationResult.Count == 0;

        }
    }
}
