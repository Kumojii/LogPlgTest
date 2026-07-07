using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using LogPlgTest.Dto;
using LogPlgTest.Models;

namespace LogPlgTest.Api
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private JsonSerializerOptions _jsonSerOpt = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public ApiClient()
        {
            _client = new HttpClient();
            // IP сервера
            _client.BaseAddress = new Uri("http://192.168.149.20:5261/");
        }

        public async Task SendLogAsync(string message)
        {
            try
            {
                var log = new
                {
                    message = message,
                    level = "Info",
                    source = "LogPlgTest",
                    timestamp = DateTime.Now
                };

                var json = JsonSerializer.Serialize(log);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("api/logs", content);

                if (!response.IsSuccessStatusCode)
                    MessageBox.Show("response.IsSuccessStatusCode = false");
            }
            catch (Exception ex)
            {
                // REVIEW: MessageBox ошибки в API ? Не лучше просто throw ?
                MessageBox.Show(ex.ToString());
            }
        }

        public async Task<EmployeeResult> VerifyEmployee(string machineName, string userName)
        {
            var response = await _client.GetAsync(
                $"api/employee/verify?machineName={machineName}&userName={userName}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            return JsonSerializer.Deserialize<EmployeeResult>(json, _jsonSerOpt);
        }

        public async Task<PluginResult> VerifyPlugin(string pluginName, string buttonName)
        {
            var response = await _client.GetAsync(
                $"api/plugin/verify?pluginName={pluginName}&buttonName={buttonName}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<PluginResult>(json, _jsonSerOpt);
        }

        public async Task<string> GetInstruction(string pluginName, string buttonName)
        {
            var response = await _client.GetAsync(
             $"api/plugin/instruction?pluginName={pluginName}&buttonName={buttonName}")
             .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var url = await response.Content.ReadAsStringAsync();

            return url;
        }

        public async Task<bool> HasAccessToThePlugin(string pluginName, string buttonName, string machineName, string userName)
        {
            var response = await _client.GetAsync(
                $"api/plugin/access?pluginName={pluginName}&buttonName={buttonName}&machineName={machineName}&userName={userName}")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<bool>(jsonResponse, _jsonSerOpt);
        }

        public async Task<PluginVersionResult> CheckPluginVersion(string pluginName, string buttonName, string version)
        {
            var response = await _client.GetAsync(
                $"api/plugin/version?pluginName={pluginName}&buttonName={buttonName}&version={version}")
                .ConfigureAwait(false); 

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<PluginVersionResult>(jsonResponse, _jsonSerOpt);
        }

        public async Task SendLog(LogCreateRequest request)
        {
            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(
                "api/logs/sendLog",
                content);

            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> Register(EmployeeRequest request)
        {
            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(
                "api/employee/register",
                content);

            response.EnsureSuccessStatusCode();

            return true;
        }

    }
}
