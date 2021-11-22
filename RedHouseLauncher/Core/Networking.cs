using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RedHouseLauncher.Core
{
    internal static class Networking
    {
        // Было в старой версии, чекнуть (После проверки auth)
        // if (data == null && method == "POST" || data != null)
        //     using (StreamWriter streamWriter = new StreamWriter(httpClient.GetStreamAsync(url).Result))
        //     {
        //         streamWriter.Write($"{data ?? ""}");
        //     }

        internal static string? Request(string url, string method = "GET", string? data = null, bool auth = false)
        {
            HttpClient httpClient = new();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            httpClient.BaseAddress = new Uri(url);

            if (auth)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", Settings.Settings.UserToken);
            }

            if (method == "GET")
            {
                using HttpResponseMessage response = httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                using Stream responseStream = response.Content.ReadAsStream();

                if (responseStream == null)
                {
                    return null;
                }

                using StreamReader streamReader = new(responseStream);
                string result = streamReader.ReadToEnd();

                return result;
            }
            else if (method == "POST")
            {
                if (data == null)
                {
                    return null;
                }

                using HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");

                using HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
                response.EnsureSuccessStatusCode();

                using Stream responseStream = response.Content.ReadAsStream();

                if (responseStream == null)
                {
                    return null;
                }

                using StreamReader streamReader = new(responseStream);
                string result = streamReader.ReadToEnd();

                return result;

            }

            return null;
        }

        internal static async Task<string?> RequestAsync(string url, string method = "GET", string? data = null, bool auth = false)
        {
            HttpClient httpClient = new();
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
                    return null;
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
