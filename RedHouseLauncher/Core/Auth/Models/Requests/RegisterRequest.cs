using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Requests
{
    internal class RegisterRequest
    {
        internal RegisterRequest(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }
        [JsonProperty("name")] internal string Name { get; private set; }
        [JsonProperty("email")] internal string Email { get; private set; }
        [JsonProperty("password")] internal string Password { get; private set; }
    }
}
