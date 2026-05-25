
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Shared.Infra.CrossCutting.ClienteContratoAPIKey
{
    public class SigningConfiguration
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfiguration(string key)
        {
            Key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
        }
    }
}
