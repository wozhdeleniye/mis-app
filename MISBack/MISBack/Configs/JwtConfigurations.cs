using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MISBack.Configs
{
    public class JwtConfigurations
    {
        public const string Issuer = "MISBackendDevelop";
        public const string Audience = "MISFronted";
        private const string Key = "Le0n228HotM0nk1yLol321H0wToKakAt";
        public const int Lifetime = 60;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
