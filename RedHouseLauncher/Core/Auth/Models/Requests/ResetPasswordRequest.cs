using Newtonsoft.Json;

namespace RedHouseLauncher.Core.Auth.Models.Requests
{
    internal class ResetPasswordModel
    {
        internal ResetPasswordModel(string email)
        {
            Email = email;
        }
        [JsonProperty("email")] internal string Email { get; set; }
    }
}
