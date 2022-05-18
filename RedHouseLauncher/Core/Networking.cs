using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
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

        private static async Task<string?> RequestAsync(HttpClient httpClient, string url, string method = "GET", string? data = null, bool auth = false)
        {
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            httpClient.BaseAddress = new Uri(url);

            if (auth)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Settings.Settings.UserToken);
            }

            switch (method)
            {
                case "GET":
                    {
                        using HttpResponseMessage response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        await using Stream responseStream = await response.Content.ReadAsStreamAsync();

                        using StreamReader streamReader = new(responseStream);
                        string result = await streamReader.ReadToEndAsync();

                        return result;
                    }
                case "POST":
                    {
                        data ??= "";

                        using HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                        using HttpResponseMessage response = await httpClient.PostAsync(url, content);
                        response.EnsureSuccessStatusCode();

                        await using Stream responseStream = await response.Content.ReadAsStreamAsync();

                        using StreamReader streamReader = new(responseStream);
                        string result = await streamReader.ReadToEndAsync();

                        return result;
                    }
                default:
                    return null;
            }
        }
    }
}
