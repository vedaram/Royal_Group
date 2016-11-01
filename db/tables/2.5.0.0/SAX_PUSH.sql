DROP TABLE [dbo].[LastDateSAXPush]
CREATE TABLE [dbo].[LastDateSAXPush](
	[DeviceId] [varchar](30) NOT NULL,
	[DeviceLocation] [varchar](250) NULL,
	[DataStamp] [bigint] NULL,
	[TemplateStamp] [bigint] NULL,
	[LastPunchDateTime] [datetime] NULL,
	[TimeZone] [int] NULL,
 CONSTRAINT [deviceid] PRIMARY KEY CLUSTERED 
(
	[DeviceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

DROP TABLE [dbo].[Device_Location]
CREATE TABLE [dbo].[Device_Location](
	[DeviceID] [varchar](30) NOT NULL,
	[DeviceLocation] [varchar](250) NULL
) ON [PRIMARY]

GO

DROP TABLE [dbo].[Trans_Raw#_All]
CREATE TABLE [dbo].[Trans_Raw#_All](
	[EmpId] [varchar](12) NULL,
	[Punch_Time] [datetime] NULL,
	[PunchDate] [datetime] NULL,
	[Verification_Code] [varchar](max) NULL,
	[CardNo] [varchar](100) NOT NULL,
	[Deviceid] [varchar](30) NULL,
	[STATUS] [int] NULL,
	[status1] [varchar](max) NULL,
	[ReceivedTime] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

DROP TABLE [dbo].[Trans_Raw#_Temp]
CREATE TABLE [dbo].[Trans_Raw#_Temp](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EmpId] [varchar](12) NULL,
	[Punch_Time] [datetime] NULL,
	[PunchDate] [datetime] NULL,
	[Verification_Code] [varchar](max) NULL,
	[CardNo] [varchar](100) NOT NULL,
	[Deviceid] [varchar](30) NULL,
	[STATUS] [int] NULL,
	[status1] [varchar](max) NULL,
	[Latitude] [varchar](25) NULL,
	[Longitude] [varchar](25) NULL,
	[Address] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

DROP TABLE [dbo].[Unprocessed_Punches]
CREATE TABLE [dbo].[Unprocessed_Punches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [varchar](12) NULL,
	[Punch_Time] [datetime] NULL,
	[PunchDate] [datetime] NULL,
	[Verification_Code] [varchar](max) NULL,
	[CardNo] [varchar](10) NULL,
	[Deviceid] [varchar](30) NULL,
	[status] [int] NULL,
	[status1] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

DROP TABLE [dbo].[DeviceIDList]
CREATE TABLE [dbo].[DeviceIDList](
	[DeviceID] [varchar](30) NULL,
	[Flag] [int] NULL
) ON [PRIMARY]

GO

DROP TABLE [dbo].[EnrollmentIDList]
CREATE TABLE [dbo].[EnrollmentIDList](
	[DeviceID] [varchar](30) NULL,
	[EnrollId] [varchar](50) NULL,
	[Privilege] [int] NULL,
	[FPID] [varchar](10) NULL,
	[FPTemplate] [varchar](max) NULL,
	[Size] [varchar](10) NULL,
	[Name] [varchar](100) NULL,
	[Password] [varchar](50) NULL,
	[Card] [varchar](50) NULL,
	[Action] [varchar](50) NULL,
	[UserID] [varchar](50) NULL,
	[Status] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

DROP TABLE [dbo].[SAXPushTemplate]
CREATE TABLE [dbo].[SAXPushTemplate](
	[DeviceID] [varchar](30) NULL,
	[DeviceIDRecent] [varchar](30) NULL,
	[EnrollId] [varchar](50) NULL,
	[Privilege] [int] NULL,
	[FPID] [varchar](10) NULL,
	[FPTemplate] [varchar](max) NULL,
	[Size] [varchar](10) NULL,
	[Name] [varchar](100) NULL,
	[Password] [varchar](50) NULL,
	[Card] [varchar](50) NULL,
	[Valid] [varchar](50) NULL,
	[ReceivedTime] [datetime] NULL,
	[ReceivedTimeRecent] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
