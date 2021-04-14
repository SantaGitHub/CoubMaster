using System.Threading.Tasks;
using Discord.Commands;
using JewishCat.DiscordBot.EFCore.Interface;

namespace JewishCat.DiscordBot.CommandHandlers
{
    public class HelpCommandHandler : ModuleBase
    {
        private readonly IUserRepository _userRepository;

        public HelpCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            if (await _userRepository.CheckIgnoringUser(Context.User.Id))
                return;
            await ReplyAsync("Usage: !c <link to coub.com> - Alias: !c !с !coub !коуб");
            await Context.Channel.DeleteMessageAsync(Context.Message);
        }
    }
}