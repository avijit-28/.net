/*
===========================================================
 FOOD DELIVERY APPLICATION
 ASP.NET MVC 5 / .NET Framework
 SQL Server + ADO.NET + Stored Procedures

 DATABASE:
     FoodArrivalDB

 TABLES:
     Customers
     Admins
     FoodItems
     Orders
     OrderDetails

 NOTE:
     Application passwords should be securely hashed in C#.
     PasswordHash columns are used here so plain-text passwords
     do not need to be stored by the application.
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
-- 2. DROP EXISTING OBJECTS (FOR RE-RUNNING THIS SCRIPT)
------------------------------------------------------------

IF OBJECT_ID('dbo.sp_Order_AutoReject', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_AutoReject;
IF OBJECT_ID('dbo.sp_Order_Reject', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_Reject;
IF OBJECT_ID('dbo.sp_Order_Accept', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_Accept;
IF OBJECT_ID('dbo.sp_Order_GetTodayRejected', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetTodayRejected;
IF OBJECT_ID('dbo.sp_Order_GetTodayAccepted', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetTodayAccepted;
IF OBJECT_ID('dbo.sp_Order_GetTodayPending', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetTodayPending;
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
-- 3. DROP TABLES IF THEY EXIST
--    Drop child tables first because of foreign keys.
------------------------------------------------------------

IF OBJECT_ID('dbo.OrderDetails', 'U') IS NOT NULL
    DROP TABLE dbo.OrderDetails;
GO

IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL
    DROP TABLE dbo.Orders;
GO

IF OBJECT_ID('dbo.FoodItems', 'U') IS NOT NULL
    DROP TABLE dbo.FoodItems;
GO

IF OBJECT_ID('dbo.Customers', 'U') IS NOT NULL
    DROP TABLE dbo.Customers;
GO

IF OBJECT_ID('dbo.Admins', 'U') IS NOT NULL
    DROP TABLE dbo.Admins;
GO


------------------------------------------------------------
-- 4. DROP TABLE TYPE IF IT EXISTS
------------------------------------------------------------

IF TYPE_ID('dbo.OrderItemType') IS NOT NULL
    DROP TYPE dbo.OrderItemType;
GO


------------------------------------------------------------
-- 5. CREATE CUSTOMERS TABLE
------------------------------------------------------------

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


------------------------------------------------------------
-- 6. CREATE ADMINS TABLE
------------------------------------------------------------

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


------------------------------------------------------------
-- 7. CREATE FOOD ITEMS TABLE
------------------------------------------------------------

CREATE TABLE dbo.FoodItems
(
    FoodItemId INT IDENTITY(1,1) PRIMARY KEY,

    ItemName NVARCHAR(150) NOT NULL,

    Description NVARCHAR(500) NULL,

    Price DECIMAL(10,2) NOT NULL,

    ImageUrl NVARCHAR(500) NULL,

    IsAvailable BIT NOT NULL DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    UpdatedAt DATETIME2 NULL,

    CONSTRAINT CK_FoodItems_Price
        CHECK (Price >= 0)
);
GO


------------------------------------------------------------
-- 8. CREATE ORDERS TABLE
------------------------------------------------------------

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

    CONSTRAINT FK_Orders_Customers
        FOREIGN KEY (CustomerId)
        REFERENCES dbo.Customers(CustomerId),

    CONSTRAINT FK_Orders_Admins
        FOREIGN KEY (ProcessedBy)
        REFERENCES dbo.Admins(AdminId),

    CONSTRAINT CK_Orders_Status
        CHECK (OrderStatus IN
        ('Pending', 'Accepted', 'Rejected')),

    CONSTRAINT CK_Orders_Total
        CHECK (TotalAmount >= 0)
);
GO


------------------------------------------------------------
-- 9. CREATE ORDER DETAILS TABLE
------------------------------------------------------------

CREATE TABLE dbo.OrderDetails
(
    OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,

    OrderId INT NOT NULL,

    FoodItemId INT NOT NULL,

    Quantity INT NOT NULL,

    UnitPrice DECIMAL(10,2) NOT NULL,

    SubTotal AS (Quantity * UnitPrice) PERSISTED,

    CONSTRAINT FK_OrderDetails_Orders
        FOREIGN KEY (OrderId)
        REFERENCES dbo.Orders(OrderId),

    CONSTRAINT FK_OrderDetails_FoodItems
        FOREIGN KEY (FoodItemId)
        REFERENCES dbo.FoodItems(FoodItemId),

    CONSTRAINT CK_OrderDetails_Quantity
        CHECK (Quantity > 0),

    CONSTRAINT CK_OrderDetails_UnitPrice
        CHECK (UnitPrice >= 0)
);
GO


------------------------------------------------------------
-- 10. CREATE INDEXES
------------------------------------------------------------

CREATE INDEX IX_Orders_CustomerId
ON dbo.Orders(CustomerId);
GO

CREATE INDEX IX_Orders_Status
ON dbo.Orders(OrderStatus);
GO

CREATE INDEX IX_Orders_OrderedAt
ON dbo.Orders(OrderedAt);
GO

CREATE INDEX IX_OrderDetails_OrderId
ON dbo.OrderDetails(OrderId);
GO


------------------------------------------------------------
-- 11. CREATE TABLE TYPE
--     Used for placing multiple food items in one order.
------------------------------------------------------------

CREATE TYPE dbo.OrderItemType AS TABLE
(
    FoodItemId INT NOT NULL,
    Quantity INT NOT NULL
);
GO


------------------------------------------------------------
-- 12. SAMPLE CUSTOMER DATA
------------------------------------------------------------

INSERT INTO dbo.Customers
(
    CustomerName,
    Email,
    PasswordHash,
    PhoneNumber,
    Address
)
VALUES
(
    'John Doe',
    'john@example.com',
    '12345',
    '9876543210',
    'Kolkata'
),
(
    'Jane Smith',
    'jane@example.com',
    '12345',
    '9876543211',
    'Bhubaneswar'
);
GO


------------------------------------------------------------
-- 13. SAMPLE ADMIN DATA
------------------------------------------------------------

INSERT INTO dbo.Admins
(
    AdminName,
    Email,
    PasswordHash,
    IsActive
)
VALUES
(
    'System Admin',
    'admin@example.com',
    'admin123',
    1
);
GO


------------------------------------------------------------
-- 14. SAMPLE FOOD ITEMS
------------------------------------------------------------

INSERT INTO dbo.FoodItems
(
    ItemName,
    Description,
    Price,
    ImageUrl,
    IsAvailable
)
VALUES
(
    'Margherita Pizza',
    'Classic cheese pizza',
    299.00,
    '/Content/images/margherita.jpg',
    1
),
(
    'Veggie Burger',
    'Delicious burger with vegetable patty',
    249.00,
    '/Content/images/veggie-burger.jpg',
    1
),
(
    'Pepperoni Pizza',
    'Pizza with pepperoni toppings',
    399.00,
    '/Content/images/pepperoni.jpg',
    1
),
(
    'Chicken Burger',
    'Juicy chicken burger',
    279.00,
    '/Content/images/chicken-burger.jpg',
    1
),
(
    'French Fries',
    'Crispy golden fries',
    149.00,
    '/Content/images/fries.jpg',
    1
);
GO


-- =========================================================
-- CUSTOMER STORED PROCEDURES
--============================================================


------------------------------------------------------------
-- 15. CUSTOMER INSERT / REGISTER
------------------------------------------------------------

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

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Customers
        WHERE Email = @Email
    )
    BEGIN
        SELECT
            0 AS Result,
            'Email already exists.' AS Message,
            NULL AS CustomerId;

        RETURN;
    END;

    INSERT INTO dbo.Customers
    (
        CustomerName,
        Email,
        PasswordHash,
        PhoneNumber,
        Address
    )
    VALUES
    (
        @CustomerName,
        @Email,
        @PasswordHash,
        @PhoneNumber,
        @Address
    );

    SELECT
        1 AS Result,
        'Customer registered successfully.' AS Message,
        CAST(SCOPE_IDENTITY() AS INT) AS CustomerId;
END;
GO


------------------------------------------------------------
-- 16. CUSTOMER LOGIN
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Customer_Login
(
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CustomerId,
        CustomerName,
        Email,
        PhoneNumber,
        Address
    FROM dbo.Customers
    WHERE Email = @Email
      AND PasswordHash = @PasswordHash
      AND IsActive = 1;
END;
GO


------------------------------------------------------------
-- 17. GET ALL CUSTOMERS
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Customer_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CustomerId,
        CustomerName,
        Email,
        PhoneNumber,
        Address,
        IsActive,
        CreatedAt
    FROM dbo.Customers
    ORDER BY CustomerId DESC;
END;
GO


------------------------------------------------------------
-- 18. GET CUSTOMER BY ID
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Customer_GetById
(
    @CustomerId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CustomerId,
        CustomerName,
        Email,
        PhoneNumber,
        Address,
        IsActive,
        CreatedAt
    FROM dbo.Customers
    WHERE CustomerId = @CustomerId;
END;
GO


------------------------------------------------------------
-- 19. UPDATE CUSTOMER
------------------------------------------------------------

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
    SET
        CustomerName = @CustomerName,
        PhoneNumber = @PhoneNumber,
        Address = @Address
    WHERE CustomerId = @CustomerId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            0 AS Result,
            'Customer not found.' AS Message;
        RETURN;
    END;

    SELECT
        1 AS Result,
        'Customer updated successfully.' AS Message;
END;
GO


------------------------------------------------------------
-- 20. DELETE / DEACTIVATE CUSTOMER
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Customer_Delete
(
    @CustomerId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Customers
    SET IsActive = 0
    WHERE CustomerId = @CustomerId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            0 AS Result,
            'Customer not found.' AS Message;
        RETURN;
    END;

    SELECT
        1 AS Result,
        'Customer deactivated successfully.' AS Message;
END;
GO


-- =========================================================
-- FOOD ITEM STORED PROCEDURES
-- ============================================================


------------------------------------------------------------
-- 21. INSERT FOOD ITEM
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_Insert
(
    @ItemName NVARCHAR(150),
    @Description NVARCHAR(500),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(500),
    @IsAvailable BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Price < 0
    BEGIN
        SELECT
            0 AS Result,
            'Price cannot be negative.' AS Message,
            NULL AS FoodItemId;
        RETURN;
    END;

    INSERT INTO dbo.FoodItems
    (
        ItemName,
        Description,
        Price,
        ImageUrl,
        IsAvailable
    )
    VALUES
    (
        @ItemName,
        @Description,
        @Price,
        @ImageUrl,
        @IsAvailable
    );

    SELECT
        1 AS Result,
        'Food item inserted successfully.' AS Message,
        CAST(SCOPE_IDENTITY() AS INT) AS FoodItemId;
END;
GO


------------------------------------------------------------
-- 22. GET ALL FOOD ITEMS
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_GetAll
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        FoodItemId,
        ItemName,
        Description,
        Price,
        ImageUrl,
        IsAvailable,
        CreatedAt,
        UpdatedAt
    FROM dbo.FoodItems
    ORDER BY FoodItemId DESC;
END;
GO


------------------------------------------------------------
-- 23. GET AVAILABLE FOOD ITEMS
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_GetAvailable
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        FoodItemId,
        ItemName,
        Description,
        Price,
        ImageUrl,
        IsAvailable
    FROM dbo.FoodItems
    WHERE IsAvailable = 1
    ORDER BY ItemName;
END;
GO


------------------------------------------------------------
-- 24. GET FOOD ITEM BY ID
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_GetById
(
    @FoodItemId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        FoodItemId,
        ItemName,
        Description,
        Price,
        ImageUrl,
        IsAvailable,
        CreatedAt,
        UpdatedAt
    FROM dbo.FoodItems
    WHERE FoodItemId = @FoodItemId;
END;
GO


------------------------------------------------------------
-- 25. UPDATE FOOD ITEM
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_Update
(
    @FoodItemId INT,
    @ItemName NVARCHAR(150),
    @Description NVARCHAR(500),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(500),
    @IsAvailable BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Price < 0
    BEGIN
        SELECT
            0 AS Result,
            'Price cannot be negative.' AS Message;
        RETURN;
    END;

    UPDATE dbo.FoodItems
    SET
        ItemName = @ItemName,
        Description = @Description,
        Price = @Price,
        ImageUrl = @ImageUrl,
        IsAvailable = @IsAvailable,
        UpdatedAt = GETDATE()
    WHERE FoodItemId = @FoodItemId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            0 AS Result,
            'Food item not found.' AS Message;
        RETURN;
    END;

    SELECT
        1 AS Result,
        'Food item updated successfully.' AS Message;
END;
GO


------------------------------------------------------------
-- 26. DELETE / DISABLE FOOD ITEM
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_Delete
(
    @FoodItemId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.FoodItems
    SET
        IsAvailable = 0,
        UpdatedAt = GETDATE()
    WHERE FoodItemId = @FoodItemId;

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            0 AS Result,
            'Food item not found.' AS Message;
        RETURN;
    END;

    SELECT
        1 AS Result,
        'Food item removed successfully.' AS Message;
END;
GO


------------------------------------------------------------
-- 27. SEARCH FOOD ITEMS
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_FoodItem_Search
(
    @Search NVARCHAR(150)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        FoodItemId,
        ItemName,
        Description,
        Price,
        ImageUrl,
        IsAvailable
    FROM dbo.FoodItems
    WHERE IsAvailable = 1
      AND
      (
          ItemName LIKE '%' + @Search + '%'
          OR Description LIKE '%' + @Search + '%'
      )
    ORDER BY ItemName;
END;
GO


-- =========================================================
-- ADMIN STORED PROCEDURES
-- ============================================================


------------------------------------------------------------
-- 28. ADMIN LOGIN
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Admin_Login
(
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        AdminId,
        AdminName,
        Email
    FROM dbo.Admins
    WHERE Email = @Email
      AND PasswordHash = @PasswordHash
      AND IsActive = 1;
END;
GO


-- =========================================================
-- ORDER STORED PROCEDURES
-- ============================================================


------------------------------------------------------------
-- 29. CREATE SINGLE-ITEM ORDER
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
            RAISERROR('Quantity must be greater than zero.',16,1);
        END;

        SELECT
            @UnitPrice = Price
        FROM dbo.FoodItems
        WHERE FoodItemId = @FoodItemId
          AND IsAvailable = 1;

        IF @UnitPrice IS NULL
        BEGIN
            RAISERROR('Food item is not available.',16,1);
        END;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Customers
            WHERE CustomerId = @CustomerId
              AND IsActive = 1
        )
        BEGIN
            RAISERROR('Customer is not active or does not exist.',16,1);
        END;

        INSERT INTO dbo.Orders
        (
            CustomerId,
            TotalAmount,
            OrderStatus
        )
        VALUES
        (
            @CustomerId,
            0,
            'Pending'
        );

        SET @OrderId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.OrderDetails
        (
            OrderId,
            FoodItemId,
            Quantity,
            UnitPrice
        )
        VALUES
        (
            @OrderId,
            @FoodItemId,
            @Quantity,
            @UnitPrice
        );

        UPDATE dbo.Orders
        SET TotalAmount =
        (
            SELECT SUM(SubTotal)
            FROM dbo.OrderDetails
            WHERE OrderId = @OrderId
        )
        WHERE OrderId = @OrderId;

        COMMIT TRANSACTION;

        SELECT
            1 AS Result,
            @OrderId AS OrderId,
            'Order placed successfully.' AS Message;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;
END;
GO


------------------------------------------------------------
-- 30. CREATE MULTIPLE-ITEM ORDER
------------------------------------------------------------

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

        IF NOT EXISTS
        (
            SELECT 1
            FROM @Items
            WHERE Quantity > 0
        )
        BEGIN
            RAISERROR('Order must contain at least one item.',16,1);
        END;

        IF EXISTS
        (
            SELECT 1
            FROM @Items i
            LEFT JOIN dbo.FoodItems f
                ON i.FoodItemId = f.FoodItemId
            WHERE f.FoodItemId IS NULL
               OR f.IsAvailable = 0
               OR i.Quantity <= 0
        )
        BEGIN
            RAISERROR('One or more food items are unavailable.',16,1);
        END;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.Customers
            WHERE CustomerId = @CustomerId
              AND IsActive = 1
        )
        BEGIN
            RAISERROR('Customer is not active or does not exist.',16,1);
        END;

        INSERT INTO dbo.Orders
        (
            CustomerId,
            TotalAmount,
            OrderStatus
        )
        VALUES
        (
            @CustomerId,
            0,
            'Pending'
        );

        SET @OrderId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.OrderDetails
        (
            OrderId,
            FoodItemId,
            Quantity,
            UnitPrice
        )
        SELECT
            @OrderId,
            i.FoodItemId,
            i.Quantity,
            f.Price
        FROM @Items i
        INNER JOIN dbo.FoodItems f
            ON i.FoodItemId = f.FoodItemId;

        UPDATE dbo.Orders
        SET TotalAmount =
        (
            SELECT SUM(SubTotal)
            FROM dbo.OrderDetails
            WHERE OrderId = @OrderId
        )
        WHERE OrderId = @OrderId;

        COMMIT TRANSACTION;

        SELECT
            1 AS Result,
            @OrderId AS OrderId,
            'Order placed successfully.' AS Message;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;
END;
GO


------------------------------------------------------------
-- 31. CUSTOMER ORDER HISTORY
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Order_GetByCustomer
(
    @CustomerId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        o.OrderId,
        o.CustomerId,
        o.TotalAmount,
        o.OrderStatus,
        o.OrderedAt,
        o.ProcessedAt
    FROM dbo.Orders o
    WHERE o.CustomerId = @CustomerId
    ORDER BY o.OrderedAt DESC;
END;
GO


------------------------------------------------------------
-- 32. ORDER DETAILS
------------------------------------------------------------

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
    INNER JOIN dbo.Customers c
        ON o.CustomerId = c.CustomerId
    INNER JOIN dbo.OrderDetails od
        ON o.OrderId = od.OrderId
    INNER JOIN dbo.FoodItems f
        ON od.FoodItemId = f.FoodItemId
    WHERE o.OrderId = @OrderId
    ORDER BY od.OrderDetailId;
END;
GO


------------------------------------------------------------
-- 33. TODAY'S PENDING ORDERS
------------------------------------------------------------

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
                SELECT
                    ', ' + f.ItemName +
                    ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f
                    ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'),
            1,
            2,
            ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c
        ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Pending'
      AND o.OrderedAt >= CAST(GETDATE() AS DATE)
      AND o.OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
    ORDER BY o.OrderedAt ASC;
END;
GO


------------------------------------------------------------
-- 34. TODAY'S ACCEPTED ORDERS
------------------------------------------------------------

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
                SELECT
                    ', ' + f.ItemName +
                    ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f
                    ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'),
            1,
            2,
            ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c
        ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Accepted'
      AND o.OrderedAt >= CAST(GETDATE() AS DATE)
      AND o.OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
    ORDER BY o.ProcessedAt DESC;
END;
GO


------------------------------------------------------------
-- 35. TODAY'S REJECTED ORDERS
------------------------------------------------------------

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
                SELECT
                    ', ' + f.ItemName +
                    ' x' + CAST(od.Quantity AS NVARCHAR(10))
                FROM dbo.OrderDetails od
                INNER JOIN dbo.FoodItems f
                    ON od.FoodItemId = f.FoodItemId
                WHERE od.OrderId = o.OrderId
                FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)'),
            1,
            2,
            ''
        ) AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c
        ON o.CustomerId = c.CustomerId
    WHERE o.OrderStatus = 'Rejected'
      AND o.OrderedAt >= CAST(GETDATE() AS DATE)
      AND o.OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
    ORDER BY o.ProcessedAt DESC;
END;
GO


------------------------------------------------------------
-- 36. ACCEPT ORDER
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Order_Accept
(
    @OrderId INT,
    @AdminId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Admins
        WHERE AdminId = @AdminId
          AND IsActive = 1
    )
    BEGIN
        SELECT
            0 AS Result,
            'Invalid admin.' AS Message;
        RETURN;
    END;

    UPDATE dbo.Orders
    SET
        OrderStatus = 'Accepted',
        ProcessedAt = GETDATE(),
        ProcessedBy = @AdminId,
        RejectionReason = NULL
    WHERE OrderId = @OrderId
      AND OrderStatus = 'Pending';

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            0 AS Result,
            'Order cannot be accepted. It may not exist or is no longer pending.' AS Message;
        RETURN;
    END;

    SELECT
        1 AS Result,
        'Order accepted successfully.' AS Message;
END;
GO


------------------------------------------------------------
-- 37. REJECT ORDER
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Order_Reject
(
    @OrderId INT,
    @AdminId INT,
    @RejectionReason NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Admins
        WHERE AdminId = @AdminId
          AND IsActive = 1
    )
    BEGIN
        SELECT
            0 AS Result,
            'Invalid admin.' AS Message;
        RETURN;
    END;

    UPDATE dbo.Orders
    SET
        OrderStatus = 'Rejected',
        ProcessedAt = GETDATE(),
        ProcessedBy = @AdminId,
        RejectionReason =
            ISNULL(NULLIF(@RejectionReason, ''),
                   'Order rejected by administrator.')
    WHERE OrderId = @OrderId
      AND OrderStatus = 'Pending';

    IF @@ROWCOUNT = 0
    BEGIN
        SELECT
            0 AS Result,
            'Order cannot be rejected. It may not exist or is no longer pending.' AS Message;
        RETURN;
    END;

    SELECT
        1 AS Result,
        'Order rejected successfully.' AS Message;
END;
GO


------------------------------------------------------------
-- 38. AUTO-REJECT ORDERS AFTER 5 MINUTES
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Order_AutoReject
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Orders
    SET
        OrderStatus = 'Rejected',
        ProcessedAt = GETDATE(),
        RejectionReason =
            'Automatically rejected after 5 minutes.'
    WHERE OrderStatus = 'Pending'
      AND DATEDIFF(SECOND, OrderedAt, GETDATE()) >= 300;

    SELECT
        @@ROWCOUNT AS RejectedOrders;
END;
GO


-- =========================================================
-- ADMIN DASHBOARD
-- ============================================================


------------------------------------------------------------
-- 39. ADMIN DASHBOARD SUMMARY
------------------------------------------------------------

CREATE PROCEDURE dbo.sp_Admin_DashboardSummary
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        (
            SELECT COUNT(*)
            FROM dbo.Orders
            WHERE OrderStatus = 'Pending'
              AND OrderedAt >= CAST(GETDATE() AS DATE)
              AND OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
        ) AS PendingOrders,

        (
            SELECT COUNT(*)
            FROM dbo.Orders
            WHERE OrderStatus = 'Accepted'
              AND OrderedAt >= CAST(GETDATE() AS DATE)
              AND OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
        ) AS AcceptedOrders,

        (
            SELECT COUNT(*)
            FROM dbo.Orders
            WHERE OrderStatus = 'Rejected'
              AND OrderedAt >= CAST(GETDATE() AS DATE)
              AND OrderedAt < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
        ) AS RejectedOrders,

        (
            SELECT COUNT(*)
            FROM dbo.FoodItems
            WHERE IsAvailable = 1
        ) AS AvailableFoodItems,

        (
            SELECT COUNT(*)
            FROM dbo.Customers
            WHERE IsActive = 1
        ) AS ActiveCustomers;
END;
GO


-- =========================================================
-- TEST / VERIFICATION QUERIES
-- ============================================================


------------------------------------------------------------
-- 40. CHECK TABLES
------------------------------------------------------------

SELECT * FROM dbo.Customers;
SELECT * FROM dbo.Admins;
SELECT * FROM dbo.FoodItems;
SELECT * FROM dbo.Orders;
SELECT * FROM dbo.OrderDetails;
GO


------------------------------------------------------------
-- 41. CHECK STORED PROCEDURES
------------------------------------------------------------

SELECT
    name AS StoredProcedureName
FROM sys.procedures
WHERE name LIKE 'sp_%'
ORDER BY name;
GO


------------------------------------------------------------
-- 42. TEST CUSTOMER LOGIN
------------------------------------------------------------

-- EXEC dbo.sp_Customer_Login
--      @Email = 'john@example.com',
--      @PasswordHash = '12345';


------------------------------------------------------------
-- 43. TEST ADMIN LOGIN
------------------------------------------------------------

-- EXEC dbo.sp_Admin_Login
--      @Email = 'admin@example.com',
--      @PasswordHash = 'admin123';


------------------------------------------------------------
-- 44. TEST FOOD ITEMS
------------------------------------------------------------

-- EXEC dbo.sp_FoodItem_GetAll;
-- EXEC dbo.sp_FoodItem_GetAvailable;
-- EXEC dbo.sp_FoodItem_GetById @FoodItemId = 1;
-- EXEC dbo.sp_FoodItem_Search @Search = 'Pizza';


------------------------------------------------------------
-- 45. TEST CUSTOMER
------------------------------------------------------------

-- EXEC dbo.sp_Customer_GetAll;
-- EXEC dbo.sp_Customer_GetById @CustomerId = 1;


------------------------------------------------------------
-- 46. TEST ORDER LISTS
------------------------------------------------------------

-- EXEC dbo.sp_Order_GetTodayPending;
-- EXEC dbo.sp_Order_GetTodayAccepted;
-- EXEC dbo.sp_Order_GetTodayRejected;


------------------------------------------------------------
-- 47. TEST DASHBOARD
------------------------------------------------------------

-- EXEC dbo.sp_Admin_DashboardSummary;


-- =========================================================
-- END OF FOOD DELIVERY DATABASE SCRIPT
-- ============================================================
