using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JewishCat.DiscordBot.EFCore.Interface;
using JewishCat.DiscordBot.Models;
using Microsoft.Extensions.Logging;

namespace JewishCat.DiscordBot.CommandHandlers
{
    [Group("admin")]
    public class AdminCommandHandler : ModuleBase
    {
        private const ulong JewishCatId = 128057102798159872;
        private readonly ILogger<AdminCommandHandler> _logger;

        public AdminCommandHandler(ILogger<AdminCommandHandler> logger)
        {
            _logger = logger;
        }

        [Command("leave")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task LeaveGuild()
        {
            Context.Guild.LeaveAsync();
            _logger.LogInformation($"Bot leaving from {Context.Guild.Name}");
            return Task.CompletedTask;
        }

        [Group("ignore")]
        public class IgnoreCommandHandler : ModuleBase
        {
            private readonly ILogger<IgnoreCommandHandler> _logger;
            private readonly IUserRepository _userRepository;

            public IgnoreCommandHandler(IUserRepository userRepository, ILogger<IgnoreCommandHandler> logger)
            {
                _userRepository = userRepository;
                _logger = logger;
            }

            [Command("add")]
            [RequireContext(ContextType.Guild)]
            public async Task AddIgnoreUser(IUser socketUser)
            {
                if (socketUser == null || Context.User.IsBot || Context.User.Id != JewishCatId)
                    return;

                var user = await _userRepository.SingleOrDefaultUser(socketUser.Id);
                if (user == null)
                {
                    user = new User(socketUser.Id, socketUser.Username);
                    await _userRepository.AddUser(user);
                }

                user.SetIgnore();
                await _userRepository.SaveEntity();
                _logger.LogInformation($"User: {socketUser.Username} - has been added to the blacklist");
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }

            [Command("remove")]
            [RequireContext(ContextType.Guild)]
            public async Task RemoveIgnoreUser(IUser socketUser)
            {
                if (socketUser == null || Context.User.IsBot || Context.User.Id != JewishCatId)
                    return;

                var user = await _userRepository.SingleOrDefaultUser(socketUser.Id);
                if (user == null)
                {
                    await ReplyAsync($"User: {socketUser.Username} - not found in blacklist");
                    return;
                }

                user.ResetIgnore();
                await _userRepository.SaveEntity();
                _logger.LogInformation($"User: {socketUser.Username} - has been removed to the blacklist");
                await Context.Channel.DeleteMessageAsync(Context.Message);
            }
        }
    }
}