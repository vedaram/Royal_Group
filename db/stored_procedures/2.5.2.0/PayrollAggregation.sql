USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[PayrollAggregation]    Script Date: 11-05-2016 01:19:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[PayrollAggregation]
(
@FromDate date,
@ToDate date,
@whereCondition NVarchar(max) = 'and 1=1'
)
as 
begin
	DECLARE @QRY VARCHAR(MAX)

	truncate table PayrollAggregationReport

	  SET @Qry=N'insert into PayrollAggregationReport
	select Emp_ID as ''EmployeeID'',Emp_Name as ''EmployeeName'', Dept_Name as ''Department'',Desig_Name as ''Designation'',
	 datename(Month, pdate) as ''Month'', datename(dw,pdate) as ''WeekDay'',PDate as ''WorkDate'',In_Punch as ''FirstIn'',Out_Punch as ''LastOut'', 
	case when WorkHrs is null then ''00:00'' else WorkHrs end as Actual_Hours, 
	CONVERT(varchar(5),convert(time(5), DATEADD(hour, DATEDIFF(hour, 0,DATEADD(minute, 30 - DATEPART(minute, CONVERT(datetime, convert(varchar(5),workhrs)) + ''00:30:00.000''),
	CONVERT(datetime, convert(varchar(5), workhrs)))), 0))) as Rounded_Hours, 
	case when CONVERT(varchar(5), convert(time, Total_Hrs)) is null then ''00:00'' else CONVERT(varchar(5), convert(time, Total_Hrs)) end as Mandatory_Hours,
	concat(
			case when datediff(minute, Total_hrs, WorkHrs) < 0  
				THEN CONCAT(''-'', CONVERT(VARCHAR, ABS(datediff(minute, Total_Hrs, WorkHRs)/60) ) ) 
				ELSE CONVERT( VARCHAR, datediff(minute, Total_Hrs, WorkHRs)/60 ) END, 
			'':'',
			case when ABS(datediff(minute, Total_Hrs, WorkHRs)%60) < 10 
			THEN CONCAT(''0'', CONVERT(varchar, ABS(datediff(minute, Total_Hrs, WorkHRs)%60))) 
			ELSE CONVERT(varchar, ABS(datediff(minute, Total_Hrs, WorkHRs)%60)) END
	) as Discrepancy,
	case status when ''A'' then ''2MP'' when ''MS'' then ''1MP'' else Status end as Status,
	convert(varchar(5),lateby) as ''LateBy'', convert(varchar(5), earlyby) as ''EarlBy'',
	case status when ''MS'' then ''1MP'' else '''' end as ''1MissingPunch'', case status when ''A'' then ''2MP'' else '''' end as ''2MissingPunch '', convert(varchar(5),OT) as ''OverTime'',
	Comp_Name
	from masterprocessdailydata where PDate between ''' + Convert(char,@FromDate) + ''' and ''' + Convert(char,@ToDate) + ''' ' + @whereCondition + ''
	
	-- in_punch is not null and //removed this condition Shift_In is not null and 

	exec(@Qry)

	update PayrollAggregationReport set Rounded_Hours=
	CONVERT(varchar(5),convert(time(5), DATEADD(hour, DATEDIFF(hour, 0,DATEADD(minute, 30 - DATEPART(minute, CONVERT(datetime, convert(varchar(5),Actual_Hours)) + '00:30:00.000'),
	CONVERT(datetime, convert(varchar(5), Actual_Hours)))), 0)))  from PayrollAggregationReport
	
	--======================================================================================================================================================================
	--Discrepancy from here to Line no: 70

	update PayrollAggregationReport set Discrepancy=((DATEDIFF(MINUTE, 0, Actual_Hours))- (DATEDIFF(MINUTE, 0, mandatory_hours)))  from PayrollAggregationReport

	Begin
		declare @TotalTimeMinute int	
		declare @DiscrepancyValue varchar(5),@EmpID varchar(100),@WorkDate datetime, @DiscrepancyHours varchar(10)
		
		set @TotalTimeMinute=0

		declare DiscrepancyCursor Cursor  for select Discrepancy,EmployeeID,WorkDate from PayrollAggregationReport

		open DiscrepancyCursor
	
		Fetch next from DiscrepancyCursor into @DiscrepancyValue,@EmpID,@WorkDate

		while @@FETCH_STATUS=0
			begin
				
				if @DiscrepancyValue like '%-%'		
					begin
						set @DiscrepancyValue =convert(int, substring(@DiscrepancyValue,2,len(@DiscrepancyValue)))
							set @DiscrepancyHours=  '-'+RIGHT('0' +  RTRIM(@DiscrepancyValue/60),2) + ':' + RIGHT('0' + RTRIM(@DiscrepancyValue%60),2)  
					end
				else
					begin
							set @DiscrepancyHours=  RIGHT('0' +  RTRIM(@DiscrepancyValue/60),2) + ':' + RIGHT('0' + RTRIM(@DiscrepancyValue%60),2)  
					end	
						
				update PayrollAggregationReport set Discrepancy='' where EmployeeID=@EmpID and WorkDate=@WorkDate
				update PayrollAggregationReport set Discrepancy=@DiscrepancyHours where EmployeeID=@EmpID and WorkDate=@WorkDate

				set @DiscrepancyValue='0'
				set @DiscrepancyValue=0
				
				Fetch next from DiscrepancyCursor into @DiscrepancyValue,@EmpID,@WorkDate
				
			end
		
		Close DiscrepancyCursor		
		Deallocate DiscrepancyCursor 
	end
	--=====

	select * from PayrollAggregationReport order by EmployeeID, EmployeeName, Department, Designation
end