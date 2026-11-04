-- =============================================================
-- CSDL của đồ án CaroGame (SQL Server)
-- Sinh tự động từ EF Core Migration (InitialCreate, AddMatchHistory) bằng lệnh:
--   dotnet ef migrations script --idempotent --project CaroGame.DAL --startup-project CaroGame.Server
-- Cách dùng: mở file trong SQL Server Management Studio (SSMS), kết nối tới
-- (localdb)\MSSQLLocalDB hoặc .\SQLEXPRESS rồi bấm Execute (F5).
-- Script chạy lại nhiều lần không lỗi (bảng đã có thì bỏ qua).
-- Muốn có sẵn tài khoản demo và lịch sử đấu: chạy tiếp file CaroGame_SampleData.sql.
-- Cách khác (không cần file này): dotnet ef database update --project CaroGame.DAL --startup-project CaroGame.Server
-- =============================================================

IF DB_ID(N'CaroGame') IS NULL
    CREATE DATABASE [CaroGame];
GO

USE [CaroGame];
GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017062007_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(20) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [IsEmailVerified] bit NOT NULL,
        [Score] int NOT NULL,
        [DefaultAvatarIndex] int NOT NULL,
        [AvatarPath] nvarchar(260) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017062007_InitialCreate'
)
BEGIN
    CREATE TABLE [VerificationCodes] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Code] nvarchar(6) NOT NULL,
        [Purpose] nvarchar(20) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [FailedAttempts] int NOT NULL,
        CONSTRAINT [PK_VerificationCodes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_VerificationCodes_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017062007_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017062007_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017062007_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_VerificationCodes_UserId] ON [VerificationCodes] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017062007_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261017062007_InitialCreate', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017063753_AddMatchHistory'
)
BEGIN
    CREATE TABLE [MatchHistories] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [OpponentId] int NULL,
        [Mode] nvarchar(20) NOT NULL,
        [Result] nvarchar(10) NOT NULL,
        [EndReason] nvarchar(20) NOT NULL,
        [BlockBothEndsRule] bit NOT NULL,
        [MoveCount] int NOT NULL,
        [ScoreChange] int NULL,
        [PlayedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_MatchHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MatchHistories_Users_OpponentId] FOREIGN KEY ([OpponentId]) REFERENCES [Users] ([Id]),
        CONSTRAINT [FK_MatchHistories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017063753_AddMatchHistory'
)
BEGIN
    CREATE INDEX [IX_MatchHistories_OpponentId] ON [MatchHistories] ([OpponentId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017063753_AddMatchHistory'
)
BEGIN
    CREATE INDEX [IX_MatchHistories_UserId] ON [MatchHistories] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017063753_AddMatchHistory'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261017063753_AddMatchHistory', N'10.0.13');
END;

COMMIT;
GO

