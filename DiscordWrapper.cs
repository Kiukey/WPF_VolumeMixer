using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace VolumeMixer
{
    internal class DiscordWrapper 
    {
        private const string TokenEndpoint = "https://discord.com/api/oauth2/token";
        private const string authorizationLink = "https://discord.com/oauth2/authorize?client_id=1229701026307903488&response_type=code&redirect_uri=https%3A%2F%2Fdiscord.com%2FRedirection&scope=identify+connections+voice";
        private const string discordBaseApiEndPoint = "https://discord.com/api/v10";
        private string token = "";
        //DiscordRestClient client = null;
        HttpClient httpClient = null;
        //SocketSelfUser user = null;
        public DiscordWrapper()
        {
            httpClient = new HttpClient();


        }

        public async void Initilize(string _code)
        {
            //client = new DiscordRestClient();
            ////client.UserVoiceStateUpdated += OnUserVoiceStateUpdated;
            await GetAccessTokenAsync(_code);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await httpClient.GetAsync("https://discord.com/api/v10/applications/1229701026307903488/commands");
            string tokenResponseJson = await response.Content.ReadAsStringAsync();

            //GetCurrentChannel();
            //await client.LoginAsync(TokenType.Bearer,_token);
            //client.


        }

        async Task<string> GetAccessTokenAsync(string code)
        {
            Dictionary<string, string> tokenRequestParameters = new Dictionary<string, string>
            {
                { "client_id", "1229701026307903488" },
                { "client_secret", "ibHps4-HJQJVxw02yJQREtqIBEl0Ihz4" },
                { "code", code},
                { "redirect_uri", "https://discord.com/Redirection" },
                { "grant_type", "authorization_code" }
            };

            var tokenRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
            var tokenResponse = await httpClient.PostAsync(TokenEndpoint, tokenRequestContent);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to obtain access token: {tokenResponse.ReasonPhrase}");
            }

            string tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
            string accessToken = JObject.Parse(tokenResponseJson)["access_token"].ToString();

            //Settings1.Default.Token = accessToken;
            //Settings1.Default.Save();
            token = accessToken;
            return token;
        }

        public void AskPermission()
        {
            Process.Start(authorizationLink);
        }

        //async Task<string> GetCurrentChannel()
        //{
        //    httpClient.GetAsync(discordBaseApiEndPoint + "/commands/")

        //    return "";
        //}
        #region Comments
        //private const string authorizationLink = "https://discord.com/oauth2/authorize?client_id=1229701026307903488&response_type=code&redirect_uri=https%3A%2F%2Fdiscord.com%2FRedirection&scope=rpc+rpc.voice.write";
        //private const string TokenEndpoint = "https://discord.com/api/oauth2/token";
        //private const string rpcEndPoint = "https://discord.com/api/v10/applications/1229701026307903488/";
        //private const string RedirectUri = "https://discord.com/Redirection";
        //private const string ClientId = "1229701026307903488";
        //private const string ClientSecret = "gojjNxWrcAmMfhmDpIFudPLhyLirdQFs";
        //private HttpClient httpClient = null;
        //private string token = "";

        //public DiscordWrapper()
        //{
        //    httpClient = new HttpClient();
        //}

        //public async void Connect(string _code)
        //{
        //    await GetAccessTokenAsync(_code);
        //}
        //public async Task<string> GetAccessTokenAsync(string code)
        //{
        //    Dictionary<string, string> tokenRequestParameters = new Dictionary<string, string>
        //    {
        //        { "client_id", ClientId },
        //        { "client_secret", ClientSecret },
        //        { "code", code},
        //        { "redirect_uri", RedirectUri },
        //        { "grant_type", "authorization_code" }
        //    };

        //    var tokenRequestContent = new FormUrlEncodedContent(tokenRequestParameters);
        //    var tokenResponse = await httpClient.PostAsync(TokenEndpoint, tokenRequestContent);

        //    if (!tokenResponse.IsSuccessStatusCode)
        //    {
        //        throw new Exception($"Failed to obtain access token: {tokenResponse.ReasonPhrase}");
        //    }

        //    string tokenResponseJson = await tokenResponse.Content.ReadAsStringAsync();
        //    string accessToken = JObject.Parse(tokenResponseJson)["access_token"].ToString();
        //    token = accessToken;

        //    return accessToken;
        //}

        //void OnRedirect(object _process , EventArgs e)
        //{
        //    //TODO check if possible to get the link here 
        //    Console.WriteLine("redirect");
        //    return;

        //}
        //public void AskPermission()
        //{
        //    Process.Start(authorizationLink);
        //}
        #endregion

    }
}
