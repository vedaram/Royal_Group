USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[MissingSwipeFilter1]    Script Date: 06/22/2016 18:36:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

  
  
    
ALTER PROCEDURE [dbo].[MissingSwipeFilter1]    
    
@where varchar(max)=''    
 AS    
BEGIN    
 TRUNCATE TABLE MissingSwipeReport    
 declare @sql varchar(max)=''    
 set @sql= 'insert into MissingSwipeReport(Emp_ID,Emp_Name,Cat_Name,Dept_Name,EmployeeCategory, Desig_Name,Shift_Code,PDate,status,Company,Shift_Name  )     
 select Emp_ID,Emp_Name,Cat_Name,Dept_Name,EmployeeCategory,Desig_Name,Shift_Code,PDate,Status,Comp_Name,Shift_Name      
 from MASTERPROCESSDAILYDATA      
  '+@where+'  order by Emp_ID, Pdate'    
 execute(@sql)    
     
END  
  