CREATE

TABLE [dbo].[MailsDetails](
[id] [bigint] IDENTITY(1,1) NOT NULL,
[EmpID] [varchar](100) NULL,
[EmpName] [varchar](100) NULL,
[ToEmailID] [varchar](max) NULL,
[CCEmailID] [varchar](max) NULL,
[Subject] [varchar](max) NULL,
[Body] [varchar](max) NULL,
[Sendlag] [int] NULL
)