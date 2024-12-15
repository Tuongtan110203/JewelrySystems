using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanVang.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Categories](
                        [CategoryId] [int] IDENTITY(1,1) NOT NULL,
                        [CategoryCode][nvarchar](30) NOT NULL,
                        [Name] [nvarchar](max) NOT NULL,
                        [Status] [nvarchar](max) NOT NULL,
                        CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([CategoryId] ASC))
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Customers](
                        [CustomerId] [int] IDENTITY(1,1) NOT NULL,
                        [CustomerName] [nvarchar](max) NULL,
                        [PhoneNumber] [nvarchar](max) NULL,
                        [Email] [nvarchar](max) NULL,
                        [Status] [nvarchar](max) NULL,
                        CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([CustomerId] ASC))
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GoldTypes]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[GoldTypes](
                        [GoldId] [int] IDENTITY(1,1) NOT NULL,
                        [GoldName] [nvarchar](max) NOT NULL,
                        [GoldCode] [nvarchar](30) NOT NULL,
                        [BuyPrice] [float] NOT NULL,
                        [SellPrice] [float] NOT NULL,
                        [UpdateTime] [datetime2] NULL,
                        [Status] [nvarchar](max) NOT NULL,
                        CONSTRAINT [PK_GoldTypes] PRIMARY KEY CLUSTERED ([GoldId] ASC))
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Roles](
                        [RoleId] [int] IDENTITY(1,1) NOT NULL,
                        [RoleName] [nvarchar](max) NOT NULL,
                        [Status] [nvarchar](max) NOT NULL,
                        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC))
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Products](
                        [ProductId] [int] IDENTITY(1,1) NOT NULL,
                        [CategoryId] [int] NOT NULL,
                        [GoldId] [int] NOT NULL,
                        [ProductCode] [nvarchar](max) NOT NULL,
                        [ProductName] [nvarchar](max) NOT NULL,
                        [Description] [nvarchar](max) NOT NULL,
                        [Image] [nvarchar](max) NULL,
                        [Quantity] [int] NOT NULL,
                        [GoldWeight] [float] NOT NULL,
                        [Wage] [float] NOT NULL,
                        [Price] [float] NOT NULL,
                        [Size] [nvarchar](max) NOT NULL,
                        [WarrantyPeriod] [int] NOT NULL,
                        [Status] [nvarchar](max) NOT NULL,
                        CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductId] ASC),
                        CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY([CategoryId]) REFERENCES [Categories]([CategoryId]) ON DELETE CASCADE,
                        CONSTRAINT [FK_Products_GoldTypes_GoldId] FOREIGN KEY([GoldId]) REFERENCES [GoldTypes]([GoldId]) ON DELETE CASCADE)
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Users](
                        [UserName] [nvarchar](99) NOT NULL,
                        [FullName] [nvarchar](max) NOT NULL,
                        [Password] [nvarchar](max) NOT NULL,
                        [Address] [nvarchar](max) NOT NULL,
                        [Phone] [nvarchar](max) NOT NULL,
                        [Email] [nvarchar](max) NOT NULL,
                        [Dob] [datetime2] NOT NULL,
                        [Level] [int] NULL,
                        [Status] [nvarchar](max) NOT NULL,
                        [RoleId] [int] NOT NULL,
                        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserName] ASC),
                        CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [Roles]([RoleId]) ON DELETE CASCADE)
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Stones]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Stones](
                        [StoneId] [int] IDENTITY(1,1) NOT NULL,
                        [StoneCode][nvarchar](30) NOT NULL,
                        [ProductId] [int] NOT NULL,
                        [Name] [nvarchar](max) NOT NULL,
                        [Price] [float] NOT NULL,
                        [Status] [nvarchar](max) NOT NULL,
                        [Color] [nvarchar](max) NOT NULL,
                        CONSTRAINT [PK_Stones] PRIMARY KEY CLUSTERED ([StoneId] ASC),
                        CONSTRAINT [FK_Stones_Products_ProductId] FOREIGN KEY([ProductId]) REFERENCES [Products]([ProductId]) ON DELETE CASCADE)
                END");

            migrationBuilder.Sql(@"
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[Orders](
            [OrderId] [int] IDENTITY(1,1) NOT NULL,
            [OrderCode] [nvarchar](max) NOT NULL,
             [UserName] [nvarchar](99) NOT NULL,
            [CustomerId] [int] NULL,
            [OrderDate] [datetime2] NOT NULL,
            [Total] [float] NOT NULL,
            [SaleById] [nvarchar](max)  NULL,
            [CashierId] [nvarchar](max)  NULL,
            [ServicerId] [nvarchar](max)  NULL,
            [CustomerName] [nvarchar](max) NULL,
            [PhoneNumber] [nvarchar](max) NULL,
            [Email] [nvarchar](max) NULL,
            [Status] [nvarchar](max) NOT NULL,
            CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([OrderId] ASC),
            CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY([CustomerId]) REFERENCES [Customers]([CustomerId]),
            CONSTRAINT [FK_Orders_Users_UserName] FOREIGN KEY([UserName]) REFERENCES [Users]([UserName]) ON DELETE CASCADE)
    END");


            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[OrderDetails](
                        [OrderDetailId] [int] IDENTITY(1,1) NOT NULL,
                        [Price] [float] NOT NULL,
                        [Quantity] [int] NOT NULL,
                        [OrderId] [int] NOT NULL,
                        [ProductId] [int] NOT NULL,
                        CONSTRAINT [PK_OrderDetails] PRIMARY KEY CLUSTERED ([OrderDetailId] ASC),
                        CONSTRAINT [FK_OrderDetails_Orders_OrderId] FOREIGN KEY([OrderId]) REFERENCES [Orders]([OrderId]) ON DELETE CASCADE,
                        CONSTRAINT [FK_OrderDetails_Products_ProductId] FOREIGN KEY([ProductId]) REFERENCES [Products]([ProductId]) ON DELETE CASCADE)
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Payments]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[Payments](
                        [PaymentId] [int] IDENTITY(1,1) NOT NULL,
                        [PaymentCode] [nvarchar](30) NOT NULL,
                        [OrderId] [int] NOT NULL,
                        [PaymentType] [nvarchar](max) NULL,
                        [Cash] [float] NULL,
                        [BankTransfer] [float] NULL,
                        [TransactionId] [nvarchar](max) NULL,
                        [PaymentTime] [datetime2] NOT NULL,
                        [Image] [nvarchar](max) NULL,
                        [Status] [nvarchar](max) NULL,
                        CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([PaymentId] ASC),
                        CONSTRAINT [FK_Payments_Orders_OrderId] FOREIGN KEY([OrderId]) REFERENCES [Orders]([OrderId]) ON DELETE CASCADE)
                END");

           
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Warranties]') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[Warranties](
                [WarrantyId] [int] IDENTITY(1,1) NOT NULL,
                [WarrantyCode] [nvarchar](30) NOT NULL,
                [StartDate] [datetime2] NOT NULL,
                [EndDate] [datetime2] NOT NULL,
                [OrderDetailId] [int] NOT NULL,
                [CustomerId] [int] NULL,
                [Status] [nvarchar](100) NOT NULL,
                CONSTRAINT [PK_Warranties] PRIMARY KEY CLUSTERED ([WarrantyId] ASC),
                CONSTRAINT [FK_Warranties_OrderDetails_OrderDetailId] FOREIGN KEY([OrderDetailId]) REFERENCES [OrderDetails]([OrderDetailId]) ON DELETE CASCADE,
                CONSTRAINT [FK_Warranties_Customers_CustomerId] FOREIGN KEY([CustomerId]) REFERENCES [Customers]([CustomerId]) ON DELETE CASCADE)
        END");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImportInvoiceDetails");

            migrationBuilder.DropTable(
                name: "ImportInvoices");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Stones");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "GoldTypes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Categories");
            migrationBuilder.DropTable(
                name: "Warranties");
        }
    }
}
