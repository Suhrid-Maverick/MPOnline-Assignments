Assignment 23: Shipment Tracking Queries

1. Tables
CREATE TABLE Vehicles
(
    VehicleId    INT IDENTITY(1,1) PRIMARY KEY,
    VehicleNo    NVARCHAR(20)  NOT NULL UNIQUE,
    VehicleType  NVARCHAR(30)  NOT NULL  -- Truck / Van / Bike
);

CREATE TABLE Shipments
(
    ShipmentId      INT IDENTITY(1,1) PRIMARY KEY,
    ConsignmentNo   NVARCHAR(30) NOT NULL UNIQUE,
    Origin          NVARCHAR(100) NOT NULL,
    Destination     NVARCHAR(100) NOT NULL,
    VehicleId       INT NOT NULL,
    DispatchDate    DATETIME NOT NULL,
    DeliveryDate    DATETIME NULL,           -- NULL means still in transit
    Status          NVARCHAR(20)  NOT NULL,  -- InTransit / Delivered / Cancelled
    FOREIGN KEY (VehicleId) REFERENCES Vehicles(VehicleId)
);

2. Sample data
INSERT INTO Vehicles (VehicleNo, VehicleType) VALUES
('TR-101', 'Truck'),
('VN-202', 'Van'),
('BK-303', 'Bike'),
('TR-404', 'Truck');

INSERT INTO Shipments (ConsignmentNo, Origin, Destination, VehicleId, DispatchDate, DeliveryDate, Status) VALUES
('CON-001', 'Delhi',    'Mumbai',   1, '2024-06-01 09:00', '2024-06-03 18:00', 'Delivered'),
('CON-002', 'Pune',     'Nashik',   2, '2024-06-02 10:30', '2024-06-02 17:00', 'Delivered'),
('CON-003', 'Chennai',  'Bangalore',3, '2024-06-28 08:00', NULL,                'InTransit'),
('CON-004', 'Kolkata',  'Ranchi',   1, '2024-06-25 14:00', '2024-06-27 09:00', 'Delivered'),
('CON-005', 'Delhi',    'Jaipur',   4, '2024-06-29 07:00', NULL,                'InTransit'),
('CON-006', 'Mumbai',   'Goa',      2, '2024-06-29 06:00', '2024-06-29 19:00', 'Delivered');

Query 1: Current Status of Each Shipment
SELECT
    sh.ConsignmentNo,
    sh.Origin,
    sh.Destination,
    v.VehicleNo,
    v.VehicleType,
    sh.DispatchDate,
    sh.DeliveryDate,
    sh.Status,
    CASE
        WHEN sh.DeliveryDate IS NULL
            THEN DATEDIFF(HOUR, sh.DispatchDate, GETDATE()) / 24.0
        ELSE DATEDIFF(HOUR, sh.DispatchDate, sh.DeliveryDate) / 24.0
    END AS TransitDaysSoFar
FROM Shipments sh
INNER JOIN Vehicles v ON v.VehicleId = sh.VehicleId
ORDER BY sh.DispatchDate DESC;

Query 2: Shipments Delivered Today (or a given day)
Using today as the reference day; replace CAST(GETDATE() AS DATE) with '2024-06-29' for any specific day.
SELECT
    sh.ConsignmentNo,
    sh.Origin,
    sh.Destination,
    v.VehicleNo,
    sh.DispatchDate,
    sh.DeliveryDate
FROM Shipments sh
INNER JOIN Vehicles v ON v.VehicleId = sh.VehicleId
WHERE CAST(sh.DeliveryDate AS DATE) = CAST(GETDATE() AS DATE)
ORDER BY sh.DeliveryDate DESC;

For a SPECIFIC given day, e.g. 2024-06-29:
WHERE CAST(sh.DeliveryDate AS DATE) = '2024-06-29';

Query 3: Average Transit Time per Vehicle
SELECT
    v.VehicleId,
    v.VehicleNo,
    v.VehicleType,
    COUNT(sh.ShipmentId)                                  AS DeliveredCount,
    AVG(DATEDIFF(HOUR, sh.DispatchDate, sh.DeliveryDate)
        / 24.0)                                           AS AvgTransitDays
FROM Shipments sh
INNER JOIN Vehicles v ON v.VehicleId = sh.VehicleId
WHERE sh.Status = 'Delivered' AND sh.DeliveryDate IS NOT NULL
GROUP BY v.VehicleId, v.VehicleNo, v.VehicleType
ORDER BY AvgTransitDays DESC;

Query 4: Shipments Still In Transit
SELECT
    sh.ConsignmentNo,
    sh.Origin,
    sh.Destination,
    v.VehicleNo,
    sh.DispatchDate,
    DATEDIFF(HOUR, sh.DispatchDate, GETDATE()) / 24.0  AS DaysInTransitSoFar
FROM Shipments sh
INNER JOIN Vehicles v ON v.VehicleId = sh.VehicleId
WHERE sh.Status = 'InTransit' AND sh.DeliveryDate IS NULL
ORDER BY sh.DispatchDate;
