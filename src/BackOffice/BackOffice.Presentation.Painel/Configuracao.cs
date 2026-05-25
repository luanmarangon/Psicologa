using Psicologa.Application.Configuracao.Services;

namespace Psicologa.Presentation.Painel
{
    public class Configuracao
    {
        private readonly ApplicationConfiguracaoService _config;

        public Configuracao(ApplicationConfiguracaoService config)
        {
            _config = config;

            var dados = _config.ObterConfiguracao();

            if (dados != null)
            {
                Nome = dados.Nome;

                CEP = dados.CEP;
                Endereco = dados.Endereco;
                Numero = dados.Numero;
                Complemento = dados.Complemento;
                Bairro = dados.Bairro;
                Cidade = dados.Cidade;
                Estado = dados.Estado;

                Whatsapp = dados.Whatsapp;
                Email = dados.Email;

                Facebook = dados.Facebook;
                Instagram = dados.Instagram;
                Linkedin = dados.Linkedin;
                LinkWhatsapp = $"https://api.whatsapp.com/send?phone={dados.Whatsapp
                                                                                    .Replace("+", "")
                                                                                    .Replace(" ", "")
                                                                                    .Replace("-", "")
                                                                                    .Replace("(", "")
                                                                                    .Replace(")", "")}";
                Slogan = dados.Slogan;


            }

        }

        public string Nome { get; set; }
        public string CEP { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Whatsapp { get; set; }
        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string Linkedin { get; set; }
        public string LinkWhatsapp { get; set; }
        public string Slogan { get; set; }

    }
}