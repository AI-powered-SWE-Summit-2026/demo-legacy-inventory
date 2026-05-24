IF DB_ID('LegacyInventoryDb') IS NULL
BEGIN
    CREATE DATABASE LegacyInventoryDb;
END
GO

USE LegacyInventoryDb;
GO

IF OBJECT_ID('dbo.Shipments', 'U') IS NOT NULL DROP TABLE dbo.Shipments;
IF OBJECT_ID('dbo.PurchaseOrderLines', 'U') IS NOT NULL DROP TABLE dbo.PurchaseOrderLines;
IF OBJECT_ID('dbo.PurchaseOrders', 'U') IS NOT NULL DROP TABLE dbo.PurchaseOrders;
IF OBJECT_ID('dbo.StockLevels', 'U') IS NOT NULL DROP TABLE dbo.StockLevels;
IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Warehouses', 'U') IS NOT NULL DROP TABLE dbo.Warehouses;
IF OBJECT_ID('dbo.Suppliers', 'U') IS NOT NULL DROP TABLE dbo.Suppliers;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
GO

CREATE TABLE dbo.Categories
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(1000) NULL,
    ParentCategoryId INT NULL
);
GO

ALTER TABLE dbo.Categories
ADD CONSTRAINT FK_Categories_ParentCategory FOREIGN KEY (ParentCategoryId) REFERENCES dbo.Categories(Id);
GO

CREATE TABLE dbo.Suppliers
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    ContactName NVARCHAR(100) NULL,
    Email NVARCHAR(200) NULL,
    Phone NVARCHAR(50) NULL,
    Address NVARCHAR(500) NULL,
    Country NVARCHAR(100) NULL,
    IsActive BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL
);
GO

CREATE TABLE dbo.Warehouses
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Location NVARCHAR(250) NULL,
    Capacity INT NOT NULL,
    ManagerName NVARCHAR(100) NULL,
    IsActive BIT NOT NULL
);
GO

CREATE TABLE dbo.Products
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    SKU NVARCHAR(50) NOT NULL,
    Description NVARCHAR(2000) NULL,
    CategoryId INT NOT NULL,
    SupplierId INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    ReorderLevel INT NOT NULL,
    IsActive BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CONSTRAINT UQ_Products_SKU UNIQUE (SKU),
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id)
);
GO

CREATE TABLE dbo.StockLevels
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProductId INT NOT NULL,
    WarehouseId INT NOT NULL,
    Quantity INT NOT NULL,
    LastUpdated DATETIME2 NOT NULL,
    ReservedQuantity INT NOT NULL,
    CONSTRAINT FK_StockLevels_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT FK_StockLevels_Warehouses FOREIGN KEY (WarehouseId) REFERENCES dbo.Warehouses(Id)
);
GO

CREATE TABLE dbo.PurchaseOrders
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SupplierId INT NOT NULL,
    OrderNumber NVARCHAR(50) NOT NULL,
    Status NVARCHAR(25) NOT NULL,
    OrderDate DATETIME2 NOT NULL,
    ExpectedDeliveryDate DATETIME2 NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Notes NVARCHAR(2000) NULL,
    CreatedByUserId NVARCHAR(100) NULL,
    CONSTRAINT UQ_PurchaseOrders_OrderNumber UNIQUE (OrderNumber),
    CONSTRAINT FK_PurchaseOrders_Suppliers FOREIGN KEY (SupplierId) REFERENCES dbo.Suppliers(Id)
);
GO

CREATE TABLE dbo.PurchaseOrderLines
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PurchaseOrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_PurchaseOrderLines_PurchaseOrders FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrders(Id),
    CONSTRAINT FK_PurchaseOrderLines_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id)
);
GO

CREATE TABLE dbo.Shipments
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    PurchaseOrderId INT NOT NULL,
    ShipmentDate DATETIME2 NULL,
    TrackingNumber NVARCHAR(100) NULL,
    CarrierName NVARCHAR(100) NULL,
    Status NVARCHAR(25) NULL,
    EstimatedArrival DATETIME2 NULL,
    CONSTRAINT FK_Shipments_PurchaseOrders FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrders(Id)
);
GO
