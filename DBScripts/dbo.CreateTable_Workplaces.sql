CREATE TABLE [dbo].[Workplaces] (
    [Id]         INT IDENTITY (1, 1) NOT NULL,
    [UserId]     INT NOT NULL,
    [UserRoleId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Workplaces_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]),
    CONSTRAINT [FK_Workplaces_UserRoles] FOREIGN KEY ([UserRoleId]) REFERENCES [dbo].[UserRoles] ([Id])
);

