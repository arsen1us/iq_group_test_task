using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.Net.Http.Headers;
using IQGROUP_test_task;
using System.Net.Http;
using Microsoft.AspNetCore.Components;

namespace IQGROUP_test_task
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        NavigationManager _navigationManager;
        HttpClient _httpClient;
        ILocalStorageService _localStorage;
        public JwtAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }
        // Войти в систему

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsStringAsync("jwt-token");
                ClaimsIdentity identity = new ClaimsIdentity();
                _httpClient.DefaultRequestHeaders.Authorization = null;

                if (!string.IsNullOrEmpty(token))
                {
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
                }
                var user = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));

                return new AuthenticationState(user);
            }
            catch(Exception ex)
            {
                // log
                _httpClient.DefaultRequestHeaders.Authorization = null;

                await _localStorage.RemoveItemAsync("jwt-token");

                var identity = new ClaimsIdentity();
                var user = new ClaimsPrincipal(identity);

                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));

                return new AuthenticationState(user);
            }
        }

        // Получение claims из payload jwt-токена

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];

            var bytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(bytes);

            return keyValuePairs.Select(k => new Claim(k.Key, k.Value.ToString()));
        }
        // Декодирование строки payload

        public static byte[] ParseBase64WithoutPadding(string payload)
        {
            switch (payload.Length % 4)
            {
                // Добавляю для компенсации 2 знака '=', если остаток 2
                case 2:
                    payload += "==";
                    break;
                // Добавляю для компенсации знак '=', если остаток 3
                case 3:
                    payload += "=";
                    break;
            }
            return Convert.FromBase64String(payload);
        }
        // Метод для выхода из аккаунта

        public async Task LogoutAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            await _localStorage.RemoveItemAsync("jwt-token");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

    }

}