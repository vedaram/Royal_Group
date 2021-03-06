USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[PayRollLinkReportFilter]    Script Date: 06/22/2016 18:55:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[PayRollLinkReportFilter]
(@fdate datetime,@tdate datetime)
as
begin
declare--MODIFIED ON 11042014-1538
@EmpId varchar(max),
@EmpName varchar(max),
@comp varchar(max),
@branch varchar(max),
@dept varchar(max),
@shift varchar(max),
@category varchar(max),
@TotalP decimal(5,1),
@TotalA decimal(5,1),
@TotalAHP DECIMAL(5,1),
@TotalH int,
@TotalL decimal(5,1),
@TotA decimal(5,1),
@TotP decimal(5,1),
@TotalAHL decimal(5,1),
@TotalPHL decimal(5,1),
@Halfday decimal(5,1),
@TotalLV decimal(5,1),
@ToalWo int,
@TotalMs int,
@TotalOt varchar(max),
@FromDate datetime,@Todate datetime,@BetweenDate datetime,@CheckDate datetime,
@index int,
@count int,
@inp int,
@oup int,
@mon varchar(max),
@yr int,
@monyr varchar(max)

--  SELECT  @ToDate =   GETDATE()
--select @BetweenDate = DATEADD(MM,-1,@ToDate)
--  select @FromDate = DATEADD(m,DATEDIFF(m,0,@BetweenDate),0)
--SELECT @ToDate = DATEADD(month, ((YEAR(@BetweenDate) - 1900) * 12) + MONTH(@BetweenDate), -1) 
Declare @Temp_EID as Table        
(                  
  id int identity(1,1),                  
  EID varchar(Max)                  
)
truncate table PayRolllinkreport
insert into @Temp_EID(EID) select distinct Emp_Id from MASTERPROCESSDAILYDATA where PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120)
select @count=COUNT(EID) from @Temp_EID 
set @index=1
While(@index <= @count) 
begin
 select @EmpId = EID from @Temp_EID where id = @index
  select @EmpName = Emp_Name from MASTERPROCESSDAILYDATA where Emp_ID = @EmpId
  Select @comp= Comp_Name from MASTERPROCESSDAILYDATA where Emp_ID =@EmpId
  Select @branch=Cat_Name from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId
  select @dept= Dept_Name from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId
  select @category= EmployeeCategory from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and pdate = @fdate
   
  select @shift= Shift_Name from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId 
  select @TotalP=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120) and (Status='P' or Status='OD' or Status='MI' or Status='MO' or Status='M' or Status='WOP' or Status='HP' or Status='MS')
  select @TotalA=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120) and Status='A'
  select @TotalH=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120) and Status='H'
  select @TotalL=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120) and (Status in(select status from leavemaster where status is not null and status !='') or Status='V' or Status='CO')
  select @ToalWo=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120) and (Status='WO')
  select @TotalAHL=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120)and Status='AHL'
  select @TotalPHL=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120)and Status='PHL'
  select @TotalAHP=COUNT(status) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and PDate between @FromDate and @todate and (Status='AHP')
  select @inp=COUNT(In_Punch) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and Out_Punch is null and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120)
  select @oup=COUNT(Out_Punch) from MASTERPROCESSDAILYDATA where Emp_ID=@EmpId and In_Punch is null and PDate between convert(datetime, @fdate, 120) and convert(datetime, @tdate, 120)
  set @TotalMs=SUM(@inp+@oup)
  set @Halfday=(@TotalAHL/2)+(@TotalPHL/2)
  set @TotP=@TotalP+(@TotalPHL/2)+(@TotalAHP/2)
  set @TotA=@TotalA+(@TotalAHL/2)+(@TotalAHP/2)
  set @TotalLV=SUM(@Halfday+@TotalL)
  select @mon=DATENAME(mm,@fdate)
  select @yr=DATEPART(yy,@tdate)
  Select @monyr=@mon+'-'+CONVERT(varchar,@yr)  
  set @CheckDate = @fdate
  select @TotalOt=OverTime from DetailedMonthlyReport where Emp_ID=@EmpId and CheckDate=@CheckDate
  if(@TotalOt is null or @TotalOt='')
  begin
      select @TotalOt=SOT from DetailedMonthlyReport where Emp_ID=@EmpId and CheckDate=@CheckDate
  end
  INSERT INTO PayrollLinkReport (CheckDate,EmpId, EmpName,CompName,BranchName,DeptName,ShiftName,TotalP,TotalA,TotalHoliday,Totalleave,TotalWeekoff,TotalMs,TotalOT,mDate)VALUES
  (@CheckDate,@EmpId, @EmpName,@comp,@branch,@dept,@category,@TotP,@TotA,@TotalH,@TotalLV,@ToalWo,@TotalMs,@TotalOt,@monyr) 
   set @index=@index+1    
end


end
