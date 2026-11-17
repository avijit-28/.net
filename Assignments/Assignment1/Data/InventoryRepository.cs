using System.Data;
using InventoryApi.Models;
using Microsoft.Data.SqlClient;

namespace InventoryApi.Data;

public sealed class InventoryRepository(IConfiguration configuration)
{
    private string ConnectionString => configuration.GetConnectionString("Inventory")!;

    private SqlConnection CreateConnection() => new(ConnectionString);

    public async Task<IReadOnlyList<Asset>> GetAllAssetsAsync(CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection();
        await using var command = new SqlCommand("sp_GetAllAssets", connection) { CommandType = CommandType.StoredProcedure };
        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var assets = new List<Asset>();
        while (await reader.ReadAsync(cancellationToken)) assets.Add(ReadAsset(reader));
        return assets;
    }

    public async Task<Asset?> GetAssetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection();
        await using var command = new SqlCommand("sp_GetAssetById", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.Add("@AssetId", SqlDbType.Int).Value = id;
        await connection.OpenAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ReadAsset(reader) : null;
    }

    public async Task<int> InsertAssetAsync(AssetInput asset, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection();
        await using var command = new SqlCommand("sp_InsertAsset", connection) { CommandType = CommandType.StoredProcedure };
        AddAssetParameters(command, asset);
        var id = new SqlParameter("@AssetId", SqlDbType.Int) { Direction = ParameterDirection.Output };
        command.Parameters.Add(id);
        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
        return (int)id.Value;
    }

    public async Task<bool> UpdateAssetAsync(int id, AssetInput asset, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection();
        await using var command = new SqlCommand("sp_UpdateAsset", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.Add("@AssetId", SqlDbType.Int).Value = id;
        AddAssetParameters(command, asset);
        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeactivateAssetAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection();
        await using var command = new SqlCommand("sp_DeleteAsset", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.Add("@AssetId", SqlDbType.Int).Value = id;
        await connection.OpenAsync(cancellationToken);
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<string?> GetPasswordHashAsync(string username, CancellationToken cancellationToken)
    {
        await using var connection = CreateConnection();
        await using var command = new SqlCommand("sp_GetUserByUsername", connection) { CommandType = CommandType.StoredProcedure };
        command.Parameters.Add("@Username", SqlDbType.NVarChar, 100).Value = username;
        await connection.OpenAsync(cancellationToken);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is null or DBNull ? null : (string)result;
    }

    private static void AddAssetParameters(SqlCommand command, AssetInput asset)
    {
        command.Parameters.Add("@AssetName", SqlDbType.NVarChar, 150).Value = asset.AssetName;
        command.Parameters.Add("@Category", SqlDbType.NVarChar, 100).Value = asset.Category;
        command.Parameters.Add("@PurchaseDate", SqlDbType.Date).Value = asset.PurchaseDate!.Value.Date;
        command.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = asset.Status;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = asset.Price;
        command.Parameters["@Price"].Precision = 11;
        command.Parameters["@Price"].Scale = 2;
    }

    private static Asset ReadAsset(SqlDataReader reader) => new(
        reader.GetInt32(reader.GetOrdinal("AssetId")),
        reader.GetString(reader.GetOrdinal("AssetName")),
        reader.GetString(reader.GetOrdinal("Category")),
        reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
        reader.GetString(reader.GetOrdinal("Status")),
        reader.GetDecimal(reader.GetOrdinal("Price")));
}
