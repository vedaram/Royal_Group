USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[sp_LeaveCard]    Script Date: 22-Jun-2016 23:21:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_LeaveCard](@where varchar(max))
as
begin
declare
@EmpId varchar(max),
@EmpName varchar(max),
@comp varchar(max),
@branch varchar(max),
@dept varchar(max),
@empcatcode varchar(max),
@categoryname varchar(max),
@leavecode varchar(max),
@leavename varchar(max),
@maxleave [decimal](18,2),
@leaveconsume [decimal](18,2),
@leavebal [decimal](18,2),
@count [decimal](18,2),
@index int,
@lid int,
@count2 [decimal](18,2),
@Lstatus int
Declare @Temp_EID as Table
(
id int identity(1,1),
EID varchar(Max),
Lcode varchar(max)
)
truncate table LeaveCard
insert into @Temp_EID(EID,Lcode) select Emp_Code,Leave_Code from Employee_Leave 

select @count=COUNT(EID) from @Temp_EID

set @index=1
While(@index <= @count)
begin
select @EmpId = EID from @Temp_EID where id = @index
select @leavecode=LCode from @Temp_EID where id=@index
select @EmpName=Emp_Name from EmployeeMaster where Emp_Code=@EmpId
select @comp=companyname  from companymaster  where  companycode in ( select emp_company from EmployeeMaster where  Emp_Code=@EmpId)
select @branch=branchname  from branchmaster  where  branchcode in (select emp_branch from EmployeeMaster where Emp_code =@EmpId)
select @dept=DeptName from deptmaster  where  deptcode in ( select emp_department from employeemaster where  Emp_code=@EmpId)
select @categoryname=empcategoryname  from employeecategorymaster where  empcategorycode in ( select emp_employee_category from employeemaster where Emp_code=@EmpId)
select @leavename=LeaveName from LeaveMaster where LeaveCode=@leavecode
select @maxleave=Max_leaves from Employee_Leave where Emp_code=@EmpId and Leave_code=@leavecode
select @leaveconsume=Leaves_applied from Employee_leave where Emp_code=@EmpId and Leave_code=@leavecode
select @leavebal=Leave_balance from Employee_Leave where Emp_code=@EmpId and Leave_code=@leavecode

insert into LeaveCard (CompName,BranchName,DeptName,EmpId,EmpName,LeaveType,MaxLeave,LeaveConsumed,LeaveBl , EmployeeCategory) 
values(@comp,@branch,@dept,@EmpId,@EmpName,@leavename,@maxleave,@leaveconsume,@leavebal ,@categoryname )
set @leaveconsume=0
set @Lstatus=0
set @leavebal=0
set @maxleave=0
set @index=@index+1

end
exec LeaveCardReport1  @where
end
