using System.Text.Json.Serialization;

namespace NYC.MobileApp.Model;

public class LoginResponseModel
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; }

    [JsonPropertyName("expiresIn")]
    public double ExpiresIn { get; set; }
    
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}
