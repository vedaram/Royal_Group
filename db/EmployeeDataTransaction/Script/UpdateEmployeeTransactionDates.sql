


Create Procedure [dbo].[UpdateEmployeeTransactionDates] 
as
begin
	
Declare
	@TransactionID bigint, @EmployeeCode varchar(100), @Shift_code varchar(100), @Terminationdate datetime, @LineManagerID varchar(100), @HistoryTransactionID int,
	@CurrentDate date

	/*Current date*/
	Set @CurrentDate =Format(GetDate(), N'yyyy-MM-dd') -- CONVERT(date,GETDATE())
	
	/*Employee Cursor*/
	DECLARE EmployeeCursor CURSOR for select Emp_Code from EmployeeMaster where Emp_Status=1   and emp_code = '14' 

	OPEN EmployeeCursor

	FETCH NEXT FROM EmployeeCursor into @EmployeeCode

	WHILE @@FETCH_STATUS = 0
	begin	

		/*Store history*/			
		Insert into EmployeeTransactionHistory  values (GETDATE(),@EmployeeCode,0,0,0,0,0,0,0,0,0,0)
		Select @HistoryTransactionID= max(TransactionID) from EmployeeTransactionHistory where EmployeeCode=@EmployeeCode

		/*Shift*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=1 order by ID desc)
		if(@TransactionID>0)
		Begin
			Select @Shift_code = TransactionData  from EmployeetransactionData where ID=@TransactionID
			update EmployeeMaster set Emp_Shift_Detail=@Shift_code where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set Shift=1 where TransactionID=@HistoryTransactionID
		End

		/*OT*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=2 order by ID desc)
		if(@TransactionID>0)
		Begin			
			update EmployeeMaster set OT_Eligibility=1 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set OverTime=1 where TransactionID=@HistoryTransactionID			
		End
		Else
		Begin
			update EmployeeMaster set OT_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*Ramadan*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=3 order by ID desc)
		if(@TransactionID>0)
		Begin			
			update EmployeeMaster set Ramadan_Eligibility=1 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set Ramadan=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set Ramadan_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*Punch Exception*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=4 order by ID desc)
		if(@TransactionID>0)
		Begin
			update EmployeeMaster set PunchException_Eligibility=1 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set PunchException=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set PunchException_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*Maternity*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=5 order by ID desc)
		if(@TransactionID>0)		
		Begin
			update EmployeeMaster set Maternity_Eligibility=1, MaternityBreakHours='01:00' where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set Maternity=1 where TransactionID=@HistoryTransactionID
		End
		else
		Begin
			update EmployeeMaster set Maternity_Eligibility=0, MaternityBreakHours='00:00' where Emp_Code=@EmployeeCode
		End

		/*WorkHourPerDay*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=6 order by ID desc)
		if(@TransactionID>0)		
		Begin
			update EmployeeMaster set WorkHourPerday_Eligibility=1 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set WorkHourPerday=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set WorkHourPerday_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*@WorkHourPerWeek*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=7 order by ID desc)
		if(@TransactionID>0)		
		Begin
			update EmployeeMaster set WorkHourPerWeek_Eligibility=1 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set WorkHourPerWeek=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set WorkHourPerWeek_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		/*WorkHourPerMonth*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=8 order by ID desc)
		if(@TransactionID>0)		
		Begin
			update EmployeeMaster set WorkHourPerMonth_Eligibility=1 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set WorkHourPerMonth=1 where TransactionID=@HistoryTransactionID
		End
		Else
		Begin
			update EmployeeMaster set WorkHourPerMonth_Eligibility=0 where Emp_Code=@EmployeeCode
		End

		
		/*Termination*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and TransactionType=9 order by ID desc)
		if(@TransactionID>0)		
		Begin
			select @Terminationdate = TransactionData from EmployeetransactionData where ID = @TransactionID

			update EmployeeMaster set Emp_Dol=@Terminationdate,Emp_Status=2 where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set Termination=1 where TransactionID=@HistoryTransactionID
		End		
					
		/*LineManager*/
		Set @TransactionID=0
		Set @TransactionID = (select top 1 ID  from EmployeetransactionData where @CurrentDate between FromDate and ToDate and EmpID=@EmployeeCode and isactive = 1  and TransactionType=10 order by ID desc)
		if(@TransactionID>0)				
		Begin
			select @LineManagerID = TransactionData from EmployeetransactionData where ID = @TransactionID

			update EmployeeMaster set ManagerId=@LineManagerID where Emp_Code=@EmployeeCode
			update EmployeeTransactionHistory set LineManagerID=1 where TransactionID=@HistoryTransactionID
		End
		else
		Begin
			update EmployeeMaster set ManagerId = NULL where Emp_Code=@EmployeeCode
		End
		
		Set @Terminationdate = NULL
		Set @LineManagerID = NULL
		Set @Shift_code = NULL
	
		FETCH NEXT FROM EmployeeCursor into @EmployeeCode

	End

	CLOSE EmployeeCursor  --close the cursor

	DEALLOCATE EmployeeCursor 

End