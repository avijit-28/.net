/*
================================================================================
Created By   : Avijit Pakhira
Date Created : 2026-11-17
Topic Name   : Enterprise Hotel Booking & Reservation System - Database Schema (DDL)
Description  : 3NF Normalized Relational Database Schema definition for MS SQL Server.
               Enforces Primary Keys, Foreign Keys, Unique Constraints, Check
               Constraints, Default Constraints, and Atomic Sequences.
================================================================================
*/

-- Create Database if not exists
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'HotelBookingDb')
BEGIN
    CREATE DATABASE HotelBookingDb;
END
GO

USE HotelBookingDb;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- -----------------------------------------------------------------------------
-- Table 1: Users
-- Description: Stores customer and administrator profile & security credentials.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        UserId          INT IDENTITY(1,1) NOT NULL,
        FullName        NVARCHAR(100)     NOT NULL,
        Email           NVARCHAR(256)     NOT NULL,
        PasswordHash    NVARCHAR(256)     NOT NULL,
        PasswordSalt    NVARCHAR(128)     NOT NULL,
        PhoneNumber     NVARCHAR(20)      NULL,
        Role            NVARCHAR(20)      NOT NULL CONSTRAINT DF_Users_Role DEFAULT (N'Customer'),
        IsActive        BIT               NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
        CreatedAt       DATETIME2(7)      NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       DATETIME2(7)      NULL,

        CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId ASC),
        CONSTRAINT UQ_Users_Email UNIQUE NONCLUSTERED (Email ASC),
        CONSTRAINT CK_Users_Role CHECK (Role IN (N'Admin', N'Customer')),
        CONSTRAINT CK_Users_Email CHECK (Email LIKE N'%_@__%.__%')
    );
END
GO

-- -----------------------------------------------------------------------------
-- Table 2: RoomTypes
-- Description: Master catalog of room tiers, base pricing, and maximum guest capacity.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.RoomTypes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoomTypes
    (
        RoomTypeId      INT IDENTITY(1,1) NOT NULL,
        TypeName        NVARCHAR(50)      NOT NULL,
        Description     NVARCHAR(500)     NULL,
        BasePrice       DECIMAL(18,2)     NOT NULL,
        MaxOccupancy    INT               NOT NULL,
        IsActive        BIT               NOT NULL CONSTRAINT DF_RoomTypes_IsActive DEFAULT (1),
        CreatedAt       DATETIME2(7)      NOT NULL CONSTRAINT DF_RoomTypes_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       DATETIME2(7)      NULL,

        CONSTRAINT PK_RoomTypes PRIMARY KEY CLUSTERED (RoomTypeId ASC),
        CONSTRAINT UQ_RoomTypes_TypeName UNIQUE NONCLUSTERED (TypeName ASC),
        CONSTRAINT CK_RoomTypes_BasePrice CHECK (BasePrice > 0.00),
        CONSTRAINT CK_RoomTypes_MaxOccupancy CHECK (MaxOccupancy >= 1)
    );
END
GO

-- -----------------------------------------------------------------------------
-- Table 3: Rooms
-- Description: Physical inventory of hotel rooms mapped to Room Types.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Rooms', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Rooms
    (
        RoomId          INT IDENTITY(1,1) NOT NULL,
        RoomNumber      NVARCHAR(20)      NOT NULL,
        RoomTypeId      INT               NOT NULL,
        FloorNumber     INT               NOT NULL CONSTRAINT DF_Rooms_FloorNumber DEFAULT (1),
        Status          NVARCHAR(30)      NOT NULL CONSTRAINT DF_Rooms_Status DEFAULT (N'Available'),
        IsActive        BIT               NOT NULL CONSTRAINT DF_Rooms_IsActive DEFAULT (1),
        CreatedAt       DATETIME2(7)      NOT NULL CONSTRAINT DF_Rooms_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       DATETIME2(7)      NULL,

        CONSTRAINT PK_Rooms PRIMARY KEY CLUSTERED (RoomId ASC),
        CONSTRAINT UQ_Rooms_RoomNumber UNIQUE NONCLUSTERED (RoomNumber ASC),
        CONSTRAINT FK_Rooms_RoomTypes FOREIGN KEY (RoomTypeId) 
            REFERENCES dbo.RoomTypes (RoomTypeId),
        CONSTRAINT CK_Rooms_Status CHECK (Status IN (N'Available', N'Occupied', N'Under Maintenance', N'Cleaning')),
        CONSTRAINT CK_Rooms_FloorNumber CHECK (FloorNumber >= 0)
    );
END
GO

-- -----------------------------------------------------------------------------
-- Table 4: Amenities
-- Description: Master catalog of room and hotel features.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Amenities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Amenities
    (
        AmenityId       INT IDENTITY(1,1) NOT NULL,
        Name            NVARCHAR(100)     NOT NULL,
        Description     NVARCHAR(255)     NULL,
        IconCode        NVARCHAR(50)      NULL,
        IsActive        BIT               NOT NULL CONSTRAINT DF_Amenities_IsActive DEFAULT (1),
        CreatedAt       DATETIME2(7)      NOT NULL CONSTRAINT DF_Amenities_CreatedAt DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Amenities PRIMARY KEY CLUSTERED (AmenityId ASC),
        CONSTRAINT UQ_Amenities_Name UNIQUE NONCLUSTERED (Name ASC)
    );
END
GO

-- -----------------------------------------------------------------------------
-- Table 5: RoomAmenities
-- Description: Many-to-Many junction table associating physical rooms with amenities.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.RoomAmenities', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoomAmenities
    (
        RoomAmenityId   INT IDENTITY(1,1) NOT NULL,
        RoomId          INT               NOT NULL,
        AmenityId       INT               NOT NULL,
        AssignedAt      DATETIME2(7)      NOT NULL CONSTRAINT DF_RoomAmenities_AssignedAt DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_RoomAmenities PRIMARY KEY CLUSTERED (RoomAmenityId ASC),
        CONSTRAINT UQ_RoomAmenities_Room_Amenity UNIQUE NONCLUSTERED (RoomId ASC, AmenityId ASC),
        CONSTRAINT FK_RoomAmenities_Rooms FOREIGN KEY (RoomId)
            REFERENCES dbo.Rooms (RoomId) ON DELETE CASCADE,
        CONSTRAINT FK_RoomAmenities_Amenities FOREIGN KEY (AmenityId)
            REFERENCES dbo.Amenities (AmenityId) ON DELETE CASCADE
    );
END
GO

-- -----------------------------------------------------------------------------
-- Sequence: BookingReferenceSeq
-- Description: Provides cycling thread-safe integers for unique human-readable
--              booking references (e.g., BK-YYYYMMDD-1001).
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.sequences WHERE name = N'BookingReferenceSeq')
BEGIN
    CREATE SEQUENCE dbo.BookingReferenceSeq
        AS INT
        START WITH 1001
        INCREMENT BY 1
        MINVALUE 1001
        MAXVALUE 9999
        CYCLE;
END
GO

-- -----------------------------------------------------------------------------
-- Table 6: Bookings
-- Description: Core reservation records with server-enforced total stay price.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Bookings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings
    (
        BookingId           INT IDENTITY(1,1) NOT NULL,
        BookingReference    NVARCHAR(30)      NOT NULL,
        UserId              INT               NOT NULL,
        RoomId              INT               NOT NULL,
        CheckInDate         DATE              NOT NULL,
        CheckOutDate        DATE              NOT NULL,
        NumberOfGuests      INT               NOT NULL CONSTRAINT DF_Bookings_Guests DEFAULT (1),
        TotalAmount         DECIMAL(18,2)     NOT NULL,
        BookingStatus       NVARCHAR(20)      NOT NULL CONSTRAINT DF_Bookings_BookingStatus DEFAULT (N'Confirmed'),
        CustomerNotes       NVARCHAR(500)     NULL,
        CreatedAt           DATETIME2(7)      NOT NULL CONSTRAINT DF_Bookings_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt           DATETIME2(7)      NULL,

        CONSTRAINT PK_Bookings PRIMARY KEY CLUSTERED (BookingId ASC),
        CONSTRAINT UQ_Bookings_BookingReference UNIQUE NONCLUSTERED (BookingReference ASC),
        CONSTRAINT FK_Bookings_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (UserId),
        CONSTRAINT FK_Bookings_Rooms FOREIGN KEY (RoomId)
            REFERENCES dbo.Rooms (RoomId),
        CONSTRAINT CK_Bookings_DateOrder CHECK (CheckOutDate > CheckInDate),
        CONSTRAINT CK_Bookings_TotalAmount CHECK (TotalAmount >= 0.00),
        CONSTRAINT CK_Bookings_NumberOfGuests CHECK (NumberOfGuests >= 1),
        CONSTRAINT CK_Bookings_BookingStatus CHECK (BookingStatus IN (N'Confirmed', N'Cancelled', N'Completed'))
    );
END
GO

-- -----------------------------------------------------------------------------
-- Table 7: Cancellations
-- Description: 1:1 financial and audit record of cancelled bookings with refund metrics.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Cancellations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Cancellations
    (
        CancellationId      INT IDENTITY(1,1) NOT NULL,
        BookingId           INT               NOT NULL,
        CancelledByUserId   INT               NOT NULL,
        CancellationReason  NVARCHAR(500)     NULL,
        RefundPercentage    DECIMAL(5,2)      NOT NULL,
        RefundAmount        DECIMAL(18,2)     NOT NULL,
        CancellationCharge  DECIMAL(18,2)     NOT NULL,
        CancelledAt         DATETIME2(7)      NOT NULL CONSTRAINT DF_Cancellations_CancelledAt DEFAULT (SYSUTCDATETIME()),

        CONSTRAINT PK_Cancellations PRIMARY KEY CLUSTERED (CancellationId ASC),
        CONSTRAINT UQ_Cancellations_BookingId UNIQUE NONCLUSTERED (BookingId ASC),
        CONSTRAINT FK_Cancellations_Bookings FOREIGN KEY (BookingId)
            REFERENCES dbo.Bookings (BookingId),
        CONSTRAINT FK_Cancellations_Users FOREIGN KEY (CancelledByUserId)
            REFERENCES dbo.Users (UserId),
        CONSTRAINT CK_Cancellations_RefundPercentage CHECK (RefundPercentage IN (0.00, 50.00, 100.00)),
        CONSTRAINT CK_Cancellations_RefundAmount CHECK (RefundAmount >= 0.00),
        CONSTRAINT CK_Cancellations_CancellationCharge CHECK (CancellationCharge >= 0.00)
    );
END
GO

-- -----------------------------------------------------------------------------
-- Table 8: AuditLogs
-- Description: Immutable security and transactional audit journal.
-- -----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.AuditLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.AuditLogs
    (
        AuditLogId          BIGINT IDENTITY(1,1) NOT NULL,
        TableName           NVARCHAR(100)        NOT NULL,
        RecordId            NVARCHAR(50)         NOT NULL,
        Action              NVARCHAR(20)         NOT NULL,
        UserId              INT                  NULL,
        OldValues           NVARCHAR(MAX)        NULL,
        NewValues           NVARCHAR(MAX)        NULL,
        Timestamp           DATETIME2(7)         NOT NULL CONSTRAINT DF_AuditLogs_Timestamp DEFAULT (SYSUTCDATETIME()),
        IpAddress           NVARCHAR(45)         NULL,

        CONSTRAINT PK_AuditLogs PRIMARY KEY CLUSTERED (AuditLogId ASC),
        CONSTRAINT FK_AuditLogs_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (UserId) ON DELETE SET NULL,
        CONSTRAINT CK_AuditLogs_Action CHECK (Action IN (N'INSERT', N'UPDATE', N'DELETE', N'CANCEL', N'LOGIN'))
    );
END
GO
/*
================================================================================
Created By   : Avijit Pakhira
Date Created : 2026-11-17
Topic Name   : Enterprise Hotel Booking & Reservation System - Performance Indexes
Description  : Production-grade Non-Clustered, Composite, and Covering Indexes
               optimizing hotel room search, date overlap intervals, foreign keys,
               and user authentication lookups.
================================================================================
*/

USE HotelBookingDb;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- -----------------------------------------------------------------------------
-- Index 1: Users - Role & Active Status (Covering Index for Admin lookups)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Users_Role_IsActive' AND object_id = OBJECT_ID(N'dbo.Users'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Users_Role_IsActive
    ON dbo.Users (Role ASC, IsActive ASC)
    INCLUDE (UserId, FullName, Email, PhoneNumber);
END
GO

-- -----------------------------------------------------------------------------
-- Index 2: Rooms - RoomTypeId & Status (Covering Index for Room Inventory & Search)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Rooms_RoomTypeId_Status' AND object_id = OBJECT_ID(N'dbo.Rooms'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Rooms_RoomTypeId_Status
    ON dbo.Rooms (RoomTypeId ASC, Status ASC, IsActive ASC)
    INCLUDE (RoomId, RoomNumber, FloorNumber);
END
GO

-- -----------------------------------------------------------------------------
-- Index 3: RoomAmenities - AmenityId & RoomId (Reverse Junction Index for Relational Division)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_RoomAmenities_AmenityId_RoomId' AND object_id = OBJECT_ID(N'dbo.RoomAmenities'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_RoomAmenities_AmenityId_RoomId
    ON dbo.RoomAmenities (AmenityId ASC, RoomId ASC);
END
GO

-- -----------------------------------------------------------------------------
-- Index 4: Bookings - RoomId, Status, Dates (CRITICAL Index for Overlap Detection & Concurrency)
-- Purpose: Direct index seek for room availability checks in usp_Booking_Create and usp_Room_SearchAvailable.
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_RoomId_Dates' AND object_id = OBJECT_ID(N'dbo.Bookings'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Bookings_RoomId_Dates
    ON dbo.Bookings (RoomId ASC, BookingStatus ASC, CheckInDate ASC, CheckOutDate ASC)
    INCLUDE (BookingId, TotalAmount, UserId, NumberOfGuests);
END
GO

-- -----------------------------------------------------------------------------
-- Index 5: Bookings - UserId & CreatedAt (Customer Reservation History)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bookings_UserId_CreatedAt' AND object_id = OBJECT_ID(N'dbo.Bookings'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Bookings_UserId_CreatedAt
    ON dbo.Bookings (UserId ASC, CreatedAt DESC)
    INCLUDE (BookingReference, RoomId, BookingStatus, TotalAmount, CheckInDate, CheckOutDate);
END
GO

-- -----------------------------------------------------------------------------
-- Index 6: Cancellations - BookingId & Date (Refund Lookup & Audit Index)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Cancellations_BookingId' AND object_id = OBJECT_ID(N'dbo.Cancellations'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Cancellations_BookingId
    ON dbo.Cancellations (BookingId ASC)
    INCLUDE (CancellationId, RefundPercentage, RefundAmount, CancellationCharge, CancelledAt);
END
GO

-- -----------------------------------------------------------------------------
-- Index 7: AuditLogs - TableName, RecordId, Timestamp (Forensic Query Index)
-- -----------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLogs_TableName_RecordId' AND object_id = OBJECT_ID(N'dbo.AuditLogs'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditLogs_TableName_RecordId
    ON dbo.AuditLogs (TableName ASC, RecordId ASC, Timestamp DESC);
END
GO
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

    -- Basic input validation
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
        -- Check email uniqueness
        IF EXISTS (SELECT 1 FROM dbo.Users WHERE Email = @Email)
        BEGIN
            THROW 50005, N'A user with this email address already exists.', 1;
        END

        -- Check phone uniqueness if supplied
        IF @PhoneNumber IS NOT NULL AND EXISTS (SELECT 1 FROM dbo.Users WHERE PhoneNumber = @PhoneNumber)
        BEGIN
            THROW 50005, N'A user with this phone number already exists.', 1;
        END

        INSERT INTO dbo.Users
        (
            FullName,
            Email,
            PasswordHash,
            PasswordSalt,
            PhoneNumber,
            Role,
            IsActive,
            CreatedAt
        )
        VALUES
        (
            @FullName,
            @Email,
            @PasswordHash,
            @PasswordSalt,
            @PhoneNumber,
            @Role,
            1,
            SYSUTCDATETIME()
        );

        SET @CreatedUserId = SCOPE_IDENTITY();

        -- Audit trail entry
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

        -- Return registered user profile (excluding sensitive password hash/salt)
        SELECT UserId, FullName, Email, PhoneNumber, Role, IsActive, CreatedAt
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

        -- Audit trail entry
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

        -- Return updated user
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

        -- Check if any active future bookings exist for any room belonging to this room type
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

    -- Result Set 1: Room & Type Information
    SELECT r.RoomId, r.RoomNumber, r.FloorNumber, r.Status, r.IsActive, r.CreatedAt, r.UpdatedAt,
           rt.RoomTypeId, rt.TypeName, rt.Description AS RoomTypeDescription, rt.BasePrice, rt.MaxOccupancy
    FROM dbo.Rooms r
    INNER JOIN dbo.RoomTypes rt ON r.RoomTypeId = rt.RoomTypeId
    WHERE r.RoomId = @RoomId;

    -- Result Set 2: Associated Amenities
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
    @AmenityIds NVARCHAR(MAX) -- Comma-separated list of Amenity IDs e.g. '1,2,5'
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

        -- Parse Amenity IDs
        DECLARE @AmenitiesToAssign TABLE (AmenityId INT PRIMARY KEY);
        INSERT INTO @AmenitiesToAssign (AmenityId)
        SELECT DISTINCT CAST(value AS INT)
        FROM STRING_SPLIT(@AmenityIds, N',')
        WHERE RTRIM(LTRIM(value)) <> N'';

        -- Verify all provided amenities exist
        IF EXISTS (
            SELECT 1 
            FROM @AmenitiesToAssign a
            WHERE NOT EXISTS (SELECT 1 FROM dbo.Amenities m WHERE m.AmenityId = a.AmenityId AND m.IsActive = 1)
        )
        BEGIN
            THROW 50003, N'One or more specified Amenity IDs do not exist or are inactive.', 1;
        END

        -- Insert missing mappings idempotently
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt)
        SELECT @RoomId, a.AmenityId, SYSUTCDATETIME()
        FROM @AmenitiesToAssign a
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.RoomAmenities ra 
            WHERE ra.RoomId = @RoomId AND ra.AmenityId = a.AmenityId
        );

        COMMIT TRANSACTION;

        -- Return current mapped amenities
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

    -- Validate dates
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

    -- Parse required amenities for Relational Division
    DECLARE @ReqAmenities TABLE (AmenityId INT PRIMARY KEY);
    IF @AmenityIds IS NOT NULL AND LEN(RTRIM(LTRIM(@AmenityIds))) > 0
    BEGIN
        INSERT INTO @ReqAmenities (AmenityId)
        SELECT DISTINCT CAST(value AS INT)
        FROM STRING_SPLIT(@AmenityIds, N',')
        WHERE RTRIM(LTRIM(value)) <> N'';
    END;

    DECLARE @RequiredAmenityCount INT = (SELECT COUNT(*) FROM @ReqAmenities);

    -- Compute Total Matching Records for Pagination Metadata
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
      -- Overlap Detection Logic: Exclude room if any Confirmed booking overlaps
      AND NOT EXISTS (
          SELECT 1 FROM dbo.Bookings b
          WHERE b.RoomId = r.RoomId
            AND b.BookingStatus IN (N'Confirmed', N'CheckedIn')
            AND b.CheckInDate < @CheckOutDate
            AND b.CheckOutDate > @CheckInDate
      )
      -- Relational Division: Must possess ALL specified amenities
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

    -- Return Paginated Available Rooms with Calculated Stay Price
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

    -- 1. Input Validation
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

    -- 2. Concurrency Transaction Boundary
    BEGIN TRANSACTION;
    BEGIN TRY
        -- A. Row-level lock on the Room to serialize concurrent attempts on this specific room
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

        -- B. Range-lock check on Bookings table to eliminate double-booking race condition
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

        -- C. Server-Enforced Stay Price Calculation (Never trust client input)
        SET @CalculatedTotalAmount = @BasePrice * @TotalNights;

        -- D. Generate Atomic Unique Booking Reference (BK-YYYYMMDD-XXXX)
        SET @SeqVal = NEXT VALUE FOR dbo.BookingReferenceSeq;
        SET @BookingRef = CONCAT(
            N'BK-',
            FORMAT(SYSUTCDATETIME(), N'yyyyMMdd'),
            N'-',
            RIGHT(N'0000' + CAST(@SeqVal AS NVARCHAR(4)), 4)
        );

        -- E. Insert Booking Record
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

        -- F. Audit Trail Entry
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

        -- Return full hydrated booking record
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

    -- 1. State Inspection & Validation
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

    -- Standardize check-in time at 14:00:00 UTC on CheckInDate
    SET @CheckInDateTime = DATEADD(HOUR, 14, CAST(@CheckInDate AS DATETIME2(7)));
    SET @HoursUntilCheckIn = DATEDIFF(MINUTE, SYSUTCDATETIME(), @CheckInDateTime) / 60.0;

    -- 2. 3-Tier Policy Calculation:
    --    > 48 hours: 100% Refund (0% charge)
    --    24 to 48 hours: 50% Refund (50% charge)
    --    < 24 hours: 0% Refund (100% charge)
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

    -- 3. Atomic Mutation Transaction
    BEGIN TRANSACTION;
    BEGIN TRY
        -- A. Update Booking status to Cancelled
        UPDATE dbo.Bookings
        SET BookingStatus = N'Cancelled',
            UpdatedAt     = SYSUTCDATETIME()
        WHERE BookingId = @BookingId;

        -- B. Insert financial breakdown into Cancellations table
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

        -- C. Audit Trail Entry
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

        -- Return Cancellation summary to caller
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
/*
================================================================================
Created By   : Avijit Pakhira
Date Created : 2026-11-17
Topic Name   : Enterprise Hotel Booking & Reservation System - Database Seed Data
Description  : Production-grade idempotent seed data populating Admin and Customer
               accounts with PBKDF2 password hashes, Room Types, Physical Rooms,
               Amenities catalog, and Room-Amenity mappings.
               Wrapped in an atomic transaction with COMMIT / ROLLBACK.
================================================================================
*/

USE HotelBookingDb;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

BEGIN TRANSACTION;
BEGIN TRY

    -- -------------------------------------------------------------------------
    -- 1. SEED USERS (Admin and Customer with Distinct Passwords & Salts)
    -- Admin Account   : admin@hotel.com       / Password: "Admin@Hotel2026!"
    -- Customer Account: customer@example.com / Password: "Customer@Pass2026!"
    -- Derived using PBKDF2 (HMAC-SHA256, 16-byte salt, 100,000 iterations, 32-byte key)
    -- -------------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'admin@hotel.com')
    BEGIN
        INSERT INTO dbo.Users (FullName, Email, PasswordHash, PasswordSalt, PhoneNumber, Role, IsActive, CreatedAt)
        VALUES (
            N'System Administrator',
            N'admin@hotel.com',
            N'VGA6NpHTjLFoY+TfGm6Tc8TS1LUzeat9kZ0wH4kPMzc=', -- Hash of "Admin@Hotel2026!"
            N'g2toFVBnKX8INaE8gTmc9g==',                     -- Unique 16-byte cryptographic salt
            N'+1-555-0100',
            N'Admin',
            1,
            SYSUTCDATETIME()
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'customer@example.com')
    BEGIN
        INSERT INTO dbo.Users (FullName, Email, PasswordHash, PasswordSalt, PhoneNumber, Role, IsActive, CreatedAt)
        VALUES (
            N'John Doe',
            N'customer@example.com',
            N'tvHoCQUO8PNftfYpAQCOgVh38LZWWRd0RZRCBbm4szc=', -- Hash of "Customer@Pass2026!"
            N'tAl6Z34BljRES8pKUPnOdQ==',                     -- Unique 16-byte cryptographic salt
            N'+1-555-0101',
            N'Customer',
            1,
            SYSUTCDATETIME()
        );
    END;

    --delete from dbo.Users where [UserId] = 2;

    -- -------------------------------------------------------------------------
    -- 2. SEED ROOM TYPES
    -- -------------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE TypeName = N'Standard Single')
    BEGIN
        INSERT INTO dbo.RoomTypes (TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt)
        VALUES (N'Standard Single', N'Cozy single room with a queen bed, desk, and ensuite bathroom.', 100.00, 1, 1, SYSUTCDATETIME());
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE TypeName = N'Deluxe Double')
    BEGIN
        INSERT INTO dbo.RoomTypes (TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt)
        VALUES (N'Deluxe Double', N'Spacious double room with a king bed, seating area, and smart TV.', 180.00, 2, 1, SYSUTCDATETIME());
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE TypeName = N'Executive Suite')
    BEGIN
        INSERT INTO dbo.RoomTypes (TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt)
        VALUES (N'Executive Suite', N'Luxury corner suite with ocean view, separate living lounge, and Jacuzzi.', 350.00, 4, 1, SYSUTCDATETIME());
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.RoomTypes WHERE TypeName = N'Presidential Penthouse')
    BEGIN
        INSERT INTO dbo.RoomTypes (TypeName, Description, BasePrice, MaxOccupancy, IsActive, CreatedAt)
        VALUES (N'Presidential Penthouse', N'Top-floor penthouse with private balcony, dining area, and concierge service.', 750.00, 6, 1, SYSUTCDATETIME());
    END;

    -- -------------------------------------------------------------------------
    -- 3. SEED PHYSICAL ROOMS
    -- -------------------------------------------------------------------------
    DECLARE @StandardTypeId INT = (SELECT RoomTypeId FROM dbo.RoomTypes WHERE TypeName = N'Standard Single');
    DECLARE @DeluxeTypeId INT = (SELECT RoomTypeId FROM dbo.RoomTypes WHERE TypeName = N'Deluxe Double');
    DECLARE @SuiteTypeId INT = (SELECT RoomTypeId FROM dbo.RoomTypes WHERE TypeName = N'Executive Suite');
    DECLARE @PenthouseTypeId INT = (SELECT RoomTypeId FROM dbo.RoomTypes WHERE TypeName = N'Presidential Penthouse');

    -- Floor 1: Standard Rooms
    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'101')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'101', @StandardTypeId, 1, N'Available', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'102')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'102', @StandardTypeId, 1, N'Available', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'103')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'103', @StandardTypeId, 1, N'Under Maintenance', 1, SYSUTCDATETIME());

    -- Floor 2: Deluxe Rooms
    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'201')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'201', @DeluxeTypeId, 2, N'Available', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'202')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'202', @DeluxeTypeId, 2, N'Available', 1, SYSUTCDATETIME());

    -- Floor 3: Executive Suites
    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'301')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'301', @SuiteTypeId, 3, N'Available', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'302')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'302', @SuiteTypeId, 3, N'Available', 1, SYSUTCDATETIME());

    -- Floor 4: Presidential Penthouse
    IF NOT EXISTS (SELECT 1 FROM dbo.Rooms WHERE RoomNumber = N'401')
        INSERT INTO dbo.Rooms (RoomNumber, RoomTypeId, FloorNumber, Status, IsActive, CreatedAt)
        VALUES (N'401', @PenthouseTypeId, 4, N'Available', 1, SYSUTCDATETIME());

    -- -------------------------------------------------------------------------
    -- 4. SEED AMENITIES
    -- -------------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE Name = N'High-Speed Wi-Fi')
        INSERT INTO dbo.Amenities (Name, Description, IconCode, IsActive, CreatedAt)
        VALUES (N'High-Speed Wi-Fi', N'Complimentary 1Gbps fiber internet connection.', N'wifi', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE Name = N'Air Conditioning')
        INSERT INTO dbo.Amenities (Name, Description, IconCode, IsActive, CreatedAt)
        VALUES (N'Air Conditioning', N'Climate-controlled central heating and air conditioning.', N'ac_unit', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE Name = N'Smart TV & Streaming')
        INSERT INTO dbo.Amenities (Name, Description, IconCode, IsActive, CreatedAt)
        VALUES (N'Smart TV & Streaming', N'55-inch 4K UHD Smart TV with Netflix and YouTube.', N'tv', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE Name = N'Mini Bar')
        INSERT INTO dbo.Amenities (Name, Description, IconCode, IsActive, CreatedAt)
        VALUES (N'Mini Bar', N'Stocked mini refrigerator with refreshments and snacks.', N'local_bar', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE Name = N'Sea View Balcony')
        INSERT INTO dbo.Amenities (Name, Description, IconCode, IsActive, CreatedAt)
        VALUES (N'Sea View Balcony', N'Private balcony with panoramic ocean sunset views.', N'balcony', 1, SYSUTCDATETIME());

    IF NOT EXISTS (SELECT 1 FROM dbo.Amenities WHERE Name = N'Jacuzzi Bathtub')
        INSERT INTO dbo.Amenities (Name, Description, IconCode, IsActive, CreatedAt)
        VALUES (N'Jacuzzi Bathtub', N'Deep hydrotherapy jacuzzi tub in private bathroom.', N'hot_tub', 1, SYSUTCDATETIME());

    -- -------------------------------------------------------------------------
    -- 5. SEED ROOM-AMENITY MAPPINGS
    -- -------------------------------------------------------------------------
    DECLARE @WifiId INT = (SELECT AmenityId FROM dbo.Amenities WHERE Name = N'High-Speed Wi-Fi');
    DECLARE @AcId INT = (SELECT AmenityId FROM dbo.Amenities WHERE Name = N'Air Conditioning');
    DECLARE @TvId INT = (SELECT AmenityId FROM dbo.Amenities WHERE Name = N'Smart TV & Streaming');
    DECLARE @MiniBarId INT = (SELECT AmenityId FROM dbo.Amenities WHERE Name = N'Mini Bar');
    DECLARE @SeaViewId INT = (SELECT AmenityId FROM dbo.Amenities WHERE Name = N'Sea View Balcony');
    DECLARE @JacuzziId INT = (SELECT AmenityId FROM dbo.Amenities WHERE Name = N'Jacuzzi Bathtub');

    DECLARE @Room101 INT = (SELECT RoomId FROM dbo.Rooms WHERE RoomNumber = N'101');
    DECLARE @Room201 INT = (SELECT RoomId FROM dbo.Rooms WHERE RoomNumber = N'201');
    DECLARE @Room301 INT = (SELECT RoomId FROM dbo.Rooms WHERE RoomNumber = N'301');
    DECLARE @Room401 INT = (SELECT RoomId FROM dbo.Rooms WHERE RoomNumber = N'401');

    -- Room 101: Standard (Wi-Fi, AC)
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room101 AND AmenityId = @WifiId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room101, @WifiId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room101 AND AmenityId = @AcId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room101, @AcId, SYSUTCDATETIME());

    -- Room 201: Deluxe (Wi-Fi, AC, Smart TV, Mini Bar)
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room201 AND AmenityId = @WifiId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room201, @WifiId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room201 AND AmenityId = @AcId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room201, @AcId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room201 AND AmenityId = @TvId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room201, @TvId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room201 AND AmenityId = @MiniBarId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room201, @MiniBarId, SYSUTCDATETIME());

    -- Room 301: Suite (Wi-Fi, AC, Smart TV, Mini Bar, Sea View, Jacuzzi)
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room301 AND AmenityId = @WifiId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room301, @WifiId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room301 AND AmenityId = @AcId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room301, @AcId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room301 AND AmenityId = @TvId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room301, @TvId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room301 AND AmenityId = @MiniBarId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room301, @MiniBarId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room301 AND AmenityId = @SeaViewId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room301, @SeaViewId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room301 AND AmenityId = @JacuzziId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room301, @JacuzziId, SYSUTCDATETIME());

    -- Room 401: Penthouse (All Amenities)
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room401 AND AmenityId = @WifiId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room401, @WifiId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room401 AND AmenityId = @AcId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room401, @AcId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room401 AND AmenityId = @TvId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room401, @TvId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room401 AND AmenityId = @MiniBarId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room401, @MiniBarId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room401 AND AmenityId = @SeaViewId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room401, @SeaViewId, SYSUTCDATETIME());
    IF NOT EXISTS (SELECT 1 FROM dbo.RoomAmenities WHERE RoomId = @Room401 AND AmenityId = @JacuzziId)
        INSERT INTO dbo.RoomAmenities (RoomId, AmenityId, AssignedAt) VALUES (@Room401, @JacuzziId, SYSUTCDATETIME());

    COMMIT TRANSACTION;
    PRINT N'Seed data successfully executed and committed.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
