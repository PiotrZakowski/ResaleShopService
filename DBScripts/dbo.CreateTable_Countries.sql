CREATE TABLE [dbo].[Countries]
(
	[Id]			INT			NOT NULL IDENTITY (1, 1) PRIMARY KEY, 
    [CountryName]	VARCHAR(50) NOT NULL, 
    [CountryTag]	VARCHAR(8)	NOT NULL
)
