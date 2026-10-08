# ADO.NET Basics & Complete Syntax Reference

A technical cheat-sheet covering every core class in ADO.NET—including required NuGet packages, namespaces, constructor overloads, properties, methods, and exact C# syntax.

---

## 1. Required Packages & Namespaces

### NuGet Package (Modern .NET Core / .NET 6, 7, 8+)
> **Note:** `System.Data.SqlClient` is legacy/deprecated. Always use `Microsoft.Data.SqlClient` for modern .NET projects.

```bash
dotnet add package Microsoft.Data.SqlClient
```

### Namespaces to Include (`using` Directives)
```csharp
using System.Data;                  // Core interfaces, DataTable, DataSet, CommandType, ConnectionState
using Microsoft.Data.SqlClient;     // SqlConnection, SqlCommand, SqlDataReader, SqlDataAdapter, SqlParameter
```

---

## 2. `SqlConnectionStringBuilder`
**Usage:** Safely builds or parses SQL Server connection strings strongly-typed in C# instead of messy string concatenation.

### Constructors (Overloads)
```csharp
// 1. Default constructor (starts empty)
SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

// 2. Initialize with an existing connection string to modify/inspect it
SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder("Server=localhost;Database=MyDb;");
```

### Key Properties & Syntax
```csharp
SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
{
    DataSource = "localhost\\SQLEXPRESS", // Server name or IP
    InitialCatalog = "ShopDB",            // Database name
    IntegratedSecurity = true,            // true = Windows Auth, false = SQL Server Auth
    UserID = "sa",                        // Used if IntegratedSecurity = false
    Password = "StrongPassword123",       // Used if IntegratedSecurity = false
    TrustServerCertificate = true,        // Bypass SSL cert validation in local dev
    ConnectTimeout = 30,                  // Timeout in seconds (default is 15)
    Pooling = true,                       // Enables connection pooling (default is true)
    MinPoolSize = 5,                      // Minimum connections kept alive in pool
    MaxPoolSize = 100                     // Maximum connections allowed in pool
};

string connString = builder.ConnectionString; // Outputs the formatted connection string
```

---

## 3. `SqlConnection`
**Usage:** Manages the physical TCP/named-pipe network connection to SQL Server and handles connection pooling automatically when disposed.

### Constructors (Overloads)
```csharp
// 1. Default constructor (ConnectionString must be set later via property)
SqlConnection conn = new SqlConnection();
conn.ConnectionString = "Server=.;Database=MyDb;Trusted_Connection=True;TrustServerCertificate=True;";

// 2. With connection string (Most Common)
SqlConnection conn = new SqlConnection("Server=.;Database=MyDb;Trusted_Connection=True;TrustServerCertificate=True;");

// 3. With ConnectionString and SqlCredential (for secure password handling in memory)
SqlConnection conn = new SqlConnection("Server=.;Database=MyDb;", sqlCredential);
```

### Key Properties & Methods
| Member | Type | Usage |
| :--- | :--- | :--- |
| `conn.ConnectionString` | Property | Gets or sets the string used to open the database. |
| `conn.State` | Property | Returns `ConnectionState` enum (`Closed`, `Open`, `Connecting`, `Executing`, `Broken`). |
| `conn.Database` | Property | Gets the name of the current database. |
| `conn.Open()` / `conn.OpenAsync()` | Method | Opens a database connection (draws from connection pool if available). |
| `conn.Close()` / `conn.Dispose()` | Method | Closes the connection and **returns it to the connection pool**. |
| `conn.BeginTransaction()` | Method | Starts a database transaction and returns a `SqlTransaction` object. |

### Recommended Usage Syntax (Always wrap in `using`)
```csharp
using (SqlConnection conn = new SqlConnection(connString))
{
    await conn.OpenAsync();
    // Execute commands here...
} // Automatically calls Dispose() & closes connection even if an exception occurs
```

---

## 4. `SqlCommand`
**Usage:** Represents a T-SQL query (`SELECT`, `INSERT`, `UPDATE`, `DELETE`) or a **Stored Procedure** to execute against a `SqlConnection`.

### Constructors (Overloads)
```csharp
// 1. Default constructor (set properties manually afterward)
SqlCommand sqlCom = new SqlCommand();
sqlCom.CommandText = "SELECT * FROM Users";
sqlCom.Connection = conn;

// 2. With CommandText (SQL query or Stored Procedure name)
SqlCommand sqlCom = new SqlCommand("SELECT * FROM Users");

// 3. With CommandText and SqlConnection (Most Common)
SqlCommand sqlCom = new SqlCommand("SELECT * FROM Users", conn);

// 4. With CommandText, SqlConnection, and SqlTransaction (Used inside transactions)
SqlCommand sqlCom = new SqlCommand("UPDATE Accounts SET Balance = Balance - 100 WHERE Id = 1", conn, transaction);
```

### Key Properties
```csharp
sqlCom.CommandText = "sp_GetUsersByRole";        // SQL string or Stored Procedure name
sqlCom.Connection = conn;                        // Active SqlConnection
sqlCom.Transaction = transaction;                // Active SqlTransaction (if any)
sqlCom.CommandTimeout = 60;                      // Seconds to wait before throwing timeout error (Default: 30)

// CommandType Enum options:
sqlCom.CommandType = CommandType.Text;           // Default: Raw SQL query ("SELECT * FROM...")
sqlCom.CommandType = CommandType.StoredProcedure;// Tells SQL Server CommandText is a Stored Proc name
sqlCom.CommandType = CommandType.TableDirect;    // Rarely used: Name of a table
```

### Execution Methods (Crucial Distinction!)
| Method (Sync / Async) | Return Type | When to Use |
| :--- | :--- | :--- |
| `ExecuteNonQuery()` / `ExecuteNonQueryAsync()` | `int` | **INSERT, UPDATE, DELETE, CREATE, ALTER**. Returns the number of rows affected. |
| `ExecuteScalar()` / `ExecuteScalarAsync()` | `object` | Queries returning a **single value** (1st column of 1st row), e.g., `SELECT COUNT(*)`, `SELECT SCOPE_IDENTITY()`. |
| `ExecuteReader()` / `ExecuteReaderAsync()` | `SqlDataReader` | **SELECT** queries returning **multiple rows/columns** as a fast forward-only stream. |
| `ExecuteXmlReader()` | `XmlReader` | Queries using SQL `FOR XML` clause. |

### Syntax Examples for Execution Methods
```csharp
// A. ExecuteNonQuery (INSERT / UPDATE / DELETE)
using SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE IsInactive = 1", conn);
int rowsAffected = await cmd.ExecuteNonQueryAsync();

// B. ExecuteScalar (Single value)
using SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users", conn);
int totalUsers = Convert.ToInt32(await cmd.ExecuteScalarAsync());

// C. ExecuteReader (Multiple rows)
using SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM Users", conn);
using SqlDataReader reader = await cmd.ExecuteReaderAsync();
```

---

## 5. `SqlParameter`
**Usage:** Safely passes parameters into `SqlCommand` to **prevent SQL Injection** and handle `NULL` values, data types, and `OUTPUT` parameters from Stored Procedures.

### Constructors (Overloads)
```csharp
// 1. Default constructor
SqlParameter param = new SqlParameter();

// 2. Parameter name and value (Infers SqlDbType automatically)
SqlParameter param = new SqlParameter("@Username", "john_doe");

// 3. Parameter name and explicit SqlDbType
SqlParameter param = new SqlParameter("@Age", SqlDbType.Int);
param.Value = 25;

// 4. Parameter name, SqlDbType, and Size (Important for VARCHAR/NVARCHAR performance)
SqlParameter param = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
param.Value = "john@example.com";

// 5. Full overload (Name, Type, Size, Direction, IsNullable, Precision, Scale, SourceCol, DataRowVersion, Value)
SqlParameter param = new SqlParameter("@NewId", SqlDbType.Int, 4, ParameterDirection.Output, false, 0, 0, null, DataRowVersion.Current, null);
```

### Ways to Add Parameters to `SqlCommand`
```csharp
// Method 1: AddWithValue (Quickest, but watch out: strings always map to NVARCHAR(MAX) which can slow down indexes)
sqlCom.Parameters.AddWithValue("@Id", 42);

// Method 2: Add with Type and Value (Best Practice for production)
sqlCom.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = "Alice";

// Method 3: Passing a constructed SqlParameter object
SqlParameter p = new SqlParameter("@Salary", SqlDbType.Decimal) { Value = 85000.50m };
sqlCom.Parameters.Add(p);

// Method 4: Handling C# null -> SQL NULL (Crucial! Passing C# 'null' directly throws an error)
string? middleName = null;
sqlCom.Parameters.AddWithValue("@MiddleName", (object?)middleName ?? DBNull.Value);

// Method 5: Output Parameter (For Stored Procedures that return OUTPUT values)
SqlParameter outputParam = new SqlParameter("@OutTotalCount", SqlDbType.Int)
{
    Direction = ParameterDirection.Output // Options: Input, Output, InputOutput, ReturnValue
};
sqlCom.Parameters.Add(outputParam);

await sqlCom.ExecuteNonQueryAsync();
int count = (int)outputParam.Value; // Read value AFTER execution
```

---

## 6. `SqlDataReader`
**Usage:** Connected, forward-only, read-only cursor that streams rows from SQL Server one at a time. **Fastest and most memory-efficient** way to read data in ADO.NET.

### Instantiation
> `SqlDataReader` has **NO public constructor**. You can only instantiate it by calling `sqlCom.ExecuteReader()`.

```csharp
// 1. Standard reader
SqlDataReader reader = sqlCom.ExecuteReader();

// 2. With CommandBehavior (e.g., closes connection automatically when reader is closed)
SqlDataReader reader = sqlCom.ExecuteReader(CommandBehavior.CloseConnection);
// Other CommandBehavior flags: SingleRow, SingleResult, SequentialAccess, SchemaOnly
```

### Key Properties & Methods
```csharp
bool hasRows = reader.HasRows;          // True if query returned at least 1 row
int fieldCount = reader.FieldCount;     // Number of columns in the current row
bool isClosed = reader.IsClosed;        // True if reader is closed

while (await reader.ReadAsync())        // Advances cursor to the next row; returns false at the end
{
    // 1. Reading by Column Name (Indexer) - Returns 'object', needs casting
    int id = Convert.ToInt32(reader["Id"]);
    string name = reader["Name"].ToString()!;

    // 2. Reading by Strongly-Typed Getters + Column Ordinal (Fastest performance)
    int colIndex = reader.GetOrdinal("Email"); // Finds column index by name
    
    // Always check for SQL NULL before reading nullable columns!
    string? email = await reader.IsDBNullAsync(colIndex) 
        ? null 
        : reader.GetString(colIndex);   // Other getters: GetInt32(), GetBoolean(), GetDecimal(), GetDateTime(), GetGuid()
}

// If your query or Stored Procedure has MULTIPLE SELECT statements (e.g., "SELECT * FROM Users; SELECT * FROM Roles;")
if (await reader.NextResultAsync())
{
    while (await reader.ReadAsync())
    {
        // Read second result set here
    }
}
```

---

## 7. Disconnected Architecture: `SqlDataAdapter`, `DataTable`, & `DataSet`
**Usage:** Fetches all data at once into in-memory tables (`DataTable` / `DataSet`) and immediately closes the connection. Heavily used in **WinForms/WPF DataGrids** and legacy reporting.
* **`DataTable`**: Represents a single in-memory table (Rows & Columns).
* **`DataSet`**: Represents an in-memory collection of multiple `DataTable`s and their `DataRelation`s.
* **`SqlDataAdapter`**: Acts as the bridge between the database and `DataTable`/`DataSet`. Opens and closes the `SqlConnection` automatically during `.Fill()`.

### `SqlDataAdapter` Constructors (Overloads)
```csharp
// 1. Default constructor
SqlDataAdapter adapter = new SqlDataAdapter();
adapter.SelectCommand = sqlCom;

// 2. With an existing SqlCommand
SqlDataAdapter adapter = new SqlDataAdapter(sqlCom);

// 3. With SQL query string and existing SqlConnection
SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Products", conn);

// 4. With SQL query string and connection string (Creates connection internally)
SqlDataAdapter adapter = new SqlDataAdapter("SELECT * FROM Products", connString);
```

### `DataTable` & `DataSet` Constructors (Overloads)
```csharp
// DataTable Constructors
DataTable dt = new DataTable();                  // Default unnamed table
DataTable dt = new DataTable("ProductsTable");   // Named table

// DataSet Constructors
DataSet ds = new DataSet();                      // Default unnamed dataset
DataSet ds = new DataSet("InventoryDataSet");    // Named dataset
```

### Usage Syntax (Filling & Reading a `DataTable`)
```csharp
using SqlConnection conn = new SqlConnection(connString);
using SqlCommand cmd = new SqlCommand("SELECT Id, Name, Price FROM Products WHERE CategoryId = @CatId", conn);
cmd.Parameters.AddWithValue("@CatId", 5);

using SqlDataAdapter adapter = new SqlDataAdapter(cmd);
DataTable dt = new DataTable();

// Fill() automatically opens 'conn' (if closed), fetches all rows into 'dt', and closes 'conn'!
adapter.Fill(dt);

// Iterating over a DataTable in memory (Connection is already closed here)
foreach (DataRow row in dt.Rows)
{
    int id = Convert.ToInt32(row["Id"]);
    string name = row["Name"].ToString()!;
    decimal price = row.Field<decimal>("Price"); // Strongly-typed LINQ-to-DataSet extension method
}
```

---

## 8. `SqlTransaction`
**Usage:** Groups multiple database commands into a single **atomic unit of work** (ACID). Either all commands succeed (`Commit`) or all changes are undone (`Rollback`).

### Instantiation
> `SqlTransaction` has **NO public constructor**. It is created via `conn.BeginTransaction()` **after** the connection is opened.

```csharp
// 1. Default IsolationLevel (ReadCommitted)
SqlTransaction transaction = conn.BeginTransaction();

// 2. With explicit IsolationLevel (ReadUncommitted, ReadCommitted, RepeatableRead, Serializable, Snapshot)
SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.RepeatableRead);

// 3. With Transaction Name / IsolationLevel
SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.ReadCommitted, "TransferFundsTx");
```

### Complete Transaction Syntax
```csharp
using SqlConnection conn = new SqlConnection(connString);
await conn.OpenAsync();

// Start transaction
using SqlTransaction transaction = conn.BeginTransaction();

try
{
    // Both commands MUST have the transaction assigned to them (via constructor or .Transaction property)
    using SqlCommand debitCmd = new SqlCommand(
        "UPDATE Accounts SET Balance = Balance - @Amount WHERE AccountId = @FromId", conn, transaction);
    debitCmd.Parameters.AddWithValue("@Amount", 500m);
    debitCmd.Parameters.AddWithValue("@FromId", 1);
    await debitCmd.ExecuteNonQueryAsync();

    using SqlCommand creditCmd = new SqlCommand(
        "UPDATE Accounts SET Balance = Balance + @Amount WHERE AccountId = @ToId", conn, transaction);
    creditCmd.Parameters.AddWithValue("@Amount", 500m);
    creditCmd.Parameters.AddWithValue("@ToId", 2);
    await creditCmd.ExecuteNonQueryAsync();

    // If both succeed, commit changes permanently to DB
    await transaction.CommitAsync();
}
catch (SqlException ex)
{
    // If anything fails, revert all changes made in this transaction
    await transaction.RollbackAsync();
    throw;
}
```

---

## 9. `SqlBulkCopy`
**Usage:** High-performance bulk insertion of thousands or millions of rows from a `DataTable` or `IDataReader` directly into a SQL Server table (orders of magnitude faster than looping `INSERT` statements).

### Constructors (Overloads)
```csharp
// 1. With an open SqlConnection
SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);

// 2. With connection string (Opens/manages its own connection)
SqlBulkCopy bulkCopy = new SqlBulkCopy(connString);

// 3. With SqlConnection, SqlBulkCopyOptions (e.g., KeepIdentity, FireTriggers, TableLock), and SqlTransaction
SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock, transaction);
```

### Usage Syntax
```csharp
using SqlBulkCopy bulkCopy = new SqlBulkCopy(conn)
{
    DestinationTableName = "dbo.ArchivedLogs",
    BatchSize = 5000,          // Rows sent per batch to SQL Server
    BulkCopyTimeout = 120      // Timeout in seconds
};

// Map source DataTable column names -> Destination SQL Table column names (if names differ)
bulkCopy.ColumnMappings.Add("LogId", "Id");
bulkCopy.ColumnMappings.Add("LogMessage", "Message");

await bulkCopy.WriteToServerAsync(dataTable);
```

---

## 10. `SqlException` (Database Error Handling)
**Usage:** Caught in `try-catch` blocks whenever SQL Server returns an error (constraint violation, deadlock, timeout, syntax error, login failure).

### Key Properties & Error Numbers
```csharp
try
{
    // ADO.NET Database code...
}
catch (SqlException ex)
{
    Console.WriteLine($"SQL Error Number: {ex.Number}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Procedure: {ex.Procedure}, Line: {ex.LineNumber}");

    // Common SQL Server Error Numbers to handle in backend APIs:
    switch (ex.Number)
    {
        case -2:   // Command Timeout expired
            break;
        case 1205: // Deadlock victim (Safe to retry transaction)
            break;
        case 2601: // Unique Index violation (Duplicate key)
        case 2627: // Unique Constraint / Primary Key violation (Duplicate record)
            break;
        case 547:  // Foreign Key constraint violation (e.g., deleting parent with child records)
            break;
        case 18456:// Login failed for user
            break;
    }
}
```
