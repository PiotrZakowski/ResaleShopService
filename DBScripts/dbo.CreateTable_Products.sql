CREATE TABLE [dbo].[Products] (
    [Id]               INT          IDENTITY (1, 1) NOT NULL,
    [ManufacturerName] VARCHAR (50) NOT NULL,
    [ModelName]        VARCHAR (50) NOT NULL,
	[Price]            MONEY        NULL DEFAULT NULL,
    [Quantity]         INT          DEFAULT ((0)) NOT NULL,
    [OriginCountry]    VARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

