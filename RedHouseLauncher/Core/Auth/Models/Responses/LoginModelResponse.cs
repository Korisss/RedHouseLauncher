using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Responses
{
    internal class LoginModelResponse
    {
        [JsonProperty("id")] internal int Id { get; set; }
        [JsonProperty("token")] internal string Token { get; set; }
    }
}
