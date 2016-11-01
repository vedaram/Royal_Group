
Create procedure [dbo].[WeeklyEmployeeReport1] (@where varchar(max))
As
Begin
  truncate table WeeklyEmployeeReport
 declare @sql Varchar(max)

  set @sql=null
  set @sql=  'Insert  into WeeklyEmployeeReport( Emp_id,Emp_Name,Pdate,In_punch ,Out_punch ,LateBy,EarlyBy) 
  select Emp_id,Emp_Name,Pdate, convert(varchar(5),convert(varchar,convert(time(7),In_Punch))) , convert(varchar(5),convert(varchar,convert(time(7),Out_Punch))),convert(varchar(5),LateBy),convert(varchar(5),EarlyBy) from MasterProcessDailyData '+@where+' and Emp_Id is not null and In_punch is not null and Out_Punch is not null  order by PDate, Emp_ID'
 
 execute(@sql)

 End