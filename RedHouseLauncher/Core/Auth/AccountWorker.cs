using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedHouseLauncher.Core.Auth.Models.Requests;
using RedHouseLauncher.Core.Auth.Models.Responses;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core.Auth
{
    internal static class AccountWorker
    {
        private static readonly string ApiUri = Settings.Settings.MasterServer ?? Config.DefaultMaster;

        internal static async Task<RegisterModelResponse?> Register(RegisterModelRequest registerModel)
        {
            string? response = await Networking.RequestAsync($"{ApiUri}users", "POST", JsonConvert.SerializeObject(registerModel));

            return response == null ? null : JsonConvert.DeserializeObject<RegisterModelResponse>(response);
        }

        internal static async Task<LoginModelResponse?> Login(LoginModelRequest model)
        {
            string? response = await Networking.RequestAsync($"{ApiUri}users/login", "POST", JsonConvert.SerializeObject(model));

            return response == null ? null : JsonConvert.DeserializeObject<LoginModelResponse>(response);
        }

        internal static async Task<string?> GetLogin()
        {
            string? response = await Networking.RequestAsync($"{ApiUri}users/{Settings.Settings.UserId}", "GET", null, true);

            if (response == null)
            {
                return null;
            }

            JObject jObject = JObject.Parse(response);
            return jObject["name"]?.ToString();
        }

        internal static async Task<object?> GetSession(string address)
        {
            string? response = await Networking.RequestAsync($"{ApiUri}users/{Settings.Settings.UserId}/play/{address}", "POST", null, true);

            return response == null ? null : JsonConvert.DeserializeObject(response);
        }

        internal static async Task ResetPassword(ResetPasswordModel resetPasswordModelRequest)
        {
            await Networking.RequestAsync($"{ApiUri}users/reset-password", "POST", JsonConvert.SerializeObject(resetPasswordModelRequest));
        }

        // TODO: Add models
        /*
        internal static async Task<JustModelResponse> Verify(JustModelRequest model)
        {
            string raw = await Networking.Request($"{ApiUri}{model.Id}/users", "POST", true,
                JsonConvert.SerializeObject(model));
            return JsonConvert.DeserializeObject<JustModelResponse>(raw);
        }

        internal static Task VerifyToken()
        {
            return Networking.Request($"{ApiUri}secure", "GET", true);
        }

        internal class JustModelRequest
        {
            internal int Id { get; set; }
        }

        internal class JustModelResponse
        {
        }
        */
    }
}
