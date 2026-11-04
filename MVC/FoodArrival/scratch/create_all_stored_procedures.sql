USE FoodArrivalDB;
GO

--------------------------------------------------------------------------------
-- 1. USER & AUTHENTICATION PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_User_CheckDuplication', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_User_CheckDuplication;
GO
CREATE PROCEDURE dbo.sp_User_CheckDuplication
    @Username NVARCHAR(100),
    @Email NVARCHAR(200),
    @Reason NVARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @Reason = NULL;

    SELECT TOP 1 @Reason = Reason FROM (
        SELECT 'Username already taken' AS Reason FROM dbo.Customers WHERE LOWER(Username) = LOWER(@Username)
        UNION ALL
        SELECT 'Email already registered' FROM dbo.Customers WHERE LOWER(Email) = LOWER(@Email)
        UNION ALL
        SELECT 'Username already taken' FROM dbo.Restaurants WHERE LOWER(Username) = LOWER(@Username)
        UNION ALL
        SELECT 'Email already registered' FROM dbo.Restaurants WHERE LOWER(Email) = LOWER(@Email)
        UNION ALL
        SELECT 'Username already taken' FROM dbo.Riders WHERE LOWER(Username) = LOWER(@Username)
        UNION ALL
        SELECT 'Email already registered' FROM dbo.Riders WHERE LOWER(Email) = LOWER(@Email)
        UNION ALL
        SELECT 'Username already taken' FROM dbo.RestaurantStaff WHERE LOWER(Username) = LOWER(@Username)
        UNION ALL
        SELECT 'Email already registered' FROM dbo.RestaurantStaff WHERE LOWER(Email) = LOWER(@Email)
        UNION ALL
        SELECT 'Username already taken' FROM dbo.Admins WHERE LOWER(Username) = LOWER(@Username)
    ) AS Dups;
END;
GO

IF OBJECT_ID('dbo.sp_Admin_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_Login;
GO
CREATE PROCEDURE dbo.sp_Admin_Login
    @LoginId NVARCHAR(100),
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AdminId, AdminName, Username, Email, IsActive
    FROM dbo.Admins
    WHERE (LOWER(Email) = LOWER(@LoginId) OR LOWER(Username) = LOWER(@LoginId))
      AND PasswordHash = @Password
      AND IsActive = 1;
END;
GO

IF OBJECT_ID('dbo.sp_Customer_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_Insert;
GO
CREATE PROCEDURE dbo.sp_Customer_Insert
    @CustomerName NVARCHAR(100),
    @Username NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(200),
    @PhoneNumber NVARCHAR(20) = NULL,
    @Address NVARCHAR(500) = NULL,
    @CustomerId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Customers (CustomerName, Username, Email, PasswordHash, PhoneNumber, Address)
    VALUES (@CustomerName, LOWER(@Username), LOWER(@Email), @PasswordHash, @PhoneNumber, @Address);
    SET @CustomerId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

IF OBJECT_ID('dbo.sp_Customer_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_Login;
GO
CREATE PROCEDURE dbo.sp_Customer_Login
    @LoginId NVARCHAR(100),
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CustomerId, CustomerName, Username, Email, PhoneNumber, Address, IsActive
    FROM dbo.Customers
    WHERE (LOWER(Email) = LOWER(@LoginId) OR LOWER(Username) = LOWER(@LoginId))
      AND PasswordHash = @Password
      AND IsActive = 1;
END;
GO

IF OBJECT_ID('dbo.sp_Customer_ResetPassword', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Customer_ResetPassword;
GO
CREATE PROCEDURE dbo.sp_Customer_ResetPassword
    @CustomerId INT,
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Customers 
    SET PasswordHash = @Password 
    WHERE CustomerId = @CustomerId;
    SELECT @@ROWCOUNT;
END;
GO

--------------------------------------------------------------------------------
-- 2. ADMIN DASHBOARD & BULLETIN & TRANSACTION PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_Admin_GetDashboardSummary', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_GetDashboardSummary;
GO
CREATE PROCEDURE dbo.sp_Admin_GetDashboardSummary
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Pending') AS PendingOrders,
        (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Accepted') AS AcceptedOrders,
        (SELECT COUNT(*) FROM dbo.Orders WHERE OrderStatus = 'Rejected') AS RejectedOrders,
        (SELECT COUNT(*) FROM dbo.FoodItems WHERE IsAvailable = 1 AND IsVerified = 1) AS AvailableFoodItems,
        (SELECT COUNT(*) FROM dbo.Customers WHERE IsActive = 1) AS ActiveCustomers,
        (SELECT COUNT(*) FROM dbo.Restaurants WHERE IsActive = 1 AND IsVerified = 1) AS TotalRestaurants,
        (SELECT COUNT(*) FROM dbo.Riders WHERE IsActive = 1) AS TotalRiders,
        (SELECT COUNT(*) FROM dbo.RestaurantStaff WHERE IsActive = 1 AND IsVerified = 1) AS TotalStaff,
        (
            (SELECT COUNT(*) FROM dbo.Restaurants WHERE IsVerified = 0 AND IsActive = 1) +
            (SELECT COUNT(*) FROM dbo.RestaurantStaff WHERE IsVerified = 0 AND IsActive = 1) +
            (SELECT COUNT(*) FROM dbo.RiderRestaurantAffiliations WHERE IsVerified = 0) +
            (SELECT COUNT(*) FROM dbo.FoodItems WHERE IsVerified = 0)
        ) AS PendingApprovalsCount,
        ISNULL((SELECT SUM(Amount) FROM dbo.Transactions WHERE PaymentStatus = 'Success'), 0) AS TotalRevenue,
        ISNULL((SELECT SUM(PlatformFee) FROM dbo.Transactions WHERE PaymentStatus = 'Success'), 0) AS PlatformEarnings;
END;
GO

IF OBJECT_ID('dbo.sp_Admin_GetAllTransactions', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_GetAllTransactions;
GO
CREATE PROCEDURE dbo.sp_Admin_GetAllTransactions
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TransactionId, t.OrderId, t.TransactionReference, t.Amount, t.PaymentMethod, 
           t.PaymentStatus, t.RestaurantShare, t.PlatformFee, t.DeliveryFee, t.CreatedAt,
           c.CustomerName, r.RestaurantName
    FROM dbo.Transactions t
    INNER JOIN dbo.Orders o ON t.OrderId = o.OrderId
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    LEFT JOIN dbo.Restaurants r ON o.RestaurantId = r.RestaurantId
    ORDER BY t.TransactionId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Admin_GetBulletins', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_GetBulletins;
GO
CREATE PROCEDURE dbo.sp_Admin_GetBulletins
    @Audience NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT BulletinId, Title, Message, TargetAudience, Priority, CreatedByAdminId, CreatedAt, IsActive
    FROM dbo.Bulletins
    WHERE IsActive = 1
      AND (@Audience IS NULL OR TargetAudience = 'All' OR TargetAudience = @Audience)
    ORDER BY BulletinId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Admin_AddBulletin', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_AddBulletin;
GO
CREATE PROCEDURE dbo.sp_Admin_AddBulletin
    @Title NVARCHAR(200),
    @Message NVARCHAR(MAX),
    @TargetAudience NVARCHAR(50) = 'All',
    @Priority NVARCHAR(20) = 'Normal',
    @AdminId INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Bulletins (Title, Message, TargetAudience, Priority, CreatedByAdminId)
    VALUES (@Title, @Message, @TargetAudience, @Priority, @AdminId);
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Admin_DeleteBulletin', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Admin_DeleteBulletin;
GO
CREATE PROCEDURE dbo.sp_Admin_DeleteBulletin
    @BulletinId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Bulletins 
    SET IsActive = 0 
    WHERE BulletinId = @BulletinId;
    SELECT @@ROWCOUNT;
END;
GO

--------------------------------------------------------------------------------
-- 3. NOTIFICATION PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_Notification_GetForUser', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Notification_GetForUser;
GO
CREATE PROCEDURE dbo.sp_Notification_GetForUser
    @Role NVARCHAR(50),
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF LOWER(@Role) = 'admin'
    BEGIN
        SELECT TOP 15 NotificationId, UserId, UserRole, Title, Message, NotificationType, IsRead, CreatedAt, TargetUrl
        FROM dbo.Notifications
        WHERE UserRole = 'Admin'
        ORDER BY CreatedAt DESC;
    END
    ELSE
    BEGIN
        SELECT TOP 15 NotificationId, UserId, UserRole, Title, Message, NotificationType, IsRead, CreatedAt, TargetUrl
        FROM dbo.Notifications
        WHERE UserRole = @Role AND UserId = @UserId
        ORDER BY CreatedAt DESC;
    END
END;
GO

IF OBJECT_ID('dbo.sp_Notification_Add', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Notification_Add;
GO
CREATE PROCEDURE dbo.sp_Notification_Add
    @UserId INT = NULL,
    @UserRole NVARCHAR(50) = 'All',
    @Title NVARCHAR(200),
    @Message NVARCHAR(MAX),
    @Type NVARCHAR(50) = 'Info',
    @TargetUrl NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Notifications (UserId, UserRole, Title, Message, NotificationType, TargetUrl)
    VALUES (@UserId, @UserRole, @Title, @Message, @Type, @TargetUrl);
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Notification_MarkAllAsRead', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Notification_MarkAllAsRead;
GO
CREATE PROCEDURE dbo.sp_Notification_MarkAllAsRead
    @Role NVARCHAR(50),
    @UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF LOWER(@Role) = 'admin'
    BEGIN
        UPDATE dbo.Notifications SET IsRead = 1 WHERE UserRole = 'Admin';
    END
    ELSE
    BEGIN
        UPDATE dbo.Notifications SET IsRead = 1 WHERE UserRole = @Role AND UserId = @UserId;
    END
    SELECT @@ROWCOUNT;
END;
GO

--------------------------------------------------------------------------------
-- 4. FOOD ITEM & RESTAURANT PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_FoodItem_GetByRestaurant', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_FoodItem_GetByRestaurant;
GO
CREATE PROCEDURE dbo.sp_FoodItem_GetByRestaurant
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FoodItemId, ItemName, Description, Price, ImageUrl, IsAvailable, Category, IsVerified, CreatedAt
    FROM dbo.FoodItems
    WHERE (RestaurantId = @RestaurantId OR RestaurantId IS NULL)
    ORDER BY FoodItemId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_Insert;
GO
CREATE PROCEDURE dbo.sp_Restaurant_Insert
    @RestaurantName NVARCHAR(150),
    @OwnerName NVARCHAR(100),
    @Username NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(200),
    @PhoneNumber NVARCHAR(20),
    @Address NVARCHAR(500),
    @Cuisine NVARCHAR(100) = 'Multicuisine',
    @Latitude DECIMAL(9,6) = 22.572600,
    @Longitude DECIMAL(9,6) = 88.363900,
    @RestaurantId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Restaurants (RestaurantName, OwnerName, Username, Email, PasswordHash, PhoneNumber, Address, Cuisine, Latitude, Longitude)
    VALUES (@RestaurantName, @OwnerName, LOWER(@Username), LOWER(@Email), @PasswordHash, @PhoneNumber, @Address, @Cuisine, @Latitude, @Longitude);
    SET @RestaurantId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_Login;
GO
CREATE PROCEDURE dbo.sp_Restaurant_Login
    @LoginId NVARCHAR(100),
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, ImageUrl, IsVerified, IsActive, Latitude, Longitude
    FROM dbo.Restaurants
    WHERE (LOWER(Email) = LOWER(@LoginId) OR LOWER(Username) = LOWER(@LoginId))
      AND PasswordHash = @Password
      AND IsActive = 1;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_GetAll;
GO
CREATE PROCEDURE dbo.sp_Restaurant_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, ImageUrl, IsVerified, IsActive, Latitude, Longitude, CreatedAt
    FROM dbo.Restaurants
    ORDER BY RestaurantId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_GetAllVerified', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_GetAllVerified;
GO
CREATE PROCEDURE dbo.sp_Restaurant_GetAllVerified
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, ImageUrl, IsVerified, IsActive, Latitude, Longitude, CreatedAt
    FROM dbo.Restaurants
    WHERE IsActive = 1 AND IsVerified = 1
    ORDER BY RestaurantName ASC;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_GetPendingVerification', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_GetPendingVerification;
GO
CREATE PROCEDURE dbo.sp_Restaurant_GetPendingVerification
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, ImageUrl, IsVerified, IsActive, Latitude, Longitude, CreatedAt
    FROM dbo.Restaurants
    WHERE IsVerified = 0 AND IsActive = 1
    ORDER BY CreatedAt ASC;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_GetById;
GO
CREATE PROCEDURE dbo.sp_Restaurant_GetById
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RestaurantId, RestaurantName, OwnerName, Username, Email, PhoneNumber, Address, Cuisine, ImageUrl, IsVerified, IsActive, Latitude, Longitude, CreatedAt
    FROM dbo.Restaurants
    WHERE RestaurantId = @RestaurantId;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_Verify', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_Verify;
GO
CREATE PROCEDURE dbo.sp_Restaurant_Verify
    @RestaurantId INT,
    @IsApproved BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Restaurants 
    SET IsVerified = @IsApproved, IsActive = CASE WHEN @IsApproved = 1 THEN 1 ELSE 0 END 
    WHERE RestaurantId = @RestaurantId;
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_GetMenu', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_GetMenu;
GO
CREATE PROCEDURE dbo.sp_Restaurant_GetMenu
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FoodItemId, ItemName, Description, Price, ImageUrl, IsAvailable, Category, IsVerified, CreatedAt
    FROM dbo.FoodItems
    WHERE RestaurantId = @RestaurantId
    ORDER BY FoodItemId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_GetOrders', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_GetOrders;
GO
CREATE PROCEDURE dbo.sp_Restaurant_GetOrders
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, 
           ISNULL(o.DeliveryAddress, c.Address) AS Address,
           ISNULL(oso.SubTotal, o.TotalAmount) AS TotalAmount,
           o.OrderStatus, 
           ISNULL(oso.DeliveryStatus, o.DeliveryStatus) AS DeliveryStatus,
           o.OrderedAt, o.ProcessedAt,
           r.RiderName,
           STUFF((
               SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
               FROM dbo.OrderDetails od
               INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
               WHERE od.OrderId = o.OrderId AND (f.RestaurantId = @RestaurantId OR f.RestaurantId IS NULL)
               FOR XML PATH(''), TYPE
           ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    LEFT JOIN dbo.OrderSubOrders oso ON o.OrderId = oso.OrderId AND oso.RestaurantId = @RestaurantId
    LEFT JOIN dbo.Riders r ON ISNULL(oso.RiderId, o.RiderId) = r.RiderId
    WHERE (o.RestaurantId = @RestaurantId OR EXISTS (SELECT 1 FROM dbo.OrderSubOrders s WHERE s.OrderId = o.OrderId AND s.RestaurantId = @RestaurantId))
    ORDER BY o.OrderId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_UpdateDeliveryStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_UpdateDeliveryStatus;
GO
CREATE PROCEDURE dbo.sp_Restaurant_UpdateDeliveryStatus
    @OrderId INT,
    @Status NVARCHAR(50),
    @RestaurantId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Orders 
    SET DeliveryStatus = @Status,
        OrderStatus = CASE WHEN @Status IN ('ReadyForPickup', 'PickedUp', 'OutForDelivery', 'Delivered') THEN 'Accepted' ELSE OrderStatus END
    WHERE OrderId = @OrderId;

    IF @RestaurantId IS NOT NULL
    BEGIN
        UPDATE dbo.OrderSubOrders
        SET PreparationStatus = @Status,
            DeliveryStatus = CASE WHEN @Status = 'ReadyForPickup' THEN 'ReadyForPickup' ELSE DeliveryStatus END
        WHERE OrderId = @OrderId AND RestaurantId = @RestaurantId;
    END
    ELSE
    BEGIN
        UPDATE dbo.OrderSubOrders
        SET PreparationStatus = @Status,
            DeliveryStatus = CASE WHEN @Status = 'ReadyForPickup' THEN 'ReadyForPickup' ELSE DeliveryStatus END
        WHERE OrderId = @OrderId;
    END
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_GetTransactions', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_GetTransactions;
GO
CREATE PROCEDURE dbo.sp_Restaurant_GetTransactions
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TransactionId, t.OrderId, t.TransactionReference, t.Amount, t.PaymentMethod, 
           t.PaymentStatus, t.RestaurantShare, t.PlatformFee, t.DeliveryFee, t.CreatedAt,
           c.CustomerName
    FROM dbo.Transactions t
    INNER JOIN dbo.Orders o ON t.OrderId = o.OrderId
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.RestaurantId = @RestaurantId 
       OR EXISTS (SELECT 1 FROM dbo.OrderSubOrders s WHERE s.OrderId = o.OrderId AND s.RestaurantId = @RestaurantId)
    ORDER BY t.TransactionId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Restaurant_ResetPassword', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Restaurant_ResetPassword;
GO
CREATE PROCEDURE dbo.sp_Restaurant_ResetPassword
    @RestaurantId INT,
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Restaurants 
    SET PasswordHash = @Password 
    WHERE RestaurantId = @RestaurantId;
    SELECT @@ROWCOUNT;
END;
GO

--------------------------------------------------------------------------------
-- 5. RIDER PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_Rider_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_Insert;
GO
CREATE PROCEDURE dbo.sp_Rider_Insert
    @RiderName NVARCHAR(100),
    @Username NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(200),
    @PhoneNumber NVARCHAR(20),
    @VehicleType NVARCHAR(50),
    @LicenseNumber NVARCHAR(100) = NULL,
    @LicenseDocumentUrl NVARCHAR(500) = NULL,
    @RiderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Riders (RiderName, Username, Email, PasswordHash, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl)
    VALUES (@RiderName, LOWER(@Username), LOWER(@Email), @PasswordHash, @PhoneNumber, @VehicleType, @LicenseNumber, @LicenseDocumentUrl);
    SET @RiderId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

IF OBJECT_ID('dbo.sp_Rider_AddAffiliation', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_AddAffiliation;
GO
CREATE PROCEDURE dbo.sp_Rider_AddAffiliation
    @RiderId INT,
    @RestaurantId INT,
    @AffiliationId INT OUTPUT,
    @AlreadyExists BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @AlreadyExists = 0;
    SET @AffiliationId = 0;

    IF EXISTS (SELECT 1 FROM dbo.RiderRestaurantAffiliations WHERE RiderId = @RiderId AND RestaurantId = @RestaurantId)
    BEGIN
        SET @AlreadyExists = 1;
        RETURN;
    END

    INSERT INTO dbo.RiderRestaurantAffiliations (RiderId, RestaurantId, IsVerified)
    VALUES (@RiderId, @RestaurantId, 0);
    SET @AffiliationId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

IF OBJECT_ID('dbo.sp_Rider_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_Login;
GO
CREATE PROCEDURE dbo.sp_Rider_Login
    @LoginId NVARCHAR(100),
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RiderId, RiderName, Username, Email, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl, CurrentStatus, IsActive, Latitude, Longitude, MaxCapacity, Rating
    FROM dbo.Riders
    WHERE (LOWER(Email) = LOWER(@LoginId) OR LOWER(Username) = LOWER(@LoginId))
      AND PasswordHash = @Password
      AND IsActive = 1;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_GetAll;
GO
CREATE PROCEDURE dbo.sp_Rider_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RiderId, RiderName, Username, Email, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl, CurrentStatus, IsActive, CreatedAt
    FROM dbo.Riders
    ORDER BY RiderId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_GetById;
GO
CREATE PROCEDURE dbo.sp_Rider_GetById
    @RiderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT RiderId, RiderName, Username, Email, PhoneNumber, VehicleType, LicenseNumber, LicenseDocumentUrl, CurrentStatus, IsActive, CreatedAt, Latitude, Longitude, MaxCapacity, Rating
    FROM dbo.Riders
    WHERE RiderId = @RiderId;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_GetAffiliations', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_GetAffiliations;
GO
CREATE PROCEDURE dbo.sp_Rider_GetAffiliations
    @RiderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.AffiliationId, a.RiderId, a.RestaurantId, a.IsVerified, a.VerifiedByRole, a.VerifiedAt, a.AffiliatedAt,
           r.RestaurantName, r.OwnerName, r.Address AS RestaurantAddress, r.PhoneNumber AS RestaurantPhone
    FROM dbo.RiderRestaurantAffiliations a
    INNER JOIN dbo.Restaurants r ON a.RestaurantId = r.RestaurantId
    WHERE a.RiderId = @RiderId
    ORDER BY a.AffiliationId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_GetByRestaurant', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_GetByRestaurant;
GO
CREATE PROCEDURE dbo.sp_Rider_GetByRestaurant
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.AffiliationId, a.RiderId, a.RestaurantId, a.IsVerified, a.VerifiedByRole, a.VerifiedAt, a.AffiliatedAt,
           rd.RiderName, rd.PhoneNumber, rd.VehicleType, rd.LicenseNumber, rd.LicenseDocumentUrl, rd.CurrentStatus
    FROM dbo.RiderRestaurantAffiliations a
    INNER JOIN dbo.Riders rd ON a.RiderId = rd.RiderId
    WHERE a.RestaurantId = @RestaurantId
    ORDER BY a.AffiliationId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_GetPendingAffiliations', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_GetPendingAffiliations;
GO
CREATE PROCEDURE dbo.sp_Rider_GetPendingAffiliations
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.AffiliationId, a.RiderId, a.RestaurantId, a.IsVerified, a.AffiliatedAt,
           rd.RiderName, rd.PhoneNumber, rd.VehicleType, rd.LicenseNumber, rd.LicenseDocumentUrl,
           r.RestaurantName
    FROM dbo.RiderRestaurantAffiliations a
    INNER JOIN dbo.Riders rd ON a.RiderId = rd.RiderId
    INNER JOIN dbo.Restaurants r ON a.RestaurantId = r.RestaurantId
    WHERE a.IsVerified = 0
    ORDER BY a.AffiliatedAt ASC;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_VerifyAffiliation', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_VerifyAffiliation;
GO
CREATE PROCEDURE dbo.sp_Rider_VerifyAffiliation
    @AffiliationId INT,
    @IsApproved BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF @IsApproved = 1
    BEGIN
        UPDATE dbo.RiderRestaurantAffiliations 
        SET IsVerified = 1, VerifiedByRole = 'Admin', VerifiedAt = GETDATE()
        WHERE AffiliationId = @AffiliationId;
    END
    ELSE
    BEGIN
        DELETE FROM dbo.RiderRestaurantAffiliations WHERE AffiliationId = @AffiliationId;
    END
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_RemoveAffiliation', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_RemoveAffiliation;
GO
CREATE PROCEDURE dbo.sp_Rider_RemoveAffiliation
    @RiderId INT,
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.RiderRestaurantAffiliations WHERE RiderId = @RiderId AND RestaurantId = @RestaurantId;
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_GetAvailableDeliveries', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_GetAvailableDeliveries;
GO
CREATE PROCEDURE dbo.sp_Rider_GetAvailableDeliveries
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, 
           ISNULL(o.DeliveryAddress, c.Address) AS Address, o.TotalAmount, 
           o.OrderStatus, o.DeliveryStatus, o.OrderedAt, 
           ISNULL(r.RestaurantName, 'FoodReady Kitchen') AS RestaurantName,
           STUFF((
               SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
               FROM dbo.OrderDetails od
               INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
               WHERE od.OrderId = o.OrderId
               FOR XML PATH(''), TYPE
           ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    LEFT JOIN dbo.Restaurants r ON o.RestaurantId = r.RestaurantId
    WHERE o.OrderStatus NOT IN ('Cancelled', 'Rejected')
      AND o.DeliveryStatus != 'Delivered'
      AND (
          o.DeliveryStatus IN ('ReadyForPickup', 'PendingPickup', 'Pending', 'Accepted')
          OR EXISTS (
              SELECT 1 FROM dbo.OrderSubOrders oso 
              WHERE oso.OrderId = o.OrderId 
                AND oso.PreparationStatus = 'ReadyForPickup'
          )
      )
    ORDER BY o.OrderedAt ASC;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_GetMyDeliveries', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_GetMyDeliveries;
GO
CREATE PROCEDURE dbo.sp_Rider_GetMyDeliveries
    @RiderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.OrderId, o.CustomerId, c.CustomerName, c.PhoneNumber, 
           ISNULL(o.DeliveryAddress, c.Address) AS Address, o.TotalAmount, 
           o.OrderStatus, o.DeliveryStatus, o.OrderedAt, o.ProcessedAt,
           STUFF((
               SELECT ', ' + f.ItemName + ' x' + CAST(od.Quantity AS NVARCHAR(10))
               FROM dbo.OrderDetails od
               INNER JOIN dbo.FoodItems f ON od.FoodItemId = f.FoodItemId
               WHERE od.OrderId = o.OrderId
               FOR XML PATH(''), TYPE
           ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS Items
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE (
        o.RiderId = @RiderId 
        OR EXISTS (
            SELECT 1 FROM dbo.OrderSubOrders oso 
            WHERE oso.OrderId = o.OrderId AND oso.RiderId = @RiderId
        )
    )
    AND o.OrderStatus NOT IN ('Cancelled', 'Rejected')
    AND (o.DeliveryStatus != 'Delivered' OR o.OrderedAt >= DATEADD(day, -1, GETDATE()))
    ORDER BY CASE WHEN o.DeliveryStatus = 'Delivered' THEN 1 ELSE 0 END, o.OrderedAt DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_AcceptDelivery', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_AcceptDelivery;
GO
CREATE PROCEDURE dbo.sp_Rider_AcceptDelivery
    @OrderId INT,
    @RiderId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Orders 
    SET RiderId = @RiderId, DeliveryStatus = 'OutForDelivery' 
    WHERE OrderId = @OrderId;

    UPDATE dbo.OrderSubOrders
    SET RiderId = @RiderId, DeliveryStatus = 'OutForDelivery'
    WHERE OrderId = @OrderId;

    UPDATE dbo.Riders SET CurrentStatus = 'On Delivery' WHERE RiderId = @RiderId;
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_CompleteDeliveryWithOtp', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_CompleteDeliveryWithOtp;
GO
CREATE PROCEDURE dbo.sp_Rider_CompleteDeliveryWithOtp
    @OrderId INT,
    @RiderId INT,
    @EnteredOtp NVARCHAR(10),
    @ErrorMessage NVARCHAR(300) OUTPUT,
    @IsSuccess BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @ErrorMessage = NULL;
    SET @IsSuccess = 0;

    DECLARE @ExpectedOtp NVARCHAR(10);
    DECLARE @CurrentStatus NVARCHAR(50);

    SELECT @ExpectedOtp = DeliveryOtp, @CurrentStatus = DeliveryStatus
    FROM dbo.Orders
    WHERE OrderId = @OrderId;

    IF @CurrentStatus IS NULL
    BEGIN
        SET @ErrorMessage = 'Order not found.';
        RETURN;
    END

    IF @CurrentStatus = 'Delivered'
    BEGIN
        SET @ErrorMessage = 'This order has already been marked as delivered.';
        RETURN;
    END

    IF @ExpectedOtp IS NULL
    BEGIN
        SET @ErrorMessage = 'No delivery OTP was set for this order. Please contact support.';
        RETURN;
    END

    IF @ExpectedOtp <> @EnteredOtp
    BEGIN
        SET @ErrorMessage = 'Invalid Delivery OTP! The code you entered does not match the customer''s PIN. Please verify with the customer.';
        RETURN;
    END

    -- OTP Matched! Confirm delivery
    UPDATE dbo.Orders 
    SET DeliveryStatus = 'Delivered', OrderStatus = 'Accepted', ProcessedAt = GETDATE() 
    WHERE OrderId = @OrderId;

    UPDATE dbo.OrderSubOrders
    SET DeliveryStatus = 'Delivered', DeliveredAt = GETDATE()
    WHERE OrderId = @OrderId;

    UPDATE dbo.Riders SET CurrentStatus = 'Available' WHERE RiderId = @RiderId;

    INSERT INTO dbo.OrderStatusHistory (OrderId, Status, ChangedByUserId, ChangedByRole, Message, CreatedAt)
    VALUES (@OrderId, 'Delivered', @RiderId, 'Rider', 'Handover confirmed via customer 6-digit OTP verification.', GETDATE());

    SET @IsSuccess = 1;
END;
GO

IF OBJECT_ID('dbo.sp_Rider_ResetPassword', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Rider_ResetPassword;
GO
CREATE PROCEDURE dbo.sp_Rider_ResetPassword
    @RiderId INT,
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Riders 
    SET PasswordHash = @Password 
    WHERE RiderId = @RiderId;
    SELECT @@ROWCOUNT;
END;
GO

--------------------------------------------------------------------------------
-- 6. RESTAURANT STAFF PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_Staff_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_Insert;
GO
CREATE PROCEDURE dbo.sp_Staff_Insert
    @StaffName NVARCHAR(100),
    @Username NVARCHAR(100),
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(200),
    @PhoneNumber NVARCHAR(20),
    @RestaurantId INT,
    @RoleTitle NVARCHAR(100) = 'Kitchen Staff',
    @StaffId INT OUTPUT,
    @StaffCode NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Count INT;
    SELECT @Count = COUNT(*) FROM dbo.RestaurantStaff WHERE RestaurantId = @RestaurantId;
    SET @StaffCode = 'STF-REST' + CAST(@RestaurantId AS NVARCHAR(10)) + '-' + RIGHT('000' + CAST(@Count + 1 AS NVARCHAR(10)), 3);

    INSERT INTO dbo.RestaurantStaff (StaffCode, StaffName, Username, Email, PasswordHash, PhoneNumber, RestaurantId, RoleTitle)
    VALUES (@StaffCode, @StaffName, LOWER(@Username), LOWER(@Email), @PasswordHash, @PhoneNumber, @RestaurantId, @RoleTitle);
    SET @StaffId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

IF OBJECT_ID('dbo.sp_Staff_Login', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_Login;
GO
CREATE PROCEDURE dbo.sp_Staff_Login
    @LoginId NVARCHAR(100),
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId, s.RoleTitle, s.IsVerified, s.IsActive,
           r.RestaurantName
    FROM dbo.RestaurantStaff s
    INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
    WHERE (LOWER(s.Email) = LOWER(@LoginId) OR LOWER(s.Username) = LOWER(@LoginId))
      AND s.PasswordHash = @Password
      AND s.IsActive = 1;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_GetById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_GetById;
GO
CREATE PROCEDURE dbo.sp_Staff_GetById
    @StaffId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId, s.RoleTitle, s.IsVerified, s.IsActive, s.CreatedAt,
           r.RestaurantName
    FROM dbo.RestaurantStaff s
    INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
    WHERE s.StaffId = @StaffId;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_GetByRestaurant', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_GetByRestaurant;
GO
CREATE PROCEDURE dbo.sp_Staff_GetByRestaurant
    @RestaurantId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId, s.RoleTitle, s.IsVerified, s.IsActive, s.CreatedAt,
           r.RestaurantName
    FROM dbo.RestaurantStaff s
    INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
    WHERE s.RestaurantId = @RestaurantId
    ORDER BY s.StaffId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_GetAll', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_GetAll;
GO
CREATE PROCEDURE dbo.sp_Staff_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId, s.RoleTitle, s.IsVerified, s.IsActive, s.CreatedAt,
           r.RestaurantName
    FROM dbo.RestaurantStaff s
    INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
    ORDER BY s.StaffId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_GetPendingVerification', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_GetPendingVerification;
GO
CREATE PROCEDURE dbo.sp_Staff_GetPendingVerification
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.StaffId, s.StaffCode, s.StaffName, s.Username, s.Email, s.PhoneNumber, s.RestaurantId, s.RoleTitle, s.IsVerified, s.IsActive, s.CreatedAt,
           r.RestaurantName
    FROM dbo.RestaurantStaff s
    INNER JOIN dbo.Restaurants r ON s.RestaurantId = r.RestaurantId
    WHERE s.IsVerified = 0 AND s.IsActive = 1
    ORDER BY s.CreatedAt ASC;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_Verify', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_Verify;
GO
CREATE PROCEDURE dbo.sp_Staff_Verify
    @StaffId INT,
    @IsApproved BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.RestaurantStaff 
    SET IsVerified = @IsApproved, IsActive = CASE WHEN @IsApproved = 1 THEN 1 ELSE 0 END 
    WHERE StaffId = @StaffId;
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_ProposeDish', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_ProposeDish;
GO
CREATE PROCEDURE dbo.sp_Staff_ProposeDish
    @ItemName NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @Price DECIMAL(10,2),
    @Category NVARCHAR(50) = 'Snacks',
    @ImageUrl NVARCHAR(500) = NULL,
    @RestaurantId INT,
    @ProposedByStaffId INT,
    @FoodItemId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.FoodItems (ItemName, Description, Price, ImageUrl, IsAvailable, Category, RestaurantId, SubmittedByStaffId, IsVerified)
    VALUES (@ItemName, @Description, @Price, @ImageUrl, 0, @Category, @RestaurantId, @ProposedByStaffId, 0);
    SET @FoodItemId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

IF OBJECT_ID('dbo.sp_Staff_GetPendingDishes', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_GetPendingDishes;
GO
CREATE PROCEDURE dbo.sp_Staff_GetPendingDishes
    @RestaurantId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT f.FoodItemId, f.ItemName, f.Description, f.Price, f.ImageUrl, f.Category, f.IsVerified, f.CreatedAt, f.RestaurantId,
           r.RestaurantName, s.StaffName AS ProposedByStaffName
    FROM dbo.FoodItems f
    LEFT JOIN dbo.Restaurants r ON f.RestaurantId = r.RestaurantId
    LEFT JOIN dbo.RestaurantStaff s ON f.SubmittedByStaffId = s.StaffId
    WHERE f.IsVerified = 0
      AND (@RestaurantId IS NULL OR f.RestaurantId = @RestaurantId)
    ORDER BY f.CreatedAt DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_VerifyDish', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_VerifyDish;
GO
CREATE PROCEDURE dbo.sp_Staff_VerifyDish
    @FoodItemId INT,
    @IsApproved BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.FoodItems 
    SET IsVerified = @IsApproved, IsAvailable = @IsApproved 
    WHERE FoodItemId = @FoodItemId;
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_ToggleDishAvailability', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_ToggleDishAvailability;
GO
CREATE PROCEDURE dbo.sp_Staff_ToggleDishAvailability
    @FoodItemId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.FoodItems 
    SET IsAvailable = CASE WHEN IsAvailable = 1 THEN 0 ELSE 1 END 
    WHERE FoodItemId = @FoodItemId;
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_GetDishesByStaff', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_GetDishesByStaff;
GO
CREATE PROCEDURE dbo.sp_Staff_GetDishesByStaff
    @StaffId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT FoodItemId, ItemName, Description, Price, ImageUrl, IsAvailable, Category, IsVerified, CreatedAt
    FROM dbo.FoodItems
    WHERE SubmittedByStaffId = @StaffId
    ORDER BY FoodItemId DESC;
END;
GO

IF OBJECT_ID('dbo.sp_Staff_ResetPassword', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Staff_ResetPassword;
GO
CREATE PROCEDURE dbo.sp_Staff_ResetPassword
    @StaffId INT,
    @Password NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.RestaurantStaff 
    SET PasswordHash = @Password 
    WHERE StaffId = @StaffId;
    SELECT @@ROWCOUNT;
END;
GO

--------------------------------------------------------------------------------
-- 7. ORDER CHECKOUT, SUBORDERS & RATINGS PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_Order_CreateMaster', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_CreateMaster;
GO
CREATE PROCEDURE dbo.sp_Order_CreateMaster
    @CustomerId INT,
    @TotalAmount DECIMAL(10,2),
    @DeliveryFee DECIMAL(10,2),
    @DeliveryAddress NVARCHAR(500),
    @PaymentMethod NVARCHAR(50),
    @DeliveryOtp NVARCHAR(10),
    @NewOrderId INT OUTPUT,
    @OrderNumber NVARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Orders 
    (CustomerId, TotalAmount, OrderStatus, OrderedAt, DeliveryFee, DeliveryAddress, PaymentStatus, PaymentMethod, DeliveryStatus, EstimatedDeliveryTime, DeliveryOtp)
    VALUES 
    (@CustomerId, @TotalAmount, 'Pending', GETDATE(), @DeliveryFee, @DeliveryAddress, 'Pending', @PaymentMethod, 'Pending', DATEADD(MINUTE, 35, GETDATE()), @DeliveryOtp);
    
    SET @NewOrderId = CAST(SCOPE_IDENTITY() AS INT);
    SET @OrderNumber = 'FR' + CAST(100000 + @NewOrderId AS NVARCHAR(20));

    UPDATE dbo.Orders 
    SET OrderNumber = @OrderNumber 
    WHERE OrderId = @NewOrderId;
END;
GO

IF OBJECT_ID('dbo.sp_Order_CreateSubOrder', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_CreateSubOrder;
GO
CREATE PROCEDURE dbo.sp_Order_CreateSubOrder
    @OrderId INT,
    @RestaurantId INT,
    @SubTotal DECIMAL(10,2),
    @PrepMinutes INT = 15,
    @DeliveryOtp NVARCHAR(10) = NULL,
    @SubOrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.OrderSubOrders 
    (OrderId, RestaurantId, SubTotal, PreparationStatus, EstimatedReadyAt, DeliveryStatus, PickupSequence, CreatedAt, DeliveryOtp)
    VALUES 
    (@OrderId, @RestaurantId, @SubTotal, 'Confirmed', DATEADD(MINUTE, @PrepMinutes, GETDATE()), 'Pending', 1, GETDATE(), @DeliveryOtp);
    
    SET @SubOrderId = CAST(SCOPE_IDENTITY() AS INT);
END;
GO

IF OBJECT_ID('dbo.sp_Order_CreateDetail', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_CreateDetail;
GO
CREATE PROCEDURE dbo.sp_Order_CreateDetail
    @OrderId INT,
    @SubOrderId INT = NULL,
    @FoodItemId INT,
    @Quantity INT,
    @UnitPrice DECIMAL(10,2),
    @SubTotal DECIMAL(10,2)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.OrderDetails (OrderId, SubOrderId, FoodItemId, Quantity, UnitPrice, SubTotal)
    VALUES (@OrderId, @SubOrderId, @FoodItemId, @Quantity, @UnitPrice, @SubTotal);
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_OrderStatusHistory_Add', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_OrderStatusHistory_Add;
GO
CREATE PROCEDURE dbo.sp_OrderStatusHistory_Add
    @OrderId INT,
    @SubOrderId INT = NULL,
    @Status NVARCHAR(50),
    @ChangedByUserId INT = NULL,
    @ChangedByRole NVARCHAR(50) = 'System',
    @Message NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.OrderStatusHistory (OrderId, SubOrderId, Status, ChangedByUserId, ChangedByRole, Message, CreatedAt)
    VALUES (@OrderId, @SubOrderId, @Status, @ChangedByUserId, @ChangedByRole, @Message, GETDATE());
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Transaction_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Transaction_Insert;
GO
CREATE PROCEDURE dbo.sp_Transaction_Insert
    @OrderId INT,
    @TransactionReference NVARCHAR(100),
    @Amount DECIMAL(10,2),
    @PaymentMethod NVARCHAR(50),
    @PaymentStatus NVARCHAR(50) = 'Success',
    @RestaurantShare DECIMAL(10,2) = 0,
    @PlatformFee DECIMAL(10,2) = 15.00,
    @DeliveryFee DECIMAL(10,2) = 40.00,
    @GatewayResponse NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Transactions 
    (OrderId, TransactionReference, Amount, PaymentMethod, PaymentStatus, RestaurantShare, PlatformFee, DeliveryFee, GatewayResponse, CreatedAt)
    VALUES 
    (@OrderId, @TransactionReference, @Amount, @PaymentMethod, @PaymentStatus, @RestaurantShare, @PlatformFee, @DeliveryFee, @GatewayResponse, GETDATE());
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Order_SyncAssignedRider', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_SyncAssignedRider;
GO
CREATE PROCEDURE dbo.sp_Order_SyncAssignedRider
    @OrderId INT,
    @AssignedRiderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @AssignedRiderId = NULL;

    SELECT TOP 1 @AssignedRiderId = RiderId 
    FROM dbo.OrderSubOrders 
    WHERE OrderId = @OrderId AND RiderId IS NOT NULL;

    IF @AssignedRiderId IS NOT NULL
    BEGIN
        UPDATE dbo.Orders 
        SET RiderId = @AssignedRiderId 
        WHERE OrderId = @OrderId AND RiderId IS NULL;
    END
END;
GO

IF OBJECT_ID('dbo.sp_OrderRating_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_OrderRating_Insert;
GO
CREATE PROCEDURE dbo.sp_OrderRating_Insert
    @OrderId INT,
    @CustomerId INT,
    @SubOrderId INT = NULL,
    @FoodRating INT,
    @RestaurantRating INT,
    @RiderRating INT,
    @ReviewText NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.OrderRatings (OrderId, CustomerId, SubOrderId, FoodRating, RestaurantRating, RiderRating, ReviewText, CreatedAt)
    VALUES (@OrderId, @CustomerId, @SubOrderId, @FoodRating, @RestaurantRating, @RiderRating, @ReviewText, GETDATE());
    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Order_Cancel', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_Cancel;
GO
CREATE PROCEDURE dbo.sp_Order_Cancel
    @OrderId INT,
    @CustomerId INT,
    @Reason NVARCHAR(500) = 'Cancelled by customer'
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Orders 
    SET OrderStatus = 'Cancelled', RejectionReason = @Reason, ProcessedAt = GETDATE()
    WHERE OrderId = @OrderId AND CustomerId = @CustomerId 
      AND OrderStatus IN ('Pending', 'Confirmed');

    DECLARE @Rows INT = @@ROWCOUNT;
    IF @Rows > 0
    BEGIN
        UPDATE dbo.OrderSubOrders
        SET PreparationStatus = 'Cancelled', DeliveryStatus = 'Cancelled'
        WHERE OrderId = @OrderId;

        EXEC dbo.sp_OrderStatusHistory_Add @OrderId = @OrderId, @SubOrderId = NULL, @Status = 'Cancelled', @ChangedByUserId = @CustomerId, @ChangedByRole = 'Customer', @Message = @Reason;
    END
    SELECT @Rows;
END;
GO

IF OBJECT_ID('dbo.sp_OrderSubOrder_ReassignRider', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_OrderSubOrder_ReassignRider;
GO
CREATE PROCEDURE dbo.sp_OrderSubOrder_ReassignRider
    @SubOrderId INT,
    @NewRiderId INT,
    @Reason NVARCHAR(200) = 'Operational rebalance'
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.OrderSubOrders
    SET RiderId = @NewRiderId,
        DeliveryStatus = 'RiderAssigned'
    WHERE SubOrderId = @SubOrderId;

    INSERT INTO dbo.OrderStatusHistory (OrderId, SubOrderId, Status, ChangedByRole, Message)
    SELECT OrderId, @SubOrderId, 'RiderReassigned', 'Admin', 'Rider reassigned: ' + @Reason
    FROM dbo.OrderSubOrders WHERE SubOrderId = @SubOrderId;

    SELECT @@ROWCOUNT;
END;
GO

IF OBJECT_ID('dbo.sp_Order_GetBasicById', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_GetBasicById;
GO
CREATE PROCEDURE dbo.sp_Order_GetBasicById
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT OrderId, ISNULL(OrderNumber, 'FR' + CAST(100000 + OrderId AS NVARCHAR(20))) AS OrderNumber, CustomerId, TotalAmount, OrderStatus, OrderedAt
    FROM dbo.Orders
    WHERE OrderId = @OrderId;
END;
GO

IF OBJECT_ID('dbo.sp_Order_UpdateMasterStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Order_UpdateMasterStatus;
GO
CREATE PROCEDURE dbo.sp_Order_UpdateMasterStatus
    @OrderId INT,
    @OrderStatus NVARCHAR(50),
    @DeliveryStatus NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Orders 
    SET OrderStatus = @OrderStatus, DeliveryStatus = @DeliveryStatus 
    WHERE OrderId = @OrderId;
    SELECT @@ROWCOUNT;
END;
GO

--------------------------------------------------------------------------------
-- 8. PAYMENT GATEWAY PROCEDURES
--------------------------------------------------------------------------------

IF OBJECT_ID('dbo.sp_Payment_GetOrderDetails', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payment_GetOrderDetails;
GO
CREATE PROCEDURE dbo.sp_Payment_GetOrderDetails
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT o.OrderId, ISNULL(o.OrderNumber, 'FR' + CAST(100000 + o.OrderId AS NVARCHAR(20))) AS OrderNumber, 
           o.TotalAmount, ISNULL(o.DeliveryFee, 40.00) AS DeliveryFee, ISNULL(o.DeliveryAddress, c.Address) AS DeliveryAddress,
           o.CustomerId, c.CustomerName, c.Email, c.PhoneNumber,
           ISNULL(o.PaymentStatus, 'Pending') AS PaymentStatus,
           (SELECT COUNT(*) FROM dbo.OrderSubOrders WHERE OrderId = o.OrderId) AS RestaurantCount
    FROM dbo.Orders o
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE o.OrderId = @OrderId;
END;
GO

IF OBJECT_ID('dbo.sp_Payment_GetOrderStatus', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payment_GetOrderStatus;
GO
CREATE PROCEDURE dbo.sp_Payment_GetOrderStatus
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TotalAmount, PaymentStatus 
    FROM dbo.Orders 
    WHERE OrderId = @OrderId;
END;
GO

IF OBJECT_ID('dbo.sp_Payment_ConfirmPayment', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payment_ConfirmPayment;
GO
CREATE PROCEDURE dbo.sp_Payment_ConfirmPayment
    @OrderId INT,
    @PaymentMethod NVARCHAR(50),
    @TxnRef NVARCHAR(100),
    @GatewayResponse NVARCHAR(500),
    @CustomerId INT,
    @Message NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1. Update Order
        UPDATE dbo.Orders 
        SET PaymentStatus = 'Paid',
            PaymentMethod = @PaymentMethod,
            OrderStatus = 'Accepted'
        WHERE OrderId = @OrderId;

        -- 2. Update Transaction
        UPDATE dbo.Transactions
        SET PaymentStatus = 'Success',
            PaymentMethod = @PaymentMethod,
            GatewayResponse = @GatewayResponse,
            TransactionReference = @TxnRef
        WHERE OrderId = @OrderId;

        -- 3. Update SubOrders
        UPDATE dbo.OrderSubOrders
        SET PreparationStatus = 'Confirmed'
        WHERE OrderId = @OrderId;

        -- 4. Insert Order Status History
        INSERT INTO dbo.OrderStatusHistory 
        (OrderId, Status, ChangedByUserId, ChangedByRole, Message, CreatedAt)
        VALUES 
        (@OrderId, 'Payment Confirmed', @CustomerId, 'Customer', @Message, GETDATE());

        COMMIT TRANSACTION;
        SELECT 1 AS IsSuccess;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0 AS IsSuccess, ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;
GO

IF OBJECT_ID('dbo.sp_Payment_GetTransactionReceipt', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_Payment_GetTransactionReceipt;
GO
CREATE PROCEDURE dbo.sp_Payment_GetTransactionReceipt
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT t.TransactionId, t.TransactionReference, t.Amount, t.PaymentMethod, t.PaymentStatus, t.GatewayResponse, t.CreatedAt,
           o.OrderNumber, o.DeliveryAddress, c.CustomerName
    FROM dbo.Transactions t
    INNER JOIN dbo.Orders o ON t.OrderId = o.OrderId
    INNER JOIN dbo.Customers c ON o.CustomerId = c.CustomerId
    WHERE t.OrderId = @OrderId;
END;
GO
