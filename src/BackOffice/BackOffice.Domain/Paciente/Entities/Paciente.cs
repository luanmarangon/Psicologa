using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Domain.Paciente.Entities
{
    public class Paciente:EntityBase
    {
        public Domain.Pessoa.Entities.Pessoa Pessoa { get; set; }
        public DateTime DataPrimeiraSessao { get; set; }
        public bool Ativo { get; set; }
        public string? Observacoes { get; set; }
        public string? ContatoEmergenciaNome { get; set; }
        public string? ContatoEmergenciaTelefone { get; set; }
        public Domain.Pessoa.Entities.Pessoa? Responsavel { get; set; }
        public string Matricula { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public Domain.Prontuario.Entities.Prontuario? Prontuario { get; set; }


        public override bool Validar()
        {
            if(!string.IsNullOrEmpty(Observacoes) && Observacoes.Length > 1000)
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "A observação deve conter no máximo 1000 caracteres.");



            return base.ValidationResult.Count == 0;
        }
    }
}
