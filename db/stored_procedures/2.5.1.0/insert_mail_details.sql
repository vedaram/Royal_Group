Create procedure [dbo].[InsertMailsDetails]
(

@EmpID varchar(max),
@EmpName varchar(max),
@ToEmailID varchar(max),
@CCEmailID varchar(max),
@Subject varchar(max),
@Body varchar(max)
)
as

begin

insert into Mailsdetails (EmpID,EmpName,ToEmailID,CCEmailID,Subject, Body,Sendlag)
values (@EmpID,@EmpName,@ToEmailID,@CCEmailID,@Subject, @Body,0)
end

 