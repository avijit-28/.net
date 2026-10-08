# C# & .NET Core Concepts Roadmap (Backend, Web API & Desktop)

A direct, intermediate-to-advanced checklist of C# and .NET concepts required for Backend, Web API, and Desktop development.

---

## 1. Object-Oriented Programming (OOP) & Design
- [ ] **Encapsulation & Access Modifiers** (`public`, `private`, `protected`, `internal`, Properties vs. Fields)
- [ ] **Inheritance & Composition** (`base`, `sealed`, favoring composition over inheritance)
- [ ] **Polymorphism** (Method Overloading, Method Overriding with `virtual` / `override`)
- [ ] **Abstraction** (Abstract Classes vs. Interfaces, Default Interface Methods)
- [ ] **SOLID Principles** (Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion)
- [ ] **Records & Immutability** (`record`, `init` accessors, value-based equality)

## 2. Core C# Language Features (Used Heavily in .NET)
- [ ] **Generics** (`List<T>`, generic methods/classes, `where` type constraints)
- [ ] **Collections & Data Structures** (`IEnumerable<T>`, `ICollection<T>`, `IList<T>`, `Dictionary<TKey, TValue>`, `HashSet<T>`)
- [ ] **Delegates, Events & Lambda Expressions** (`Func<>`, `Action<>`, `Predicate<>`, Event Handlers—crucial for Desktop apps)
- [ ] **LINQ (Language Integrated Query)** (`Where`, `Select`, `FirstOrDefault`, `GroupBy`, `Join`, Deferred vs. Immediate Execution, `IEnumerable` vs. `IQueryable`)
- [ ] **Exception Handling & Custom Exceptions** (`try-catch-finally`, `using` statements, `IDisposable`, global error handling)
- [ ] **Extension Methods** (Extending existing types without inheritance—heavily used in Web API configuration)
- [ ] **Nullable Reference Types** (`?`, `??`, `??=`, avoiding `NullReferenceException`)
- [ ] **Attributes & Reflection** (Reading metadata at runtime, `[Required]`, `[HttpGet]`, custom attributes)

## 3. Asynchronous & Concurrent Programming
- [ ] **Async / Await Pattern** (`Task`, `Task<T>`, `ValueTask`, non-blocking I/O for APIs and UI threads)
- [ ] **Cancellation Tokens** (`CancellationToken` for aborting long-running API/DB requests)
- [ ] **Multithreading & Concurrency** (Thread safety, `lock`, `SemaphoreSlim`, `ConcurrentDictionary`, `Parallel.ForEach`)

## 4. Database & Data Access (SQL & ORMs)
- [ ] **ADO.NET Basics** (`SqlConnection`, `SqlCommand`, `SqlDataReader`, Connection Pooling)
- [ ] **Dapper (Micro-ORM)** (Raw SQL queries, parameterized queries, mapping to C# objects, Stored Procedures)
- [ ] **Entity Framework Core (EF Core)**
  - [ ] Code-First vs. Database-First & Migrations
  - [ ] `DbContext` & `DbSet<T>` lifecycle
  - [ ] Relationships (One-to-Many, Many-to-Many, Fluent API vs. Data Annotations)
  - [ ] Tracking vs. No-Tracking queries (`AsNoTracking()`)
  - [ ] Loading Strategies (Eager loading with `.Include()`, Explicit, Lazy Loading)
  - [ ] Database Transactions & Concurrency tokens

## 5. .NET Architecture & Application Fundamentals
- [ ] **Dependency Injection (DI) & IoC Containers** (Service lifetimes: `Transient`, `Scoped`, `Singleton`)
- [ ] **Configuration & Options Pattern** (`appsettings.json`, `IConfiguration`, `IOptions<T>`, User Secrets, Environment Variables)
- [ ] **Logging** (`ILogger<T>`, Structured Logging with Serilog/NLog)
- [ ] **Serialization & Deserialization** (`System.Text.Json`, JSON attributes, custom converters)
- [ ] **Memory Management** (Stack vs. Heap, Value vs. Reference types, Garbage Collection, `IDisposable`)

## 6. ASP.NET Core & Web API (Backend Priority)
- [ ] **Request Pipeline & Middleware** (How HTTP requests flow, custom middleware, `UseRouting`, `UseAuthentication`)
- [ ] **Controllers vs. Minimal APIs** (Routing, Action Results, `IActionResult` / `ActionResult<T>`)
- [ ] **Model Binding & Validation** (`[FromBody]`, `[FromQuery]`, `[FromRoute]`, Data Annotations, **FluentValidation**)
- [ ] **DTOs (Data Transfer Objects) & Mapping** (Separating DB entities from API models, AutoMapper / Mapster / manual mapping)
- [ ] **Authentication & Authorization** (JWT Bearer Tokens, Claims, Roles, Policy-based Authorization, OAuth2 / OpenID Connect, ASP.NET Core Identity)
- [ ] **HTTP Clients** (`IHttpClientFactory`, `HttpClient`, resilience/retries with Polly)
- [ ] **Caching** (`IMemoryCache`, Distributed Caching with Redis)
- [ ] **Background Jobs / Hosted Services** (`IHostedService`, `BackgroundService`, Hangfire / Quartz.NET)
- [ ] **API Documentation & Versioning** (Swagger / OpenAPI, API Versioning, Rate Limiting, CORS)

## 7. Desktop Development (WPF / WinForms)
- [ ] **Event-Driven Programming** (UI events, routed events)
- [ ] **UI Threading** (`Dispatcher` in WPF—updating UI from background threads without freezing the app)
- [ ] **Data Binding & INotifyPropertyChanged** (Two-way binding between UI and C# properties)
- [ ] **MVVM Pattern (Model-View-ViewModel)** (`ICommand` / `RelayCommand`, ViewModels, CommunityToolkit.Mvvm)

## 8. Common Backend Design Patterns & Testing
- [ ] **Repository & Unit of Work Patterns** (Abstracting data access)
- [ ] **CQRS & Mediator Pattern** (Separating reads and writes using MediatR)
- [ ] **Clean / Onion / N-Tier Architecture** (Structuring projects into Domain, Application, Infrastructure, and API layers)
- [ ] **Unit & Integration Testing** (xUnit / NUnit, Mocking dependencies with Moq or NSubstitute, `WebApplicationFactory`)
