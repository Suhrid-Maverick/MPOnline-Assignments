Assignment 01: Users Table + Various SELECT Queries

1. Create the Users table with CHECK constraint (Age > 18)
CREATE TABLE Users
(
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50)  NOT NULL,
    Email    NVARCHAR(100) NOT NULL,
    Age      INT           NOT NULL CHECK (Age > 18)
);

2. Sample data
INSERT INTO Users (Username, Email, Age) VALUES
('Alice',   'alice@example.com',   22),
('Bob',     'bob@example.com',     19),
('Charlie', 'charlie@example.com', 28),
('David',   'david@example.com',   35),
('Eve',     'eve@example.com',     24),
('Frank',   'frank@example.com',   45),
('Grace',   'grace@example.com',   20),
('Heidi',   'heidi@example.com',   31);

Query 1: Find users older than the average age
SELECT Id, Username, Email, Age
FROM Users
WHERE Age > (SELECT AVG(Age) FROM Users)
ORDER BY Age DESC;

Query 2: Find users whose age matches one of the ages greater than 25
SELECT Id, Username, Email, Age
FROM Users
WHERE Age IN (SELECT Age FROM Users WHERE Age > 25)
ORDER BY Age;

Query 3: Check whether there is at least one user older than 30; Returns 1 if true, 0 if false
SELECT CASE
           WHEN EXISTS (SELECT 1 FROM Users WHERE Age > 30)
           THEN 'Yes, at least one user is older than 30'
           ELSE 'No user is older than 30'
       END AS Result;

Query 4: Find users older than the average age of all OTHER users(i.e., exclude the current user from the average)
SELECT u1.Id, u1.Username, u1.Email, u1.Age
FROM Users u1
WHERE u1.Age > (
    SELECT AVG(u2.Age)
    FROM Users u2
    WHERE u2.Id <> u1.Id
)
ORDER BY u1.Age DESC;

Query 5: Show each user along with the overall average age
SELECT Id,
       Username,
       Email,
       Age,
       (SELECT AVG(Age) FROM Users) AS OverallAvgAge,
       Age - (SELECT AVG(Age) FROM Users) AS DiffFromAvg
FROM Users
ORDER BY Id;
