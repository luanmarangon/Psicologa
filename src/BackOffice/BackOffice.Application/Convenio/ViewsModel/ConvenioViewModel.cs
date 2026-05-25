using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Psicologa.Application.Convenio.ViewsModel
{
    public class ConvenioViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }

        public string Nome { get; set; }
        public string Icon { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public bool Ativo { get; set; }
        public bool DestaqueHome { get; set; }
    }

    public class ConvenioConsultaViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }

        public string Nome { get; set; }
        public string Icon { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }

        public bool Ativo { get; set; }
        public bool DestaqueHome { get; set; }
    }
}