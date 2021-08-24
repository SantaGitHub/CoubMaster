using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using JewishCat.DiscordBot.EFCore.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;

namespace JewishCat.DiscordBot.CommandHandlers
{
    public class CoubCommandHandler : ModuleBase
    {
        private const string CoubsUrlBase = "coubs/";
        private readonly ILogger<CoubCommandHandler> _logger;
        private readonly IRestClient _restClient;
        private readonly IUserRepository _userRepository;

        public CoubCommandHandler(IRestClient client, ILogger<CoubCommandHandler> logger,
            IUserRepository userRepository)
        {
            _restClient = client;
            _logger = logger;
            _userRepository = userRepository;
        }

        [Command("c")]
        [Alias("с", "coub", "коуб")]
        [RequireContext(ContextType.Guild)]
        public async Task CoubAsync(params string[] coubs)
        {
            if (await _userRepository.CheckIgnoringUser(Context.User.Id))
                return;
            await CoubAsync(string.Join(' ', coubs));
        }

        [Command("c")]
        [Alias("с", "coub", "коуб")]
        [RequireContext(ContextType.Guild)]
        public async Task CoubAsync(string input)
        {
            if (await _userRepository.CheckIgnoringUser(Context.User.Id))
                return;

            _logger.LogInformation(
                $"User: {Context.User.Username} execute command in {Context.Guild.Name} with param: {input}");
            if (string.IsNullOrEmpty(input) || !input.Contains("coub.com/view/"))
                return;
            
            var coubLink = input.Split(' ').First();
            var ping = input.Split(' ').Length > 1 ? string.Join(' ', input.Split(' ')[1..]) : string.Empty;
            var name = coubLink.Split('/').Last();

            var iconUrl = string.IsNullOrEmpty(Context.User.GetAvatarUrl())
                ? Context.User.GetDefaultAvatarUrl()
                : Context.User.GetAvatarUrl();
            var builder = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder()
                {
                    Name = (await Context.Guild.GetUserAsync(Context.User.Id)).Nickname,
                    IconUrl = iconUrl
                },
                Color = Color.Orange
            };
            builder.AddField(x =>
            {
                x.Name = "Link to coub:";
                x.Value = coubLink;
                x.IsInline = false;
            });
            
            var request = new RestRequest($"{CoubsUrlBase}{name}", Method.GET);
            request.AddHeader("Cookie", "is_logged_in=false");
            var response = await _restClient.ExecuteAsync(request);

            if (string.IsNullOrEmpty(response.Content))
                return;

            if (response.Content.Contains("Coub not found"))
            {
                builder.Color = Color.Red;
                builder.AddField(x =>
                {
                    x.Name = "Message:";
                    x.Value = "Coub не найден";
                    x.IsInline = false;
                });
                await ReplyAsync("", embed: builder.Build());
                await Context.Channel.DeleteMessageAsync(Context.Message);
                return;
            }

            var coubModel = JsonConvert.DeserializeObject<CoubModel>(response.Content);

            var nsfw = coubModel.tags.Any(i => i.id == 67203 || i.title.Contains("nsfw")) || coubModel.communities.Any(i => i.id == 24 || i.title.Contains("nsfw"));

            var client = new RestClient();
            if (string.IsNullOrEmpty(coubModel.file_versions.share.@default))
            {
                builder.Color = Color.Red;
                builder.AddField(x =>
                {
                    x.Name = "Message:";
                    x.Value = "Coub has no share link. Please click the \"Download\" button on the Coub website for a link to appear. And try again";
                    x.IsInline = false;
                });
                await ReplyAsync("", embed: builder.Build());
                await Context.Channel.DeleteMessageAsync(Context.Message);
                return;
            }
            
            if (!File.Exists($"coubs/{name}.mp4"))
            {
                var requestFile = new RestRequest($"{coubModel.file_versions.share.@default}", Method.GET);
                var responseBytes = await client.ExecuteAsync(requestFile);

                if (responseBytes.StatusCode != HttpStatusCode.OK)
                    return;

                if (!Directory.Exists("coubs"))
                    Directory.CreateDirectory("coubs");

                await File.WriteAllBytesAsync($"coubs/{name}.mp4", responseBytes.RawBytes);
            }


            if (!string.IsNullOrEmpty(ping))
            {
                builder.AddField(x =>
                {
                    x.Name = "Message:";
                    x.Value = ping;
                    x.IsInline = false;
                });
            }
            
            await Context.Channel.SendFileAsync($"coubs/{name}.mp4", "", embed: builder.Build(), isSpoiler: nsfw);
            await Context.Channel.DeleteMessageAsync(Context.Message);
        }
    }
}