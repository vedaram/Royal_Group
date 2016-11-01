USE [STW_DB]
GO

/****** Object:  Table [dbo].[PayrollAggregationReport]    Script Date: 11-05-2016 01:29:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PayrollAggregationReport](
	[EmployeeID] [varchar](100) NOT NULL,
	[EmployeeName] [varchar](100) NULL,
	[Department] [varchar](100) NULL,
	[Designation] [varchar](100) NULL,
	[Month] [varchar](100) NULL,
	[WeekDay] [varchar](100) NULL,
	[WorkDate] [date] NULL,
	[FirstIn] [datetime] NULL,
	[LastOut] [datetime] NULL,
	[Actual_Hours] [varchar](10) NULL,
	[Rounded_Hours] [varchar](10) NULL,
	[Mandatory_Hours] [varchar](10) NULL,
	[Discrepancy] [varchar](10) NULL,
	[Status] [varchar](10) NULL,
	[LateBy] [varchar](10) NULL,
	[EarlBy] [varchar](10) NULL,
	[OneMissingPunch] [varchar](10) NULL,
	[TwoMissingPunch] [varchar](10) NULL,
	[OverTime] [varchar](10) NULL,
	[CompanyName] [varchar](255) NULL,
	[BranchName] [varchar](255) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


