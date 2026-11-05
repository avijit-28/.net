-- ======================================================================
-- ASP.NET Web API 2 Practice Projects Database Setup Script
-- Target Database: WEBAPI_DB
-- Projects Covered:
--   1. Project 1: 3-Layer Architecture (Users & Authentication)
--   2. Project 2: ADO.NET Direct & MVC Client (Customer Management)
-- ======================================================================

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'WEBAPI_DB')
BEGIN
    CREATE DATABASE WEBAPI_DB;
END;
GO

USE WEBAPI_DB;
GO

-- ======================================================================
-- 1. PROJECT 1: USERS TABLE & STORED PROCEDURES (3-Layer Architecture)
-- ======================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserID INT PRIMARY KEY,
        UserName NVARCHAR(100) NOT NULL,
        UserPassword NVARCHAR(100) NOT NULL,
        UserRoles NVARCHAR(50) NOT NULL,
        UserEmailID NVARCHAR(100) NOT NULL
    );

    INSERT INTO Users (UserID, UserName, UserPassword, UserRoles, UserEmailID)
    VALUES 
    (101, 'john_doe', 'pass123', 'Admin', 'john@example.com'),
    (102, 'alice_smith', 'alice2026', 'User', 'alice@example.com'),
    (103, 'bob_manager', 'bobsecret', 'Manager', 'bob@example.com');
END;
GO

CREATE OR ALTER PROCEDURE spGetLoginDetails
    @UserID INT,
    @UserPassword NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, UserName, UserPassword, UserRoles, UserEmailID 
    FROM Users 
    WHERE UserID = @UserID AND UserPassword = @UserPassword;
END;
GO

CREATE OR ALTER PROCEDURE spInsertUsers
    @UserID INT,
    @UserName NVARCHAR(100),
    @UserPassword NVARCHAR(100),
    @UserRoles NVARCHAR(50),
    @UserEmailID NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID)
    BEGIN
        SELECT 'User with ID ' + CAST(@UserID AS NVARCHAR(20)) + ' already exists.' AS Result;
        RETURN;
    END
    INSERT INTO Users (UserID, UserName, UserPassword, UserRoles, UserEmailID)
    VALUES (@UserID, @UserName, @UserPassword, @UserRoles, @UserEmailID);
    SELECT 'User ' + CAST(@UserID AS NVARCHAR(20)) + ' added successfully.' AS Result;
END;
GO

CREATE OR ALTER PROCEDURE spUpdateUsers
    @UserID INT,
    @UserName NVARCHAR(100),
    @UserPassword NVARCHAR(100),
    @UserRoles NVARCHAR(50),
    @UserEmailID NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID)
    BEGIN
        SELECT 'User with ID ' + CAST(@UserID AS NVARCHAR(20)) + ' not found.' AS Result;
        RETURN;
    END
    UPDATE Users 
    SET UserName = @UserName,
        UserPassword = @UserPassword,
        UserRoles = @UserRoles,
        UserEmailID = @UserEmailID
    WHERE UserID = @UserID;
    SELECT 'User ' + CAST(@UserID AS NVARCHAR(20)) + ' updated successfully.' AS Result;
END;
GO

CREATE OR ALTER PROCEDURE spDeleteUser
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID)
    BEGIN
        SELECT 'User with ID ' + CAST(@UserID AS NVARCHAR(20)) + ' not found.' AS Result;
        RETURN;
    END
    DELETE FROM Users WHERE UserID = @UserID;
    SELECT 'User ' + CAST(@UserID AS NVARCHAR(20)) + ' deleted successfully.' AS Result;
END;
GO

CREATE OR ALTER PROCEDURE spGetAllUsers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, UserName, UserPassword, UserRoles, UserEmailID FROM Users;
END;
GO

-- ======================================================================
-- 2. PROJECT 2: CUSTOMER TABLE & STORED PROCEDURES (ADO.NET Direct & MVC)
-- ======================================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Customer')
BEGIN
    CREATE TABLE Customer (
        CustomerId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        MobileNo NVARCHAR(20) NOT NULL,
        EmailID NVARCHAR(100) NOT NULL,
        Country NVARCHAR(50) NOT NULL
    );

    INSERT INTO Customer (Name, MobileNo, EmailID, Country)
    VALUES 
    ('Amit Kumar', '9876543210', 'amit.kumar@example.com', 'India'),
    ('Sarah Connor', '9123456780', 'sarah.c@example.com', 'USA'),
    ('Carlos Diaz', '9554433221', 'carlos.d@example.com', 'Spain');
END;
GO

CREATE OR ALTER PROCEDURE spGetCustomers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CustomerId, Name, MobileNo, EmailID, Country FROM Customer;
END;
GO

CREATE OR ALTER PROCEDURE spGetCustomerByID
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CustomerId, Name, MobileNo, EmailID, Country FROM Customer WHERE CustomerId = @CustomerId;
END;
GO

CREATE OR ALTER PROCEDURE spInsertCustomer
    @Name NVARCHAR(100),
    @MobileNo NVARCHAR(20),
    @EmailID NVARCHAR(100),
    @Country NVARCHAR(50),
    @CustomerID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Customer (Name, MobileNo, EmailID, Country)
    VALUES (@Name, @MobileNo, @EmailID, @Country);
    SET @CustomerID = SCOPE_IDENTITY();
END;
GO

CREATE OR ALTER PROCEDURE spUpdateCustomer
    @ID INT,
    @Name NVARCHAR(100),
    @MobileNo NVARCHAR(20),
    @EmailID NVARCHAR(100),
    @Country NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Customer 
    SET Name = @Name,
        MobileNo = @MobileNo,
        EmailID = @EmailID,
        Country = @Country
    WHERE CustomerId = @ID;
END;
GO

CREATE OR ALTER PROCEDURE spDeleteCustomer
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Customer WHERE CustomerId = @ID;
END;
GO
