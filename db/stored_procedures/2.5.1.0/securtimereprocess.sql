GO
/****** Object:  StoredProcedure [dbo].[securtimereprocess]    Script Date: 4/29/2016 8:03:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--sp_rename 'movedata','securtimereprocess'modified on 04112014
ALTER procedure [dbo].[securtimereprocess] (@fromdate datetime,@todate datetime)
as
begin
declare @fkey int
insert into trans_raw# (empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1) select empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1 from mastertrans_raw# where punchdate between @fromdate and @todate+1
delete from mastertrans_raw# where punchdate between @fromdate and @todate+1
select @fkey=isfkey from shiftsetting
if(@fkey is null or @fkey=0)
begin
delete from process_data where pdate between  @fromdate and @todate+1
end
else
begin
delete from process_datal where pdate between  @fromdate and @todate+1
delete from process_data where pdate between  @fromdate and @todate+1
end
delete from masterprocessdailydata where pdate  between (select min(punchdate) from Trans_Raw#) and (select MAX(punchdate) from Trans_Raw#)
delete from overtime where otdate  between (select min(punchdate) from Trans_Raw#) and (select MAX(punchdate) from Trans_Raw#)
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
exec sp_processdailydata_rawdata

end
exec Sp_SetDownloader_DetailedMonthlyReport
insert into mastertrans_raw# (empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1) select empid,punch_time,punchdate,verification_code,cardno,deviceid,status,status1 from trans_raw# 
truncate table trans_raw#
end