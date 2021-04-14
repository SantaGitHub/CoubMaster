using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JewishCat.DiscordBot.EFCore;
using JewishCat.DiscordBot.EFCore.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestSharp;

namespace JewishCat.DiscordBot
{
    public class Program
    {
        private const string BaseUrl = "https://coub.com/api/v2/";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<LocalDbContext>(builder => builder.UseSqlite());
                    services.AddTransient<IUserRepository, UserRepository>();

                    services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Info,
                        MessageCacheSize = 1000,
                        AlwaysDownloadUsers = true
                    }));
                    services.AddSingleton(new CommandService(new CommandServiceConfig
                    {
                        LogLevel = LogSeverity.Info,
                        DefaultRunMode = RunMode.Async
                    }));
                    services.AddScoped<IRestClient, RestClient>(_ => new RestClient(BaseUrl)
                    {
                        UserAgent = "Discord Bot"
                    });
                    services.AddHostedService<BotBackgroundService>();
                });
        }
    }
}