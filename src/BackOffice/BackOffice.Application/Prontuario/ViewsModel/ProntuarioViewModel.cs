using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Psicologa.Application.Prontuario.ViewsModel
{
    public class ProntuarioViewModel
    {
        public int Id { get; set; }
        public Domain.Paciente.Entities.Paciente Paciente { get; set; }
        public string QueixaPrincipal { get; set; }
        public string ObjetivoTratamento { get; set; }
        public string HistoricoFamiliar { get; set; }
        public string ObservacoesIniciais { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataEncerramento { get; set; }
    }

    public class ProntuarioConsultaViewModel
    {
        public int Id { get; set; }
        public Domain.Paciente.Entities.Paciente Paciente { get; set; }
        public string QueixaPrincipal { get; set; }
        public string ObjetivoTratamento { get; set; }
        public string HistoricoFamiliar { get; set; }
        public string ObservacoesIniciais { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime? DataEncerramento { get; set; }



    }
}
