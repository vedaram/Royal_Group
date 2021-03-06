
/****** Object:  StoredProcedure [dbo].[leave]    Script Date: 28-06-2016 12:21:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[leave]
(
@mode varchar(max),
@CompanyCode varchar (max),
@EmployeeCategoryCode varchar (max),
@leavecode varchar(10),
@Newleavecode varchar(10)=null,
@leavename varchar(50)=null,
@Maxleave [decimal](18,1)=null,
@maxleavecarry [decimal](18,1)=null,
@woflag int=null,
@leavestatus varchar(10)
)
as
begin
if @mode='I'
begin
insert into LeaveMaster(CompanyCode,EmployeeCategoryCode,LeaveCode,LeaveName,Maxleave,Maxleavecarryforward,woflag , Status)
values(@CompanyCode,@EmployeeCategoryCode,@leavecode,@leavename,@Maxleave,@maxleavecarry,@woflag , @leavestatus)
end
if @mode='U'
begin
update LeaveMaster set LeaveCode=@leavecode,LeaveName=@leavename,Maxleave=@Maxleave,woflag=@woflag,Status = @leavestatus , 
Maxleavecarryforward=@maxleavecarry where LeaveCode=@leavecode and EmployeeCategoryCode=@EmployeeCategoryCode and CompanyCode=@CompanyCode

end

if @mode='D'
begin
delete from LeaveMaster where LeaveCode=@leavecode
end
end