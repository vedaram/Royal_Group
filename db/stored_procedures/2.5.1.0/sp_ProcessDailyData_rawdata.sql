GO
/****** Object:  StoredProcedure [dbo].[sp_ProcessDailyData_rawdata]    Script Date: 4/29/2016 8:04:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_ProcessDailyData_rawdata]        
       (@fromdateforreprocess datetime=null,@todateforreprocess datetime=null)
AS        
BEGIN      
       --Declare variable Section   
       --modified for Break deduction for weekoff and holidays AND INSERT DATA IN OVERTIME TABLE AND IF LEAVE IS APPROVED BUT EMPLOYEE PUNCHED STATUS SHOULD CHANGE on 12112014/1230 by vikash     
       Declare @CountEmpID int,        
       @EmpIDIndex int,        
       @CountPunchDate int,        
       @PunchDateIndex int,        
       @ReadEmpID varchar(max),        
       @ReadEmpName varchar(Max),        
       @ReadPDate datetime,        
       @InPunch datetime,        
       @OutPunch datetime,
       @TotalShiftTime time,         
       @TotalHrs varchar(Max), 
       @empcat varchar(max),       
       @TotalHrsMin int, 
       @TotalHrsMin1 int,       
       @Status varchar(Max),        
       @TotalHrsTimeIn datetime,        
       @TotalHrsTimeOut datetime,
       @ShiftCodeyesterday  varchar(Max),        
       @ShiftCode varchar(Max),    
       @ShiftCodeyes varchar(Max),     
       @CalWeeklyOff1 varchar(Max),        
       @CalWeeklyOff2 varchar(Max),        
       @WeeklyOff1 varchar(Max),        
       @WeeklyOff2 varchar(max),        
       @rtDayofWeek varchar(Max),        
       @ShiftIN datetime,
       @ShiftINyes datetime,
       @ShiftOUTyes datetime,    
       @ShiftOUTyesterday datetime,        
       @ShiftOUT datetime,        
       @ShiftINTime datetime,        
       @ShiftOUTTime datetime ,        
       @EarlyLeaversMin int,
       @EarlyLeaversMin1 int,
       @Totalhrstime time,        
       @EarlyLeaverstimeformat varchar(Max),        
       @LateComersMin int,
       @LateComersMin1 int,                
       @LateComerstimeformat varchar(Max),        
       @Overtimemin int, 
       @sOvertimetimeformat varchar(8),
       @holidaygroup varchar(max),       
       @Overtimetimeformat varchar(8),        
       @Comp_id varchar(max),        
       @Comp_Name varchar(max),      
       @Cat_Name varchar(max),        
       @Dept_id varchar(max),        
       @Dept_Name varchar(max),        
       @Desig_id varchar(max),        
       @Desig_Name varchar(max),        
       @ShiftTime time,     
       @Cat_Code varchar(max),     
       @OutTime datetime,   
       @shift_Name varchar(max),  
       @shift_Nameyes varchar(max), 
       @contact varchar(max),  
       @DOJ Datetime,  
       @daytemp varchar(max),
       @tempcol varchar(max),
       @SqlQuery varchar(max),
       @workhrs time,
       @overtime time,
       @maxovertime time,
       @minovertime time,
       @halfday varchar(max),
       @halfday_shift varchar(max),
       @halfday_endtime datetime,
       @halfday_starttime datetime,
       @breakin datetime,
       @breakout datetime,
       @tot1  datetime,
       @sumtotmin int,
       @sumtot1 datetime,
       @countrow int,
       @initialrow int,
       @actual int,
       @breakminute int,
       @brkHrsMinute int,
       @fromdate datetime,
       @fromdate1 datetime,
       @fromdate2 datetime,
       @todate datetime,
       @fDate datetime,
       @tDate datetime,
       @punchdate datetime,
       @InGrace int,
       @OutGrace int,
       @ShiftIN_GraceIn datetime, 
       @ShiftOUT_GraceOut datetime,
       @ShiftOUT_GraceOut1 datetime,
       @ShiftIN_GraceInyes datetime, 
       @ShiftOUT_GraceOutyes datetime,
       @outcutoffmin int,
       @incutoffmin int,
       @breakoutcutoftime datetime,
       @breakoutcutoftimemin datetime,
       @breakincutoftimemin datetime,
       @breakincutoftime datetime,
       @statuswoh varchar(10),
       @includep int,
       @Employee_OT_Elegibality int,
       @isramadan varchar(1),
       @ramadanstdt datetime,
       @ramadantodt datetime,
       @ramadanflag varchar(1),
       @ShiftFlag bit,
       @ShiftNightFlag bit,
       @isNormalShift varchar(1),
       @ingracecal int,
       @outgracecal int,
       @midtimestart time(7),
       @midtimeend time(7),
       @ingrace1 time(7),
       @outgrace1 time(7),
       @isaso bit,
       @outcutoffmin1 int,
       @incutoffmin1 int,
       @breakoutcutoftime1 datetime,
       @breakincutoftime1 datetime,
       
       @EarlyComersMin int,
       @EarlyComerstimeformat varchar(Max),
       @LateGoMin int,
       @LateGotimeformat varchar(Max),
       @lastgracetime time(7),
        --Break In & Out Lateby & Early Leavers caluclation variables
       @BoutEarlyBy varchar(max),
       @BoutEarlyByMin int,
       @Bout_Grace int,
       @BoutEarly_Grace time,
       @Bout_Grace_Yes int,
       @DateUpdate datetime,
       @CheckTime datetime,
       @BinLateBy varchar(max),
       @BinLatebyMin int,   
       @Bin_Grace int,
       @BinLateBy_grace int ,
       @Bin_Grace_Yes int,
       @ShiftCode_Manual varchar(max),
       @EmplCategory_Manual varchar(max),
       @TempH1 int,
       @tempH2 int,
       @TempH int,
       @ReadPDateP datetime,@ReadPDatePP datetime,
       --Break Deduction on WOP & HP
       @MinReqHrsForDed int,
       @MinsToBeDeducted int,  
       @RequireDeduction int,         
       @ReadDateTemp datetime,
       @re_flag int
       --Break In & Out Lateby & Early Leavers caluclation variables End 
              
       declare @empid as varchar(12),@count as int,@count1 as int,@index1 as int,@index as int,@fholiday as datetime,@tholiday as datetime,@lcount int,
               @lindex as int,@lfrom as datetime,@lto as datetime,@ldate as datetime,@piInPunch as varchar(20),@piOutPunch as varchar(20),
               @piWorkDate as datetime, @intime as datetime,@outtime1 as datetime,@EarlyLeavers as int,@Earlyleaversformat as varchar(max),
               @month as int,@year as int,@day as int,@nOutPunch as datetime,@nshiftmax as datetime,@temphrs as datetime,@punchcountaday as int,@maxtime as datetime,@maxtimeyesterday as datetime,@ShiftINyesterday as datetime,
               @nightpunctcount as int,@np12count as int,@readpdateyes as datetime,@prevday as datetime,@today as datetime,@app int,@OTE as varchar(1),@breakin_Manual varchar(20),@breakout_Manual varchar(20)
               ,@chkmailforot int, @otcount int,@dol datetime
       DECLARE @holidays TABLE
       (
       hindex int identity(1,1),
       fdt datetime,tdt datetime
       )  
       Declare @leaves table
       (
       lindex int identity(1,1),
       empid varchar(12),
       fdate datetime,
       tdate datetime,
       lid varchar(10),
       hlstatus int
       )
       Declare @leaves1 table
(
lindex int identity(1,1),
empid varchar(10),
fdate datetime,
tdate datetime,
lid int,
hl_ststus int,
leavetype varchar(max)
)
       Declare @cancelledleaves table
       (
       lindex int identity(1,1),
       empid varchar(12),
       fdate datetime,
       tdate datetime,
       lid varchar(10)
       )
       declare @covod table
       (
       lindex int identity(1,1),
       empid varchar(12),
       fdate datetime,
       tdate datetime,
       lcode varchar(max)
       ) 
       declare @covodleaves table
       (
         tid int identity(1,1),
         empid varchar(12),
         ldates datetime,
         lecode varchar(max)
       )
       declare @templeave table
       (
         tid int identity(1,1),
         empid varchar(12),
         ldates datetime
        )  
         declare @templeave1 table
(
  tid int identity(1,1),
  empid varchar(10),
  ldates datetime,
  hlflvststatus int,
  leavetype varchar(max)
) 
       declare @tempcancelleave table
         (
         tid int identity(1,1),
         empid varchar(12),
         ldates datetime
         )    
                     
       declare @punches table
       (
       pindex int identity(1,1),
       empid varchar(12),
       pdate datetime,
       pin varchar(20),
       pout varchar(20),
       Bout varchar(20),
       Bin varchar(20),
       app int
       )


       declare @nightpunches table
       (
       nindex int identity(1,1),
       ptime datetime
       )  

       declare @npbefore12 table
       (
       nindex int identity(1,1),
       ptime datetime
       )              
                     
                     
       --set the variables        
       set @EmpIDIndex = 1        
       set @PunchDateIndex = 1        
       --set the variables        
                     
          --Truncate Table ProcessDailyData  
       Truncate table process_data_daily      
       Truncate Table PDateDetails        
       Truncate Table StoreEmpID         
       Truncate Table ProcessingPunches        
       --Truncate Table Temp_Check        
       Truncate Table Trans_RawProcessDailyData        
       insert into Trans_RawProcessDailyData select EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid, status ,status1 from Trans_Raw# 
       --insert into StoreEmpID(EID) select Emp_Code from EmployeeMaster where Emp_Status=1  ORDER by Emp_Code asc---insertin the employee details order by empid in ascending order        
              select @re_flag = re_flag from ReprocessFlag

       if(@re_flag=1)
       begin
         --insert into StoreEmpID(EID) select distinct(t.Empid) from Trans_Raw# t join EmployeeMaster e on e.Emp_Code=t.EmpId where e.Emp_Status =1 order by t.EmpId asc---insertin the employee details order by empid in ascending order        
           insert into StoreEmpID(EID) select distinct(t.EID) from storeempid1 t order by t.EID ---insertin the employee details order by empid in ascending order        
       end
       else
       begin
           insert into StoreEmpID(EID) select Emp_Code from EmployeeMaster where Emp_Status not in (2,3) order by Emp_Code asc---insertin the employee details order by empid in ascending order        
       end

       --insert into StoreEmpID(EID) values('10001')
       
       
       select TOP 1 @fromdate1 = PunchDate from MasterTrans_Raw# order by PunchDate desc
       select TOP 1 @fromdate2 = PunchDate from Trans_Raw# order by PunchDate
       ---changed by sumaiya to reprocess for absent or woff---
        if(@re_flag=1)
       begin
       set @todate = @todateforreprocess
       end
       else
       begin
       select TOP 1 @todate = PunchDate from Trans_Raw# order by PunchDate DESC
       end

       ----changes end------------
       --set @punchdate = DATEADD(dd,-1,@fromdate)
       if (@fromdate1 <= @fromdate2)
       begin
              set @fromdate = @fromdate1
       end
       else
       begin
              set @fromdate = @fromdate2
       end 
       --set @punchdate = DATEADD(dd,-1,@fromdate)
       ---changed by sumaiya to reprocess for absent or woff---
        if(@re_flag=1)
       begin
       set @fromdate = @fromdateforreprocess
       end      
       else if((@fromdate is null) or (@fromdate=''))
       begin
              select top 1 @fromdate=PunchDate from Trans_Raw# order by PunchDate asc
       end
              ----changes end------------
       
       set @fDate=@fromdate
       set @tDate= @todate
	   insert into StoreEmpID(EID) select distinct Emp_Code from EmployeeMaster where emp_dol between convert(date,@fDate) and convert(date,@tDate) and Emp_Status  in (2,3) order by Emp_Code asc
	    select @CountEmpID = COUNT(EID) from StoreEmpID ---Employee id counter   
	   --set @fromdate='2016-04-30'
       -- set @todate= '2016-05-05'
        
       insert into Trans_RawProcessDailyData (EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid,status1,status) select EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid,status1,status from masterTrans_Raw# where PunchDate between convert(datetime,@fromdate)-1 and convert(datetime,@fromdate)
  

       While (DATEDIFF(dd,@fromdate,@todate) >= 0)
       begin
              insert into PDateDetails (PunchDate1) values (@fromdate)
              set @fromdate = DATEADD(dd,1,@fromdate) 
       end 
              select @CountPunchDate =COUNT(PunchDate1) from PDateDetails  ---Punchdate counter        
                      
       While(@EmpIDIndex <= @CountEmpID)---Employeeid counter loop        
       Begin        
              set @ReadEmpID = NULL        
              set @Comp_id = NUll         
              set @Comp_Name = NUll         
              set @isaso= null
              set @Dept_id =NUll        
              set @Dept_Name = Null        
              set @Desig_id = Null        
              set @Desig_Name = NUll   
              SET @Cat_Code = NULL  
              SET @Cat_Name = NULL   
              set @contact=NULL  
              set @DOJ=NULL 
              set @empcat=null  
              set @holidaygroup=null   
              set @statuswoh=null    
              set @sOvertimetimeformat=null   
              set @includep = null 
              set @Employee_OT_Elegibality =null
              set @isNormalShift=null 
              set @chkmailforot=0
              set @otcount=0
              select @ReadEmpID = EID from StoreEmpID where Id =@EmpIDIndex---reading the employee id from storeEmpid table based on the index         
              --set  @ReadEmpID ='E002'
              set @ReadEmpName = NUll   
			  set @dol=null     
              select @ReadEmpName = Emp_Name from EmployeeMaster where Emp_Code = @ReadEmpID ---readin the employee name from employee table based on the employee id        
              select @empcat=Emp_Employee_Category from EmployeeMaster where Emp_Code = @ReadEmpID
              select @Comp_id = Emp_Company from EmployeeMaster where Emp_Code = @ReadEmpID --reading the comp id based on the employee id        
              select @Comp_Name = CompanyName from CompanyMaster where CompanyCode = @Comp_id ---reading the comp name based on the comp id        
              select @Dept_id = Emp_Department from EmployeeMaster where Emp_Code = @ReadEmpID---reading the dept id based on the employee id        
              select @Dept_Name = DeptName from DeptMaster where DeptCode = @Dept_id ---reading the dept name based on the dept id        
              select @Desig_id = Emp_Designation from EmployeeMaster where Emp_Code = @ReadEmpID    ----reading the desig id based on the employee id        
              select @Desig_Name = DesigName from DesigMaster where DesigCode = @Desig_id --reading the desig name based on the desig id        
              select @Cat_Code = Emp_Branch from EmployeeMaster where Emp_Code = @ReadEmpID  
              select @Cat_Name =BranchName   from BranchMaster where BranchCode = @Cat_Code  
              --select @holidaygroup=HolidayCode from  BranchMaster where BranchCode = @Cat_Code 
              select @contact= Emp_Phone from EmployeeMaster where Emp_Code=@ReadEmpID  
              select @DOJ=Emp_Doj from EmployeeMaster where Emp_Code=@ReadEmpID 
			  select @dol=Emp_Dol from EmployeeMaster where Emp_Code=@ReadEmpID and emp_status in ('3','2')
			     
              select @actual=actual from shiftsetting where CompanyCode=@Comp_id
              select @includep=includeprocess from EmployeeCategoryMaster where EmpCategoryCode=@empcat
              select @Employee_OT_Elegibality = OT_Eligibility from EmployeeMaster where Emp_Code = @ReadEmpID
              select @RequireDeduction = BreakDeductionRequired from ShiftSetting where CompanyCode = @Comp_id      
              Select @MinReqHrsForDed = datediff(Minute,0,convert(time,MinWorkHrsForDeduction)) from ShiftSetting where CompanyCode = @Comp_id       
              select @MinsToBeDeducted =TotalDeductionTime from ShiftSetting where CompanyCode = @Comp_id       

              select @isaso= 1  from ShiftSetting where isaso=1 and CompanyCode=@Comp_id 
              set @isaso= case when @isaso is null then 0 else @isaso end
              
              While(@PunchDateIndex <= @CountPunchDate)---Punchdate counter loop                
              Begin 
                      Truncate table process_data_daily      
                      Truncate table TempDay       
                      Truncate Table npbefore12  
                      truncate table nightpunches      
                      Truncate Table ProcessingPunches ---truncating the table which consists of the all the punches of a employee on a particular day        
                      set @ReadPDate = NUll  
                      set @statuswoh=null 
                      set @Status = null 
                      set @outcutoffmin=null       
                      select @ReadPDate = PunchDate1 from PDateDetails where Id = @PunchDateIndex --reads the first date based on the index        
                     
                     set @Totalhrstime =null    
                     set @ShiftCode = NULL    
                     set @ShiftCodeyes = NULL      
                     set @TotalHrs= NUll        
                     set @TotalHrsMin=NUll  
                     set @TotalHrsMin1=NUll      
                     set @LateComersMin = NUll 
                     set @LateComersMin1 = NUll     
                     set @LateComerstimeformat= NUll        
                     set @EarlyLeaversMin = NUll 
                     set @EarlyLeaversMin1 =Null      
                     set @EarlyLeaverstimeformat =NUll        
                     set @Overtimemin = NUll     
                     set @Overtimetimeformat = NUll    
                     set @ShiftIN = NUll        
                     set @ShiftOUT = NUll 
                     set @ShiftINyes = NUll        
                     set @ShiftOUTyes = NUll 
                     set @overtime=null
                     set @workhrs =null
                     set @TotalShiftTime =null 
                     set @shift_Name =null 
                     set @halfday_starttime=null  
                     set @halfday_endtime=null   
                     set @halfday =null
                     set @halfday_shift =null
                     set @breakin=null
                     set @breakout=null
                     set @sumtot1=convert(datetime,DATEADD(N,0,0))
                     set @initialrow=1
                     set @OutPunch=null
                     set @InPunch=null
                     set @WeeklyOff1=null
                     set @WeeklyOff2=null
                     set @sOvertimetimeformat=null
                     set @rtDayofWeek=null
                     set @ShiftIN_GraceInyes=null 
                     set    @ShiftOUT_GraceOutyes=null
                     set @ShiftIN_GraceIn = null
                     set @ShiftOUT_GraceOut =null
                     set @EarlyComersMin=null
                     set @EarlyComerstimeformat=null
               set @LateGoMin=null
               set @LateGotimeformat=null
               set @BoutEarlyBy = null
               set @BoutEarlyByMin =null
               set @BinLateBy = null
               set @BinLatebyMin = null
               set @DAteUpdate=null
               set @CheckTime=null
               set @shiftflag=null
               set @ShiftNightFlag=null
                  set @chkmailforot=0
                  set @otcount=0
                     set @month =DATEPART(MM, @ReadPDate)
                     set @year=DATEPART(YYYY, @ReadPDate)   
                     set @day=DATEPART(DD, @ReadPDate)
					 
					 ----Madhu code on 12042016 to exclude the processig for days<doj and days>dol
					 
					 if(@ReadPDate<@doj or @ReadPDate>@dol)
					 begin
					      set @PunchDateIndex = @PunchDateIndex + 1  
					      continue
					  end

					 ----Ends---- 
                     set @daytemp = 'day'                       
                     if exists(select 1 from ShiftEmployee where Empid=@ReadEmpID and Month=@month and year=@year)--------Integrating with ShiftRoster
                     begin
                           set @SqlQuery='insert into TempDay select '+ @daytemp + convert(char,@day)+ 'from ShiftEmployee where Empid=''' + @ReadEmpID + ''' and Month = ' + convert(char,@month) + 'and year = ' + convert(char,@year)
                           execute(@SqlQuery)
                           select @ShiftCode = ShiftCode from TempDay  
                           set @ShiftFlag=1  --setting this flag to check while considering  status as WOP OR P in case of week off  
                           if( @ShiftCode is null)
                                  begin
                                         select @ShiftCode = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID --reading the shiftcode from the employee table based on the employee id         
                                         set @ShiftFlag=0      
                                  end                     
                     end 
                     if( @ShiftCode is null)
                     begin
                           select @ShiftCode = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID --reading the shiftcode from the employee table based on the employee id         
                           set @ShiftFlag=0      
                     end 
                     
                     
                     --Auto Shift WO Start 
                     
                     if (@ShiftCode is NULL or @ShiftCode = '')
                     Begin
                           SELECT @rtDayofWeek = Datename(DW,@ReadPDate)
                           set @ReadPDateP=DATEADD(dd,-1,@ReadPDate)
                           if exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate=@ReadPDateP)
                                  Begin
                                         Select @ShiftCode= Shift_Code from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate=@ReadPDateP
                                                if (@ShiftCode is NULL)
                                                Begin
                                                       set @ReadPDatePP=DATEADD(dd,-1,@ReadPDateP)
                                                       if exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate=@ReadPDatePP)
                                                       Begin
                                                              Select @ShiftCode= Shift_Code from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate=@ReadPDatePP
                                                       End
                                                End
                                  End
                     End
                     
                     --Auto Shift WO End                 
                     
                     
                     ----Ramadan shift code start1
                     set @isramadan = null
                     --select @isramadan=isramadan from ShiftSetting
                     select @isramadan = isramadan from ShiftSetting where CompanyCode in( select Emp_Company from EmployeeMaster where Emp_Code=@ReadEmpID )
                     if(@isramadan='1')
                     begin
                           select @ramadanstdt=ramadanstdt,@ramadantodt=ramadantodt from ShiftSetting where CompanyCode in( select Emp_Company from EmployeeMaster where Emp_Code=@ReadEmpID )
                           if exists(select 1 from ShiftSetting where @ReadPDate between @ramadanstdt and @ramadantodt)
                           begin
                                  select @shift_Name=Shift_Desc, @ShiftIN = CONVERT(Time,Ramadan_InTime),@ShiftOUT =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode         
                                  select @TotalShiftTime= Ramadan_MaxOverTime_General from Shift Where Shift_Code=@ShiftCode
                                  set @ramadanflag='1'
                           end
                           else
                           begin
                                  select @shift_Name=Shift_Desc, @ShiftIN = CONVERT(Time,In_Time),@ShiftOUT =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode         
                                  select @TotalShiftTime= MaxOverTime_General from Shift Where Shift_Code=@ShiftCode
                                  set @ramadanflag='0'
                           end
                     end
                     else
                     begin                  
                           select @shift_Name=Shift_Desc, @ShiftIN = CONVERT(Time,In_Time),@ShiftOUT =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode         
                           select @TotalShiftTime= MaxOverTime_General from Shift Where Shift_Code=@ShiftCode
                           set @ramadanflag='0'
                     end                       
                     ----Ramadan shift code ends   
                     
                     select @isNormalShift= chkifNormalShift from Shift where Shift_Code=@ShiftCode
                     --select @shift_Name=Shift_Desc, @ShiftIN = CONVERT(Time,In_Time),@ShiftOUT =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode         
                     select @InGrace=GraceIn,@OutGrace=GraceOut,@Bout_Grace = Grace_Bout,@Bin_Grace = Grace_Bin  from Shift where Shift_Code = @ShiftCode
                     set @ShiftIN_GraceIn=DATEADD(MINUTE,@InGrace,@ShiftIN)
                     set @ShiftOUT_GraceOut1=DATEADD(MINUTE,-@OutGrace,@ShiftOUT)               
                     set @ShiftOUT_GraceOut = @ReadPDate + @ShiftOUT_GraceOut1
                     --select @ShiftTime = CONVERT(time,@ShiftOUT)
              --select @TotalShiftTime= MaxOverTime_General from Shift Where Shift_Code=@ShiftCode
                     set @halfday=DATENAME(DW,@ReadPDate) 
                     select @halfday_shift=Halfday from shift  where Shift_Code =@ShiftCode
                     if(@halfday=@halfday_shift)
                     begin                                  
                           ----Ramadan shift code start2                                              
                           if (@ramadanflag='0')
                           begin
                                  select @TotalShiftTime= worktime_halfday from Shift Where Shift_Code=@ShiftCode
                           end
                           else
                           begin
                                  select @TotalShiftTime= Ramadan_worktime_halfday from Shift Where Shift_Code=@ShiftCode
                           end                                                                   
                           ----Ramadan shift code ends              
                                                       
                           --select @TotalShiftTime= worktime_halfday from Shift Where Shift_Code=@ShiftCode
                           select @ShiftIN=CONVERT(Time,starttime_halfday)from Shift where Shift_Code=@ShiftCode
                     select @ShiftOUT =  CONVERT(Time,endtime_halfday)  from Shift where Shift_Code = @ShiftCode
                     end                             
                     set @month =DATEPART(MM, dateadd(dd,-1,@ReadPDate))
                     set @year=DATEPART(YYYY, dateadd(dd,-1,@ReadPDate))   
                     set @day=DATEPART(DD, dateadd(dd,-1,@ReadPDate))                                  
                     if exists(select 1 from ShiftEmployee where Empid=@ReadEmpID and Month=@month and year=@year)--------Integrating with ShiftRoster
                     begin
                           set @SqlQuery='insert into TempDay select '+ @daytemp + convert(char,@day)+ 'from ShiftEmployee where Empid=''' + @ReadEmpID + ''' and Month = ' + convert(char,@month) + 'and year = ' + convert(char,@year)
                           execute(@SqlQuery)
                           select @ShiftCodeyes = ShiftCode from TempDay   
                           set @shiftNightFlag=1
                           if(@ShiftCodeyes is null)
                                  begin
                                         select @ShiftCodeyes = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID --reading the shiftcode from the employee table based on the employee id         
                                         set @shiftNightFlag=0
                                  end                
                     end 
                     if(@ShiftCodeyes is null)
                     begin
                           select @ShiftCodeyes = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID --reading the shiftcode from the employee table based on the employee id         
                           set @shiftNightFlag=0
                     end                                 
                     if (@ramadanflag='0') -- Ramadan shift code
                     begin                             
                           select @shift_Nameyes=Shift_Desc, @ShiftINyes = CONVERT(Time,In_Time),@ShiftOUTyes =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCodeyes --getting the shiftin and shift out from the shift table based on the shiftcode         
                     end
                     else
                     begin
                           select @shift_Nameyes=Shift_Desc, @ShiftINyes = CONVERT(Time,Ramadan_InTime),@ShiftOUTyes =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCodeyes --getting the shiftin and shift out from the shift table based on the shiftcode         
                     end                                      
                     --select @shift_Nameyes=Shift_Desc, @ShiftINyes = CONVERT(Time,In_Time),@ShiftOUTyes =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCodeyes --getting the shiftin and shift out from the shift table based on the shiftcode         
        
                     select @InGrace=GraceIn,@OutGrace=GraceOut from Shift where Shift_Code = @ShiftCodeyes
                     set @ShiftIN_GraceInyes=DATEADD(MINUTE,@InGrace,@ShiftINyes)
                     set @ShiftOUT_GraceOutyes=DATEADD(MINUTE,-@OutGrace,@ShiftOUTyes)    
                     if (@ShiftINyes >@ShiftOUTyes) ---For Night Shift
                     begin  
                     select @shift_Name = Shift_Desc from Shift where Shift_Code = @ShiftCodeyes 
                 set @ShiftOUT_GraceOut1=DATEADD(MINUTE,-@OutGrace,@ShiftOUT)
                     set @ShiftOUT_GraceOut = dateadd(DAY,1,@ReadPDate) + @ShiftOUT_GraceOut1                                                                  
                           if Exists (select 1 from process_data where pdate = dateadd(DAY,-1,@ReadPDate) and EmpId = @ReadEmpID)--if record exists in the raw table for a particular employee and for a particular date        
                           Begin  
                           set @ShiftOUT_GraceOutyes = @ReadPDate + @ShiftOUT_GraceOutyes
                                  SELECT @rtDayofWeek =  Datename(DW,dateadd(dd,-1,@ReadPDate))
                                  set @WeeklyOff1=null
                                  set @WeeklyOff2=null
                           select @WeeklyOff1 = WeeklyOff1, @WeeklyOff2=WeeklyOff2 from shift where Shift_Code = @ShiftCodeyes --N2862013
                                  if(@actual=1)           
                                  begin
                                         insert into process_data_daily(Empid,pdate,In_punch,Out_punch,Sum) select Empid,pdate,In_punch,Out_punch,Sum from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID 
                                         if (@ramadanflag = '0')
                                         begin                      
                                                if (@isNormalShift ='0')
                                                begin
                                                
                                                    select @CheckTime= convert(time,in_time)+convert(datetime,ingrace) from shift where Shift_Code=@ShiftCodeyes
                                                    if( Convert(time,@CheckTime) >  Convert(time,DAteadd(HOUR,14,0)))
                                                              Begin
                                                                     set @DateUpdate=dateadd(DAY,-1,@ReadPDate)
                                                              End
                                                    Else
                                                              Begin
                                                                     set @DateUpdate=@ReadPDate
                                                              End
                                                    
                                                       select  @InPunch =min(In_punch) from process_data_daily where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID and In_punch<=(select convert(datetime,(@DateUpdate +convert(time,in_time)+convert(datetime,ingrace))) from shift where Shift_Code=@ShiftCodeyes) 
                                                       
                                                       ---To calculating the break out and break in punch from the range Start
                                                       select @incutoffmin=sum((datepart(HH,breakin)*60)+DATEPART(mi,breakin)+(datepart(HH,brkingrace)*60)+DATEPART(mi,brkingrace)) from shift where Shift_Code=@ShiftCodeyes
                                                       select @outcutoffmin=sum((datepart(HH,breakout)*60)+DATEPART(mi,breakout)+(datepart(HH,brkoutgrace)*60)+DATEPART(mi,brkoutgrace)) from shift where Shift_Code=@ShiftCodeyes
                                                       if(((select convert(time,Out_Time) from Shift where Shift_Code = @ShiftCodeyes) < (Select Convert(time,breakout)from Shift where Shift_Code =@ShiftCodeyes))and((Select Convert(time,breakout)from Shift where Shift_Code =@ShiftCodeyes)<Convert(time,'23:59:00.000')))
                                                       begin
                                                       select @breakoutcutoftime=DATEADD(MI,@outcutoffmin,DATEADD(DAY,-1,@ReadPDate))
                                                       end
                                                       else
                                                       begin
                                                       select @breakoutcutoftime=DATEADD(MI,@outcutoffmin,@ReadPDate)
                                                       end
                                                       if(((select convert(time,Out_Time) from Shift where Shift_Code = @ShiftCodeyes) < (Select Convert(time,breakin)from Shift where Shift_Code =@ShiftCodeyes))and ((Select Convert(time,breakin)from Shift where Shift_Code =@ShiftCodeyes)<Convert(time,'23:59:00.000')))
                                                       begin
                                                       select @breakincutoftime=DATEADD(MI,@incutoffmin,DATEADD(DAY,-1,@ReadPDate))
                                                       end
                                                       else
                                                       begin
                                                       select @breakincutoftime=DATEADD(MI,@incutoffmin,@ReadPDate)
                                                       end
                                                       select @incutoffmin1=sum(((datepart(HH,breakin)*60)+DATEPART(mi,breakin))-((datepart(HH,brkingrace)*60)+DATEPART(mi,brkingrace))) from shift where Shift_Code=@ShiftCodeyes
                                                       select @outcutoffmin1=sum(((datepart(HH,breakout)*60)+DATEPART(mi,breakout))-((datepart(HH,brkoutgrace)*60)+DATEPART(mi,brkoutgrace))) from shift where Shift_Code=@ShiftCodeyes
                                                       --NNNNNNNNN CHanges 
                                                       if(((select convert(time,Out_Time) from Shift where Shift_Code = @ShiftCodeyes) < (Select Convert(time,breakout)from Shift where Shift_Code =@ShiftCodeyes))and ((Select Convert(time,breakout)from Shift where Shift_Code =@ShiftCodeyes)<Convert(time,'23:59:00.000')))
                                                       begin
                                                       select @breakoutcutoftime1=DATEADD(MI,@outcutoffmin1,DATEADD(DAY,-1,@ReadPDate))
                                                       end
                                                       else
                                                       begin
                                                       select @breakoutcutoftime1=DATEADD(MI,@outcutoffmin1,@ReadPDate)
                                                       end
                                                       if(((select convert(time,Out_Time) from Shift where Shift_Code = @ShiftCodeyes) < (Select Convert(time,breakin)from Shift where Shift_Code =@ShiftCodeyes))and ((Select Convert(time,breakin)from Shift where Shift_Code =@ShiftCodeyes)<Convert(time,'23:59:00.000')))
                                                       begin
                                                       select @breakincutoftime1=DATEADD(MI,@incutoffmin1,DATEADD(DAY,-1,@ReadPDate))
                                                       end
                                                       else
                                                       begin
                                                       select @breakincutoftime1=DATEADD(MI,@incutoffmin1,@ReadPDate)                                                                           
                                                       end    
                                                       --NNNNNNNNN CHanges End                                              
                                                       select top 1 @breakout=Punch_Time from Trans_RawProcessDailyData where  Empid =@ReadEmpID and Punch_Time>=@breakoutcutoftime1 and Punch_Time<=@breakoutcutoftime  order by Punch_Time desc
                                                       select top 1 @breakin=Punch_Time from Trans_RawProcessDailyData where  Empid =@ReadEmpID and Punch_Time>=@breakincutoftime1 and Punch_Time<=@breakincutoftime order by Punch_Time asc
                                                       select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where  Empid =@ReadEmpID and Punch_Time>@breakincutoftime and convert(datetime,Punch_Time)<=(select convert(datetime,(@ReadPDate +convert(time,Out_Time)+CONVERT(time,outgrace)+convert(datetime,MaxWorkTime))) from shift where Shift_Code=@ShiftCodeyes) and PunchDate=@ReadPDate order by Punch_Time desc                                                                                          
                                                end
                                                else
                                                begin
                                                              select  @InPunch =min(In_punch) from process_data_daily where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID --and convert(time,In_punch)<=(select convert(time,convert(datetime,@ShiftIN)+convert(datetime,@ingrace1)))
                                                       set @breakout =null
                                                       set @breakin = null
                                                       select top 1 @OutPunch =Out_punch from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID order by id desc
                                                       if(@OutPunch is null)
                                                       begin 
                                                       select @OutPunch =MAX(In_punch) from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID
                                                              if(@InPunch =@OutPunch)
                                                                     begin                        
                                                                           set @OutPunch =null                        
                                                                     end
                                                       end
                                             end
                                                                     if(@isNormalShift !='0')
                                                begin  
                                                       select @countrow =count(id) from process_data_daily 
                                                       if(@InPunch is not null and @OutPunch is not null)
                                                              begin
                                                              while(@initialrow <=@countrow )
                                                              begin                                           
                                                                     select @tot1 =SUM from process_data_daily where id=@initialrow
                                                                     if ((@tot1 is not null) or (@tot1 !=''))
                                                                           begin
                                                                           set @sumtot1 =@sumtot1 +@tot1  
                                                                     end
                                                                     set @initialrow =@initialrow +1                                                   
                                                              end ----end while
                                                       end
                                                end
                                                       else if (@isNormalShift ='0')
                                                begin
                                                if(@InPunch is not null and @OutPunch is not null and @breakin is not null and @breakout is not null) 
                                                Begin  
                                                set @TempH1 = DATEDIFF(Minute,@InPunch,@breakout)
                                                set @tempH2 = DATEDIFF(Minute,@breakin,@OutPunch)
                                                set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                set @sumtot1 = DATEADD(N,@TempH,0)                                          
                                         End
                                                else if(@InPunch is not null and @breakout is null and @breakin is not null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =0
                                                              set @tempH2 = DATEDIFF(Minute,@breakin,@OutPunch)
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)       
                                                       end
                                                else if(@InPunch is not null and @breakout is not null and @breakin is null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =DATEDIFF(Minute,@InPunch,@breakout)
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)       
                                                       end 
                                                else if(@InPunch is not null and @breakout is null and @breakin is null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =DATEDIFF(Minute,@InPunch,@OutPunch)
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)       
                                                       end           
                                                else if(@InPunch is null and @breakout is not null and @breakin is not null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =0
                                                              set @tempH2 =DATEDIFF(Minute,@breakin,@OutPunch)
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0) 
                                                       end 
                                                else if(@InPunch is not null and @breakout is not null and @breakin is not null and @OutPunch is null)
                                                       begin
                                                              set @TempH1 =DATEDIFF(Minute,@InPunch,@breakout)
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0) 
                                                       end    
                                                else if(@InPunch is null  and @breakout is not null and @breakin is not null and @OutPunch is null)
                                                       begin
                                                              set @TempH1 =0
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)                                                
                                                       end 
                                                end
                                                
                                                --New Split Shift Pattren End
                                                                                                                                                       
                                                set @Totalhrstime =CONVERT(time,@sumtot1)                           
                                                set @TotalHrs = CONVERT(char(8),CONVERT(time,@Totalhrstime))
                                                set @TotalHrsMin=Datediff(MINUTE,0,@Totalhrstime)
                                                IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                                Begin
                                                       select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                       if @Employee_OT_Elegibality > 0
                                                       begin
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                          else
                                                          begin
                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                          end
                                                       end
                                                end
                                                else if exists(select 1 from holidaylist where hdate=dateadd(DAY,-1,@ReadPDate) and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))
                                                begin  
                                                       select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                           else
                                                          begin
                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                           end
                                                       
                                                end
                                                else
                                                begin
                                                       set @TotalHrs=CONVERT(char(8),convert(time,@sumtot1))        
                                                end    
                                         end
                                         else 
                                         begin
                                                set @breakin=null
                                                set @breakout=null
                                                select  @InPunch =min(In_punch) from process_data_daily where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID 
                                                select top 1 @OutPunch =Out_punch from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID order by id desc
                                                if (@OutPunch is null)
                                                begin
                                                       select @OutPunch =MAX(In_punch) from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID
                                                       if(@InPunch =@OutPunch)
                                                       begin
                                                              set @OutPunch =null
                                                       end
                                                end 
                                                
                                                select @countrow =count(id) from process_data_daily 
                                                if(@InPunch is not null and @OutPunch is not null)
                                                begin
                                                while(@initialrow <=@countrow )
                                                begin
                                                       select @tot1 =SUM from process_data_daily where id=@initialrow
                                                       if ((@tot1 is not null) or (@tot1 !=''))
                                                       begin
                                                              set @sumtot1 =@sumtot1 +@tot1  
                                                       end
                                                       set @initialrow =@initialrow +1
                                                end ----end while
                                                end
                                                set @Totalhrstime =CONVERT(time,@sumtot1)                           
                                                set @TotalHrs = CONVERT(char(8),CONVERT(time,@Totalhrstime))    
                                                set @TotalHrsMin=Datediff(MINUTE,0,@Totalhrstime)
                                                IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                                Begin
                                                if((@TotalHrsMin >=@MinReqHrsForDed) and(@RequireDeduction  =1))
                                                begin
                                                       set @TotalHrsMin =@TotalHrsMin -@MinsToBeDeducted                                                 
                                                end
                                                       select @sOvertimetimeformat=CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
                                                       if @Employee_OT_Elegibality > 0
                                                       begin
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                           else
                                                          begin
                                                             update overtime  SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                          end
                                                       end
                                                end
                                                else if exists(select 1 from holidaylist where hdate=@ReadPDate and hgroup in(select HolidayCode from BranchMaster where BranchCode = @Cat_Code))
                                                begin  
                                                       select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                       if @Employee_OT_Elegibality > 0
                                                       begin
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                           else
                                                          begin
                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                          end
                                                       end
                                                end
                                                
                                                       end--end of @ramadanflag false part                    
                                  end ----true part of if(@actual=1) end        
                                  else
                                  begin ----false part of if(@actual=1) start     
                                         select  @InPunch =min(In_punch) from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID 
                                         select top 1 @OutPunch =Out_punch from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID order by id desc
                                         if (@OutPunch is null)
                                         begin
                                                select @OutPunch =MAX(In_punch) from process_data where pdate =dateadd(DAY,-1,@ReadPDate) and Empid =@ReadEmpID
                                                if(@InPunch =@OutPunch)
                                                begin
                                                       set @OutPunch =null
                                                end
                                         end  
                                              if(@InPunch is not null and @OutPunch is not null)        
                                                Begin                                              
                                                       select @breakminute=DATEPART(HOUR, breakhrs) * 60 + DATEPART(MINUTE, breakhrs) From shiftsetting  where CompanyCode=@Comp_id
                                                       select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)
                                                       select @WeeklyOff1 = WeeklyOff1, @WeeklyOff2=WeeklyOff2 from shift where Shift_Code = @ShiftCodeyes --N2862013
                                                       IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2 or @ShiftCodeyes='woff' or (select 1 from holidaylist where hdate=DATEADD(DAY,-1,@ReadPDate)and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))=1) --changes on 14032014
                                                              Begin 
                                                                 set @TotalHrsMin=@TotalHrsMin
                                                              End
                                                       Else
                                                              Begin
                                                                     set @TotalHrsMin=@TotalHrsMin-@breakminute   
                                                              End
                                                       
                                                       if(@TotalHrsMin >0)
                                                       begin     
                                                       select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                                       set @Totalhrstime=CONVERT(time,@TotalHrs)        
                                                       end
                                                       else
                                                       begin 
                                                       set @TotalHrsMin = 0
                                                       select @TotalHrs = CONVERT(char(8),DATEADD(n,0,0),108)--calculating the Total Hrs.
                                                       set @Totalhrstime=CONVERT(time,@TotalHrs)   
                                                       end    
                                                end    
                                         
                                  end ----false part of if(@actual=1) ends 
                                  
                                       
                                  --NNN=Caluclation for WOP,HP& Workrd Hours Start
                                  
                                  if(@InPunch is not null and @OutPunch is not null)        
                                         Begin
                                                select @breakminute=DATEPART(HOUR, breakhrs) * 60 + DATEPART(MINUTE, breakhrs) From shiftsetting  where CompanyCode=@Comp_id
                                                IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                                Begin 
                                                       IF(@ShiftNightFlag=1 and @isaso=0)
                                                              Begin
                                                                     set @statuswoh='P'
                                                              End
                                                        Else
                                                              Begin
                                                                     set @statuswoh='WOP'
                                                                     if((@TotalHrsMin >= @MinReqHrsForDed)  and (@RequireDeduction =1))
                                                                           begin
                                                                                  set  @TotalHrsMin=@TotalHrsMin - @MinsToBeDeducted 
                                                                           end 
                                                                     select @sOvertimetimeformat=CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                                                     
                                                                       select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                                       if(@otcount=0)
                                                                        begin
                                                                             insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                             set @chkmailforot=1
                                                                        end
                                                                         else
                                                                        begin
                                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                                        end
                                                                     
                                                              End 
                                                       
                                                       
                                                       select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                                       
                                                end
                                                else if exists(select 1 from holidaylist where hdate=DATEADD(DAY,-1,@ReadPDate)and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))
                                                begin
                                                       set @statuswoh='HP'
                                                
                                                              if((@TotalHrsMin >= @MinReqHrsForDed)  and (@RequireDeduction =1))
                                                       begin
                                                           set  @TotalHrsMin=@TotalHrsMin - @MinsToBeDeducted 
                                                       end  
                                                       select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs. 
                                                       select @sOvertimetimeformat=CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                                       
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                           else
                                                          begin
                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                          end
                                                       
                                                end
                                                else if(@ShiftCodeyes ='woff')
                                                begin
                                                set @statuswoh='WOP'
                                                       select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                                       select @sOvertimetimeformat=CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                                       
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                           else
                                                          begin
                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                          end
                                                       
                                                end
                                                else   
                                                begin                                
                                                              select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                                       set @Totalhrstime=CONVERT(time,@TotalHrs)                                        
                                                end      
                                         end                               
                                  
                                  --NNN=Caluclation for WOP,HP& Workrd Hours  End 
                                           
                                  if(@InPunch > (@ReadPDate-1+ @ShiftIN_GraceInyes))--using datetime instead of time done on 08032014 by vinay
                                  begin
                                         select  @LateComersMin = Datediff(Minute,(@ReadPDate-1+@ShiftINyes),@InPunch)        
                                         select  @LateComerstimeformat = Convert(char(8),DateAdd(n,@LateComersMin,0),108)--calculation for the latecomers        
                                  end
                                  if(@InPunch < (@ReadPDate-1+@ShiftINyes))
                                  begin                      
                                         select  @EarlyComersMin = Datediff(Minute,@InPunch,(@ReadPDate-1+@ShiftINyes))        
                                         select  @EarlyComerstimeformat = Convert(char(8),DateAdd(n,@EarlyComersMin,0),108)--calculation for the EarlyComers        
                                  end
                                  
                                  --Breakout Early by Cal
                                  if(@breakout is not null)
                                  begin
                                                select  @ReadDateTemp=case when Convert(datetime,breakout)>DATEADD(HH,12,0) then @ReadPDate-1 else @ReadPDate end  from shift where Shift_Code=@ShiftCodeyes--changed by vinay08032014
                                                if(@breakout<(select (@ReadDateTemp+DATEADD(MINUTE,-Grace_Bout,breakout)) from Shift where Shift_Code=@ShiftCodeyes))                               
                                                       begin 
                                                              select @BoutEarlyByMin = DATEDIFF(Minute,@breakout,(@ReadDateTemp+breakout)) from Shift where Shift_Code = @ShiftCodeyes               
                                                              select  @BoutEarlyBy = Convert(char(8),DateAdd(n,@BoutEarlyByMin,0),108)--calculation for the EarlyComers        
                                                       END
                                                set    @ReadDateTemp=null
                                  End
                                  --Breakout Early by Cal End 
                                                                                                                                          
                                  if(@TotalHrs is not null or @TotalHrs !='')        
                                  Begin
                                         --select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCodeyes 
                                         if(@includep=1) 
                                         begin
                                                select @workhrs =CONVERT(time(7),Totalhrs) from employeecategorymaster where empcategorycode=@empcat
                                         end
                                         else
                                         begin                                  
                                                ----Ramadan shift code start3                                                  
                                                if (@ramadanflag='0')
                                                begin
                                                       select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCodeyes 
                                                end
                                                else
                                                begin
                                                       select @workhrs =Ramadan_MaxOverTime_General from shift where Shift_Code =@ShiftCodeyes 
                                                end                                             
                                                ----Ramadan shift code ends 
                                                --select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCode
                                         end
                                         if ((@Totalhrstime >@workhrs) and (@Employee_OT_Elegibality >0) and (@sOvertimetimeformat is null)) 
                                         begin                                  
                                                set @overtime=convert(datetime,@Totalhrstime)-convert(datetime,@workhrs) 
                                                if (@includep !=1 or @includep is null)
                                                begin
                                                       select @maxovertime=MaxWorkTime from shift where Shift_Code =@ShiftCodeyes
                                                       select @minovertime = minovertime from Shift where Shift_Code =@ShiftCodeyes                                            
                                                       if @maxovertime !=''
                                                       begin 
                                                              if @overtime >@maxovertime 
                                                              begin
                                                                     set @overtime =@maxovertime
                                                              end  
                                                       end                                             
                                                       if @minovertime!=''
                                                       begin
                                                              if @overtime <@minovertime 
                                                              begin
                                                                     set @overtime =null
                                                              end
                                                              
                                                       end                                             
                                                       if ((@maxovertime ='00:00:00.000') and (@minovertime='00:00:00.000')) 
                                                       begin
                                                              set @overtime =null
                                                       end
                                                end ----part of @includep !=1 ends
                                                --------------CODE CHANGE ON 14102014 BY VIKASH TO INSERT DATA IN OVERTIME TABLE----------
                                                              
                                             select @otcount= count(*) from overtime where empid=@readempid and otdate=dateadd(DAY,-1,@ReadPDate)
                                                 --if(@otcount=0)
                                                 --  begin
                                                 --        insert into Overtime(EMPID, OTDate, OTHrs, Flag,Mflag, EmpName) values(@ReadEmpID,dateadd(DAY,-1,@ReadPDate),convert(varchar(5),@overtime),1,1,@ReadEmpName)
                                                 --        set @chkmailforot=1
                                                 --  end
                                                 --   else
                                                 --  begin
                                                 --     update overtime set OTHrs=Convert(varchar(5),convert(time, @overtime)) where EMPID=@ReadEmpID and OTDate=dateadd(DAY,-1,@ReadPDate)
                                                 --  end
                                                -----------------------CODE CHANGE END--------------------------------------------------------                                              
                                                set @Overtimetimeformat=CONVERT(varchar,@overtime)
                                         end ------end if(@Totalhrstime >@workhrs )                                     
                                         if(@OutPunch < @ShiftOUT_GraceOutyes)         
                                         Begin                              
                                                select @EarlyLeaversMin = Datediff(Minute,@OutPunch,(@ReadPDate+@ShiftOUTyes))        
                                                select @EarlyLeaverstimeformat = CONVERT(char(8),Dateadd(n,@EarlyLeaversMin,0),108)--Calculation for the early leavers        
                                         End  
                                         if(@OutPunch > (@ReadPDate+@ShiftOUTyes))         
                                         Begin  
                                                select @LateGoMin = Datediff(Minute,(@ReadPDate+@ShiftOUTyes),@OutPunch)        
                                                select @LateGotimeformat = CONVERT(char(8),Dateadd(n,@LateGoMin,0),108)--Calculation for the Late leavers        
                                         End        
                                  end ------end if (@TotalHrs is not null and @TotalHrs is not null)                        
                                  if((@ShiftINyes  >  @ShiftOUTyes) and (@EarlyLeaverstimeformat is not null))        
                                  Begin  
                                         select @EarlyLeaversMin = Datediff(Minute,@OutPunch,(@ReadPDate+@ShiftOUTyes))        
                                         select @EarlyLeaverstimeformat = CONVERT(char(8),Dateadd(n,@EarlyLeaversMin,0),108)--Calculation for the early leavers        
                                  End 
                                  
                                  --calculation for the Break in Late By     
                                         if(@breakin is not null)
                                                begin
                                                select  @ReadDateTemp=case when Convert(datetime,breakin)>DATEADD(HH,12,0) then @ReadPDate-1 else @ReadPDate end  from shift where Shift_Code=@ShiftCodeyes--chabged by vinay08032014
                                                        if(@breakin>(select (@ReadDateTemp+DATEADD(MINUTE,Grace_Bin,breakin)) from Shift where Shift_Code=@ShiftCodeyes))                               
                                                              begin 
                                                                     select @BinLatebyMin = DATEDIFF(Minute,(@ReadDateTemp+breakin),@breakin) from Shift where Shift_Code = @ShiftCodeyes                 
                                                                     select  @BinLateBy = Convert(char(8),DateAdd(n,@BinLatebyMin,0),108)--calculation for the EarlyComers        
                                                        END
                                                        set @ReadDateTemp=null
                                                 End
                               --calculation for the Break in Late By 
                               
                                  ---New Changes to set the @Status in case of @actual=1 is true start
                                  IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                  Begin 
                                         IF(@ShiftNightFlag=1 and @isaso=0)
                                                Begin
                                                       set @statuswoh='P'
                                                End
                                          Else
                                                Begin
                                                       set @statuswoh='WOP'
                                                End 
                                  End
                                  ELSE IF exists(select 1 from holidaylist where hdate=Dateadd(DAY,-1,@ReadPDate) and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))
                                  Begin
                                         set @statuswoh='HP'
                                  End
                                  ---New Changes to set the @Status in case of @actual=1 is true Ends
                                  if exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate =dateadd(DAY,-1,@ReadPDate))
                                  begin                        
                                         if (@OutPunch is null)
                                         begin                          
                                                set @statuswoh='MS'
                                                update MASTERPROCESSDAILYDATA set In_Punch =@InPunch,Out_Punch=@OutPunch,WorkHRs=@TotalHrs,LateBy=@LateComerstimeformat,EarlyBy =@EarlyLeaverstimeformat,OT =@Overtimetimeformat,status=@statuswoh,Shift_Code =@ShiftCodeyes,Shift_In =@ShiftINyes,Shift_Out =@ShiftOUTyes,Dept_Name =@Dept_Name ,Desig_Name =@Desig_Name,contact=@contact,Shift_Name=@shift_Name,BreakOut=@breakout,BreakIn=@breakin,OT_Start=@ShiftOUTyes,EarlyCome=@EarlyComerstimeformat,LateGo=@LateGotimeformat,sot=@sOvertimetimeformat,BreakOut_EarlyBy = @BoutEarlyBy,BreakIn_LateBy = @BinLateBy where Emp_ID=@ReadEmpID and PDate=dateadd(DAY,-1,@ReadPDate) 
                                         end
                                         else
                                         begin
                                                if(@statuswoh is null)
                                                begin
                                                       set @statuswoh='P'
                                                end
                                                update MASTERPROCESSDAILYDATA set In_Punch =@InPunch,Out_Punch=@OutPunch,WorkHRs=@TotalHrs,LateBy=@LateComerstimeformat,EarlyBy =@EarlyLeaverstimeformat,OT =@Overtimetimeformat,status=@statuswoh,Shift_Code =@ShiftCodeyes,Shift_In =@ShiftINyes,Shift_Out =@ShiftOUTyes,Dept_Name =@Dept_Name ,Desig_Name =@Desig_Name,contact=@contact,Shift_Name=@shift_Name,BreakOut=@breakout,BreakIn=@breakin,OT_Start=@ShiftOUTyes,EarlyCome=@EarlyComerstimeformat,LateGo=@LateGotimeformat,sot=@sOvertimetimeformat,BreakOut_EarlyBy = @BoutEarlyBy,BreakIn_LateBy = @BinLateBy where Emp_ID=@ReadEmpID and PDate=dateadd(DAY,-1,@ReadPDate) 
                                         end
                                         --update MASTERPROCESSDAILYDATA set In_Punch =@InPunch,Out_Punch=@OutPunch,WorkHRs=@TotalHrs,LateBy=@LateComerstimeformat,EarlyBy =@EarlyLeaverstimeformat,OT =@Overtimetimeformat,status='P' where Emp_ID=@ReadEmpID and PDate=dateadd(DAY,-1,@ReadPDate) 
                                  end
                                  else
                                  begin
                                         --insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ) values(@ReadEmpID,@ReadEmpName,dateadd(DAY,-1,@ReadPDate),@InPunch,@OutPunch,'P',@TotalHrs,@LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ)         
                                         if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate =@ReadPDate)                                     
                                         begin
                                                if (@OutPunch is null)
                                                begin
                                                       set @statuswoh='MS'
                                                       insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUTyes,@Overtimetimeformat,@ShiftINyes,@ShiftOUTyes,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCodeyes,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                                       --insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                                end
                                                else
                                                begin
                                                       if(@statuswoh is null)
                                                       begin
                                                              set @statuswoh='P'
                                                       end
                                                       insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUTyes,@Overtimetimeformat,@ShiftINyes,@ShiftOUTyes,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCodeyes,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                                       --insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                                end
                                         end                       
                                                else
                                         begin
                                                --insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ) values(@ReadEmpID,@ReadEmpName,dateadd(DAY,-1,@ReadPDate),@InPunch,@OutPunch,'P',@TotalHrs,@LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ)         
                                                if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate =@ReadPDate)
                                                begin
                                                       if (@OutPunch is null)
                                                       begin
                                                              set @statuswoh='MS'
                                                              insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUTyes,@Overtimetimeformat,@ShiftINyes,@ShiftOUTyes,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCodeyes,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                                              --insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                                       end
                                                       else
                                                       begin
                                                              if(@statuswoh is null)
                                                       begin
                                                              set @statuswoh='P'
                                                       end       
                                                       insert MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUTyes,@Overtimetimeformat,@ShiftINyes,@ShiftOUTyes,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCodeyes,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)                                  
                                                       --insert MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@statuswoh,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)                                  
                                                end
                                         end                          
                                  end  
                           end                                  
                     end                         
                     end
                                   
                     Truncate table process_data_daily   
                     set @InPunch=null
                     set @OutPunch=null
                     set @breakin=null
                     set @breakout=null
                     set @TotalHrs=null
                     set @TotalHrsMin=null
                     set @LateComerstimeformat=null
                     set @Overtimetimeformat=null
                     set @Totalhrstime=null     
                     set @sOvertimetimeformat=null     
                     set @initialrow =1   
                     set @sumtot1 = 0     
                     set @EarlyLeaverstimeformat = null
                     set @LateComerstimeformat=null
                     --Bout Earlyby & Bin Late By Cal
                     set @BoutEarlyBy=null
                     set @BoutEarlyByMin=0
                     set @BinLateBy=null
                     set @BinLatebyMin = 0
                     select @shift_Name = Shift_Desc from Shift where Shift_Code = @ShiftCode
                     --Bout Earlyby & Bin Late By Cal End 
                     SELECT @rtDayofWeek =  Datename(DW,@ReadPDate)
                     if Exists (select 1 from process_data where pdate = @ReadPDate and EmpId = @ReadEmpID)--if record exists in the raw table for a particular employee and for a particular date        
                     Begin                     
                           if(@actual=1)            
                           begin
                                  insert into process_data_daily(Empid,pdate,In_punch,Out_punch,Sum) select Empid,pdate,In_punch,Out_punch,Sum from process_data where pdate =@ReadPDate and Empid =@ReadEmpID 
                                  if (@ramadanflag = '0')
                                  begin  
                                         if(@isNormalShift = '0')
                                         begin
                                                select  @InPunch =min(In_punch) from process_data_daily where pdate =@ReadPDate and Empid =@ReadEmpID and convert(time,In_punch)<=(select convert(time,convert(datetime,in_time)+convert(datetime,ingrace)) from shift where Shift_Code=@ShiftCode)
                                                select @incutoffmin=sum((datepart(HH,breakin)*60)+DATEPART(mi,breakin)+(datepart(HH,brkingrace)*60)+DATEPART(mi,brkingrace)) from shift where Shift_Code=@ShiftCode
                                                select @outcutoffmin=sum((datepart(HH,breakout)*60)+DATEPART(mi,breakout)+(datepart(HH,brkoutgrace)*60)+DATEPART(mi,brkoutgrace)) from shift where Shift_Code=@ShiftCode
                                                select @breakoutcutoftime=DATEADD(MI,@outcutoffmin,@ReadPDate)
                                                select @breakincutoftime=DATEADD(MI,@incutoffmin,@ReadPDate)
                                                
                                                select @incutoffmin1=sum(((datepart(HH,breakin)*60)+DATEPART(mi,breakin))-((datepart(HH,brkingrace)*60)+DATEPART(mi,brkingrace))) from shift where Shift_Code=@ShiftCode
                                                select @outcutoffmin1=sum(((datepart(HH,breakout)*60)+DATEPART(mi,breakout))-((datepart(HH,brkoutgrace)*60)+DATEPART(mi,brkoutgrace))) from shift where Shift_Code=@ShiftCode
                                                select @breakoutcutoftime1=DATEADD(MI,@outcutoffmin1,@ReadPDate)
                                                select @breakincutoftime1=DATEADD(MI,@incutoffmin1,@ReadPDate)                                                                           
                                                
                                                select top 1 @breakout=Punch_Time from Trans_RawProcessDailyData where  Empid =@ReadEmpID and Punch_Time>=@breakoutcutoftime1 and Punch_Time<=@breakoutcutoftime  order by Punch_Time desc
                                                select top 1 @breakin=Punch_Time from Trans_RawProcessDailyData where  Empid =@ReadEmpID and Punch_Time>=@breakincutoftime1 and Punch_Time<=@breakincutoftime order by Punch_Time asc
                                                ---To calculating the break out and break in punch from the range End
                                         end
                                         else
                                                       begin
                                                                           select  @InPunch =min(In_punch) from process_data_daily where pdate =@ReadPDate and Empid =@ReadEmpID  --and convert(time,In_punch)<=(select convert(time,convert(datetime,@ShiftIN)+convert(datetime,@ingrace1)))
                                                                     
                                                       end
                                      if(@isNormalShift = '0')
                                           begin
                                                       select @maxovertime=MaxWorkTime from shift where Shift_Code =@ShiftCode
                                                select @minovertime = minovertime from Shift where Shift_Code =@ShiftCode
                                                if ((@maxovertime !='00:00:00.000') and (@minovertime !='00:00:00.000')) 
                                                              begin
                                                                     select @lastgracetime=convert(time,convert(datetime,Out_Time)+convert(datetime,MaxWorkTime)) from shift where Shift_Code=@ShiftCode
                                                                     if (@lastgracetime <= '06:00:00.000')
                                                                           begin
                                                                                  set @lastgracetime = '23:59:00.000'
                                                                                  select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(convert(time,@lastgracetime)) order by Punch_Time desc
                                                                           end
                                                                     else
                                                                           begin
                                                                                  select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(select convert(time,convert(datetime,Out_Time)+convert(datetime,MaxWorkTime)) from shift where Shift_Code=@ShiftCode ) order by Punch_Time desc
                                                                           end                                                                                                   
                                                              end    
                                                else                                     
                                                       begin                                                  
                                                              select @lastgracetime=convert(time,convert(datetime,Out_Time)+convert(datetime,outgrace)) from shift where Shift_Code=@ShiftCode
                                                              if (@lastgracetime <= '02:00:00.000')
                                                                     begin
                                                                           set @lastgracetime = '23:59:00.000'
                                                                           select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(convert(time,@lastgracetime)) order by Punch_Time desc
                                                                     end
                                                              else
                                                                     begin
                                                                           select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(select convert(time,convert(datetime,Out_Time)+convert(datetime,outgrace)) from shift where Shift_Code=@ShiftCode ) order by Punch_Time desc
                                                                     end
                                                       end
                                                ----To calculate the Out punch if OverTime range is defined Ends
                                                ----select top 1 @OutPunch=Punch_Time from Trans_Raw# where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(select convert(time,convert(datetime,Out_Time)+convert(datetime,outgrace)) from shift where Shift_Code=@ShiftCode ) order by Punch_Time desc
                                         end
                                         else
                                         begin
                                                select @maxovertime=MaxWorkTime from shift where Shift_Code =@ShiftCode
                                                select @minovertime = minovertime from Shift where Shift_Code =@ShiftCode
                                                if ((@maxovertime !='00:00:00.000') and (@minovertime !='00:00:00.000')) 
                                                       begin
                                                              select @lastgracetime=convert(time,convert(datetime,Out_Time)+convert(datetime,MaxWorkTime)) from shift where Shift_Code=@ShiftCode
                                                              if (@lastgracetime <= '06:00:00.000')
                                                                     begin
                                                                           set @lastgracetime = '23:59:00.000'
                                                                           select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(convert(time,@lastgracetime)) order by Punch_Time desc
                                                                     end
                                                              else
                                                                     begin
                                                                           select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(select convert(time,convert(datetime,Out_Time)+convert(datetime,MaxWorkTime)) from shift where Shift_Code=@ShiftCode ) order by Punch_Time desc
                                                                     end                                                                                                    
                                                       end    
                                                else                                     
                                                begin                                                  
                                                       select @lastgracetime=convert(time,convert(datetime,Out_Time)+convert(datetime,outgrace)) from shift where Shift_Code=@ShiftCode
                                                       if (@lastgracetime <= '02:00:00.000')
                                                       begin
                                                              set @lastgracetime = '23:59:00.000'
                                                              select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(convert(time,@lastgracetime)) order by Punch_Time desc
                                                       end
                                                       else
                                                       begin
                                                              select top 1 @OutPunch=Punch_Time from Trans_RawProcessDailyData where PunchDate=@ReadPDate and Empid =@ReadEmpID and convert(time,Punch_Time)>(select convert(time,convert(datetime,breakin)+convert(datetime,brkingrace)) from shift  where Shift_Code=@ShiftCode) and CONVERT(time,Punch_Time)<=(select convert(time,convert(datetime,Out_Time)+convert(datetime,outgrace)) from shift where Shift_Code=@ShiftCode ) order by Punch_Time desc
                                                       end
                                                end
                                     
                     if (@OutPunch is null)
                                         begin                      
                                         select @OutPunch =MAX(In_punch) from process_data where pdate =@ReadPDate and Empid =@ReadEmpID
                                                if(@InPunch =@OutPunch)
                                                       begin                        
                                                              set @OutPunch =null                        
                                                       end                      
                                                end
                                         end
                                         --select top 1 @OutPunch =Out_punch from process_data where pdate =@ReadPDate and Empid =@ReadEmpID order by id desc
                                         if @isNormalShift ='0'
                                         begin
                                         if(@InPunch=@OutPunch)
                                         begin
                                                set @OutPunch =null
                                         end                        
                                                                     if(@isNormalShift !='0')
                                         begin                                    
                                                if(@InPunch is not null and @OutPunch is not null) 
                                                Begin                
                                                       select @countrow =count(id) from process_data_daily                                        
                                                       while(@initialrow <=@countrow)
                                                       begin                     
                                                              select @tot1 =SUM from process_data_daily where id=@initialrow                                                               
                                                              if ((@tot1 is not null) or (@tot1 !=''))
                                                              begin
                                                                     set @sumtot1 =@sumtot1 +@tot1  
                                                              end
                                                              set @initialrow =@initialrow +1                              
                                                       end ----end while
                                                End           
                                         end
                                         --Total Hour Calculation For Normal Shift & Actual =1 end
                                         --New Split Shift Pattren End
                                         else if(@isNormalShift ='0')
                                         begin
                                         if(@InPunch is not null and @OutPunch is not null and @breakin is not null and @breakout is not null) 
                                                Begin  
                                                set @TempH1 = DATEDIFF(Minute,@InPunch,@breakout)
                                                set @tempH2 = DATEDIFF(Minute,@breakin,@OutPunch)
                                                set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                set @sumtot1 = DATEADD(N,@TempH,0)                                          
                                                                     End
                                                else if(@InPunch is not null and @breakout is null and @breakin is not null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =0
                                                              set @tempH2 = DATEDIFF(Minute,@breakin,@OutPunch)
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)       
                                                       end
                                                else if(@InPunch is not null and @breakout is not null and @breakin is null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =DATEDIFF(Minute,@InPunch,@breakout)
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)       
                                                       end 
                                                else if(@InPunch is not null and @breakout is null and @breakin is null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =DATEDIFF(Minute,@InPunch,@OutPunch)
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)       
                                                       end           
                                                else if(@InPunch is null and @breakout is not null and @breakin is not null and @OutPunch is not null)
                                                       begin
                                                              set @TempH1 =0
                                                              set @tempH2 =DATEDIFF(Minute,@breakin,@OutPunch)
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0) 
                                                       end 
                                                else if(@InPunch is not null and @breakout is not null and @breakin is not null and @OutPunch is null)
                                                       begin
                                                              set @TempH1 =DATEDIFF(Minute,@InPunch,@breakout)
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0) 
                                                       end    
                                                else if(@InPunch is null  and @breakout is not null and @breakin is not null and @OutPunch is null)
                                                       begin
                                                              set @TempH1 =0
                                                              set @tempH2 =0
                                                              set @TempH = @TempH1+@tempH2                                                                                                                                                                                                                                                
                                                              set @sumtot1 = DATEADD(N,@TempH,0)                                                
                                                       end 
                                                end
                                                       --New Split Shift Pattren End     
                                                       
                                         end
                                         else 
                                                begin 
                                                select @InPunch =min(In_punch) from process_data_daily where pdate =@ReadPDate and Empid =@ReadEmpID
                                                select top 1 @OutPunch =Out_punch from process_data where pdate =@ReadPDate and Empid =@ReadEmpID order by id desc
                                                if (@OutPunch is null)
                                                       begin
                                                              select @OutPunch =MAX(In_punch) from process_data where pdate =@ReadPDate and Empid =@ReadEmpID
                                                       if(@InPunch =@OutPunch)
                                                              begin
                                                                     set @OutPunch =null
                                                              end
                                                       end
                                                if(@InPunch is not null and @OutPunch is not null)
                                                       Begin
                                                       
                                                       select @countrow =count(id) from process_data_daily        
                                                       if(@InPunch is not null and @OutPunch is not null)
                                                       begin                           
                                                       while(@initialrow <=@countrow)
                                                       begin                     
                                                              select @tot1 =SUM from process_data_daily where id=@initialrow                                                               
                                                              if ((@tot1 is not null) or (@tot1 !=''))
                                                              begin
                                                                     set @sumtot1 =@sumtot1 +@tot1  
                                                              end
                                                              set @initialrow =@initialrow +1                              
                                                       end ----end while    
                                                              end
                                                       end
                                                end                                                                         
                                         --set @TotalHrs=CONVERT(char(8),convert(time,@sumtot1))      --nnnnnn
                                         set @Totalhrstime =CONVERT(time,@sumtot1)
                                         set @TotalHrs = CONVERT(char(8),convert(time,@Totalhrstime))
                                         SELECT @rtDayofWeek =  Datename(DW,@ReadPDate)
                                         set @WeeklyOff1=null
                                         set @WeeklyOff2=null
                                         select @WeeklyOff1=WeeklyOff1, @WeeklyOff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode
                                         IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                         Begin 
                                                IF(@ShiftFlag=1 and @isaso=0)
                                                       Begin
                                                              set @Status='P'
                                                       End
                                                 Else
                                                       Begin
                                                              set @status='WOP'
                                                              select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                              
                                                                select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                                if(@otcount=0)
                                                                 begin
                                                                       insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                       set @chkmailforot=1
                                                                 end
                                                                 else
                                                                 begin
                                                                    update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                                 end
                                                              
                                                       End                                                    
                                         end
                                         else if exists(select 1 from holidaylist where hdate=@ReadPDate and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))
                                         begin
                                                set @Status='HP'
                                                                                         
                                                select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                
                                                  select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                  if(@otcount=0)
                                                   begin
                                                         insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                         set @chkmailforot=1
                                                   end
                                                    else
                                                   begin
                                                      update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                   end
                                                
                                         end
                                         else if(@ShiftCode ='woff')
                                         begin
                                                set @Status='WOP'
                                                                                  
                                                select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                           
                                                  select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                  if(@otcount=0)
                                                   begin
                                                         insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                         set @chkmailforot=1
                                                   end
                                                    else
                                                   begin
                                                      update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                   end
                                                
                                         end
                                         else
                                         begin                                           
                                                set @TotalHrs=CONVERT(char(8),convert(time,@sumtot1))    
                                         end                     
                                  end
                                  else
                                  begin
                                         set @breakin=null
                                         set @breakout=null
                                         select  @InPunch =min(In_punch) from process_data where pdate =@ReadPDate and Empid =@ReadEmpID 
                                         select top 1 @OutPunch =Out_punch from process_data where pdate =@ReadPDate and Empid =@ReadEmpID order by id desc
                                         if (@OutPunch is null)
                                         begin                      
                                                select @OutPunch =MAX(In_punch) from process_data where pdate =@ReadPDate and Empid =@ReadEmpID
                                                if(@InPunch =@OutPunch)
                                                begin                        
                                                       set @OutPunch =null
                                                end
                                         end   
                                         if(@InPunch is not null and @OutPunch is not null)
                                         begin                                    
                                         select @countrow =count(id) from process_data_daily                                     
                                         if(@InPunch is not null and @OutPunch is not null)
                                         begin
                                         while(@initialrow <=@countrow)
                                         begin                     
                                                select @tot1 =SUM from process_data_daily where id=@initialrow                                                                     
                                                if ((@tot1 is not null) or (@tot1 !=''))
                                                begin
                                                       set @sumtot1 =@sumtot1 +@tot1  
                                                end
                                                set @initialrow =@initialrow +1                              
                                         end
                                         end
                                         end
                                         
                                         set @Totalhrstime =CONVERT(time,@sumtot1)
                                         set @TotalHrs = CONVERT(char(8),convert(time,@Totalhrstime))
                                         SELECT @rtDayofWeek =  Datename(DW,@ReadPDate)
                                         set @WeeklyOff1=null
                                         set @WeeklyOff2=null
                                         select @WeeklyOff1=WeeklyOff1, @WeeklyOff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode
                                         IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                         Begin 
                                                IF(@ShiftFlag=1 and @isaso=0)
                                                       Begin
                                                              set @Status='P'
                                                       End
                                                 Else
                                                       Begin
                                                              set @status='WOP'
                                                              if((DATEDIFF(minute,0,convert(time,@sumtot1)) >=@MinReqHrsForDed) and (@RequireDeduction  =1))
                                                                     begin 
                                                                     set @sumtot1 =dateadd(N,(-1*@MinsToBeDeducted),DATEADD(N,DATEDIFF(minute,0,convert(time,@sumtot1)),0)) 
                                                                     end    
                                                              select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                  
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                           else
                                                          begin
                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                          end
                                                       
                                                       End           
                                                       
                                                set @Totalhrstime =CONVERT(time,@sumtot1)                 
                                                
                                         end
                                         else if exists(select 1 from holidaylist where hdate=@ReadPDate and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))
                                         begin
                                                set @Status='HP'
                                                if((DATEDIFF(minute,0,convert(time,@sumtot1)) >=@MinReqHrsForDed) and (@RequireDeduction =1))
                                                begin 
                                                set @sumtot1 =dateadd(N,(-1*@MinsToBeDeducted),DATEADD(N,DATEDIFF(minute,0,convert(time,@sumtot1)),0)) 
                                                end
                                                set @Totalhrstime =CONVERT(time,@sumtot1)
                                                select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                
                                                  select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                  if(@otcount=0)
                                                   begin
                                                         insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                         set @chkmailforot=1
                                                   end
                                                    else
                                                   begin
                                                      update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                   end
                                                
                                         end
                                         else if(@ShiftCode ='woff')
                                         begin
                                                set @Status='WOP'                                         
                                                select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                
                                                  select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                  if(@otcount=0)
                                                   begin
                                                         insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                         set @chkmailforot=1
                                                   end
                                                   else
                                                   begin
                                                      update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                   end
                                         
                                         end
                                         else
                                         begin                                           
                                                set @TotalHrs=CONVERT(char(8),convert(time,@sumtot1))    
                                         end 
                                         
                                                                     end -- end of ramadanflag false part
                                                end ----if(@actual=1)
                           else
                           begin                     
                                  select  @InPunch =min(In_punch) from process_data where pdate =@ReadPDate and Empid =@ReadEmpID 
                                  select top 1 @OutPunch =Out_punch from process_data where pdate =@ReadPDate and Empid =@ReadEmpID order by id desc
                                  if (@OutPunch is null)
                                  begin
                                         select @OutPunch =MAX(In_punch) from process_data where pdate =@ReadPDate and Empid =@ReadEmpID
                                         if(@InPunch =@OutPunch)
                                         begin                        
                                                set @OutPunch =null
                                         end
                                  end                       
                                  if(@InPunch is not null and @OutPunch is not null)        
                                  Begin                                
                                         select @breakminute=DATEPART(HOUR, breakhrs) * 60 + DATEPART(MINUTE, breakhrs) From shiftsetting  where CompanyCode=@Comp_id
                                         select @TotalHrsMin = DATEDIFF(Minute,@InPunch,@OutPunch)
                                         select @WeeklyOff1 = WeeklyOff1, @WeeklyOff2=WeeklyOff2 from shift where Shift_Code = @ShiftCode --N2862013
                                         IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2 or @ShiftCode='woff' or (select 1 from holidaylist where hdate=@ReadPDate and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))=1) 
                                                Begin                                    
                                                       set @TotalHrsMin=@TotalHrsMin
                                                End
                                         Else
                                                Begin
                                                       set @TotalHrsMin=@TotalHrsMin-@breakminute 
                                                End                  
                                         if(@TotalHrsMin >0)
                                         begin
                                         select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)--calculating the Total Hrs.
                                         set @Totalhrstime=CONVERT(time,@TotalHrs)
                                         end
                                         else
                                         begin
                                         select @TotalHrs = CONVERT(char(8),DATEADD(n,0,0),108)--calculating the Total Hrs.
                                         set @Totalhrstime=CONVERT(time,@TotalHrs)
                                         end
                                         
                                         select @sumtot1=@Totalhrstime   --NNN-For NS4     
                                  end
                           end ----end else if(@actual=1)
                                           
                           set @month =DATEPART(MM, @ReadPDate)
                           set @year=DATEPART(YYYY, @ReadPDate)   
                           set @day=DATEPART(DD, @ReadPDate) 
                           set @daytemp = 'day'  
                           if(@InPunch> (@readPdate+@ShiftIN_GraceIn))
                           begin                      
                                  select  @LateComersMin = Datediff(Minute,(@readPdate+@ShiftIN),@InPunch)        
                                  select  @LateComerstimeformat = Convert(char(8),DateAdd(n,@LateComersMin,0),108)--calculation for the latecomers        
                           end
                           if(@InPunch < (@readPdate+@ShiftIN))
                           begin                      
                                  select  @EarlyComersMin = Datediff(Minute,@InPunch,(@readPdate+@ShiftIN))        
                                  select  @EarlyComerstimeformat = Convert(char(8),DateAdd(n,@EarlyComersMin,0),108)--calculation for the EarlyComers        
                           end
                           --Caluclation of Early by in bout  
                           if(@breakout is not null)
                                         begin
                                                       if(convert(time,@breakout) < (select convert(time,DATEADD(MINUTE,-Grace_Bout,breakout)) from Shift where Shift_Code= @ShiftCode))
                                                       select @BoutEarlyByMin = DATEDIFF(Minute,CONVERT(time,@breakout),convert(time,breakout)) from Shift where Shift_Code = @ShiftCode
                                                       select @BoutEarlyBy = CONVERT(char(8),Dateadd(n,@BoutEarlyByMin,0),108) --Caluclation of Early by in bout  
                                         end
                           --Caluclation of Early by in bout  End          
                           if(@halfday=@halfday_shift)
                           begin
                                  ----Ramadan shift code start4                                               
                                  if (@ramadanflag='0')
                                         begin
                                                if(@halfday=@halfday_shift)
                                                begin
                                                       select @workhrs = worktime_halfday from shift where Shift_Code =@ShiftCode 
                                                end
                                                else
                                                begin
                                                select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                                                end
                                         end
                                         else
                                         if(@halfday =@halfday_shift)
                                         begin
                                                select @workhrs =Ramadan_MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                                         end
                                         else                                     
                                         begin
                                                select @workhrs =Ramadan_MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                                         end                                                           
                                  ----Ramadan shift code ends 
                                  --select @workhrs =worktime_halfday from shift where Shift_Code =@ShiftCode 
                           end
                           else
                           begin
                                  ----Ramadan shift code start5
                                  if (@ramadanflag='0')
                                         begin
                                                if(@halfday=@halfday_shift)
                                                begin
                                                       select @workhrs = worktime_halfday from shift where Shift_Code =@ShiftCode 
                                                end
                                                else
                                                begin
                                                select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                                                end
                                         end
                                         else
                                         if(@halfday =@halfday_shift)
                                         begin
                                                select @workhrs =Ramadan_MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                                         end
                                         else                                     
                                         begin
                                                select @workhrs =Ramadan_MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                                         end                                    
                                  ----Ramadan shift code ends
                                  --select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                           end
                           if(@TotalHrs is not null or @TotalHrs !='')        
                           Begin
                                  --select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCodeyes 
                                  if(@includep=1) 
                                  begin
                                         select @workhrs =CONVERT(time(7),Totalhrs) from employeecategorymaster where empcategorycode=@empcat
                                  end
                                  else
                                   begin                                  
                                         ----Ramadan shift code start3                                                  
                                         if (@ramadanflag='0')
                                         begin
                                                if(@halfday=@halfday_shift)
                                                begin
                                                       select @workhrs = worktime_halfday from shift where Shift_Code =@ShiftCode 
                                                end
                                                else
                                                begin
                                                select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCode
                                                end
                                         end
                                         else
                                         if(@halfday =@halfday_shift)
                                         begin
                                                select @workhrs =Ramadan_MaxOverTime_General from shift where Shift_Code =@ShiftCode 
                                         end
                                         else                                     
                                         begin
                                                select @workhrs =Ramadan_MaxOverTime_General from shift where Shift_Code =@ShiftCode
                                         end                                                           
                                                ----Ramadan shift code ends 
                                                --select @workhrs =MaxOverTime_General from shift where Shift_Code =@ShiftCode
                                  end
-----------                                                             
                                  if(@halfday=@halfday_shift)
                                  begin
                                         select @halfday_endtime=endtime_halfday from shift where Shift_Code =@ShiftCode 
                                         if(CONVERT(time,@ShiftOUT)< CONVERT(time,@halfday_endtime))--Did not understand this condition
                                         begin
                                              select @EarlyLeaversMin=Datediff(Minute,convert(time,@ShiftOUT),CONVERT(time,@halfday_endtime))
                                              select @EarlyLeaverstimeformat=CONVERT(char(8),Dateadd(n,@EarlyLeaversMin,0),108)
                                         end
                                         if(CONVERT(time,@OutPunch) < CONVERT(time,@halfday_endtime))         
                                         Begin  
                                                select @EarlyLeaversMin = Datediff(Minute,CONVERT(time,@OutPunch),CONVERT(time,@halfday_endtime))        
                                                select @EarlyLeaverstimeformat = CONVERT(char(8),Dateadd(n,@EarlyLeaversMin,0),108)--Calculation for the early leavers        
                                         End
                                         if(CONVERT(time,@OutPunch) >CONVERT(time, @halfday_endtime))         
                                         Begin  
                                                select @LateGoMin = Datediff(Minute,CONVERT(time,@halfday_endtime),CONVERT(time,@OutPunch)  )     
                                                select @LateGotimeformat = CONVERT(char(8),Dateadd(n,@LateGoMin,0),108)--Calculation for the Late leavers        
                                         End
                                  end
                                  else
                                  begin
                                          if(@OutPunch < @ReadPDATE+@ShiftOUT_GraceOut1) 
                                         Begin  
                                                select @EarlyLeaversMin = Datediff(Minute,@OutPunch,(@readPdate+@ShiftOUT))        
                                                select @EarlyLeaverstimeformat = CONVERT(char(8),Dateadd(n,@EarlyLeaversMin,0),108)--Calculation for the early leavers        
                                         End 
                                         if(@OutPunch > (@readPdate+@ShiftOUT)) 
                                         Begin  
                                                select @LateGoMin = Datediff(Minute,(@readPdate+@ShiftOUT),@OutPunch)        
                                                select @LateGotimeformat = CONVERT(char(8),Dateadd(n,@LateGoMin,0),108)--Calculation for the Late leavers    
                                         End                                                                                
                                  end
                           end ------end if (@TotalHrs is not null and @TotalHrs is not null)  
                           --Caluclation of Late by in bin  End
                           if(@breakin is not null)
                                         begin
                                                       if(convert(time,@breakin) > (select convert(time,DATEADD(MINUTE,Grace_Bin,breakin)) from Shift where Shift_Code= @ShiftCode))
                                                       select @BinLatebyMin = DATEDIFF(Minute,convert(time,breakin),convert(time,@breakin)) from Shift where Shift_Code= @ShiftCode
                                                       select @BinLateBy = CONVERT(char(8),Dateadd(n,@BinLatebyMin,0),108) --Caluclation of Early by in bout  
                                         end
                           --Caluclation of Late by in bin  End                                 
                            
                            SELECT @rtDayofWeek =  Datename(DW,@ReadPDate)
                            set @WeeklyOff1=null
                            set @WeeklyOff2=null
                                         select @WeeklyOff1=WeeklyOff1, @WeeklyOff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode
                                         IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                         Begin 
                                                IF(@ShiftFlag=1  and @isaso=0)
                                                       Begin
                                                              set @Status='P'
                                                       End
                                                 Else
                                                       Begin
                                                              set @status='WOP'
                                                              if((DATEDIFF(minute,0,convert(time,@sumtot1)) >=@MinReqHrsForDed) and (@RequireDeduction =1))
                                                                     begin 
                                                                     set @sumtot1 =dateadd(N,(-1*@MinsToBeDeducted),DATEADD(N,DATEDIFF(minute,0,convert(time,@sumtot1)),0)) 
                                                                     end 
                                                              select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                              
                                                                select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                                if(@otcount=0)
                                                                 begin
                                                                       insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                       set @chkmailforot=1
                                                                 end
                                                                 else
                                                                 begin
                                                                    update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                                 end
                                                              
                                                       End                                             
                                                
                                                set @TotalHrs = CONVERT(char(8),convert(time,@sumtot1))                              
                                                
                                         end
                                         else if exists(select 1 from holidaylist where hdate=@ReadPDate and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code) )
                                         begin
                                                set @Status='HP'
                                                
                                                if((DATEDIFF(minute,0,convert(time,@sumtot1)) >=@MinReqHrsForDed) and (@RequireDeduction =1))
                                                begin 
                                                set @sumtot1 =dateadd(N,(-1*@MinsToBeDeducted),DATEADD(N,DATEDIFF(minute,0,convert(time,@sumtot1)),0)) 
                                                end
                                                
                                                set @TotalHrs = CONVERT(char(8),convert(time,@sumtot1))
                                                select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                
                                                  select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                  if(@otcount=0)
                                                   begin
                                                         insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                         set @chkmailforot=1
                                                   end
                                             else
                                                   begin
                                                      update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                   end
                                                
                                         end
                                         else if (@ShiftCode='woff')
                                                Begin
                                                       set @Status='WOP'                                                    
                                                       if((DATEDIFF(minute,0,convert(time,@sumtot1)) >=@MinReqHrsForDed) and (@RequireDeduction =1))
                                                              begin 
                                                                     set @sumtot1 =dateadd(N,(-1*@MinsToBeDeducted),DATEADD(N,DATEDIFF(minute,0,convert(time,@sumtot1)),0)) 
                                                              end                                                    
                                                       set @TotalHrs = CONVERT(char(8),convert(time,@sumtot1))
                                                       select @sOvertimetimeformat=CONVERT(char(8),convert(time,@sumtot1))
                                                       
                                                         select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                         if(@otcount=0)
                                                          begin
                                                                insert into Overtime(EMPID, OTDate, OTHrs, Flag, EmpName) values(@ReadEmpID,@ReadPDate,Convert(varchar(5),convert(time, @sOvertimetimeformat)),2,@ReadEmpName)
                                                                set @chkmailforot=1
                                                          end
                                                          else
                                                          begin
                                                             update overtime SET Flag=2, OTHrs=Convert(varchar(5),convert(time, @sOvertimetimeformat))where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                          end
                                                       
                                                End
                                         else
                                                begin         
                                                       set @sumtot1=case when @sumtot1=0 then null else @sumtot1 end                             
                                                       set @TotalHrs=CONVERT(char(8),convert(time,@sumtot1))        
                                                end   
                                         --else
                                         --begin              
                                         --    set @sumtot1=case when @sumtot1=0 then null else @sumtot1 end                               
                                         --     set @TotalHrs=CONVERT(char(8),convert(time,@sumtot1))    
                                         --end   
                            if ((@Totalhrstime >@workhrs) and (@Employee_OT_Elegibality >0) and (@sOvertimetimeformat is null)) 
                                  begin                                  
                                         set @overtime=convert(datetime,@Totalhrstime)-convert(datetime,@workhrs) 
                                         if @includep !=1
                                                BEGIN
                                                              select @maxovertime=MaxWorkTime from shift where Shift_Code =@ShiftCode
                                                              select @minovertime = minovertime from Shift where Shift_Code =@ShiftCode                                               
                                                              if @maxovertime !=''
                                                              begin 
                                                                     if @overtime >@maxovertime 
                                                                     begin
                                                                           set @overtime =@maxovertime
                                                                     end  
                                                              end                                             
                                                              if @minovertime!=''
                                                                     begin
                                                                           if @overtime <@minovertime 
                                                                           begin
                                                                                  set @overtime =null
                                                                           end 
                                                                           
                                                                     end                                             
                                                              if ((@maxovertime ='00:00:00.000') and (@minovertime='00:00:00.000')) 
                                                                     begin
                                                                           set @overtime =null
                                                                     end
                                                END
                                                --------------CODE CHANGE ON 14102014 BY VIKASH TO INSERT DATA IN OVERTIME TABLE----------
                                                                           
                                             select @otcount= count(*) from overtime where empid=@readempid and otdate=@readpdate
                                                 --if(@otcount=0)
                                                 --  begin
                                                 --        insert into Overtime(EMPID, OTDate, OTHrs, Flag,Mflag, EmpName) values(@ReadEmpID,@ReadPDate,convert(varchar(5),@overtime),1,1,@ReadEmpName)
                                                 --        set @chkmailforot=1
                                                 --  end
                                                 --   else
                                                 --  begin
                                                 --     update overtime set OTHrs=convert(varchar(5),@overtime)where EMPID=@ReadEmpID and OTDate=@ReadPDate
                                                 --  end
                                         ----------------------CODE CHANGE END--------------------------------------------------------                                         
                                         set @Overtimetimeformat=CONVERT(varchar,@overtime)
                                  end    
                     if exists(select 1 from ShiftEmployee where Empid=@ReadEmpID and Month=@month and year=@year)--------Integrating with ShiftRoster
                           begin
                                  set @SqlQuery='insert into TempDay select '+ @daytemp + convert(char,@day)+ 'from ShiftEmployee where Empid=''' + @ReadEmpID + ''' and Month = ' + convert(char,@month) + 'and year = ' + convert(char,@year)
                                  execute(@SqlQuery)
                                  select @ShiftCode = ShiftCode from TempDay   
                                  set @ShiftFlag=1  
                                         if(@ShiftCode is null)
                                                begin 
                                                       select @ShiftCode = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID
                                                       set @ShiftFlag=0      
                                                end                     
                           end 
                     if(@ShiftCode is null)
                           begin 
                                  select @ShiftCode = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID
                                  set @ShiftFlag=0      
                           end
                           set @WeeklyOff1=null
                           set @WeeklyOff2=null
                           select @WeeklyOff1=WeeklyOff1, @WeeklyOff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode        
                           select @CalWeeklyOff1=CONVERT(varchar,DATEPART(dd,@ReadPDate)) 
                           SELECT @rtDayofWeek =  Datename(DW,@ReadPDate)
                            
                             IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)        
                                  Begin        
                                          set @Status = 'WO'  
                                          if @InPunch is not null
                                                 begin
                                                       IF(@ShiftFlag=1 and @isaso=0)
                                                              Begin
                                                                     set @Status='P'
                                                              End
                                                       Else
                                                              Begin
                                                                     set @status='WOP'
                                                              End   
                                                 end   
                             End 
                          Else if(@Status is null and  @ShiftCode='woff' and   @InPunch is not null)
                                  Begin
                                         set @Status='WOP'
                                  End
                          Else if (@Status is null)
                               begin
                                         set @Status='P'                                                                                                                                                                                        
                               end
                                                
                           if exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate =@ReadPDate)
                           begin                        
                                  if (@OutPunch is null or @InPunch is null)
                                  begin
                                         update MASTERPROCESSDAILYDATA set In_Punch =@InPunch,Out_Punch=@OutPunch,WorkHRs=@TotalHrs,LateBy=@LateComerstimeformat,EarlyBy =@EarlyLeaverstimeformat,OT =@Overtimetimeformat,status='MS',Shift_Code =@ShiftCode,Shift_In =@ShiftIN,Shift_Out =@ShiftOUT,Dept_Name =@Dept_Name ,Desig_Name =@Desig_Name,contact=@contact,Shift_Name=@shift_Name,BreakOut=@breakout,BreakIn=@breakin,OT_Start=@ShiftOUT,EarlyCome=@EarlyComerstimeformat,LateGo=@LateGotimeformat,BreakOut_EarlyBy =@BoutEarlyBy,BreakIn_LateBy = @BinLateBy where Emp_ID=@ReadEmpID and PDate=@ReadPDate 
                                  end
                                  else
                                  begin
                                         --update MASTERPROCESSDAILYDATA set In_Punch =@InPunch,Out_Punch=@OutPunch,WorkHRs=@TotalHrs,LateBy=@LateComerstimeformat,EarlyBy =@EarlyLeaverstimeformat,OT =@Overtimetimeformat,status=@Status,Shift_Code =@ShiftCode,Shift_In =@ShiftIN,Shift_Out =@ShiftOUT,Dept_Name =@Dept_Name ,Desig_Name =@Desig_Name,contact=@contact,Shift_Name=@shift_Name,BreakOut=@breakout,BreakIn=@breakin,OT_Start=@ShiftOUT,EarlyCome=@EarlyComerstimeformat,LateGo=@LateGotimeformat,BreakOut_EarlyBy =@BoutEarlyBy,BreakIn_LateBy = @BinLateBy   where Emp_ID=@ReadEmpID and PDate=@ReadPDate 
                                         update MASTERPROCESSDAILYDATA set In_Punch =@InPunch,Out_Punch=@OutPunch,WorkHRs=@TotalHrs,LateBy=@LateComerstimeformat,EarlyBy =@EarlyLeaverstimeformat,OT =@Overtimetimeformat,status=@Status,Shift_Code =@ShiftCode,Shift_In =@ShiftIN,Shift_Out =@ShiftOUT,Dept_Name =@Dept_Name ,Desig_Name =@Desig_Name,contact=@contact,Shift_Name=@shift_Name,BreakOut=@breakout,BreakIn=@breakin,OT_Start=@ShiftOUT,EarlyCome=@EarlyComerstimeformat,LateGo=@LateGotimeformat,BreakOut_EarlyBy =@BoutEarlyBy,BreakIn_LateBy = @BinLateBy ,sot=@sOvertimetimeformat  where Emp_ID=@ReadEmpID and PDate=@ReadPDate 
                                  end
                           end                                                                                   
                           else
                           begin                       
                           if (@OutPunch is null or @InPunch is null)
                           begin
                                  --if(@Status is null)
                                  --begin
                                         set @Status = 'MS'
                                  --end
                                         insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@Status,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                  end
                                  else
                                  begin     
                                         if(@Status is null)
                                         begin
                                                set @Status= 'P'
                                         end                                       
                                         insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,In_Punch,Out_Punch,Status,WorkHRs,Total_Hrs,LateBy,EarlyBy,OT_Start,OT,Shift_In,Shift_Out,Comp_Name,Dept_Name,Desig_Name, Shift_Code,Cat_Name,Shift_Name,contact,DOJ,BreakOut,BreakIn,sot,EarlyCome,LateGo,BreakOut_EarlyBy,BreakIn_LateBy) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@InPunch,@OutPunch,@Status,@TotalHrs,@TotalShiftTime, @LateComerstimeformat,@EarlyLeaverstimeformat,@ShiftOUT,@Overtimetimeformat,@ShiftIN,@ShiftOUT,@Comp_Name,@Dept_Name,@Desig_Name,@ShiftCode,@Cat_Name,@shift_Name,@contact,@DOJ,@breakout,@breakin,@sOvertimetimeformat,@EarlyComerstimeformat,@LateGotimeformat,@BoutEarlyBy,@BinLateBy)         
                                  end                       
                           end
                     end                                     
                      --end -----end if Exists (select 1 from process_data where PunchDate = @ReadPDate and EmpId = @ReadEmpID) or @ShiftOUT<@ShiftIN   
                     else
                     begin
                           set @ShiftCode= NUll   
                           set @Status = NUll         
                           set @CalWeeklyOff1 = NUll        
                           set @CalWeeklyOff2 = NUll 
                           set @month =DATEPART(MM, @ReadPDate)
                           set @year=DATEPART(YYYY, @ReadPDate)   
                           set @day=DATEPART(DD, @ReadPDate)
                           set @daytemp = 'day'
                           --set @tempcol=@daytemp +convert(char,@day)
                           if exists(select 1 from ShiftEmployee where Empid=@ReadEmpID and Month=@month and year=@year)
                           begin
                                  set @SqlQuery='insert into TempDay select '+ @daytemp + convert(char,@day)+ 'from ShiftEmployee where Empid=''' + @ReadEmpID + ''' and Month = ' + convert(char,@month) + 'and year = ' + convert(char,@year)
                                  execute(@SqlQuery)
                                  select @ShiftCode = ShiftCode from TempDay
                                  --select @ShiftCode= @tempcol from ShiftEmployee where Empid=@ReadEmpID and Month=@month and year=@year
                                  if (@ShiftCode is NULL)
                                         BEgin
                                                SELECT @rtDayofWeek = Datename(DW,@ReadPDate)
                                                set @ReadPDateP=DATEADD(dd,-1,@ReadPDate)
                                                       if exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate=@ReadPDateP)
                                                       Begin
                                                              Select @ShiftCode= Shift_Code from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate=@ReadPDateP
                                                                     If (@ShiftCode is NULL)
                                                                           Begin
                                                                                  set @ReadPDatePP=DATEADD(dd,-1,@ReadPDateP)
                                                                                  Select @ShiftCode= Shift_Code from MASTERPROCESSDAILYDATA where Emp_ID=@ReadEmpID and PDate=@ReadPDateP
                                                                           End
                                                                           set @WeeklyOff1=null
                                                                           set @WeeklyOff2=null
                                                                           select @WeeklyOff1=WeeklyOff1, @WeeklyOff2=WeeklyOff2 from Shift where Shift_Code=@ShiftCode
                                                                     IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek = @WeeklyOff2)
                                                                           Begin
                                                                                  set @Status='WO'
                                                                                  if @InPunch is not null
                                                                                         IF(@ShiftFlag=1 and @isaso=0)
                                                                                                Begin
                                                                                                       set @Status='P'
                                                                                                End
                                                                                          Else
                                                                                                Begin
                                                                                                       set @status='WOP'
                                                                                                End  
                                                                                  if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID =@ReadEmpID and PDate=@ReadPDate)
                                                                                         begin
                                                                                                insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,Status,Comp_Name,Dept_Name,Desig_Name, Cat_Name, Shift_In, Shift_Out, Shift_Code,Shift_Name,contact,DOJ) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@Status,@Comp_Name,@Dept_Name,@Desig_Name, @Cat_Name, @ShiftIN, @ShiftOUT, @ShiftCode,@shift_Name,@contact,@DOJ)
                                                                                         end
                                                                           End
                                                                           Else
                                                                                  Begin
                                                                                         set @Status='A'
                                                                                         if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID =@ReadEmpID and PDate=@ReadPDate)
                                                                                         begin
                                                                                                insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,Status,Comp_Name,Dept_Name,Desig_Name, Cat_Name, Shift_In, Shift_Out, Shift_Code,Shift_Name,contact,DOJ) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@Status,@Comp_Name,@Dept_Name,@Desig_Name, @Cat_Name, @ShiftIN, @ShiftOUT, @ShiftCode,@shift_Name,@contact,@DOJ)
                                                                                         end
                                                                                  End
                                                       End
                                                Else
                                                       Begin
                                                              set @Status='A'
                                                              if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID =@ReadEmpID and PDate=@ReadPDate)
                                                                     begin
                                                                           insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,Status,Comp_Name,Dept_Name,Desig_Name, Cat_Name, Shift_In, Shift_Out, Shift_Code,Shift_Name,contact,DOJ) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@Status,@Comp_Name,@Dept_Name,@Desig_Name, @Cat_Name, @ShiftIN, @ShiftOUT, @ShiftCode,@shift_Name,@contact,@DOJ)
                                                                     end
                                                              End
                                                       End
                                  else if @ShiftCode ='woff'
                                  begin                                                                                           
                                         set @Status='WO'
                                         if @InPunch is not null
                                         begin
                                                set @Status='WOP'
                                         end                                                    
                                         if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID =@ReadEmpID and PDate=@ReadPDate)
                                         begin
                                                insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,Status,Comp_Name,Dept_Name,Desig_Name, Cat_Name, Shift_In, Shift_Out, Shift_Code,Shift_Name,contact,DOJ) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@Status,@Comp_Name,@Dept_Name,@Desig_Name, @Cat_Name, @ShiftIN, @ShiftOUT, @ShiftCode,@shift_Name,@contact,@DOJ)
                                         end
                                  end
                                  else
                                  begin
                                         set @Status='A'
                                         if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID =@ReadEmpID and PDate=@ReadPDate)
                                         begin
                                                insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,Status,Comp_Name,Dept_Name,Desig_Name, Cat_Name, Shift_In, Shift_Out, Shift_Code,Shift_Name,contact,DOJ) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@Status,@Comp_Name,@Dept_Name,@Desig_Name, @Cat_Name, @ShiftIN, @ShiftOUT, @ShiftCode,@shift_Name,@contact,@DOJ)
                                         end
                                  end
                           end 
                           else 
                           begin
                                  select @ShiftCode = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@ReadEmpID --reading the shiftcode from the employee table based on the employee id         
                                  set @WeeklyOff1=null
                                  set @WeeklyOff2=null        
                                  select @WeeklyOff1=WeeklyOff1, @WeeklyOff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode        
                                  select @CalWeeklyOff1=CONVERT(varchar,DATEPART(dd,@ReadPDate))        
                                  SELECT @rtDayofWeek =  Datename(DW,@ReadPDate)        
                                  --select @ShiftCode = Shift_Code from Employee where Emp_ID=@ReadEmpID --reading the shiftcode from the employee table based on the employee id         
                                  if(@ramadanflag='0')
                                  begin
                                         select @ShiftIN = CONVERT(Time,In_Time),@ShiftOUT =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode          
                                  end
                                  else
                                  begin
                                         select @ShiftIN = CONVERT(Time,Ramadan_InTime),@ShiftOUT =  CONVERT(Time,Ramadan_OutTime)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode          
                                  end
                                  ----select @ShiftIN = CONVERT(Time,In_Time),@ShiftOUT =  CONVERT(Time,Out_Time)  from Shift where Shift_Code = @ShiftCode --getting the shiftin and shift out from the shift table based on the shiftcode  
                                  
                                  --------------new change for halfday start and end time -------------
                                  set @halfday=DATENAME(DW,@ReadPDate) 
                     select @halfday_shift=Halfday from shift  where Shift_Code =@ShiftCode
                     if(@halfday=@halfday_shift)
                     begin                                  
                           ----Ramadan shift code start2                                              
                           if (@ramadanflag='0')
                           begin
                                  select @TotalShiftTime= worktime_halfday from Shift Where Shift_Code=@ShiftCode
                           end
                           else
                           begin
                                  select @TotalShiftTime= Ramadan_worktime_halfday from Shift Where Shift_Code=@ShiftCode
                           end                                                                   
                           ----Ramadan shift code ends              
                                                       
                           --select @TotalShiftTime= worktime_halfday from Shift Where Shift_Code=@ShiftCode
                           select @ShiftIN=CONVERT(Time,starttime_halfday)from Shift where Shift_Code=@ShiftCode
                     select @ShiftOUT =  CONVERT(Time,endtime_halfday)  from Shift where Shift_Code = @ShiftCode
                     end
                                  
                                  
                                  --------------end--------------------------------------------------
                                          
                                  IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek =  @WeeklyOff2)    
                                         Begin  
                                                       set @Status = 'WO'  
                                                       if @InPunch is not null
                                                begin
                                                       IF(@ShiftFlag=1 and @isaso=0)
                                                              Begin
                                                                     set @Status='P'
                                                              End
                                                        Else
                                                              Begin
                                                                     set @status='WOP'
                                                              End 
                                                end    
                                         End                                       
                                  Else        
                                         Begin        
                                                set @Status = 'A'        
                                         End 
                                  if not exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID =@ReadEmpID and PDate=@ReadPDate)
                                  begin       
                                         insert into MASTERPROCESSDAILYDATA(Emp_ID,Emp_Name,PDate,Status,Comp_Name,Dept_Name,Desig_Name, Cat_Name, Shift_In, Shift_Out, Shift_Code,Shift_Name,contact,DOJ,Total_Hrs) values(@ReadEmpID,@ReadEmpName,@ReadPDate,@Status,@Comp_Name,@Dept_Name,@Desig_Name, @Cat_Name, @ShiftIN, @ShiftOUT, @ShiftCode,@shift_Name,@contact,@DOJ,@TotalShiftTime)        
                                  end
                                  set @Status = NUll     
                                  --end  
                           end      
                     end ------end if not exist
                     set @PunchDateIndex = @PunchDateIndex + 1        
              End -------end while                           
          --End ------end while  
              set @PunchDateIndex = 1          
              set @EmpIDIndex = @EmpIDIndex + 1      
       end
       set @EmpIDIndex= 1
              
       -------For Holidays----  
       truncate table storebranch
       --truncate table T_Table
       declare @bcount int,@icount int,@bcode varchar(max),@hgcode varchar(max)
       set @icount=1
       insert into storebranch select distinct BranchCode,HolidayCode from BranchMaster
       select @bcount=COUNT(*) from storebranch
       while(@icount<=@bcount)
       begin
              truncate table holidays           
              select @bcode=bcode from storebranch where id=@icount
              insert into holidays(fdt,tdt) select convert(datetime,holfrom,103),convert(datetime,holto,103) from HolidayMaster where holgrpcode in( select HolidayCode from BranchMaster where BranchCode=@bcode) and (holfrom Between  convert(date,@fdate) and convert(date,@tdate) or  holto Between  convert(date,@fdate) and convert(date,@tdate) )
              select @count=count(fdt) from holidays 
              set @index=1
              while(@index<=@count)
              begin
                     select @fholiday=fdt,@tholiday=tdt from holidays where hindex=@index
                     while(datediff(dd,@fholiday,@tholiday) >= 0)
                     begin
                           update MASTERPROCESSDAILYDATA set Status='H' where PDate=@fholiday and Emp_ID in(select Emp_Code from EmployeeMaster where Emp_Branch=@bcode) and Status ='A'--Status='' -- Status is not null 
                                  
                           set @fholiday=DATEADD(dd,1,@fholiday)
                     end
                     set @index =@index+1
              end
              set @icount=@icount+1
       end
                
                 declare @hlstatus int=0
       ------for leaves management adding vacataion and compoff also-------       
       --insert into @leaves(empid,fdate,tdate,lid) select EmployeeCode,FromDate,ToDate,Leave_id  from LeavesApproval where Flag=2 and LeaveCode not in('CO','OD','V') 
       insert into @leaves(empid,fdate,tdate,lid, hlstatus) select empid,startdate,enddate,leavetype, hl_status from leave1 where Flag=2 and (startdate Between  convert(date,@fdate) and convert(date,@tdate) or  enddate Between  convert(date,@fdate) and convert(date,@tdate) )
       declare @leavetype varchar(10)
       select @lcount=COUNT(empid)from @leaves 
       set @lindex=1
       while @lindex<=@lcount 
       begin
              select @empid=empid,@lfrom=fdate,@lto=tdate, @hlstatus=hlstatus, @leavetype=lid from @leaves where lindex=@lindex
              if(@hlstatus=1)
              begin
                     update MASTERPROCESSDAILYDATA set Status ='PHL' where Emp_ID=@empid and PDate=@lfrom and  Status ='P'
                     update MASTERPROCESSDAILYDATA set Status ='AHL' where Emp_ID=@empid and PDate=@lfrom and  Status ='A'
              end
              else
              begin
                     while DATEDIFF(dd,@lfrom,@lto)>=0
                     begin
                       if EXISTS(Select 1 from MASTERPROCESSDAILYDATA where PDate=@lfrom and Emp_ID =@empid)                   
                           insert into @templeave(empid,ldates) values (@empid,@lfrom)
                           set @lfrom=DATEADD(dd,1,@lfrom)
                     end
              end
              set @lindex=@lindex+1
       end
    declare @GetStatus varchar(max)=null
       select @lcount =COUNT(empid) from @templeave 
       set @lindex =1
       while @lindex<=@lcount 
       begin
          set @GetStatus=null
          ------------------------------------code change on 17102014 by vikash to check if status should be update based on updated status----------
              select @empid=empid,@ldate=ldates  from @templeave where tid=@lindex 
              select @GetStatus = status from MASTERPROCESSDAILYDATA where Emp_ID=@empid and pdate=@ldate
              IF @GetStatus=@leavetype
              BEGIN
                     if @leavetype='V'
                     begin
                      
                       update MASTERPROCESSDAILYDATA set Status='V' where Emp_ID=@empid and PDate=@ldate and Status !='WO' and Status !='H'
                     end
                     else if @leavetype='CO'
                     begin
                        update MASTERPROCESSDAILYDATA set Status='CO' where Emp_ID=@empid and PDate=@ldate and Status !='WO' and Status !='H'
                     end
              END
              else
              begin
                  --if @GetStatus='A'
                  --begin
                      update MASTERPROCESSDAILYDATA set Status='L' where Emp_ID=@empid and PDate=@ldate and Status !='WO' and Status !='H'
                  --end
                  --else
                  --begin
                  --   update MASTERPROCESSDAILYDATA set Status=@GetStatus where Emp_ID=@empid and PDate=@ldate and Status !='WO' and Status !='H'
                  --end
              end------------------------------------------------------------------------------------------------------------------------------------------
              set @lindex =@lindex +1
       end 

  -------------------------------------------for checking lop change on 02082013------------------------------------------------------------------------
       declare @hlststus int=0, @temphlststus int=0,@leavetype1 varchar(max)=null
       SET @lcount=NULL
       insert into @leaves1 (empid,fdate,tdate,lid,hl_ststus, leavetype)select Empid,StartDate,EndDate,Leave_id,hl_status, LeaveType from Lossonpay where Flag=2 AND Leavetype not in('OD','V') and (startdate Between  convert(date,@fdate) and convert(date,@tdate) or  enddate Between  convert(date,@fdate) and convert(date,@tdate) )
      set @leavetype=null--code change on 24122013 by vikash to update the loss on pay flag and based on that will update
       select @lcount=COUNT(empid)from @leaves1 
       set @lindex=1
       while @lindex<=@lcount 
       begin
         select @empid=empid,@lfrom=fdate,@lto=tdate,@hlststus=hl_ststus, @leavetype=leavetype from @leaves1 where lindex=@lindex
               
           begin
                           while DATEDIFF(dd,@lfrom,@lto)>=0
                            begin
                             insert into @templeave1(empid,ldates,hlflvststatus, leavetype) values (@empid,@lfrom,@hlststus, @leavetype)
                             set @lfrom=DATEADD(dd,1,@lfrom)
                           end
            end 
              set @empid=null
            set @lfrom=null
            set @lto=null
            set @hlststus=null
            set @leavetype=null
           set @lindex=@lindex+1
       end
         select @lcount =COUNT(empid) from @templeave1 
          set @GetStatus =null
         
       set @lindex =1
       while @lindex<=@lcount 
       begin
        select @empid=empid,@ldate=ldates,@temphlststus=hlflvststatus, @leavetype1=leavetype from @templeave1 where tid=@lindex 
         --select @GetStatus = status from MASTERPROCESSDAILYDATA where Emp_ID=@empid and pdate=@ldate
         --IF @GetStatus='A'
         --BEGIN
             update MASTERPROCESSDAILYDATA set Status='LWP' where Emp_ID=@empid and PDate=@ldate
         --END
         set @lindex =@lindex +1
         
          set @empid=null
        set @lfrom=null
        set @lto=null
        set @leavetype=null
        set @leavetype=null
        set @ldate=null
       end 
       ----On Duty(OD)     
       declare @ccount int,@codcode varchar(max)
       --insert into @covod(empid,fdate,tdate,lcode)select EmployeeCode,FromDate,ToDate,LeaveCode from LeavesApproval where Flag=2 and LeaveCode in('CO','OD','V')
       insert into @covod(empid,fdate,tdate,lcode)select empid,startdate,enddate,leavetype from ODLeave where Flag=2 and (startdate Between  convert(date,@fdate) and convert(date,@tdate) or  enddate Between  convert(date,@fdate) and convert(date,@tdate) )
       select @ccount=COUNT(empid) from @covod 
       set @lindex =1 
       while(@lindex <=@ccount)
       begin
              select @empid=empid,@lfrom=fdate,@lto=tdate,@codcode=lcode from @covod where lindex=@lindex
              while DATEDIFF(dd,@lfrom,@lto)>=0
              begin
                     insert into @covodleaves(empid,ldates,lecode) values (@empid,@lfrom,@codcode) 
                     set @lfrom=DATEADD(dd,1,@lfrom) 
              end           
              set @lindex=@lindex+1
       end
       select @lcount =COUNT(empid) from @covodleaves 
       set @lindex =1
       while @lindex<=@lcount 
       begin
              select @empid=empid,@ldate=ldates,@codcode=lecode from @covodleaves where tid=@lindex 
              update MASTERPROCESSDAILYDATA set Status=@codcode where Emp_ID=@empid and PDate=@ldate 
              set @lindex =@lindex +1
       end 
                     
                     
       ------For leave Cancelled ---------
       --declare @eid varchar(max),@cfrom datetime,@cto datetime,@clcode int,@in datetime
       --insert into @cancelledleaves(empid,fdate,tdate,lid) select EmployeeCode,FromDate,ToDate,Leave_id  from LeavesApproval where lstatus =1 
       --select @lcount=COUNT(empid) from @cancelledleaves   
       --set @lindex=1
       --while @lindex<=@lcount 
       --begin      
       --     select @eid=empid,@cfrom=fdate ,@cto =tdate,@clcode =lid from @cancelledleaves where lindex =@lindex 
       --     if(@cto ='1900-01-01')
       --     begin
       --            --exec spApprovalDeleteHDLeave @clcode
       --            select @in =In_Punch from MASTERPROCESSDAILYDATA where Emp_ID =@eid and PDate =@cfrom 
       --            if (@in is null)
       --            begin
       --                   update MASTERPROCESSDAILYDATA set Status='A' where Emp_ID =@eid and PDate =@cfrom 
       --            end
       --            else
       --            begin
       --                   update MASTERPROCESSDAILYDATA set Status='P' where Emp_ID =@eid and PDate =@cfrom 
       --            end  
       --     end
       --     else
       --     begin
       --            --exec spApprovalDeleteLeave @clcode
       --            while(DATEDIFF(DD,@cfrom,@cto)>=0)
       --            begin
       --                   select @in =In_Punch from MASTERPROCESSDAILYDATA where Emp_ID =@eid and PDate =@cfrom 
       --                   if (@in is null)
       --                   begin
       --                         update MASTERPROCESSDAILYDATA set Status='A' where Emp_ID =@eid and PDate =@cfrom 
       --                   end
       --                   else
       --                   begin
       --                         update MASTERPROCESSDAILYDATA set Status='P' where Emp_ID =@eid and PDate =@cfrom 
       --                   end               
       --                   set @cfrom =DATEADD(dd,1,@cfrom)
       --            end
       --     end      
       --     set @lindex =@lindex +1 
       --end
                
       
       --Manual Punch New One Fixed Reprocessing issue
       
       -------For manual punches---------      
        insert into @punches(empid,pdate,pin,pout,Bout,Bin,app) select EmpCode,convert(datetime,WorkDate,103),InPunch,OutPunch,BreakOut,BreakIn,Approve from PunchForApproval where Approve = 2
select @count=COUNT(empid) from @punches
set @index=1
while @index<=@count
begin
set @EmplCategory_Manual =null
set @includep = null

select @empid=empid,@piInPunch=pin,@piOutPunch =pout,@breakout_Manual = Bout,@breakin_Manual = Bin,@piWorkDate=pdate,@app=app from @punches where pindex=@index
select @Cat_Code = Emp_Branch from EmployeeMaster where Emp_Code = @empid
select @Comp_id=Emp_Company  from EmployeeMaster where Emp_Code = @empid
--select @holidaygroup=HolidayCode from BranchMaster where BranchCode = @Cat_Code
select @EmplCategory_Manual = Emp_Employee_Category from EmployeeMaster where Emp_Code = @empid
select @includep=includeprocess from EmployeeCategoryMaster where EmpCategoryCode=@EmplCategory_Manual
select @RequireDeduction = BreakDeductionRequired from ShiftSetting where CompanyCode = @Comp_id       
Select @MinReqHrsForDed = datediff(Minute,0,convert(time,MinWorkHrsForDeduction)) from ShiftSetting where CompanyCode = @Comp_id     
select @MinsToBeDeducted =TotalDeductionTime from ShiftSetting where CompanyCode = @Comp_id     
select @brkHrsMinute=DATEPART(HOUR, breakhrs) * 60 + DATEPART(MINUTE, breakhrs) From shiftsetting  where CompanyCode=@Comp_id

if(@RequireDeduction is null or @RequireDeduction='' )
       Begin
              set @RequireDeduction=0
       End
if(@MinReqHrsForDed is null  or @MinReqHrsForDed='')
       Begin
              set @MinReqHrsForDed=0
       End
if(@MinsToBeDeducted is null or @MinsToBeDeducted='' )
       Begin
              set @MinsToBeDeducted=0
       End
if(@brkHrsMinute is null or @brkHrsMinute='' )
       Begin
              set @brkHrsMinute=0
       End    

if exists ( select 1 from MASTERPROCESSDAILYDATA where (PDate=@piWorkDate and Emp_ID=@empid) and status not in ('M','MI','MO'))
begin
--update MASTERPROCESSDAILYDATA set In_Punch=@piInPunch,Out_Punch=@pioutpunch,Status ='M' where PDate=@piWorkDate and Emp_ID=@piEmpCode
if ( @piInPunch !='' and @piOutPunch !='' and @breakout_Manual is null and @breakin_Manual is null)
begin
--update MASTERPROCESSDAILYDATA set In_Punch=convert(time,@piInPunch),Out_Punch=convert(time,@pioutpunch),Status ='M' where PDate=@piWorkDate and Emp_ID=@empid
update MASTERPROCESSDAILYDATA set In_Punch=@piInPunch,Out_Punch=@pioutpunch,Status ='M' where PDate=@piWorkDate and Emp_ID=@empid
end
else if(@piInPunch !='' and @piOutPunch !='' and (@breakout_Manual is not null or @breakin_Manual is not null))
begin
--update MASTERPROCESSDAILYDATA set In_Punch=convert(time,@piInPunch),Out_Punch=convert(time,@pioutpunch),BreakOut = CONVERT(time,@breakout_Manual),BreakIn = CONVERT(time,@breakin_Manual),Status ='M' where PDate=@piWorkDate and Emp_ID=@empid
update MASTERPROCESSDAILYDATA set In_Punch=@piInPunch,Out_Punch=@pioutpunch,BreakOut = @breakout_Manual,BreakIn = @breakin_Manual,Status ='M' where PDate=@piWorkDate and Emp_ID=@empid
end
if @piInPunch ='' or @piInPunch is null
begin
select @piInPunch=in_punch from MASTERPROCESSDAILYDATA where PDate=@piWorkDate and Emp_ID=@empid
if @piInPunch ='' or @piInPunch is null
begin
update MASTERPROCESSDAILYDATA set In_Punch=null,Status='MO' where PDate=@piWorkDate and Emp_ID=@empid
set @intime =null
end
else
begin
update MASTERPROCESSDAILYDATA set Status='MO' where PDate=@piWorkDate and Emp_ID=@empid
select @intime=convert(time,In_Punch) from MASTERPROCESSDAILYDATA where PDate=@piWorkDate and Emp_ID=@empid
end
end
else
begin
select @intime=convert(time,@piInPunch)
end
if @piOutPunch ='' or @piOutPunch is null
begin
select @piOutPunch =Out_Punch from MASTERPROCESSDAILYDATA where PDate=@piWorkDate and Emp_ID=@empid
if @piOutPunch ='' or @piOutPunch is null
begin
update MASTERPROCESSDAILYDATA set Out_Punch=null,Status='MI' where PDate=@piWorkDate and Emp_ID=@empid
set @outtime =null
end
else
begin
update MASTERPROCESSDAILYDATA set Status='MI' where PDate=@piWorkDate and Emp_ID=@empid
select @outtime=CONVERT(time,Out_Punch) from MASTERPROCESSDAILYDATA where PDate=@piWorkDate and Emp_ID=@empid
end
end
else
begin
select @outtime=CONVERT(time,@piOutPunch)
end


if(@app=1)
begin
update MASTERPROCESSDAILYDATA set Status='MO' where PDate=@piWorkDate and Emp_ID=@empid
end
else if(@app=2)
begin
update MASTERPROCESSDAILYDATA set Status='MI' where PDate=@piWorkDate and Emp_ID=@empid
end
else
begin
update MASTERPROCESSDAILYDATA set Status='M' where PDate=@piWorkDate and Emp_ID=@empid
end


set @intime =@piWorkDate+ @piInPunch
set @outtime =@piWorkDate+@piOutPunch
select @shiftintime =(@piWorkDate+CONVERT(time,Shift_In)),@shiftouttime=(@piWorkDate+CONVERT(time,Shift_Out)) from MASTERPROCESSDAILYDATA where PDate=@piWorkDate and Emp_ID=@empid

set @LateComersMin = NUll
set @LateComerstimeformat= NUll
set @EarlyLeavers = NUll
set @Earlyleaversformat=NUll
set @TotalHrs= NUll
set @TotalHrsMin=NUll
set @overtime=NUll
set @Totalhrstime=NUll
set @workhrs=NUll
set @maxovertime=NUll
set @Overtimetimeformat=null
set @sOvertimetimeformat =null
set @BoutEarlyBy=null
set @BinLateBy=null  
--select @ShiftCode = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@empid
select @ShiftCode= Shift_Code from MASTERPROCESSDAILYDATA where PDate=@piWorkDate and Emp_ID=@empid
select @OTE=OT_Eligibility from EmployeeMaster where Emp_Code=@empid
select  @empcat= emp_employee_category from EmployeeMaster where Emp_Code=@empid
if(@includep = 1)
       begin
              select @workhrs =CONVERT(time(7),Totalhrs) from employeecategorymaster where empcategorycode=@empcat
       end
else
       begin
              select @workhrs =MaxOverTime_General from shift where Shift_Code = @ShiftCode
       end
if(@intime is not null and @outtime is not null and @breakout_Manual is null and @breakin_Manual is null)
       Begin
              select @TotalHrsMin = DATEDIFF(Minute,@intime,@outtime)              
              select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
              
       End
else if(@intime is not null and @outtime is not null and(@breakout_Manual is not null or @breakin_Manual is not null))
       begin
              set    @TotalHrsMin = DATEDIFF(Minute,@intime,@breakout_Manual) + DATEDIFF(Minute,@breakin_Manual,@outtime)
              select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
       end
set @rtDayofWeek = Datename(DW,@piWorkDate)
set @WeeklyOff1=null
set @WeeklyOff2=null
select @WeeklyOff1=WeeklyOff1, @WeeklyOff2=WeeklyOff2 from Shift where Shift_Code=@ShiftCode

if(@intime is not null and @outtime is not null)
Begin
    select @breakminute=((DATEPART (HH, convert(time,convert(datetime,@breakin)-CONVERT(datetime,@breakout)))*60)+DATEPART (MI, convert(time,convert(datetime,@breakin)-CONVERT(datetime,@breakout))))
       if @breakminute is null or @breakminute=''
              begin
                     set @breakminute = 0
              end
end



if(@intime>@shiftintime )
begin
       select @LateComersMin = Datediff(Minute,@shiftintime,@intime)
       select @LateComerstimeformat = Convert(char(8),DateAdd(n,@LateComersMin,0),108)
end
if(@outtime<@shiftouttime)
begin
       select @EarlyLeavers =DATEDIFF(Minute,@outtime,@shiftouttime)
       select @Earlyleaversformat =CONVERT(char(8),dateadd(n,@EarlyLeavers,0),108)
end

--calculation for the Break in Late By
if(@breakin_Manual is not null)
begin
if(CONVERT(time,@breakin_Manual)>(select CONVERT(time,DATEADD(MINUTE,Grace_Bin,breakin)) from Shift where Shift_Code=@ShiftCode))
       begin
              select @BinLatebyMin = DATEDIFF(Minute,CONVERT(time,breakin),convert(time,@breakin_Manual)) from Shift where Shift_Code = @ShiftCode
              select @BinLateBy = Convert(char(8),DateAdd(n,@BinLatebyMin,0),108)--calculation for the EarlyComers
       END
End
--calculation for the Break in Late By


--Caluclation of Early by in bout
if(@breakout_Manual is not null)
begin
if(convert(time,@breakout_Manual) < (select convert(time,DATEADD(MINUTE,-Grace_Bout,breakout)) from Shift where Shift_Code= @ShiftCode))
       begin
              select @BoutEarlyByMin = DATEDIFF(Minute,CONVERT(time,@breakout_Manual),convert(time,breakout)) from Shift where Shift_Code = @ShiftCode
              select @BoutEarlyBy = CONVERT(char(8),Dateadd(n,@BoutEarlyByMin,0),108) --Caluclation of Early by in bout
       end
end
--Caluclation of Early by in bout End

IF (@rtDayofWeek = @WeeklyOff1 or @rtDayofWeek = @WeeklyOff2 or @ShiftCode = 'woff')
Begin
    set @LateComerstimeformat=null
       set @Earlyleaversformat=null
       set @BoutEarlyBy=null
       set @BinLateBy=null
       --select @TotalHrsMin = DATEDIFF(Minute,@intime,@outtime)
       set @TotalHrsMin=@TotalHrsMin-@breakminute

       if((@TotalHrsMin >= @MinReqHrsForDed) and(@RequireDeduction =1))
               begin
               set @TotalHrsMin = @TotalHrsMin - @MinsToBeDeducted
               end
       select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
       select @sOvertimetimeformat=CONVERT(char(8),convert(time,@TotalHrs))
end
else if exists(select 1 from holidaylist where hdate=@piWorkDate and hgroup in (select HolidayCode from BranchMaster where BranchCode = @Cat_Code))
begin
     set @LateComerstimeformat=null
       set @Earlyleaversformat=null
       set @BoutEarlyBy=null
       set @BinLateBy=null  
       --select @TotalHrsMin = DATEDIFF(Minute,@intime,@outtime)
       set @TotalHrsMin=@TotalHrsMin-@breakminute

       if((@TotalHrsMin >= @MinReqHrsForDed) and(@RequireDeduction =1))
        begin
        set @TotalHrsMin = @TotalHrsMin - @MinsToBeDeducted
        end
        
       select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
       select @sOvertimetimeformat=CONVERT(char(8),convert(time,@TotalHrs))
end
Else
Begin
       set @TotalHrsMin=@TotalHrsMin-@brkHrsMinute
       select @TotalHrs = CONVERT(char(8),DATEADD(n,@TotalHrsMin,0),108)
End

--else
--Begin
--     select @TotalHrsMin = DATEDIFF(Minute,@intime,@outtime)
--End

--select @ShiftTime = CONVERT(time,@shiftouttime)
--select @OutTime = CONVERT(time,@outtime)
set @Totalhrstime = Convert(Time,(DATEADD(n,@TotalHrsMin,0)))
if(@Totalhrstime > @workhrs and @OTE='1' and @sOvertimetimeformat is null)
Begin
set @overtime=convert(datetime,@Totalhrstime)-convert(datetime,@workhrs)
select @maxovertime=MaxWorkTime from shift where Shift_Code =@ShiftCode
select @minovertime = minovertime from Shift where Shift_Code =@ShiftCode
if @maxovertime !=''
begin
if @overtime >@maxovertime
begin
set @overtime =@maxovertime
end
end
if @minovertime!=''
begin
if @overtime <@minovertime
begin
set @overtime =null
end
end
if ((@maxovertime ='00:00:00.000') and (@minovertime='00:00:00.000'))
begin
set @overtime =null
end

set @Overtimetimeformat=CONVERT(varchar,@overtime)
End
--update MASTERPROCESSDAILYDATA set In_Punch=convert(time,@piInPunch) ,Out_Punch =convert(time,@piOutPunch) ,EarlyBy=@Earlyleaversformat,LateBy=@LateComerstimeformat,OT=@Overtimetimeformat,WorkHRs=@TotalHrs,SOT=@sOvertimetimeformat where PDate=@piWorkDate and Emp_ID=@empid
update MASTERPROCESSDAILYDATA set EarlyBy=@Earlyleaversformat,LateBy=@LateComerstimeformat,BreakIn_LateBy=@BinLateBy,BreakOut_EarlyBy=@BoutEarlyBy,OT=@Overtimetimeformat,WorkHRs=@TotalHrs,SOT=@sOvertimetimeformat where PDate=@piWorkDate and Emp_ID=@empid
end
set @index=@index+1
end

       --Manual Punch New One Fixed Reprocessing issue End    
       update MASTERPROCESSDAILYDATA set shift_in =null ,shift_out=null, Total_hrs=null,EarlyBy=null,LateBy=null,EarlyCome=null,LateGo=null where (status='WO' or status='WOP'  or status='HP' or  status='H') and  PDate Between  @fdate and @tdate       
       update MASTERPROCESSDAILYDATA set workhrs='00:00:00' where (workhrs is null or workhrs='' or workhrs='NULL')
END


