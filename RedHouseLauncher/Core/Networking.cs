using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core
{
    internal static class Networking
    {
        internal static async Task<string?> RequestAsync(string url, string method = "GET", string? data = null, bool auth = false)
        {
            using HttpClient httpClient = new();
            return await RequestAsync(httpClient, url, method, data, auth);
        }

        internal static async Task<string?> RequestAsync(HttpClient httpClient, string url, string method = "GET", string? data = null, bool auth = false)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            httpClient.BaseAddress = new Uri(url);

            if (auth)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", Settings.Settings.UserToken);
            }

            if (method == "GET")
            {
                using HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using Stream responseStream = await response.Content.ReadAsStreamAsync();

                if (responseStream == null)
                {
                    return null;
                }

                using StreamReader streamReader = new(responseStream);
                string result = await streamReader.ReadToEndAsync();

                return result;
            }
            else if (method == "POST")
            {
                if (data == null)
                {
                    data = "";
                }

                using HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                using HttpResponseMessage response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                using Stream responseStream = await response.Content.ReadAsStreamAsync();

                if (responseStream == null)
                {
                    return null;
                }

                using StreamReader streamReader = new(responseStream);
                string result = await streamReader.ReadToEndAsync();

                return result;
            }

            return null;
        }
    }
}
