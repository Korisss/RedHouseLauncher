using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Responses
{
    internal class LoginResponse
    {
        public LoginResponse(long id, string token)
        {
            Id = id;
            Token = token;
        }

        [JsonProperty("id")] internal long Id { get; set; }
        [JsonProperty("token")] internal string Token { get; set; }
    }
}
