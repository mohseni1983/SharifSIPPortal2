
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/22/2020 11:43:41
-- Generated from EDMX file: E:\Visual Studio Projects\SIPSoftSharif\SIPSoftSharif\Models\SIP.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [madadkar_online];
GO

-- Creating table 'HamiEditSet'
CREATE TABLE [dbo].[HamiEditSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HamiId] int  NOT NULL,
    [HamiFname] nvarchar(max)  NULL,
    [HamiLname] nvarchar(max)  NULL,
    [OldMobile1] nvarchar(max)  NULL,
    [NewMobile1] nvarchar(max)  NULL,
    [OldMobile2] nvarchar(max)  NULL,
    [NewMobile2] nvarchar(max)  NULL,
    [OldPhone1] nvarchar(max)  NULL,
    [NewPhone1] nvarchar(max)  NULL,
    [OldPhone2] nvarchar(max)  NULL,
    [NewPhone2] nvarchar(max)  NULL,
    [Email] nvarchar(max)  NULL,
    [NationalCode] nvarchar(max)  NULL,
    [MadadkarId] int  NOT NULL,
    [MadadkarName] nvarchar(max)  NULL,
    [EditDate] datetime  NULL
);
GO

-- Creating table 'HamiMadadjouSet'
CREATE TABLE [dbo].[HamiMadadjouSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HamiId] nvarchar(max)  NOT NULL,
    [HamiFname] nvarchar(max)  NULL,
    [HamiLname] nvarchar(max)  NULL,
    [MadadjouId] nvarchar(max)  NOT NULL,
    [MadadjouFname] nvarchar(max)  NULL,
    [MadadjouLname] nvarchar(max)  NULL,
    [Amount] int  NULL,
    [AddDate] datetime  NULL
);
GO





-- Creating primary key on [Id], [HamiId] in table 'HamiEditSet'
ALTER TABLE [dbo].[HamiEditSet]
ADD CONSTRAINT [PK_HamiEditSet]
    PRIMARY KEY CLUSTERED ([Id], [HamiId] ASC);
GO

-- Creating primary key on [Id], [HamiId] in table 'HamiMadadjouSet'
ALTER TABLE [dbo].[HamiMadadjouSet]
ADD CONSTRAINT [PK_HamiMadadjouSet]
    PRIMARY KEY CLUSTERED ([Id], [HamiId] ASC);
GO

