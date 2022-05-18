using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Responses
{
    internal class LoginModelResponse
    {
        public LoginModelResponse(string id, string token)
        {
            Id = id;
            Token = token;
        }

        [JsonProperty("id")] internal string Id { get; set; }
        [JsonProperty("token")] internal string Token { get; set; }
    }
}
