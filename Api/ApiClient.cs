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
            _client.BaseAddress = new Uri("http://192.168.149.20:5261/");
            _client.Timeout = TimeSpan.FromSeconds(5);
        }

        private async Task<ApiResult<T>> GetAsync<T>(string url)
        {
            try
            {
                var response = await _client.GetAsync(url).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResult<T>
                    {
                        Success = false,
                        Error = $"HTTP {(int)response.StatusCode}"
                    };
                }

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var data = JsonSerializer.Deserialize<T>(json, _jsonSerOpt);

                if (data == null)
                {
                    return new ApiResult<T>
                    {
                        Success = false,
                        Error = "Сервер вернул пустой ответ."
                    };
                }

                return new ApiResult<T>
                {
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResult<T>
                {
                    Success = false,
                    Error = ex.Message,
                    Exception = ex
                };
            }
        }

        private async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);

                using (var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"))
                {


                    var response = await _client.PostAsync(url, content)
                        .ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        return new ApiResult<TResponse>
                        {
                            Success = false,
                            Error = $"HTTP {(int)response.StatusCode}"
                        };
                    }

                    var responseJson = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    if (string.IsNullOrWhiteSpace(responseJson))
                    {
                        return new ApiResult<TResponse>
                        {
                            Success = false,
                            Error = "Пустой ответ сервера."
                        };
                    }

                    var data = JsonSerializer.Deserialize<TResponse>(
                        responseJson,
                        _jsonSerOpt);

                    if (data == null)
                    {
                        return new ApiResult<TResponse>
                        {
                            Success = false,
                            Error = "Не удалось десериализовать ответ."
                        };
                    }

                    return new ApiResult<TResponse>
                    {
                        Success = true,
                        Data = data
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResult<TResponse>
                {
                    Success = false,
                    Error = ex.Message,
                    Exception = ex
                };
            }
        }

        private async Task<ApiResult> PostAsync<TRequest>(string url, TRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request);

                using (var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"))
                {

                    var response = await _client.PostAsync(url, content)
                        .ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        return new ApiResult
                        {
                            Success = false,
                            Error = $"HTTP {(int)response.StatusCode}"
                        };
                    }

                    return new ApiResult
                    {
                        Success = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResult
                {
                    Success = false,
                    Error = ex.Message,
                    Exception = ex
                };
            }
        }

        public Task<ApiResult<EmployeeResult>> VerifyEmployee(string machineName, string userName)
        {
            return GetAsync<EmployeeResult>(
                $"api/employee/verify?machineName={Uri.EscapeDataString(machineName)}&userName={Uri.EscapeDataString(userName)}");

        }

        public Task<ApiResult<PluginResult>> VerifyPlugin(string pluginName, string buttonName)
        {
            return GetAsync<PluginResult>(
                $"api/plugin/verify?pluginName={Uri.EscapeDataString(pluginName)}&buttonName={Uri.EscapeDataString(buttonName)}");
        }

        public Task<ApiResult<string>> GetInstruction(string pluginName, string buttonName)
        {
            return GetAsync<string>(
                $"api/plugin/instruction?pluginName={Uri.EscapeDataString(pluginName)}&buttonName={Uri.EscapeDataString(buttonName)}");
        }

        public Task<ApiResult<bool>> HasAccessToThePlugin(string pluginName, string buttonName, string machineName, string userName)
        {
            return GetAsync<bool>($"api/plugin/access?" +
                $"pluginName={Uri.EscapeDataString(pluginName)}" +
                $"&buttonName={Uri.EscapeDataString(buttonName)}" +
                $"&machineName={Uri.EscapeDataString(machineName)}" +
                $"&userName={Uri.EscapeDataString(userName)}");

        }

        public Task<ApiResult<PluginVersionResult>> CheckPluginVersion(string pluginName, string buttonName, string version)
        {
            return GetAsync<PluginVersionResult>($"api/plugin/version?" +
                $"pluginName={Uri.EscapeDataString(pluginName)}" +
                $"&buttonName={Uri.EscapeDataString(buttonName)}" +
                $"&version={Uri.EscapeDataString(version)}");
        }

        public Task<ApiResult> SendLog(LogCreateRequest request)
        {
            return PostAsync("api/logs/sendLog", request);
        }

        public Task<ApiResult> Register(EmployeeRequest request)
        {
            return PostAsync("api/employee/register", request);
        }

        public Task<ApiResult> Update(EmployeeRequest request)
        {
            return PostAsync("api/employee/update", request);
        }
    }
}
