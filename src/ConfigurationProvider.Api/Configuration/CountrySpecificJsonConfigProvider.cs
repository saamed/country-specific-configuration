using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration.Json;

namespace ConfigurationProvider.Configuration
{
    public class CountrySpecificJsonConfigProvider : JsonConfigurationProvider
    {
        private readonly IEnumerable<ICountryCodeProvider> _countryCodeProviders;
        private readonly string _country;

        public CountrySpecificJsonConfigProvider(IEnumerable<ICountryCodeProvider> countryCodeProviders,
            string country,
            JsonConfigurationSource source) : base(source)
        {
            _countryCodeProviders = countryCodeProviders;
            _country = country;
        }

        public override bool TryGet(string key, out string value)
        {
            if (_countryCodeProviders.Select(provider => provider.Provide())
                .Any(countryCode => countryCode != null && countryCode == _country))
            {
                return base.TryGet(key, out value);
            }

            value = null;
            return false;
        }
    }
}