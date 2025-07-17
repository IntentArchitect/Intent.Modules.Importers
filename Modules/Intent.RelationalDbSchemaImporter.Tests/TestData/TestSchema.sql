-- Test Schema for Integration Tests
-- Creates a simple database schema with tables, views, indexes, and stored procedures

-- Create a custom schema
CREATE SCHEMA TestSchema;
GO

-- Create main tables
CREATE TABLE TestSchema.Users (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserName nvarchar(255) NOT NULL,
    Email nvarchar(320) NOT NULL,
    FirstName nvarchar(100) NULL,
    LastName nvarchar(100) NULL,
    DateCreated datetime2 NOT NULL DEFAULT GETDATE(),
    IsActive bit NOT NULL DEFAULT 1,
    Balance decimal(18,2) NULL
);

CREATE TABLE TestSchema.Orders (
    Id int IDENTITY(1,1) PRIMARY KEY,
    UserId int NOT NULL,
    OrderNumber nvarchar(50) NOT NULL,
    OrderDate datetime2 NOT NULL DEFAULT GETDATE(),
    TotalAmount decimal(18,2) NOT NULL,
    Status nvarchar(20) NOT NULL DEFAULT 'Pending',
    CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES TestSchema.Users(Id)
);

CREATE TABLE TestSchema.OrderItems (
    Id int IDENTITY(1,1) PRIMARY KEY,
    OrderId int NOT NULL,
    ProductName nvarchar(200) NOT NULL,
    Quantity int NOT NULL,
    UnitPrice decimal(18,2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES TestSchema.Orders(Id)
);

-- Create indexes
CREATE UNIQUE INDEX IX_Users_UserName ON TestSchema.Users (UserName);
CREATE INDEX IX_Users_Email ON TestSchema.Users (Email);
CREATE INDEX IX_Orders_OrderDate ON TestSchema.Orders (OrderDate);
CREATE INDEX IX_Orders_Status ON TestSchema.Orders (Status);
GO

-- Create a simple stored procedure
CREATE PROCEDURE TestSchema.GetUserOrders
    @UserId int,
    @Status nvarchar(20) = NULL
AS
BEGIN
    SELECT 
        o.Id,
        o.OrderNumber,
        o.OrderDate,
        o.TotalAmount,
        o.Status
    FROM TestSchema.Orders o
    WHERE o.UserId = @UserId
    AND (@Status IS NULL OR o.Status = @Status)
    ORDER BY o.OrderDate DESC;
END;
GO

-- Create a stored procedure with output parameter
CREATE PROCEDURE TestSchema.GetUserOrderCount
    @UserId int,
    @OrderCount int OUTPUT
AS
BEGIN
    SELECT @OrderCount = COUNT(*)
    FROM TestSchema.Orders
    WHERE UserId = @UserId;
END;
GO

-- Insert some test data
INSERT INTO TestSchema.Users (UserName, Email, FirstName, LastName, Balance)
VALUES 
    ('john.doe', 'john.doe@example.com', 'John', 'Doe', 1500.00),
    ('jane.smith', 'jane.smith@example.com', 'Jane', 'Smith', 2300.50),
    ('bob.wilson', 'bob.wilson@example.com', 'Bob', 'Wilson', 750.25);

INSERT INTO TestSchema.Orders (UserId, OrderNumber, TotalAmount, Status)
VALUES 
    (1, 'ORD-001', 299.99, 'Completed'),
    (1, 'ORD-002', 149.50, 'Pending'),
    (2, 'ORD-003', 599.00, 'Completed'),
    (3, 'ORD-004', 89.95, 'Cancelled');

INSERT INTO TestSchema.OrderItems (OrderId, ProductName, Quantity, UnitPrice)
VALUES 
    (1, 'Laptop Bag', 1, 299.99),
    (2, 'USB Cable', 3, 49.83),
    (3, 'Wireless Mouse', 2, 299.50),
    (4, 'Keyboard', 1, 89.95); 