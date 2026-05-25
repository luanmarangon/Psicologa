using System.Text.Json.Serialization;
using Shared.Infra.CrossCutting.JSONConverter;

namespace Psicologa.Application.Usuario.ViewsModel
{
    public class UsuarioViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string SenhaConfirmacao { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PerfilId { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int ClienteIdVinculo { get; set; }
        public string PerfilNome { get; set; }

    }

    public class UsuarioConsultaViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Nome { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PessoaId { get; set; }
        public string PessoaNome { get; set; }
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int PerfilId { get; set; }
        public string PerfilNome { get; set; }
    }

}
