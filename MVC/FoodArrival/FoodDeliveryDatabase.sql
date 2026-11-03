/*
    Food Delivery Web App - SQL Server schema and stored procedures
    Target: SQL Server 2016+ (OPENJSON is used for order line items).
    Run in SSMS or sqlcmd with permission to create a database.
    The optional demo seed section is commented out at the bottom.
*/

USE master;
GO

IF DB_ID(N'FoodDeliveryDB') IS NULL
BEGIN
    CREATE DATABASE FoodDeliveryDB;
END;
GO

USE FoodDeliveryDB;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'food')
    EXEC(N'CREATE SCHEMA food AUTHORIZATION dbo;');
GO

/* 1. Tables */
IF OBJECT_ID(N'food.Users', N'U') IS NULL
BEGIN
    CREATE TABLE food.Users
    (
        UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
        FullName NVARCHAR(120) NOT NULL,
        Email NVARCHAR(254) NOT NULL,
        PasswordHash NVARCHAR(500) NOT NULL,
        Phone NVARCHAR(30) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Users_Email UNIQUE (Email)
    );
END;
GO

IF OBJECT_ID(N'food.Roles', N'U') IS NULL
BEGIN
    CREATE TABLE food.Roles
    (
        RoleId SMALLINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        RoleName NVARCHAR(30) NOT NULL,
        Description NVARCHAR(250) NULL,
        IsSelfAssignable BIT NOT NULL CONSTRAINT DF_Roles_SelfAssignable DEFAULT 0,
        IsActive BIT NOT NULL CONSTRAINT DF_Roles_IsActive DEFAULT 1,
        CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName),
        CONSTRAINT CK_Roles_Name CHECK (RoleName IN (N'Admin', N'Customer', N'Staff'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM food.Roles WHERE RoleName = N'Admin')
    INSERT food.Roles(RoleName, Description, IsSelfAssignable) VALUES (N'Admin', N'Platform administrator', 0);
IF NOT EXISTS (SELECT 1 FROM food.Roles WHERE RoleName = N'Customer')
    INSERT food.Roles(RoleName, Description, IsSelfAssignable) VALUES (N'Customer', N'Food ordering customer', 1);
IF NOT EXISTS (SELECT 1 FROM food.Roles WHERE RoleName = N'Staff')
    INSERT food.Roles(RoleName, Description, IsSelfAssignable) VALUES (N'Staff', N'Restaurant or delivery operations staff', 0);
GO

IF OBJECT_ID(N'food.UserRoles', N'U') IS NULL
BEGIN
    CREATE TABLE food.UserRoles
    (
        UserRoleId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UserRoles PRIMARY KEY,
        UserId INT NOT NULL,
        RoleId SMALLINT NOT NULL,
        AssignedByUserId INT NULL,
        AssignedAt DATETIME2(0) NOT NULL CONSTRAINT DF_UserRoles_AssignedAt DEFAULT SYSUTCDATETIME(),
        IsActive BIT NOT NULL CONSTRAINT DF_UserRoles_IsActive DEFAULT 1,
        CONSTRAINT FK_UserRoles_User FOREIGN KEY (UserId) REFERENCES food.Users(UserId),
        CONSTRAINT FK_UserRoles_Role FOREIGN KEY (RoleId) REFERENCES food.Roles(RoleId),
        CONSTRAINT FK_UserRoles_AssignedBy FOREIGN KEY (AssignedByUserId) REFERENCES food.Users(UserId)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'food.UserRoles') AND name = N'UX_UserRoles_OneActiveRole')
    CREATE UNIQUE INDEX UX_UserRoles_OneActiveRole ON food.UserRoles(UserId) WHERE IsActive = 1;
GO

IF OBJECT_ID(N'food.StaffProfiles', N'U') IS NULL
BEGIN
    CREATE TABLE food.StaffProfiles
    (
        UserId INT NOT NULL CONSTRAINT PK_StaffProfiles PRIMARY KEY,
        StaffType NVARCHAR(30) NOT NULL,
        EmployeeCode NVARCHAR(30) NULL,
        Phone NVARCHAR(30) NULL,
        IsAvailable BIT NOT NULL CONSTRAINT DF_StaffProfiles_IsAvailable DEFAULT 1,
        VehicleType NVARCHAR(50) NULL,
        VehicleNumber NVARCHAR(30) NULL,
        JoinedAt DATETIME2(0) NOT NULL CONSTRAINT DF_StaffProfiles_JoinedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_StaffProfiles_User FOREIGN KEY (UserId) REFERENCES food.Users(UserId),
        CONSTRAINT UQ_StaffProfiles_EmployeeCode UNIQUE (EmployeeCode),
        CONSTRAINT CK_StaffProfiles_Type CHECK (StaffType IN (N'Restaurant', N'DeliveryAgent'))
    );
END;
GO

IF OBJECT_ID(N'food.AccountRoleRequests', N'U') IS NULL
BEGIN
    CREATE TABLE food.AccountRoleRequests
    (
        RequestId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_AccountRoleRequests PRIMARY KEY,
        UserId INT NOT NULL,
        RequestedRole NVARCHAR(30) NOT NULL,
        RequestedStaffType NVARCHAR(30) NOT NULL,
        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_AccountRoleRequests_Status DEFAULT N'Pending',
        RequestNote NVARCHAR(500) NULL,
        RequestedAt DATETIME2(0) NOT NULL CONSTRAINT DF_AccountRoleRequests_RequestedAt DEFAULT SYSUTCDATETIME(),
        ReviewedByUserId INT NULL,
        ReviewedAt DATETIME2(0) NULL,
        CONSTRAINT FK_AccountRoleRequests_User FOREIGN KEY (UserId) REFERENCES food.Users(UserId),
        CONSTRAINT FK_AccountRoleRequests_Reviewer FOREIGN KEY (ReviewedByUserId) REFERENCES food.Users(UserId),
        CONSTRAINT CK_AccountRoleRequests_Role CHECK (RequestedRole = N'Staff'),
        CONSTRAINT CK_AccountRoleRequests_StaffType CHECK (RequestedStaffType IN (N'Restaurant', N'DeliveryAgent')),
        CONSTRAINT CK_AccountRoleRequests_Status CHECK (Status IN (N'Pending', N'Approved', N'Rejected'))
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'food.AccountRoleRequests') AND name = N'UX_AccountRoleRequests_PendingUser')
    CREATE UNIQUE INDEX UX_AccountRoleRequests_PendingUser ON food.AccountRoleRequests(UserId) WHERE Status = N'Pending';
GO

/* Migrate users created by the earlier single-column role version. */
IF COL_LENGTH(N'food.Users', N'Role') IS NOT NULL
BEGIN
    INSERT food.UserRoles(UserId, RoleId, IsActive)
    SELECT u.UserId, r.RoleId, 1
    FROM food.Users u
    INNER JOIN food.Roles r ON r.RoleName = CASE WHEN u.Role IN (N'Restaurant', N'DeliveryAgent') THEN N'Staff' ELSE u.Role END
    WHERE NOT EXISTS (SELECT 1 FROM food.UserRoles ur WHERE ur.UserId = u.UserId AND ur.IsActive = 1);

    INSERT food.StaffProfiles(UserId, StaffType, Phone)
    SELECT u.UserId, CASE WHEN u.Role = N'DeliveryAgent' THEN N'DeliveryAgent' ELSE N'Restaurant' END, u.Phone
    FROM food.Users u
    WHERE u.Role IN (N'Restaurant', N'DeliveryAgent')
      AND NOT EXISTS (SELECT 1 FROM food.StaffProfiles sp WHERE sp.UserId = u.UserId);

    IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = N'DF_Users_Role')
        ALTER TABLE food.Users DROP CONSTRAINT DF_Users_Role;
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Users_Role')
        ALTER TABLE food.Users DROP CONSTRAINT CK_Users_Role;
    ALTER TABLE food.Users DROP COLUMN Role;
END;
GO

CREATE OR ALTER VIEW food.vw_UserAccess
AS
    SELECT u.UserId, u.FullName, u.Email, u.PasswordHash, u.Phone, u.IsActive,
           role.RoleName, sp.StaffType, pending.RequestId AS PendingRoleRequestId,
           pending.RequestedStaffType, pending.RequestedAt AS StaffRequestDate
    FROM food.Users u
    INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
    INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.IsActive = 1
    LEFT JOIN food.StaffProfiles sp ON sp.UserId = u.UserId
    LEFT JOIN food.AccountRoleRequests pending ON pending.UserId = u.UserId AND pending.Status = N'Pending';
GO

IF OBJECT_ID(N'food.Restaurants', N'U') IS NULL
BEGIN
    CREATE TABLE food.Restaurants
    (
        RestaurantId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Restaurants PRIMARY KEY,
        OwnerUserId INT NOT NULL,
        Name NVARCHAR(160) NOT NULL,
        Description NVARCHAR(1000) NULL,
        Cuisine NVARCHAR(80) NOT NULL,
        Phone NVARCHAR(30) NULL,
        Email NVARCHAR(254) NULL,
        AddressLine NVARCHAR(250) NOT NULL,
        City NVARCHAR(100) NOT NULL,
        PostalCode NVARCHAR(20) NULL,
        ImageUrl NVARCHAR(500) NULL,
        IsActive BIT NOT NULL CONSTRAINT DF_Restaurants_IsActive DEFAULT 1,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Restaurants_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Restaurants_Owner FOREIGN KEY (OwnerUserId) REFERENCES food.Users(UserId)
    );
END;
GO

IF OBJECT_ID(N'food.MenuItems', N'U') IS NULL
BEGIN
    CREATE TABLE food.MenuItems
    (
        MenuItemId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MenuItems PRIMARY KEY,
        RestaurantId INT NOT NULL,
        Name NVARCHAR(160) NOT NULL,
        Description NVARCHAR(600) NULL,
        Category NVARCHAR(80) NULL,
        Price DECIMAL(10,2) NOT NULL,
        ImageUrl NVARCHAR(500) NULL,
        IsVegetarian BIT NOT NULL CONSTRAINT DF_MenuItems_IsVegetarian DEFAULT 0,
        IsAvailable BIT NOT NULL CONSTRAINT DF_MenuItems_IsAvailable DEFAULT 1,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_MenuItems_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_MenuItems_UpdatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_MenuItems_Restaurants FOREIGN KEY (RestaurantId) REFERENCES food.Restaurants(RestaurantId),
        CONSTRAINT CK_MenuItems_Price CHECK (Price > 0)
    );
END;
GO

IF OBJECT_ID(N'food.Addresses', N'U') IS NULL
BEGIN
    CREATE TABLE food.Addresses
    (
        AddressId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Addresses PRIMARY KEY,
        UserId INT NOT NULL,
        Label NVARCHAR(50) NULL,
        RecipientName NVARCHAR(120) NOT NULL,
        Phone NVARCHAR(30) NOT NULL,
        AddressLine NVARCHAR(250) NOT NULL,
        AddressLine2 NVARCHAR(250) NULL,
        City NVARCHAR(100) NOT NULL,
        StateOrRegion NVARCHAR(100) NULL,
        PostalCode NVARCHAR(20) NULL,
        DeliveryInstructions NVARCHAR(500) NULL,
        IsDefault BIT NOT NULL CONSTRAINT DF_Addresses_IsDefault DEFAULT 0,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Addresses_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Addresses_Users FOREIGN KEY (UserId) REFERENCES food.Users(UserId)
    );
END;
GO

IF OBJECT_ID(N'food.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE food.Orders
    (
        OrderId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
        OrderNumber NVARCHAR(32) NOT NULL,
        CustomerUserId INT NOT NULL,
        RestaurantId INT NOT NULL,
        AddressId INT NULL,
        DeliveryRecipient NVARCHAR(120) NOT NULL,
        DeliveryPhone NVARCHAR(30) NOT NULL,
        DeliveryAddressLine NVARCHAR(250) NOT NULL,
        DeliveryAddressLine2 NVARCHAR(250) NULL,
        DeliveryCity NVARCHAR(100) NOT NULL,
        DeliveryPostalCode NVARCHAR(20) NULL,
        DeliveryInstructions NVARCHAR(500) NULL,
        Subtotal DECIMAL(10,2) NOT NULL,
        DeliveryFee DECIMAL(10,2) NOT NULL CONSTRAINT DF_Orders_DeliveryFee DEFAULT 0,
        TaxAmount DECIMAL(10,2) NOT NULL CONSTRAINT DF_Orders_TaxAmount DEFAULT 0,
        TotalAmount AS (Subtotal + DeliveryFee + TaxAmount) PERSISTED,
        OrderStatus NVARCHAR(20) NOT NULL CONSTRAINT DF_Orders_Status DEFAULT N'Placed',
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT SYSUTCDATETIME(),
        UpdatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Orders_UpdatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Orders_OrderNumber UNIQUE (OrderNumber),
        CONSTRAINT FK_Orders_Customer FOREIGN KEY (CustomerUserId) REFERENCES food.Users(UserId),
        CONSTRAINT FK_Orders_Restaurant FOREIGN KEY (RestaurantId) REFERENCES food.Restaurants(RestaurantId),
        CONSTRAINT FK_Orders_Address FOREIGN KEY (AddressId) REFERENCES food.Addresses(AddressId),
        CONSTRAINT CK_Orders_Amounts CHECK (Subtotal >= 0 AND DeliveryFee >= 0 AND TaxAmount >= 0),
        CONSTRAINT CK_Orders_Status CHECK (OrderStatus IN (N'Placed', N'Accepted', N'Rejected', N'Preparing', N'Ready', N'OutForDelivery', N'Delivered', N'Cancelled'))
    );
END;
GO

IF OBJECT_ID(N'food.OrderItems', N'U') IS NULL
BEGIN
    CREATE TABLE food.OrderItems
    (
        OrderItemId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderItems PRIMARY KEY,
        OrderId BIGINT NOT NULL,
        MenuItemId INT NOT NULL,
        ItemName NVARCHAR(160) NOT NULL,
        UnitPrice DECIMAL(10,2) NOT NULL,
        Quantity INT NOT NULL,
        LineTotal AS (UnitPrice * Quantity) PERSISTED,
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES food.Orders(OrderId) ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_MenuItems FOREIGN KEY (MenuItemId) REFERENCES food.MenuItems(MenuItemId),
        CONSTRAINT CK_OrderItems_UnitPrice CHECK (UnitPrice > 0),
        CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity BETWEEN 1 AND 50)
    );
END;
GO

IF OBJECT_ID(N'food.Payments', N'U') IS NULL
BEGIN
    CREATE TABLE food.Payments
    (
        PaymentId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Payments PRIMARY KEY,
        OrderId BIGINT NOT NULL,
        Method NVARCHAR(30) NOT NULL,
        Amount DECIMAL(10,2) NOT NULL,
        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Payments_Status DEFAULT N'Pending',
        ProviderReference NVARCHAR(120) NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Payments_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Payments_Order UNIQUE (OrderId),
        CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderId) REFERENCES food.Orders(OrderId),
        CONSTRAINT CK_Payments_Amount CHECK (Amount >= 0),
        CONSTRAINT CK_Payments_Method CHECK (Method IN (N'Cash', N'Card', N'UPI', N'Other')),
        CONSTRAINT CK_Payments_Status CHECK (Status IN (N'Pending', N'Authorized', N'Paid', N'Failed', N'Refunded'))
    );
END;
GO

IF OBJECT_ID(N'food.DeliveryAssignments', N'U') IS NULL
BEGIN
    CREATE TABLE food.DeliveryAssignments
    (
        AssignmentId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DeliveryAssignments PRIMARY KEY,
        OrderId BIGINT NOT NULL,
        AgentUserId INT NOT NULL,
        AssignedByUserId INT NULL,
        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_DeliveryAssignments_Status DEFAULT N'Assigned',
        AssignedAt DATETIME2(0) NOT NULL CONSTRAINT DF_DeliveryAssignments_AssignedAt DEFAULT SYSUTCDATETIME(),
        PickedUpAt DATETIME2(0) NULL,
        DeliveredAt DATETIME2(0) NULL,
        CONSTRAINT UQ_DeliveryAssignments_Order UNIQUE (OrderId),
        CONSTRAINT FK_DeliveryAssignments_Order FOREIGN KEY (OrderId) REFERENCES food.Orders(OrderId),
        CONSTRAINT FK_DeliveryAssignments_Agent FOREIGN KEY (AgentUserId) REFERENCES food.Users(UserId),
        CONSTRAINT FK_DeliveryAssignments_AssignedBy FOREIGN KEY (AssignedByUserId) REFERENCES food.Users(UserId),
        CONSTRAINT CK_DeliveryAssignments_Status CHECK (Status IN (N'Assigned', N'PickedUp', N'Delivered', N'Failed'))
    );
END;
GO

IF OBJECT_ID(N'food.Reviews', N'U') IS NULL
BEGIN
    CREATE TABLE food.Reviews
    (
        ReviewId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Reviews PRIMARY KEY,
        OrderId BIGINT NOT NULL,
        CustomerUserId INT NOT NULL,
        RestaurantId INT NOT NULL,
        Rating TINYINT NOT NULL,
        Comment NVARCHAR(1000) NULL,
        CreatedAt DATETIME2(0) NOT NULL CONSTRAINT DF_Reviews_CreatedAt DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Reviews_Order UNIQUE (OrderId),
        CONSTRAINT FK_Reviews_Order FOREIGN KEY (OrderId) REFERENCES food.Orders(OrderId),
        CONSTRAINT FK_Reviews_Customer FOREIGN KEY (CustomerUserId) REFERENCES food.Users(UserId),
        CONSTRAINT FK_Reviews_Restaurant FOREIGN KEY (RestaurantId) REFERENCES food.Restaurants(RestaurantId),
        CONSTRAINT CK_Reviews_Rating CHECK (Rating BETWEEN 1 AND 5)
    );
END;
GO

/* 2. Indexes */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'food.Restaurants') AND name = N'IX_Restaurants_City_Active')
    CREATE INDEX IX_Restaurants_City_Active ON food.Restaurants(City, IsActive) INCLUDE (Name, Cuisine);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'food.MenuItems') AND name = N'IX_MenuItems_Restaurant_Available')
    CREATE INDEX IX_MenuItems_Restaurant_Available ON food.MenuItems(RestaurantId, IsAvailable) INCLUDE (Name, Category, Price);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'food.Orders') AND name = N'IX_Orders_Customer_Created')
    CREATE INDEX IX_Orders_Customer_Created ON food.Orders(CustomerUserId, CreatedAt DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'food.Orders') AND name = N'IX_Orders_Restaurant_Status')
    CREATE INDEX IX_Orders_Restaurant_Status ON food.Orders(RestaurantId, OrderStatus, CreatedAt DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'food.DeliveryAssignments') AND name = N'IX_DeliveryAssignments_Agent_Status')
    CREATE INDEX IX_DeliveryAssignments_Agent_Status ON food.DeliveryAssignments(AgentUserId, Status, AssignedAt DESC);
GO

/* 3. Browse procedures */
CREATE OR ALTER PROCEDURE food.usp_Restaurant_Search
    @City NVARCHAR(100) = NULL,
    @Search NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.RestaurantId, r.Name, r.Description, r.Cuisine, r.City, r.AddressLine, r.ImageUrl,
           CAST(AVG(CAST(rv.Rating AS DECIMAL(4,2))) AS DECIMAL(3,2)) AS AverageRating,
           COUNT(DISTINCT rv.ReviewId) AS ReviewCount
    FROM food.Restaurants r
    LEFT JOIN food.Reviews rv ON rv.RestaurantId = r.RestaurantId
    WHERE r.IsActive = 1
      AND (@City IS NULL OR r.City LIKE N'%' + @City + N'%')
      AND (@Search IS NULL OR r.Name LIKE N'%' + @Search + N'%' OR r.Cuisine LIKE N'%' + @Search + N'%'
           OR EXISTS (SELECT 1 FROM food.MenuItems mi WHERE mi.RestaurantId = r.RestaurantId AND mi.IsAvailable = 1 AND mi.Name LIKE N'%' + @Search + N'%'))
    GROUP BY r.RestaurantId, r.Name, r.Description, r.Cuisine, r.City, r.AddressLine, r.ImageUrl
    ORDER BY r.Name;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Menu_List
    @RestaurantId INT,
    @Category NVARCHAR(80) = NULL,
    @VegetarianOnly BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Restaurants WHERE RestaurantId = @RestaurantId AND IsActive = 1)
        THROW 51001, 'Restaurant was not found or is inactive.', 1;
    SELECT MenuItemId, RestaurantId, Name, Description, Category, Price, ImageUrl, IsVegetarian, IsAvailable
    FROM food.MenuItems
    WHERE RestaurantId = @RestaurantId AND IsAvailable = 1
      AND (@Category IS NULL OR Category = @Category)
      AND (@VegetarianOnly = 0 OR IsVegetarian = 1)
    ORDER BY Category, Name;
END;
GO

/* 4. Restaurant menu management */
CREATE OR ALTER PROCEDURE food.usp_MenuItem_Save
    @ActorUserId INT,
    @RestaurantId INT,
    @MenuItemId INT = NULL,
    @Name NVARCHAR(160),
    @Description NVARCHAR(600) = NULL,
    @Category NVARCHAR(80) = NULL,
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(500) = NULL,
    @IsVegetarian BIT = 0,
    @IsAvailable BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Restaurants r
                   INNER JOIN food.Users u ON u.UserId = r.OwnerUserId AND u.IsActive = 1
                   INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
                   INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.RoleName = N'Staff'
                   INNER JOIN food.StaffProfiles sp ON sp.UserId = u.UserId AND sp.StaffType = N'Restaurant'
                   WHERE r.RestaurantId = @RestaurantId AND r.OwnerUserId = @ActorUserId)
        THROW 51002, 'Actor does not own this restaurant.', 1;
    IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL OR @Price <= 0
        THROW 51003, 'Menu item name and a positive price are required.', 1;

    IF @MenuItemId IS NULL
    BEGIN
        INSERT food.MenuItems(RestaurantId, Name, Description, Category, Price, ImageUrl, IsVegetarian, IsAvailable)
        VALUES (@RestaurantId, @Name, @Description, @Category, @Price, @ImageUrl, @IsVegetarian, @IsAvailable);
        SET @MenuItemId = CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE food.MenuItems
        SET Name = @Name, Description = @Description, Category = @Category, Price = @Price,
            ImageUrl = @ImageUrl, IsVegetarian = @IsVegetarian, IsAvailable = @IsAvailable,
            UpdatedAt = SYSUTCDATETIME()
        WHERE MenuItemId = @MenuItemId AND RestaurantId = @RestaurantId;
        IF @@ROWCOUNT = 0 THROW 51004, 'Menu item was not found for this restaurant.', 1;
    END;
    SELECT @MenuItemId AS MenuItemId;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_MenuItem_SetAvailability
    @ActorUserId INT,
    @MenuItemId INT,
    @IsAvailable BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE mi
    SET IsAvailable = @IsAvailable, UpdatedAt = SYSUTCDATETIME()
    FROM food.MenuItems mi
    INNER JOIN food.Restaurants r ON r.RestaurantId = mi.RestaurantId
    INNER JOIN food.Users u ON u.UserId = r.OwnerUserId AND u.IsActive = 1
    INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
    INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.RoleName = N'Staff'
    INNER JOIN food.StaffProfiles sp ON sp.UserId = u.UserId AND sp.StaffType = N'Restaurant'
    WHERE mi.MenuItemId = @MenuItemId AND r.OwnerUserId = @ActorUserId;
    IF @@ROWCOUNT = 0 THROW 51005, 'Menu item was not found for this restaurant owner.', 1;
    SELECT @MenuItemId AS MenuItemId, @IsAvailable AS IsAvailable;
END;
GO

/* 5. Customer ordering and history */
CREATE OR ALTER PROCEDURE food.usp_Order_Place
    @CustomerUserId INT,
    @RestaurantId INT,
    @AddressId INT = NULL,
    @DeliveryRecipient NVARCHAR(120),
    @DeliveryPhone NVARCHAR(30),
    @DeliveryAddressLine NVARCHAR(250),
    @DeliveryAddressLine2 NVARCHAR(250) = NULL,
    @DeliveryCity NVARCHAR(100),
    @DeliveryPostalCode NVARCHAR(20) = NULL,
    @DeliveryInstructions NVARCHAR(500) = NULL,
    @DeliveryFee DECIMAL(10,2) = 0,
    @TaxAmount DECIMAL(10,2) = 0,
    @PaymentMethod NVARCHAR(30) = N'Cash',
    @ItemsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users u INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
                   INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.RoleName = N'Customer'
                   WHERE u.UserId = @CustomerUserId AND u.IsActive = 1)
        THROW 51010, 'Active customer account was not found.', 1;
    IF NOT EXISTS (SELECT 1 FROM food.Restaurants WHERE RestaurantId = @RestaurantId AND IsActive = 1)
        THROW 51011, 'Restaurant was not found or is inactive.', 1;
    IF @AddressId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM food.Addresses WHERE AddressId = @AddressId AND UserId = @CustomerUserId)
        THROW 51012, 'Address does not belong to the customer.', 1;
    IF NULLIF(LTRIM(RTRIM(@DeliveryRecipient)), N'') IS NULL OR NULLIF(LTRIM(RTRIM(@DeliveryPhone)), N'') IS NULL
       OR NULLIF(LTRIM(RTRIM(@DeliveryAddressLine)), N'') IS NULL OR NULLIF(LTRIM(RTRIM(@DeliveryCity)), N'') IS NULL
        THROW 51013, 'Delivery recipient, phone, address, and city are required.', 1;
    IF @DeliveryFee < 0 OR @TaxAmount < 0 OR @PaymentMethod NOT IN (N'Cash', N'Card', N'UPI', N'Other')
        THROW 51014, 'Invalid fee, tax, or payment method.', 1;
    IF ISJSON(@ItemsJson) <> 1 THROW 51015, 'Order items must be valid JSON.', 1;

    DECLARE @Requested TABLE (MenuItemId INT NOT NULL PRIMARY KEY, Quantity INT NOT NULL);
    INSERT @Requested(MenuItemId, Quantity)
    SELECT MenuItemId, SUM(Quantity)
    FROM OPENJSON(@ItemsJson) WITH (MenuItemId INT N'$.menuItemId', Quantity INT N'$.quantity')
    GROUP BY MenuItemId;
    IF NOT EXISTS (SELECT 1 FROM @Requested) THROW 51016, 'At least one order item is required.', 1;
    IF EXISTS (SELECT 1 FROM @Requested WHERE MenuItemId IS NULL OR Quantity NOT BETWEEN 1 AND 50)
        THROW 51017, 'Each item must have a valid menu item ID and quantity from 1 to 50.', 1;

    DECLARE @Subtotal DECIMAL(10,2);
    IF EXISTS
    (
        SELECT 1 FROM @Requested q
        LEFT JOIN food.MenuItems mi ON mi.MenuItemId = q.MenuItemId
        WHERE mi.MenuItemId IS NULL OR mi.RestaurantId <> @RestaurantId OR mi.IsAvailable = 0
    ) THROW 51018, 'An item is unavailable or does not belong to this restaurant.', 1;

    SELECT @Subtotal = SUM(mi.Price * q.Quantity)
    FROM @Requested q INNER JOIN food.MenuItems mi ON mi.MenuItemId = q.MenuItemId;
    IF @Subtotal IS NULL OR @Subtotal <= 0 THROW 51019, 'Order subtotal must be positive.', 1;

    DECLARE @OrderId BIGINT, @OrderNumber NVARCHAR(32) = N'FD-' + CONVERT(NVARCHAR(8), GETDATE(), 112) + N'-' + UPPER(LEFT(REPLACE(CONVERT(NVARCHAR(36), NEWID()), N'-', N''), 12));
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT food.Orders
        (OrderNumber, CustomerUserId, RestaurantId, AddressId, DeliveryRecipient, DeliveryPhone,
         DeliveryAddressLine, DeliveryAddressLine2, DeliveryCity, DeliveryPostalCode, DeliveryInstructions,
         Subtotal, DeliveryFee, TaxAmount)
        VALUES
        (@OrderNumber, @CustomerUserId, @RestaurantId, @AddressId, @DeliveryRecipient, @DeliveryPhone,
         @DeliveryAddressLine, @DeliveryAddressLine2, @DeliveryCity, @DeliveryPostalCode, @DeliveryInstructions,
         @Subtotal, @DeliveryFee, @TaxAmount);
        SET @OrderId = SCOPE_IDENTITY();

        INSERT food.OrderItems(OrderId, MenuItemId, ItemName, UnitPrice, Quantity)
        SELECT @OrderId, mi.MenuItemId, mi.Name, mi.Price, q.Quantity
        FROM @Requested q INNER JOIN food.MenuItems mi ON mi.MenuItemId = q.MenuItemId;

        INSERT food.Payments(OrderId, Method, Amount, Status)
        VALUES (@OrderId, @PaymentMethod, @Subtotal + @DeliveryFee + @TaxAmount, N'Pending');
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;

    SELECT OrderId, OrderNumber, Subtotal, DeliveryFee, TaxAmount, TotalAmount, OrderStatus, CreatedAt
    FROM food.Orders WHERE OrderId = @OrderId;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Order_CustomerList
    @CustomerUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users u INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
                   INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.RoleName = N'Customer'
                   WHERE u.UserId = @CustomerUserId AND u.IsActive = 1)
        THROW 51021, 'Active customer account was not found.', 1;
    SELECT o.OrderId, o.OrderNumber, o.RestaurantId, r.Name AS RestaurantName,
           o.Subtotal, o.DeliveryFee, o.TaxAmount, o.TotalAmount, o.OrderStatus, o.CreatedAt,
           da.Status AS DeliveryStatus
    FROM food.Orders o
    INNER JOIN food.Restaurants r ON r.RestaurantId = o.RestaurantId
    LEFT JOIN food.DeliveryAssignments da ON da.OrderId = o.OrderId
    WHERE o.CustomerUserId = @CustomerUserId
    ORDER BY o.CreatedAt DESC;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Order_CustomerDetails
    @CustomerUserId INT,
    @OrderId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users u INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
                   INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.RoleName = N'Customer'
                   WHERE u.UserId = @CustomerUserId AND u.IsActive = 1)
        THROW 51022, 'Active customer account was not found.', 1;
    IF NOT EXISTS (SELECT 1 FROM food.Orders WHERE OrderId = @OrderId AND CustomerUserId = @CustomerUserId)
        THROW 51020, 'Order was not found for this customer.', 1;
    SELECT o.OrderId, o.OrderNumber, o.RestaurantId, r.Name AS RestaurantName,
           o.DeliveryRecipient, o.DeliveryPhone, o.DeliveryAddressLine, o.DeliveryAddressLine2,
           o.DeliveryCity, o.DeliveryPostalCode, o.DeliveryInstructions,
           o.Subtotal, o.DeliveryFee, o.TaxAmount, o.TotalAmount, o.OrderStatus, o.CreatedAt,
           da.AgentUserId, da.Status AS DeliveryStatus, da.PickedUpAt, da.DeliveredAt,
           p.Method AS PaymentMethod, p.Status AS PaymentStatus
    FROM food.Orders o
    INNER JOIN food.Restaurants r ON r.RestaurantId = o.RestaurantId
    LEFT JOIN food.DeliveryAssignments da ON da.OrderId = o.OrderId
    LEFT JOIN food.Payments p ON p.OrderId = o.OrderId
    WHERE o.OrderId = @OrderId;
    SELECT MenuItemId, ItemName, UnitPrice, Quantity, LineTotal
    FROM food.OrderItems WHERE OrderId = @OrderId ORDER BY OrderItemId;
END;
GO

/* 6. Restaurant order operations */
CREATE OR ALTER PROCEDURE food.usp_Order_RestaurantList
    @ActorUserId INT,
    @RestaurantId INT,
    @OrderStatus NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Restaurants r
                   INNER JOIN food.Users u ON u.UserId = r.OwnerUserId AND u.IsActive = 1
                   INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
                   INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.RoleName = N'Staff'
                   INNER JOIN food.StaffProfiles sp ON sp.UserId = u.UserId AND sp.StaffType = N'Restaurant'
                   WHERE r.RestaurantId = @RestaurantId AND r.OwnerUserId = @ActorUserId)
        THROW 51030, 'Actor does not own this restaurant.', 1;
    SELECT o.OrderId, o.OrderNumber, o.CustomerUserId, u.FullName AS CustomerName,
           o.DeliveryRecipient, o.DeliveryPhone, o.DeliveryAddressLine, o.DeliveryCity,
           o.Subtotal, o.DeliveryFee, o.TaxAmount, o.TotalAmount, o.OrderStatus, o.CreatedAt
    FROM food.Orders o INNER JOIN food.Users u ON u.UserId = o.CustomerUserId
    WHERE o.RestaurantId = @RestaurantId AND (@OrderStatus IS NULL OR o.OrderStatus = @OrderStatus)
    ORDER BY o.CreatedAt DESC;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Order_RestaurantSetStatus
    @ActorUserId INT,
    @OrderId BIGINT,
    @NewStatus NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF @NewStatus NOT IN (N'Accepted', N'Rejected', N'Preparing', N'Ready', N'Cancelled')
        THROW 51031, 'Restaurant status is not allowed.', 1;
    IF NOT EXISTS
       (SELECT 1 FROM food.Orders o INNER JOIN food.Restaurants r ON r.RestaurantId = o.RestaurantId
        INNER JOIN food.Users u ON u.UserId = r.OwnerUserId AND u.IsActive = 1
        INNER JOIN food.UserRoles ur ON ur.UserId = u.UserId AND ur.IsActive = 1
        INNER JOIN food.Roles role ON role.RoleId = ur.RoleId AND role.RoleName = N'Staff'
        INNER JOIN food.StaffProfiles sp ON sp.UserId = u.UserId AND sp.StaffType = N'Restaurant'
        WHERE o.OrderId = @OrderId AND r.OwnerUserId = @ActorUserId)
        THROW 51032, 'Order was not found for this restaurant owner.', 1;

    UPDATE food.Orders
    SET OrderStatus = @NewStatus, UpdatedAt = SYSUTCDATETIME()
    WHERE OrderId = @OrderId AND
       ((OrderStatus = N'Placed' AND @NewStatus IN (N'Accepted', N'Rejected', N'Cancelled'))
        OR (OrderStatus = N'Accepted' AND @NewStatus IN (N'Preparing', N'Cancelled'))
        OR (OrderStatus = N'Preparing' AND @NewStatus = N'Ready'));
    IF @@ROWCOUNT = 0 THROW 51033, 'Requested order status transition is not allowed.', 1;
    SELECT OrderId, OrderNumber, OrderStatus, UpdatedAt FROM food.Orders WHERE OrderId = @OrderId;
END;
GO

/* 7. Delivery assignment and status */
CREATE OR ALTER PROCEDURE food.usp_Delivery_Assign
    @ActorUserId INT,
    @OrderId BIGINT,
    @AgentUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @ActorUserId AND Role = N'Admin' AND IsActive = 1)
        THROW 51040, 'Only an active administrator can assign deliveries.', 1;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @AgentUserId AND Role = N'DeliveryAgent' AND IsActive = 1)
        THROW 51041, 'Active delivery agent was not found.', 1;
    IF NOT EXISTS (SELECT 1 FROM food.Orders WHERE OrderId = @OrderId AND OrderStatus = N'Ready')
        THROW 51042, 'Only ready orders can be assigned.', 1;
    IF EXISTS (SELECT 1 FROM food.DeliveryAssignments WHERE OrderId = @OrderId AND Status <> N'Failed')
        THROW 51043, 'Order already has an active or completed delivery assignment.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF EXISTS (SELECT 1 FROM food.DeliveryAssignments WHERE OrderId = @OrderId AND Status = N'Failed')
            UPDATE food.DeliveryAssignments
            SET AgentUserId = @AgentUserId, AssignedByUserId = @ActorUserId,
                Status = N'Assigned', AssignedAt = SYSUTCDATETIME(), PickedUpAt = NULL, DeliveredAt = NULL
            WHERE OrderId = @OrderId AND Status = N'Failed';
        ELSE
            INSERT food.DeliveryAssignments(OrderId, AgentUserId, AssignedByUserId)
            VALUES (@OrderId, @AgentUserId, @ActorUserId);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
    SELECT AssignmentId, OrderId, AgentUserId, Status, AssignedAt
    FROM food.DeliveryAssignments WHERE OrderId = @OrderId;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Delivery_AgentList
    @AgentUserId INT,
    @IncludeCompleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @AgentUserId AND Role = N'DeliveryAgent' AND IsActive = 1)
        THROW 51053, 'Active delivery agent account was not found.', 1;
    SELECT da.AssignmentId, da.OrderId, o.OrderNumber, da.Status, da.AssignedAt, da.PickedUpAt, da.DeliveredAt,
           r.Name AS RestaurantName, r.AddressLine AS PickupAddress,
           o.DeliveryRecipient, o.DeliveryPhone, o.DeliveryAddressLine, o.DeliveryAddressLine2, o.DeliveryCity, o.DeliveryPostalCode
    FROM food.DeliveryAssignments da
    INNER JOIN food.Orders o ON o.OrderId = da.OrderId
    INNER JOIN food.Restaurants r ON r.RestaurantId = o.RestaurantId
    WHERE da.AgentUserId = @AgentUserId AND (@IncludeCompleted = 1 OR da.Status IN (N'Assigned', N'PickedUp'))
    ORDER BY da.AssignedAt DESC;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Delivery_SetStatus
    @AgentUserId INT,
    @OrderId BIGINT,
    @NewStatus NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    IF @NewStatus NOT IN (N'PickedUp', N'Delivered', N'Failed')
        THROW 51045, 'Delivery status is not allowed.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        UPDATE food.DeliveryAssignments
        SET Status = @NewStatus,
            PickedUpAt = CASE WHEN @NewStatus = N'PickedUp' THEN SYSUTCDATETIME() ELSE PickedUpAt END,
            DeliveredAt = CASE WHEN @NewStatus = N'Delivered' THEN SYSUTCDATETIME() ELSE DeliveredAt END
        WHERE OrderId = @OrderId AND AgentUserId = @AgentUserId
          AND ((Status = N'Assigned' AND @NewStatus IN (N'PickedUp', N'Failed'))
            OR (Status = N'PickedUp' AND @NewStatus IN (N'Delivered', N'Failed')));
        IF @@ROWCOUNT = 0 THROW 51046, 'Assignment not found or delivery status transition is not allowed.', 1;

        IF @NewStatus = N'PickedUp'
        BEGIN
            UPDATE food.Orders SET OrderStatus = N'OutForDelivery', UpdatedAt = SYSUTCDATETIME()
            WHERE OrderId = @OrderId AND OrderStatus = N'Ready';
            IF @@ROWCOUNT = 0 THROW 51047, 'Order is not ready for pickup.', 1;
        END
        ELSE IF @NewStatus = N'Delivered'
        BEGIN
            UPDATE food.Orders SET OrderStatus = N'Delivered', UpdatedAt = SYSUTCDATETIME()
            WHERE OrderId = @OrderId AND OrderStatus = N'OutForDelivery';
            IF @@ROWCOUNT = 0 THROW 51048, 'Order is not out for delivery.', 1;
        END
        ELSE IF @NewStatus = N'Failed'
        BEGIN
            UPDATE food.Orders SET OrderStatus = N'Ready', UpdatedAt = SYSUTCDATETIME()
            WHERE OrderId = @OrderId AND OrderStatus = N'OutForDelivery';
            IF @@ROWCOUNT = 0 THROW 51049, 'Order is not out for delivery.', 1;
        END
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
    SELECT AssignmentId, OrderId, AgentUserId, Status, PickedUpAt, DeliveredAt
    FROM food.DeliveryAssignments WHERE OrderId = @OrderId;
END;
GO

/* 8. Reviews */
CREATE OR ALTER PROCEDURE food.usp_Review_Create
    @CustomerUserId INT,
    @OrderId BIGINT,
    @Rating TINYINT,
    @Comment NVARCHAR(1000) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @CustomerUserId AND Role = N'Customer' AND IsActive = 1)
        THROW 51054, 'Active customer account was not found.', 1;
    IF @Rating NOT BETWEEN 1 AND 5 THROW 51050, 'Rating must be from 1 through 5.', 1;
    IF NOT EXISTS (SELECT 1 FROM food.Orders WHERE OrderId = @OrderId AND CustomerUserId = @CustomerUserId AND OrderStatus = N'Delivered')
        THROW 51051, 'Only the customer of a delivered order may review it.', 1;
    IF EXISTS (SELECT 1 FROM food.Reviews WHERE OrderId = @OrderId)
        THROW 51052, 'This order has already been reviewed.', 1;
    INSERT food.Reviews(OrderId, CustomerUserId, RestaurantId, Rating, Comment)
    SELECT OrderId, CustomerUserId, RestaurantId, @Rating, @Comment
    FROM food.Orders WHERE OrderId = @OrderId;
    SELECT ReviewId, OrderId, RestaurantId, Rating, Comment, CreatedAt
    FROM food.Reviews WHERE ReviewId = SCOPE_IDENTITY();
END;
GO

/* 9. Account and role-workflow support */
CREATE OR ALTER PROCEDURE food.usp_Account_Register
    @FullName NVARCHAR(120),
    @Email NVARCHAR(254),
    @Phone NVARCHAR(30),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    IF NULLIF(LTRIM(RTRIM(@FullName)), N'') IS NULL OR NULLIF(LTRIM(RTRIM(@Email)), N'') IS NULL
       OR NULLIF(LTRIM(RTRIM(@PasswordHash)), N'') IS NULL
        THROW 51060, 'Name, email, and password hash are required.', 1;
    IF EXISTS (SELECT 1 FROM food.Users WHERE Email = @Email)
        THROW 51061, 'An account with this email already exists.', 1;
    INSERT food.Users(FullName, Email, Phone, PasswordHash, Role)
    VALUES (@FullName, LOWER(LTRIM(RTRIM(@Email))), @Phone, @PasswordHash, N'Customer');
    SELECT CONVERT(INT, SCOPE_IDENTITY()) AS UserId;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Account_FindByEmail
    @Email NVARCHAR(254)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, FullName, Email, PasswordHash, Role
    FROM food.Users
    WHERE Email = LOWER(LTRIM(RTRIM(@Email))) AND IsActive = 1;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Address_List
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AddressId, Label, RecipientName, Phone, AddressLine, AddressLine2, City,
           StateOrRegion, PostalCode, DeliveryInstructions, IsDefault
    FROM food.Addresses
    WHERE UserId = @UserId
    ORDER BY IsDefault DESC, CreatedAt DESC;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Address_Save
    @UserId INT,
    @Label NVARCHAR(50) = NULL,
    @RecipientName NVARCHAR(120),
    @Phone NVARCHAR(30),
    @AddressLine NVARCHAR(250),
    @AddressLine2 NVARCHAR(250) = NULL,
    @City NVARCHAR(100),
    @StateOrRegion NVARCHAR(100) = NULL,
    @PostalCode NVARCHAR(20) = NULL,
    @DeliveryInstructions NVARCHAR(500) = NULL,
    @IsDefault BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @UserId AND Role = N'Customer' AND IsActive = 1)
        THROW 51080, 'Active customer account was not found.', 1;
    IF NULLIF(LTRIM(RTRIM(@RecipientName)), N'') IS NULL OR NULLIF(LTRIM(RTRIM(@Phone)), N'') IS NULL
       OR NULLIF(LTRIM(RTRIM(@AddressLine)), N'') IS NULL OR NULLIF(LTRIM(RTRIM(@City)), N'') IS NULL
        THROW 51081, 'Recipient, phone, street address, and city are required.', 1;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF @IsDefault = 1 OR NOT EXISTS (SELECT 1 FROM food.Addresses WHERE UserId = @UserId AND IsDefault = 1)
            UPDATE food.Addresses SET IsDefault = 0 WHERE UserId = @UserId;
        INSERT food.Addresses(UserId, Label, RecipientName, Phone, AddressLine, AddressLine2, City, StateOrRegion,
                              PostalCode, DeliveryInstructions, IsDefault)
        VALUES (@UserId, @Label, @RecipientName, @Phone, @AddressLine, @AddressLine2, @City, @StateOrRegion,
                @PostalCode, @DeliveryInstructions,
                CASE WHEN @IsDefault = 1 OR NOT EXISTS (SELECT 1 FROM food.Addresses WHERE UserId = @UserId) THEN 1 ELSE 0 END);
        DECLARE @AddressId INT = CONVERT(INT, SCOPE_IDENTITY());
        COMMIT TRANSACTION;
        SELECT @AddressId AS AddressId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Restaurant_OwnedList
    @OwnerUserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RestaurantId, Name, Cuisine, City, IsActive
    FROM food.Restaurants
    WHERE OwnerUserId = @OwnerUserId
    ORDER BY Name;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Menu_OwnerList
    @ActorUserId INT,
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Restaurants r INNER JOIN food.Users u ON u.UserId = r.OwnerUserId
                   WHERE r.RestaurantId = @RestaurantId AND r.OwnerUserId = @ActorUserId AND u.Role = N'Restaurant' AND u.IsActive = 1)
        THROW 51062, 'Actor does not own this restaurant.', 1;
    SELECT MenuItemId, Name, Description, Category, Price, IsVegetarian, IsAvailable
    FROM food.MenuItems WHERE RestaurantId = @RestaurantId
    ORDER BY Category, Name;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Admin_ReadyOrders
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.OrderId, o.OrderNumber, r.Name AS RestaurantName, o.DeliveryCity,
           da.AgentUserId
    FROM food.Orders o
    INNER JOIN food.Restaurants r ON r.RestaurantId = o.RestaurantId
    LEFT JOIN food.DeliveryAssignments da ON da.OrderId = o.OrderId
    WHERE o.OrderStatus = N'Ready'
    ORDER BY o.CreatedAt;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Admin_AgentList
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, FullName
    FROM food.Users
    WHERE Role = N'DeliveryAgent' AND IsActive = 1
    ORDER BY FullName;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Admin_UserList
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, FullName, Email, Role, IsActive, CreatedAt
    FROM food.Users
    ORDER BY CreatedAt DESC;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Admin_UserUpdate
    @ActorUserId INT,
    @UserId INT,
    @Role NVARCHAR(20),
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @ActorUserId AND Role = N'Admin' AND IsActive = 1)
        THROW 51070, 'Only an active administrator can manage accounts.', 1;
    IF @Role NOT IN (N'Customer', N'Restaurant', N'DeliveryAgent', N'Admin')
        THROW 51071, 'Role is invalid.', 1;
    IF @UserId = @ActorUserId AND (@Role <> N'Admin' OR @IsActive = 0)
        THROW 51072, 'Administrators cannot disable or demote their own account.', 1;
    UPDATE food.Users SET Role = @Role, IsActive = @IsActive WHERE UserId = @UserId;
    IF @@ROWCOUNT = 0 THROW 51073, 'User was not found.', 1;
    SELECT UserId, FullName, Email, Role, IsActive FROM food.Users WHERE UserId = @UserId;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Admin_RestaurantList
AS
BEGIN
    SET NOCOUNT ON;
    SELECT r.RestaurantId, r.OwnerUserId, u.FullName AS OwnerName, r.Name, r.Cuisine, r.AddressLine, r.City, r.IsActive
    FROM food.Restaurants r INNER JOIN food.Users u ON u.UserId = r.OwnerUserId
    ORDER BY r.Name;
END;
GO

CREATE OR ALTER PROCEDURE food.usp_Admin_RestaurantSave
    @ActorUserId INT,
    @RestaurantId INT = NULL,
    @OwnerUserId INT,
    @Name NVARCHAR(160),
    @Cuisine NVARCHAR(80),
    @AddressLine NVARCHAR(250),
    @City NVARCHAR(100),
    @Description NVARCHAR(1000) = NULL,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @ActorUserId AND Role = N'Admin' AND IsActive = 1)
        THROW 51074, 'Only an active administrator can manage restaurants.', 1;
    IF NOT EXISTS (SELECT 1 FROM food.Users WHERE UserId = @OwnerUserId AND Role = N'Restaurant' AND IsActive = 1)
        THROW 51075, 'Restaurant owner must be an active restaurant account.', 1;
    IF NULLIF(LTRIM(RTRIM(@Name)), N'') IS NULL OR NULLIF(LTRIM(RTRIM(@Cuisine)), N'') IS NULL
       OR NULLIF(LTRIM(RTRIM(@AddressLine)), N'') IS NULL OR NULLIF(LTRIM(RTRIM(@City)), N'') IS NULL
        THROW 51076, 'Name, cuisine, address, and city are required.', 1;
    IF @RestaurantId IS NULL
    BEGIN
        INSERT food.Restaurants(OwnerUserId, Name, Cuisine, AddressLine, City, Description, IsActive)
        VALUES (@OwnerUserId, @Name, @Cuisine, @AddressLine, @City, @Description, @IsActive);
        SET @RestaurantId = CONVERT(INT, SCOPE_IDENTITY());
    END
    ELSE
    BEGIN
        UPDATE food.Restaurants
        SET OwnerUserId = @OwnerUserId, Name = @Name, Cuisine = @Cuisine, AddressLine = @AddressLine,
            City = @City, Description = @Description, IsActive = @IsActive
        WHERE RestaurantId = @RestaurantId;
        IF @@ROWCOUNT = 0 THROW 51077, 'Restaurant was not found.', 1;
    END;
    SELECT @RestaurantId AS RestaurantId;
END;
GO

/* 10. Optional demo seed data - uncomment and replace sample password hashes before use.
   PasswordHash values below are intentionally placeholders and cannot be used to log in.
*/
/*
IF NOT EXISTS (SELECT 1 FROM food.Users WHERE Email = N'demo.customer@example.test')
    INSERT food.Users(FullName, Email, PasswordHash, Role)
    VALUES (N'Demo Customer', N'demo.customer@example.test', N'REPLACE_WITH_REAL_PASSWORD_HASH', N'Customer');

IF NOT EXISTS (SELECT 1 FROM food.Users WHERE Email = N'demo.restaurant@example.test')
BEGIN
    INSERT food.Users(FullName, Email, PasswordHash, Role)
    VALUES (N'Demo Restaurant Owner', N'demo.restaurant@example.test', N'REPLACE_WITH_REAL_PASSWORD_HASH', N'Restaurant');
    DECLARE @DemoOwnerId INT = SCOPE_IDENTITY();
    INSERT food.Restaurants(OwnerUserId, Name, Cuisine, AddressLine, City)
    VALUES (@DemoOwnerId, N'Sample Kitchen', N'Indian', N'10 Market Road', N'Bengaluru');
    DECLARE @DemoRestaurantId INT = SCOPE_IDENTITY();
    INSERT food.MenuItems(RestaurantId, Name, Description, Category, Price, IsVegetarian, IsAvailable)
    VALUES (@DemoRestaurantId, N'Masala Dosa', N'Crisp dosa with potato masala', N'Breakfast', 120.00, 1, 1);
END;
*/

PRINT N'FoodDeliveryDB schema and stored procedures are ready.';
GO
