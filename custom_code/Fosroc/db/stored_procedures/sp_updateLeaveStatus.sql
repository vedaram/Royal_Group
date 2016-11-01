
/****** Object:  StoredProcedure [dbo].[sp_updateLeaveStatus]    Script Date: 06/25/2016 17:21:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[sp_updateLeaveStatus]
as
begin
declare @index int,@count int,@flag int,@lfrom datetime,@lto datetime,@empid1 varchar(max),@leavetype varchar(max),@cntleave int,@index1 int, 
@cntemp int,@indemp int,@empid varchar(max),@lvbal decimal(5,2),@catcode varchar(max),@locdate datetime,@totalL decimal(5,2) 
  
set @indemp=1  
truncate table storeempid  
--update employee set emp_cat='008' where emp_id='1900'  
--select * from employeemaster
insert into storeempid (eid)  select distinct emp_code from employeemaster where emp_employee_category in('001','002') and emp_code!='1900'  and Emp_Status=1 
--insert into storeempid (eid) values ('2071')  
select @cntemp=count(*) from storeempid  
while(@indemp<=@cntemp)  
begin  
select @empid=eid from storeempid where id=@indemp  
select @catcode=emp_branch from employeemaster where emp_code=@empid  
select @locdate=date from location where loc=@catcode  
--if(@empid='1970' or @empid='1706')  
--begin  
    -- set @locdate='01-Jan-2015' 
     set @locdate=cast(year(getdate()) as varchar(4))+'-01-01'
--end 
truncate table tempfindleave  
insert into tempfindleave(empid,fdate,tdate,leavetype,flag) select empid,startdate,enddate,leavetype,flag from leave1 where  empid=@empid and flag in (2)    and year(ldate)=year(@locdate)
select @count=COUNT(empid)from tempfindleave  
set @index=1  
set @index1=1  


while (@index<=@count)  
begin  
  
set @lfrom =null  
set @lto =null  
select @empid1=empid,@lfrom=fdate,@lto=tdate, @leavetype=leavetype,@flag=flag from tempfindleave where lid=@index  
    while DATEDIFF(dd,@lfrom,@lto)>=0  
      begin  
             ---update masterprocessdaily data emp day status-------
       update masterprocessdailydata set status=dbo.fnFirsties(@leavetype) where emp_id=@empid1 and pdate=@lfrom
       set @lfrom=DATEADD(dd,1,@lfrom)  

     end  
set @index=@index+1      
end   
 

set @indemp=@indemp+1 
end  


end
   



