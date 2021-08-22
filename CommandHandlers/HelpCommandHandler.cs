using System.Threading.Tasks;
using Discord;
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
            var embedMessage = new EmbedBuilder()
            {
                Color = Color.Blue,
                Description = "Bot commands",
                Author = new EmbedAuthorBuilder()
                {
                    Name = Context.Client.CurrentUser.Username,
                    IconUrl = Context.Client.CurrentUser.GetAvatarUrl()
                }
            };
            embedMessage.AddField(x =>
            {
                x.Name = "Commands:";
                x.Value = "Usage: !c <link to coub.com> - Alias: !c !с !coub !коуб";
                x.IsInline = false;
            });
            await ReplyAsync("", embed: embedMessage.Build());
            await Context.Channel.DeleteMessageAsync(Context.Message);
        }
    }
}