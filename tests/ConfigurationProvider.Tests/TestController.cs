using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ConfigurationProvider.Tests
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly TestConfiguration _testConfiguration;

        public TestController(IOptionsSnapshot<TestConfiguration> testConfiguration)
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