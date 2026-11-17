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
    -- 1. SEED USERS (Admin and Customer)
    -- Default Password for both accounts: "Password@123"
    -- Pre-computed PBKDF2-SHA256 salted hashes:
    -- -------------------------------------------------------------------------
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Email = N'admin@hotel.com')
    BEGIN
        INSERT INTO dbo.Users (FullName, Email, PasswordHash, PasswordSalt, PhoneNumber, Role, IsActive, CreatedAt)
        VALUES (
            N'System Administrator',
            N'admin@hotel.com',
            N'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', -- PBKDF2 hash of "Password@123"
            N'BwIAAACkAABBAAAAYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY=',
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
            N'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', -- PBKDF2 hash of "Password@123"
            N'BwIAAACkAABBAAAAYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXoxMjM0NTY=',
            N'+1-555-0101',
            N'Customer',
            1,
            SYSUTCDATETIME()
        );
    END;

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
