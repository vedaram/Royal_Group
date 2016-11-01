USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[PayrollDiscrepancyHours]    Script Date: 11-05-2016 01:22:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  procedure [dbo].[PayrollDiscrepancyHours]
(
	@EmpID varchar(100) ,
	@DiscrepancyHours varchar(6) output
)
as
begin
	--globla variables
	declare @TempValue time
	declare @TotalTimeMinute int=0

	-------------------------------------------------------------------------------------------------------------------------------
	--Discrepancy from here to Line no: 70
	Begin
		declare @N_Time int=0, @P_Time int=0		
		declare @DiscrepancyValue varchar(5)
		
		set @TempValue='00:00'
		set @TotalTimeMinute=0

		declare DiscrepancyCursor Cursor  for select Discrepancy from PayrollAggregationReport where EmployeeId=@EmpID

		open DiscrepancyCursor
	
		Fetch next from DiscrepancyCursor into @DiscrepancyValue

		while @@FETCH_STATUS=0
			begin
				if @DiscrepancyValue like ':'
					begin
						set @TempValue='00:00'
						Fetch next from DiscrepancyCursor into @DiscrepancyValue
					end
				else
					begin
						if @DiscrepancyValue like '%-%'		
							begin
								set @TempValue =substring(@DiscrepancyValue,2,len(@DiscrepancyValue))
								set @N_Time =@N_Time+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 
						
							end
						else
							begin
								set @TempValue =@DiscrepancyValue
								set @P_Time =@P_Time+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 
							end	

						Fetch next from DiscrepancyCursor into @DiscrepancyValue
					end
			end
		
		if @N_Time<@P_Time
			begin
				set @TotalTimeMinute=@P_Time-@N_Time
				set @DiscrepancyHours= RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2)  
			end
		else
			begin
				set @TotalTimeMinute=@N_Time-@P_Time
				 set @DiscrepancyHours=  '-'+RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2)  
			end

		select 	@DiscrepancyHours as 'DiscrepancyHours'

		Close DiscrepancyCursor		
		Deallocate DiscrepancyCursor 
	end
	-------------------------------------------------------------------------------------------------------------------------------
	
	-------------------------------------------------------------------------------------------------------------------------------
	--Actual_Hours from here to Line no: 101
	Begin

		set @TempValue='00:00'
		set @TotalTimeMinute=0

		declare @Actual_Hours varchar(10)
	
		declare Actual_HoursCursor Cursor   for select Actual_Hours from PayrollAggregationReport where EmployeeId=@EmpID

		open Actual_HoursCursor
	
		Fetch next from Actual_HoursCursor into @TempValue

		while @@FETCH_STATUS=0
			begin					
				
				set @TotalTimeMinute=@TotalTimeMinute+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 						
			
				Fetch next from Actual_HoursCursor into @TempValue
			
			end		
		
		set @Actual_Hours= RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2) 
	
		select 	@Actual_Hours as 'Actual_Hours'

		Close Actual_HoursCursor
		Deallocate Actual_HoursCursor 		
	end
	-------------------------------------------------------------------------------------------------------------------------------

	-------------------------------------------------------------------------------------------------------------------------------
	--Rounded_Hours from here to Line no: 131
	Begin

			set @TempValue='00:00'
		set @TotalTimeMinute=0

		declare @Rounded_Hours varchar(10)
	
		declare Rounded_HoursCursor Cursor   for select Rounded_Hours from PayrollAggregationReport where EmployeeId=@EmpID

		open Rounded_HoursCursor
	
		Fetch next from Rounded_HoursCursor into @TempValue

		while @@FETCH_STATUS=0
			begin					
				
				set @TotalTimeMinute=@TotalTimeMinute+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 						
			
				Fetch next from Rounded_HoursCursor into @TempValue
			
			end		
		
		set @Rounded_Hours= RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2) 
	
		select 	@Rounded_Hours as 'Rounded_Hours'

		Close Rounded_HoursCursor
		Deallocate Rounded_HoursCursor
	end
	-------------------------------------------------------------------------------------------------------------------------------

	-------------------------------------------------------------------------------------------------------------------------------
	--Mandatory_Hours from here to Line no: 161
	Begin

			set @TempValue='00:00'
		set @TotalTimeMinute=0

		declare @Mandatory_Hours varchar(10)
	
		declare Mandatory_HoursCursor Cursor   for select Mandatory_Hours from PayrollAggregationReport where EmployeeId=@EmpID

		open Mandatory_HoursCursor
	
		Fetch next from Mandatory_HoursCursor into @TempValue

		while @@FETCH_STATUS=0
			begin					
				
				set @TotalTimeMinute=@TotalTimeMinute+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 						
			
				Fetch next from Mandatory_HoursCursor into @TempValue
			
			end		
		
		set @Mandatory_Hours= RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2) 
	
		select 	@Mandatory_Hours as 'Mandatory_Hours'

		Close Mandatory_HoursCursor
		Deallocate Mandatory_HoursCursor
	end
	-------------------------------------------------------------------------------------------------------------------------------

	-------------------------------------------------------------------------------------------------------------------------------
	--LateBy from here to Line no: 191
	Begin

			set @TempValue='00:00'
		set @TotalTimeMinute=0

		declare @LateBy_Hours varchar(10)
	
		declare LateBy_HoursCursor Cursor   for select LateBy from PayrollAggregationReport where EmployeeId=@EmpID and Lateby is not null

		open LateBy_HoursCursor
	
		Fetch next from LateBy_HoursCursor into @TempValue

		while @@FETCH_STATUS=0
			begin					
				
				set @TotalTimeMinute=@TotalTimeMinute+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 						
			
				Fetch next from LateBy_HoursCursor into @TempValue
			
			end		
		
		set @LateBy_Hours= RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2) 
	
		select 	@LateBy_Hours as 'LateBy'

		Close LateBy_HoursCursor
		Deallocate LateBy_HoursCursor
	end
	-------------------------------------------------------------------------------------------------------------------------------

	-------------------------------------------------------------------------------------------------------------------------------
	--EarlBy from here to Line no: 221
	Begin
			set @TempValue='00:00'
		set @TotalTimeMinute=0

		declare @EarlBy_Hours varchar(10)
	
		declare EarlBy_HoursCursor Cursor   for select EarlBy from PayrollAggregationReport where EmployeeId=@EmpID and EarlBy is not null

		open EarlBy_HoursCursor
	
		Fetch next from EarlBy_HoursCursor into @TempValue

		while @@FETCH_STATUS=0
			begin					
				
				set @TotalTimeMinute=@TotalTimeMinute+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 						
			
				Fetch next from EarlBy_HoursCursor into @TempValue
			
			end		
		
		set @EarlBy_Hours= RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2) 
	
		select 	@EarlBy_Hours as 'EarlBy'

		Close EarlBy_HoursCursor
		Deallocate EarlBy_HoursCursor
	end
	-------------------------------------------------------------------------------------------------------------------------------

	-------------------------------------------------------------------------------------------------------------------------------
	--OverTime from here to Line no: 270
	Begin
		set @TempValue='00:00'
		set @TotalTimeMinute=0
		declare @OverTime_Hours varchar(10)
	
		declare OverTime_HoursCursor Cursor   for select OverTime from PayrollAggregationReport where EmployeeId=@EmpID and OverTime is not null

		open OverTime_HoursCursor
	
		Fetch next from OverTime_HoursCursor into @TempValue

		while @@FETCH_STATUS=0
			begin					
				
				set @TotalTimeMinute=@TotalTimeMinute+ LTRIM(DATEDIFF(MINUTE, 0, @TempValue)) 						
			
				Fetch next from OverTime_HoursCursor into @TempValue
			
			end		
		
		set @OverTime_Hours= RIGHT('0' +  RTRIM(@TotalTimeMinute/60),2) + ':' + RIGHT('0' + RTRIM(@TotalTimeMinute%60),2) 
	
		select 	@OverTime_Hours as 'OverTime'

		Close OverTime_HoursCursor
		Deallocate OverTime_HoursCursor
	end
	-------------------------------------------------------------------------------------------------------------------------------

	select count(Status) as 'Status' from PayrollAggregationReport where EmployeeId=@EmpID
	select count(OneMissingPunch) as '1MP' from PayrollAggregationReport where EmployeeId= @EmpID and OneMissingPunch is not null and OneMissingPunch!=''
	select count(TwoMissingPunch) as '2MP' from PayrollAggregationReport where EmployeeId= @EmpID and TwoMissingPunch is not null and TwoMissingPunch!=''
end
