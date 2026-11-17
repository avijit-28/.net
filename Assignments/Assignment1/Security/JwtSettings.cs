namespace InventoryApi.Security;

public sealed class JwtSettings
{
    public string Issuer { get; init; } = "InventoryApi";
    public string Audience { get; init; } = "InventoryApiClients";
    public string SigningKey { get; init; } = "";
    public int ExpiresMinutes { get; init; } = 60;
}
