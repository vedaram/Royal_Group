CREATE TABLE [dbo].[WeeklyEmployeeReport](
	[In_Punch] [time](7) NULL,
	[out_punch] [time](7) NULL,
	[LateBy] [varchar](max) NULL,
	[EarlyBy] [varchar](max) NULL,
	[Emp_Id] [varchar](max) NULL,
	[Emp_Name] [varchar](max) NULL,
	[pdate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]