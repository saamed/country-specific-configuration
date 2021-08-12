using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ConfigurationProvider.Configuration
{
    public class JwtCountryCodeProvider : ICountryCodeProvider
    {
        private const string AuthorizationHeaderName = "Authorization";
        private const string CountryClaimName = "country";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtConfiguration _jwtConfiguration;

        public JwtCountryCodeProvider(IHttpContextAccessor httpContextAccessor,
            IOptions<JwtConfiguration> jwtConfigurationOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtConfiguration = jwtConfigurationOptions.Value;
        }

        public string Provide()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context is null)
            {
                return null;
            }

            if (!context.Request.Headers.TryGetValue(AuthorizationHeaderName, out var token))
            {
                return null;
            }

            if (token.Count > 1)
            {
                throw new Exception("Ambiguous authorization headers");
            }

            var tokenValue = ((string) token).Split(" ").Last();
            var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);
            var jwtToken = GetJwtSecurityToken(tokenValue, key);
            var country = jwtToken.Claims.FirstOrDefault(x => x.Type == CountryClaimName)?.Value;

            return country;
        }

        private static JwtSecurityToken GetJwtSecurityToken(string tokenValue, byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(tokenValue, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = false
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken) validatedToken;
            return jwtToken;
        }
    }
}