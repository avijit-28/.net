# Grand Azure Hotel & Reservation Management System
### Enterprise ASP.NET Core Web API (.NET 10) & Pure ADO.NET Architecture

**Author:** Avijit Pakhira  
**Date:** 2026-11-17  
**Database:** Microsoft SQL Server 2025  
**Data Access:** Pure ADO.NET (`Microsoft.Data.SqlClient`) using 100% Stored Procedures  

---

## 1. Project Directory Structure

```
D:\avi\C#\Assignments\Assignment2\
├── HotelBooking.slnx                                   # .NET 10 Solution File
├── README.md                                           # System Documentation & Run Guide
│
├── database/                                           # 📂 Transact-SQL Scripts
│   ├── 00_Complete_Master_Setup.sql                    # Combined master setup script
│   ├── 01_Schema.sql                                   # 3NF Tables, Constraints, and Sequence
│   ├── 02_Indexes.sql                                  # Covering & Non-Clustered Indexes
│   ├── 03_StoredProcedures.sql                         # 33 Stored Procedures with ACID Transactions
│   └── 04_SeedData.sql                                 # Seed accounts with PBKDF2 hashes
│
├── tests/                                              # 📂 Postman Automated Testing Suite
│   ├── HotelBooking_Capstone.postman_collection.json   # Postman collection (pm.test assertions)
│   └── HotelBooking_Environment.postman_environment.json# Decoupled environment variables
│
└── src/                                                # 📂 Clean N-Tier Application Source Code
    ├── HotelBooking.Models/                            # Domain Entities & API DTOs
    │   ├── Entities/                                   # User, Room, RoomType, Amenity, Booking, Cancellation
    │   ├── DTOs/                                       # Auth, Rooms, RoomTypes, Amenities, Search, Bookings
    │   └── Common/                                     # ApiResponse<T>
    │
    ├── HotelBooking.DAL/                               # Data Access Layer (Pure ADO.NET)
    │   ├── Connection/                                 # IDbConnectionFactory using Microsoft.Data.SqlClient
    │   └── Repositories/                               # UserRepository, RoomRepository, BookingRepository, etc.
    │
    ├── HotelBooking.BAL/                               # Business Access Layer
    │   ├── Security/                                   # PasswordHasher (PBKDF2 SHA-256), JwtTokenGenerator
    │   └── Services/                                   # AuthService, RoomService, BookingService, etc.
    │
    └── HotelBooking.API/                               # Web API & Frontend Hosting
        ├── Controllers/                                # REST Controllers (Modules 3 through 10)
        ├── Middleware/                                 # RFC 7807 GlobalExceptionHandlingMiddleware
        ├── wwwroot/                                    # Built-in Interactive Web UI (HTML, CSS, JS)
        ├── appsettings.json                            # SQL Connection string & JWT configuration
        └── Program.cs                                  # ASP.NET Core pipeline & DI registration
```

---

## 2. Default Seed Accounts

Both accounts use PBKDF2 (HMAC-SHA256, 100,000 iterations, unique 16-byte cryptographic salt):

| Role | Email | Password | Access Capabilities |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin@hotel.com` | `Admin@Hotel2026!` | Inventory management, room creation, maintenance toggling, global booking audit. |
| **Customer** | `customer@example.com` | `Customer@Pass2026!` | Room availability search, stay reservation, booking history, and 3-tier cancellation. |

---

## 3. How to Run the Application

### Option A: From Command Line
```powershell
cd "D:\avi\C#\Assignments\Assignment2\src\HotelBooking.API"
dotnet run --launch-profile "http"
```
Then open your browser at:
- **Web UI:** [http://localhost:5102](http://localhost:5102)
- **OpenAPI Schema:** [http://localhost:5102/openapi/v1.json](http://localhost:5102/openapi/v1.json)

### Option B: From Visual Studio 2022+ / VS Code
1. Open `D:\avi\C#\Assignments\Assignment2\HotelBooking.slnx`.
2. Set `HotelBooking.API` as the Startup Project.
3. Press `F5` or `Ctrl+F5` to run with the `http` profile.

---

## 4. Key Architectural Highlights

1. **Pure ADO.NET & Zero ORM:** Every query executes through `Microsoft.Data.SqlClient.SqlCommand` calling Stored Procedures with explicitly typed `SqlParameter` objects.
2. **Double-Booking Elimination:** `usp_Booking_Create` uses row locks `WITH (UPDLOCK, ROWLOCK)` on `Rooms` and range locks `WITH (UPDLOCK, HOLDLOCK)` on `Bookings`, returning HTTP 409 Conflict if an overlap occurs.
3. **Automated 3-Tier Cancellation Policy:**
   - **> 48 hours before check-in:** 100% Refund (0% fee).
   - **24 to 48 hours before check-in:** 50% Refund (50% fee).
   - **< 24 hours before check-in:** 0% Refund (100% fee).
4. **Relational Division for Amenities:** `usp_Room_SearchAvailable` ensures rooms possess **all** selected amenities (`HAVING COUNT(DISTINCT AmenityId) = @Count`).
5. **RFC 7807 ProblemDetails:** All database validation and constraint errors are translated to standardized RFC 7807 problem details responses by the custom middleware.
