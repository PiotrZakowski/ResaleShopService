CREATE TABLE [dbo].[Prices] (
    [Id]         INT         IDENTITY (1, 1) NOT NULL,
    [ProductId]  INT         NOT NULL,
    [CountryId] INT NOT NULL,
    [Price]      MONEY       NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Prices_ToProducts] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Products] ([Id]), 
    CONSTRAINT [FK_Prices_ToCountries] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Countries]([Id])
);

