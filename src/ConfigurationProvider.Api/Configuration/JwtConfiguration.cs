namespace ConfigurationProvider.Configuration
{
    public class JwtConfiguration
    {
        public const string SectionName = "JWT";

        public string Secret { get; set; }
    }
}