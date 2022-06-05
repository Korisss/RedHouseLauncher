using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Requests
{
    internal class LoginRequest
    {
        internal LoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
        [JsonProperty("email")] internal string Email { get; private set; }
        [JsonProperty("password")] internal string Password { get; private set; }
    }
}
