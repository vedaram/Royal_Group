USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[LateComerReport1]    Script Date: 06/22/2016 18:35:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[LateComerReport1]
(
@where varchar(max)
) 
	AS
BEGIN
	TRUNCATE TABLE LateComersReport
	declare @sql varchar(max)
	set @sql=null
	
set @sql='insert into LateComersReport (Emp_ID,Emp_Name,Cat_Name,Dept_Name,EmployeeCategory , Desig_Name, Shift_Code, PDate,Shift_In,In_Punch,LateBy,Company,Shift_Name , 
status  )
 select Emp_ID,Emp_Name,Cat_Name,Dept_Name, EmployeeCategory , Desig_Name,Shift_code,PDate,Shift_In,In_Punch,LateBy,Comp_Name,Shift_Name , status 
 from MASTERPROCESSDAILYDATA '+@where+' and LateBy is not null order by Emp_ID, Pdate'
execute(@sql)

END
