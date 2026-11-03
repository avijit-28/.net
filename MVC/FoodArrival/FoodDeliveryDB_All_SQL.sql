/*
===========================================================
 FOODREADY - COMPLETE FOOD DELIVERY APPLICATION DATABASE SCRIPT
 ASP.NET MVC 5 / .NET Framework 4.8.1
 SQL Server + ADO.NET + Stored Procedures

 DATABASE:
     FoodArrivalDB

 4 APPLICATION ROLES:
     1. Admin
     2. Restaurant (Kitchen Dispatch)
     3. Rider (Delivery Partner)
     4. Customer
===========================================================
*/

------------------------------------------------------------
-- 1. CREATE DATABASE
------------------------------------------------------------
IF DB_ID('FoodArrivalDB') IS NULL
BEGIN
    CREATE DATABASE FoodArrivalDB;
END
GO

USE FoodArrivalDB;
GO

------------------------------------------------------------
-- 2. DROP EXISTING PROCEDURES
------------------------------------------------------------
IF OBJECT_ID('dbo.sp_Order_AutoReject', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_AutoReject;
IF OBJECT_ID('dbo.sp_Order_Reject', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_Reject;
IF OBJECT_ID('dbo.sp_Order_Accept', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_Accept;
IF OBJECT_ID('dbo.sp_Order_GetTodayRejected', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetTodayRejected;
IF OBJECT_ID('dbo.sp_Order_GetTodayAccepted', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetTodayAccepted;
IF OBJECT_ID('dbo.sp_Order_GetTodayPending', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetTodayPending;
IF OBJECT_ID('dbo.sp_Order_GetRejected', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetRejected;
IF OBJECT_ID('dbo.sp_Order_GetAccepted', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetAccepted;
IF OBJECT_ID('dbo.sp_Order_GetPending', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetPending;
IF OBJECT_ID('dbo.sp_Order_GetDetails', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetDetails;
IF OBJECT_ID('dbo.sp_Order_GetByCustomer', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetByCustomer;
IF OBJECT_ID('dbo.sp_Order_CreateMultiple', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_CreateMultiple;
IF OBJECT_ID('dbo.sp_Order_Create', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_Create;
IF OBJECT_ID('dbo.sp_Admin_DashboardSummary', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_DashboardSummary;
IF OBJECT_ID('dbo.sp_Admin_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_Login;
IF OBJECT_ID('dbo.sp_FoodItem_Search', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_Search;
IF OBJECT_ID('dbo.sp_FoodItem_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_Delete;
IF OBJECT_ID('dbo.sp_FoodItem_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_Update;
IF OBJECT_ID('dbo.sp_FoodItem_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_GetById;
IF OBJECT_ID('dbo.sp_FoodItem_GetAvailable', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_GetAvailable;
IF OBJECT_ID('dbo.sp_FoodItem_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_GetAll;
IF OBJECT_ID('dbo.sp_FoodItem_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_Insert;
IF OBJECT_ID('dbo.sp_Customer_Delete', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_Delete;
IF OBJECT_ID('dbo.sp_Customer_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_Update;
IF OBJECT_ID('dbo.sp_Customer_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_GetById;
IF OBJECT_ID('dbo.sp_Customer_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_GetAll;
IF OBJECT_ID('dbo.sp_Customer_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_Login;
IF OBJECT_ID('dbo.sp_Customer_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_Insert;
GO

------------------------------------------------------------
-- 3. DROP TABLES IF THEY EXIST (Drop Child Tables First)
------------------------------------------------------------
IF OBJECT_ID('dbo.Notifications', 'U') IS NOT NULL DROP TABLE dbo.Notifications;
IF OBJECT_ID('dbo.OrderDetails', 'U') IS NOT NULL DROP TABLE dbo.OrderDetails;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.FoodItems', 'U') IS NOT NULL DROP TABLE dbo.FoodItems;
IF OBJECT_ID('dbo.Riders', 'U') IS NOT NULL DROP TABLE dbo.Riders;
IF OBJECT_ID('dbo.Restaurants', 'U') IS NOT NULL DROP TABLE dbo.Restaurants;
IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Admins', 'U') IS NOT NULL DROP TABLE dbo.Admins;
GO

------------------------------------------------------------
-- 4. DROP TYPE IF EXISTS
------------------------------------------------------------
IF TYPE_ID('dbo.OrderItemType') IS NOT NULL DROP TYPE dbo.OrderItemType;
GO

------------------------------------------------------------
-- 5. CREATE TABLES FOR 4 ROLES
------------------------------------------------------------

-- ROLE 1: CUSTOMERS
CREATE TABLE dbo.Customers
(
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ROLE 2: ADMINS
CREATE TABLE dbo.Admins
(
    AdminId INT IDENTITY(1,1) PRIMARY KEY,
    AdminName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ROLE 3: RESTAURANTS
CREATE TABLE dbo.Restaurants
(
    RestaurantId INT IDENTITY(1,1) PRIMARY KEY,
    RestaurantName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    Address NVARCHAR(500) NULL,
    Cuisine NVARCHAR(150) NULL,
    Rating DECIMAL(2,1) NOT NULL DEFAULT 4.5,
    ImageUrl NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- ROLE 4: RIDERS
CREATE TABLE dbo.Riders
(
    RiderId INT IDENTITY(1,1) PRIMARY KEY,
    RiderName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,
    VehicleType NVARCHAR(100) NOT NULL DEFAULT 'Motorbike',
    CurrentStatus NVARCHAR(50) NOT NULL DEFAULT 'Available',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

-- FOOD ITEMS TABLE (WITH CATEGORY)
CREATE TABLE dbo.FoodItems
(
    FoodItemId INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    Price DECIMAL(10,2) NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    Category NVARCHAR(100) NOT NULL DEFAULT 'Snacks',
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT CK_FoodItems_Price CHECK (Price >= 0)
);
GO

-- ORDERS TABLE
CREATE TABLE dbo.Orders
(
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    OrderStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    OrderedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    ProcessedAt DATETIME2 NULL,
    ProcessedBy INT NULL,
    RejectionReason NVARCHAR(500) NULL,
    RestaurantId INT NULL,
    RiderId INT NULL,
    DeliveryStatus NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers(CustomerId),
    CONSTRAINT FK_Orders_Admins FOREIGN KEY (ProcessedBy) REFERENCES dbo.Admins(AdminId),
    CONSTRAINT FK_Orders_Restaurants FOREIGN KEY (RestaurantId) REFERENCES dbo.Restaurants(RestaurantId),
    CONSTRAINT FK_Orders_Riders FOREIGN KEY (RiderId) REFERENCES dbo.Riders(RiderId),
    CONSTRAINT CK_Orders_Status CHECK (OrderStatus IN ('Pending', 'Accepted', 'Rejected')),
    CONSTRAINT CK_Orders_Total CHECK (TotalAmount >= 0)
);
GO

-- ORDER DETAILS TABLE
CREATE TABLE dbo.OrderDetails
(
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId INT NOT NULL,
    FoodItemId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    SubTotal AS (Quantity * UnitPrice) PERSISTED,
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(OrderId),
    CONSTRAINT FK_OrderDetails_FoodItems FOREIGN KEY (FoodItemId) REFERENCES dbo.FoodItems(FoodItemId),
    CONSTRAINT CK_OrderDetails_Quantity CHECK (Quantity > 0),
    CONSTRAINT CK_OrderDetails_UnitPrice CHECK (UnitPrice >= 0)
);
GO

-- NOTIFICATIONS TABLE
CREATE TABLE dbo.Notifications
(
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,
    TargetRole NVARCHAR(50) NOT NULL,
    TargetUserId INT NULL,
    Title NVARCHAR(150) NOT NULL,
    Message NVARCHAR(500) NOT NULL,
    NotificationType NVARCHAR(50) NOT NULL DEFAULT 'Info',
    IsRead BIT NOT NULL DEFAULT 0,
    TargetUrl NVARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);
GO

------------------------------------------------------------
-- 6. INDEXES
------------------------------------------------------------
CREATE INDEX IX_Orders_CustomerId ON dbo.Orders(CustomerId);
CREATE INDEX IX_Orders_Status ON dbo.Orders(OrderStatus);
CREATE INDEX IX_Orders_OrderedAt ON dbo.Orders(OrderedAt);
CREATE INDEX IX_OrderDetails_OrderId ON dbo.OrderDetails(OrderId);
GO

------------------------------------------------------------
-- 7. TABLE TYPE FOR MULTI-ITEM CHECKOUT
------------------------------------------------------------
CREATE TYPE dbo.OrderItemType AS TABLE
(
    FoodItemId INT NOT NULL,
    Quantity INT NOT NULL
);
GO

------------------------------------------------------------
-- 8. CUSTOMER PROCEDURES
------------------------------------------------------------
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE PROCEDURE dbo.sp_Customer_Insert
(
    @CustomerName NVARCHAR(150),
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(255),
    @PhoneNumber NVARCHAR(20),
    @Address NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.Customers WHERE Email = @Email)
    BEGIN
        SELECT 0 AS Result, 'Email already exists.' AS Message, NULL AS CustomerId;
        RETURN;
    END;

    INSERT INTO dbo.Customers (CustomerName, Email, PasswordHash, PhoneNumber, Address)
    VALUES (@CustomerName, @Email, @PasswordHash, @PhoneNumber, @Address);

    SELECT 1 AS Result, 'Customer registered successfully.' AS Message, CAST(SCOPE_IDENTITY() AS INT) AS CustomerId;
END;
GO

CREATE PROCEDURE dbo.sp_Customer_Login
(
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, CustomerName, Email, PhoneNumber, Address
    FROM dbo.Customers
    WHERE Email = @Email AND PasswordHash = @PasswordHash AND IsActive = 1;
END;
GO

CREATE PROCEDURE dbo.sp_Customer_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, CustomerName, Email, PhoneNumber, Address, IsActive, CreatedAt
    FROM dbo.Customers
    ORDER BY CustomerId DESC;
END;
GO

CREATE PROCEDURE dbo.sp_Customer_GetById
(
    @CustomerId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CustomerId, CustomerName, Email, PhoneNumber, Address, IsActive, CreatedAt
    FROM dbo.Customers
    WHERE CustomerId = @CustomerId;
END;
GO

CREATE PROCEDURE dbo.sp_Customer_Update
(
    @CustomerId INT,
    @CustomerName NVARCHAR(150),
    @PhoneNumber NVARCHAR(20),
    @Address NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customers
    SET CustomerName = @CustomerName, PhoneNumber = @PhoneNumber, Address = @Address
    WHERE CustomerId = @CustomerId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Result, 'Customer not found.' AS Message;
        RETURN;
    END;

    SELECT 1 AS Result, 'Customer updated successfully.' AS Message;
END;
GO

CREATE PROCEDURE dbo.sp_Customer_Delete
(
    @CustomerId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customers SET IsActive = 0 WHERE CustomerId = @CustomerId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Result, 'Customer not found.' AS Message;
        RETURN;
    END;

    SELECT 1 AS Result, 'Customer deactivated successfully.' AS Message;
END;
GO

------------------------------------------------------------
-- 9. FOOD ITEM PROCEDURES
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_Insert
(
    @ItemName NVARCHAR(150),
    @Description NVARCHAR(500),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(500),
    @IsAvailable BIT,
    @Category NVARCHAR(100) = 'Snacks'
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Price < 0
    BEGIN
        SELECT 0 AS Result, 'Price cannot be negative.' AS Message, NULL AS FoodItemId;
        RETURN;
    END;

    INSERT INTO dbo.FoodItems (ItemName, Description, Price, ImageUrl, IsAvailable, Category)
    VALUES (@ItemName, @Description, @Price, @ImageUrl, @IsAvailable, ISNULL(@Category, 'Snacks'));

    SELECT 1 AS Result, 'Food item inserted successfully.' AS Message, CAST(SCOPE_IDENTITY() AS INT) AS FoodItemId;
END;
GO

CREATE PROCEDURE dbo.sp_FoodItem_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT FoodItemId, ItemName, Description, Price, ImageUrl, Category, IsAvailable, CreatedAt, UpdatedAt
    FROM dbo.FoodItems
    ORDER BY FoodItemId DESC;
END;
GO

CREATE PROCEDURE dbo.sp_FoodItem_GetAvailable
AS
BEGIN
    SET NOCOUNT ON;

    SELECT FoodItemId, ItemName, Description, Price, ImageUrl, Category, IsAvailable
    FROM dbo.FoodItems
    WHERE IsAvailable = 1
    ORDER BY ItemName;
END;
GO

CREATE PROCEDURE dbo.sp_FoodItem_GetById
(
    @FoodItemId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT FoodItemId, ItemName, Description, Price, ImageUrl, Category, IsAvailable, CreatedAt, UpdatedAt
    FROM dbo.FoodItems
    WHERE FoodItemId = @FoodItemId;
END;
GO

CREATE PROCEDURE dbo.sp_FoodItem_Update
(
    @FoodItemId INT,
    @ItemName NVARCHAR(150),
    @Description NVARCHAR(500),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(500),
    @IsAvailable BIT,
    @Category NVARCHAR(100) = 'Snacks'
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Price < 0
    BEGIN
        SELECT 0 AS Result, 'Price cannot be negative.' AS Message;
        RETURN;
    END;

    UPDATE dbo.FoodItems
    SET ItemName = @ItemName,
        Description = @Description,
        Price = @Price,
        ImageUrl = @ImageUrl,
        IsAvailable = @IsAvailable,
        Category = ISNULL(@Category, 'Snacks'),
        UpdatedAt = GETDATE()
    WHERE FoodItemId = @FoodItemId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Result, 'Food item not found.' AS Message;
        RETURN;
    END;

    SELECT 1 AS Result, 'Food item updated successfully.' AS Message;
END;
GO

CREATE PROCEDURE dbo.sp_FoodItem_Delete
(
    @FoodItemId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.FoodItems SET IsAvailable = 0, UpdatedAt = GETDATE() WHERE FoodItemId = @FoodItemId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Result, 'Food item not found.' AS Message;
        RETURN;
    END;

    SELECT 1 AS Result, 'Food item removed successfully.' AS Message;
END;
GO

CREATE PROCEDURE dbo.sp_FoodItem_Search
(
    @Search NVARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT FoodItemId, ItemName, Description, Price, ImageUrl, Category, IsAvailable
    FROM dbo.FoodItems
    WHERE IsAvailable = 1
      AND (ItemName LIKE '%' + @Search + '%' OR Description LIKE '%' + @Search + '%' OR Category LIKE '%' + @Search + '%')
    ORDER BY ItemName;
END;
GO

------------------------------------------------------------
-- 10. ADMIN PROCEDURES
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Admin_Login
(
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AdminId, AdminName, Email
    FROM dbo.Admins
    WHERE Email = @Email AND PasswordHash = @PasswordHash AND IsActive = 1;
END;
GO

CREATE PROCEDURE dbo.sp_Admin_DashboardSummary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Pending') AS PendingOrders,
        (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Accepted') AS AcceptedOrders,
        (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Rejected') AS RejectedOrders,
        (SELECT COUNT(*) FROM dbo.FoodItems WHERE IsAvailable = 1) AS AvailableFoodItems,
        (SELECT COUNT(*) FROM dbo.Customers WHERE IsActive = 1) AS ActiveCustomers;
END;
GO

------------------------------------------------------------
-- 11. ORDER PROCEDURES
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Order_Create
(
    @CustomerId INT,
    @FoodItemId INT,
    @Quantity INT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @UnitPrice DECIMAL(10,2);
        DECLARE @OrderId INT;

        IF @Quantity <= 0
        BEGIN
            RAISERROR('Quantity must be greater than zero.', 16, 1);
        END;

        SELECT @UnitPrice = Price FROM dbo.FoodItems WHERE FoodItemId = @FoodItemId AND IsAvailable = 1;
        IF @UnitPrice IS NULL
        BEGIN
            RAISERROR('Food item is not available.', 16, 1);
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerId = @CustomerId AND IsActive = 1)
        BEGIN
            RAISERROR('Customer is not active or does not exist.', 16, 1);
        END;

        INSERT INTO dbo.Orders (CustomerId, TotalAmount, OrderStatus, RestaurantId, DeliveryStatus)
        VALUES (@CustomerId, 0, 'Pending', 1, 'Pending');

        SET @OrderId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.OrderDetails (OrderId, FoodItemId, Quantity, UnitPrice)
        VALUES (@OrderId, @FoodItemId, @Quantity, @UnitPrice);

        UPDATE dbo.Orders
        SET TotalAmount = (SELECT SUM(SubTotal) FROM dbo.OrderDetails WHERE OrderId = @OrderId)
        WHERE OrderId = @OrderId;

        COMMIT TRANSACTION;

        SELECT 1 AS Result, @OrderId AS OrderId, 'Order placed successfully.' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE PROCEDURE dbo.sp_Order_CreateMultiple
(
    @CustomerId INT,
    @Items dbo.OrderItemType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @OrderId INT;

        IF NOT EXISTS (SELECT 1 FROM @Items WHERE Quantity > 0)
        BEGIN
            RAISERROR('Order must contain at least one item.', 16, 1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @Items i
            LEFT JOIN dbo.FoodItems f ON i.FoodItemId = f.FoodItemId
            WHERE f.FoodItemId IS NULL OR f.IsAvailable = 0 OR i.Quantity <= 0
        )
        BEGIN
            RAISERROR('One or more food items are unavailable.', 16, 1);
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.Customers WHERE CustomerId = @CustomerId AND IsActive = 1)
        BEGIN
            RAISERROR('Customer is not active or does not exist.', 16, 1);
        END;

        INSERT INTO dbo.Orders (CustomerId, TotalAmount, OrderStatus, RestaurantId, DeliveryStatus)
        VALUES (@CustomerId, 0, 'Pending', 1, 'Pending');

        SET @OrderId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.OrderDetails (OrderId, FoodItemId, Quantity, UnitPrice)
        SELECT @OrderId, i.FoodItemId, i.Quantity, f.Price
        FROM @Items i
        INNER JOIN dbo.FoodItems f ON i.FoodItemId = f.FoodItemId;

        UPDATE dbo.Orders
        SET TotalAmount = (SELECT SUM(SubTotal) FROM dbo.OrderDetails WHERE OrderId = @OrderId)
        WHERE OrderId = @OrderId;

        COMMIT TRANSACTION;

        SELECT 1 AS Result, @OrderId AS OrderId, 'Order placed successfully.' AS Message;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetByCustomer
(
    @CustomerId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT o.OrderId, o.CustomerId, o.TotalAmount, o.OrderStatus, o.OrderedAt, o.ProcessedAt
    FROM dbo.Orders o
    WHERE o.CustomerId = @CustomerId
    ORDER BY o.OrderedAt DESC;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetDetails
(
    @OrderId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        c.CustomerId,
        c.CustomerName,
        c.Email,
        c.PhoneNumber,
        c.Address,
        o.TotalAmount,
        o.OrderStatus,
        o.OrderedAt,
        o.ProcessedAt,
        o.RejectionReason,
        f.FoodItemId,
        f.ItemName,
        od.Quantity,
        od.UnitPrice,
        od.SubTotal
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    INNER JOIN dbo.OrderDetails od ON o.OrderId = od.OrderId
    INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
    WHERE o.OrderId = @OrderId
    ORDER BY od.OrderDetailId;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetPending
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        c.CustomerId,
        c.CustomerName,
        c.Email,
        o.TotalAmount,
        o.OrderedAt,
        o.OrderStatus,
        STUFF
        (
            (
                SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Pending'
    ORDER BY o.OrderedAt DESC;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetAccepted
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        c.CustomerId,
        c.CustomerName,
        c.Email,
        o.TotalAmount,
        o.OrderedAt,
        o.ProcessedAt,
        o.OrderStatus,
        STUFF
        (
            (
                SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Accepted'
    ORDER BY o.ProcessedAt DESC;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetRejected
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        c.CustomerId,
        c.CustomerName,
        c.Email,
        o.TotalAmount,
        o.OrderedAt,
        o.ProcessedAt,
        o.RejectionReason,
        o.OrderStatus,
        STUFF
        (
            (
                SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Rejected'
    ORDER BY o.ProcessedAt DESC;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetTodayPending
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        c.CustomerId,
        c.CustomerName,
        c.Email,
        o.TotalAmount,
        o.OrderedAt,
        o.OrderStatus,
        STUFF
        (
            (
                SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Pending'
      AND o.OrderedAt >= CAST(GETDATE() AS DATE)
      AND o.OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
    ORDER BY o.OrderedAt ASC;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetTodayAccepted
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        c.CustomerId,
        c.CustomerName,
        c.Email,
        o.TotalAmount,
        o.OrderedAt,
        o.ProcessedAt,
        o.OrderStatus,
        STUFF
        (
            (
                SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Accepted'
      AND o.OrderedAt >= CAST(GETDATE() AS DATE)
      AND o.OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
    ORDER BY o.ProcessedAt DESC;
END;
GO

CREATE PROCEDURE dbo.sp_Order_GetTodayRejected
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        c.CustomerId,
        c.CustomerName,
        c.Email,
        o.TotalAmount,
        o.OrderedAt,
        o.ProcessedAt,
        o.RejectionReason,
        o.OrderStatus,
        STUFF
        (
            (
                SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'), 1, 2, ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Rejected'
      AND o.OrderedAt >= CAST(GETDATE() AS DATE)
      AND o.OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
    ORDER BY o.ProcessedAt DESC;
END;
GO

CREATE PROCEDURE dbo.sp_Order_Accept
(
    @OrderId INT,
    @AdminId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Admins WHERE AdminId = @AdminId AND IsActive = 1)
    BEGIN
        SELECT 0 AS Result, 'Invalid admin.' AS Message;
        RETURN;
    END;

    UPDATE dbo.Orders
    SET OrderStatus = 'Accepted',
        ProcessedAt = GETDATE(),
        ProcessedBy = @AdminId,
        RejectionReason = NULL,
        DeliveryStatus = 'Preparing'
    WHERE OrderId = @OrderId AND OrderStatus = 'Pending';

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Result, 'Order cannot be accepted. It may not exist or is no longer pending.' AS Message;
        RETURN;
    END;

    SELECT 1 AS Result, 'Order accepted successfully.' AS Message;
END;
GO

CREATE PROCEDURE dbo.sp_Order_Reject
(
    @OrderId INT,
    @AdminId INT,
    @RejectionReason NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Admins WHERE AdminId = @AdminId AND IsActive = 1)
    BEGIN
        SELECT 0 AS Result, 'Invalid admin.' AS Message;
        RETURN;
    END;

    UPDATE dbo.Orders
    SET OrderStatus = 'Rejected',
        ProcessedAt = GETDATE(),
        ProcessedBy = @AdminId,
        RejectionReason = ISNULL(NULLIF(@RejectionReason, ''), 'Order rejected by administrator.')
    WHERE OrderId = @OrderId AND OrderStatus = 'Pending';

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT 0 AS Result, 'Order cannot be rejected. It may not exist or is no longer pending.' AS Message;
        RETURN;
    END;

    SELECT 1 AS Result, 'Order rejected successfully.' AS Message;
END;
GO

CREATE PROCEDURE dbo.sp_Order_AutoReject
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Orders
    SET OrderStatus = 'Rejected',
        ProcessedAt = GETDATE(),
        RejectionReason = 'Automatically rejected after 5 minutes.'
    WHERE OrderStatus = 'Pending'
      AND DATEDIFF(SECOND, OrderedAt, GETDATE()) >= 300;

    SELECT @@ROWCOUNT AS RejectedOrders;
END;
GO

------------------------------------------------------------
-- 12. SAMPLE SEED DATA ACROSS ALL 4 ROLES
------------------------------------------------------------

-- SAMPLE CUSTOMERS
INSERT INTO dbo.Customers (CustomerName, Email, PasswordHash, PhoneNumber, Address)
VALUES
('John Doe', 'john@example.com', '12345', '9876543210', '123 Park Street, Kolkata'),
('Jane Smith', 'jane@example.com', '12345', '9876543211', 'Bhubaneswar Central'),
('Rahul Das', 'rahul@example.com', '12345', '9876501234', 'Flat 4B, Park Street, Kolkata'),
('Priya Rout', 'priya@example.com', '12345', '9876505678', 'Plot 12, Salt Lake Sector V, Kolkata'),
('Aman Sen', 'aman@example.com', '12345', '9876509999', '88 Southern Avenue, Kolkata');
GO

-- SAMPLE ADMIN
INSERT INTO dbo.Admins (AdminName, Email, PasswordHash, IsActive)
VALUES
('System Admin', 'admin@example.com', 'admin123', 1);
GO

-- SAMPLE RESTAURANT
INSERT INTO dbo.Restaurants (RestaurantName, Email, PasswordHash, PhoneNumber, Address, Cuisine, Rating, ImageUrl)
VALUES
('FoodReady Central Kitchen', 'restaurant@example.com', 'rest123', '9876500001', '124 Food Street, Downtown', 'Multi-Cuisine, Italian, Burgers', 4.8, 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=500&q=80');
GO

-- SAMPLE RIDER
INSERT INTO dbo.Riders (RiderName, Email, PasswordHash, PhoneNumber, VehicleType, CurrentStatus)
VALUES
('Amit Kumar (Speedy)', 'rider@example.com', 'rider123', '9876599999', 'Hero Splendor Plus', 'Available');
GO

-- SAMPLE FOOD ITEMS (Matching all category tabs and mockups)
INSERT INTO dbo.FoodItems (ItemName, Description, Price, ImageUrl, Category, IsAvailable)
VALUES
('Margherita Pizza', 'Classic cheese & fresh basil on crispy thin crust', 299.00, '/Content/images/margherita.jpg', 'Pizza', 1),
('Veggie Burger', 'Delicious burger with vegetable patty, lettuce and cheese', 249.00, '/Content/images/veggie-burger.jpg', 'Burger', 1),
('Pepperoni Pizza', 'Loaded with savoury spiced pepperoni and melted mozzarella', 399.00, '/Content/images/pepperoni.jpg', 'Pizza', 1),
('Chicken Burger', 'Juicy grilled chicken patty with smoky BBQ sauce', 279.00, '/Content/images/chicken-burger.jpg', 'Burger', 1),
('French Fries', 'Crispy golden potato fries served with tangy mayo', 149.00, '/Content/images/fries.jpg', 'Snacks', 1),
('Paneer Butter Masala', 'Cottage cheese cubes simmered in rich creamy tomato gravy', 280.00, '/Content/images/paneer.jpg', 'Indian', 1),
('Butter Naan', 'Soft and fluffy Indian tandoori bread brushed with butter', 45.00, '/Content/images/naan.jpg', 'Indian', 1),
('Veg Hakka Noodles', 'Wok-tossed noodles with crunchy fresh veggies & oriental sauces', 199.00, '/Content/images/noodles.jpg', 'Chinese', 1),
('Chilli Chicken', 'Tender chicken tossed in fiery green chillies and soy sauce', 260.00, '/Content/images/chilli-chicken.jpg', 'Chinese', 1),
('Cold Coffee', 'Thick creamy blended cold coffee topped with chocolate drizzle', 120.00, '/Content/images/cold-coffee.jpg', 'Drinks', 1),
('Fresh Lime Soda', 'Refreshing chilled fizzy soda with fresh squeezed lemon', 80.00, '/Content/images/lime-soda.jpg', 'Drinks', 1),
('Chicken Dum Biryani', 'Aromatic basmati rice cooked on slow dum with tender chicken', 320.00, '/Content/images/biryani.jpg', 'Indian', 1),
('Creamy Alfredo Pasta', 'Penne pasta tossed in velvety garlic parmesan cream sauce', 260.00, '/Content/images/pasta.jpg', 'Snacks', 1),
('Molten Chocolate Cake', 'Warm chocolate lava cake with rich flowing chocolate center', 160.00, '/Content/images/dessert.jpg', 'Snacks', 1);
GO

-- SAMPLE INITIAL ORDERS
INSERT INTO dbo.Orders (CustomerId, TotalAmount, OrderStatus, OrderedAt, ProcessedAt, ProcessedBy, RestaurantId, DeliveryStatus)
VALUES
(1, 598.00, 'Accepted', DATEADD(MINUTE, -15, GETDATE()), GETDATE(), 1, 1, 'ReadyForPickup'),
(2, 249.00, 'Rejected', DATEADD(MINUTE, -20, GETDATE()), DATEADD(MINUTE, -15, GETDATE()), 1, 1, 'Cancelled'),
(3, 698.00, 'Pending', GETDATE(), NULL, NULL, 1, 'Pending'),
(4, 448.00, 'Pending', DATEADD(MINUTE, -1, GETDATE()), NULL, NULL, 1, 'Pending'),
(5, 986.00, 'Pending', DATEADD(MINUTE, -2, GETDATE()), NULL, NULL, 1, 'Pending');
GO

INSERT INTO dbo.OrderDetails (OrderId, FoodItemId, Quantity, UnitPrice)
VALUES
(1, 1, 2, 299.00),
(2, 2, 1, 249.00),
(3, 1, 1, 299.00),
(3, 3, 1, 399.00),
(4, 2, 1, 249.00),
(4, 8, 1, 199.00),
(5, 1, 2, 299.00),
(5, 3, 1, 388.00);
GO

SELECT 'FoodArrivalDB Database Setup Complete!' AS Status;
GO
