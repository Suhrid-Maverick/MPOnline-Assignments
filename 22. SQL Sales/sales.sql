Assignment 22: Sales by Salesman & Category + Monthly Sales

1. Tables
CREATE TABLE Salesmen
(
    SalesmanId INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL
);

CREATE TABLE Categories
(
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL
);

CREATE TABLE Sales
(
    SaleId     INT IDENTITY(1,1) PRIMARY KEY,
    SalesmanId INT NOT NULL,
    CategoryId INT NOT NULL,
    Amount     DECIMAL(12,2) NOT NULL,
    SaleDate   DATE NOT NULL,
    FOREIGN KEY (SalesmanId) REFERENCES Salesmen(SalesmanId),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

2. Sample data
INSERT INTO Salesmen (Name) VALUES
('Ravi'), ('Sunita'), ('Arjun'), ('Priya');

INSERT INTO Categories (Name) VALUES
('Electronics'), ('Furniture'), ('Stationery'), ('Groceries');

INSERT INTO Sales (SalesmanId, CategoryId, Amount, SaleDate) VALUES
(1, 1, 55000.00, '2024-01-15'),
(1, 2, 12000.00, '2024-01-20'),
(1, 1, 8000.00,  '2024-02-05'),
(2, 3, 4500.00,  '2024-01-18'),
(2, 4, 9800.00,  '2024-02-10'),
(2, 4, 12500.00, '2024-03-01'),
(3, 1, 72000.00, '2024-01-22'),
(3, 2, 18000.00, '2024-02-28'),
(3, 3, 2200.00,  '2024-03-15'),
(4, 4, 6500.00,  '2024-01-29'),
(4, 1, 41000.00, '2024-02-12'),
(4, 2, 9800.00,  '2024-03-20');

Query 1: Sales by Salesman and Category
SELECT
    s.Name            AS Salesman,
    c.Name            AS Category,
    SUM(sl.Amount)    AS TotalSales,
    COUNT(sl.SaleId)  AS NumberOfSales
FROM Sales sl
INNER JOIN Salesmen  s ON s.SalesmanId = sl.SalesmanId
INNER JOIN Categories c ON c.CategoryId = sl.CategoryId
GROUP BY s.Name, c.Name
ORDER BY s.Name, c.Name;

Query 2: Total monthly sales grouped by salesman
SELECT
    s.Name                                            AS Salesman,
    YEAR(sl.SaleDate)                                 AS SaleYear,
    MONTH(sl.SaleDate)                                AS SaleMonth,
    DATENAME(MONTH, sl.SaleDate)                      AS MonthName,
    SUM(sl.Amount)                                    AS TotalSales,
    COUNT(sl.SaleId)                                  AS NumberOfSales
FROM Sales sl
INNER JOIN Salesmen s ON s.SalesmanId = sl.SalesmanId
GROUP BY s.Name, YEAR(sl.SaleDate), MONTH(sl.SaleDate), DATENAME(MONTH, sl.SaleDate)
ORDER BY s.Name, SaleYear, SaleMonth;
