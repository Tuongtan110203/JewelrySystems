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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Categories] (
        [CategoryId] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([CategoryId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Customers] (
        [CustomerId] int NOT NULL IDENTITY,
        [CustomerName] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [Status] nvarchar(max) NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [GoldTypes] (
        [GoldId] int NOT NULL IDENTITY,
        [GoldName] nvarchar(max) NOT NULL,
        [BuyPrice] float NOT NULL,
        [SellPrice] float NOT NULL,
        [UpdateTime] datetime2 NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_GoldTypes] PRIMARY KEY ([GoldId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Roles] (
        [RoleId] int NOT NULL IDENTITY,
        [RoleName] nvarchar(max) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Products] (
        [ProductId] int NOT NULL IDENTITY,
        [CategoryId] int NOT NULL,
        [GoldId] int NOT NULL,
        [ProductCode] nvarchar(max) NOT NULL,
        [ProductName] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Image] nvarchar(max) NULL,
        [Quantity] int NOT NULL,
        [GoldWeight] float NOT NULL,
        [Wage] float NOT NULL,
        [Price] float NOT NULL,
        [Size] nvarchar(max) NOT NULL,
        [WarrantyPeriod] int NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId]),
        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([CategoryId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Products_GoldTypes_GoldId] FOREIGN KEY ([GoldId]) REFERENCES [GoldTypes] ([GoldId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Users] (
        [UserName] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NOT NULL,
        [Email] nvarchar(max) NOT NULL,
        [Dob] datetime2 NOT NULL,
        [Level] int NULL,
        [Status] nvarchar(max) NOT NULL,
        [RoleId] int NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([UserName]),
        CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([RoleId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Stones] (
        [StoneId] int NOT NULL IDENTITY,
        [ProductId] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [Price] float NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [IsPrimary] bit NOT NULL,
        [Color] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Stones] PRIMARY KEY ([StoneId]),
        CONSTRAINT [FK_Stones_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Orders] (
        [OrderId] int NOT NULL IDENTITY,
        [OrderCode] nvarchar(max) NOT NULL,
        [UserName] nvarchar(450) NOT NULL,
        [CustomerId] int NULL,
        [OrderDate] datetime2 NOT NULL,
        [Total] float NOT NULL,
        [SaleById] nvarchar(max) NULL,
        [CashierId] nvarchar(max) NULL,
        [ServicerId] nvarchar(max) NULL,
        [CustomerName] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
        CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]),
        CONSTRAINT [FK_Orders_Users_UserName] FOREIGN KEY ([UserName]) REFERENCES [Users] ([UserName]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [OrderDetails] (
        [OrderDetailId] int NOT NULL IDENTITY,
        [Price] float NOT NULL,
        [Quantity] int NOT NULL,
        [OrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        CONSTRAINT [PK_OrderDetails] PRIMARY KEY ([OrderDetailId]),
        CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderDetails_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Payments] (
        [PaymentId] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [PaymentType] nvarchar(max) NULL,
        [Cash] float NULL,
        [BankTransfer] float NULL,
        [TransactionId] nvarchar(max) NULL,
        [PaymentTime] datetime2 NOT NULL,
        [Image] nvarchar(max) NULL,
        [Status] nvarchar(max) NULL,
        CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentId]),
        CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE TABLE [Warranties] (
        [WarrantyId] int NOT NULL IDENTITY,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [OrderDetailId] int NOT NULL,
        [CustomerId] int NULL,
        [Status] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Warranties] PRIMARY KEY ([WarrantyId]),
        CONSTRAINT [FK_Warranties_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Warranties_OrderDetails_OrderDetailId] FOREIGN KEY ([OrderDetailId]) REFERENCES [OrderDetails] ([OrderDetailId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_OrderDetails_OrderId] ON [OrderDetails] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_OrderDetails_ProductId] ON [OrderDetails] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Orders_UserName] ON [Orders] ([UserName]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Payments_OrderId] ON [Payments] ([OrderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Products_GoldId] ON [Products] ([GoldId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Stones_ProductId] ON [Stones] ([ProductId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Warranties_CustomerId] ON [Warranties] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    CREATE INDEX [IX_Warranties_OrderDetailId] ON [Warranties] ([OrderDetailId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240709174745_UpdateAll'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240709174745_UpdateAll', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710142306_UpdateCategoryCode'
)
BEGIN
    ALTER TABLE [Categories] ADD [CategoryCode] nvarchar(max) NOT NULL DEFAULT N'';
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710142306_UpdateCategoryCode'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240710142306_UpdateCategoryCode', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710143540_UpdateCategoryCodeV2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240710143540_UpdateCategoryCodeV2', N'8.0.5');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710172719_UpdatePropertiseCodeAllClass'
)
BEGIN
    ALTER TABLE [Warranties] ADD [WarrantyCode] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710172719_UpdatePropertiseCodeAllClass'
)
BEGIN
    ALTER TABLE [Stones] ADD [StoneCode] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710172719_UpdatePropertiseCodeAllClass'
)
BEGIN
    ALTER TABLE [Payments] ADD [PaymentCode] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710172719_UpdatePropertiseCodeAllClass'
)
BEGIN
    ALTER TABLE [GoldTypes] ADD [GoldCode] nvarchar(max) NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20240710172719_UpdatePropertiseCodeAllClass'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20240710172719_UpdatePropertiseCodeAllClass', N'8.0.5');
END;
GO

COMMIT;
GO

