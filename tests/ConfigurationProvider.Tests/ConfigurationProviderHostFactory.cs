using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationProvider.Tests
{
    public class ConfigurationProviderHostFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddOptions<TestConfiguration>()
                    .BindConfiguration(TestConfiguration.SectionName);
            });
        }
    }
}