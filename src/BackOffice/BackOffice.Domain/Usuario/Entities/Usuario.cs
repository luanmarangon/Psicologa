using Shared.Infra.CrossCutting.ValidationResult;
using System;

namespace Psicologa.Domain.Usuario.Entities
{
    public class Usuario : EntityBase
    {
        public string Nome { get; set; }
        public string Senha { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataSenha { get; set; }
        public Domain.Pessoa.Entities.Pessoa Pessoa { get; set; }
        public PerfilUsuario.TpPerfil Perfil { get; set; }
        //public string  Perfil { get; set; }
        public string CodigoSeguranca { get; set; }


        public Usuario()
        {
            Nome = "";
            Senha = "";
            DataCadastro = new DateTime();
            DataSenha = new DateTime();
            Pessoa = new Pessoa.Entities.Pessoa();
            Perfil = PerfilUsuario.TpPerfil.Indefinido;
            //Perfil = "Indefinido";
            CodigoSeguranca = "";
        }

        public override bool Validar()
        {
            if (string.IsNullOrEmpty(Nome))
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Nome de Usuário é obrigatório.");
            }
            else if (Pessoa.Id <= 0)
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Selecione uma Pessoa (usuário).");
            }
            else if (Perfil == PerfilUsuario.TpPerfil.Indefinido)
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Selecione um Perfil (usuário).");
            }
            else if (Id <= 0 && string.IsNullOrEmpty(Senha))
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "A Senha é obrigatória.");
            }

            return base.ValidationResult.Count == 0;
        }

        public bool Validar(string senhaConfirmacao)
        {
            if (Validar())
            {
                if (Id <= 0)
                {
                    if (string.IsNullOrEmpty(senhaConfirmacao))
                    {
                        base.ValidationResult.Add(Message.TypeMessage.InvalidField, "A confirmação da Senha é obrigatória.");
                    }
                    else if (Senha != senhaConfirmacao)
                    {
                        base.ValidationResult.Add(Message.TypeMessage.InvalidField, "As senhas são diferentes.");
                    }
                }
            }
            
            return base.ValidationResult.Count == 0;
        }



    }
}
