-- =============================================
-- LARGE E-COMMERCE DATABASE SCHEMA
-- =============================================

-- =============================================
-- TABLES
-- =============================================

-- Users Table
CREATE TABLE Users (
                       UserID INT IDENTITY(1,1) PRIMARY KEY,
                       Username NVARCHAR(50) NOT NULL UNIQUE,
                       Email NVARCHAR(100) NOT NULL UNIQUE,
                       PasswordHash NVARCHAR(255) NOT NULL,
                       FirstName NVARCHAR(50) NOT NULL,
                       LastName NVARCHAR(50) NOT NULL,
                       DateOfBirth DATE,
                       PhoneNumber NVARCHAR(15),
                       IsActive BIT DEFAULT 1,
                       CreatedDate DATETIME2 DEFAULT GETDATE(),
                       ModifiedDate DATETIME2 DEFAULT GETDATE(),
                       LastLoginDate DATETIME2
);

-- User Addresses
CREATE TABLE UserAddresses (
                               AddressID INT IDENTITY(1,1) PRIMARY KEY,
                               UserID INT NOT NULL,
                               AddressType NVARCHAR(20) NOT NULL, -- 'Billing', 'Shipping'
                               AddressLine1 NVARCHAR(255) NOT NULL,
                               AddressLine2 NVARCHAR(255),
                               City NVARCHAR(100) NOT NULL,
                               State NVARCHAR(50) NOT NULL,
                               ZipCode NVARCHAR(10) NOT NULL,
                               Country NVARCHAR(50) NOT NULL DEFAULT 'USA',
                               IsDefault BIT DEFAULT 0,
                               CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Categories
CREATE TABLE Categories (
                            CategoryID INT IDENTITY(1,1) PRIMARY KEY,
                            CategoryName NVARCHAR(100) NOT NULL UNIQUE,
                            Description NVARCHAR(500),
                            ParentCategoryID INT,
                            IsActive BIT DEFAULT 1,
                            CreatedDate DATETIME2 DEFAULT GETDATE(),
                            ModifiedDate DATETIME2 DEFAULT GETDATE()
);

-- Suppliers
CREATE TABLE Suppliers (
                           SupplierID INT IDENTITY(1,1) PRIMARY KEY,
                           SupplierName NVARCHAR(100) NOT NULL,
                           ContactName NVARCHAR(100),
                           Email NVARCHAR(100),
                           Phone NVARCHAR(15),
                           Address NVARCHAR(255),
                           City NVARCHAR(100),
                           State NVARCHAR(50),
                           ZipCode NVARCHAR(10),
                           Country NVARCHAR(50) DEFAULT 'USA',
                           IsActive BIT DEFAULT 1,
                           CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Products
CREATE TABLE Products (
                          ProductID INT IDENTITY(1,1) PRIMARY KEY,
                          ProductName NVARCHAR(255) NOT NULL,
                          Description NVARCHAR(1000),
                          SKU NVARCHAR(50) NOT NULL UNIQUE,
                          CategoryID INT NOT NULL,
                          SupplierID INT NOT NULL,
                          UnitPrice DECIMAL(10,2) NOT NULL,
                          UnitsInStock INT NOT NULL DEFAULT 0,
                          UnitsOnOrder INT NOT NULL DEFAULT 0,
                          ReorderLevel INT NOT NULL DEFAULT 0,
                          Discontinued BIT DEFAULT 0,
                          Weight DECIMAL(8,2),
                          Dimensions NVARCHAR(50),
                          ImageURL NVARCHAR(500),
                          CreatedDate DATETIME2 DEFAULT GETDATE(),
                          ModifiedDate DATETIME2 DEFAULT GETDATE()
);

-- Product Reviews
CREATE TABLE ProductReviews (
                                ReviewID INT IDENTITY(1,1) PRIMARY KEY,
                                ProductID INT NOT NULL,
                                UserID INT NOT NULL,
                                Rating INT NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
                                Title NVARCHAR(200),
                                ReviewText NVARCHAR(2000),
                                IsApproved BIT DEFAULT 0,
                                HelpfulCount INT DEFAULT 0,
                                CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Shopping Cart
CREATE TABLE ShoppingCart (
                              CartID INT IDENTITY(1,1) PRIMARY KEY,
                              UserID INT NOT NULL,
                              ProductID INT NOT NULL,
                              Quantity INT NOT NULL DEFAULT 1,
                              UnitPrice DECIMAL(10,2) NOT NULL,
                              CreatedDate DATETIME2 DEFAULT GETDATE(),
                              ModifiedDate DATETIME2 DEFAULT GETDATE()
);

-- Orders
CREATE TABLE Orders (
                        OrderID INT IDENTITY(1,1) PRIMARY KEY,
                        UserID INT NOT NULL,
                        OrderDate DATETIME2 DEFAULT GETDATE(),
                        RequiredDate DATETIME2,
                        ShippedDate DATETIME2,
                        ShippingAddressID INT NOT NULL,
                        BillingAddressID INT NOT NULL,
                        SubTotal DECIMAL(12,2) NOT NULL,
                        TaxAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
                        ShippingAmount DECIMAL(12,2) NOT NULL DEFAULT 0,
                        TotalAmount DECIMAL(12,2) NOT NULL,
                        OrderStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
                        PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
                        TrackingNumber NVARCHAR(50),
                        Notes NVARCHAR(1000)
);

-- Order Details
CREATE TABLE OrderDetails (
                              OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
                              OrderID INT NOT NULL,
                              ProductID INT NOT NULL,
                              Quantity INT NOT NULL,
                              UnitPrice DECIMAL(10,2) NOT NULL,
                              Discount DECIMAL(5,2) DEFAULT 0,
                              LineTotal AS (Quantity * UnitPrice * (1 - Discount)) PERSISTED
);

-- Payment Methods
CREATE TABLE PaymentMethods (
                                PaymentMethodID INT IDENTITY(1,1) PRIMARY KEY,
                                UserID INT NOT NULL,
                                PaymentType NVARCHAR(20) NOT NULL, -- 'Credit Card', 'PayPal', 'Bank Transfer'
                                CardNumber NVARCHAR(20), -- Encrypted
                                CardHolderName NVARCHAR(100),
                                ExpiryMonth INT,
                                ExpiryYear INT,
                                IsDefault BIT DEFAULT 0,
                                IsActive BIT DEFAULT 1,
                                CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Payments
CREATE TABLE Payments (
                          PaymentID INT IDENTITY(1,1) PRIMARY KEY,
                          OrderID INT NOT NULL,
                          PaymentMethodID INT NOT NULL,
                          Amount DECIMAL(12,2) NOT NULL,
                          PaymentDate DATETIME2 DEFAULT GETDATE(),
                          PaymentStatus NVARCHAR(20) NOT NULL DEFAULT 'Pending',
                          TransactionID NVARCHAR(100),
                          ProcessorResponse NVARCHAR(500),
                          CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Inventory Transactions
CREATE TABLE InventoryTransactions (
                                       TransactionID INT IDENTITY(1,1) PRIMARY KEY,
                                       ProductID INT NOT NULL,
                                       TransactionType NVARCHAR(20) NOT NULL, -- 'Purchase', 'Sale', 'Return', 'Adjustment'
                                       Quantity INT NOT NULL,
                                       UnitPrice DECIMAL(10,2),
                                       ReferenceID INT, -- Could be OrderID, PurchaseOrderID, etc.
                                       Notes NVARCHAR(500),
                                       CreatedDate DATETIME2 DEFAULT GETDATE(),
                                       CreatedBy INT NOT NULL
);

-- Audit Log
CREATE TABLE AuditLog (
                          AuditID INT IDENTITY(1,1) PRIMARY KEY,
                          TableName NVARCHAR(100) NOT NULL,
                          RecordID INT NOT NULL,
                          Action NVARCHAR(10) NOT NULL, -- 'INSERT', 'UPDATE', 'DELETE'
                          OldValues NVARCHAR(MAX),
                          NewValues NVARCHAR(MAX),
                          ChangedBy INT,
                          ChangeDate DATETIME2 DEFAULT GETDATE()
);

-- =============================================
-- FOREIGN KEY CONSTRAINTS
-- =============================================

-- User Addresses
ALTER TABLE UserAddresses
    ADD CONSTRAINT FK_UserAddresses_Users FOREIGN KEY (UserID) REFERENCES Users(UserID);

-- Categories (Self-referencing)
ALTER TABLE Categories
    ADD CONSTRAINT FK_Categories_ParentCategory FOREIGN KEY (ParentCategoryID) REFERENCES Categories(CategoryID);

-- Products
ALTER TABLE Products
    ADD CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryID) REFERENCES Categories(CategoryID),
    CONSTRAINT FK_Products_Suppliers FOREIGN KEY (SupplierID) REFERENCES Suppliers(SupplierID);

-- Product Reviews
ALTER TABLE ProductReviews
    ADD CONSTRAINT FK_ProductReviews_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT FK_ProductReviews_Users FOREIGN KEY (UserID) REFERENCES Users(UserID);

-- Shopping Cart
ALTER TABLE ShoppingCart
    ADD CONSTRAINT FK_ShoppingCart_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_ShoppingCart_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID);

-- Orders
ALTER TABLE Orders
    ADD CONSTRAINT FK_Orders_Users FOREIGN KEY (UserID) REFERENCES Users(UserID),
    CONSTRAINT FK_Orders_ShippingAddress FOREIGN KEY (ShippingAddressID) REFERENCES UserAddresses(AddressID),
    CONSTRAINT FK_Orders_BillingAddress FOREIGN KEY (BillingAddressID) REFERENCES UserAddresses(AddressID);

-- Order Details
ALTER TABLE OrderDetails
    ADD CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID);

-- Payment Methods
ALTER TABLE PaymentMethods
    ADD CONSTRAINT FK_PaymentMethods_Users FOREIGN KEY (UserID) REFERENCES Users(UserID);

-- Payments
ALTER TABLE Payments
    ADD CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    CONSTRAINT FK_Payments_PaymentMethods FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethods(PaymentMethodID);

-- Inventory Transactions
ALTER TABLE InventoryTransactions
    ADD CONSTRAINT FK_InventoryTransactions_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID),
    CONSTRAINT FK_InventoryTransactions_Users FOREIGN KEY (CreatedBy) REFERENCES Users(UserID);

-- =============================================
-- INDEXES
-- =============================================

-- Users
CREATE NONCLUSTERED INDEX IX_Users_Email ON Users(Email);
CREATE NONCLUSTERED INDEX IX_Users_Username ON Users(Username);
CREATE NONCLUSTERED INDEX IX_Users_LastLoginDate ON Users(LastLoginDate);

-- Products
CREATE NONCLUSTERED INDEX IX_Products_CategoryID ON Products(CategoryID);
CREATE NONCLUSTERED INDEX IX_Products_SupplierID ON Products(SupplierID);
CREATE NONCLUSTERED INDEX IX_Products_SKU ON Products(SKU);
CREATE NONCLUSTERED INDEX IX_Products_Price ON Products(UnitPrice);
CREATE NONCLUSTERED INDEX IX_Products_Stock ON Products(UnitsInStock);

-- Orders
CREATE NONCLUSTERED INDEX IX_Orders_UserID ON Orders(UserID);
CREATE NONCLUSTERED INDEX IX_Orders_OrderDate ON Orders(OrderDate);
CREATE NONCLUSTERED INDEX IX_Orders_Status ON Orders(OrderStatus);
CREATE NONCLUSTERED INDEX IX_Orders_TrackingNumber ON Orders(TrackingNumber);

-- Order Details
CREATE NONCLUSTERED INDEX IX_OrderDetails_OrderID ON OrderDetails(OrderID);
CREATE NONCLUSTERED INDEX IX_OrderDetails_ProductID ON OrderDetails(ProductID);

-- Shopping Cart
CREATE NONCLUSTERED INDEX IX_ShoppingCart_UserID ON ShoppingCart(UserID);
CREATE NONCLUSTERED INDEX IX_ShoppingCart_ProductID ON ShoppingCart(ProductID);

-- Product Reviews
CREATE NONCLUSTERED INDEX IX_ProductReviews_ProductID ON ProductReviews(ProductID);
CREATE NONCLUSTERED INDEX IX_ProductReviews_UserID ON ProductReviews(UserID);
CREATE NONCLUSTERED INDEX IX_ProductReviews_Rating ON ProductReviews(Rating);

-- Inventory Transactions
CREATE NONCLUSTERED INDEX IX_InventoryTransactions_ProductID ON InventoryTransactions(ProductID);
CREATE NONCLUSTERED INDEX IX_InventoryTransactions_Date ON InventoryTransactions(CreatedDate);

-- Audit Log
CREATE NONCLUSTERED INDEX IX_AuditLog_TableName ON AuditLog(TableName);
CREATE NONCLUSTERED INDEX IX_AuditLog_RecordID ON AuditLog(RecordID);
CREATE NONCLUSTERED INDEX IX_AuditLog_Date ON AuditLog(ChangeDate);

-- =============================================
-- TRIGGERS
-- =============================================
GO

-- Trigger to update ModifiedDate on Users table
CREATE TRIGGER TR_Users_UpdateModifiedDate
    ON Users
    AFTER UPDATE
              AS
BEGIN
    SET NOCOUNT ON;

UPDATE Users
SET ModifiedDate = GETDATE()
    FROM Users u
    INNER JOIN inserted i ON u.UserID = i.UserID;
END;
GO

-- Trigger to update ModifiedDate on Products table
CREATE TRIGGER TR_Products_UpdateModifiedDate
    ON Products
    AFTER UPDATE
              AS
BEGIN
    SET NOCOUNT ON;

UPDATE Products
SET ModifiedDate = GETDATE()
    FROM Products p
    INNER JOIN inserted i ON p.ProductID = i.ProductID;
END;
GO

-- Trigger to update inventory when order is placed
CREATE TRIGGER TR_OrderDetails_UpdateInventory
    ON OrderDetails
    AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update product stock
UPDATE Products
SET UnitsInStock = UnitsInStock - i.Quantity
    FROM Products p
    INNER JOIN inserted i ON p.ProductID = i.ProductID;

-- Log inventory transaction
INSERT INTO InventoryTransactions (ProductID, TransactionType, Quantity, UnitPrice, ReferenceID, CreatedBy)
SELECT
    i.ProductID,
    'Sale',
    -i.Quantity,
    i.UnitPrice,
    i.OrderID,
    o.UserID
FROM inserted i
         INNER JOIN Orders o ON i.OrderID = o.OrderID;
END;
GO

-- Audit trigger for Users table
CREATE TRIGGER TR_Users_Audit
    ON Users
    AFTER INSERT, UPDATE, DELETE
    AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Action NVARCHAR(10);
    
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
        SET @Action = 'UPDATE';
ELSE IF EXISTS (SELECT * FROM inserted)
        SET @Action = 'INSERT';
ELSE
        SET @Action = 'DELETE';

INSERT INTO AuditLog (TableName, RecordID, Action, OldValues, NewValues, ChangedBy)
SELECT
    'Users',
    ISNULL(i.UserID, d.UserID),
    @Action,
    CASE WHEN @Action IN ('UPDATE', 'DELETE') THEN
             (SELECT d.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
END,
        CASE WHEN @Action IN ('UPDATE', 'INSERT') THEN 
            (SELECT i.* FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
END,
        USER_ID()
    FROM inserted i
    FULL OUTER JOIN deleted d ON i.UserID = d.UserID;
END;
GO

-- Trigger to automatically calculate order totals
CREATE TRIGGER TR_Orders_CalculateTotals
    ON OrderDetails
    AFTER INSERT, UPDATE, DELETE
    AS
BEGIN
    SET NOCOUNT ON;

UPDATE Orders
SET SubTotal = (
    SELECT SUM(LineTotal)
    FROM OrderDetails
    WHERE OrderID = Orders.OrderID
),
    TotalAmount = SubTotal + TaxAmount + ShippingAmount
    FROM Orders
WHERE OrderID IN (
    SELECT DISTINCT OrderID FROM inserted
    UNION
    SELECT DISTINCT OrderID FROM deleted
    );
END;
GO

-- =============================================
-- STORED PROCEDURES
-- =============================================

-- Get User Profile
CREATE PROCEDURE sp_GetUserProfile
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;

SELECT
    u.UserID,
    u.Username,
    u.Email,
    u.FirstName,
    u.LastName,
    u.DateOfBirth,
    u.PhoneNumber,
    u.IsActive,
    u.CreatedDate,
    u.LastLoginDate
FROM Users u
WHERE u.UserID = @UserID AND u.IsActive = 1;

-- Get user addresses
SELECT
    AddressID,
    AddressType,
    AddressLine1,
    AddressLine2,
    City,
    State,
    ZipCode,
    Country,
    IsDefault
FROM UserAddresses
WHERE UserID = @UserID;
END;
GO

-- Create New Order
CREATE PROCEDURE sp_CreateOrder
    @UserID INT,
    @ShippingAddressID INT,
    @BillingAddressID INT,
    @TaxAmount DECIMAL(12,2) = 0,
    @ShippingAmount DECIMAL(12,2) = 0,
    @OrderID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY
        -- Create the order
INSERT INTO Orders (UserID, ShippingAddressID, BillingAddressID, SubTotal, TaxAmount, ShippingAmount, TotalAmount)
        VALUES (@UserID, @ShippingAddressID, @BillingAddressID, 0, @TaxAmount, @ShippingAmount, @TaxAmount + @ShippingAmount);
        
        SET @OrderID = SCOPE_IDENTITY();
        
        -- Move items from shopping cart to order details
INSERT INTO OrderDetails (OrderID, ProductID, Quantity, UnitPrice)
SELECT
    @OrderID,
    sc.ProductID,
    sc.Quantity,
    sc.UnitPrice
FROM ShoppingCart sc
WHERE sc.UserID = @UserID;

-- Clear shopping cart
DELETE FROM ShoppingCart WHERE UserID = @UserID;

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
ROLLBACK TRANSACTION;
        THROW;
END CATCH;
END;
GO

-- Add Product to Cart
CREATE PROCEDURE sp_AddToCart
    @UserID INT,
    @ProductID INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UnitPrice DECIMAL(10,2);
    
    -- Get product price
SELECT @UnitPrice = UnitPrice
FROM Products
WHERE ProductID = @ProductID AND Discontinued = 0;

IF @UnitPrice IS NULL
BEGIN
        RAISERROR('Product not found or discontinued', 16, 1);
        RETURN;
END;
    
    -- Check if item already in cart
    IF EXISTS (SELECT 1 FROM ShoppingCart WHERE UserID = @UserID AND ProductID = @ProductID)
BEGIN
UPDATE ShoppingCart
SET Quantity = Quantity + @Quantity,
    ModifiedDate = GETDATE()
WHERE UserID = @UserID AND ProductID = @ProductID;
END
ELSE
BEGIN
INSERT INTO ShoppingCart (UserID, ProductID, Quantity, UnitPrice)
VALUES (@UserID, @ProductID, @Quantity, @UnitPrice);
END;
END;
GO

-- Get Product Search Results
CREATE PROCEDURE sp_SearchProducts
    @SearchTerm NVARCHAR(255) = NULL,
    @CategoryID INT = NULL,
    @MinPrice DECIMAL(10,2) = NULL,
    @MaxPrice DECIMAL(10,2) = NULL,
    @SortBy NVARCHAR(20) = 'Name',
    @SortOrder NVARCHAR(4) = 'ASC',
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

WITH ProductSearch AS (
    SELECT
        p.ProductID,
        p.ProductName,
        p.Description,
        p.SKU,
        p.UnitPrice,
        p.UnitsInStock,
        p.ImageURL,
        c.CategoryName,
        s.SupplierName,
        COALESCE(AVG(CAST(pr.Rating AS FLOAT)), 0) AS AverageRating,
        COUNT(pr.ReviewID) AS ReviewCount
    FROM Products p
             INNER JOIN Categories c ON p.CategoryID = c.CategoryID
             INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
             LEFT JOIN ProductReviews pr ON p.ProductID = pr.ProductID AND pr.IsApproved = 1
    WHERE p.Discontinued = 0
      AND (@SearchTerm IS NULL OR p.ProductName LIKE '%' + @SearchTerm + '%' OR p.Description LIKE '%' + @SearchTerm + '%')
      AND (@CategoryID IS NULL OR p.CategoryID = @CategoryID)
      AND (@MinPrice IS NULL OR p.UnitPrice >= @MinPrice)
      AND (@MaxPrice IS NULL OR p.UnitPrice <= @MaxPrice)
    GROUP BY p.ProductID, p.ProductName, p.Description, p.SKU, p.UnitPrice, p.UnitsInStock, p.ImageURL, c.CategoryName, s.SupplierName
)
SELECT *
FROM ProductSearch
ORDER BY
    CASE WHEN @SortBy = 'Name' AND @SortOrder = 'ASC' THEN ProductName END ASC,
    CASE WHEN @SortBy = 'Name' AND @SortOrder = 'DESC' THEN ProductName END DESC,
    CASE WHEN @SortBy = 'Price' AND @SortOrder = 'ASC' THEN UnitPrice END ASC,
    CASE WHEN @SortBy = 'Price' AND @SortOrder = 'DESC' THEN UnitPrice END DESC,
    CASE WHEN @SortBy = 'Rating' AND @SortOrder = 'ASC' THEN AverageRating END ASC,
    CASE WHEN @SortBy = 'Rating' AND @SortOrder = 'DESC' THEN AverageRating END DESC
OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- Get Order Details
CREATE PROCEDURE sp_GetOrderDetails
    @OrderID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Order header
SELECT
    o.OrderID,
    o.OrderDate,
    o.OrderStatus,
    o.PaymentStatus,
    o.SubTotal,
    o.TaxAmount,
    o.ShippingAmount,
    o.TotalAmount,
    o.TrackingNumber,
    u.FirstName + ' ' + u.LastName AS CustomerName,
    u.Email AS CustomerEmail,
    sa.AddressLine1 AS ShippingAddress1,
    sa.AddressLine2 AS ShippingAddress2,
    sa.City AS ShippingCity,
    sa.State AS ShippingState,
    sa.ZipCode AS ShippingZip
FROM Orders o
         INNER JOIN Users u ON o.UserID = u.UserID
         INNER JOIN UserAddresses sa ON o.ShippingAddressID = sa.AddressID
WHERE o.OrderID = @OrderID;

-- Order line items
SELECT
    od.OrderDetailID,
    od.ProductID,
    p.ProductName,
    p.SKU,
    od.Quantity,
    od.UnitPrice,
    od.Discount,
    od.LineTotal
FROM OrderDetails od
         INNER JOIN Products p ON od.ProductID = p.ProductID
WHERE od.OrderID = @OrderID;
END;
GO

-- Update Product Stock
CREATE PROCEDURE sp_UpdateProductStock
    @ProductID INT,
    @Quantity INT,
    @TransactionType NVARCHAR(20),
    @UnitPrice DECIMAL(10,2) = NULL,
    @Notes NVARCHAR(500) = NULL,
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
BEGIN TRANSACTION;

BEGIN TRY
        -- Update product stock
UPDATE Products
SET UnitsInStock = UnitsInStock + @Quantity,
    ModifiedDate = GETDATE()
WHERE ProductID = @ProductID;

-- Log inventory transaction
INSERT INTO InventoryTransactions (ProductID, TransactionType, Quantity, UnitPrice, Notes, CreatedBy)
VALUES (@ProductID, @TransactionType, @Quantity, @UnitPrice, @Notes, @UserID);

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
ROLLBACK TRANSACTION;
        THROW;
END CATCH;
END;
GO

-- Get Sales Report
CREATE PROCEDURE sp_GetSalesReport
    @StartDate DATETIME2,
    @EndDate DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

SELECT
    p.ProductID,
    p.ProductName,
    p.SKU,
    c.CategoryName,
    SUM(od.Quantity) AS TotalQuantitySold,
    SUM(od.LineTotal) AS TotalRevenue,
    AVG(od.UnitPrice) AS AverageSellingPrice,
    COUNT(DISTINCT o.OrderID) AS NumberOfOrders
FROM OrderDetails od
         INNER JOIN Orders o ON od.OrderID = o.OrderID
         INNER JOIN Products p ON od.ProductID = p.ProductID
         INNER JOIN Categories c ON p.CategoryID = c.CategoryID
WHERE o.OrderDate BETWEEN @StartDate AND @EndDate
  AND o.OrderStatus IN ('Completed', 'Shipped')
GROUP BY p.ProductID, p.ProductName, p.SKU, c.CategoryName
ORDER BY TotalRevenue DESC;
END;
GO

-- =============================================
-- VIEWS
-- =============================================

-- Product Catalog View
CREATE VIEW vw_ProductCatalog
AS
SELECT
    p.ProductID,
    p.ProductName,
    p.Description,
    p.SKU,
    p.UnitPrice,
    p.UnitsInStock,
    p.ImageURL,
    c.CategoryName,
    s.SupplierName,
    COALESCE(AVG(CAST(pr.Rating AS FLOAT)), 0) AS AverageRating,
    COUNT(pr.ReviewID) AS ReviewCount
FROM Products p
         INNER JOIN Categories c ON p.CategoryID = c.CategoryID
         INNER JOIN Suppliers s ON p.SupplierID = s.SupplierID
         LEFT JOIN ProductReviews pr ON p.ProductID = pr.ProductID AND pr.IsApproved = 1
WHERE p.Discontinued = 0
GROUP BY p.ProductID, p.ProductName, p.Description, p.SKU, p.UnitPrice, p.UnitsInStock, p.ImageURL, c.CategoryName, s.SupplierName;
GO

-- Order Summary View
CREATE VIEW vw_OrderSummary
AS
SELECT
    o.OrderID,
    o.OrderDate,
    o.OrderStatus,
    o.PaymentStatus,
    u.FirstName + ' ' + u.LastName AS CustomerName,
    u.Email AS CustomerEmail,
    COUNT(od.OrderDetailID) AS ItemCount,
    o.TotalAmount,
    o.TrackingNumber
FROM Orders o
         INNER JOIN Users u ON o.UserID = u.UserID
         INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
GROUP BY o.OrderID, o.OrderDate, o.OrderStatus, o.PaymentStatus, u.FirstName, u.LastName, u.Email, o.TotalAmount, o.TrackingNumber;
GO
