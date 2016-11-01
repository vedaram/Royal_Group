GO
/****** Object:  StoredProcedure [dbo].[Sp_ShiftAssign]    Script Date: 4/29/2016 8:03:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[Sp_ShiftAssign]
as
Begin
	Declare @CountEmpID as int,
	@CountPunchDate int,
	@CountPunchDateTEmp int,
	@PunchDateIndex int,
	@EmpIDIndex int,
	@ReadEmpID varchar(max),
	@InPunch datetime ,
	@ReadPDate datetime,
	@ReadPDateyes datetime,
	@tempReadPDate datetime,
	@maxtime datetime,
	@maxtimeTemp datetime,
	@ShiftCode varchar(Max),
	@month  int,@year  int,@day  int,@SqlQuery varchar(max),@daytemp varchar(max),
	@dayofweek varchar(max),
	@nweeksat datetime,
	@scount int,
	@outpunch datetime,
	@starttime1 Bigint,
	@starttime2 Bigint,
	@starttimeDateTime datetime,
	@aso varchar(5),
	@saso varchar(5),
	@scode varchar(max),
	@s nvarchar(4000),
	@ldate datetime,
	@spassigncount int,
	@SIn datetime,
	@SOut datetime ,
	@spinitialcount int,
	@shiftin datetime,
	@shiftout datetime,@monthp  int,@yearp  int,@dayp  int,
	@compid varchar(max),
	@InPunchInTime time, 
    @InTimeTemp  varchar(20),
    @MinShiftIn time,
    @MaxShiftIn time,				
	@DateUpdate date,
	@shiftCodeMin varchar(20),
	@shiftCodeMax varchar(20),
	@isramadan varchar(1),
	@ramadanstdt datetime,
	@ramadantodt datetime,
	@ramadanflag varchar(1),
	@qry Nvarchar(4000),
	@dayCheckChar char(6),
	@dayCheck int,
	@FrmDate1 Datetime,
	@ToDate1 Datetime,
	@loopFlag int
	

	

	set @EmpIDIndex = 1        
	set @PunchDateIndex = 1 
	   
	Truncate Table StoreEmpID
	Truncate Table PDateDetails 
	insert into StoreEmpID(EID) select Emp_Code from EmployeeMaster where Emp_Status=1 order by Emp_Code asc---insertin the employee details order by empid in ascending order        
	--insert into StoreEmpID(EID) values('0611')	
	   
	select @CountEmpID = COUNT(EID) from StoreEmpID   
	select top 1 @ldate= PunchDate from Trans_Raw# order by PunchDate desc
		
	select TOP 1 @FrmDate1 = PunchDate from Trans_Raw# order by PunchDate 
	select TOP 1 @ToDate1 = PunchDate from Trans_Raw# order by PunchDate desc
	 
	--insert into PDateDetails(PunchDate1) select Distinct PunchDate from Trans_Raw# order by PunchDate  Asc
	
	While (DATEDIFF(dd,@FrmDate1,@ToDate1) >= 0)
	begin
		insert into PDateDetails (PunchDate1) values (@FrmDate1)
		set @FrmDate1 = DATEADD(dd,1,@FrmDate1) 
	end 

	select @CountPunchDate =COUNT(PunchDate1) from PDateDetails
	   
	----Ramadan shift code start
	set @isramadan = null
	select @isramadan=isramadan from ShiftSettingtemp
	if(@isramadan='1')
	begin
		select @ramadanstdt=ramadanstdt,@ramadantodt=ramadantodt from ShiftSettingtemp
		if exists(select 1 from ShiftSettingtemp where @ReadPDate between @ramadanstdt and @ramadantodt)
		begin
			select @maxtime=convert(datetime,convert(time,Ramadan_OutTime)) + convert(datetime,max_overtime) from shift where convert(time,Ramadan_OutTime) in(select min(convert(time,Ramadan_OutTime)) from shift)
			set @ramadanflag='1'
		end
		else
		begin
			select @maxtime=convert(datetime,convert(time,Out_Time)) + convert(datetime,max_overtime) from shift where convert(time,Out_Time) in(select min(convert(time,Out_Time)) from shift)
			set @ramadanflag='0'
		end
	end
	else
	begin                   
		select @maxtime=convert(datetime,convert(time,Out_Time)) + convert(datetime,max_overtime) from shift where convert(time,Out_Time) in(select min(convert(time,Out_Time)) from shift)
		set @ramadanflag='0'
	end        
	----Ramadan shift code ends   
	   
	--select @maxtime=convert(datetime,convert(time,Out_Time)) + convert(datetime,max_overtime) from shift where convert(time,Out_Time) in(select min(convert(time,Out_Time)) from shift)
	   
	While(@EmpIDIndex <= @CountEmpID)---Employeeid counter loop        
	Begin
		set @ReadEmpID = NULL  
		set @scode=null
		set @compid =null
		set @loopFlag=0
		select @ReadEmpID = EID from StoreEmpID where Id =@EmpIDIndex
		select @compid=emp_company from EmployeeMaster where Emp_Code=@ReadEmpID
		--select @aso=isaso from EmployeeMaster  where Emp_Code=@ReadEmpID  
			
		While(@PunchDateIndex <= @CountPunchDate)---Punchdate counter loop        
		Begin 
			set @InPunch=Null
			set @ReadPDate = NUll  
			set @scount=null  
			set @starttime1=null
			set @starttime2=null    
			set @saso=null
			set @nweeksat=null
			set @scode=null
			set @shiftin=null
			set @shiftout=null			
			set @ReadPDateyes=null
			set @maxtime=null
			set @MinShiftIn=null
			set @MaxShiftIn=null
			set @DateUpdate=null
			set @SIn =null
			set	@SOut =null
			set @DateUpdate=null
			set @ShiftCode=null
			set @MaxtimeTemp=null
			set @outpunch=null
			set @InPunchInTime=null
			set @dayCheckChar=null
			set @dayCheck=null
			
			
			
			select @ReadPDate = PunchDate1 from PDateDetails where Id = @PunchDateIndex
	             
			set @month =DATEPART(MM, @ReadPDate)
			set @year=DATEPART(YYYY, @ReadPDate)   
			set @day=DATEPART(DD, @ReadPDate) 
			set @daytemp = 'day' 
				 
	                                    			
			if exists(select 1 from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=@ReadPDate)
			begin	   
				set @ReadPDateyes= DATEADD(dd,-1,@ReadPDate)
				set @monthp =	   DATEPART(MM, @ReadPDateyes)
				set @yearp=		   DATEPART(YYYY, @ReadPDateyes)   
				set @dayp=		   DATEPART(DD, @ReadPDateyes) 
				   
				set @s = N'select @var = day'+convert(varchar,@dayp)+' From shiftemployee where Empid='''+ @ReadEmpID + ''' and Month = ' + convert(char,@monthp) + ' and year = ' + convert(char,@yearp)+''
				execute sp_executesql @s, N'@var varchar(max) output', @var = @scode output
					
				if (@ramadanflag='0')
					begin
						select @shiftin=convert(time,In_Time),@shiftout=convert(time,Out_Time) from shift where Shift_Code=@scode and CompanyCode=@compid
					end
				else
					begin
						select @shiftin=convert(time,Ramadan_InTime),@shiftout=convert(time,Ramadan_OutTime) from shift where Shift_Code=@scode and CompanyCode=@compid
					end				
	               
				if(@shiftin>@shiftout)
				begin
					if (@ramadanflag ='0')
						begin
							select @maxtime=convert(time,out_time+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @scode and CompanyCode=@compid
						end
					else
						begin
							select @maxtime=convert(time,Ramadan_OutTime+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @scode and CompanyCode=@compid
						end
					select top 1 @InPunch=Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=@ReadPDate and convert(time,Punch_Time)>CONVERT(time,@maxtime) order by Punch_Time asc
				end
				else
				begin
					select top 1 @InPunch=Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=@ReadPDate order by Punch_Time asc
				end
				if((@InPunch is not null)and(@InPunch !=''))
				begin
					--select top 1 @ShiftCode=Shift_Code from shift order by ABS(DATEDIFF(minute,CONVERT(time,In_Time),CONVERT(time,@InPunch)))
					
		  --Changes by vinay starts
						   
					select top 1 @MinShiftIn= In_Time,@shiftCodeMin=Shift_Code from shift  where  CompanyCode=@compid  order by Convert(time,In_Time) asc
					select top 1 @MaxShiftIn= In_Time,@shiftCodeMax=Shift_Code from shift where  CompanyCode=@compid  order by Convert(time,In_Time) desc
		
					if (@ramadanflag ='0')
					   begin	
						  set @InTimeTemp='In_Time'	
					   end
					else
					  begin
						  set @InTimeTemp= 'Ramadan_InTime'						
					  end
				     
				     set @InPunchInTime=@InPunch
		--=================================================================================================================================================		     
		--This block will work and find shift_code and date to be updated in shift roster
						   
				       if((@InPunchInTime >= @MinShiftIn) and ( @InPunchInTime <= @MaxShiftIn))
						     Begin
									set @qry= N'select top 1 @var1=Shift_Code ,@starttime=In_time from shift where  CompanyCode='''+@compid+''' order by ABS(DATEDIFF(minute,CONVERT(time,In_Time),CONVERT(time,'''+Convert(char,@InPunch)+''')))'--works for both ramadhan and normal shift timings
									execute sp_executesql @qry, N'@var1 varchar(max) output,@starttime datetime output', @var1 = @ShiftCode output,@starttime=@starttimeDateTime output									
									set @DateUpdate=@ReadPDate									
						     End
						   
				      Else if(@InPunchInTime < @MinShiftIn)
						Begin
							select  @starttime1= ABS(DATEDIFF(minute,DATEADD(dd,-1,@ReadPDate)+@MaxShiftIn,CONVERT(datetime,@InPunch)))
							select  @starttime2= ABS(DATEDIFF(minute,@ReadPDate+@MinShiftIn,CONVERT(datetime,@InPunch)))
							
							if(@starttime1<@starttime2)
								Begin
									set @DateUpdate=DATEADD(dd,-1,@ReadPDate)
									set @ShiftCode=@shiftCodeMax
								End
							Else
								Begin									
									set @DateUpdate=@ReadPDate
									set @ShiftCode=@shiftCodeMin
								End							
						End
						
				    Else if(@InPunchInTime > @MaxShiftIn)
					   Begin
							select  @starttime1=  ABS(DATEDIFF(minute,@ReadPDate+@MaxShiftIn,CONVERT(datetime,@InPunch)))
							select  @starttime2=  ABS(DATEDIFF(minute,DATEADD(dd,1,@ReadPDate)+@MinShiftIn,CONVERT(datetime,@InPunch)))
							   
							    if(@starttime1<@starttime2)
								    Begin
										set @DateUpdate=@ReadPDate
										set @ShiftCode=@shiftCodeMax
								    End
								Else
									Begin									
										set @DateUpdate=DATEADD(dd,1,@ReadPDate)
										set @ShiftCode=@shiftCodeMin
									End
					   End
				--=================================================================================================================================================	   
				    
				---------------------------------------------------------------------------------------------------------------------------------------
				----This block will work and find shift_code if shift_in time is same for more than one shift
				
					 select @scount= COUNT(*) from shift  where  CompanyCode=@compid and Convert(time,In_Time)=convert(time,(select In_Time from shift where shift_code=@ShiftCode))--Finding more shifts starting with same time
				     select @SIn= In_Time,@SOut=Out_Time from shift  where  CompanyCode=@compid and Shift_Code=@ShiftCode				  
					
					if(@scount>=2)
					begin
						if(@SIn>@SOut)--Night shift
							Begin					
								if (@ramadanflag = '0')
									begin
										select top 1 @MaxtimeTemp=MAX(convert(time,Out_Time+CONVERT(datetime,max_overtime))) from shift where  CompanyCode=@compid  and Convert(time,In_Time)=Convert(time,@SIn)								
										select top 1 @outpunch=Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and (PunchDate= @DateUpdate or  PunchDate=DATEADD(day,1,@DateUpdate)) and Punch_Time <= DateAdd(day,1,@DateUpdate)+@MaxtimeTemp and Punch_Time > @InPunch order by Punch_Time desc									
										select top 1 @ShiftCode=Shift_Code from shift where  CompanyCode=@compid and CONVERT(time,In_Time)=convert(time,@SIn) order by ABS(DATEDIFF(minute,CONVERT(time,Out_Time),CONVERT(time,@outpunch)))
									end
								else
									begin
										select top 1 @MaxtimeTemp=MAX(convert(time,Ramadan_OutTime+CONVERT(datetime,max_overtime))) from shift where  CompanyCode=@compid  and Convert(time,In_Time)=Convert(time,@SIn)								
										select top 1 @outpunch=Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and (PunchDate= @DateUpdate or  PunchDate=DATEADD(day,1,@DateUpdate)) and Punch_Time <=  Dateadd(day,1,@DateUpdate)+@MaxtimeTemp and Punch_Time > @InPunch  order by Punch_Time desc									
										select top 1 @ShiftCode=Shift_Code from shift where  CompanyCode=@compid and CONVERT(time,Ramadan_InTime)=convert(time,@SIn) order by ABS(DATEDIFF(minute,CONVERT(time,Ramadan_OutTime),CONVERT(time,@outpunch)))
									end	
							End
						Else--DayShift
							Begin
								select top 1 @outpunch=Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=@ReadPDate and Punch_Time > @InPunch  order by Punch_Time desc
								
								if (@ramadanflag = '0')
									begin
										select top 1 @ShiftCode=Shift_Code from shift where  CompanyCode=@compid and CONVERT(time,In_Time)=convert(time,@SIn) order by ABS(DATEDIFF(minute,CONVERT(time,Out_Time),CONVERT(time,@outpunch)))
									end
								else
									begin
										select top 1 @ShiftCode=Shift_Code from shift where  CompanyCode=@compid and CONVERT(time,Ramadan_InTime)=convert(time,@SIn) order by ABS(DATEDIFF(minute,CONVERT(time,Ramadan_OutTime),CONVERT(time,@outpunch)))
									end	
							End	
									
					end
			    
			---------------------------------------------------------------------------------------------------------------------------------------        		    
					if exists(select 1 from ShiftEmployee where Empid=@ReadEmpID and Month=DatePart(MONTH,@DateUpdate) and year=DatePart(YEAR,@DateUpdate))--------Integrating with ShiftRoster
						begin
								if(Convert(datetime,@DateUpdate)<@ReadPDate )--Here @DateUpdate contains previous day's date
									Begin
				  						set @dayCheckChar= 'day'+convert(char,DatePart(DAY,@DateUpdate))						
				  						set @s = N'select @var3 = 1 From shiftemployee where Empid='''+ @ReadEmpID + ''' and Month = '''+Convert(char,DatePart(MONTH,@DateUpdate))+''' and year = '''+Convert(char,DatePart(YEAR,@DateUpdate))+''' and '+@dayCheckChar+' is null or '+@dayCheckChar+' ='''' '
										execute sp_executesql @s, N'@var3 int output', @var3 = @dayCheck output	
										if(@dayCheck  = 1 )
											Begin
													set @SqlQuery='update ShiftEmployee set day'+convert(char,DatePart(Day,@DateUpdate))+'='''+@ShiftCode+''' where Empid=''' + @ReadEmpID + ''' and Month = '+convert(char,Datepart(MONTH,@DateUpdate))+'and year = ' +convert(char,Datepart(YEAR,@DateUpdate))+'';
													execute(@SqlQuery)
											End									
									End
								Else
									Begin
										set @SqlQuery='update ShiftEmployee set day'+convert(char,DatePart(Day,@DateUpdate))+'='''+@ShiftCode+''' where Empid=''' + @ReadEmpID + ''' and Month = '+convert(char,Datepart(MONTH,@DateUpdate))+'and year = ' +convert(char,Datepart(YEAR,@DateUpdate))+'';
										execute(@SqlQuery)
									End
						end 
					else 
						begin
							set @SqlQuery='insert into ShiftEmployee(Empid,Month,year) values ('''+@ReadEmpID+''','+convert(char,Datepart(MONTH,@DateUpdate))+','+convert(char,Datepart(YEAR,@DateUpdate))+')';
							execute(@SqlQuery)
							set @SqlQuery='update ShiftEmployee set day'+convert(char,Datepart(DAY,@DateUpdate))+'='''+@ShiftCode+''' where Empid=''' + @ReadEmpID + ''' and Month = ' +convert(char,Datepart(MONTH,@DateUpdate))+'and year = ' +convert(char,Datepart(YEAR,@DateUpdate))+'';
							execute(@SqlQuery)
						end                    
				end
				
			end	 
			if(Convert(datetime,@DateUpdate)<@ReadPDate )--loop should not work more than twice for same date so this block is used
				Begin
					set @PunchDateIndex = @PunchDateIndex
					set @loopFlag=@loopFlag+1 
					if(@loopFlag=2)
						Begin
							set @PunchDateIndex = @PunchDateIndex + 1
							set @loopFlag=0  
						End
				End   
			Else
				Begin
					set @PunchDateIndex = @PunchDateIndex + 1
					set @loopFlag=0  					 
			    End	 
		end    --Changes by vinay Ends
		set @PunchDateIndex = 1    
		set @EmpIDIndex = @EmpIDIndex + 1        
	end
end
