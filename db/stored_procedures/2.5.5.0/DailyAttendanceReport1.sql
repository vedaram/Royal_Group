USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[DailyAttendanceReport1]    Script Date: 06/22/2016 18:30:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[DailyAttendanceReport1] (@where varchar(max))
as Begin    
truncate table   presentabsent  
 declare @sql varchar(max)
 set @sql=null
 set @sql = 'insert into presentabsent(Emp_ID, Emp_Name, PDate, In_Time, Out_Time, Status,Comp_Name,Branch_Name,Dept_Name,Shift_Name , Employee_Category)
 select Emp_ID, Emp_Name, PDate, In_Punch, Out_Punch, Status,Comp_Name,Cat_Name,Dept_Name, EmployeeCategory as  Employee_Category ,Shift_Name  from MASTERPROCESSDAILYDATA '+@where+' order by PDate'
 execute (@sql)
  
End