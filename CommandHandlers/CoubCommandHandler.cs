using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
            foreach (var coub in coubs) await CoubAsync(coub);
        }

        [Command("c")]
        [Alias("с", "coub", "коуб")]
        [RequireContext(ContextType.Guild)]
        public async Task CoubAsync(string coub)
        {
            if (await _userRepository.CheckIgnoringUser(Context.User.Id))
                return;

            _logger.LogInformation(
                $"User: {Context.User.Username} execute command in {Context.Guild.Name} with param: {coub}");
            if (!coub.Contains("coub.com/view/"))
                return;

            var name = coub.Split('/').Last();
            if (!File.Exists($"coubs/{name}.mp4"))
            {
                var request = new RestRequest($"{CoubsUrlBase}{name}", Method.GET);
                request.AddHeader("Cookie", "is_logged_in=false");
                var response = await _restClient.ExecuteAsync(request);

                if (string.IsNullOrEmpty(response.Content))
                    return;

                if (response.Content.Contains("Coub not found"))
                {
                    await ReplyAsync($"Coub: {coub} - не найден");
                    return;
                }

                var coubModel = JsonConvert.DeserializeObject<CoubModel>(response.Content);

                var client = new RestClient();
                if (string.IsNullOrEmpty(coubModel.file_versions.share.@default))
                {
                    await ReplyAsync(
                        $"Coub: {coub} has no share link. Please click the \"Download\" button on the Coub website for a link to appear. And try again");
                    return;
                }

                var requestFile = new RestRequest($"{coubModel.file_versions.share.@default}", Method.GET);
                var responseBytes = await client.ExecuteAsync(requestFile);

                if (responseBytes.StatusCode != HttpStatusCode.OK)
                    return;

                if (!Directory.Exists("coubs"))
                    Directory.CreateDirectory("coubs");

                await File.WriteAllBytesAsync($"coubs/{name}.mp4", responseBytes.RawBytes);
            }

            await Context.Channel.SendFileAsync($"coubs/{name}.mp4");
            await Context.Channel.DeleteMessageAsync(Context.Message);
        }
    }
}