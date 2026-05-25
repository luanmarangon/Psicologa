using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Infra.CrossCutting.ClienteContratoAPIKey
{
    public class AppSettings
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }

        public AppSettings()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(AppContext.BaseDirectory).AddJsonFile(".vartechs/VarTechsClienteContratoAPIKey.appsettings.json", false);
            var root = configurationBuilder.Build();
            Audience = root.GetSection("AppSettings").GetSection("JWTAudience").Value;
            Issuer = root.GetSection("AppSettings").GetSection("JWTIssuer").Value;
            Key = root.GetSection("AppSettings").GetSection("JWTKey").Value;
        }

    }
}
