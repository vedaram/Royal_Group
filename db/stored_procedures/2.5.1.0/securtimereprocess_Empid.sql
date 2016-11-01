GO
/****** Object:  StoredProcedure [dbo].[securtimereprocess_Empid]    Script Date: 4/29/2016 8:03:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[securtimereprocess_Empid] (@fromdate datetime,@todate datetime,@empid varchar(max))
as
begin
truncate table storeempid1
insert into StoreEmpid1 (eid) select * from dbo.Split(@empid, ',')
declare @fkey int

--Set re_flag=1 in  ReprocessFlag table
declare @re_flag int
select @re_flag=count(*) from ReprocessFlag
if(@re_flag >= 1)begin
update ReprocessFlag set re_flag = 1
end
else begin
insert into ReprocessFlag values(1)
end
------

insert into trans_raw# (empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1) select empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1 from mastertrans_raw# where punchdate between @fromdate and @todate+1 and EmpId in (select eid from storeempid1)
delete from mastertrans_raw# where punchdate between @fromdate and @todate+1 and EmpId in (select eid from storeempid1)

select @fkey=isfkey from shiftsetting
if(@fkey is null or @fkey=0)
	begin
		delete from process_data where pdate between  @fromdate and @todate+1 and EmpId in (select eid from storeempid1)
	end
else
	begin
		delete from process_datal where pdate between  @fromdate and @todate+1 and EmpId in (select eid from storeempid1)
	
		delete from process_data where pdate between  @fromdate and @todate+1 and EmpId in (select eid from storeempid1)
	end
	delete from overtime where OTDate between (select min(punchdate) from Trans_Raw#) and (select max(punchdate) from Trans_Raw#)  and EmpId in (select eid from storeempid1)
	
	delete from masterprocessdailydata where pdate  between (select min(punchdate) from Trans_Raw#)   and (select max(punchdate) from Trans_Raw#)  and Emp_Id in (select eid from storeempid1)
delete from masterprocessdailydata where pdate  between @fromdate   and @todate  and Emp_Id in (select eid from storeempid1)

----end--------
truncate table ShiftEmployee_Processing
insert into ShiftEmployee_Processing select *from ShiftEmployee where month between DATEPART(mm,@fromdate) and DATEPART(mm,@todate) and year in (datepart(yy,@fromdate),datepart(yy,@todate))
if(@fkey=1)
begin
--exec sp_rawdata_function
exec sp_processdailydata_rawdata_function
end
else
begin
exec sp_rawdata
---changed by sumaiya to reprocess the data properly for selected dates -----
exec sp_processdailydata_rawdata @fromdate,@todate
---changes end -----
end
exec Sp_SetDownloader_DetailedMonthlyReport
insert into mastertrans_raw# (empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1) select empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1 from trans_raw#  where EmpId in (select eid from storeempid1)
delete from trans_raw# where EmpId in (select eid from storeempid1)
--Set re_flag=1 in  ReprocessFlag table
update ReprocessFlag set re_flag = 0
end