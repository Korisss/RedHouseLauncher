using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Responses
{
    internal class RegisterModelResponse
    {
        [JsonProperty("id")] internal int Id { get; set; }
    }
}
