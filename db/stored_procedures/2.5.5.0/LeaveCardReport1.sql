USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[LeaveCardReport1]    Script Date: 22-Jun-2016 23:42:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[LeaveCardReport1](@where varchar(max))
as
begin
truncate table templeavecard
declare @sql varchar(max)
set @sql=null
set @sql='insert into TempLeaveCard (CompName,BranchName,DeptName,EmpId,EmpName,LeaveType,MaxLeave,LeaveConsumed,LeaveBl, EmployeeCategory)
 select CompName,BranchName,DeptName,EmpId,EmpName,LeaveType,MaxLeave,LeaveConsumed,LeaveBl , EmployeeCategory from LeaveCard '+@where+'' 
execute(@sql)
End