using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Psicologa.Application.Configuracao.ViewsModel
{
    public class ConfiguracaoFuncionamentoViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }

        public List<FuncionamentoDiaViewModel> Funcionamento { get; set; }
    }

    public class FuncionamentoDiaViewModel
    {
        public int DiaSemana { get; set; }

        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public List<FuncionamentoPeriodoViewModel> Periodos { get; set; }
    }

    public class FuncionamentoPeriodoViewModel
    {
        public string HoraInicio { get; set; }

        public string HoraFim { get; set; }
    }
}
