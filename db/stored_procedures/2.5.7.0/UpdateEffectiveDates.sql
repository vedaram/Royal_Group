Create Procedure [dbo].[UpdateEffectiveDates]
as
begin
	
Declare
	@TransactionID INT, @EmployeeCode varchar(100), @EmployeeName varchar(100), @EmployeeCompany varchar(50), @EmployeeBranch varchar(100), @EmployeeDepartment varchar(100),
	@EmployeeCategory varchar(100), @ShiftFromdate datetime, @ShiftTodate datetime, @Shift_Eligibility int, @Shift_code varchar(100), @OTFromdate datetime, @OTTodate datetime,
	@OT_Eligibility int, @RamadanFromdate datetime, @RamadanTodate datetime, @Ramadan_Eligibility int, @PunchExceptionFromdate datetime, @PunchExceptionTodate datetime,
	@PunchException_Eligibility int, @MaternityFromdate datetime, @MaternityTodate datetime, @Maternity_Eligibility int, @MaternityBreakHours time(7), @ChildDateofBirth datetime,
	@WorkHourPerdayFromdate datetime, @WorkHourPerdayTodate datetime, @WorkHourPerday_Eligibility int, @WorkHourPerday int, @WorkHourPerWeekFromdate datetime, @WorkHourPerWeekTodate datetime ,
	@WorkHourPerWeek_Eligibility int, @WorkHourPerWeek int, @WorkHourPerMonthFromdate datetime, @WorkHourPerMonthTodate datetime, @WorkHourPerMonth_Eligibility int,
	@WorkHourPerMonth int, @Terminationdate datetime, @LineManagerFromdate datetime, @LineManagerTodate datetime, @LineManagerID varchar(100), @HistoryTransactionID int,
	@CurrentDate datetime

	/*Current date*/
	Set @CurrentDate = CONVERT(date,GETDATE())
	
	/*Employee Cursor*/
	DECLARE EmployeeCursor CURSOR for select Emp_Code from EmployeeMaster where Emp_Status=1 

	OPEN EmployeeCursor

	FETCH NEXT FROM EmployeeCursor into @EmployeeCode

	WHILE @@FETCH_STATUS = 0
	begin	

		/*Store history*/			
		Insert into EmployeeTransactionHistory  values (GETDATE(),@EmployeeCode,0,0,0,0,0,0,0,0,0,0)

		/*get max id*/
		Select @HistoryTransactionID= max(TransactionID) from EmployeeTransactionHistory where EmployeeCode=@EmployeeCode

		/*Shift*/
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between ShiftFromdate and ShiftTodate
		if(@TransactionID>0)
		Begin				
			select @Shift_Eligibility = Shift_Eligibility,	@Shift_code = Shift_code from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				
			if(@Shift_Eligibility=1)
			Begin
				update EmployeeMaster set Emp_Shift_Detail=@Shift_code where Emp_Code=@EmployeeCode
				update EmployeeTransactionHistory set Shift=1 where TransactionID=@HistoryTransactionID
			End			
		End
		
				
		/*OT*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between OTFromdate and OTTodate
		if(@TransactionID>0)
		Begin
			select @OT_Eligibility = OT_Eligibility from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				
			
			update EmployeeMaster set OT_Eligibility=@OT_Eligibility where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set OverTime=1 where TransactionID=@HistoryTransactionID			
		End
		Else
		Begin
			update EmployeeMaster set OT_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*Ramadan*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between RamadanFromdate and RamadanTodate
		if(@TransactionID>0)
		Begin
			select @Ramadan_Eligibility = Ramadan_Eligibility from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				

			update EmployeeMaster set Ramadan_Eligibility=@Ramadan_Eligibility where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set Ramadan=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set Ramadan_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*Punch Exception*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between PunchExceptionFromdate and PunchExceptionTodate
		if(@TransactionID>0)
		Begin
			select @PunchException_Eligibility = PunchException_Eligibility from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				

			update EmployeeMaster set PunchException_Eligibility=@PunchException_Eligibility where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set PunchException=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set PunchException_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*Maternity*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between MaternityFromdate and MaternityTodate
		if(@TransactionID>0)		
		Begin
			select @Maternity_Eligibility = Maternity_Eligibility from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				

			update EmployeeMaster set Maternity_Eligibility=@Maternity_Eligibility, MaternityBreakHours='01:00' where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set Maternity=1 where TransactionID=@HistoryTransactionID
		End
		else
		Begin
			update EmployeeMaster set Maternity_Eligibility=0, MaternityBreakHours='00:00' where Emp_Code=@EmployeeCode
		End

		/*WorkHourPerDay*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between WorkHourPerdayFromdate and WorkHourPerdayTodate
		if(@TransactionID>0)		
		Begin
			select @WorkHourPerday_Eligibility = WorkHourPerday_Eligibility from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				
			
			update EmployeeMaster set WorkHourPerday_Eligibility=@WorkHourPerday_Eligibility where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set WorkHourPerday=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set WorkHourPerday_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*@WorkHourPerWeek*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between @WorkHourPerWeekFromdate and @WorkHourPerWeekTodate
		if(@TransactionID>0)		
		Begin
			select @WorkHourPerWeek_Eligibility = WorkHourPerWeek_Eligibility from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				

			update EmployeeMaster set WorkHourPerWeek_Eligibility=@WorkHourPerWeek_Eligibility where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set WorkHourPerWeek=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set WorkHourPerWeek_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*WorkHourPerMonth*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between WorkHourPerMonthFromdate and WorkHourPerMonthTodate
		if(@TransactionID>0)		
		Begin
			select @WorkHourPerMonth_Eligibility = WorkHourPerMonth_Eligibility from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				

			update EmployeeMaster set WorkHourPerMonth_Eligibility=@WorkHourPerMonth_Eligibility where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set WorkHourPerMonth=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set WorkHourPerMonth_Eligibility=0 where Emp_Code=@EmployeeCode
		End
			
		/*LineManager*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate between LineManagerFromdate and LineManagerTodate
		if(@TransactionID>0)				
		Begin
			select @LineManagerID = LineManagerID from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				

			update EmployeeMaster set ManagerId=@LineManagerID where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set LineManagerID=1 where TransactionID=@HistoryTransactionID
		End
		

		/*Termination*/
		Set @TransactionID=0
		Select @TransactionID=TransactionID from Employee_TransactionData where EmployeeCode = @EmployeeCode and @CurrentDate=Terminationdate
		if(@TransactionID>0)		
		Begin
			select @Terminationdate = Terminationdate from Employee_TransactionData where EmployeeCode = @EmployeeCode and TransactionID=@TransactionID				

			update EmployeeMaster set Emp_Dol=@Terminationdate,Emp_Status=2 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set Termination=1 where TransactionID=@HistoryTransactionID
		End		
	
		Set @Shift_Eligibility=NULL
		Set @OT_Eligibility=NULL
		Set @PunchException_Eligibility=NULL
		Set @Maternity_Eligibility=NULL		
		Set @WorkHourPerMonth_Eligibility=NULL
		Set @WorkHourPerWeek_Eligibility=NULL
		Set @WorkHourPerday_Eligibility=NULL
		Set @Terminationdate=NULL
		Set @LineManagerID=NULL
	
		FETCH NEXT FROM EmployeeCursor into @EmployeeCode

	End

	CLOSE EmployeeCursor  --close the cursor

	DEALLOCATE EmployeeCursor 

End