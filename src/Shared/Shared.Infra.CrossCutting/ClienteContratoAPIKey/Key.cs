using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Shared.Infra.CrossCutting.ClienteContratoAPIKey
{
    public static class Key
    {

        public static string NewAPIKey(int clienteContratoId)
        {
            AppSettings settings = new AppSettings();

            var userClaims = new List<Claim>()
            {
                new Claim("data", DateTime.Now.ToString())
            };

            var ig = new GenericIdentity(clienteContratoId.ToString(), "id");
            var identidade = new ClaimsIdentity(ig, userClaims);

            var handler = new JwtSecurityTokenHandler();
            SigningConfiguration signingConfiguration =
                new SigningConfiguration(settings.Key);
            var dadosToken = handler.CreateToken(new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Audience = settings.Audience,
                Issuer = settings.Issuer,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddYears(30),
                Subject = identidade,
                SigningCredentials = signingConfiguration.SigningCredentials
            });

            return handler.WriteToken(dadosToken);
        }


        public static string GetTokenId(string token)
        {
            string id = "";
            try
            {
                var handler = new JwtSecurityTokenHandler();

                var dadosToken = handler.ReadJwtToken(token);
                id = dadosToken.Claims.FirstOrDefault(a => a.Type == "unique_name").Value;
            }
            catch { }

            return id;
        }
    }
}
