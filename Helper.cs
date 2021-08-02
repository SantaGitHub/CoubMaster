using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JewishCat.DiscordBot
{
    public static class Helper
    {
        public static IConfigurationBuilder AddConfigsForBuilder(this IConfigurationBuilder builder, IHostEnvironment env)
        {
            var result = builder
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
            result.AddEnvironmentVariables();
            return result;
        }
    }
}