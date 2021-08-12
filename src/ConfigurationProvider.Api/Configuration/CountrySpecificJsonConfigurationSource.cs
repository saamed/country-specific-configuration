using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ConfigurationProvider.Configuration
{
    public class CountrySpecificJsonConfigurationSource : JsonConfigurationSource
    {
        private readonly IEnumerable<ICountryCodeProvider> _countryCodeProviders;
        private readonly string _country;

        public CountrySpecificJsonConfigurationSource(IEnumerable<ICountryCodeProvider> countryCodeProviders,
            string country, string environment)
        {
            _countryCodeProviders = countryCodeProviders;
            _country = country;
            Optional = false;
            Path = $"appsettings.{environment}.{country}.json";
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new CountrySpecificJsonConfigProvider(_countryCodeProviders, _country, this);
        }
    }
}