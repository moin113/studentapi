-- Student Management System Database Creation Script
-- Use this script to create the database and required tables manually.

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StudentManagementDB')
BEGIN
    CREATE DATABASE StudentManagementDB;
END
GO

USE StudentManagementDB;
GO


-- -- =============================================
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
        CreatedDate DATETIME2(7)        NOT NULL  DEFAULT GETUTCDATE(),
        UpdatedDate DATETIME2(7)        NULL,
        IsDeleted   BIT                 NOT NULL  DEFAULT 0,   -- soft delete

        CONSTRAINT PK_tblStudents PRIMARY KEY CLUSTERED (Id ASC)
    );
END
GO

-- =============================================
-- Table: tblUsers  (for JWT Auth)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tblUsers')
BEGIN
    CREATE TABLE tblUsers (
        Id           INT IDENTITY(1,1)  NOT NULL,
        FullName     NVARCHAR(100)      NOT NULL,
        Email        NVARCHAR(150)      NOT NULL,
        PasswordHash NVARCHAR(500)      NOT NULL,   -- BCrypt hash
        Role         NVARCHAR(50)       NOT NULL  DEFAULT 'User',  -- Admin / User
        CreatedDate  DATETIME2(7)       NOT NULL  DEFAULT GETUTCDATE(),
        IsActive     BIT                NOT NULL  DEFAULT 1,

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
        Id          INT IDENTITY(1,1)   NOT NULL,
        UserId      INT                 NOT NULL,
        Token       NVARCHAR(500)       NOT NULL,
        ExpiresAt   DATETIME2(7)        NOT NULL,
        CreatedAt   DATETIME2(7)        NOT NULL  DEFAULT GETUTCDATE(),
        RevokedAt   DATETIME2(7)        NULL,       -- NULL = still active
        IsRevoked   BIT                 NOT NULL  DEFAULT 0,

        CONSTRAINT PK_tblRefreshTokens PRIMARY KEY CLUSTERED (Id ASC),
        CONSTRAINT FK_tblRefreshTokens_tblUsers
            FOREIGN KEY (UserId) REFERENCES tblUsers(Id)
            ON DELETE CASCADE
    );
END
GO

-- Unique email per student
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

-- Unique email per user
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


-- Fast lookup by IsDeleted (used in every Get query)
CREATE INDEX IX_tblStudents_IsDeleted
    ON tblStudents (IsDeleted ASC)
    INCLUDE (Id, Name, Email, Age, Course, CreatedDate);
GO

-- Fast pagination by CreatedDate DESC
CREATE INDEX IX_tblStudents_CreatedDate
    ON tblStudents (CreatedDate DESC)
    WHERE IsDeleted = 0;
GO

-- Fast refresh token lookup by UserId
CREATE INDEX IX_tblRefreshTokens_UserId
    ON tblRefreshTokens (UserId ASC)
    INCLUDE (Token, ExpiresAt, IsRevoked);
GO

-- Fast token validation lookup
CREATE INDEX IX_tblRefreshTokens_Token
    ON tblRefreshTokens (Token ASC)
    INCLUDE (UserId, ExpiresAt, IsRevoked, RevokedAt);
GO


-- Password is: Admin@123  (BCrypt hashed)
IF NOT EXISTS (SELECT 1 FROM tblUsers WHERE Email = 'admin@studentmgmt.com')
BEGIN
    INSERT INTO tblUsers (FullName, Email, PasswordHash, Role, CreatedDate, IsActive)
    VALUES (
        'System Admin',
        'admin@studentmgmt.com',
        '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy',
        'Admin',
        GETUTCDATE(),
        1
    );
END
GO



-- Initial Admin User (Password: AdminPassword123 - Hashed version would vary, this is for demonstration)
-- Note: Use the /api/Auth/register endpoint to create users with properly hashed passwords.
INSERT INTO Users (FullName, Email, PasswordHash, Role) 
VALUES ('System Administrator', 'admin@test.com', 'AQAAAAIAAYagAAAAENnS5...', 'Admin');
GO
