# Food Delivery Web App - Project Plan

## Project overview

Build a responsive food delivery website on the existing ASP.NET MVC 5 starter project. The project targets .NET Framework 4.8.1, so the implementation will keep that runtime and use MVC controllers, Razor views, view models, and a repository/data-access layer. SQL Server will own the relational data and stored procedures.

The supplied archives and PDF are examples for reference. Their specific setup steps, tables, and workflows are not requirements unless adopted in this plan.

## Account roles and registration

- Roles are normalized in `food.Roles` and `food.UserRoles`; a user has one active application role at a time. Customer, Staff, and Admin are the supported roles.
- Customer registration is self-service. A public Staff registration creates a customer account and a pending request in `food.AccountRoleRequests`; it does not grant staff permissions.
- Staff profiles distinguish restaurant staff from delivery agents. An administrator reviews and approves staff requests, assigns the staff type, and can later change roles or deactivate accounts.
- Admin is never offered on the public registration form. Bootstrap the first administrator using a database administrator connection; after that, admin access is assigned from the protected admin screen.

## Roles and journeys

### Customer

1. Browse and search active restaurants; open a restaurant to filter its available menu.
2. Add items and quantities to a cart; review prices and the order total.
3. Enter a delivery address and place an order.
4. View order history and the current order/delivery status; review an order after delivery.

### Restaurant

1. Maintain menu item descriptions, prices, and availability.
2. Review incoming orders belonging to that restaurant.
3. Move orders through accepted, preparing, ready, and rejected states.
4. View order history and customer-provided reviews.

### Delivery agent

1. View deliveries assigned to their account.
2. See pickup and delivery details for an active assignment.
3. Update delivery status through picked up and delivered, or report a failed delivery.

### Administrator

1. Manage account roles, restaurants, and food/menu items.
2. Accept or reject pending orders, review accepted/rejected queues, and assign delivery agents to ready orders.
3. Review operational orders and user/restaurant activity.

Pending orders are stored as `Placed` in the application schema. Opening or refreshing the admin dashboard runs `food.usp_Order_AutoReject`, which rejects only orders still in `Placed` state after five minutes. Admin order actions validate the Admin role and current order state in SQL. Admin menu management supports create, edit, and availability toggling. Items are disabled instead of physically deleted so prior order history remains intact.

## UI/UX direction

- Use a clean food-first visual style with warm accent colors, generous food imagery, and readable type. Keep the primary action visible on restaurant, menu, cart, and checkout screens.
- Use a responsive navigation bar and layouts that work on narrow mobile screens as well as desktop.
- Restaurant cards should show name, cuisine, rating, and a concise delivery estimate. Menu rows/cards should show item name, short description, price, availability, and a clear add control.
- Keep cart quantity changes and subtotal/total feedback immediate and understandable. Show delivery address and a final order summary before submission.
- Represent order and delivery states with both text and color; never use color alone. Provide clear labels, keyboard focus, validation messages, and confirmation for consequential actions.
- Include explicit loading, empty, success, and recoverable error states, especially for searches, empty carts, order history, and unavailable menu items.

## Architecture and technology

| Area | Choice |
| --- | --- |
| Web framework | ASP.NET MVC 5, .NET Framework 4.8.1 |
| UI | Razor views, Bootstrap 5, existing jQuery and unobtrusive validation assets |
| Language | C# |
| Database | Microsoft SQL Server |
| Database tooling | SQL Server Management Studio (SSMS); `FoodDeliveryDatabase.sql` for setup |
| IDE | Visual Studio with ASP.NET and web development workload |
| Application structure | Controllers coordinate requests; view models shape screen data; repository/data-access classes call parameterized stored procedures |

Keep SQL and connection handling out of controllers and views. Use parameterized `SqlCommand` calls with `CommandType.StoredProcedure` in repository classes. Keep passwords as one-way password hashes (never plain text); authorize every role-specific action on the server. Payment processing in this first version records a pending payment intent only; connect a real payment provider before accepting live payments.

## Database deliverable

`FoodDeliveryDatabase.sql` is a single SQL Server script. It creates the `FoodDeliveryDB` database if needed, then creates schema `food` with normalized `Roles` and `UserRoles`, `StaffProfiles`, pending `AccountRoleRequests`, users, restaurants, menu items, saved addresses, orders, order-item snapshots, payments, delivery assignments, and reviews. It defines keys, foreign keys, uniqueness, status/role checks, money and quantity checks, and common lookup indexes. Stored procedures are grouped after the tables. Optional demo records are in a separately marked, commented section at the end.

The script exposes procedures for restaurant search, menu browsing, menu-item add/update, menu availability, order placement, customer order history/details, restaurant order listing/status changes, delivery assignment/status changes, and order reviews. Multi-row order creation is transactional; item prices are read from the database rather than trusted from the browser. Procedure inputs are parameterized, ownership is checked in the procedure, and invalid operations raise SQL errors for the repository to handle.

`FoodDeliveryDB_All_SQL.sql` was provided as an additional example. It targets a separate `FoodArrivalDB` database with `dbo.Customers`/`dbo.FoodItems` tables and includes drop-and-recreate sections. Its five-minute rejection behavior and admin order/menu workflow have been incorporated into the existing `FoodDeliveryDB` / `food.*` design rather than swapping the application to an incompatible database contract.

## Build phases

1. **Database and repository foundation:** run the SQL script on a development SQL Server instance, set a local connection string, and add typed models/repositories for stored procedure results.
2. **Authentication and roles:** registration/login, hashed passwords, role authorization, and role-aware navigation.
3. **Restaurant and menu browsing:** restaurant search, restaurant detail, menu filters, and item availability.
4. **Cart and checkout:** server-side order validation, address capture, order creation, payment-pending record, order confirmation, and history.
5. **Restaurant and delivery operations:** restaurant order queue/status updates, admin assignment, and delivery agent workflow.
6. **Administration and reviews:** restaurant/user management, operational views, and eligible order reviews.
7. **Integration and acceptance:** verify complete journeys, error handling, accessibility, responsive behavior, and database/application authorization boundaries.

## Acceptance criteria

- A new development database can be initialized by running the SQL script in order; rerunning it does not duplicate schema objects or demo data.
- Customers can find active restaurants, browse available menus, place an order using current database prices, and view its status and history.
- Restaurants can only manage their own menu and orders; delivery agents can only update their own assignments; customers can only access their own orders and addresses.
- An order and its item snapshots are committed together or rolled back together. Invalid, unavailable, or foreign menu items cannot be used to place an order.
- Order status transitions and review eligibility are enforced server-side, with clear UI feedback for invalid or failed operations.
- Core screens work at mobile and desktop widths and provide accessible labels, text status indicators, validation feedback, and empty/error states.

## Local setup notes

1. Open `FoodArrival.slnx` in Visual Studio.
2. In SSMS, run `FoodDeliveryDatabase.sql` against a SQL Server instance. The optional demo section is commented out by default.
3. Configure a local `FoodDeliveryDB` connection string in the development environment; do not commit production credentials.
4. Register a customer account, bootstrap the first admin account as described below, then use the admin screen to approve staff requests and add restaurants.
5. Build and run using IIS Express. Online payment processing is not enabled; order payment records remain pending.

### First administrator bootstrap

After registering the first account, run this once in SSMS using a database administrator login, replacing the email with that account's address:

```sql
USE FoodDeliveryDB;
DECLARE @UserId INT = (SELECT UserId FROM food.Users WHERE Email = N'admin@example.com');
DECLARE @AdminRoleId SMALLINT = (SELECT RoleId FROM food.Roles WHERE RoleName = N'Admin');
IF @UserId IS NULL THROW 51090, 'Register the bootstrap account first.', 1;
BEGIN TRANSACTION;
UPDATE food.UserRoles SET IsActive = 0 WHERE UserId = @UserId AND IsActive = 1;
INSERT food.UserRoles(UserId, RoleId, IsActive) VALUES (@UserId, @AdminRoleId, 1);
COMMIT TRANSACTION;
```

The account can then sign in, approve Staff requests, manage user roles, create restaurants, and assign delivery agents through the Admin screen. Public registration does not create Admin accounts.

