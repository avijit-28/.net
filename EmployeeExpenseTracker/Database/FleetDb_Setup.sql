CREATE DATABASE FleetDb;

USE FleetDb;

--Primary Table: Employees
CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Designation NVARCHAR(100) NOT NULL,
    DOB DATE NOT NULL,
    DOJ DATE NOT NULL,
    Salary DECIMAL(18, 2) NOT NULL,
    Address NVARCHAR(250) DEFAULT 'N/A'
);

--  Demo Data for Employees (Assignment Data)
INSERT INTO Employees (Name, Designation, DOB, DOJ, Salary, Address) VALUES
('Ranjit Das', 'General Employee', '1988-05-20', '2015-01-07', 15000.00, 'N/A'),
('Sagar Das', 'Manager', '1989-06-04', '2016-01-18', 40000.00, 'N/A'),
('Samer Sen', 'Project Manager', '1984-04-19', '2015-04-13', 35000.00, 'N/A'),
('Sandeep Gupta', 'Team Leader', '1981-04-18', '2016-01-20', 28000.00, 'N/A'),
('Sujit Sen', 'General Employee', '1987-06-04', '2015-04-07', 12000.00, 'N/A'),
('Suresh Das', 'Project Manager', '1985-06-04', '2016-01-07', 30000.00, 'N/A');

-- Child Table: Expenses (Linked via Foreign Key to Employees)
CREATE TABLE Expenses (
    ExpenseId INT IDENTITY(1,1) PRIMARY KEY,
    EmployeeId INT NOT NULL,
    EmployeeName NVARCHAR(150) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    ExpenseDate DATETIME2 NOT NULL,
    PaymentMethod NVARCHAR(50) NOT NULL,
    HasReceipt BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Expenses_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
);

