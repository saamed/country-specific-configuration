using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConfigurationProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly TestConfiguration _testConfiguration;

        public CountryController(IOptionsSnapshot<TestConfiguration> testConfiguration)
        {
            _testConfiguration = testConfiguration.Value;
        }

        [HttpGet]
        public object Get()
        {
            return new {TestConfigurationValue = _testConfiguration.Value};
        }
    }
}