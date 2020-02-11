CREATE TABLE [dbo].[Users] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Username]     VARCHAR (50)  NOT NULL,
    [Password]     VARCHAR (50)  NOT NULL,
    [GoogleId]     VARCHAR (200) NULL,
    [RefreshToken] VARCHAR (MAX) NULL,
    [BearerToken]  VARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

