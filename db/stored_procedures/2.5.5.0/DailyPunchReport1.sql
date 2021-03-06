USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[DailyPunchReport1]    Script Date: 23-Jun-2016 01:15:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[DailyPunchReport1] (@where varchar(max))    
as Begin    
 truncate table DailyEmployeePunchReport    
 declare @sql varchar(max)
 set @sql=null
 
set @sql=' insert into DailyEmployeePunchReport(Emp_ID, Emp_Name, PDate,Punch_Time,Comp_Name,Branch_Name,Dept_Name,Shift_Name , EmployeeCategory)
select m.EmpId , e.Emp_Name, m.PunchDate,CONVERT(varchar,m.punch_time,108) ,c.Companyname,b.branchname,d.DeptName,s.Shift_Desc , ecm.empcategoryname from MasterTrans_Raw# m
 left outer join EmployeeMaster e on m.EmpId =e.Emp_Code left outer join CompanyMaster c on e.Emp_Company=c.CompanyCode left outer 
 join BranchMaster b on e.Emp_Branch  =  b.BranchCode  left outer join DeptMaster d on e.Emp_Department = d.DeptCode  left outer join
  shift s  on e.Emp_Shift_Detail  = s.Shift_Code  left outer join
  EmployeeCategoryMaster ecm   on e.Emp_Employee_Category  = ecm.EmpCategoryCode '+@where+' order by PunchDate'

execute(@sql)
End