using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardingHouse.Infrastructure.Persistence.Migrations;

/// <summary>
/// Establishes a migration baseline for the existing database and adds RefreshTokens.
/// All other tables already exist and are represented by the model snapshot.
/// </summary>
public partial class BaselineExistingSchemaAndRefreshTokens : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
                THROW 50001, 'The existing Users table is required before applying this migration.', 1;

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
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // RefreshTokens might have existed before this baseline was applied.
        // Leave the table and its data intact if the migration is rolled back.
    }
}
