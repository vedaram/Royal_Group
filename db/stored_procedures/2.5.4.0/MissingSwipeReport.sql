
  
  
    
ALTER PROCEDURE [dbo].[MissingSwipeFilter1]    
    
@where varchar(max)=''    
 AS    
BEGIN    
 TRUNCATE TABLE MissingSwipeReport    
 declare @sql varchar(max)=''    
 set @sql= 'insert into MissingSwipeReport(Emp_ID,Emp_Name,Cat_Name,Dept_Name,Desig_Name,Shift_Code,PDate,status,Company,Shift_Name )     
 select Emp_ID,Emp_Name,Cat_Name,Dept_Name,Desig_Name,Shift_Code,PDate,Status,Comp_Name,Shift_Name     
 from MASTERPROCESSDAILYDATA      
  '+@where+'  order by Emp_ID, Pdate'    
 execute(@sql)    
     
END  
  