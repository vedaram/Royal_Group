GO
/****** Object:  StoredProcedure [dbo].[sp_Rawdata]    Script Date: 4/29/2016 8:04:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_Rawdata]
as
	begin
	Declare @CountEmpID as int,
	@CountPunchDate int,
	@PunchDateIndex int,
	@EmpIDIndex int,
	@ReadEmpID varchar(max),
	@ReadPDate datetime, @month as int,@year as int,@day as int,@daytemp varchar(max),@SqlQuery varchar(max),@ShiftCode varchar(Max),
	@ShiftIN datetime, 
	@ShiftOUTyesterday datetime,
	@ShiftCodeyesterday  varchar(Max),                
	@ShiftOUT datetime ,
	@InPunch datetime,
	@OutPunch datetime,        
	@InPunch1007 datetime,
	@OutPunch1007 datetime,
	@TotalHrs varchar(Max),        
	@TotalHrsMin int,
	@TotalHrsMin1 int,
	@TotalHrsforday varchar(max),
	@TotalHrsfordayinmin int,
	@pid as int,
	@tempin datetime,
	@tempout datetime,
	@ncount int,
	@value int,
	@forinpunch int,
	@firstdate datetime,
	@processed int,

	@fromdate datetime,
	@todate datetime,
	@punchdate datetime,
	@isaso varchar(5),
	@cicount int,
	@ctcount int,

	@isramadan varchar(1),
	@ramadanstdt datetime,
	@ramadantodt datetime,
	@ramadanflag varchar(1),
	@re_flag int,
	
	@FrmDate1 datetime,
	@ToDAte1 datetime,

	 @MinTransDate datetime,
	 @MaxTransDate datetime
	 
	declare @empid as varchar(10),@count as int,@index as int,@fholiday as datetime,@tholiday as datetime,@lcount int,
		 @lindex as int,@lfrom as datetime,@lto as datetime,@ldate as datetime,@piInPunch as varchar(20),@piOutPunch as varchar(20),
		 @piWorkDate as datetime, @intime as datetime,@outtime1 as datetime,@EarlyLeavers as int,@Earlyleaversformat as varchar(max),
		 @nOutPunch as datetime,@nshiftmax as datetime,@temphrs as datetime,@punchcountaday as int,@maxtime as datetime,@maxtimeyesterday as datetime,@ShiftINyesterday as datetime,
		 @nightpunctcount as int,@np12count as int,@readpdateyes as datetime,@ROWCOUNT as int,@i as int,@j as int,@rowindex as int    ,
		 @fromdate1 datetime,@fromdate2 datetime,@fromdate3 datetime

		set @EmpIDIndex=1
		set @PunchDateIndex =1
		set @processed =0
		set @cicount=1


	truncate table Trans_RawProcessDailyData;

	---------------------------------------------------------------------------------------------------------------------------------------------------------------
	--If received Punches older punches then we will take old pucnhes from mastertrans_raw#
	select TOP 1 @fromdate1 = PunchDate from MasterTrans_Raw# order by PunchDate desc---Madhu Changes on 12th July 2013 start
	select TOP 1 @fromdate2 = PunchDate from Trans_Raw# order by PunchDate

	if(@fromdate2<=@fromdate1)
	begin
		select TOP 1 @fromdate3 = PunchDate from Trans_Raw# order by PunchDate desc
		select @re_flag = re_flag from ReprocessFlag-- start changes found by Mahantesh on 21082013 for reprocessing --during Blend Gourmet testing
		if(@re_flag is null)
			begin
				set @re_flag=0
			end
		if(@re_flag!=1)
		Begin
			insert into Trans_Raw#(EmpId,PunchDate,Punch_Time,CardNo,Deviceid,Verification_Code) select EmpId,PunchDate,Punch_Time,CardNo,Deviceid,Verification_Code from MasterTrans_Raw# where PunchDate between @fromdate2 and @fromdate3
			--insert into Trans_Raw#(EmpId,PunchDate,Punch_Time,CardNo,Deviceid,Verification_Code) select EmpId,PunchDate,Punch_Time,CardNo,Deviceid,Verification_Code from MasterTrans_Raw# where PunchDate between @fromdate2 and @fromdate3 and EmpId in(select distinct(EmpId) from Trans_Raw#)
		End
		--end
		----insert into Trans_Raw#(EmpId,PunchDate,Punch_Time,CardNo,Deviceid,Verification_Code) select EmpId,PunchDate,Punch_Time,CardNo,Deviceid,Verification_Code from MasterTrans_Raw# where PunchDate between @fromdate2 and @fromdate3
		delete from MasterTrans_Raw# where PunchDate between @fromdate2 and @fromdate3 and EmpId in(select distinct(EmpId) from Trans_Raw#)
		delete from process_data where pdate between @fromdate2 and @fromdate3 and EmpId in(select distinct(EmpId) from Trans_Raw#)

	end---Madhu Changes on 12th July 2013 Ends
	---------------------------------------------------------------------------------------------------------------------------------------------------------------

	-----------------------------------------------------Query Started here To Delete Duplicate Record from Trans_raw# Table---------------------------------
	;WITH CTE (EmpID, Punch_Time, DuplicateCount)
	AS
	(
		SELECT EmpID, Punch_Time,
		ROW_NUMBER() OVER(PARTITION BY EmpID, Punch_Time ORDER BY Punch_Time) AS DuplicateCount
		FROM Trans_raw#
	)
	DELETE
	FROM CTE
	WHERE DuplicateCount > 1
    ------------------------------------------------------Query Ended here to Delete Duplicate Record from Trans_raw#-----------------------------------------

	

	-----auto shift-------
	   select @isaso=1 from shiftsetting where isaso=1
	   if(@isaso=1)
	   begin
	   exec sp_shiftassign
	   end
	-----autoshift--------   
	   insert into Trans_RawProcessDailyData select EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid, status,status1 from Trans_Raw#  
	   truncate table PDateDetails
	   truncate table StoreEmpID      
	   insert into StoreEmpID(EID) select distinct(t.Empid) from Trans_Raw# t join EmployeeMaster e on e.Emp_Code=t.EmpId where e.Emp_Status not in (2,3) order by t.EmpId asc---insertin the employee details order by empid in ascending order        
	   
	   
	   
	   select TOP 1 @FrmDate1 = PunchDate from Trans_RawProcessDailyData order by PunchDate 
	   select TOP 1 @ToDate1 = PunchDate from Trans_RawProcessDailyData order by PunchDate desc
	   insert into StoreEmpID(EID) select distinct Emp_Code From EmployeeMaster where Emp_Dol between convert(date,@FrmDate1) and convert(date,@ToDate1) and Emp_Status in (2,3)
	   select @CountEmpID = COUNT(EID) from StoreEmpID ---Employee id counter
	 
	--insert into PDateDetails(PunchDate1) select Distinct PunchDate from Trans_Raw# order by PunchDate  Asc
	
	 --===========================================================================================================   
	  -- If processor stop in between, this will avoid duplicate values in process_data
	  
	   select @MinTransDate =min(punchDate) from Trans_raw#
		select @MaxTransDate =Max(punchDate) from Trans_raw#  
		
	  if(@re_flag!=1)
	  begin
       delete from Process_data where pdate between @MinTransDate and @MaxTransDate
	  end
	  if(@re_flag=1)
	  begin
       delete from Process_data where pdate between @MinTransDate and @MaxTransDate  and EmpId in(select distinct(EmpId) from Trans_Raw#)
	  end

     --===========================================================================================================


	While (DATEDIFF(dd,@FrmDate1,@ToDate1) >= 0)
	begin
		insert into PDateDetails (PunchDate1) values (@FrmDate1)
		set @FrmDate1 = DATEADD(dd,1,@FrmDate1) 
	end 

	  	   
	   --insert into PDateDetails(PunchDate1) select Distinct PunchDate from Trans_RawProcessDailyData order by PunchDate Asc---selects distinct punchdates from raw table order by punchdate in ascending order        
	   
	   select @CountPunchDate =COUNT(PunchDate1) from PDateDetails  ---Punchdate counter   for normal processing
		
	    
	   
	   While(@EmpIDIndex <= @CountEmpID)---Employeeid counter loop        
	   Begin

		set @TotalHrsfordayinmin=null
		set @ShiftCode=null
		set @ShiftCodeyesterday=null
		set @ShiftINyesterday=null
		set @maxtimeyesterday=null
		set @maxtime=null
		set @ShiftOUTyesterday=null
		set @TotalHrsfordayinmin=null
	    
		set @InPunch =null
			set @OutPunch =null
			set @TotalHrs =null
			set @TotalHrsMin =null 
			set  @nshiftmax=null
			set @temphrs=null
			set @TotalHrsMin1=null
	     
		 select @ReadEmpID = EID from StoreEmpID where Id =@EmpIDIndex
	         
		  while (@PunchDateIndex <= @CountPunchDate)
		  begin
			 Truncate table TempDay       
			 Truncate Table npbefore12  
			 truncate table nightpunches      
			 Truncate Table ProcessingPunches ---truncating the table which consists of the all the punches of a employee on a particular day        
			 set @InPunch =null
			 set @OutPunch =null
			 set @ShiftCodeyesterday=null--CHANGE ON 03072013
			 set @ShiftCode=null--CHANGE ON 03072013
			 set @ReadPDate = NUll--CHANGE ON 03072013  
			 SET @MAXTIME = NULL --CHANGE ON 03072013
			 SET @MAXTIMEYESTERDAY=NULL --CHANGE ON 03072013
			 SET @SHIFTINYESTERDAY=NULL --CHANGE ON 03072013
			 SET @nightpunctcount=NULL --CHANGE ON 03072013
			 SET @SHIFTIN = NULL --CHANGE ON 03072013
			 SET @SHIFTOUT =NULL --CHANGE ON 03072013  
			 select @ReadPDate = PunchDate1 from PDateDetails where Id = @PunchDateIndex --reads the first date based on the index        
	       
			 set @month =DATEPART(MM, @ReadPDate)
			 set @year=DATEPART(YYYY, @ReadPDate)   
			 set @day=DATEPART(DD, @ReadPDate) 
			 set @daytemp = 'day' 
			 
			 if exists(select 1 from ShiftEmployee_Processing where Empid=@ReadEmpID and Month=@month and year=@year)--------Integrating with ShiftRoster
				   begin
					 set @SqlQuery='insert into TempDay select '+ @daytemp + convert(char,@day)+ 'from ShiftEmployee_Processing where Empid='''+ @ReadEmpID + ''' and Month = ' + convert(char,@month) + 'and year = ' + convert(char,@year)
					 execute(@SqlQuery)
					 select @ShiftCode = ShiftCode from TempDay
					 IF (@ShiftCode IS NULL or @ShiftCode = '' or @ShiftCode='-- Select --')--CODE CHANGE ON 03/07/2013 IF NOT GETTING THE SHIFT CODE IN SHIFT EMPLOYEE TABLE
					 BEGIN
						SELECT @ShiftCode = Emp_Shift_Detail FROM EmployeeMaster WHERE Emp_Code=@ReadEmpID
					 END
				   end 
			else 
				   begin
					select @ShiftCode = Emp_Shift_Detail FROM EmployeeMaster WHERE Emp_Code=@ReadEmpID --reading the shiftcode from the employee table based on the employee id         
				   end
	           
				----Ramadan shift code start
				set @isramadan = null
				----select @isramadan=isramadan from ShiftSettingtemp
				select @isramadan = isramadan from ShiftSetting where CompanyCode in( select Emp_Company from EmployeeMaster where Emp_Code=@ReadEmpID )
				if(@isramadan='1')
				begin
					select @ramadanstdt=ramadanstdt,@ramadantodt=ramadantodt from ShiftSettingtemp
					if exists(select 1 from ShiftSettingtemp where @ReadPDate between @ramadanstdt and @ramadantodt)
					begin
						select @ShiftIN = CONVERT(Time,Ramadan_InTime),@ShiftOUT =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode         
						set @ramadanflag='1'
					end
					else
					begin
						select @ShiftIN = CONVERT(Time,In_Time),@ShiftOUT =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode         
						set @ramadanflag='0'
					end
				end
				else
				begin                   
					select @ShiftIN = CONVERT(Time,In_Time),@ShiftOUT =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode         
					set @ramadanflag='0'
				end          
	         
	        
	           
			 if (@ShiftOUT<@ShiftIN)----for Night Shift 
			 begin             
				 set @TotalHrsMin=null
				 set @TotalHrs =null             
				 set @nshiftmax=convert(time,DATEADD(N,1439,0))
				 set @temphrs =convert(time,DATEADD(N,0,0))         
				 Truncate table tempday  -------------------take yesterday shift and assign maxtime = shiftout time of yesterday             
				 if @day=1
					 begin
						  set @readpdateyes =DATEADD(day,-1,@ReadPDate)
						  set @day=DATEPART(DAY,@readpdateyes)
						  set @month = DATEPART(MONTH,@readpdateyes) 
						  set @year =DATEPART(YEAR,@readpdateyes)
					 end
				 else
				 begin
				   set @day=@day -1
				 end
	             
			   if exists(select 1 from ShiftEmployee_Processing where Empid=@ReadEmpID and Month=@month and year=@year)--------Integrating with ShiftRoster
					begin
						   set @SqlQuery='insert into TempDay select '+ @daytemp + convert(char,@day)+ 'from ShiftEmployee_Processing where Empid=''' + @ReadEmpID + '''
						   and Month = ' + convert(char,@month) + 'and year = ' + convert(char,@year)
						   execute(@SqlQuery)
						   select @ShiftCodeyesterday = ShiftCode from TempDay
						 IF (@ShiftCodeyesterday IS NULL or @ShiftCodeyesterday = '' or @ShiftCodeyesterday='-- Select --')-- CODE CHANGE ON 03/07/2013 IF NOT GET THE SHIFTCODE IN SHIFTEMPLOYEE TABLE
							 BEGIN
								SELECT @ShiftCodeyesterday = Emp_Shift_Detail FROM EmployeeMaster WHERE Emp_Code=@ReadEmpID
							 END
			             
						if (@ramadanflag='0')
							begin
								select @ShiftINyesterday = CONVERT(Time,In_Time),@ShiftOUTyesterday =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCodeyesterday
								select @maxtimeyesterday =CONVERT(Time,Out_Time+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
							end
						else
							begin
								select @ShiftINyesterday = CONVERT(Time,Ramadan_InTime),@ShiftOUTyesterday =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCodeyesterday
								select @maxtimeyesterday =CONVERT(Time,Ramadan_OutTime+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
							end 				
						
				   end			   
			  else
				   Begin
						SELECT @ShiftCodeyesterday = Emp_Shift_Detail FROM EmployeeMaster WHERE Emp_Code=@ReadEmpID
						if (@ramadanflag='0')
							begin
								select @ShiftINyesterday = CONVERT(Time,In_Time),@ShiftOUTyesterday =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCodeyesterday
								select @maxtimeyesterday =CONVERT(Time,Out_Time+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
							end
						else
							begin
								select @ShiftINyesterday = CONVERT(Time,Ramadan_InTime),@ShiftOUTyesterday =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCodeyesterday
								select @maxtimeyesterday =CONVERT(Time,Ramadan_OutTime+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
							end 	
				   End
	          
				 if (@ramadanflag='0')
					begin
						select @maxtime=convert(time,out_time+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCode  
					end
					else
					begin
						select @maxtime=convert(time,Ramadan_OutTime+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCode  
					end    
					
				

				 if @ShiftINyesterday > @ShiftOUTyesterday 
				  begin
	                 
					 if not exists(select 1 from process_data where pdate =DATEADD(DD,-1,@ReadPDate) and Empid=@ReadEmpID)
					  begin
	                  
						select top 1 @InPunch =Punch_Time from Trans_Raw# where EmpId =@ReadEmpID and PunchDate =@ReadPDate and CONVERT(time,Punch_Time)<CONVERT(time,@maxtime) order by Punch_Time ASC
	                    
						if(@InPunch is not null) or (@InPunch !='')
						 begin                     
						   insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
						 end                   
					  end
				         
				  set @forinpunch =null
				  set @ncount=null
				  set @processed=0
				  truncate table nightpunches              
				  select top 1 @firstdate =PunchDate from Trans_Raw# order by PunchDate asc              
				 if (@processed = 0)
				 begin
	              
				  set @processed=1
	              
				  --insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID  and PunchDate=@firstdate and convert(time,Punch_Time)<convert(time,@maxtime) order by Punch_Time asc
					insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID  and PunchDate=@ReadPDate and convert(time,Punch_Time)<convert(time,@maxtimeyesterday) and  Punch_Time not in(select In_punch from process_data where EmpId=@ReadEmpID) and Punch_Time not in(select Out_punch from process_data where EmpId=@ReadEmpID and Out_punch is not null) order by Punch_Time asc--CODE GIVEN BY HEMANT ON 03072013

				  select @ncount =COUNT(ptime) from nightpunches

	                                          
				  if (@ncount >=2)
				 begin
				 	
				   if((@ncount %2) = 0)
					begin
					 set @ROWCOUNT=@ncount/2
				     
					 if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
					   begin
				      
						set @ROWCOUNT =@ROWCOUNT +1
				    
					  end
				      
					 if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null
					   begin
				      
						set @ROWCOUNT =@ROWCOUNT +1
				    
					  end
				     
					end
				   else
					begin
					 set @ROWCOUNT =@ncount /2
					 set @ROWCOUNT =@ROWCOUNT +1
					end
				  	    
				  
				   set @i=1
				   set @j=@i+1
				   set @rowindex =1
				 
				   while(@rowindex <= @ROWCOUNT)
					 begin
				        
					   set @InPunch =null
					   set @OutPunch =null 
					   set @TotalHrs =null
					   set @TotalHrsMin =null
				        
					   select @InPunch =ptime from nightpunches where nindex=@i
					   select @OutPunch =ptime from nightpunches where nindex =@j
				       
					   if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
						begin
				      
						  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
						  select @OutPunch =ptime from nightpunches where nindex =@i 
				    
						end
				        
						 if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null for previous day
						begin
				      
						  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=dateadd(dd,-1,@ReadPDate) and In_punch is not null and Out_punch is null
						  select @OutPunch =ptime from nightpunches where nindex =@i 			    
						end			       
					   if @InPunch < @OutPunch 
						begin
						   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
						   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
				           
						   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
						end
					   else
						begin
				        
						   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
							  Begin  
								   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
								   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
								   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
								   set @temphrs =convert(time,DATEADD(N,0,0))
								   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
														     
								   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
								   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
		                           
		                           
		                           
							   End 
				        
						end
				        
						if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)
						 begin
				    						 			    
							   if exists(select 1 from process_data where In_punch=@InPunch  and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
								begin
						      
								 update process_data set Out_punch=@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
			      				 set @j=@j-1
						      	 		    
							   end
							   
							   if exists(select 1 from process_data where In_punch=@InPunch  and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null for previous day
								begin
						      
								 update process_data set Out_punch=@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate) and In_punch =@InPunch 
			      				 set @j=@j-1
						      	 		    
							   end
						       
							   else
								begin				         
									insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)

								end
				    
						 end -------end if
	                    
						if @OutPunch is null or @OutPunch = '' -----calculating sum of totals hours
						 begin			         
							  insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
				 
						end -------end if
				    
				    
	                    
						set @i=@j+1
						set @j=@i+1
						set @rowindex =@rowindex +1
				        
					 end-------end While 
				        
				  end ------end if (@ncount >2)
				  
				  if (@ncount = 1)------------------------------if exists only one punch
				 begin
				 
				   select @InPunch =ptime  from nightpunches  
				   set @OutPunch =null
				   set @TotalHrs =null
				   
				   if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null for yesterday as it is night shift we have to consider yesterday also
					   begin
						      
						  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=dateadd(dd,-1,@ReadPDate) and In_punch is not null and Out_punch is null
						  select @OutPunch =ptime  from nightpunches   
					      
						  if @InPunch < @OutPunch 
							begin
							   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
							   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
					           
							   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
							end
						   else
							begin
					        
							   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
								  Begin  
									   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
									   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
									   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
									   set @temphrs =convert(time,DATEADD(N,0,0))
									   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
															     
									   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
									   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
			                           
									   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
			                           
								   End 
					        
							end -------end  if @InPunch < @OutPunch
				       
				    	      
						   update process_data set Out_punch =@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate) and In_punch =@InPunch 
				      
					 end -------end if
				   
				   
				    
				   else
					begin
				      
					  if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
					begin
				      
					  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
					  select @OutPunch =ptime  from nightpunches   
				      
					  if @InPunch < @OutPunch 
						begin
						   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
						   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
				           
						   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
						end
					   else
						begin
				        
						   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
							  Begin  
								   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
								   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
								   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
								   set @temphrs =convert(time,DATEADD(N,0,0))
								   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
														     
								   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
								   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
								 End 
				        
						end -------end  if @InPunch < @OutPunch
				       
				    	      
					  update process_data set Out_punch =@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
				      
					end
				     
					 else
					  begin
				      
					   insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
				      
					  end
				      
					end
				   			   	   
				 
				 end ------end for if (@punchcountaday =1 )
	    
			   end -----end if(@processed!=1)
			   end
			   
	           
			  truncate table nightpunches
			 
	        
				 if((@shiftcodeYEsterday is null) or  (@shiftcodeYEsterday ='') or (@shiftcodeYEsterday='woff') or ( @ShiftOUTyesterday > @ShiftINyesterday)) --changes on 03032014 vinay
					  Begin
						 insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=@ReadPDate  order by Punch_Time asc
						 insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=DATEADD(dd,1,@ReadPDate) and convert(time,Punch_Time)<=convert(time,@maxtime)  order by Punch_Time asc
					  End 
				 Else--changes on 03032014 vinay
					  Begin
						insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=@ReadPDate and convert(time,Punch_Time)>convert(time,@maxtimeYesterday)  order by Punch_Time asc							 		   
						insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID and PunchDate=DATEADD(dd,1,@ReadPDate) and convert(time,Punch_Time)<=convert(time,@maxtime)  order by Punch_Time asc
					  End	
					  			
				 select @nightpunctcount=COUNT(ptime) from nightpunches
							   
				 if (@nightpunctcount >=2)
				 begin			 	
				   if((@nightpunctcount %2) = 0)
					begin
					 set @ROWCOUNT=@nightpunctcount/2			     
					 if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
					   begin			      
						set @ROWCOUNT =@ROWCOUNT +1			    
					  end			      
					 if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null
					   begin			      
						set @ROWCOUNT =@ROWCOUNT +1			    
					  end			     
					end
				   else
					begin
					 set @ROWCOUNT =@nightpunctcount /2
					 set @ROWCOUNT =@ROWCOUNT +1
					end		  	    
				   set @i=1
				   set @j=@i+1
				   set @rowindex =1			 
				   while(@rowindex <= @ROWCOUNT)
					 begin			        
					   set @InPunch =null
					   set @OutPunch =null 
					   set @TotalHrs =null
					   set @TotalHrsMin =null			        
					   select @InPunch =ptime from nightpunches where nindex=@i
					   select @OutPunch =ptime from nightpunches where nindex =@j			       
					   if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
						begin			      
						  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
						  select @OutPunch =ptime from nightpunches where nindex =@i 			    
						end			       
					   if @InPunch < @OutPunch 
						begin
						   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
						   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)			           
						   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
						end
					   else
						begin
				        
						   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
							  Begin  
								   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
								   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
								   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
								   set @temphrs =convert(time,DATEADD(N,0,0))
								   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
														     
								   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
								   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
		                           
								   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
		                           
							   End 
				        
						end
				        
						if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)
						 begin
				    						 			    
							   if exists(select 1 from process_data where In_punch=@InPunch  and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
								begin
						      
								 update process_data set Out_punch=@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
			      				 set @j=@j-1
						      	 		    
							   end
						       
							   else
								begin
						         
								 if ((@value =1) or (@forinpunch =1))
								  begin
									insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
								  end
								 else
								  begin
									insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,@ReadPDate,@InPunch,@OutPunch,@TotalHrs)
								  end 
						        			         
								end
				    
						 end -------end if
	                    
						if ((@OutPunch is null or @OutPunch = '') and (@InPunch is not null))-----calculating sum of totals hours
						 begin
				    
						   insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,@ReadPDate,@InPunch,@OutPunch,@TotalHrs)
				    
						end -------end if
				    
				    
	                    
						set @i=@j+1
						set @j=@i+1
						set @rowindex =@rowindex +1
				        
					 end-------end While 
				        
				end ------end if (@nightpunctcount >2) 
				
				if (@nightpunctcount = 1) ------------------------------if exists only one punch
				 begin			
				   select @InPunch =ptime  from nightpunches  
				   set @OutPunch =null
				   set @TotalHrs =null			   
				   if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
					begin			      
					  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
					  select @OutPunch =ptime  from nightpunches     
					  if @InPunch < @OutPunch 
						begin
						   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
						   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)			           
						   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
						end
					   else
						begin			        
						   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
							  Begin  
								   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
								   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
								   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
								   set @temphrs =convert(time,DATEADD(N,0,0))
								   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)													     
								   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
								   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
								   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin
							   End 			        
						end -------end  if @InPunch < @OutPunch			       
					   update process_data set Out_punch =@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
					end			    
				   else
					begin			      
					  if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null for yesterday as it is night shift we have to consider yesterday also
					   begin					 
						set @TotalHrs =null  
						--change here
						if @OutPunch <> @InPunch
						begin
							select @OutPunch =ptime  from nightpunches 
						end
						    
						if convert(time,@OutPunch) < convert(time,@maxtime)
			    		 begin  
				    	   
						  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=dateadd(dd,-1,@ReadPDate) and In_punch is not null and Out_punch is null
					      
						  if @InPunch < @OutPunch 
							begin
							   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
							   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
					           
							   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
							end
						   else
							begin
					        
							   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
								  Begin  
									   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
									   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
									   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
									   set @temphrs =convert(time,DATEADD(N,0,0))
									   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
															     
									   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
									   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
								   End 
					        
							end -------end  if @InPunch < @OutPunch
				       
			    			 if @InPunch != @OutPunch 
			    			  begin 
							  update process_data set Out_punch =@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate) and In_punch =@InPunch 
							 end
				            
							end
						   else
							begin
				            
							  select @InPunch =ptime  from nightpunches 
							  set @OutPunch =null
							  insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,@ReadPDate,@InPunch,@OutPunch,@TotalHrs)
				            
							end
					 end -------end if
				     
					 else
					  begin
						  insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,@ReadPDate,@InPunch,@OutPunch,@TotalHrs) 
					  end			      
					end		   			   	   
				 
				 end ------end for if (@punchcountaday =1 )
							  
		  end---------end if (@ShiftOUT<@ShiftIN)
	      
		  else 
		   begin       
			   Truncate table tempday  -------------------take yesterday shift and assign maxtime = shiftout time of yesterday
			 set @ROWCOUNT =null
			   truncate table ProcessingPunches
			   set @value =null                
				   if @day=1
					 begin
					  set @readpdateyes =DATEADD(day,-1,@ReadPDate)
					  set @day=DATEPART(DAY,@readpdateyes)
					  set @month= DATEPART(MONTH,@readpdateyes)
					  set @year=DATEPART(YEAR,@readpdateyes)
					 end
					else
					 begin
					  set @day=@day -1
					 end                 
		 if exists(select 1 from ShiftEmployee_Processing where Empid=@ReadEmpID and Month=@month and year=@year)--------Integrating with ShiftRoster
			 begin
				  set @SqlQuery='insert into TempDay select '+ @daytemp + convert(char,@day)+ 'from ShiftEmployee_Processing where Empid=''' + @ReadEmpID + ''' and Month = ' + convert(char,@month) + 'and year = ' + convert(char,@year)
				  execute(@SqlQuery)
				  select @ShiftCodeyesterday = ShiftCode from TempDay
				  IF (@ShiftCodeyesterday IS NULL or @ShiftCodeyesterday='' or @ShiftCodeyesterday='-- Select --')-- CODE CHANGE ON 03/07/2013 IF SHIFT CODE IS NOT AVAILABLE IN SHIFTEMPLOYEE TABLE
					  BEGIN
						SELECT @ShiftCodeyesterday = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID
					  END
	              
				  if (@ramadanflag='0')
					begin
						select @ShiftINyesterday = CONVERT(Time,In_Time),@ShiftOUTyesterday =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCodeyesterday
						select @maxtimeyesterday =CONVERT(Time,Out_Time+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
					end
				  else
					begin
						select @ShiftINyesterday = CONVERT(Time,Ramadan_InTime),@ShiftOUTyesterday =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCodeyesterday
						select @maxtimeyesterday =CONVERT(Time,Ramadan_OutTime+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
					end  
		    end 
		   Else
			Begin
				SELECT @ShiftCodeyesterday = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID
				 
				  if (@ramadanflag='0')
					begin
						select @ShiftINyesterday = CONVERT(Time,In_Time),@ShiftOUTyesterday =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCodeyesterday
						select @maxtimeyesterday =CONVERT(Time,Out_Time+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
					end
				  else
					begin
						select @ShiftINyesterday = CONVERT(Time,Ramadan_InTime),@ShiftOUTyesterday =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCodeyesterday
						select @maxtimeyesterday =CONVERT(Time,Ramadan_OutTime+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCodeyesterday 
					end  
			End 
	            
				if (@ramadanflag='0')
				begin
					select @maxtime=convert(time,out_time+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCode  
				end
				else
				begin
				   select @maxtime=convert(time,Ramadan_OutTime+CONVERT(datetime,max_overtime)) from shift where Shift_Code = @ShiftCode  
				end
				
				if @ShiftINyesterday>@ShiftOUTyesterday -----------if yesterday is night shift
				  begin  	              
					insert into ProcessingPunches(Empid,PPdate,Inpunch) select EmpId,PunchDate,Punch_Time from Trans_RawProcessDailyData  where PunchDate = @ReadPDate and EmpId = @ReadEmpID and convert(time,Punch_Time)>convert(time,@maxtimeyesterday) order by Punch_Time asc--inserting all the punches of a particular employee for a particular date--- fix on 10072013   
					--insert into nightpunches select punch_time from Trans_Raw# where EmpId =@ReadEmpID and PunchDate =@ReadPDate and CONVERT(time,Punch_Time)<CONVERT(time,@maxtimeyesterday) 
					if exists(select 1 from Trans_Raw# where EmpId =@ReadEmpID and PunchDate =@ReadPDate and CONVERT(time,Punch_Time)<=CONVERT(time,@maxtimeyesterday))-- fix on 10072013
					begin                 
						  --set @value =1
						 -- insert into nightpunches select punch_time from Trans_Raw# where EmpId =@ReadEmpID and PunchDate =@ReadPDate and CONVERT(time,Punch_Time)<=CONVERT(time,@maxtimeyesterday) --changes on 25022014 vinay
						 insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID  and PunchDate=@readpdate and convert(time,Punch_Time)<=convert(time,@maxtimeyesterday) and  Punch_Time not in(select In_punch from process_data where EmpId=@ReadEmpID) and Punch_Time not in(select Out_punch from process_data where EmpId=@ReadEmpID and Out_punch is not null) order by Punch_Time asc--CODE GIVEN BY HEMANT ON 03072013
					end 
					
	/******************************************************************************************************************************************************* */	 --changes on 25022014 vinay									
							if not exists(select 1 from process_data where pdate =DATEADD(DD,-1,@ReadPDate) and Empid=@ReadEmpID)
  								begin			                  
									select top 1 @InPunch =Punch_Time from Trans_Raw# where EmpId =@ReadEmpID and PunchDate =@ReadPDate and CONVERT(time,Punch_Time)<CONVERT(time,@maxtimeyesterday) order by Punch_Time ASC	----Fix by Madhu on 03022016 if yestarday is Night shift and today is general then the Cuff off should be considered for yestarday shift out time                     		                    
									if(@InPunch is not null) or (@InPunch !='')
										begin                     
											insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
										end                   
								end	
								
								
								
								set @forinpunch =null
								set @ncount=null
								set @processed=0
								--truncate table nightpunches              
								select top 1 @firstdate = PunchDate from Trans_Raw# order by PunchDate asc 
							             
							if (@processed = 0)
							begin
			              
											set @processed=1
						              
												
									--insert into nightpunches(ptime) select Punch_Time from Trans_Raw# where EmpId=@ReadEmpID  and PunchDate=@readpdate and convert(time,Punch_Time)<=convert(time,@maxtimeyesterday) and  Punch_Time not in(select In_punch from process_data where EmpId=@ReadEmpID) and Punch_Time not in(select Out_punch from process_data where EmpId=@ReadEmpID and Out_punch is not null) order by Punch_Time asc--CODE GIVEN BY HEMANT ON 03072013

											select @ncount =COUNT(ptime) from nightpunches

					                                          
										if (@ncount >=2)
										begin
								 	
												 if((@ncount %2) = 0)
												 begin
															set @ROWCOUNT=@ncount/2
									     
													if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
													begin
									      
															set @ROWCOUNT =@ROWCOUNT +1
									    
													end
									      
													if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null
													begin
									      
															set @ROWCOUNT =@ROWCOUNT + 1
									    
													end
									     
												end
												else
												begin
														 set @ROWCOUNT =@ncount / 2
														 set @ROWCOUNT =@ROWCOUNT + 1
												end
										  	    
										  
														set @i=1
														set @j=@i+1
														set @rowindex =1
								 
											while(@rowindex <= @ROWCOUNT)
											begin
										        
														   set @InPunch =null
														   set @OutPunch =null 
														   set @TotalHrs =null
														   set @TotalHrsMin =null
										        
														   select @InPunch =ptime from nightpunches where nindex=@i
														   select @OutPunch =ptime from nightpunches where nindex =@j
									       
													if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
													begin
									      
														  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
														  select @OutPunch =ptime from nightpunches where nindex =@i 
									    
													end
								        
													if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null for previous day
													begin
									      
														  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=dateadd(dd,-1,@ReadPDate) and In_punch is not null and Out_punch is null
														  select @OutPunch =ptime from nightpunches where nindex =@i 			    
													end
															       
													if @InPunch < @OutPunch 
													begin
														   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
														   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
									           
															--set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
													end
												
													else
													begin
											        
															if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
															Begin  
																   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
																   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
																   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
																   set @temphrs =convert(time,DATEADD(N,0,0))
																   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
																						     
																   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
																   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
										                           
																   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
															End 
													end
								        
												   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)
														begin
												    						 			    
															   if exists(select 1 from process_data where In_punch=@InPunch  and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
															   begin
														      
																	 update process_data set Out_punch=@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
																	 set @j=@j-1
														      	 		    
															   end
															   
															   if exists(select 1 from process_data where In_punch=@InPunch  and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null for previous day
															   begin
														      
																	 update process_data set Out_punch=@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate) and In_punch =@InPunch 
																	 set @j=@j-1
														      	 		    
															   end
														       
															   else
															   begin
														         
																	 --if (@value =1)
																	 -- begin
																		insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
																	 
														        			         
															   end
												    
														 end -------end if
					                    
								-----calculating sum of totals hours-----
					                    
												if @OutPunch is null or @OutPunch = ''		
													 begin
											         
												 				   insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
														 
													 end -------end if
							    
							    
					                
													set @i=@j+1
													set @j=@i+1
													set @rowindex =@rowindex +1
								        
										  end-------end While(@rowindex <= @ROWCOUNT) 
								        
								  end ------end if (@ncount >2)
								  
						-------------------if exists only one punch----------------
								  
								  if (@ncount = 1)
								  begin
								 
											   select @InPunch =ptime  from nightpunches  
											   set @OutPunch =null
											   set @TotalHrs =null
								   
											if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate))--------to read inpunch from process_data if inpunch exists and outpunch is null for yesterday as it is night shift we have to consider yesterday also
											begin
											      
													  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=dateadd(dd,-1,@ReadPDate) and In_punch is not null and Out_punch is null
													  select @OutPunch =ptime  from nightpunches   
											      
													if @InPunch < @OutPunch 
													begin
													   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
													   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
											           
													   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
													end
													else
													begin
											        
													   if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
													   Begin  
															   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
															   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
															   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
															   set @temphrs =convert(time,DATEADD(N,0,0))
															   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
																					     
															   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
															   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
									                           
															 
									                           
													   End 
											        
													end -------end  if @InPunch < @OutPunch
										       
										    	      
													update process_data set Out_punch =@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =dateadd(dd,-1,@ReadPDate) and In_punch =@InPunch 
									      
											end -------end if
								   
								   
								    
										   else
										   begin
										      
												 if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
												 begin
											      
															  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
															  select @OutPunch =ptime  from nightpunches   
											      
														if @InPunch < @OutPunch 
															begin
																   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
																   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
														           
																   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
															end
														else
														   begin
											        
															  if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)       
															  Begin  
																   set @nshiftmax=convert(time,DATEADD(N,1439,0))  
																   --set @nshiftmax=@nshiftmax+convert(time,DATEADD(N,01,0))    
																   select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@nshiftmax)
																   set @temphrs =convert(time,DATEADD(N,0,0))
																   select @TotalHrsMin1=DATEDIFF(Minute,@temphrs,@OutPunch)
																						     
																   set @TotalHrsMin=@TotalHrsMin+@TotalHrsMin1+1    
																   select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.        
										                           
																   --set @TotalHrsfordayinmin =@TotalHrsfordayinmin + @TotalHrsMin 
															  End 
											        
												  		  end  
												 			update process_data set Out_punch =@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
										      
												 end
										     
												 else
												 begin									      
														insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
									      		 end
											 end
						   			   	 
								  end ------end correct(@ncount = 1)correct-----------(wrong(for if (@punchcountaday =1 ))wrong
			    
					   end
						truncate table nightpunches
				end
	/* *********************************************************************************************************************************************************/ --changes on 25022014 vinay
			   else
				  begin
					insert into ProcessingPunches(Empid,PPdate,Inpunch) select EmpId,PunchDate,Punch_Time from Trans_RawProcessDailyData  where PunchDate = @ReadPDate and EmpId = @ReadEmpID order by Punch_Time asc--inserting all the punches of a particular employee for a particular date        
				  end
	                 
				  select @punchcountaday =COUNT(Inpunch) from ProcessingPunches 
	              
				  IF (@punchcountaday >= 2)
				   BEGIN	
				    
					IF((@punchcountaday % 2) = 0)          
					BEGIN  
	            
					 SET @ROWCOUNT = @punchcountaday / 2  
					 
					  if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
					   begin
				      
						set @ROWCOUNT =@ROWCOUNT +1
				    
					  end   
	        
					END                 
					ELSE          
					  BEGIN          
	                 
						 SET @ROWCOUNT = @punchcountaday / 2          
	             
						 SET @ROWCOUNT = @ROWCOUNT + 1          
	                 
					  END  
				  
				  SET @i = 1                      
			                             
				  SET @j = @i + 1 
				  set @rowindex =1
				  set @TotalHrsfordayinmin=0                           
			                          
				  WHILE(@rowindex <= @ROWCOUNT)                            
				  BEGIN
				    
					set @InPunch =null
					set @OutPunch =null
					set @TotalHrs =null
					set @TotalHrsMin =null 
					set @InPunch1007=null --Changes on 15072013
					set @OutPunch1007=null

				    
					-- start fix on 10072013
					select @InPunch1007 = Inpunch from ProcessingPunches where Id =@i
				    
					select @OutPunch1007 =inpunch from ProcessingPunches where Id =@j 
				    
					if @InPunch1007 > @OutPunch1007
					begin
						set @InPunch = @OutPunch1007
						set @OutPunch = @InPunch1007
					end
					else
					begin
						set @InPunch = @InPunch1007
						set @OutPunch = @OutPunch1007
					end
				    
					-- end fix on 10072013
					if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
					begin
				      
					  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
					  select @OutPunch =inpunch from ProcessingPunches where Id =@i 
				    
					end
				    
					if (@InPunch !='' and @InPunch is not null) and (@OutPunch !='' and @OutPunch is not null)
					begin
				    
					 select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
					 select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
				    
					   if exists(select 1 from process_data where In_punch=@InPunch  and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
						begin
				      
						 update process_data set Out_punch=@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
			      		 set @j=@j-1
				      	 		    
					   end
				       
					   else
						begin			         
							insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,@ReadPDate,@InPunch,@OutPunch,@TotalHrs)
						end	     
					 end 
				    
					if @OutPunch is null or @OutPunch = '' and @InPunch is not null -----calculating sum of totals hours
					begin			    
					  insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,@ReadPDate,@InPunch,@OutPunch,@TotalHrs)
					end
				    
				    
					set @i=@j+1
					set @j=@i+1
					set @rowindex =@rowindex +1
				    
				  End ------------ end while (@rowindex <= @ROWCOUNT)
				
				End --------------end if (@punchcountaday > 2)
				if (@punchcountaday = 1)------------------------------if exists only one punch
				 begin
				 
				   select @InPunch =Inpunch  from ProcessingPunches  
				   set @OutPunch =null
				   set @TotalHrs =null
				   
				   if exists(select 1 from process_data where In_punch is not null and Out_punch is null and Empid =@ReadEmpID and pdate =@ReadPDate)--------to read inpunch from process_data if inpunch exists and outpunch is null
					begin
				      
					  select @InPunch = in_punch from process_data where Empid =@ReadEmpID and pdate=@ReadPDate and In_punch is not null and Out_punch is null
					  select @OutPunch =Inpunch  from ProcessingPunches  
				      
					  select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)        
					  select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
				      
					  update process_data set Out_punch =@OutPunch,sum=@TotalHrs where Empid =@ReadEmpID and pdate =@ReadPDate and In_punch =@InPunch 
				      
					end			    
				   else
					begin			    
					  if @value=1
					   begin
						 insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,dateadd(dd,-1,@ReadPDate),@InPunch,@OutPunch,@TotalHrs)
					   end
					  else
					   begin
						 insert into process_data(Empid,pdate,In_punch,Out_punch,sum) values (@ReadEmpID,@ReadPDate,@InPunch,@OutPunch,@TotalHrs)
					   end
				     
					end
				   			   	   
				 
				 end ------end for if (@punchcountaday =1 )     
			       
		   end------end else
	      
		  set @PunchDateIndex = @PunchDateIndex + 1        
	      
		 end ---------end while 2  
	      
		set @PunchDateIndex = 1          
		set @EmpIDIndex = @EmpIDIndex + 1           
	   End -----------end while 1
end
