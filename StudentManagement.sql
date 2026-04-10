-- Student Management System Database Creation Script
-- Use this script to create the database and required tables manually.

CREATE DATABASE StudentManagementDb;
GO

USE StudentManagementDb;
GO

-- 1. Users Table (for Authentication)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User',
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO

-- 2. Students Table
CREATE TABLE Students (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Age INT NOT NULL,
    Course NVARCHAR(100) NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL
);
GO

-- 3. RefreshTokens Table
CREATE TABLE RefreshTokens (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Token NVARCHAR(MAX) NOT NULL,
    Expires DATETIME2 NOT NULL,
    IsRevoked BIT NOT NULL DEFAULT 0,
    Created DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UserId INT NOT NULL,
    CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
GO

-- Initial Admin User (Password: AdminPassword123 - Hashed version would vary, this is for demonstration)
-- Note: Use the /api/Auth/register endpoint to create users with properly hashed passwords.
INSERT INTO Users (FullName, Email, PasswordHash, Role) 
VALUES ('System Administrator', 'admin@test.com', 'AQAAAAIAAYagAAAAENnS5...', 'Admin');
GO
