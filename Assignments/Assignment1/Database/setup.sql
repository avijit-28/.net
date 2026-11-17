IF DB_ID(N'InventoryDb') IS NULL CREATE DATABASE InventoryDb;
GO
USE InventoryDb;
GO

IF OBJECT_ID(N'dbo.Assets', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Assets
    (
        AssetId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Assets PRIMARY KEY,
        AssetName nvarchar(150) NOT NULL,
        Category nvarchar(100) NOT NULL,
        PurchaseDate date NOT NULL,
        Status nvarchar(30) NOT NULL,
        Price decimal(11,2) NOT NULL CONSTRAINT CK_Assets_Price CHECK (Price >= 0)
    );
END;
IF OBJECT_ID(N'dbo.ApiUsers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApiUsers
    (
        Username nvarchar(100) NOT NULL CONSTRAINT PK_ApiUsers PRIMARY KEY,
        PasswordHash nvarchar(500) NOT NULL
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetAllAssets AS
BEGIN
    SET NOCOUNT ON;
    SELECT AssetId, AssetName, Category, PurchaseDate, Status, Price FROM dbo.Assets ORDER BY AssetId;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_GetAssetById @AssetId int AS
BEGIN
    SET NOCOUNT ON;
    SELECT AssetId, AssetName, Category, PurchaseDate, Status, Price FROM dbo.Assets WHERE AssetId = @AssetId;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_InsertAsset
    @AssetName nvarchar(150), @Category nvarchar(100), @PurchaseDate date, @Status nvarchar(30), @Price decimal(11,2), @AssetId int OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.Assets (AssetName, Category, PurchaseDate, Status, Price) VALUES (@AssetName, @Category, @PurchaseDate, @Status, @Price);
    SET @AssetId = CONVERT(int, SCOPE_IDENTITY());
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_UpdateAsset
    @AssetId int, @AssetName nvarchar(150), @Category nvarchar(100), @PurchaseDate date, @Status nvarchar(30), @Price decimal(11,2)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Assets SET AssetName=@AssetName, Category=@Category, PurchaseDate=@PurchaseDate, Status=@Status, Price=@Price WHERE AssetId=@AssetId;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_DeleteAsset @AssetId int AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Assets SET Status=N'Inactive' WHERE AssetId=@AssetId;
END;
GO
CREATE OR ALTER PROCEDURE dbo.sp_GetUserByUsername @Username nvarchar(100) AS
BEGIN
    SET NOCOUNT ON;
    SELECT PasswordHash FROM dbo.ApiUsers WHERE Username=@Username;
END;
GO
