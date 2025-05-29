using System.Collections.Generic;

namespace NYC.MobileApp.API;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }  // e.g. "Success", "Invalid credentials", etc.
    public T Data { get; set; }
    public List<string> Errors { get; set; }  // Optional: useful for validation or field errors
}
