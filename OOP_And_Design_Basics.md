# Object-Oriented Programming (OOP) & Design in C#

A complete technical reference covering every OOP and Design concept in C#—including namespaces, access modifiers, all constructor types & overloading, properties, inheritance, polymorphism, abstraction, `record` types, and SOLID principles with exact syntax.

---

## 1. Required Namespaces & Setup
OOP features are built directly into the C# language and runtime (`System`), so no external NuGet packages are required.

```csharp
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // For [SetsRequiredMembers] attribute on constructors
```

---

## 2. Classes, Constructors & Constructor Overloading
**Usage:** A class is a reference-type blueprint for creating objects. Constructors initialize the state of an object when `new` is called.

### All Constructor Types & Overloading Syntax
* **Default / Parameterless Constructor**: Initializes with default values.
* **Parameterized Constructor (Overloaded)**: Accepts arguments to set initial state.
* **Constructor Chaining (`: this(...)`)**: Calls another constructor in the **same** class to avoid duplicate initialization code.
* **Static Constructor (`static ClassName()`)**: Runs **once** automatically before the first instance is created or any static member is accessed. Cannot take parameters or have access modifiers.
* **Copy Constructor**: Takes an instance of the same class to clone its state.
* **Private Constructor**: Prevents external instantiation (used in Singleton pattern or static utility classes).
* **Primary Constructor (C# 12+)**: Declares constructor parameters directly on the class header.

```csharp
public class BankAccount
{
    public string AccountNumber { get; }
    public string OwnerName { get; set; }
    public decimal Balance { get; private set; }
    public static decimal InterestRate { get; private set; }

    // 1. Static Constructor (Runs ONCE per application lifetime before any BankAccount is used)
    static BankAccount()
    {
        InterestRate = 0.045m; // 4.5%
    }

    // 2. Default / Parameterless Constructor (Chains to the 2-parameter constructor using ': this(...)')
    public BankAccount() : this("UNKNOWN", "Anonymous")
    {
    }

    // 3. Overloaded Constructor #1 (Chains to the 3-parameter master constructor)
    public BankAccount(string accountNumber, string ownerName) : this(accountNumber, ownerName, 0.0m)
    {
    }

    // 4. Overloaded Constructor #2 (Master Constructor)
    public BankAccount(string accountNumber, string ownerName, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = initialBalance;
    }

    // 5. Copy Constructor (Clones state from an existing instance)
    public BankAccount(BankAccount existingAccount)
    {
        AccountNumber = existingAccount.AccountNumber;
        OwnerName = existingAccount.OwnerName;
        Balance = existingAccount.Balance;
    }
}

// Instantiation Syntax:
BankAccount acc1 = new BankAccount();                                  // Calls Default
BankAccount acc2 = new BankAccount("ACC-101", "Alice");                // Calls Overload #1
BankAccount acc3 = new BankAccount("ACC-102", "Bob", 5000m);           // Calls Overload #2
BankAccount acc4 = new BankAccount(acc3);                              // Calls Copy Constructor
```

### Primary Constructors (Modern C# 12+ Syntax - Heavily used in Web API Dependency Injection)
```csharp
// Parameters are declared directly in the class declaration:
public class OrderService(IOrderRepository repository, ILogger<OrderService> logger)
{
    // If you add an overload, it MUST chain to the primary constructor via ': this(...)'
    public OrderService() : this(new SqlOrderRepository(), new ConsoleLogger<OrderService>())
    {
    }

    public void ProcessOrder(int orderId)
    {
        logger.Log($"Processing {orderId}");
        repository.Save(orderId);
    }
}
```

---

## 3. Encapsulation, Access Modifiers & Properties
**Usage:** Hides internal object state and forces all state modifications through controlled properties or methods with validation rules.

### Complete Access Modifiers Table
| Modifier | Accessible Inside Class | Derived Class (Same Project) | Derived Class (Other Project) | Any Class (Same Project/Assembly) | Any Class (Other Project) |
| :--- | :---: | :---: | :---: | :---: | :---: |
| `public` | Yes | Yes | Yes | Yes | Yes |
| `private` | **Yes** | No | No | No | No |
| `protected` | Yes | **Yes** | **Yes** | No | No |
| `internal` | Yes | Yes | No | **Yes** | No |
| `protected internal` | Yes | Yes | **Yes** | **Yes** | No |
| `private protected` | Yes | **Yes** | No | No | No |
| `file` *(C# 11+)* | Only visible inside the **current `.cs` file** (used for source generators/helpers). |

### Fields vs. Properties Syntax (All Property Variations)
```csharp
public class Employee
{
    // 1. Constant (Compile-time constant, implicitly static)
    public const int MaxVacationDays = 30;

    // 2. Readonly Field (Can ONLY be assigned inline or inside a constructor)
    private readonly Guid _employeeId = Guid.NewGuid();

    // 3. Private Backing Field (Used when custom validation logic is needed in a property)
    private decimal _salary;

    // 4. Full Property with Validation (Encapsulation in action)
    public decimal Salary
    {
        get => _salary;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Salary cannot be negative.");
            _salary = value;
        }
    }

    // 5. Auto-Implemented Property (Compiler creates hidden backing field automatically)
    public string Department { get; set; } = "General";

    // 6. Read-Only Auto-Property (Can only be set inline or in the constructor)
    public DateTime HireDate { get; } = DateTime.UtcNow;

    // 7. Write-Protected Auto-Property (Publicly readable, only modifiable inside this class)
    public bool IsActive { get; private set; } = true;

    // 8. Init-Only Property (C# 9+: Can be set in constructor OR object initializer '{ }', then becomes immutable)
    public string NationalId { get; init; } = string.Empty;

    // 9. Required Property (C# 11+: Forces caller to initialize this property in '{ }' when creating object)
    public required string FullName { get; init; }

    // 10. Computed / Expression-Bodied Property (No backing field; calculated on the fly)
    public decimal MonthlySalary => _salary / 12;

    // Constructor with [SetsRequiredMembers] tells compiler this constructor satisfies 'required' properties
    public Employee() { }

    [SetsRequiredMembers]
    public Employee(string fullName, string nationalId, decimal salary)
    {
        FullName = fullName;
        NationalId = nationalId;
        Salary = salary;
    }
}

// Object Initializer Syntax (Uses 'required' and 'init' properties):
Employee emp = new Employee
{
    FullName = "John Wick",      // Required! Won't compile if omitted
    NationalId = "NID-998877",   // Init-only! Cannot be changed after this block
    Salary = 120000m,
    Department = "Security"
};
// emp.NationalId = "NEW";       // COMPILER ERROR: Init-only property can only be assigned in initializer
```

---

## 4. Inheritance & Composition
**Usage:**
* **Inheritance (`is-a` relationship)**: A child/derived class inherits members from a single parent/base class (`: BaseClass`). C# supports **single class inheritance** only.
* **Composition (`has-a` relationship)**: Building complex objects by combining smaller component objects as fields/properties instead of inheriting from a deep class hierarchy.

### Inheritance Keywords
* `: base(...)` — Calls a specific constructor on the parent class before the child constructor body runs.
* `base.MethodName()` — Calls the parent class's implementation of a method.
* `sealed` — Prevents a class from being inherited, or prevents an `override` method from being overridden further down the hierarchy.

### Inheritance & `: base(...)` Constructor Overloading Syntax
```csharp
// Base (Parent) Class
public class Vehicle
{
    public string Vin { get; }
    public int MaxSpeed { get; protected set; } // 'protected': accessible by child classes, hidden from public

    // Base Constructor #1
    public Vehicle(string vin) : this(vin, 120)
    {
    }

    // Base Constructor #2
    public Vehicle(string vin, int maxSpeed)
    {
        Vin = vin;
        MaxSpeed = maxSpeed;
    }

    public void StartEngine() => Console.WriteLine($"Vehicle {Vin} engine started.");
}

// Derived (Child) Class — Sealed so no class can inherit from ElectricCar
public sealed class ElectricCar : Vehicle
{
    public int BatteryCapacityKWh { get; }

    // Child Constructor #1 -> Calls Base Constructor #1 via ': base(vin)'
    public ElectricCar(string vin) : base(vin)
    {
        BatteryCapacityKWh = 75;
    }

    // Child Constructor #2 -> Calls Base Constructor #2 via ': base(vin, maxSpeed)'
    public ElectricCar(string vin, int maxSpeed, int batteryCapacityKWh) 
        : base(vin, maxSpeed)
    {
        BatteryCapacityKWh = batteryCapacityKWh;
    }
}
```

### Favoring Composition Over Inheritance (Backend Design Best Practice)
**Why:** Deep inheritance trees are fragile—changing a base class can break dozens of child classes. Composition lets you swap behavior at runtime (and is the foundation of Dependency Injection).

```csharp
// Component classes (Focused responsibilities)
public class Engine
{
    public void Ignite() => Console.WriteLine("Engine ignited.");
}

public class GpsNavigator
{
    public void NavigateTo(string destination) => Console.WriteLine($"Routing to {destination}...");
}

// Composed class ("Car HAS-AN Engine and HAS-A GpsNavigator")
public class SmartCar
{
    private readonly Engine _engine;
    private readonly GpsNavigator _gps;

    // Components injected via Constructor
    public SmartCar(Engine engine, GpsNavigator gps)
    {
        _engine = engine;
        _gps = gps;
    }

    public void DriveTo(string destination)
    {
        _engine.Ignite();
        _gps.NavigateTo(destination);
    }
}
```

---

## 5. Polymorphism
**Usage:** Allows a single interface or base type to represent different underlying forms/behaviors.
1. **Compile-Time (Static) Polymorphism**: Method Overloading & Operator Overloading.
2. **Runtime (Dynamic) Polymorphism**: Method Overriding via `virtual` and `override`.

### A. Compile-Time Polymorphism: Method Overloading & Operator Overloading
**Rule:** Overloaded methods must have the same name but **different parameter types, number of parameters, or parameter order** (return type alone cannot overload a method).

```csharp
public class PaymentProcessor
{
    // 1. Overloaded Methods (Same name, different parameter signatures)
    public bool Charge(decimal amount) 
        => Charge(amount, "USD", null);

    public bool Charge(decimal amount, string currency) 
        => Charge(amount, currency, null);

    public bool Charge(decimal amount, string currency, string? couponCode)
    {
        Console.WriteLine($"Charged {amount} {currency} (Coupon: {couponCode ?? "None"})");
        return true;
    }
}

// 2. Operator Overloading (Customizing '+', '-', '==' for your own types)
public readonly struct Money(decimal amount, string currency)
{
    public decimal Amount { get; } = amount;
    public string Currency { get; } = currency;

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add different currencies.");
        return new Money(a.Amount + b.Amount, a.Currency);
    }
}
```

### B. Runtime Polymorphism: `virtual`, `override`, `sealed override`, & `new`
* `virtual`: Marks a base class method/property as **optional** to override in derived classes.
* `override`: Replaces the base `virtual`/`abstract` implementation at runtime (even when referenced via a base-type variable).
* `sealed override`: Overrides the method for the current class, but blocks any further child classes from overriding it.
* `new` (Method Hiding): Hides the base method without polymorphism (calls depend on the variable's compile-time type, not runtime object type—rarely recommended).

```csharp
public class NotificationSender
{
    // Virtual method provides default behavior that child classes CAN override
    public virtual void Send(string message)
    {
        Console.WriteLine($"[Default Log Notification]: {message}");
    }

    public void SendTimestamped(string message) => Console.WriteLine($"{DateTime.UtcNow}: {message}");
}

public class EmailNotificationSender : NotificationSender
{
    // Overrides base implementation at runtime
    public override void Send(string message)
    {
        base.Send(message); // Optional: Call parent logic first
        Console.WriteLine($"[Sending Email]: {message}");
    }
}

public class SmsNotificationSender : NotificationSender
{
    // 'sealed override': Overrides NotificationSender.Send, but no subclass of SmsNotificationSender can override it again
    public sealed override void Send(string message)
    {
        Console.WriteLine($"[Sending SMS]: {message}");
    }

    // 'new' keyword: Method Hiding (NOT polymorphic when accessed via NotificationSender reference)
    public new void SendTimestamped(string message) => Console.WriteLine($"[SMS Time]: {message}");
}

// Runtime Polymorphism in Action:
List<NotificationSender> senders = new List<NotificationSender>
{
    new NotificationSender(),
    new EmailNotificationSender(),
    new SmsNotificationSender()
};

foreach (NotificationSender sender in senders)
{
    // Calls the exact overridden Send() method of the actual runtime object!
    sender.Send("Server CPU at 95%");
}
```

---

## 6. Abstraction: Abstract Classes vs. Interfaces
**Usage:** Defines contracts (what an object can do) without exposing full implementation details (how it does it).

### Key Differences: Abstract Class vs. Interface
| Feature | `abstract class` | `interface` |
| :--- | :--- | :--- |
| **Multiple Inheritance** | A class can inherit from **ONLY ONE** abstract class. | A class can implement **MULTIPLE** interfaces. |
| **Constructors** | **Yes** (`protected` constructors called by derived classes). | **No** instance constructors allowed. |
| **Instance Fields (State)** | **Yes** (can hold private/protected fields & state). | **No** instance fields (only properties, methods, events, static fields). |
| **Default Implementation** | **Yes** (can mix regular methods, `virtual` methods, and `abstract` methods). | **Yes** (Since C# 8+, Default Interface Methods are supported). |
| **When to Use** | Shared base state + shared code among closely related classes (`is-a`). | Decoupled contracts for Dependency Injection & capabilities (`can-do`). |

### A. `abstract class` Syntax (With Protected Constructors & `abstract` Members)
> An `abstract` class **cannot be instantiated directly** with `new`. Any `abstract` method/property has no body (`;`) and **must** be overridden by non-abstract child classes.

```csharp
public abstract class ReportGenerator
{
    public string ReportTitle { get; }

    // Abstract classes should use 'protected' constructors (only callable by child classes via ': base(...)')
    protected ReportGenerator(string reportTitle)
    {
        ReportTitle = reportTitle;
    }

    // 1. Concrete Method (Shared by all subclasses)
    public void LogGenerationStart() => Console.WriteLine($"Starting generation for: {ReportTitle}");

    // 2. Abstract Property (Must be implemented by subclasses)
    public abstract string FileExtension { get; }

    // 3. Abstract Method (No body! Subclasses MUST use 'override' to provide the body)
    public abstract byte[] GenerateBytes(string rawData);
}

public class PdfReportGenerator : ReportGenerator
{
    // Calls protected base constructor
    public PdfReportGenerator(string title) : base(title)
    {
    }

    public override string FileExtension => ".pdf";

    public override byte[] GenerateBytes(string rawData)
    {
        LogGenerationStart();
        return System.Text.Encoding.UTF8.GetBytes($"[PDF HEADER] {rawData}");
    }
}
```

### B. `interface` Syntax (Multiple Implementation, Default Interface Methods & Explicit Implementation)
```csharp
public interface IExportable
{
    // 1. Standard Interface Contract (No body, implicitly public)
    string ExportToJson();

    // 2. Default Interface Method (C# 8+: Provides a fallback implementation if the class doesn't implement it)
    void PrintSummary()
    {
        Console.WriteLine($"Summary JSON: {ExportToJson()}");
    }
}

public interface IAuditable
{
    DateTime CreatedAt { get; }
    void LogAudit();
}

// A class can inherit from 1 base class AND implement multiple interfaces (Base class ALWAYS comes first!)
public class InvoiceDocument : ReportGenerator, IExportable, IAuditable
{
    public DateTime CreatedAt { get; } = DateTime.UtcNow;

    public InvoiceDocument(string title) : base(title) { }

    public override string FileExtension => ".inv";

    public override byte[] GenerateBytes(string rawData) => Array.Empty<byte>();

    // Implementing IExportable
    public string ExportToJson() => $"{{ \"title\": \"{ReportTitle}\", \"created\": \"{CreatedAt:O}\" }}";

    // Explicit Interface Implementation (Only accessible when cast to IAuditable)
    // Useful when two interfaces have methods with the exact same name!
    void IAuditable.LogAudit()
    {
        Console.WriteLine($"Audited Invoice '{ReportTitle}' at {CreatedAt}");
    }
}

// Usage of Explicit Interface Implementation & Default Interface Method:
InvoiceDocument doc = new InvoiceDocument("Q4 Tax Invoice");
// doc.LogAudit();              // COMPILER ERROR (Explicitly implemented)
((IAuditable)doc).LogAudit();   // Works! Casts to interface first
((IExportable)doc).PrintSummary(); // Calls the Default Interface Method
```

---

## 7. Records & Immutability (`record`, `record struct`, `with`)
**Usage:** Introduced in C# 9/10 specifically for **immutable Data Transfer Objects (DTOs)**, API Request/Response models, Value Objects, and Events.
* **Value-based Equality**: Two `record` instances are equal (`==`) if all their properties have the same values (unlike classes, which compare memory references).
* **Built-in Formatting**: Automatically overrides `ToString()`, `Equals()`, and `GetHashCode()`.
* **Non-destructive Mutation (`with` expression)**: Creates a modified copy of an immutable record without mutating the original.

### Syntax & Constructor Overloading in Records
```csharp
// 1. Positional Record (Reference Type - 'record' or 'record class')
// Automatically creates 'public init' properties (Id, Name, Price) and a Deconstruct() method!
public record ProductDto(int Id, string Name, decimal Price);

// 2. Record with Custom Constructors, Additional Properties & Methods
public record UserProfileDto
{
    public int UserId { get; init; }
    public string Username { get; init; }
    public string Role { get; init; } = "Standard";

    // Overload #1
    public UserProfileDto(int userId, string username)
    {
        UserId = userId;
        Username = username;
    }

    // Overload #2 (Chains to Overload #1)
    public UserProfileDto(int userId, string username, string role) : this(userId, username)
    {
        Role = role;
    }
}

// 3. Record Struct (Value Type allocated on Stack - C# 10+)
public readonly record struct GeoCoordinate(double Latitude, double Longitude);
```

### Record Operations (`==`, `with`, Deconstruction)
```csharp
ProductDto p1 = new ProductDto(1, "Laptop", 1200m);
ProductDto p2 = new ProductDto(1, "Laptop", 1200m);

// 1. Value-Based Equality (True for records, would be False for regular classes!)
bool areEqual = (p1 == p2); // true

// 2. Non-Destructive Mutation ('with' expression copies p1 and changes only Price)
ProductDto discountedLaptop = p1 with { Price = 999m };

// 3. Built-in Deconstruction into tuple variables
var (id, name, price) = discountedLaptop;

// 4. Built-in ToString() outputs: "ProductDto { Id = 1, Name = Laptop, Price = 999 }"
Console.WriteLine(discountedLaptop);
```

---

## 8. SOLID Principles in C# (Practical Backend Patterns)
**Usage:** The 5 foundational object-oriented design rules for writing maintainable, testable, loosely-coupled .NET backend applications.

### 1. **S** — Single Responsibility Principle (SRP)
**Rule:** A class should have **only one reason to change** (one job). Don't mix database persistence, business calculations, and email sending in a single class.

```csharp
// BAD: UserService validates, saves to SQL, and sends welcome emails.
// GOOD: Split into single-purpose classes:
public class UserValidator { public bool IsValid(string email) => email.Contains('@'); }
public class UserRepository { public void SaveToDb(string email) { /* ADO.NET / EF Core logic */ } }
public class EmailService   { public void SendWelcome(string email) { /* SMTP logic */ } }
```

### 2. **O** — Open/Closed Principle (OCP)
**Rule:** Classes should be **open for extension, but closed for modification**. Instead of modifying an existing `if/else` or `switch` block every time you add a new discount type, use an interface/abstract class and add a new implementation class.

```csharp
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal total);
}

public class VipDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal total) => total * 0.80m; // 20% off
}

public class SeasonalDiscount : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal total) => total - 15m;   // Flat $15 off
}

// CheckoutService NEVER needs to be modified when you invent a new discount type!
public class CheckoutService
{
    public decimal CalculateFinalPrice(decimal total, IDiscountStrategy discountStrategy)
    {
        return discountStrategy.ApplyDiscount(total);
    }
}
```

### 3. **L** — Liskov Substitution Principle (LSP)
**Rule:** Subclasses must be **completely substitutable** for their base class/interface without breaking the program or throwing unexpected `NotImplementedException`s.

```csharp
// BAD: ReadOnlyFile inherits from WritableFile and throws NotSupportedException on Save().
// GOOD: Separate read and write contracts so ReadOnlyFile never pretends it can write:
public interface IReadableFile { string ReadAllText(); }
public interface IWritableFile : IReadableFile { void WriteAllText(string content); }

public class ReadOnlyConfigFile : IReadableFile
{
    public string ReadAllText() => "AppConfig=True";
}

public class LogFile : IWritableFile
{
    public string ReadAllText() => "Logs...";
    public void WriteAllText(string content) => Console.WriteLine($"Wrote: {content}");
}
```

### 4. **I** — Interface Segregation Principle (ISP)
**Rule:** Clients should not be forced to implement fat interfaces containing methods they don't use. Split large interfaces into smaller, role-specific interfaces.

```csharp
// BAD: One giant IWorker interface with Work(), EatLunch(), and RechargeBattery() (Robots don't eat lunch!).
// GOOD: Segregated interfaces:
public interface IWorkable   { void Work(); }
public interface IFeedable   { void EatLunch(); }
public interface IChargeable { void RechargeBattery(); }

public class HumanEmployee : IWorkable, IFeedable
{
    public void Work() => Console.WriteLine("Coding...");
    public void EatLunch() => Console.WriteLine("Eating lunch...");
}

public class WarehouseRobot : IWorkable, IChargeable
{
    public void Work() => Console.WriteLine("Moving pallets...");
    public void RechargeBattery() => Console.WriteLine("Charging at dock...");
}
```

### 5. **D** — Dependency Inversion Principle (DIP)
**Rule:** High-level business modules (`OrderManager`) must **not** depend on low-level infrastructure modules (`SqlOrderDatabase`) directly. Both should depend on **abstractions (`interfaces`)**. This is the exact principle behind **ASP.NET Core Dependency Injection**.

```csharp
// 1. Abstraction (Interface)
public interface IPaymentGateway
{
    bool ProcessPayment(decimal amount);
}

// 2. Low-level Module #1 (Stripe Implementation)
public class StripeGateway : IPaymentGateway
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Paid ${amount} via Stripe.");
        return true;
    }
}

// 3. Low-level Module #2 (PayPal Implementation)
public class PayPalGateway : IPaymentGateway
{
    public bool ProcessPayment(decimal amount)
    {
        Console.WriteLine($"Paid ${amount} via PayPal.");
        return true;
    }
}

// 4. High-level Module (Depends ONLY on IPaymentGateway interface, NEVER 'new StripeGateway()')
public class OrderController
{
    private readonly IPaymentGateway _paymentGateway;

    // Constructor Injection: Caller (or .NET DI Container) passes in whichever implementation is configured
    public OrderController(IPaymentGateway paymentGateway)
    {
        _paymentGateway = paymentGateway;
    }

    public void Checkout(decimal cartTotal)
    {
        _paymentGateway.ProcessPayment(cartTotal);
    }
}
```
