using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConfigurationProvider.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddCountrySpecificConfiguration(this IConfigurationBuilder builder,
            HostBuilderContext context, string countryCode)
        {
            var serviceProvider = new ServiceCollection()
                .AddHttpContextAccessor()
                .AddTransient<ICountryCodeProvider, JwtCountryCodeProvider>()
                .Configure<JwtConfiguration>(context.Configuration.GetSection(JwtConfiguration.SectionName))
                .AddSingleton(sp =>
                    ActivatorUtilities.CreateInstance<CountrySpecificJsonConfigurationSource>(sp, countryCode,
                        context.HostingEnvironment.EnvironmentName))
                .BuildServiceProvider();

            var sources = serviceProvider.GetServices<CountrySpecificJsonConfigurationSource>();

            foreach (var source in sources)
            {
                builder.Add(source);
            }

            return builder;
        }
    }
}