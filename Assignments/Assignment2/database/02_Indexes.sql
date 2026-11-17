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
