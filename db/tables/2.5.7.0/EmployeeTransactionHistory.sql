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