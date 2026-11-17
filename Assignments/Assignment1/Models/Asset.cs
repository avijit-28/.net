namespace InventoryApi.Models;

public sealed record Asset(int AssetId, string AssetName, string Category, DateTime PurchaseDate, string Status, decimal Price);

public sealed class AssetInput
{
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(150, MinimumLength = 1)]
    public string AssetName { get; init; } = "";

    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 1)]
    public string Category { get; init; } = "";

    [System.ComponentModel.DataAnnotations.Required]
    public DateTime? PurchaseDate { get; init; }

    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(30, MinimumLength = 1)]
    public string Status { get; init; } = "";

    [System.ComponentModel.DataAnnotations.Range(typeof(decimal), "0.00", "999999999.99")]
    public decimal Price { get; init; }
}

public sealed class LoginRequest
{
    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(100)]
    public string Username { get; init; } = "";

    [System.ComponentModel.DataAnnotations.Required, System.ComponentModel.DataAnnotations.StringLength(200)]
    public string Password { get; init; } = "";
}

public sealed record LoginResponse(string AccessToken, string TokenType, DateTime ExpiresAtUtc);
