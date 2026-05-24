USE LegacyInventoryDb;
GO

SET IDENTITY_INSERT dbo.Categories ON;
INSERT INTO dbo.Categories (Id, Name, Description, ParentCategoryId) VALUES
(1, 'Electronics', 'Electronic devices and accessories', NULL),
(2, 'Office Supplies', 'General office and print supplies', NULL),
(3, 'Safety Equipment', 'PPE and warehouse safety stock', NULL),
(4, 'Mobile Devices', 'Phones and handheld accessories', 1);
SET IDENTITY_INSERT dbo.Categories OFF;
GO

SET IDENTITY_INSERT dbo.Suppliers ON;
INSERT INTO dbo.Suppliers (Id, Name, ContactName, Email, Phone, Address, Country, IsActive, CreatedAt) VALUES
(1, 'Northwind Components', 'Alicia Brown', 'alicia@northwind.example', '555-1000', '100 Market Street', 'USA', 1, '2024-01-10'),
(2, 'Global Industrial Supply', 'Brian Stone', 'brian@global.example', '555-1001', '250 Industrial Way', 'USA', 1, '2024-01-11'),
(3, 'Contoso Safety', 'Carla Green', 'carla@contoso.example', '555-1002', '20 Harbor Road', 'Canada', 1, '2024-01-12'),
(4, 'Fabrikam Devices', 'Diego Ortiz', 'diego@fabrikam.example', '555-1003', '90 Device Avenue', 'Mexico', 1, '2024-01-13'),
(5, 'Wide World Wholesale', 'Emma Reed', 'emma@wideworld.example', '555-1004', '17 Commerce Blvd', 'UK', 1, '2024-01-14');
SET IDENTITY_INSERT dbo.Suppliers OFF;
GO

SET IDENTITY_INSERT dbo.Warehouses ON;
INSERT INTO dbo.Warehouses (Id, Name, Location, Capacity, ManagerName, IsActive) VALUES
(1, 'Central Warehouse', 'Dallas, TX', 5000, 'Mark Turner', 1),
(2, 'East Distribution Hub', 'Atlanta, GA', 3200, 'Sara Liu', 1),
(3, 'West Overflow Site', 'Phoenix, AZ', 2200, 'James Patel', 1);
SET IDENTITY_INSERT dbo.Warehouses OFF;
GO

SET IDENTITY_INSERT dbo.Products ON;
INSERT INTO dbo.Products (Id, Name, SKU, Description, CategoryId, SupplierId, UnitPrice, ReorderLevel, IsActive, CreatedAt, UpdatedAt) VALUES
(1, 'Barcode Scanner', 'INV-1001', 'Handheld barcode scanner', 1, 1, 149.99, 8, 1, '2024-01-20', '2024-02-01'),
(2, 'Thermal Printer', 'INV-1002', 'Label thermal printer', 1, 1, 289.00, 5, 1, '2024-01-20', '2024-02-01'),
(3, 'Packing Tape', 'INV-1003', 'Industrial packing tape', 2, 2, 6.99, 30, 1, '2024-01-20', '2024-02-01'),
(4, 'Shipping Labels', 'INV-1004', 'Direct thermal labels', 2, 2, 18.50, 25, 1, '2024-01-20', '2024-02-01'),
(5, 'Safety Gloves', 'INV-1005', 'Warehouse protective gloves', 3, 3, 12.00, 20, 1, '2024-01-20', '2024-02-01'),
(6, 'Safety Glasses', 'INV-1006', 'Protective eyewear', 3, 3, 9.50, 18, 1, '2024-01-20', '2024-02-01'),
(7, 'Android Handheld', 'INV-1007', 'Rugged mobile device', 4, 4, 799.00, 4, 1, '2024-01-20', '2024-02-01'),
(8, 'iOS Handheld', 'INV-1008', 'Warehouse handheld device', 4, 4, 899.00, 3, 1, '2024-01-20', '2024-02-01'),
(9, 'Monitor Arm', 'INV-1009', 'Adjustable desk mount', 2, 5, 54.25, 6, 1, '2024-01-20', '2024-02-01'),
(10, 'Keyboard', 'INV-1010', 'USB office keyboard', 2, 5, 24.95, 10, 1, '2024-01-20', '2024-02-01'),
(11, 'Mouse', 'INV-1011', 'Wireless mouse', 2, 5, 19.95, 10, 1, '2024-01-20', '2024-02-01'),
(12, 'Wi-Fi Access Point', 'INV-1012', 'Warehouse network access point', 1, 1, 165.00, 5, 1, '2024-01-20', '2024-02-01'),
(13, 'Ethernet Switch', 'INV-1013', '24-port managed switch', 1, 1, 245.75, 4, 1, '2024-01-20', '2024-02-01'),
(14, 'Pallet Jack Wheel', 'INV-1014', 'Replacement wheel kit', 2, 2, 35.80, 12, 1, '2024-01-20', '2024-02-01'),
(15, 'Reflective Vest', 'INV-1015', 'High-visibility safety vest', 3, 3, 14.20, 15, 1, '2024-01-20', '2024-02-01'),
(16, 'Forklift Battery Water', 'INV-1016', 'Battery maintenance fluid', 3, 3, 8.40, 10, 1, '2024-01-20', '2024-02-01'),
(17, 'Tablet Mount', 'INV-1017', 'Vehicle tablet mount', 4, 4, 65.00, 6, 1, '2024-01-20', '2024-02-01'),
(18, 'Charging Dock', 'INV-1018', 'Multi-device charging dock', 4, 4, 129.00, 5, 1, '2024-01-20', '2024-02-01'),
(19, 'Clipboard', 'INV-1019', 'Heavy-duty clipboard', 2, 5, 4.25, 20, 1, '2024-01-20', '2024-02-01'),
(20, 'Cable Ties', 'INV-1020', 'Industrial cable ties pack', 2, 2, 11.35, 25, 1, '2024-01-20', '2024-02-01');
SET IDENTITY_INSERT dbo.Products OFF;
GO

INSERT INTO dbo.StockLevels (ProductId, WarehouseId, Quantity, LastUpdated, ReservedQuantity) VALUES
(1, 1, 12, '2024-02-10', 3),
(1, 2, 4, '2024-02-10', 1),
(2, 1, 3, '2024-02-10', 0),
(2, 3, 1, '2024-02-10', 0),
(3, 1, 80, '2024-02-10', 10),
(3, 2, 20, '2024-02-10', 5),
(4, 1, 24, '2024-02-10', 3),
(4, 2, 8, '2024-02-10', 1),
(5, 1, 18, '2024-02-10', 2),
(5, 3, 6, '2024-02-10', 1),
(6, 1, 16, '2024-02-10', 2),
(6, 2, 2, '2024-02-10', 0),
(7, 1, 2, '2024-02-10', 1),
(7, 2, 1, '2024-02-10', 0),
(8, 1, 0, '2024-02-10', 0),
(8, 3, 1, '2024-02-10', 0),
(9, 2, 10, '2024-02-10', 2),
(10, 2, 14, '2024-02-10', 4),
(11, 2, 7, '2024-02-10', 1),
(12, 1, 5, '2024-02-10', 1),
(13, 1, 4, '2024-02-10', 0),
(14, 3, 30, '2024-02-10', 6),
(15, 1, 13, '2024-02-10', 2),
(16, 3, 9, '2024-02-10', 1),
(17, 1, 5, '2024-02-10', 1),
(18, 2, 4, '2024-02-10', 0),
(19, 2, 40, '2024-02-10', 5),
(20, 3, 12, '2024-02-10', 3),
(20, 1, 6, '2024-02-10', 1),
(10, 3, 2, '2024-02-10', 0);
GO

SET IDENTITY_INSERT dbo.PurchaseOrders ON;
INSERT INTO dbo.PurchaseOrders (Id, SupplierId, OrderNumber, Status, OrderDate, ExpectedDeliveryDate, TotalAmount, Notes, CreatedByUserId) VALUES
(1, 1, 'PO-2024-001', 'Submitted', '2024-02-01', '2024-02-07', 588.98, 'Barcode refresh', 'legacy.user'),
(2, 2, 'PO-2024-002', 'Approved', '2024-02-03', '2024-02-12', 279.60, 'Packing supply top-up', 'legacy.user'),
(3, 3, 'PO-2024-003', 'Received', '2024-01-15', '2024-01-22', 189.00, 'Safety restock', 'warehouse.manager'),
(4, 4, 'PO-2024-004', 'Draft', '2024-02-08', '2024-02-20', 1598.00, 'Rugged device rollout', 'ops.lead'),
(5, 5, 'PO-2024-005', 'Cancelled', '2024-01-28', '2024-02-10', 124.75, 'Office peripherals', 'legacy.user');
SET IDENTITY_INSERT dbo.PurchaseOrders OFF;
GO

INSERT INTO dbo.PurchaseOrderLines (PurchaseOrderId, ProductId, Quantity, UnitPrice, TotalPrice) VALUES
(1, 1, 2, 149.99, 299.98),
(1, 2, 1, 289.00, 289.00),
(2, 3, 20, 6.99, 139.80),
(2, 4, 4, 18.45, 73.80),
(2, 20, 6, 11.00, 66.00),
(3, 5, 10, 12.00, 120.00),
(3, 6, 6, 11.50, 69.00),
(4, 7, 1, 799.00, 799.00),
(4, 8, 1, 799.00, 799.00),
(5, 10, 5, 24.95, 124.75);
GO

SET IDENTITY_INSERT dbo.Shipments ON;
INSERT INTO dbo.Shipments (Id, PurchaseOrderId, ShipmentDate, TrackingNumber, CarrierName, Status, EstimatedArrival) VALUES
(1, 1, '2024-02-04', 'TRK-10001', 'Legacy Carrier', 'InTransit', '2024-02-08'),
(2, 2, '2024-02-05', 'TRK-10002', 'Legacy Carrier', 'Pending', '2024-02-11'),
(3, 3, '2024-01-18', 'TRK-10003', 'Legacy Carrier', 'Delivered', '2024-01-21');
SET IDENTITY_INSERT dbo.Shipments OFF;
GO
