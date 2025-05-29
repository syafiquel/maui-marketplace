using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using NYC.MobileApp.Model;
using NYC.MobileApp.Views;

namespace NYC.MobileApp.API;

public class AuthHttpMessageHandler: DelegatingHandler
{
    private readonly HttpClient _httpClient;

    public AuthHttpMessageHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Attach Bearer Token
        string token = Preferences.Get("BearerToken", "");
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            bool isRefreshed = await RefreshTokenAsync();

            if (isRefreshed)
            {
                // Retry the request with the new token
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Preferences.Get("BearerToken", ""));
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }

    private async Task<bool> RefreshTokenAsync()
    {
        string refreshToken = Preferences.Get("RefreshToken", "");

        if (string.IsNullOrEmpty(refreshToken))
        {
            await LogoutUserAsync();
            return false;
        }

        var refreshRequest = new HttpRequestMessage(HttpMethod.Post, "https://yourapi.com/token/refresh")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" }
            })
        };

        HttpResponseMessage response = await _httpClient.SendAsync(refreshRequest);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var newTokenResponse = JsonSerializer.Deserialize<LoginResponseModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (newTokenResponse != null && !string.IsNullOrEmpty(newTokenResponse.AccessToken))
            {
                Preferences.Set("BearerToken", newTokenResponse.AccessToken);
                Preferences.Set("TokenExpiresAt", DateTime.UtcNow.AddSeconds(newTokenResponse.ExpiresIn).ToString("o"));

                if (!string.IsNullOrEmpty(newTokenResponse.RefreshToken))
                {
                    Preferences.Set("RefreshToken", newTokenResponse.RefreshToken);
                }

                return true;
            }
        }

        await LogoutUserAsync();
        return false;
    }

    private async Task LogoutUserAsync()
    {
        Preferences.Remove("BearerToken");
        Preferences.Remove("RefreshToken");
        Preferences.Remove("TokenExpiresAt");

        Application.Current.MainPage = new LoginView();
    }
}