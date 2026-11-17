# Inventory Tracking Web API

ASP.NET Core .NET 10 API for inventory CRUD and JWT login. All database access uses ADO.NET stored procedures; no ORM is used.

## Setup

1. Run `Database/setup.sql` against SQL Server LocalDB (for example, using `sqlcmd -S "(localdb)\MSSQLLocalDB" -i Database/setup.sql`).
2. Configure the local connection string and JWT signing key with .NET User Secrets:

   ```powershell
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:Inventory" "Server=(localdb)\MSSQLLocalDB;Database=InventoryDb;Integrated Security=true;TrustServerCertificate=true"
   dotnet user-secrets set "Jwt:SigningKey" "<generate a random secret of at least 32 bytes>"
   ```

   Environment variables `ConnectionStrings__Inventory` and `Jwt__SigningKey` can be used instead. Do not commit real keys or database credentials.
3. Generate a hash for a user password and insert it into the database:

   ```powershell
   dotnet run -- hash-password "your-password"
   ```

   Then run `INSERT dbo.ApiUsers (Username, PasswordHash) VALUES (N'your-user', N'<generated-hash>');` in `InventoryDb`.
4. Run `dotnet run`. The API listens at the URL printed by ASP.NET Core.
5. Import `Postman/InventoryApi.postman_collection.json`, set `baseUrl` to the local API URL, then call Login. The collection saves the returned token for the asset requests.

The login endpoint is public. All `/api/assets` endpoints require a valid bearer token. Delete changes the asset status to `Inactive`; the record remains available through list and get operations.
