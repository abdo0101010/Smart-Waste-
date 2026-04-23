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
CREATE TABLE [Admins] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY ([Id])
);

CREATE TABLE [HubStaff] (
    [StaffID] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [passwordHash] nvarchar(max) NOT NULL,
    [Role] nvarchar(50) NULL DEFAULT N'Sorter',
    CONSTRAINT [PK__HubStaff__96D4AAF74529BE54] PRIMARY KEY ([StaffID])
);

CREATE TABLE [Recyclers] (
    [RecyclerID] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [Phone] nvarchar(20) NOT NULL,
    [passwordHash] nvarchar(max) NOT NULL,
    [VehicleInfo] nvarchar(100) NULL,
    [Status] nvarchar(20) NULL DEFAULT N'Offline',
    [Rating] decimal(3,2) NULL DEFAULT 5.0,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__Recycler__CF8A55D5FEB653EF] PRIMARY KEY ([RecyclerID])
);

CREATE TABLE [Rewards] (
    [RewardID] int NOT NULL IDENTITY,
    [Title] nvarchar(100) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    [CostPoints] decimal(10,2) NOT NULL,
    [StockQuantity] int NOT NULL DEFAULT 0,
    CONSTRAINT [PK__Rewards__8250159966275F67] PRIMARY KEY ([RewardID])
);

CREATE TABLE [Users] (
    [UserID] int NOT NULL IDENTITY,
    [FullName] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [Address] nvarchar(255) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [WalletPoints] decimal(10,2) NOT NULL DEFAULT 0.0,
    [CreatedAt] datetime NOT NULL DEFAULT ((getdate())),
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK__Users__1788CCAC6EBB6AB5] PRIMARY KEY ([UserID])
);

CREATE TABLE [WasteCategories] (
    [CategoryID] int NOT NULL IDENTITY,
    [CategoryName] nvarchar(50) NOT NULL,
    [PointsPerUnit] decimal(10,2) NOT NULL,
    [UnitType] nvarchar(20) NULL DEFAULT N'Item',
    CONSTRAINT [PK__WasteCat__19093A2BB1FAC898] PRIMARY KEY ([CategoryID])
);

CREATE TABLE [PickupRequests] (
    [RequestID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [RecyclerID] int NULL,
    [HubStaffID] int NULL,
    [RequestDate] datetime NULL DEFAULT ((getdate())),
    [PickupDate] datetime NULL,
    [VerificationDate] datetime NULL,
    [Status] nvarchar(50) NOT NULL DEFAULT N'Pending',
    [QRCode] nvarchar(100) NOT NULL,
    [EstimatedPoints] decimal(10,2) NOT NULL DEFAULT 0.0,
    [FinalPoints] decimal(10,2) NOT NULL DEFAULT 0.0,
    CONSTRAINT [PK__PickupRe__33A8519AFA03C46A] PRIMARY KEY ([RequestID]),
    CONSTRAINT [FK_Requests_Recycler] FOREIGN KEY ([RecyclerID]) REFERENCES [Recyclers] ([RecyclerID]),
    CONSTRAINT [FK_Requests_Staff] FOREIGN KEY ([HubStaffID]) REFERENCES [HubStaff] ([StaffID]),
    CONSTRAINT [FK_Requests_User] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [SupportTickets] (
    [TicketID] int NOT NULL IDENTITY,
    [Subject] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Status] int NOT NULL,
    [Priority] int NOT NULL,
    [Rating] int NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CitizenID] int NOT NULL,
    [DriverID] int NULL,
    CONSTRAINT [PK_SupportTickets] PRIMARY KEY ([TicketID]),
    CONSTRAINT [FK_SupportTickets_Recyclers_DriverID] FOREIGN KEY ([DriverID]) REFERENCES [Recyclers] ([RecyclerID]),
    CONSTRAINT [FK_SupportTickets_Users_CitizenID] FOREIGN KEY ([CitizenID]) REFERENCES [Users] ([UserID]) ON DELETE CASCADE
);

CREATE TABLE [UserRedemptions] (
    [RedemptionID] int NOT NULL IDENTITY,
    [UserID] int NOT NULL,
    [RewardID] int NOT NULL,
    [RedeemedAt] datetime NULL DEFAULT ((getdate())),
    [VoucherCode] nvarchar(50) NULL,
    CONSTRAINT [PK__UserRede__410680D10B55621C] PRIMARY KEY ([RedemptionID]),
    CONSTRAINT [FK_Redemption_Reward] FOREIGN KEY ([RewardID]) REFERENCES [Rewards] ([RewardID]),
    CONSTRAINT [FK_Redemption_User] FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
);

CREATE TABLE [Feedbacks] (
    [FeedbackID] int NOT NULL IDENTITY,
    [RequestID] int NOT NULL,
    [Rating] int NULL,
    [Comment] nvarchar(500) NULL,
    [CreatedAt] datetime NULL DEFAULT ((getdate())),
    CONSTRAINT [PK__Feedback__6A4BEDF6AA4CCE99] PRIMARY KEY ([FeedbackID]),
    CONSTRAINT [FK_Feedback_Request] FOREIGN KEY ([RequestID]) REFERENCES [PickupRequests] ([RequestID])
);

CREATE TABLE [RequestItems] (
    [ItemID] int NOT NULL IDENTITY,
    [RequestID] int NOT NULL,
    [CategoryID] int NOT NULL,
    [Quantity] decimal(10,2) NOT NULL,
    [Source] nvarchar(50) NOT NULL,
    CONSTRAINT [PK__RequestI__727E83EB51E2336E] PRIMARY KEY ([ItemID]),
    CONSTRAINT [FK_Items_Category] FOREIGN KEY ([CategoryID]) REFERENCES [WasteCategories] ([CategoryID]),
    CONSTRAINT [FK_Items_Request] FOREIGN KEY ([RequestID]) REFERENCES [PickupRequests] ([RequestID]) ON DELETE CASCADE
);

CREATE INDEX [IX_Feedbacks_RequestID] ON [Feedbacks] ([RequestID]);

CREATE INDEX [IX_PickupRequests_HubStaffID] ON [PickupRequests] ([HubStaffID]);

CREATE INDEX [IX_PickupRequests_RecyclerID] ON [PickupRequests] ([RecyclerID]);

CREATE INDEX [IX_PickupRequests_UserID] ON [PickupRequests] ([UserID]);

CREATE UNIQUE INDEX [UQ__Recycler__5C7E359E9AF9A312] ON [Recyclers] ([Phone]);

CREATE INDEX [IX_RequestItems_CategoryID] ON [RequestItems] ([CategoryID]);

CREATE INDEX [IX_RequestItems_RequestID] ON [RequestItems] ([RequestID]);

CREATE INDEX [IX_SupportTickets_CitizenID] ON [SupportTickets] ([CitizenID]);

CREATE INDEX [IX_SupportTickets_DriverID] ON [SupportTickets] ([DriverID]);

CREATE INDEX [IX_UserRedemptions_RewardID] ON [UserRedemptions] ([RewardID]);

CREATE INDEX [IX_UserRedemptions_UserID] ON [UserRedemptions] ([UserID]);

CREATE UNIQUE INDEX [UQ__Users__A9D10534AFF072C8] ON [Users] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260422182504_v1', N'9.0.13');

COMMIT;
GO

