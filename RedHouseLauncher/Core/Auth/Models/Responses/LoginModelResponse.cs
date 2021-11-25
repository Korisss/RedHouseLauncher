using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Responses
{
    internal class LoginModelResponse
    {
        internal LoginModelResponse(int id, string token)
        {
            Id = id;
            Token = token;
        }
        [JsonProperty("id")] internal int Id { get; set; }
        [JsonProperty("token")] internal string Token { get; set; }
    }
}
