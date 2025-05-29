using NYC.MobileApp.Model;

namespace NYC.MobileApp.API;

public interface IApiService
{
    Task<LoginResponseModel?> LoginAsync(string username, string password);
    Task<T> GetAsync<T>(string endpoint);
    Task<T> PostAsync<T>(string endpoint, object data);
    Task<T> PutAsync<T>(string endpoint, object data);
    Task DeleteAsync(string endpoint);
}