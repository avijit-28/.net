/*
================================================================================
Created By   : Avijit Pakhira
Date Created : 2026-11-17
Topic Name   : Enterprise Hotel Booking & Reservation System - Stored Procedures
Description  : Production-grade Stored Procedures across Modules 3 through 10.
               Strictly structured with:
                 - SET NOCOUNT ON and SET XACT_ABORT ON
                 - Explicit BEGIN TRANSACTION, COMMIT, and ROLLBACK in TRY...CATCH
                 - Concurrency Isolation Hints (UPDLOCK, ROWLOCK, HOLDLOCK)
                 - Dynamic Server-Side Stay Pricing & 3-Tier Refund Calculation
                 - Zero Inline SQL strings permitted.
================================================================================
*/

USE HotelBookingDb;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================================================
-- MODULE 3: USER & AUTHENTICATION STORED PROCEDURES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 1. usp_User_Register
-- Description: Registers a new user. Enforces unique email/phone validation.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_User_Register
    @FullName       NVARCHAR(100),
    @Email          NVARCHAR(256),
    @PasswordHash   NVARCHAR(256),
    @PasswordSalt   NVARCHAR(128),
    @PhoneNumber    NVARCHAR(20) = NULL,
    @Role           NVARCHAR(20) = N'Customer',
    @CreatedUserId  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Email NOT LIKE N'%_@__%.__%'
    BEGIN
        THROW 50002, N'Invalid email address format.', 1;
    END

    IF @Role NOT IN (N'Admin', N'Customer')
    BEGIN
        THROW 50002, N'Invalid user role specified. Must be Admin or Customer.', 1;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email)
        BEGIN
            THROW 50005, N'A user with this email address already exists.', 1;
        END

        IF @PhoneNumber IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Users WHERE PhoneNumber = @PhoneNumber)
        BEGIN
            THROW 50005, N'A user with this phone number already exists.', 1;
        END

        INSERT INTO dbo.Users (FullName, Email, PasswordHash, PasswordSalt, PhoneNumber, Role, IsActive, CreatedAt)
        VALUES (@FullName, @Email, @PasswordHash, @PasswordSalt, @PhoneNumber, @Role, 1, SYSUTCDATETIME());

        SET @CreatedUserId = SCOPE_IDENTITY();

        INSERT INTO dbo.AuditLogs (TableName, RecordId, Action, UserId, NewValues, Timestamp)
        VALUES (
            N'Users',
            CAST(@CreatedUserId AS NVARCHAR(50)),
            N'INSERT',
            @CreatedUserId,
            (SELECT @FullName AS FullName, @Email AS Email, @Role AS Role FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            SYSUTCDATETIME()
        );

        COMMIT TRANSACTION;

        SELECT UserId, FullName, Email, PhoneNumber, Role, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Users
        WHERE UserId = @CreatedUserId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 2. usp_User_Login
-- Description: Retrieves user security record by email for credential verification.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_User_Login
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, FullName, Email, PasswordHash, PasswordSalt, Role, IsActive
    FROM dbo.Users
    WHERE Email = @Email;
END;
GO

-- -----------------------------------------------------------------------------
-- 3. usp_User_GetById
-- Description: Retrieves user profile details by primary key.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_User_GetById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT UserId, FullName, Email, PhoneNumber, Role, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Users
    WHERE UserId = @UserId;
END;
GO

-- -----------------------------------------------------------------------------
-- 4. usp_User_UpdateProfile
-- Description: Updates user profile information (FullName, PhoneNumber).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_User_UpdateProfile
    @UserId      INT,
    @FullName    NVARCHAR(100),
    @PhoneNumber NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = @UserId)
        BEGIN
            THROW 50003, N'User not found.', 1;
        END

        IF @PhoneNumber IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Users WHERE PhoneNumber = @PhoneNumber AND UserId <> @UserId)
        BEGIN
            THROW 50005, N'Phone number is already associated with another account.', 1;
        END

        UPDATE dbo.Users
        SET FullName    = @FullName,
            PhoneNumber = @PhoneNumber,
            UpdatedAt   = SYSUTCDATETIME()
        WHERE UserId = @UserId;

        INSERT INTO dbo.AuditLogs (TableName, RecordId, Action, UserId, NewValues, Timestamp)
        VALUES (
            N'Users',
            CAST(@UserId AS NVARCHAR(50)),
            N'UPDATE',
            @UserId,
            (SELECT @FullName AS FullName, @PhoneNumber AS PhoneNumber FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            SYSUTCDATETIME()
        );

        COMMIT TRANSACTION;

        SELECT UserId, FullName, Email, PhoneNumber, Role, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Users
        WHERE UserId = @UserId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 5. usp_User_ToggleStatus
-- Description: Soft activates or deactivates a user account (Admin only).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_User_ToggleStatus
    @UserId         INT,
    @IsActive       BIT,
    @AdminUserId    INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = @UserId)
        BEGIN
            THROW 50003, N'User not found.', 1;
        END

        UPDATE dbo.Users
        SET IsActive  = @IsActive,
            UpdatedAt = SYSUTCDATETIME()
        WHERE UserId = @UserId;

        INSERT INTO dbo.AuditLogs (TableName, RecordId, Action, UserId, NewValues, Timestamp)
        VALUES (
            N'Users',
            CAST(@UserId AS NVARCHAR(50)),
            N'UPDATE',
            @AdminUserId,
            (SELECT @IsActive AS IsActive FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            SYSUTCDATETIME()
        );

        COMMIT TRANSACTION;

        SELECT UserId, FullName, Email, PhoneNumber, Role, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Users
        WHERE UserId = @UserId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- =============================================================================
-- MODULE 4: ROOM TYPE STORED PROCEDURES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 6. usp_RoomType_Create
-- Description: Creates a new room tier (Admin only).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomType_Create
    @TypeName           NVARCHAR(50),
    @Description        NVARCHAR(500) = NULL,
    @BasePrice          DECIMAL(18,2),
    @MaxOccupancy       INT,
    @CreatedRoomTypeId  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @BasePrice <= 0.00
    BEGIN
        THROW 50002, N'Base price per night must be greater than zero.', 1;
    END

    IF @MaxOccupancy < 1
    BEGIN
        THROW 50002, N'Max occupancy must be at least 1 guest.', 1;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE TypeName = @TypeName)
        BEGIN
            THROW 50005, N'A room type with this name already exists.', 1;
        END

        INSERT INTO dbo.RoomTypes (TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt)
        VALUES (@TypeName, @Description, @BasePrice, @MaxOccupancy, 1, SYSUTCDATETIME());

        SET @CreatedRoomTypeId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT RoomTypeId, TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt
        FROM dbo.RoomTypes
        WHERE RoomTypeId = @CreatedRoomTypeId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 7. usp_RoomType_GetAll
-- Description: Retrieves all active room types (supports @IncludeInactive for Admin).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomType_GetAll
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RoomTypeId, TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt, UpdatedAt
    FROM dbo.RoomTypes
    WHERE (@IncludeInactive = 1 OR IsActive = 1)
    ORDER BY BasePrice ASC;
END;
GO

-- -----------------------------------------------------------------------------
-- 8. usp_RoomType_GetById
-- Description: Retrieves single room type details by ID.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomType_GetById
    @RoomTypeId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RoomTypeId, TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt, UpdatedAt
    FROM dbo.RoomTypes
    WHERE RoomTypeId = @RoomTypeId;
END;
GO

-- -----------------------------------------------------------------------------
-- 9. usp_RoomType_Update
-- Description: Modifies pricing, occupancy, description, or name (Admin only).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomType_Update
    @RoomTypeId     INT,
    @TypeName       NVARCHAR(50),
    @Description    NVARCHAR(500) = NULL,
    @BasePrice      DECIMAL(18,2),
    @MaxOccupancy   INT,
    @IsActive       BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @BasePrice <= 0.00
    BEGIN
        THROW 50002, N'Base price must be greater than zero.', 1;
    END

    IF @MaxOccupancy < 1
    BEGIN
        THROW 50002, N'Max occupancy must be at least 1.', 1;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE RoomTypeId = @RoomTypeId)
        BEGIN
            THROW 50003, N'Room type not found.', 1;
        END

        IF EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE TypeName = @TypeName AND RoomTypeId <> @RoomTypeId)
        BEGIN
            THROW 50005, N'Another room type with this name already exists.', 1;
        END

        UPDATE dbo.RoomTypes
        SET TypeName     = @TypeName,
            Description  = @Description,
            BasePrice    = @BasePrice,
            MaxOccupancy = @MaxOccupancy,
            IsActive     = @IsActive,
            UpdatedAt    = SYSUTCDATETIME()
        WHERE RoomTypeId = @RoomTypeId;

        COMMIT TRANSACTION;

        SELECT RoomTypeId, TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt, UpdatedAt
        FROM dbo.RoomTypes
        WHERE RoomTypeId = @RoomTypeId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 10. usp_RoomType_SoftDelete
-- Description: Soft-deletes a room type if no active future bookings exist for its rooms.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomType_SoftDelete
    @RoomTypeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE RoomTypeId = @RoomTypeId)
        BEGIN
            THROW 50003, N'Room type not found.', 1;
        END

        IF EXISTS (
            SELECT 1 
            FROM dbo.Bookings b
            INNER JOIN dbo.Rooms r ON b.RoomId = r.RoomId
            WHERE r.RoomTypeId = @RoomTypeId
              AND b.BookingStatus = N'Confirmed'
              AND b.CheckOutDate >= CAST(SYSUTCDATETIME() AS DATE)
        )
        BEGIN
            THROW 50006, N'Cannot deactivate RoomType: active future bookings exist for rooms under this type.', 1;
        END

        UPDATE dbo.RoomTypes
        SET IsActive  = 0,
            UpdatedAt = SYSUTCDATETIME()
        WHERE RoomTypeId = @RoomTypeId;

        COMMIT TRANSACTION;

        SELECT RoomTypeId, TypeName, IsActive, UpdatedAt
        FROM dbo.RoomTypes
        WHERE RoomTypeId = @RoomTypeId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- =============================================================================
-- MODULE 5: ROOM INVENTORY STORED PROCEDURES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 11. usp_Room_Create
-- Description: Creates a physical room entry mapped to a RoomType (Admin only).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Room_Create
    @RoomNumber     NVARCHAR(20),
    @RoomTypeId     INT,
    @FloorNumber    INT = 1,
    @Status         NVARCHAR(30) = N'Available',
    @CreatedRoomId  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Status NOT IN (N'Available', N'Occupied', N'Under Maintenance', N'Cleaning')
    BEGIN
        THROW 50002, N'Invalid room status.', 1;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE RoomTypeId = @RoomTypeId AND IsActive = 1)
        BEGIN
            THROW 50003, N'Active RoomType not found.', 1;
        END

        IF EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = @RoomNumber)
        BEGIN
            THROW 50005, N'A room with this room number already exists.', 1;
        END

        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (@RoomNumber, @RoomTypeId, @FloorNumber, @Status, 1, SYSUTCDATETIME());

        SET @CreatedRoomId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT r.RoomId, r.RoomNumber, r.FloorNumber, r.Status, r.IsActive, r.CreatedAt,
               rt.RoomTypeId, rt.TypeName, rt.BasePrice, rt.MaxOccupancy
        FROM dbo.Rooms r
        INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
        WHERE r.RoomId = @CreatedRoomId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 12. usp_Room_GetAll
-- Description: Retrieves all physical rooms with RoomType details.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Room_GetAll
    @RoomTypeId         INT = NULL,
    @Status             NVARCHAR(30) = NULL,
    @IncludeInactive    BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.RoomId, r.RoomNumber, r.FloorNumber, r.Status, r.IsActive, r.CreatedAt, r.UpdatedAt,
           rt.RoomTypeId, rt.TypeName, rt.BasePrice, rt.MaxOccupancy
    FROM dbo.Rooms r
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE (@IncludeInactive = 1 OR r.IsActive = 1)
      AND (@RoomTypeId IS NULL OR r.RoomTypeId = @RoomTypeId)
      AND (@Status IS NULL OR r.Status = @Status)
    ORDER BY r.FloorNumber ASC, r.RoomNumber ASC;
END;
GO

-- -----------------------------------------------------------------------------
-- 13. usp_Room_GetById
-- Description: Retrieves full details of a specific room along with mapped amenities.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Room_GetById
    @RoomId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT r.RoomId, r.RoomNumber, r.FloorNumber, r.Status, r.IsActive, r.CreatedAt, r.UpdatedAt,
           rt.RoomTypeId, rt.TypeName, rt.Description AS RoomTypeDescription, rt.BasePrice, rt.MaxOccupancy
    FROM dbo.Rooms r
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE r.RoomId = @RoomId;

    SELECT a.AmenityId, a.Name, a.Description, a.IconCode, ra.AssignedAt
    FROM dbo.RoomAmenities ra
    INNER JOIN dbo.Amenities a ON ra.AmenityId = a.AmenityId
    WHERE ra.RoomId = @RoomId AND a.IsActive = 1
    ORDER BY a.Name ASC;
END;
GO

-- -----------------------------------------------------------------------------
-- 14. usp_Room_Update
-- Description: Updates room details (room number, floor, room type) (Admin only).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Room_Update
    @RoomId         INT,
    @RoomNumber     NVARCHAR(20),
    @RoomTypeId     INT,
    @FloorNumber    INT,
    @Status         NVARCHAR(30),
    @IsActive       BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Status NOT IN (N'Available', N'Occupied', N'Under Maintenance', N'Cleaning')
    BEGIN
        THROW 50002, N'Invalid room status.', 1;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomId = @RoomId)
        BEGIN
            THROW 50003, N'Room not found.', 1;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE RoomTypeId = @RoomTypeId AND IsActive = 1)
        BEGIN
            THROW 50003, N'Specified RoomType does not exist or is inactive.', 1;
        END

        IF EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = @RoomNumber AND RoomId <> @RoomId)
        BEGIN
            THROW 50005, N'Another room with this room number already exists.', 1;
        END

        UPDATE dbo.Rooms
        SET RoomNumber  = @RoomNumber,
            RoomTypeId  = @RoomTypeId,
            FloorNumber = @FloorNumber,
            Status      = @Status,
            IsActive    = @IsActive,
            UpdatedAt   = SYSUTCDATETIME()
        WHERE RoomId = @RoomId;

        COMMIT TRANSACTION;

        SELECT r.RoomId, r.RoomNumber, r.FloorNumber, r.Status, r.IsActive, r.UpdatedAt,
               rt.RoomTypeId, rt.TypeName, rt.BasePrice, rt.MaxOccupancy
        FROM dbo.Rooms r
        INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
        WHERE r.RoomId = @RoomId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 15. usp_Room_UpdateStatus
-- Description: Transitions room status (e.g., Available <-> Under Maintenance).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Room_UpdateStatus
    @RoomId INT,
    @Status NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @Status NOT IN (N'Available', N'Occupied', N'Under Maintenance', N'Cleaning')
    BEGIN
        THROW 50002, N'Invalid status transition.', 1;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomId = @RoomId)
        BEGIN
            THROW 50003, N'Room not found.', 1;
        END

        UPDATE dbo.Rooms
        SET Status    = @Status,
            UpdatedAt = SYSUTCDATETIME()
        WHERE RoomId = @RoomId;

        COMMIT TRANSACTION;

        SELECT RoomId, RoomNumber, Status, UpdatedAt
        FROM dbo.Rooms
        WHERE RoomId = @RoomId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 16. usp_Room_SoftDelete
-- Description: Soft deletes a room only if no active future bookings exist.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Room_SoftDelete
    @RoomId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomId = @RoomId)
        BEGIN
            THROW 50003, N'Room not found.', 1;
        END

        IF EXISTS (
            SELECT 1 
            FROM dbo.Bookings
            WHERE RoomId = @RoomId
              AND BookingStatus = N'Confirmed'
              AND CheckOutDate >= CAST(SYSUTCDATETIME() AS DATE)
        )
        BEGIN
            THROW 50006, N'Cannot deactivate room with active future confirmed bookings.', 1;
        END

        UPDATE dbo.Rooms
        SET IsActive  = 0,
            UpdatedAt = SYSUTCDATETIME()
        WHERE RoomId = @RoomId;

        COMMIT TRANSACTION;

        SELECT RoomId, RoomNumber, IsActive, UpdatedAt
        FROM dbo.Rooms
        WHERE RoomId = @RoomId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- =============================================================================
-- MODULE 6: AMENITIES STORED PROCEDURES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 17. usp_Amenity_Create
-- Description: Adds a new amenity; prevents duplicate names (case-insensitive).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Amenity_Create
    @Name               NVARCHAR(100),
    @Description        NVARCHAR(255) = NULL,
    @IconCode           NVARCHAR(50) = NULL,
    @CreatedAmenityId   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM dbo.Amenities WHERE LOWER(LTRIM(RTRIM(Name))) = LOWER(LTRIM(RTRIM(@Name))))
        BEGIN
            THROW 50005, N'An amenity with this name already exists.', 1;
        END

        INSERT INTO dbo.Amenities (Name, Description, IconCode, IsActive, CreatedAt)
        VALUES (@Name, @Description, @IconCode, 1, SYSUTCDATETIME());

        SET @CreatedAmenityId = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT AmenityId, Name, Description, IconCode, IsActive, CreatedAt
        FROM dbo.Amenities
        WHERE AmenityId = @CreatedAmenityId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 18. usp_Amenity_GetAll
-- Description: Retrieves all active amenities.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Amenity_GetAll
    @IncludeInactive BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AmenityId, Name, Description, IconCode, IsActive, CreatedAt
    FROM dbo.Amenities
    WHERE (@IncludeInactive = 1 OR IsActive = 1)
    ORDER BY Name ASC;
END;
GO

-- -----------------------------------------------------------------------------
-- 19. usp_Amenity_GetById
-- Description: Retrieves single amenity by ID.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Amenity_GetById
    @AmenityId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT AmenityId, Name, Description, IconCode, IsActive, CreatedAt
    FROM dbo.Amenities
    WHERE AmenityId = @AmenityId;
END;
GO

-- -----------------------------------------------------------------------------
-- 20. usp_Amenity_Update
-- Description: Updates an amenity's details (Admin only).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Amenity_Update
    @AmenityId      INT,
    @Name           NVARCHAR(100),
    @Description    NVARCHAR(255) = NULL,
    @IconCode       NVARCHAR(50) = NULL,
    @IsActive       BIT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE AmenityId = @AmenityId)
        BEGIN
            THROW 50003, N'Amenity not found.', 1;
        END

        IF EXISTS (SELECT 1 FROM dbo.Amenities WHERE LOWER(LTRIM(RTRIM(Name))) = LOWER(LTRIM(RTRIM(@Name))) AND AmenityId <> @AmenityId)
        BEGIN
            THROW 50005, N'Another amenity with this name already exists.', 1;
        END

        UPDATE dbo.Amenities
        SET Name        = @Name,
            Description = @Description,
            IconCode    = @IconCode,
            IsActive    = @IsActive
        WHERE AmenityId = @AmenityId;

        COMMIT TRANSACTION;

        SELECT AmenityId, Name, Description, IconCode, IsActive, CreatedAt
        FROM dbo.Amenities
        WHERE AmenityId = @AmenityId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 21. usp_Amenity_SoftDelete
-- Description: Soft deletes an amenity.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Amenity_SoftDelete
    @AmenityId INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE AmenityId = @AmenityId)
        BEGIN
            THROW 50003, N'Amenity not found.', 1;
        END

        UPDATE dbo.Amenities
        SET IsActive = 0
        WHERE AmenityId = @AmenityId;

        COMMIT TRANSACTION;

        SELECT AmenityId, Name, IsActive
        FROM dbo.Amenities
        WHERE AmenityId = @AmenityId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- =============================================================================
-- MODULE 7: ROOM AMENITIES STORED PROCEDURES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 22. usp_RoomAmenities_Assign
-- Description: Assigns a list of comma-separated AmenityIds to a room idempotently.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomAmenities_Assign
    @RoomId     INT,
    @AmenityIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomId = @RoomId AND IsActive = 1)
        BEGIN
            THROW 50003, N'Active Room not found.', 1;
        END

        DECLARE @AmenitiesToAssign TABLE (AmenityId INT PRIMARY KEY);
        INSERT INTO @AmenitiesToAssign (AmenityId)
        SELECT DISTINCT CAST(value AS INT)
        FROM STRING_SPLIT(@AmenityIds, N',')
        WHERE RTRIM(LTRIM(value)) <> N'';

        IF EXISTS (
            SELECT 1 
            FROM @AmenitiesToAssign a
            WHERE NOT EXISTS (SELECT 1 FROM dbo.Amenities m WHERE m.AmenityId = a.AmenityId AND m.IsActive = 1)
        )
        BEGIN
            THROW 50003, N'One or more specified Amenity IDs do not exist or are inactive.', 1;
        END

        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt)
        SELECT @RoomId, a.AmenityId, SYSUTCDATETIME()
        FROM @AmenitiesToAssign a
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.RoomAmenities ra 
            WHERE ra.RoomId = @RoomId AND ra.AmenityId = a.AmenityId
        );

        COMMIT TRANSACTION;

        SELECT a.AmenityId, a.Name, a.Description, a.IconCode, ra.AssignedAt
        FROM dbo.RoomAmenities ra
        INNER JOIN dbo.Amenities a ON ra.AmenityId = a.AmenityId
        WHERE ra.RoomId = @RoomId
        ORDER BY a.Name ASC;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 23. usp_RoomAmenities_GetByRoomId
-- Description: Retrieves all active amenities mapped to a specific room.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomAmenities_GetByRoomId
    @RoomId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.AmenityId, a.Name, a.Description, a.IconCode, ra.AssignedAt
    FROM dbo.RoomAmenities ra
    INNER JOIN dbo.Amenities a ON ra.AmenityId = a.AmenityId
    WHERE ra.RoomId = @RoomId AND a.IsActive = 1
    ORDER BY a.Name ASC;
END;
GO

-- -----------------------------------------------------------------------------
-- 24. usp_RoomAmenities_Remove
-- Description: Unmaps a specific amenity from a room.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_RoomAmenities_Remove
    @RoomId     INT,
    @AmenityId  INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @RoomId AND AmenityId = @AmenityId)
        BEGIN
            THROW 50003, N'Amenity mapping for this room does not exist.', 1;
        END

        DELETE FROM dbo.RoomAmenities
        WHERE RoomId = @RoomId AND AmenityId = @AmenityId;

        COMMIT TRANSACTION;

        SELECT @RoomId AS RoomId, @AmenityId AS RemovedAmenityId, N'Successfully unmapped' AS Status;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- =============================================================================
-- MODULE 8: HOTEL SEARCH & AVAILABILITY STORED PROCEDURE
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 25. usp_Room_SearchAvailable
-- Description: Multi-criteria room search with half-open date interval overlap
--              detection, relational division for amenities, and dynamic pricing.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Room_SearchAvailable
    @CheckInDate                DATE,
    @CheckOutDate               DATE,
    @Guests                     INT = NULL,
    @RoomTypeId                 INT = NULL,
    @MinPrice                   DECIMAL(18,2) = NULL,
    @MaxPrice                   DECIMAL(18,2) = NULL,
    @AmenityIds                 NVARCHAR(MAX) = NULL,
    @PageNumber                 INT = 1,
    @PageSize                   INT = 10,
    @TotalRecords               INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @CheckInDate < CAST(SYSUTCDATETIME() AS DATE)
    BEGIN
        THROW 50002, N'Check-in date cannot be in the past.', 1;
    END

    IF @CheckOutDate <= @CheckInDate
    BEGIN
        THROW 50002, N'Check-out date must be strictly after check-in date.', 1;
    END

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 OR @PageSize > 50 SET @PageSize = 10;

    DECLARE @StayNights INT = DATEDIFF(DAY, @CheckInDate, @CheckOutDate);

    DECLARE @ReqAmenities TABLE (AmenityId INT PRIMARY KEY);
    IF @AmenityIds IS NOT NULL AND LEN(RTRIM(LTRIM(@AmenityIds))) > 0
    BEGIN
        INSERT INTO @ReqAmenities (AmenityId)
        SELECT DISTINCT CAST(value AS INT)
        FROM STRING_SPLIT(@AmenityIds, N',')
        WHERE RTRIM(LTRIM(value)) <> N'';
    END;

    DECLARE @RequiredAmenityCount INT = (SELECT COUNT(*) FROM @ReqAmenities);

    SELECT @TotalRecords = COUNT(DISTINCT r.RoomId)
    FROM dbo.Rooms r
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE r.IsActive = 1
      AND rt.IsActive = 1
      AND r.Status <> N'Under Maintenance'
      AND (@RoomTypeId IS NULL OR r.RoomTypeId = @RoomTypeId)
      AND (@Guests IS NULL OR rt.MaxOccupancy >= @Guests)
      AND (@MinPrice IS NULL OR rt.BasePrice >= @MinPrice)
      AND (@MaxPrice IS NULL OR rt.BasePrice <= @MaxPrice)
      AND NOT EXISTS (
          SELECT 1 FROM dbo.Bookings b
          WHERE b.RoomId = r.RoomId
            AND b.BookingStatus IN (N'Confirmed', N'CheckedIn')
            AND b.CheckInDate < @CheckOutDate
            AND b.CheckOutDate > @CheckInDate
      )
      AND (
          @RequiredAmenityCount = 0
          OR r.RoomId IN (
              SELECT ra.RoomId
              FROM dbo.RoomAmenities ra
              INNER JOIN @ReqAmenities req ON ra.AmenityId = req.AmenityId
              GROUP BY ra.RoomId
              HAVING COUNT(DISTINCT ra.AmenityId) = @RequiredAmenityCount
          )
      );

    SELECT r.RoomId,
           r.RoomNumber,
           r.FloorNumber,
           r.Status,
           rt.RoomTypeId,
           rt.TypeName AS RoomTypeName,
           rt.Description AS RoomTypeDescription,
           rt.BasePrice AS BasePricePerNight,
           (@StayNights * rt.BasePrice) AS TotalStayPrice,
           @StayNights AS StayNights,
           rt.MaxOccupancy
    FROM dbo.Rooms r
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE r.IsActive = 1
      AND rt.IsActive = 1
      AND r.Status <> N'Under Maintenance'
      AND (@RoomTypeId IS NULL OR r.RoomTypeId = @RoomTypeId)
      AND (@Guests IS NULL OR rt.MaxOccupancy >= @Guests)
      AND (@MinPrice IS NULL OR rt.BasePrice >= @MinPrice)
      AND (@MaxPrice IS NULL OR rt.BasePrice <= @MaxPrice)
      AND NOT EXISTS (
          SELECT 1 FROM dbo.Bookings b
          WHERE b.RoomId = r.RoomId
            AND b.BookingStatus IN (N'Confirmed', N'CheckedIn')
            AND b.CheckInDate < @CheckOutDate
            AND b.CheckOutDate > @CheckInDate
      )
      AND (
          @RequiredAmenityCount = 0
          OR r.RoomId IN (
              SELECT ra.RoomId
              FROM dbo.RoomAmenities ra
              INNER JOIN @ReqAmenities req ON ra.AmenityId = req.AmenityId
              GROUP BY ra.RoomId
              HAVING COUNT(DISTINCT ra.AmenityId) = @RequiredAmenityCount
          )
      )
    ORDER BY rt.BasePrice ASC, r.RoomNumber ASC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- =============================================================================
-- MODULE 9: HOTEL BOOKING STORED PROCEDURES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 26. usp_Booking_Create
-- Description: Atomic reservation with UPDLOCK, HOLDLOCK range locking to eliminate
--              double-booking race conditions, dynamic server-side pricing,
--              and unique sequence reference generation.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Booking_Create
    @UserId                     INT,
    @RoomId                     INT,
    @CheckInDate                DATE,
    @CheckOutDate               DATE,
    @NumberOfGuests             INT = 1,
    @CustomerNotes              NVARCHAR(500) = NULL,
    @CreatedBookingId           INT OUTPUT,
    @GeneratedBookingReference  NVARCHAR(30) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @CheckInDate < CAST(SYSUTCDATETIME() AS DATE)
    BEGIN
        THROW 50002, N'Check-in date cannot be in the past.', 1;
    END

    IF @CheckOutDate <= @CheckInDate
    BEGIN
        THROW 50002, N'Check-out date must be strictly after check-in date.', 1;
    END

    IF @NumberOfGuests < 1
    BEGIN
        THROW 50002, N'NumberOfGuests must be at least 1.', 1;
    END

    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserId = @UserId AND IsActive = 1)
    BEGIN
        THROW 50003, N'User does not exist or account is inactive.', 1;
    END

    DECLARE @TotalNights INT = DATEDIFF(DAY, @CheckInDate, @CheckOutDate);
    DECLARE @BasePrice DECIMAL(18,2);
    DECLARE @MaxOccupancy INT;
    DECLARE @CalculatedTotalAmount DECIMAL(18,2);
    DECLARE @SeqVal INT;
    DECLARE @BookingRef NVARCHAR(30);

    BEGIN TRANSACTION;
    BEGIN TRY
        SELECT @BasePrice = rt.BasePrice,
               @MaxOccupancy = rt.MaxOccupancy
        FROM dbo.Rooms r WITH (UPDLOCK, ROWLOCK)
        INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
        WHERE r.RoomId = @RoomId 
          AND r.IsActive = 1
          AND rt.IsActive = 1
          AND r.Status <> N'Under Maintenance';

        IF @BasePrice IS NULL
        BEGIN
            THROW 50001, N'Selected room is not active, does not exist, or is under maintenance.', 1;
        END

        IF @NumberOfGuests > @MaxOccupancy
        BEGIN
            THROW 50002, N'NumberOfGuests exceeds maximum occupancy for this room category.', 1;
        END

        IF EXISTS (
            SELECT 1 
            FROM dbo.Bookings WITH (UPDLOCK, HOLDLOCK)
            WHERE RoomId = @RoomId
              AND BookingStatus IN (N'Confirmed', N'CheckedIn')
              AND CheckInDate < @CheckOutDate
              AND CheckOutDate > @CheckInDate
        )
        BEGIN
            THROW 50001, N'Double-booking conflict: Room is already booked for the requested dates.', 1;
        END

        SET @CalculatedTotalAmount = @BasePrice * @TotalNights;

        SET @SeqVal = NEXT VALUE FOR dbo.BookingReferenceSeq;
        SET @BookingRef = CONCAT(
            N'BK-',
            FORMAT(SYSUTCDATETIME(), N'yyyyMMdd'),
            N'-',
            RIGHT(N'0000' + CAST(@SeqVal AS NVARCHAR(4)), 4)
        );

        INSERT INTO dbo.Bookings
        (
            BookingReference,
            UserId,
            RoomId,
            CheckInDate,
            CheckOutDate,
            NumberOfGuests,
            TotalAmount,
            BookingStatus,
            CustomerNotes,
            CreatedAt
        )
        VALUES
        (
            @BookingRef,
            @UserId,
            @RoomId,
            @CheckInDate,
            @CheckOutDate,
            @NumberOfGuests,
            @CalculatedTotalAmount,
            N'Confirmed',
            @CustomerNotes,
            SYSUTCDATETIME()
        );

        SET @CreatedBookingId = SCOPE_IDENTITY();
        SET @GeneratedBookingReference = @BookingRef;

        INSERT INTO dbo.AuditLogs (TableName, RecordId, Action, UserId, NewValues, Timestamp)
        VALUES (
            N'Bookings',
            CAST(@CreatedBookingId AS NVARCHAR(50)),
            N'INSERT',
            @UserId,
            (SELECT @BookingRef AS BookingRef, @RoomId AS RoomId, @CheckInDate AS CheckIn, 
                    @CheckOutDate AS CheckOut, @CalculatedTotalAmount AS TotalAmount FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            SYSUTCDATETIME()
        );

        COMMIT TRANSACTION;

        SELECT b.BookingId, b.BookingReference, b.UserId, b.RoomId, r.RoomNumber,
               rt.TypeName AS RoomTypeName, rt.BasePrice AS NightlyRate,
               b.CheckInDate, b.CheckOutDate, @TotalNights AS TotalNights,
               b.NumberOfGuests, b.TotalAmount, b.BookingStatus, b.CustomerNotes, b.CreatedAt
        FROM dbo.Bookings b
        INNER JOIN dbo.Rooms r ON b.RoomId = r.RoomId
        INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
        WHERE b.BookingId = @CreatedBookingId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 27. usp_Booking_GetById
-- Description: Retrieves full booking details by BookingId.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Booking_GetById
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.BookingId, b.BookingReference, b.UserId, u.FullName AS CustomerName, u.Email AS CustomerEmail,
           b.RoomId, r.RoomNumber, rt.TypeName AS RoomTypeName, rt.BasePrice AS NightlyRate,
           b.CheckInDate, b.CheckOutDate, DATEDIFF(DAY, b.CheckInDate, b.CheckOutDate) AS TotalNights,
           b.NumberOfGuests, b.TotalAmount, b.BookingStatus, b.CustomerNotes, b.CreatedAt, b.UpdatedAt
    FROM dbo.Bookings b
    INNER JOIN dbo.Users u ON b.UserId = u.UserId
    INNER JOIN dbo.Rooms r ON b.RoomId = r.RoomId
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE b.BookingId = @BookingId;
END;
GO

-- -----------------------------------------------------------------------------
-- 28. usp_Booking_GetByReference
-- Description: Retrieves full booking details by BookingReference (BK-YYYYMMDD-XXXX).
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Booking_GetByReference
    @BookingReference NVARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.BookingId, b.BookingReference, b.UserId, u.FullName AS CustomerName, u.Email AS CustomerEmail,
           b.RoomId, r.RoomNumber, rt.TypeName AS RoomTypeName, rt.BasePrice AS NightlyRate,
           b.CheckInDate, b.CheckOutDate, DATEDIFF(DAY, b.CheckInDate, b.CheckOutDate) AS TotalNights,
           b.NumberOfGuests, b.TotalAmount, b.BookingStatus, b.CustomerNotes, b.CreatedAt, b.UpdatedAt
    FROM dbo.Bookings b
    INNER JOIN dbo.Users u ON b.UserId = u.UserId
    INNER JOIN dbo.Rooms r ON b.RoomId = r.RoomId
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE b.BookingReference = @BookingReference;
END;
GO

-- -----------------------------------------------------------------------------
-- 29. usp_Booking_GetByUser
-- Description: Retrieves paginated bookings for a specific customer.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Booking_GetByUser
    @UserId     INT,
    @PageNumber INT = 1,
    @PageSize   INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 OR @PageSize > 50 SET @PageSize = 10;

    SELECT b.BookingId, b.BookingReference, b.RoomId, r.RoomNumber, rt.TypeName AS RoomTypeName,
           b.CheckInDate, b.CheckOutDate, b.NumberOfGuests, b.TotalAmount, b.BookingStatus, b.CreatedAt
    FROM dbo.Bookings b
    INNER JOIN dbo.Rooms r ON b.RoomId = r.RoomId
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE b.UserId = @UserId
    ORDER BY b.CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- -----------------------------------------------------------------------------
-- 30. usp_Booking_GetAll
-- Description: Retrieves all system bookings for Admin with date and status filters.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Booking_GetAll
    @Status         NVARCHAR(20) = NULL,
    @StartDate      DATE = NULL,
    @EndDate        DATE = NULL,
    @PageNumber     INT = 1,
    @PageSize       INT = 20,
    @TotalRecords   INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 OR @PageSize > 100 SET @PageSize = 20;

    SELECT @TotalRecords = COUNT(*)
    FROM dbo.Bookings b
    WHERE (@Status IS NULL OR b.BookingStatus = @Status)
      AND (@StartDate IS NULL OR b.CheckInDate >= @StartDate)
      AND (@EndDate IS NULL OR b.CheckOutDate <= @EndDate);

    SELECT b.BookingId, b.BookingReference, b.UserId, u.FullName AS CustomerName, u.Email AS CustomerEmail,
           b.RoomId, r.RoomNumber, rt.TypeName AS RoomTypeName,
           b.CheckInDate, b.CheckOutDate, b.NumberOfGuests, b.TotalAmount, b.BookingStatus, b.CreatedAt
    FROM dbo.Bookings b
    INNER JOIN dbo.Users u ON b.UserId = u.UserId
    INNER JOIN dbo.Rooms r ON b.RoomId = r.RoomId
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE (@Status IS NULL OR b.BookingStatus = @Status)
      AND (@StartDate IS NULL OR b.CheckInDate >= @StartDate)
      AND (@EndDate IS NULL OR b.CheckOutDate <= @EndDate)
    ORDER BY b.CreatedAt DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- =============================================================================
-- MODULE 10: HOTEL CANCELLATION STORED PROCEDURES
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 31. usp_Booking_Cancel
-- Description: Cancels a booking, calculates 3-tier refund policy inside an atomic
--              transaction, and logs the cancellation record and audit log.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Booking_Cancel
    @BookingId            INT,
    @CancelledByUserId    INT,
    @CancellationReason   NVARCHAR(500) = NULL,
    @CancellationId       INT OUTPUT,
    @RefundPercentage     DECIMAL(5,2) OUTPUT,
    @RefundAmount         DECIMAL(18,2) OUTPUT,
    @CancellationCharge   DECIMAL(18,2) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @CheckInDate DATE;
    DECLARE @TotalAmount DECIMAL(18,2);
    DECLARE @BookingStatus NVARCHAR(20);
    DECLARE @CheckInDateTime DATETIME2(7);
    DECLARE @HoursUntilCheckIn DECIMAL(10,2);

    SELECT 
        @CheckInDate = CheckInDate,
        @TotalAmount = TotalAmount,
        @BookingStatus = BookingStatus
    FROM dbo.Bookings
    WHERE BookingId = @BookingId;

    IF @BookingStatus IS NULL
    BEGIN
        THROW 50003, N'Booking does not exist.', 1;
    END

    IF @BookingStatus = N'Cancelled'
    BEGIN
        THROW 50004, N'Booking has already been cancelled.', 1;
    END

    IF @BookingStatus = N'Completed'
    BEGIN
        THROW 50008, N'Cannot cancel a completed booking.', 1;
    END

    IF CAST(SYSUTCDATETIME() AS DATE) >= @CheckInDate
    BEGIN
        THROW 50008, N'Cannot cancel a booking on or after the scheduled check-in date.', 1;
    END

    SET @CheckInDateTime = DATEADD(HOUR, 14, CAST(@CheckInDate AS DATETIME2(7)));
    SET @HoursUntilCheckIn = DATEDIFF(MINUTE, SYSUTCDATETIME(), @CheckInDateTime) / 60.0;

    IF @HoursUntilCheckIn >= 48.0
    BEGIN
        SET @RefundPercentage = 100.00;
        SET @RefundAmount = @TotalAmount;
        SET @CancellationCharge = 0.00;
    END
    ELSE IF @HoursUntilCheckIn >= 24.0 AND @HoursUntilCheckIn < 48.0
    BEGIN
        SET @RefundPercentage = 50.00;
        SET @RefundAmount = ROUND((@TotalAmount * 0.50), 2);
        SET @CancellationCharge = @TotalAmount - @RefundAmount;
    END
    ELSE
    BEGIN
        SET @RefundPercentage = 0.00;
        SET @RefundAmount = 0.00;
        SET @CancellationCharge = @TotalAmount;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.Bookings
        SET BookingStatus = N'Cancelled',
            UpdatedAt     = SYSUTCDATETIME()
        WHERE BookingId = @BookingId;

        INSERT INTO dbo.Cancellations
        (
            BookingId,
            CancelledByUserId,
            CancellationReason,
            RefundPercentage,
            RefundAmount,
            CancellationCharge,
            CancelledAt
        )
        VALUES
        (
            @BookingId,
            @CancelledByUserId,
            @CancellationReason,
            @RefundPercentage,
            @RefundAmount,
            @CancellationCharge,
            SYSUTCDATETIME()
        );

        SET @CancellationId = SCOPE_IDENTITY();

        INSERT INTO dbo.AuditLogs (TableName, RecordId, Action, UserId, OldValues, NewValues, Timestamp)
        VALUES (
            N'Bookings',
            CAST(@BookingId AS NVARCHAR(50)),
            N'CANCEL',
            @CancelledByUserId,
            (SELECT N'Confirmed' AS Status, @TotalAmount AS Amount FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            (SELECT N'Cancelled' AS Status, @RefundPercentage AS RefundPct, @RefundAmount AS RefundAmt, @CancellationCharge AS Charge FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),
            SYSUTCDATETIME()
        );

        COMMIT TRANSACTION;

        SELECT c.CancellationId, c.BookingId, b.BookingReference, c.CancelledAt,
               c.CancellationReason, c.RefundPercentage, c.RefundAmount, c.CancellationCharge,
               c.CancelledByUserId, b.TotalAmount AS OriginalBookingAmount,
               CAST(ROUND(@HoursUntilCheckIn, 0) AS INT) AS HoursBeforeCheckIn
        FROM dbo.Cancellations c
        INNER JOIN dbo.Bookings b ON c.BookingId = b.BookingId
        WHERE c.CancellationId = @CancellationId;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

-- -----------------------------------------------------------------------------
-- 32. usp_Cancellation_GetDetails
-- Description: Retrieves cancellation and refund details for a booking.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Cancellation_GetDetails
    @BookingId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT c.CancellationId, c.BookingId, b.BookingReference, c.CancelledAt,
           c.CancellationReason, c.RefundPercentage, c.RefundAmount, c.CancellationCharge,
           c.CancelledByUserId, u.FullName AS CancelledByUserName, b.TotalAmount AS OriginalBookingAmount
    FROM dbo.Cancellations c
    INNER JOIN dbo.Bookings b ON c.BookingId = b.BookingId
    INNER JOIN dbo.Users u ON c.CancelledByUserId = u.UserId
    WHERE c.BookingId = @BookingId;
END;
GO

-- =============================================================================
-- CROSS-CUTTING / AUDIT TRAIL STORED PROCEDURE
-- =============================================================================

-- -----------------------------------------------------------------------------
-- 33. usp_AuditLog_Create
-- Description: Inserts an audit log record into the immutable AuditLogs table.
-- -----------------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_AuditLog_Create
    @TableName  NVARCHAR(100),
    @RecordId   NVARCHAR(50),
    @Action     NVARCHAR(20),
    @UserId     INT = NULL,
    @OldValues  NVARCHAR(MAX) = NULL,
    @NewValues  NVARCHAR(MAX) = NULL,
    @IpAddress  NVARCHAR(45) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.AuditLogs
    (
        TableName,
        RecordId,
        Action,
        UserId,
        OldValues,
        NewValues,
        Timestamp,
        IpAddress
    )
    VALUES
    (
        @TableName,
        @RecordId,
        @Action,
        @UserId,
        @OldValues,
        @NewValues,
        SYSUTCDATETIME(),
        @IpAddress
    );
END;
GO
