
using System;

namespace Psicologa.Domain.Prontuario.Entities
{
    public class Prontuario:EntityBase
    {
        public int Id { get; set; }
        public Domain.Paciente.Entities.Paciente Paciente { get; set; }
        public string QueixaPrincipal { get; set; }
        public string ObjetivoTratamento { get; set; }
        public string HistoricoFamiliar { get; set; }
        public string ObservacoesIniciais { get; set; }
        public bool Ativo {  get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataEncerramento { get; set; }

        public override bool Validar()
        {
            return base.ValidationResult.Count == 0;
        }
    }
}
