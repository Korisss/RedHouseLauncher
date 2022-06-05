using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Responses
{
    internal class RegisterResponse
    {
        [JsonProperty("id")] internal long Id { get; set; }
    }
}
