using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JewishCat.DiscordBot
{
    public class BotBackgroundService : BackgroundService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly ILogger<BotBackgroundService> _logger;
        private readonly IServiceProvider _services;
        private readonly string _token;


        public BotBackgroundService(DiscordSocketClient client, ILogger<BotBackgroundService> logger,
            CommandService commands, IServiceProvider services)
        {
            _client = client;
            _logger = logger;
            _commands = commands;
            _services = services;
            _token = "ODE0MTQ0MjY5MDUyMjgwODMz.YDZk7w.Go2qHfbqw_vL99EpVx9KM0IsKeo";
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.MessageReceived += ClientOnMessageReceived;
            _commands.Log += CommandsOnLog;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(),
                _services);
            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();
            _logger.LogInformation("Started Bot");
            await base.StartAsync(cancellationToken);
        }

        private Task CommandsOnLog(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical("Exception: " + arg.Exception.Message + "Message: " + arg.Message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError("Exception: " + arg.Exception.Message + "Message: " + arg.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning("Message: " + arg.Message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation("Message: " + arg.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace("Message: " + arg.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug("Message: " + arg.Message);
                    break;
                default:
                    _logger.LogError("Message: " + arg.Message);
                    break;
            }

            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!stoppingToken.IsCancellationRequested) continue;
                _client.LogoutAsync();
                _client.Dispose();
            }

            return Task.CompletedTask;
        }

        private async Task ClientOnMessageReceived(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null) return;
            var argPos = 0;
            var guild = message.Channel is SocketGuildChannel channel ? channel.Guild : (SocketGuild) null;
            _logger.LogTrace($"Message in {guild?.Name} from {message.Author.Username}: {message.Content}");
            if (!(message.HasCharPrefix('!',
                      ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser,
                      ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped Bot");
            await _client.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}