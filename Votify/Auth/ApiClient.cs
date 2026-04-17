using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace Votify.Auth
{
    public class ApiClient
    {
        BlazorAuthStateProvider _stateProvider;
        HttpClient _httpClient;

        public ApiClient(BlazorAuthStateProvider stateProvider, HttpClient httpClient, NavigationManager nav)
        {
            _stateProvider = stateProvider;
            _httpClient = httpClient;
            _httpClient.BaseAddress ??= new Uri(nav.BaseUri);
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await _stateProvider.ObtenerToken();
            if (token != null)
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<HttpResponseMessage> GetAsync(string url)
        {
            await SetAuthHeaderAsync();
            return await _httpClient.GetAsync(url);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            await SetAuthHeaderAsync();
            return await _httpClient.PostAsync(url, content);
        }

        public async Task<HttpResponseMessage> PutAsync(string url, HttpContent content)
        {
            await SetAuthHeaderAsync();
            return await _httpClient.PutAsync(url, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            await SetAuthHeaderAsync();
            return await _httpClient.DeleteAsync(url);
        }

        public async Task<HttpResponseMessage> PostMultipartAsync(string url, MultipartFormDataContent dataContent)
        {
            await SetAuthHeaderAsync();
            return await _httpClient.PostAsync(url, dataContent);
        }

        public async Task<T?> GetFromJsonAsync<T>(string url)
        {
            var response = await GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string url, T body)
        {
            await SetAuthHeaderAsync();
            return await _httpClient.PostAsJsonAsync(url, body);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string url, T body)
        {
            await SetAuthHeaderAsync();
            return await _httpClient.PutAsJsonAsync(url, body);
        }

        public async Task<HttpResponseMessage> PostAnonymousAsync<T>(string url, T body)
    => await _httpClient.PostAsJsonAsync(url, body);
    }
}
