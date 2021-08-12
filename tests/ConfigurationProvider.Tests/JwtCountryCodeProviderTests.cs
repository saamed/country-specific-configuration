using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConfigurationProvider.Configuration;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace ConfigurationProvider.Tests
{
    public class JwtCountryCodeProviderTests
    {
        private const string JwtKey = "SOME_JWT_KEY_THAT_NEEDS_TO_BE_LONG";
        private const string Country = "PL";

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtCountryCodeProvider _provider;

        public JwtCountryCodeProviderTests()
        {
            var jwtConfigurationOptions = Mock.Of<IOptions<JwtConfiguration>>();
            Mock.Get(jwtConfigurationOptions)
                .Setup(m => m.Value)
                .Returns(new JwtConfiguration() {Secret = JwtKey});

            _httpContextAccessor = Mock.Of<IHttpContextAccessor>();

            _provider = new JwtCountryCodeProvider(_httpContextAccessor, jwtConfigurationOptions);
        }

        [Fact]
        public void GivenJwtWithCountryClaim_ShouldReturnCountryValue()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", $"Bearer {GenerateToken(true, Country)}");
            Mock.Get(_httpContextAccessor).Setup(m => m.HttpContext).Returns(context);

            _provider.Provide().Should().BeNull();
        }

        [Fact]
        public void GivenJwtWithoutCountryClaim_ShouldReturnNull()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers.Add("Authorization", $"Bearer {GenerateToken(false)}");
            Mock.Get(_httpContextAccessor).Setup(m => m.HttpContext).Returns(context);

            _provider.Provide().Should().BeNull();
        }

        [Fact]
        public void GivenNullHttpContext_ShouldReturnNull()
        {
            Mock.Get(_httpContextAccessor).Setup(m => m.HttpContext).Returns((HttpContext) null);
            _provider.Provide().Should().BeNull();
        }

        [Fact]
        public void GivenNoAuthorizationHeader_ShouldReturnNull()
        {
            Mock.Get(_httpContextAccessor).Setup(m => m.HttpContext).Returns(new DefaultHttpContext());
            _provider.Provide().Should().BeNull();
        }

        private string GenerateToken(bool addCountryClaim, string countryClaimValue = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {new Claim("sub", "1234567890")}),
            };

            if (addCountryClaim)
            {
                descriptor.Claims = new Dictionary<string, object>()
                {
                    {"Country", countryClaimValue}
                };
            }

            var jwtKeyBytes = Encoding.ASCII.GetBytes(JwtKey);
            descriptor.SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(jwtKeyBytes), SecurityAlgorithms.HmacSha256);
            return handler.CreateEncodedJwt(descriptor);
        }
    }
}