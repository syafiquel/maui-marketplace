using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using NYC.MobileApp.Model;
using NYC.MobileApp.Views;

namespace NYC.MobileApp.API;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://dev.newyorkerconcept.com/api/v1/");
        //_httpClient.BaseAddress = new Uri("https://777c-2404-160-8322-87e3-5536-5124-e482-1e03.ngrok-free.app/api/v1/");
    }

    public async Task<LoginResponseModel?> LoginAsync(string email, string password)
    {
        var loginData = new { email = email, Password = password };
        var json = JsonSerializer.Serialize(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("auth/login", content); // Replace with your login endpoint
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponseModel>(responseJson);

        if (!string.IsNullOrWhiteSpace(result?.AccessToken))
        {
            Preferences.Set("BearerToken", result.AccessToken);
            Preferences.Set("TokenExpiresAt", DateTime.UtcNow.AddSeconds(result.ExpiresIn).ToString("o"));
            Preferences.Set("RefreshToke ", result.RefreshToken);
            Preferences.Set("UserEmail", result.UserEmail);
            Preferences.Set("UserFullName", result.UserFullName);
        }

        return result;
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        AddAuthorizationHeader();

        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool isRefreshed = await RefreshTokenAsync();

                if (isRefreshed)
                {
                    // Retry the request after refreshing the token
                    AddAuthorizationHeader();
                    response = await _httpClient.GetAsync(endpoint);
                }
            }

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        AddAuthorizationHeader();

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            bool isRefreshed = await RefreshTokenAsync();

            if (isRefreshed)
            {
                // Retry the request after refreshing the token
                AddAuthorizationHeader();
                response = await _httpClient.PostAsync(endpoint, content);
            }
        }

        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<T> PutAsync<T>(string endpoint, object data)
    {
        AddAuthorizationHeader();

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            bool isRefreshed = await RefreshTokenAsync();

            if (isRefreshed)
            {
                // Retry the request after refreshing the token
                AddAuthorizationHeader();
                response = await _httpClient.PutAsync(endpoint, content);
            }
        }

        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task DeleteAsync(string endpoint)
    {
        AddAuthorizationHeader();

        var response = await _httpClient.DeleteAsync(endpoint);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            bool isRefreshed = await RefreshTokenAsync();

            if (isRefreshed)
            {
                // Retry the request after refreshing the token
                AddAuthorizationHeader();
                response = await _httpClient.DeleteAsync(endpoint);
            }
        }

        response.EnsureSuccessStatusCode();
    }

    private void AddAuthorizationHeader()
    {
        var bearerToken = Preferences.Get("BearerToken", null);
        if (!string.IsNullOrEmpty(bearerToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
    }

    private async Task<bool> RefreshTokenAsync()
    {
        string refreshToken = Preferences.Get("RefreshToken", "");

        if (string.IsNullOrEmpty(refreshToken))
        {
            // No refresh token available, logout user
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
                // Save new tokens
                Preferences.Set("BearerToken", newTokenResponse.AccessToken);
                Preferences.Set("TokenExpiresAt", DateTime.UtcNow.AddSeconds(newTokenResponse.ExpiresIn).ToString("o"));

                if (!string.IsNullOrEmpty(newTokenResponse.RefreshToken))
                {
                    Preferences.Set("RefreshToken", newTokenResponse.RefreshToken);
                }

                return true;
            }
        }

        // Refresh failed, logout user
        await LogoutUserAsync();
        return false;
    }

    public async Task LogoutUserAsync()
    {

        try
        {
            AddAuthorizationHeader();
            Preferences.Remove("BearerToken");
            Preferences.Remove("RefreshToken");
            Preferences.Remove("TokenExpiresAt");
            await _httpClient.PostAsync("auth/logout", null);

        }
        catch (Exception ex)
        {
            throw new Exception($"Logout failed {ex.Message}", ex);
        }



        Application.Current.MainPage = new LoginView();
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword, string confirmedPassword)
    {
        try
        {
            var changePasswordModel = new
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmedPassword = confirmedPassword
            };

            AddAuthorizationHeader();

            var json = JsonSerializer.Serialize(changePasswordModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("auth/change-password", content);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                bool isRefreshed = await RefreshTokenAsync();
                if (isRefreshed)
                {
                    AddAuthorizationHeader();
                    response = await _httpClient.PutAsync("auth/change-password", content);
                }
            }

            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Password change faileded {ex.Message}", ex);
        }
    }
    

}