Assignment 13: Customer/Product/Order Joins

1. Create tables
CREATE TABLE Customers
(
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL,
    City       NVARCHAR(50)
);

CREATE TABLE Products
(
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    Name      NVARCHAR(100) NOT NULL,
    Price     DECIMAL(10,2) NOT NULL
);

CREATE TABLE Orders
(
    OrderId    INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NULL,           -- intentionally nullable to demo missing customer
    ProductId  INT NOT NULL,
    OrderDate  DATETIME DEFAULT GETDATE(),
    Quantity   INT NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    -- Note: CustomerId intentionally NOT constrained to demonstrate the
    -- "order references a customer that doesn't exist" case via outer join.
);

2. Sample data
INSERT INTO Customers (Name, City) VALUES
('Alice',   'Delhi'),
('Bob',     'Mumbai'),
('Charlie', 'Bangalore');
-- David has NO orders

INSERT INTO Products (Name, Price) VALUES
('Laptop',   55000.00),
('Mouse',       500.00),
('Keyboard',   1500.00),
('Monitor',   9000.00);

INSERT INTO Orders (CustomerId, ProductId, Quantity) VALUES
(1, 1, 1),    -- Alice buys Laptop
(2, 2, 2),    -- Bob buys 2 Mice
(1, 4, 1),    -- Alice buys Monitor
(3, 3, 1),    -- Charlie buys Keyboard
(NULL, 2, 1), -- unknown customer (no customer row) -> demonstrates missing ref
(2, 1, 1);    -- Bob buys Laptop

Query 1: Retrieve customer, product, and order details together (INNER JOIN)
SELECT
    o.OrderId,
    c.Name        AS Customer,
    p.Name        AS Product,
    p.Price,
    o.Quantity,
    o.OrderDate
FROM Orders o
INNER JOIN Customers c ON c.CustomerId = o.CustomerId
INNER JOIN Products  p ON p.ProductId  = o.ProductId
ORDER BY o.OrderId;

Query 2: Show ALL customers, even those who haven't placed any orders (LEFT OUTER JOIN from Customers)
SELECT
    c.CustomerId,
    c.Name        AS Customer,
    c.City,
    o.OrderId,
    p.Name        AS Product,
    o.Quantity
FROM Customers c
LEFT JOIN Orders   o ON o.CustomerId = c.CustomerId
LEFT JOIN Products p ON p.ProductId  = o.ProductId
ORDER BY c.CustomerId, o.OrderId;

Query 3: Show ALL orders, including those that reference a customer that does not exist (RIGHT OUTER JOIN from Customers) (or equivalently LEFT JOIN from Orders -> Customers)
SELECT
    o.OrderId,
    o.CustomerId,
    COALESCE(c.Name, '<NO CUSTOMER>') AS Customer,
    p.Name                            AS Product,
    o.Quantity,
    o.OrderDate
FROM Orders o
LEFT JOIN Customers c ON c.CustomerId = o.CustomerId
LEFT JOIN Products  p ON p.ProductId  = o.ProductId
ORDER BY o.OrderId;
