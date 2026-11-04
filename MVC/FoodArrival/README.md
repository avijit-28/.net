# FoodArrival / FoodReady

FoodArrival is a multi-role food ordering and delivery web application built with ASP.NET MVC 5, .NET Framework 4.8.1, SQL Server, and ADO.NET. The app supports customers, restaurant owners, restaurant staff, riders, and administrators.

## Documentation

- [Workflow and architecture (PDF)](README.pdf)
- [Project implementation plan](FOOD_DELIVERY_PROJECT_PLAN.md) — a planning document; some parts describe a target design and may differ from the current implementation.

## Technology

| Area | Implementation |
| --- | --- |
| Web | ASP.NET MVC 5 / .NET Framework 4.8.1 |
| UI | Razor views, Bootstrap, jQuery and JavaScript |
| Data | SQL Server, ADO.NET, stored procedures, and repositories in `DAL/` |
| Application logic | MVC controllers and focused services in `Services/` |
| Project | `FoodArrival.csproj` / `FoodArrival.slnx` |

## Architecture

```text
Browser (Razor pages, Bootstrap, jQuery/AJAX)
                 │ HTTP / MVC routes
                 ▼
Controllers (customer, food, orders, payment, tracking,
             restaurant, staff, rider, admin, home)
        ┌────────┴────────┐
        ▼                 ▼
Services              Repositories (DAL)
tracking / dispatch       │
        │                 ▼
        └──────────── SQL Server
                       stored procedures + tables
```

`Global.asax` registers routes, bundles, and global filters. Controllers coordinate requests and use session values for role-specific identity. Repositories map database results to the models and view models. `DbHelper` opens SQL connections and provides stored-procedure helpers. The rider allocation and tracking services contain dispatch and timeline logic. Razor views render the portals under `Views/`.

## Main user workflows

### Customer order

1. Browse food and restaurant listings.
2. Add menu items to the cart and submit delivery details.
3. Complete the payment simulation; payment and transaction data are recorded.
4. Follow order and restaurant sub-order progress in order history or tracking.
5. Provide the delivery OTP to the rider at handoff; rate the order when available.

### Restaurant and kitchen staff

1. Sign in to the appropriate portal.
2. Restaurant owners maintain menu availability, review incoming orders, and update preparation status.
3. Staff use their kitchen dashboard and related restaurant operations.
4. Ready sub-orders become candidates for rider dispatch.

### Rider delivery

1. Sign in and review assigned or available deliveries.
2. Pick up restaurant sub-orders in their assigned sequence.
3. Update delivery progress and confirm delivery with the customer OTP.

### Administration

1. Review platform metrics, orders, and transactions.
2. Manage approvals, riders, food items, and public bulletins.
3. Monitor operational status across customer, restaurant, and delivery workflows.

### Order lifecycle (conceptual)

```text
Cart → Order submitted → Payment recorded → Restaurant prepares
     → Ready for pickup → Rider assigned → Picked up
     → Out for delivery → OTP handoff → Delivered
```

The application also models multi-restaurant orders: one master order can have restaurant-specific sub-orders. Tracking combines order, sub-order, delivery, and status-history data. Exact transitions and allowed actions are enforced by the relevant controller and database routines.

## Important code locations

| Path | Responsibility |
| --- | --- |
| `Controllers/` | MVC endpoints and portal actions |
| `DAL/` | Database helper and repositories |
| `Services/RiderAllocationService.cs` | Rider candidate scoring, batching, and assignment |
| `Services/OrderTrackingService.cs` | Tracking view model, timeline, and delivery map data |
| `Models/` | Domain models and view models |
| `Views/` | Razor pages by user portal |
| `App_Start/` | Routing, bundles, and global filter setup |
| `FoodDeliveryDB_All_SQL.sql` | Consolidated database setup script in this repository |
| `Web.config` | Connection string and application configuration |

The separately maintained SQL scripts are in `D:\avi\sql\FoodReady\` (for example, `00_README_And_Master_Execution_Order.sql` through `14_OrderStatusHistory.sql`). Confirm that those scripts match the application version before applying them. The repository also contains the consolidated `FoodDeliveryDB_All_SQL.sql`.

## Local setup

1. Install Visual Studio with ASP.NET and web development support, .NET Framework 4.8.1 targeting pack, and SQL Server (LocalDB, Express, or Developer).
2. Create the `FoodArrivalDB` database using the consolidated SQL script or the matching ordered scripts from `D:\avi\sql\FoodReady\`.
3. Set the `FoodDeliveryDB` connection string in `Web.config` for your local SQL Server. The checked-in configuration includes a local fallback credential; replace it with a secure local setting and do not use that credential in a shared or production environment.
4. Open `FoodArrival.slnx` or `FoodArrival.csproj` in Visual Studio and restore NuGet packages.
5. Run with IIS Express and open the local URL shown by Visual Studio.

## Configuration and implementation notes

- The configured database name is `FoodArrivalDB`; the connection string entry is named `FoodDeliveryDB`.
- The database helper currently includes a fallback connection string in code. Remove embedded credentials and use a managed secret or secure deployment configuration before deployment.
- User identity is session based in the current controllers; review session timeout, authorization checks, and CSRF protections as part of any production hardening.
- Tracking and dispatch code contains fallback/demo coordinates and ETA calculations. Treat map positions and ETAs as illustrative unless connected to trusted live location and routing data.
- The payment gateway is a simulation. Do not treat it as a real payment processor.
- Do not rely on sample login credentials from older documentation; use the credentials configured in your local database.
