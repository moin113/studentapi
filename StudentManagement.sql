-- ============================================
-- Student Management System — Database Setup
-- Run this once on any machine to set up DB
-- Default Admin: admin@test.com / AdminPassword123
-- ============================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StudentManagementDB')
BEGIN
    CREATE DATABASE StudentManagementDB;
END
GO

USE StudentManagementDB;
GO

-- =============================================
-- Table: tblStudents
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tblStudents')
BEGIN
    CREATE TABLE tblStudents (
        Id          INT IDENTITY(1,1)   NOT NULL,
        Name        NVARCHAR(100)       NOT NULL,
        Email       NVARCHAR(150)       NOT NULL,
        Age         INT                 NOT NULL,
        Course      NVARCHAR(100)       NOT NULL,
        CreatedDate DATETIME2(7)        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedDate DATETIME2(7)        NULL,
        IsDeleted   BIT                 NOT NULL DEFAULT 0,
        CONSTRAINT PK_tblStudents PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- =============================================
-- Table: tblUsers
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tblUsers')
BEGIN
    CREATE TABLE tblUsers (
        Id           INT IDENTITY(1,1)  NOT NULL,
        FullName     NVARCHAR(100)      NOT NULL,
        Email        NVARCHAR(150)      NOT NULL,
        PasswordHash NVARCHAR(500)      NOT NULL,
        Role         NVARCHAR(50)       NOT NULL DEFAULT 'User',
        CreatedDate  DATETIME2(7)       NOT NULL DEFAULT GETUTCDATE(),
        IsActive     BIT                NOT NULL DEFAULT 1,
        CONSTRAINT PK_tblUsers PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- =============================================
-- Table: tblRefreshTokens
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tblRefreshTokens')
BEGIN
    CREATE TABLE tblRefreshTokens (
        Id        INT IDENTITY(1,1)  NOT NULL,
        UserId    INT                NOT NULL,
        Token     NVARCHAR(500)      NOT NULL,
        ExpiresAt DATETIME2(7)       NOT NULL,
        CreatedAt DATETIME2(7)       NOT NULL DEFAULT GETUTCDATE(),
        RevokedAt DATETIME2(7)       NULL,
        IsRevoked BIT                NOT NULL DEFAULT 0,
        CONSTRAINT PK_tblRefreshTokens PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_tblRefreshTokens_tblUsers
            FOREIGN KEY (UserId) REFERENCES tblUsers(Id)
            ON DELETE CASCADE
    );
END
GO

-- =============================================
-- Unique Constraints
-- =============================================
IF NOT EXISTS (
    SELECT * FROM sys.indexes
    WHERE name = 'UQ_tblStudents_Email'
    AND object_id = OBJECT_ID('tblStudents')
)
BEGIN
    ALTER TABLE tblStudents
    ADD CONSTRAINT UQ_tblStudents_Email UNIQUE (Email);
END
GO

IF NOT EXISTS (
    SELECT * FROM sys.indexes
    WHERE name = 'UQ_tblUsers_Email'
    AND object_id = OBJECT_ID('tblUsers')
)
BEGIN
    ALTER TABLE tblUsers
    ADD CONSTRAINT UQ_tblUsers_Email UNIQUE (Email);
END
GO

-- =============================================
-- Performance Indexes
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblStudents_IsDeleted'
    AND object_id = OBJECT_ID('tblStudents'))
BEGIN
    CREATE INDEX IX_tblStudents_IsDeleted
        ON tblStudents (IsDeleted ASC)
        INCLUDE (Id, Name, Email, Age, Course, CreatedDate);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblStudents_CreatedDate'
    AND object_id = OBJECT_ID('tblStudents'))
BEGIN
    CREATE INDEX IX_tblStudents_CreatedDate
        ON tblStudents (CreatedDate DESC)
        WHERE IsDeleted = 0;
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblRefreshTokens_UserId'
    AND object_id = OBJECT_ID('tblRefreshTokens'))
BEGIN
    CREATE INDEX IX_tblRefreshTokens_UserId
        ON tblRefreshTokens (UserId ASC)
        INCLUDE (Token, ExpiresAt, IsRevoked);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblRefreshTokens_Token'
    AND object_id = OBJECT_ID('tblRefreshTokens'))
BEGIN
    CREATE INDEX IX_tblRefreshTokens_Token
        ON tblRefreshTokens (Token ASC)
        INCLUDE (UserId, ExpiresAt, IsRevoked, RevokedAt);
END
GO

-- =============================================
-- Seed Admin User
-- Email:    admin@test.com
-- Password: AdminPassword123
-- =============================================
IF NOT EXISTS (SELECT 1 FROM tblUsers WHERE Email = 'admin@test.com')
BEGIN
    INSERT INTO tblUsers (FullName, Email, PasswordHash, Role, CreatedDate, IsActive)
    VALUES (
        'System Administrator',
        'admin@test.com',
        '$2a$11$XyjiiNdDmIsBFh/1vJB2U.EjvqSe7n8Twjnq77PHta5s7Jkrb6IqK',
        'Admin',
        GETUTCDATE(),
        1
    );
END
GO

-- =============================================
-- Verify Setup
-- =============================================
SELECT t.name AS TableName, p.rows AS [RowCount]
FROM sys.tables t
JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0,1)
ORDER BY t.name;

SELECT t.name AS TableName, i.name AS IndexName, i.type_desc
FROM sys.indexes i
JOIN sys.tables t ON i.object_id = t.object_id
WHERE i.name IS NOT NULL
ORDER BY t.name, i.name;

SELECT Id, FullName, Email, Role, IsActive FROM tblUsers;
GO
