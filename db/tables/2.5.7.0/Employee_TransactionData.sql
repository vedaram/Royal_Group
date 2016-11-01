CREATE TABLE [dbo].[Employee_TransactionData](
	[TransactionID] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeCode] [varchar](100) NULL,
	[EmployeeName] [varchar](100) NULL,
	[EmployeeCompany] [varchar](50) NULL,
	[EmployeeBranch] [varchar](100) NULL,
	[EmployeeDepartment] [varchar](100) NULL,
	[EmployeeCategory] [varchar](100) NULL,
	[ShiftFromdate] [datetime] NULL,
	[ShiftTodate] [datetime] NULL,
	[Shift_Eligibility] [int] NULL,
	[shift_code] [varchar](20) NULL,
	[Shift_name] [varchar](100) NULL,
	[OTFromdate] [datetime] NULL,
	[OTTodate] [datetime] NULL,
	[OT_Eligibility] [int] NULL,
	[RamadanFromdate] [datetime] NULL,
	[RamadanTodate] [datetime] NULL,
	[Ramadan_Eligibility] [int] NULL,
	[PunchExceptionFromdate] [datetime] NULL,
	[PunchExceptionTodate] [datetime] NULL,
	[PunchException_Eligibility] [int] NULL,
	[MaternityFromdate] [datetime] NULL,
	[MaternityTodate] [datetime] NULL,
	[Maternity_Eligibility] [int] NULL,
	[ChildDateofBirth] [datetime] NULL,
	[MaternityBreakHours] [time](7) NULL,
	[WorkHourPerdayFromdate] [datetime] NULL,
	[WorkHourPerdayTodate] [datetime] NULL,
	[WorkHourPerday_Eligibility] [int] NULL,
	[WorkHourPerday] [time](7) NULL,
	[WorkHourPerWeekFromdate] [datetime] NULL,
	[WorkHourPerWeekTodate] [datetime] NULL,
	[WorkHourPerWeek_Eligibility] [int] NULL,
	[WorkHourPerWeek] [time](7) NULL,
	[WorkHourPerMonthFromdate] [datetime] NULL,
	[WorkHourPerMonthTodate] [datetime] NULL,
	[WorkHourPerMonth_Eligibility] [int] NULL,
	[WorkHourPerMonth] [time](7) NULL,
	[Termination_Eligibility] [int] NULL,
	[Terminationdate] [datetime] NULL,
	[LineManagerFromdate] [datetime] NULL,
	[LineManagerTodate] [datetime] NULL,
	[LineManagerID] [varchar](100) NULL,
	[Line_Manager_Eligibility] [int] NULL
	
) ON [PRIMARY]