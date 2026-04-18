using HKShop.DTOs;

namespace HKShop.Services;

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;

    public static ServiceResult Ok(string message = "Success") => new() { Success = true, Message = message };
    public static ServiceResult Fail(string message) => new() { Success = false, Message = message };
}

public class AdminProductsPageResult
{
    public List<HangHoaResponse> Products { get; set; } = new();
    public List<CategoryResponse> Categories { get; set; } = new();
    public int TotalPages { get; set; }
}

public class AdminClientsPageResult
{
    public List<ClientResponse> Clients { get; set; } = new();
    public List<int> Roles { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}

public class LoginResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public string? RedirectUrl { get; set; }
}

public class PaypalCaptureResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
}
