--USE [myntraphase5]

--sp_rename 'movedata','securtimereprocess'modified on 04112014
ALTER procedure [dbo].[securtimereprocess] (@fromdate datetime,@todate datetime)
as
begin
declare @fkey int
declare @re_flag int
select @re_flag=re_flag from ReprocessFlag
if( @re_flag is null)begin
insert into ReprocessFlag values(0)
end

------
if (@re_flag=0)
begin
  update ReprocessFlag set re_flag = 1
  truncate table storeempid1
 insert into StoreEmpid1 (eid) select distinct emp_code from employeemaster where Emp_Status=1 
 insert into StoreEmpid1 (eid) select distinct emp_code from employeemaster where Emp_Status!=1  and emp_dol between @fromdate and @todate 
	insert into trans_raw# (empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1) select empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1 from mastertrans_raw# where punchdate between @fromdate and @todate+1
	delete from mastertrans_raw# where punchdate between @fromdate and @todate+1
	select @fkey=isfkey from shiftsetting
	if(@fkey is null or @fkey=0)
	begin
	delete from process_data where pdate between  @fromdate and @todate+1
	end
	else
	begin
	delete from process_datal where pdate between  @fromdate and @todate+1  and EmpId in (select eid from storeempid1)
	delete from process_data where pdate between  @fromdate and @todate+1  and EmpId in (select eid from storeempid1)
	end
	delete from masterprocessdailydata where pdate  between @fromdate   and @todate  and Emp_Id in (select eid from storeempid1)
	truncate table ShiftEmployee_Processing
    insert into ShiftEmployee_Processing select *from ShiftEmployee where month between DATEPART(mm,@fromdate) and  DATEPART(mm,@todate) and year in (DATEPART(yyyy,@todate),DATEPART(yyyy,@fromdate))
	if(@fkey=1)
	begin
	--exec sp_rawdata_function
	exec sp_processdailydata_rawdata_function
	end
	else
	begin
	exec sp_rawdata
	exec sp_processdailydata_rawdata  @fromdate,@todate

	end
	--exec Sp_SetDownloader_DetailedMonthlyReport
	insert into mastertrans_raw# (empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1) select empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1 from trans_raw# where punchdate between @fromdate and @todate+1 and EmpId in (select eid from storeempid1)
	delete from Trans_Raw#  where punchdate between @fromdate and @todate+1 and EmpId in (select eid from storeempid1)
	update ReprocessFlag set re_flag = 0
end
end