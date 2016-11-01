CREATE TABLE [dbo].[EmployeeTransactionHistory](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionDateTime] [datetime] NOT NULL,
	[EmployeeCode] [varchar](100) NULL,
	[Shift] [int] NULL,
	[OverTime] [int] NULL,
	[Ramadan] [int] NULL,
	[PunchException] [int] NULL,
	[Maternity] [int] NULL,
	[WorkHourPerday] [int] NULL,
	[WorkHourPerWeek] [int] NULL,
	[WorkHourPerMonth] [int] NULL,
	[Termination] [int] NULL,
	[LineManagerID] [int] NULL
) ON [PRIMARY]

create table EmployeeTransactionData
(
ID int identity (1,1),
EmpID varchar(100),
TransactionType int,
FromDate Date,
ToDate Date,
TransactionData varchar(max),
IsActive int
)
GO

create table TransactionType
(
TransactionType int,
TransactionName varchar(max)
)
Go

insert into TransactionType values (1,'Shift')
insert into TransactionType values (2,'OT')
insert into TransactionType values (3,'Ramadan')
insert into TransactionType values (4,'Punch')
insert into TransactionType values (5,'Maternity')
insert into TransactionType values (6,'WorkHourPerDay')
insert into TransactionType values (7,'WorkHourPerWeek')
insert into TransactionType values (8,'WorkHourPerMonth')
insert into TransactionType values (9,'Termination')
insert into TransactionType values (10,'LineManager')



CREATE TYPE [dbo].[Employee_Transaction1] AS TABLE(
	[id] [int] NULL,
	[TransactionType] [int] NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
	[TransactionData] [varchar](max) NULL,
	[StatusFlag] [varchar](10) NULL
)


CREATE TABLE [dbo].[EmployeeTransactionDummy](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TransactionType] [int] NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
	[TransactionData] [varchar](max) NULL,
	[Statusflag] [varchar](10) NULL,
	[EmpId] [varchar](100) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]