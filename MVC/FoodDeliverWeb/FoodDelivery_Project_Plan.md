# Food Delivery Web — Project Plan

## 1. Purpose and scope

Build a food delivery web application with **ASP.NET MVC on .NET Framework**, using the supplied `MVConFramework.zip` as the application sample and `food_delivery_app.zip` as the database reference. The supplied material already points to a multi-role platform: customers browse restaurants and order; restaurants manage menus and fulfil orders; delivery agents handle assigned deliveries; administrators review operations, safety status, and platform analytics.

This document is a planning guide. SQL examples are reference patterns; align parameter names and columns with the final schema before running them.

## 2. What the samples contain

- **Application:** ASP.NET MVC 5, .NET Framework 4.8.1, Razor views, controllers for customers, restaurants, orders, stores, agents, and admin, plus repositories using ADO.NET `SqlConnection`/`SqlCommand`.
- **Database:** SQL Server `AssignmentDB`, `hra` schema, user/store/order tables plus menu items, order items, reviews, delivery agents, food-safety verification, notifications, and restaurant certificates.
- **Business behavior:** state-restricted restaurant browsing and order placement, a 7% platform fee, invoice numbers, notifications, order status transitions, reviews, agent enrollment, and operational reporting.
- **Existing UI direction:** customer restaurant search and city/state filters, restaurant menu and checkout, order history/tracking, restaurant menu/orders/invoices/reviews, agent dashboard, and admin reporting.

Treat SQL comments and seed records as sample design intent, not as production policy. Confirm fees, delivery rules, verification requirements, and role permissions with the project owner before launch.

## 3. Recommended stack and tools

| Area | Choice | Use |
|---|---|---|
| Web framework | ASP.NET MVC 5 on .NET Framework 4.8.1 | Match the supplied solution and its `System.Web.Mvc`/Razor structure. |
| Language | C# | Controllers, view models, services, and repository code. |
| Database | Microsoft SQL Server | Existing `AssignmentDB` scripts and stored procedures. |
| Data access | ADO.NET `SqlConnection`, `SqlCommand`, `CommandType.StoredProcedure` | Fits current repository implementation; use typed parameters and explicit timeouts. |
| Views and UI | Razor, Bootstrap (existing bundled Bootstrap), HTML/CSS, JavaScript | Responsive customer and role-based dashboards. |
| IDE | Visual Studio with ASP.NET/.NET Framework workload | Build, run, debug, and manage the MVC solution. |
| Database tooling | SQL Server Management Studio (SSMS) or Azure Data Studio | Apply schema/procedure scripts and inspect/query SQL Server. |
| Source control | Git | Track code and database script changes. |
| Local hosting | IIS Express; IIS for a Windows deployment | Match classic ASP.NET hosting requirements. |

This is a **legacy-compatible** choice because the supplied app is MVC 5 on .NET Framework. For a new greenfield product, evaluate ASP.NET Core MVC before committing; moving to it is a separate migration and changes hosting and project structure.

## 4. Architecture and request flow

```text
Browser (Razor views + Bootstrap)
  -> MVC Controller (HTTP validation, authorization, model binding)
  -> Service layer (business rules and transaction boundaries)
  -> Repository (ADO.NET, stored procedure calls)
  -> SQL Server (hra schema, constraints, indexes, procedures)
```

Keep controllers thin. Put role/business rules in services, parameterized database calls in repositories, and page-specific data in view models. Do not let views call repositories or put SQL in controllers. Use anti-forgery tokens on state-changing forms and server-side validation even when client validation is enabled.

## 5. Core roles and features

### Customer

- Choose delivery state and city; search restaurants and dishes; filter cuisine, rating, availability, and verified status.
- View restaurant profile, menu, item availability, prices, and food-safety status.
- Manage cart, checkout, delivery address, order confirmation, invoice/receipt, order history, and status tracking.
- Submit a review after an eligible completed order; receive notifications.

### Restaurant

- Manage profile, operating status, menu, prices, and item availability.
- View incoming orders and move them through accepted, preparing, ready for pickup, and completed/cancelled states according to an agreed state machine.
- View reviews and respond; view invoices and sales summaries; submit/renew certificates and food-safety documents.

### Delivery agent

- Enroll and maintain profile/availability; view assigned deliveries and delivery details.
- Update pickup/delivery progress; review completed deliveries and earnings.

### Administrator

- Manage users/roles, restaurant verification, agent enrollment, and safety audit records.
- Review orders, notifications, disputes, and reports; filter analytics by state, city, restaurant, and date range.

## 6. UI/UX plan

### Shared layout

- Responsive top navigation with brand, location selector, search, account menu, cart count, and notifications.
- Role-aware navigation after sign-in; clear active page state and consistent buttons, forms, badges, and alerts.
- Accessible labels, keyboard focus, sufficient contrast, meaningful image alt text, and validation messages next to fields.
- Loading, empty, success, and error states for each important page; do not rely on color alone for order status.

### Customer journey

1. **Discover:** location/state selector first; prominent restaurant/dish search; cuisine chips and restaurant cards with delivery estimate, rating, price range, and verification badge.
2. **Restaurant/menu:** clear categories, item descriptions/prices, dietary/allergen notes where available, quantity controls, and a persistent cart summary on larger screens.
3. **Checkout:** confirm delivery address and same-state availability; show item subtotal, fees, taxes (if applicable), discounts, and total before placing order.
4. **Confirmation/tracking:** order number, receipt, clear status timeline, estimated time, restaurant/agent contact rules, and help entry point.

### Role dashboards

- **Restaurant:** incoming order queue first, status actions with confirmation, menu management, sales/invoices, reviews, and verification status.
- **Agent:** availability toggle, assigned delivery queue, pickup/drop-off address and contact details, status action, and earnings summary.
- **Admin:** summary cards plus filterable tables and detail views; distinguish pending, approved, rejected, and expired verification records.

Use mobile-first layouts. Avoid claiming “live” tracking, fixed delivery times, or verified safety unless the system actually provides that evidence; show estimates and audit dates with clear meaning.

## 7. Database and stored procedure plan

Use schema `hra` consistently (the sample repository calls procedures such as `hra.usp_GetStoresByCityOrSearch`). Standardize names as `hra.usp_<Verb><Entity>`. Use `SET NOCOUNT ON`, typed parameters, `TRY/CATCH`, deterministic ordering, pagination for lists, and explicit transactions for multi-table writes. Prefer constraints/foreign keys and unique indexes for invariants. Never concatenate user input into SQL.

### Procedure inventory

| Area | Stored procedure responsibilities |
|---|---|
| Discovery | `usp_GetStoresByCityOrSearch`, `usp_GetStoreDetailsWithMenu`, `usp_GetMenuItemsByStore` |
| Menu | `usp_AddOrUpdateMenuItem`, `usp_DeleteMenuItem` (prefer soft disable if order history references an item) |
| Orders | `usp_PlaceCustomerOrder`, `usp_GetCustomerOrders`, `usp_GetOrderDetails`, `usp_UpdateOrderStatus`, `usp_CancelOrder` |
| Restaurant | `usp_GetRestaurantOrders`, `usp_GetRestaurantInvoicesAndSales`, `usp_GetRestaurantCertificate` |
| Delivery | `usp_RegisterDeliveryAgent`, `usp_ToggleDeliveryAgentStatus`, `usp_GetDeliveryAgentDashboard`, `usp_AssignDeliveryAgent` |
| Reviews | `usp_AddRestaurantReview`, `usp_RespondToReview` |
| Safety/admin | `usp_VerifyRestaurantFoodSafety`, `usp_GetAdminCityAnalytics`, `usp_GetNotifications`, `usp_MarkNotificationRead` |

Confirm the actual baseline schema first. The supplied scripts include incremental `ALTER TABLE` operations; make deployment scripts repeatable or track applied migrations to avoid duplicate-column failures.

### Single-page SQL reference: order placement pattern

The following is a structured procedure template for the most important write path. It assumes `hra.MenuItems` has `MenuItemId`, `StoreId`, `ItemName`, `Price`, `IsAvailable`, and that `hra.Orders`/`hra.OrderItems` have the indicated columns. Adapt names and required legacy columns to the actual schema before execution. The application should send a table-valued parameter, not a comma-separated list of IDs.

```sql
-- One-time type creation (deploy before the procedure).
CREATE TYPE hra.OrderLineInput AS TABLE
(
    MenuItemId INT NOT NULL,
    Quantity   INT NOT NULL CHECK (Quantity > 0),
    PRIMARY KEY (MenuItemId)
);
GO

CREATE OR ALTER PROCEDURE hra.usp_PlaceCustomerOrder
    @CustomerId       INT,
    @DeliveryAddress  NVARCHAR(250),
    @DeliveryState    NVARCHAR(100),
    @Lines            hra.OrderLineInput READONLY,
    @OrderId          INT OUTPUT,
    @InvoiceNumber    NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @CustomerId IS NULL OR @CustomerId <= 0
        THROW 50001, 'A valid customer is required.', 1;
    IF NULLIF(LTRIM(RTRIM(@DeliveryAddress)), N'') IS NULL
        THROW 50002, 'Delivery address is required.', 1;
    IF NOT EXISTS (SELECT 1 FROM @Lines)
        THROW 50003, 'The order must contain at least one item.', 1;

    DECLARE @StoreId INT,
            @Subtotal DECIMAL(12,2),
            @PlatformFee DECIMAL(12,2),
            @OrderDate DATETIME2(0) = SYSUTCDATETIME();

    BEGIN TRY
        BEGIN TRANSACTION;

        -- A single restaurant per order; lock relevant menu rows while validating/pricing.
        SELECT @StoreId = MIN(mi.StoreId)
        FROM @Lines AS l
        JOIN hra.MenuItems AS mi WITH (UPDLOCK, HOLDLOCK)
          ON mi.MenuItemId = l.MenuItemId;

        IF @StoreId IS NULL
           OR EXISTS (
                SELECT 1
                FROM @Lines AS l
                LEFT JOIN hra.MenuItems AS mi
                  ON mi.MenuItemId = l.MenuItemId
                WHERE mi.MenuItemId IS NULL
                   OR mi.StoreId <> @StoreId
                   OR mi.IsAvailable = 0
                   OR l.Quantity <= 0
           )
            THROW 50004, 'One or more items are invalid, unavailable, or from another restaurant.', 1;

        -- Enforce the state rule in the database, not only in the browser.
        IF NOT EXISTS
        (
            SELECT 1
            FROM hra.Stores AS s WITH (UPDLOCK, HOLDLOCK)
            WHERE s.StoreId = @StoreId
              AND s.IsActive = 1
              AND s.IsVerified = 1
              AND UPPER(LTRIM(RTRIM(s.State))) = UPPER(LTRIM(RTRIM(@DeliveryState)))
        )
            THROW 50005, 'Restaurant is inactive, unverified, or outside the delivery state.', 1;

        SELECT @Subtotal = SUM(CONVERT(DECIMAL(12,2), mi.Price) * l.Quantity)
        FROM @Lines AS l
        JOIN hra.MenuItems AS mi ON mi.MenuItemId = l.MenuItemId;

        -- Example policy from the supplied sample. Make this configurable if business policy may change.
        SET @PlatformFee = ROUND(@Subtotal * 0.07, 2);
        SET @InvoiceNumber = CONCAT('FD-', CONVERT(CHAR(8), @OrderDate, 112), '-',
                                    RIGHT(CONCAT('00000000', CONVERT(VARCHAR(8), ABS(CHECKSUM(NEWID())))), 8));

        INSERT INTO hra.Orders
            (CustomerId, StoreId, CreatedAt, OrderStatus, DeliveryAddress,
             DeliveryState, InvoiceNumber, PlatformFee)
        VALUES
            (@CustomerId, @StoreId, @OrderDate, N'Placed', @DeliveryAddress,
             @DeliveryState, @InvoiceNumber, @PlatformFee);

        SET @OrderId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO hra.OrderItems (OrderId, MenuItemId, Quantity, UnitPrice)
        SELECT @OrderId, mi.MenuItemId, l.Quantity, mi.Price
        FROM @Lines AS l
        JOIN hra.MenuItems AS mi ON mi.MenuItemId = l.MenuItemId;

        -- Add notification rows here using the actual notification schema.
        -- Never generate a physical distance or promise live tracking unless integrated data supports it.

        COMMIT TRANSACTION;

        SELECT @OrderId AS OrderId, @InvoiceNumber AS InvoiceNumber,
               @Subtotal AS Subtotal, @PlatformFee AS PlatformFee,
               @Subtotal + @PlatformFee AS AmountBeforeTaxAndOtherFees;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO
```

**Important adjustments before using this template:** the sample SQL may have different order/item column names or mandatory columns; inspect the table definitions and align the `INSERT` lists. The sample calculates 7% platform fee and uses state restriction. Decide whether the fee applies to food subtotal or another base, and add delivery fee, tax, discount, and rounding policy before calculating the customer total. Use a database sequence or a unique invoice table/index in production; the illustrative random invoice suffix is not a guaranteed uniqueness mechanism. Add idempotency protection so a browser retry cannot create duplicate orders. Use a genuine agent availability/assignment flow instead of assuming every agent is free.

### SQL design rules

- Use `DECIMAL`, never `FLOAT`, for monetary amounts; snapshot item unit price into `OrderItems` when placing an order.
- Add foreign keys, check constraints, unique invoice numbers, and indexes for common filters (store/state/city, customer/date, agent/status, notification/user/read state).
- Use UTC timestamps consistently and convert to local time at presentation. Add `rowversion` or equivalent concurrency checks for status updates.
- Define legal order status transitions and authorized actors; reject invalid transitions in the service and database write procedure.
- Return a stable result shape; avoid `SELECT *`. Use explicit columns and paging for large lists.
- Parameterize all values from C#; pass `SqlDbType` and size explicitly. Avoid `AddWithValue` type inference.
- Keep seed data out of production and do not use random distance or sample credentials as operational data.

## 8. Security, configuration, and reliability

- The sample `Web.config` contains a SQL Server `sa` username/password. Treat it as exposed sample credentials: rotate it, remove it from source control, and use a least-privilege SQL login or Windows authentication. Store deployment secrets in protected environment configuration.
- Change `customErrors` from `Off` for production; do not expose stack traces or connection errors to users.
- Enforce role and ownership checks for every order, restaurant, and agent record. Hiding a link is not authorization.
- Hash passwords with a vetted password hasher; never store or compare plaintext passwords. Protect cookies, require HTTPS, and apply anti-forgery validation.
- Validate uploads and restrict file type/size for certificates/images. Log security-relevant actions without storing passwords, tokens, or payment data.
- If payments are added, use a payment provider and never store raw card numbers or CVV.
- Add health checks/logging, structured exception handling, and backups/restore practice appropriate to deployment.

## 9. Delivery phases

1. **Baseline and cleanup:** open/build the supplied solution; inspect actual table definitions, key relationships, stored procedure signatures, and routes; remove binaries and local IDE data from source control; move secrets out of `Web.config`.
2. **Schema foundation:** reconcile a clean database install path; add constraints/indexes; define role and order status vocabularies; seed development-only data.
3. **Customer MVP:** sign-up/sign-in, location selection, restaurant/menu search, cart/checkout, atomic order placement, receipt, order history, and state-safe order lookup.
4. **Restaurant operations:** menu CRUD, incoming order queue, validated status changes, reviews, invoice/sales view, and verification status.
5. **Delivery and admin:** agent onboarding/availability/assignment, delivery dashboard, admin verification, notifications, and filtered reports.
6. **Polish and release:** responsive/accessibility pass, security/configuration hardening, logging, deployment setup, and documented rollback/backup procedure.

## 10. Acceptance checklist

- Customer cannot browse/order from an out-of-state restaurant; server/database reject tampered requests.
- Checkout validates availability and prices under transaction and produces one order with item-price snapshots and a unique invoice.
- Only authorized owners/roles can view or change a given order/menu/agent record.
- Order state changes follow approved transitions and notify the intended audience.
- Empty/error/loading states and form validation are understandable on mobile and desktop.
- Production configuration contains no sample credentials or debug error pages.
- Database install/migration scripts can be applied predictably to a clean environment.

## 11. Suggested repository organization

```text
FoodDeliverWeb/
  Controllers/             # HTTP endpoints and page orchestration
  Models/                  # Domain/data models
  ViewModels/               # Page-specific input/output models
  Services/                 # Business rules and transactions
  Data/                      # ADO.NET repositories and SQL parameter helpers
  Views/                     # Razor pages by feature/role
  Content/                   # CSS and static assets
  Scripts/                   # JavaScript
  Database/
    Migrations/              # Ordered schema changes
    Procedures/              # Stored procedure scripts
    Seed/                    # Development-only seed data
  Docs/
```

The supplied project currently combines some repository concerns in `Models`; moving responsibilities can be incremental and should not block an MVP.
