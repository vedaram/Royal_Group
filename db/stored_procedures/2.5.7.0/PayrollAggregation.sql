

ALTER procedure [dbo].[PayrollAggregation]
(
@CompanyName varchar(30),
@FromDate date,
@ToDate date,
@whereCondition NVarchar(max) = 'and 1=1'
)
as 
begin
	DECLARE @QRY VARCHAR(MAX),@companycode varchar(20) , @FixedBreakDeductionMinute int 
	select @companycode = companycode from companymaster where companyname = @CompanyName
	select @FixedBreakDeductionMinute = (datepart(HOUR,breakhrs)*60)+datepart(MINUTE,breakhrs)   from ShiftSetting where CompanyCode = @companycode
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
	Comp_Name , cat_name
	from masterprocessdailydata where PDate between ''' + Convert(char,@FromDate) + ''' and ''' + Convert(char,@ToDate) + ''' ' + @whereCondition + ''
	
	-- in_punch is not null and //removed this condition Shift_In is not null and 

	exec(@Qry)

	update PayrollAggregationReport set Rounded_Hours=
	CONVERT(varchar(5),convert(time(5), DATEADD(hour, DATEDIFF(hour, 0,DATEADD(minute, 30 - DATEPART(minute, CONVERT(datetime, convert(varchar(5),Actual_Hours)) + '00:30:00.000'),
	CONVERT(datetime, convert(varchar(5), Actual_Hours)))), 0)))  from PayrollAggregationReport
	
	--==================  Update mandatory  hours  as total hours - break hours --- 
	update PayrollAggregationReport set Mandatory_Hours = CONVERT(varchar(5)  , dateadd(MINUTE , - @FixedBreakDeductionMinute, convert(time, Mandatory_Hours))) 
	where Mandatory_Hours is not null and  Mandatory_Hours != '00:00'
	
	--====================================
	
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

	--  For leave Name updation i.e. when sttaus is L it has to show which leave a person applied 
	Begin
		
		declare @leavename varchar(50),@EmpID1 varchar(100),@WorkDate1 datetime, @leavetype varchar(10),@status varchar(10)
		
		

		declare leavename Cursor  for select status,EmployeeID,WorkDate from PayrollAggregationReport where status ='L' --IN ( select status from leavemaster where companycode = @companycode)

		open leavename
	
		Fetch next from leavename into @status,@EmpID1,@WorkDate1

		while @@FETCH_STATUS=0
			begin
				select  @leavetype =  leavetype from leave1 where @WorkDate1 between startdate and enddate and empid  = @EmpID1
				select  @leavename =  leavename from leavemaster where leavecode = @leavetype
						
				update PayrollAggregationReport set status='' where EmployeeID=@EmpID1 and WorkDate=@WorkDate1
				update PayrollAggregationReport set status =@leavename where EmployeeID=@EmpID1 and WorkDate=@WorkDate1

				
				
				Fetch next from leavename into @status,@EmpID1,@WorkDate1
				
			end
		
		Close leavename		
		Deallocate leavename 
	end

	--=======================================================================================================
	--- ===================== updating only approved OT from overtime table 
	update PayrollAggregationReport  set overtime  = null 
	 UPDATE P SET overtime = case when  OT.flag=2 then OT.othrs else null END 
	FROM  OverTime OT inner join  PayrollAggregationReport P on  OT.EMPID = P.EmployeeID and OT.OTDate = P.WorkDate and OT.flag is not null
	-----========================================================================================================
		
		

	--select * from PayrollAggregationReport order by cast(EmployeeID as int), EmployeeName, Department, Designation
	--select * from PayrollAggregationReport order by Department, Designation ,cast(EmployeeID as int)
	select * from PayrollAggregationReport order by Department , Designation,  case IsNumeric(EmployeeID) when 1 then Replicate('0', 100 - Len(EmployeeID)) + EmployeeID else EmployeeID end
    
	--select * from PayrollAggregationReport order by Department, Designation , EmployeeID

end