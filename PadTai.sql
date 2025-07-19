GO
create database PadTai
go
USE [PadTai]
go

CREATE TABLE Clients (
   ClientID INT PRIMARY KEY,                        
   ClientName NVARCHAR(50)
);
--ALTER TABLE Clients ADD Port INT;
SELECT  ClientID, ClientName FROM Clients

CREATE TABLE Employees (
    ID INT PRIMARY KEY,                        
    PersonalType NVARCHAR(50) NOT NULL,       
    Gender NVARCHAR(10) NOT NULL,             
    Contact NVARCHAR(15)  NOT NULL,            
    Name NVARCHAR(100) NOT NULL,              
    Password NVARCHAR(255),          
    DateOfStarting DATETIME NOT NULL,         
    Picture VARBINARY(MAX),                    
    ClientID INT NOT NULL,                     
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID) 
);

--DBCC CHECKIDENT ('Employeelogin', RESEED, 0)
CREATE TABLE Employeelogin(
    ID INT PRIMARY KEY IDENTITY(1,1),
    PersonalType NVARCHAR(50) NOT NULL,       
    UserName NVARCHAR(100) NOT NULL,
	Thedaydate DATE NOT NULL,         
    TimeOfStarting TIME NOT NULL,
	TimeOfStoping TIME,
	Contact NVARCHAR(15)  NOT NULL,            
    ClientID INT NOT NULL     	
);

CREATE TABLE Reservetable (
    ReservationID INT NOT NULL,                        
    Guestgender NVARCHAR(10) NOT NULL,             
    GuestContact NVARCHAR(15)  NOT NULL,            
    GuestName NVARCHAR(100) NOT NULL,  
	Guesttime  NVARCHAR(100)  NOT NULL,         
    Ordertime DATETIME NOT NULL,  
	Clientsqte NVARCHAR(100),
    Tablenumber NVARCHAR(100) NOT NULL,
	Cancelled NVARCHAR(50),
	Completed NVARCHAR(50),
    ClientID INT NOT NULL,                     
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
	PRIMARY KEY (ClientID, ReservationID)
);
ALTER TABLE Reservetable
ADD CONSTRAINT chk_Cancelled_Completed
CHECK (NOT (Cancelled IS NOT NULL AND Completed IS NOT NULL));

-- Create a filtered unique index
CREATE UNIQUE INDEX UQ_Tablenumber_Active
ON Reservetable (Tablenumber)
WHERE Cancelled IS NULL AND Completed IS NULL;


CREATE TABLE Tabletcount (
    ClientID INT PRIMARY KEY,
    LastReserveId INT NOT NULL
	);

--Login Manager
create Table Login(
	Username NVarchar(150) NOT NULL,
	Password NVarchar (20) NOT NULL,
);

-- Create FoodItems Table
CREATE TABLE FoodItems (
    FoodID INT PRIMARY KEY IDENTITY(1,1),
    FoodName NVARCHAR(100) NOT NULL,
	FooditemtypeID INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL
);
    ALTER Table FoodItems ADD IsChecked BIT NULL;
    ALTER Table FoodItems ADD GroupID INT NULL;
    ALTER Table FoodItems ADD SubgroupID INT NULL;
    ALTER Table FoodItems ADD SubsubgroupID INT NULL;
	ALTER Table FoodItems ADD FOREIGN KEY (GroupID) REFERENCES Groups(GroupID) ON DELETE SET NULL;
    ALTER Table FoodItems ADD FOREIGN KEY (SubgroupID) REFERENCES Subgroups(SubgroupID) ON DELETE SET NULL;
    ALTER Table FoodItems ADD FOREIGN KEY (SubsubgroupID) REFERENCES Subsubgroups(SubsubgroupID) ON DELETE SET NULL;

-- Create FoodTypes Table
CREATE TABLE FoodItemsTypes (
    FooditemtypeID INT PRIMARY KEY IDENTITY(1,1),
    FooditemtypeName NVARCHAR(50) NOT NULL
);

-- Create the Groups table
CREATE TABLE Groups (
    GroupID INT PRIMARY KEY IDENTITY(1,1),
    GroupName NVARCHAR(100) NOT NULL
);

-- Create the Subgroups table
CREATE TABLE Subgroups (
    SubgroupID INT PRIMARY KEY IDENTITY(1,1),
    SubgroupName NVARCHAR(100) NOT NULL,
    GroupID INT,
    FOREIGN KEY (GroupID) REFERENCES Groups(GroupID) ON DELETE CASCADE
);

-- Create the Subsubgroups table
CREATE TABLE Subsubgroups (
    SubsubgroupID INT PRIMARY KEY IDENTITY(1,1),
    SubsubgroupName NVARCHAR(100) NOT NULL,
    SubgroupID INT,
    FOREIGN KEY (SubgroupID) REFERENCES Subgroups(SubgroupID) ON DELETE CASCADE
);

-- Create the Dishes table
CREATE TABLE Dishes (
    DishID INT PRIMARY KEY IDENTITY(1,1),
    DishName NVARCHAR(100) NOT NULL,
    GroupID INT NULL,
    SubgroupID INT NULL,
    SubsubgroupID INT NULL,
    FOREIGN KEY (GroupID) REFERENCES Groups(GroupID) ON DELETE SET NULL,
    FOREIGN KEY (SubgroupID) REFERENCES Subgroups(SubgroupID) ON DELETE SET NULL,
    FOREIGN KEY (SubsubgroupID) REFERENCES Subsubgroups(SubsubgroupID) ON DELETE SET NULL
);

 CREATE TABLE PaymentGroups (
     PaymentgroupID INT PRIMARY KEY,
     PaymentGroupName NVARCHAR(20) NOT NULL
);

 CREATE TABLE PaymentTypes (
     PaymentypeID INT PRIMARY KEY,
     PaymenttypeName NVARCHAR(50) NOT NULL,
     PaymentgroupID INT,
     FOREIGN KEY (PaymentgroupID) REFERENCES PaymentGroups(PaymentgroupID) ON DELETE SET NULL
);

-- Create OrderTypes Table
CREATE TABLE OrderTypes (
    OrdertypeID INT PRIMARY KEY IDENTITY(1,1),
    OrdertypeName NVARCHAR(50) NOT NULL
);

CREATE TABLE PlacetoEat (
    PlacetoEatID INT PRIMARY KEY IDENTITY(1,1),
    PlacetoEatName NVARCHAR(50) NOT NULL
);

CREATE TABLE Discounts(
    DiscountID INT PRIMARY KEY IDENTITY(1,1),
	Thediscount NVARCHAR(100) NOT NULL
);

-- Create Receipts Table
CREATE TABLE Receipts (
    ReceiptId INT NOT NULL,
    FoodName NVARCHAR(100)NOT NULL,
	Foodprice NVARCHAR(100) NOT NULL,
	OrderDateTime DATETIME NOT NULL,
    PaymenttypeName NVARCHAR(50) NOT NULL,
	PlacetoEatName NVARCHAR(100) NOT NULL,
    Thediscount NVARCHAR(100) NOT NULL,
    TotalPrice DECIMAL(38, 2) NOT NULL,
	FooditemtypeID NVARCHAR NOT NULL,
	ClientID INT NOT NULL,                     
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
	PRIMARY KEY (ClientID, ReceiptId)

);
ALTER TABLE Receipts ALTER COLUMN [FoodName] NVARCHAR(MAX);
ALTER TABLE Receipts ALTER COLUMN [Foodprice] NVARCHAR(MAX);
ALTER TABLE Receipts ALTER COLUMN [FooditemtypeID] NVARCHAR(MAX);

CREATE TABLE Receiptcount (
    ClientID INT PRIMARY KEY,
    LastReceiptId INT NOT NULL
);


--DBCC CHECKIDENT ('Weborders', RESEED, 0)
CREATE TABLE Weborders (
	WebreceiptID INT NOT NULL,
    CustomerName NVARCHAR(100) NOT NULL,
    Contact NVARCHAR(50) NOT NULL,
    OrderTime DATETIME NOT NULL,
    PreparationTime DATETIME NOT NULL,
    DeliveryAddress NVARCHAR(255)NOT NULL,
    DeliveryType NVARCHAR(50)NOT NULL,
    Orderstatus NVARCHAR(50)NOT NULL,
	ClientID INT NOT NULL,                     
);

CREATE TABLE ReceiptsArchive (
    ReceiptId INT NOT NULL,
    FoodName NVARCHAR(100)NOT NULL,
	Foodprice NVARCHAR(100) NOT NULL,
	OrderDateTime DATETIME NOT NULL,
    PaymenttypeName NVARCHAR(50) NOT NULL,
	PlacetoEatName NVARCHAR(100) NOT NULL,
    Thediscount NVARCHAR(100) NOT NULL,
    TotalPrice DECIMAL(38, 2) NOT NULL,
	FooditemtypeID NVARCHAR NOT NULL,
	ClientID INT NOT NULL,                     
    FOREIGN KEY (ClientID) REFERENCES Clients(ClientID),
	PRIMARY KEY (ClientID, ReceiptId)

);
ALTER TABLE ReceiptsArchive ALTER COLUMN [FoodName] NVARCHAR(MAX);
ALTER TABLE ReceiptsArchive ALTER COLUMN [Foodprice] NVARCHAR(MAX);
ALTER TABLE ReceiptsArchive ALTER COLUMN [FooditemtypeID] NVARCHAR(MAX);

