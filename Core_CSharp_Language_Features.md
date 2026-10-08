# Core C# Language Features (Used Heavily in .NET)

A complete technical reference covering every core C# language feature from Section 2 of the roadmap—including required namespaces, constructor overloads, collection interfaces, delegates/events, LINQ (`IEnumerable` vs `IQueryable`), exception handling (`IDisposable`), extension methods, nullable reference types, and attributes/reflection.

---

## 1. Required Namespaces & Packages
All core language features are built into the .NET Base Class Library (BCL).

```csharp
using System;
using System.Collections.Generic;          // List<T>, Dictionary<TKey, TValue>, HashSet<T>, IEnumerable<T>
using System.Linq;                         // LINQ extension methods (Where, Select, GroupBy, FirstOrDefault)
using System.Linq.Expressions;             // Expression<Func<T, bool>> used by IQueryable / EF Core
using System.Reflection;                   // PropertyInfo, MethodInfo, Attribute inspection at runtime
using System.ComponentModel.DataAnnotations; // Built-in validation attributes ([Required], [MaxLength], [Range])
```

---

## 2. Generics (`<T>` & `where` Type Constraints)
**Usage:** Allows you to write classes, interfaces, and methods with a placeholder for the data type (`T`), providing compile-time type safety and avoiding boxing/casting overhead. Heavily used in API Response wrappers (`ApiResponse<T>`) and Generic Repositories (`IRepository<T>`).

### Complete `where` Type Constraints Table
| Constraint | Meaning |
| :--- | :--- |
| `where T : class` | `T` must be a **reference type** (class, interface, delegate, or array). |
| `where T : struct` | `T` must be a non-nullable **value type** (`int`, `bool`, `DateTime`, custom `struct`). |
| `where T : new()` | `T` must have a **public parameterless constructor** (allows calling `new T()` inside the generic code; must be listed last). |
| `where T : BaseClassName` | `T` must inherit from `BaseClassName` (or be that class). |
| `where T : IInterfaceName` | `T` must implement `IInterfaceName`. |
| `where T : notnull` | `T` cannot be a nullable type. |

### Generic Class, Constructors & Generic Method Syntax
```csharp
public interface IEntity
{
    int Id { get; set; }
}

// Generic Class with multiple constraints: T must be a class, implement IEntity, and have a default constructor
public class ApiResponse<T> where T : class, IEntity, new()
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    // 1. Default Constructor (Initializes Data using 'new T()' thanks to the 'new()' constraint)
    public ApiResponse()
    {
        IsSuccess = true;
        Message = "Success";
        Data = new T();
    }

    // 2. Overloaded Constructor #1 (Data payload only)
    public ApiResponse(T data) : this(true, "Success", data)
    {
    }

    // 3. Overloaded Constructor #2 (Full control)
    public ApiResponse(bool isSuccess, string message, T? data = null)
    {
        IsSuccess = isSuccess;
        Message = message;
        Data = data;
    }
}

// Generic Method inside a non-generic class (with multiple type parameters TSource and TResult)
public static class ObjectHelper
{
    public static T CloneWithDefault<T>() where T : new()
    {
        return new T();
    }

    public static TResult ConvertValue<TSource, TResult>(TSource input, Func<TSource, TResult> converter)
    {
        return converter(input);
    }
}
```

---

## 3. Collections & Data Structures
**Usage:** Stores, queries, and manipulates groups of objects in memory. Choosing the right interface for return types and parameters is critical in .NET APIs.

### Collection Interfaces Hierarchy (When to use which in Backend APIs)
| Interface | Capabilities | When to Use |
| :--- | :--- | :--- |
| `IEnumerable<T>` | **Forward-only iteration** (`foreach` + LINQ). Cannot `.Add()`, `.Remove()`, or `.Count` without iterating. | **Method parameters** accepting any sequence, or lazy-evaluated streams. |
| `IReadOnlyCollection<T>` / `IReadOnlyList<T>` | Iteration + `.Count` (+ `[index]` for List), **immutable** to caller. | **Return types** from Services/Repositories when callers shouldn't modify the list. |
| `ICollection<T>` | Iteration + `.Count`, `.Add()`, `.Remove()`, `.Contains()`, `.Clear()`. | **EF Core Navigation Properties** (e.g., `public ICollection<Order> Orders { get; set; }`). |
| `IList<T>` | Everything in `ICollection<T>` + **Index access** (`list[0]`, `InsertAt`, `RemoveAt`). | When positional index manipulation is required. |

### A. `List<T>` (Dynamic Array)
**Usage:** Fast $O(1)$ index lookup and fast appending. Most common collection in C#.

```csharp
// Constructors (Overloads)
List<string> list1 = new List<string>();                        // 1. Default (empty, default capacity)
List<string> list2 = new List<string>(capacity: 100);           // 2. Pre-allocated capacity (avoids array resizing)
List<string> list3 = new List<string>(existingEnumerable);      // 3. Copies elements from any IEnumerable<T>
List<string> list4 = ["Admin", "Manager", "User"];              // 4. C# 12 Collection Expression syntax

// Key Methods
list1.Add("Developer");
list1.AddRange(["QA", "DevOps"]);
bool exists = list1.Contains("QA");
list1.Remove("QA");
list1.RemoveAll(role => role.StartsWith("Dev"));                // Removes all matching a predicate
```

### B. `Dictionary<TKey, TValue>` (Key-Value Hash Map)
**Usage:** Ultra-fast $O(1)$ lookup by unique key (caches, lookup tables, grouping).

```csharp
// Constructors (Overloads)
// 1. Default constructor
Dictionary<int, string> dict1 = new Dictionary<int, string>();

// 2. With initial capacity
Dictionary<int, string> dict2 = new Dictionary<int, string>(capacity: 50);

// 3. With StringComparer (Crucial when string keys should be CASE-INSENSITIVE!)
Dictionary<string, int> headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

// 4. From an existing IDictionary or IEnumerable<KeyValuePair<TKey, TValue>>
Dictionary<int, string> dict4 = new Dictionary<int, string>(dict1);

// Key Methods & Safe Lookup Syntax
headerMap["Authorization"] = 1;                  // Inserts OR overwrites if key already exists
headerMap.Add("Content-Type", 2);                // Throws ArgumentException if key already exists!
bool added = headerMap.TryAdd("Content-Type", 3);// Safe Add: Returns false instead of throwing if key exists

// Safe Read: ALWAYS use TryGetValue to avoid KeyNotFoundException in 1 lookup!
if (headerMap.TryGetValue("authorization", out int val)) // Matches "Authorization" due to OrdinalIgnoreCase!
{
    Console.WriteLine($"Found value: {val}");
}
```

### C. `HashSet<T>` (Unique Set)
**Usage:** Stores **unique** elements only and provides $O(1)$ `.Contains()` checks (as opposed to $O(N)$ in a `List<T>`) plus mathematical set operations.

```csharp
// Constructors (Overloads)
HashSet<string> set1 = new HashSet<string>();                                         // 1. Default
HashSet<string> set2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);         // 2. Case-insensitive
HashSet<int> uniqueIds = new HashSet<int>(listWithDuplicateIds);                      // 3. Deduplicates a List!

// Key Methods
bool wasAdded = set1.Add("tag1");     // Returns true first time, false if "tag1" is already in the set
set1.UnionWith(["tag2", "tag3"]);     // Combines sets
set1.IntersectWith(["tag1", "tag2"]); // Keeps only elements present in both
set1.ExceptWith(["tag2"]);            // Removes specified elements
```

### D. `Queue<T>` (FIFO) & `Stack<T>` (LIFO)
```csharp
// Queue<T> (First-In, First-Out) - Constructors: (), (int capacity), (IEnumerable<T>)
Queue<string> jobQueue = new Queue<string>();
jobQueue.Enqueue("Job-1");
if (jobQueue.TryDequeue(out string? nextJob)) { /* Process Job-1 */ }

// Stack<T> (Last-In, First-Out) - Constructors: (), (int capacity), (IEnumerable<T>)
Stack<string> undoStack = new Stack<string>();
undoStack.Push("Action-1");
if (undoStack.TryPop(out string? lastAction)) { /* Undo Action-1 */ }
```

---

## 4. Delegates, Events & Lambda Expressions
**Usage:** A **Delegate** is a type-safe function pointer (allows passing methods as parameters/callbacks). **Lambdas (`=>`)** are inline anonymous functions assigned to delegates. **Events** implement the Publisher/Subscriber pattern (backbone of WinForms/WPF UI events and domain notifications).

### Built-In Delegates (Used in 99% of .NET Code)
| Built-in Delegate | Signature | Usage |
| :--- | :--- | :--- |
| `Action` / `Action<T1, ...>` | Returns `void` (0 to 16 input parameters). | Callbacks that perform an action without returning a value (`ForEach`, logging, event callbacks). |
| `Func<TResult>` / `Func<T1, ..., TResult>` | Returns `TResult` (last type parameter is **always** the return type). | LINQ queries (`Where`, `Select`), factory methods, data transformations. |
| `Predicate<T>` | Returns `bool` (takes 1 input `T`). | Used by `List<T>.FindAll()`, `List<T>.RemoveAll()`. Equivalent to `Func<T, bool>`. |

### Custom Delegate, Built-in Delegates & Lambda Syntax
```csharp
// 1. Custom Delegate Declaration (Only needed when using 'out' or 'ref' parameters, or for readability)
public delegate decimal TaxCalculationHandler(decimal amount, out decimal taxRateApplied);

// 2. Action<T> (Takes input, returns void)
Action<string> logMessage = msg => Console.WriteLine($"[LOG]: {msg}");
logMessage("Application started.");

// 3. Func<T1, T2, TResult> (Takes 2 ints, returns bool)
Func<int, int, bool> isGreater = (a, b) => a > b;
bool result = isGreater(10, 5); // true

// 4. Multi-statement Lambda (Uses '{ }' and 'return')
Func<decimal, decimal, decimal> calculateDiscount = (price, discountPercent) =>
{
    decimal discount = price * (discountPercent / 100m);
    return price - discount;
};

// 5. Predicate<T> (Takes T, returns bool)
Predicate<int> isEven = num => num % 2 == 0;
```

### Events (`event EventHandler<TEventArgs>`) — Standard .NET Pattern
**Usage:** The `event` keyword wraps a delegate so external classes can **only** subscribe (`+=`) or unsubscribe (`-=`), preventing external classes from invoking the event directly or wiping out other subscribers with `= null`.

```csharp
// 1. Custom EventArgs Class (Holds data sent when the event fires)
public class OrderPlacedEventArgs : EventArgs
{
    public int OrderId { get; }
    public decimal TotalAmount { get; }

    public OrderPlacedEventArgs(int orderId, decimal totalAmount)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
    }
}

// 2. Publisher Class (Raises the event)
public class OrderProcessor
{
    // Standard .NET Event signature using generic EventHandler<TEventArgs>
    public event EventHandler<OrderPlacedEventArgs>? OrderPlaced;

    public void PlaceOrder(int orderId, decimal total)
    {
        Console.WriteLine($"Order {orderId} saved to database.");
        
        // Null-conditional invoke: Fires event ONLY if at least 1 subscriber is attached
        OnOrderPlaced(new OrderPlacedEventArgs(orderId, total));
    }

    protected virtual void OnOrderPlaced(OrderPlacedEventArgs e)
    {
        OrderPlaced?.Invoke(this, e);
    }
}

// 3. Subscriber Usage (e.g., in Desktop UI or Event Listener)
OrderProcessor processor = new OrderProcessor();

// Subscribe using +=
EventHandler<OrderPlacedEventArgs> emailHandler = (sender, e) =>
    Console.WriteLine($"Sending email for Order #{e.OrderId} (${e.TotalAmount})");

processor.OrderPlaced += emailHandler;
processor.PlaceOrder(1001, 250.00m);

// ALWAYS unsubscribe using -= when disposing long-lived objects to prevent memory leaks!
processor.OrderPlaced -= emailHandler;
```

---

## 5. LINQ (Language Integrated Query)
**Usage:** Queries in-memory collections (`IEnumerable<T>`) or external databases via EF Core (`IQueryable<T>`) using C# method chains.

### Crucial Backend Distinction: `IEnumerable<T>` vs. `IQueryable<T>`
| Feature | `IEnumerable<T>` (`System.Linq`) | `IQueryable<T>` (`System.Linq.Queryable`) |
| :--- | :--- | :--- |
| **Delegate Parameter** | Takes compiled bytecode `Func<T, bool>`. | Takes an **Expression Tree** `Expression<Func<T, bool>>`. |
| **Where Filtering Happens** | **In C# Application RAM** (fetches ALL rows from DB first, then filters in memory!). | **Inside SQL Server** (EF Core translates the Expression Tree into a SQL `WHERE` clause!). |
| **When to Use** | Querying in-memory `List<T>`, `Array`, `Dictionary`. | Building database queries on `DbSet<T>` **before** calling `.ToListAsync()`. |

### Deferred vs. Immediate Execution
* **Deferred (Lazy) Execution**: Methods like `.Where()`, `.Select()`, `.OrderBy()`, `.Skip()`, `.Take()` **do NOT execute the query** when called—they only build the query pipeline. The query runs only when iterated (`foreach`) or materialized.
* **Immediate Execution**: Methods like `.ToList()`, `.ToArray()`, `.ToDictionary()`, `.Count()`, `.FirstOrDefault()`, `.SingleOrDefault()`, `.Any()`, `.Sum()` execute the query **immediately** and cache/return the result.

### Essential LINQ Operators Cheat-Sheet
```csharp
public record EmployeeRecord(int Id, string Name, string Dept, decimal Salary, List<string> Skills);

List<EmployeeRecord> employees =
[
    new(1, "Alice", "IT", 95000m, ["C#", "SQL"]),
    new(2, "Bob", "HR", 60000m, ["Recruiting"]),
    new(3, "Charlie", "IT", 110000m, ["C#", "Azure", "Docker"])
];

// 1. Filtering & Sorting (Deferred Execution)
var highPaidIt = employees
    .Where(e => e.Dept == "IT" && e.Salary > 90000m)
    .OrderByDescending(e => e.Salary)
    .ThenBy(e => e.Name);

// 2. Projection: Select & SelectMany (Flattening nested lists)
List<string> names = employees.Select(e => e.Name).ToList();
List<string> allUniqueSkills = employees.SelectMany(e => e.Skills).Distinct().ToList(); // ["C#", "SQL", "Recruiting", "Azure", "Docker"]

// 3. Single Element Lookup (CRITICAL Differences!)
// FirstOrDefault: Returns 1st match, or default (null) if 0 matches. Does NOT throw if multiple matches exist.
EmployeeRecord? firstIt = employees.FirstOrDefault(e => e.Dept == "IT");

// SingleOrDefault: Returns the ONLY match, or null if 0 matches. THROWS InvalidOperationException if >1 match exists! (Use for Primary Key / Unique Email lookups)
EmployeeRecord? empById = employees.SingleOrDefault(e => e.Id == 1);

// 4. Quantifiers (Fast boolean checks - Always prefer .Any() over .Count() > 0!)
bool hasItStaff = employees.Any(e => e.Dept == "IT");
bool allEarnAbove50k = employees.All(e => e.Salary >= 50000m);

// 5. Grouping & Aggregations (GroupBy, Count, Sum, Average, Max)
var deptStats = employees
    .GroupBy(e => e.Dept)
    .Select(group => new
    {
        Department = group.Key,
        HeadCount = group.Count(),
        TotalPayroll = group.Sum(e => e.Salary),
        AverageSalary = group.Average(e => e.Salary)
    })
    .ToList();

// 6. Pagination (Skip & Take - Used in every Web API list endpoint)
int pageNumber = 2, pageSize = 10;
var pagedResult = employees
    .OrderBy(e => e.Id)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToList();

// 7. Inner Join (Joining two collections on a matching key)
var departments = new[] { new { DeptCode = "IT", Location = "Building A" } };
var joined = employees.Join(
    inner: departments,
    outerKeySelector: emp => emp.Dept,
    innerKeySelector: dept => dept.DeptCode,
    resultSelector: (emp, dept) => new { emp.Name, dept.Location }
).ToList();
```

---

## 6. Exception Handling, Custom Exceptions & `IDisposable`
**Usage:** Gracefully handles runtime faults, preserves stack traces, and deterministically frees unmanaged resources (database connections, file handles, network sockets).

### Custom Exception Class (All 3 Standard Constructor Overloads)
**Rule:** Every custom exception in .NET should inherit from `Exception` and provide the **3 standard constructors**.

```csharp
public class EntityNotFoundException : Exception
{
    public string EntityName { get; } = string.Empty;
    public object? EntityKey { get; }

    // 1. Default Constructor
    public EntityNotFoundException() 
        : base("The requested entity was not found.")
    {
    }

    // 2. Constructor with Custom Message
    public EntityNotFoundException(string message) 
        : base(message)
    {
    }

    // 3. Constructor with Message and InnerException (Wraps lower-level exceptions without losing original causes)
    public EntityNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    // 4. Domain-specific Overloaded Constructor
    public EntityNotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.")
    {
        EntityName = entityName;
        EntityKey = key;
    }
}
```

### `try-catch-finally`, `when` Exception Filters & Re-throwing (`throw;` vs `throw ex;`)
```csharp
try
{
    throw new EntityNotFoundException("User", 42);
}
// Exception filter ('when'): Only catches if the condition evaluates to true
catch (EntityNotFoundException ex) when (ex.EntityName == "User")
{
    Console.WriteLine($"User lookup failed: {ex.Message}");
    
    // CRITICAL: Always use 'throw;' to rethrow! 
    // 'throw;' preserves the original stack trace. 'throw ex;' resets the stack trace to this line!
    throw; 
}
catch (Exception ex)
{
    // Wrap lower-level exception into a higher-level exception while passing 'ex' as InnerException
    throw new ApplicationException("An unexpected service error occurred.", ex);
}
finally
{
    // Always runs whether an exception was thrown or not
}
```

### `IDisposable`, `IAsyncDisposable` & `using` Declarations
**Usage:** Classes that hold unmanaged resources (`SqlConnection`, `FileStream`, `HttpClient`) implement `IDisposable`. Always wrap them in `using` so `.Dispose()` is guaranteed to run inside a compiler-generated `finally` block.

```csharp
public class TemporaryFileWriter : IDisposable, IAsyncDisposable
{
    private readonly System.IO.StreamWriter _writer;
    private bool _disposed;

    public TemporaryFileWriter(string filePath)
    {
        _writer = new System.IO.StreamWriter(filePath);
    }

    public void WriteLine(string text)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writer.WriteLine(text);
    }

    // Synchronous Dispose
    public void Dispose()
    {
        if (!_disposed)
        {
            _writer.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    // Asynchronous Dispose (Used with 'await using')
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _writer.DisposeAsync();
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}

// Modern C# 'using' declaration (Disposes automatically at the end of the current scope/method):
using var writer = new TemporaryFileWriter("temp.txt");
writer.WriteLine("Hello");
```

---

## 7. Extension Methods (`this` Keyword)
**Usage:** Adds new methods to existing types (`string`, `IServiceCollection`, `IQueryable<T>`) **without** modifying the original class or inheriting from it. All of LINQ (`.Where()`, `.Select()`) and ASP.NET Core DI setup (`services.AddScoped()`) are built using Extension Methods.

### Rules for Creating an Extension Method
1. Must be declared inside a **`static class`**.
2. The method itself must be **`static`**.
3. The first parameter must have the **`this` modifier** preceding the type being extended.

```csharp
public static class QueryableAndStringExtensions
{
    // 1. Extending 'string': Checks if string has text and truncates it safely
    public static string Truncate(this string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= maxLength ? value : value[..maxLength] + "...";
    }

    // 2. Extending 'IQueryable<T>': Reusable pagination extension for EF Core & LINQ!
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        int validPage = Math.Max(1, pageNumber);
        int validSize = Math.Clamp(pageSize, 1, 100);
        return query.Skip((validPage - 1) * validSize).Take(validSize);
    }
}

// Calling Syntax (Called just like an instance method on the object!):
string bio = "Senior .NET Backend Developer".Truncate(11); // "Senior .NET..."
// var pagedUsers = dbContext.Users.Paginate(pageNumber: 2, pageSize: 20).ToListAsync();
```

---

## 8. Nullable Types & Null-Safety Operators (`?`, `?.`, `??`, `??=`, `!`)
**Usage:** Prevents the billion-dollar mistake (`NullReferenceException`) by distinguishing between values that can be `null` and values that are guaranteed non-null.

### All C# Null Operators & Pattern Matching Checks
| Operator / Pattern | Name | Example & Meaning |
| :--- | :--- | :--- |
| `int?` / `Nullable<T>` | Nullable Value Type | Allows a `struct` (`int`, `bool`, `DateTime`) to hold `null`. Has `.HasValue` and `.Value`. |
| `string?` | Nullable Reference Type | Tells the compiler this reference variable is allowed to be `null` and forces you to check before dereferencing. |
| `?.` and `?[]` | Null-Conditional (Elvis) | `user?.Address?.City` — Evaluates to `null` immediately if `user` or `Address` is `null` instead of throwing. |
| `??` | Null-Coalescing | `string name = inputName ?? "Guest";` — Returns left side if non-null; otherwise returns fallback on right side. |
| `??=` | Null-Coalescing Assignment | `list ??= new List<int>();` — Assigns right side to left variable **only if** left variable is currently `null`. |
| `is null` / `is not null` | Null Pattern Matching | `if (user is not null)` — Preferred over `!= null` because it cannot be bypassed by overloaded `==` operators. |
| `!` (Postfix) | Null-Forgiving Operator | `user!.Name` — Silences compiler warning when you know for sure a value isn't null (e.g., EF Core `DbSet<T> null!`). |

```csharp
public class UserSession
{
    private List<string>? _permissions;

    public string GetDisplayCity(string? customTitle, DateTime? loginTime)
    {
        // 1. Nullable Value Type (DateTime?) handling
        DateTime effectiveTime = loginTime ?? DateTime.UtcNow;

        // 2. Null-Coalescing Assignment (Lazy initialization in 1 line!)
        _permissions ??= new List<string> { "Read" };

        // 3. Throw helper if argument is null (Modern .NET 7/8+)
        ArgumentNullException.ThrowIfNull(customTitle);

        // 4. Null-Coalescing with Throw Expression
        string firstPerm = _permissions.FirstOrDefault() 
            ?? throw new InvalidOperationException("No permissions found.");

        return $"{customTitle} ({firstPerm}) at {effectiveTime:T}";
    }
}
```

---

## 9. Attributes & Reflection
**Usage:**
* **Attributes (`[Attribute]`)**: Declarative metadata tags attached to classes, properties, methods, or parameters (e.g., `[HttpGet]`, `[Authorize]`, `[Required]`, `[JsonPropertyName]`).
* **Reflection (`System.Reflection`)**: Inspects types, attributes, properties, and invokes methods dynamically at runtime (how DI containers, AutoMapper, JSON serializers, and ORMs work under the hood).

### Creating a Custom Attribute (With Constructor Overloads)
```csharp
// AttributeUsage restricts where the attribute can be placed (Class, Property, Method)
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AuditFieldAttribute : Attribute
{
    public string DisplayName { get; }
    public bool IsSensitive { get; set; } // Optional named property

    // 1. Default Constructor
    public AuditFieldAttribute() : this("DefaultField")
    {
    }

    // 2. Overloaded Constructor with positional parameter
    public AuditFieldAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

// Applying Built-in & Custom Attributes to a Model
public class CustomerAccount
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [AuditField("Customer Email")] // Uses positional constructor
    public string Email { get; set; } = string.Empty;

    [AuditField("Social Security Number", IsSensitive = true)] // Positional + Named property
    public string Ssn { get; set; } = string.Empty;
}
```

### Reflection Syntax (Inspecting Properties, Reading Attributes & Setting Values at Runtime)
```csharp
CustomerAccount account = new CustomerAccount { Id = 10, Email = "dev@dotnet.com", Ssn = "999-00-1111" };

// 1. Get Type metadata (via typeof(T) at compile time, or instance.GetType() at runtime)
Type type = typeof(CustomerAccount); // or account.GetType();

// 2. Iterate through all public instance properties
PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

foreach (PropertyInfo prop in properties)
{
    // Read property value dynamically from the 'account' instance
    object? rawValue = prop.GetValue(account);

    // Check if our custom [AuditField] attribute is applied to this property
    AuditFieldAttribute? auditAttr = prop.GetCustomAttribute<AuditFieldAttribute>();

    if (auditAttr is not null)
    {
        string outputValue = auditAttr.IsSensitive ? "***REDACTED***" : $"{rawValue}";
        Console.WriteLine($"{auditAttr.DisplayName} ({prop.Name}): {outputValue}");
    }
}

// 3. Dynamically modifying a property and instantiating a type via Reflection:
PropertyInfo? emailProp = type.GetProperty(nameof(CustomerAccount.Email));
emailProp?.SetValue(account, "updated@dotnet.com");

// Dynamically create an instance of CustomerAccount at runtime without 'new':
CustomerAccount? dynamicInstance = (CustomerAccount?)Activator.CreateInstance(typeof(CustomerAccount));
```
