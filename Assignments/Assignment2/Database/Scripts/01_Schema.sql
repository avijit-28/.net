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
