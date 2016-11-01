CREATE TABLE [dbo].[EmployeeTransactionData](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EmpID] [varchar](100) NULL,
	[TransactionType] [int] NULL,
	[FromDate] [date] NULL,
	[ToDate] [date] NULL,
	[TransactionData] [varchar](max) NULL,
	[IsActive] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TYPE [dbo].[Employee_Transaction1] AS TABLE(
 [id] [int] NULL,
 [TransactionType] [int] NULL,
 [FromDate] [date] NULL,
 [ToDate] [date] NULL,
 [TransactionData] [varchar](max) NULL,
 [StatusFlag] [varchar](10) NULL
)


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


CREATE TABLE [dbo].[EmployeeTransactionDummy](
 [ID] [int] IDENTITY(1,1) NOT NULL,
 [TransactionType] [int] NULL,
 [FromDate] [date] NULL,
 [ToDate] [date] NULL,
 [TransactionData] [varchar](max) NULL,
 [Statusflag] [varchar](10) NULL,
 [EmpId] [varchar](100) NULL,
 [shift_desc] [varchar](200) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[TransactionType](
 [TransactionType] [int] NULL,
 [TransactionName] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (1, N'Shift')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (2, N'OT')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (3, N'Ramadan')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (4, N'Punch')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (5, N'Maternity')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (6, N'WorkHourPerDay')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (7, N'WorkHourPerWeek')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (8, N'WorkHourPerMonth')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (9, N'Termination')

INSERT [dbo].[TransactionType] ([TransactionType], [TransactionName]) VALUES (10, N'LineManager')