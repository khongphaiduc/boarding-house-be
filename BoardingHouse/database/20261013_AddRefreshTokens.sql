-- Run once against the existing BoardingHouse database.
-- TokenHash stores the SHA-256 hash of a random refresh token as 64 hexadecimal characters.
SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        Id uniqueidentifier NOT NULL
            CONSTRAINT DF_RefreshTokens_Id DEFAULT (newsequentialid()),
        UserId uniqueidentifier NOT NULL,
        TokenHash varchar(64) NOT NULL,
        CreatedAt datetime2(0) NOT NULL
            CONSTRAINT DF_RefreshTokens_CreatedAt DEFAULT (sysutcdatetime()),
        ExpiresAt datetime2(0) NOT NULL,
        RevokedAt datetime2(0) NULL,
        CONSTRAINT PK_RefreshTokens PRIMARY KEY (Id),
        CONSTRAINT FK_RefreshTokens_Users FOREIGN KEY (UserId)
            REFERENCES dbo.Users (Id) ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UQ_RefreshTokens_TokenHash
        ON dbo.RefreshTokens (TokenHash);

    CREATE INDEX IX_RefreshTokens_User_ExpiresAt
        ON dbo.RefreshTokens (UserId, ExpiresAt);
END;

COMMIT TRANSACTION;
