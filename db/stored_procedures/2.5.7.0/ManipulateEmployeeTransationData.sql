



create PROCEDURE [dbo].[ManipulateEmployeeTransationData]

(
@Mode varchar(max),
@EmployeeCode varchar(max),
@EmployeeName varchar(max)=null,
@EmployeeCompany varchar(max)=null,
@EmployeeBranch varchar(max)=null,
@EmployeeDepartment varchar(max)=null,
@EmployeeCategory varchar(max)=null,
@ShiftFromdate  date = null , 
@ShiftTodate date = null , 
@Shift_Eligibility int = 0 ,
@Shift_name varchar(20) = null , 
@OTFromdate date = null , 
@OTTodate date = null , 
@OT_Eligibility int = 0 ,
@RamadanFromdate datetime = null , 
@RamadanTodate datetime = null , 
@Ramadan_Eligibility int = 0 ,
@PunchExceptionFromdate date = null ,
@PunchExceptionTodate date = null ,
@PunchException_Eligibility int = 0 ,
@MaternityFromdate date = null , 
@MaternityTodate date = null , 
@Maternity_Eligibility int = 0 , 
@ChildDateofBirth date = null , 
@WorkHourPerdayFromdate date = null , 
@WorkHourPerdayTodate date = null , 
@WorkHourPerday_Eligibility int = 0 , 
@WorkHourPerday int = 0 , 
@WorkHourPerWeekFromdate date = null , 
@WorkHourPerWeekTodate date = null , 
@WorkHourPerWeek_Eligibility int = 0 , 
@WorkHourPerWeek int = 0 , 
@WorkHourPerMonthFromdate date = null , 
@WorkHourPerMonthTodate date = null , 
@WorkHourPerMonth_Eligibility int = 0 , 
@WorkHourPerMonth int = 0 , 
@Terminationdate date = null , 
@LineManagerFromdate date = null , 
@LineManagerTodate date = null , 
@LineManagerID  varchar(100) = null ,
@Line_Manager_Eligibility int = 0 , 
@Termination_Eligibility int = 0 ,
@MaternityBreakHours time = null 


 

)

as	

BEGIN

		select @EmployeeBranch = emp_branch from employeemaster where emp_code = @EmployeeCode
		select @EmployeeName = emp_name from employeemaster where emp_code = @EmployeeCode
		select @EmployeeCompany = emp_company from employeemaster where emp_code = @EmployeeCode
		select @EmployeeDepartment = emp_department from employeemaster where emp_code = @EmployeeCode
		
		select @EmployeeCategory = Emp_Employee_Category from employeemaster where emp_code = @EmployeeCode
	
	IF @Mode='I'

	Begin

	

		insert into Employee_TransactionData(EmployeeCode,EmployeeName,EmployeeCompany,EmployeeBranch,EmployeeDepartment,EmployeeCategory,
		ShiftFromdate,  ShiftTodate,Shift_Eligibility,Shift_name,
		OTFromdate,OTTodate,OT_Eligibility,
		RamadanFromdate,RamadanTodate,Ramadan_Eligibility,
		PunchExceptionFromdate,PunchExceptionTodate,PunchException_Eligibility,
		MaternityFromdate,MaternityTodate,Maternity_Eligibility,ChildDateofBirth, MaternityBreakHours ,
		WorkHourPerdayFromdate,WorkHourPerdayTodate,WorkHourPerday_Eligibility, WorkHourPerday,
		WorkHourPerWeekFromdate,WorkHourPerWeekTodate , WorkHourPerWeek_Eligibility ,WorkHourPerWeek ,
		 WorkHourPerMonthFromdate , WorkHourPerMonthTodate , WorkHourPerMonth_Eligibility , WorkHourPerMonth , 
		 Termination_Eligibility ,  Terminationdate , 
		 LineManagerFromdate , LineManagerTodate , LineManagerID , Line_Manager_Eligibility)
		values(@EmployeeCode ,@EmployeeName,@EmployeeCompany,@EmployeeBranch,@EmployeeDepartment,@EmployeeCategory,
		@ShiftFromdate,@ShiftTodate , @Shift_Eligibility,@Shift_name ,
		 @OTFromdate , @OTTodate , @OT_Eligibility , 
		 @RamadanFromdate , @RamadanTodate , @Ramadan_Eligibility ,
		  @PunchExceptionFromdate , @PunchExceptionTodate,@PunchException_Eligibility,
		  @MaternityFromdate,@MaternityTodate,@Maternity_Eligibility,@ChildDateofBirth ,@MaternityBreakHours , 
		   @WorkHourPerdayFromdate ,@WorkHourPerdayTodate , @WorkHourPerday_Eligibility , @WorkHourPerday ,
		    @WorkHourPerWeekFromdate , @WorkHourPerWeekTodate , @WorkHourPerWeek_Eligibility,@WorkHourPerWeek , 
			@WorkHourPerMonthFromdate , @WorkHourPerMonthTodate , @WorkHourPerMonth_Eligibility , @WorkHourPerMonth ,
			 @Termination_Eligibility , @Terminationdate ,
			 @LineManagerFromdate , @LineManagerTodate , @LineManagerID , @Line_Manager_Eligibility)
		

	

	End

	

	IF @Mode='U'

	Begin

	update  Employee_TransactionData set EmployeeCode = @EmployeeCode ,EmployeeName = @EmployeeName ,EmployeeCompany = @EmployeeCompany ,EmployeeBranch = @EmployeeBranch ,
EmployeeDepartment = @EmployeeDepartment ,EmployeeCategory = @EmployeeCategory ,ShiftFromdate = @ShiftFromdate ,ShiftTodate = @ShiftTodate ,Shift_Eligibility = @Shift_Eligibility ,
Shift_name = @Shift_name ,OTFromdate = @OTFromdate ,OTTodate = @OTTodate ,OT_Eligibility = @OT_Eligibility ,RamadanFromdate = @RamadanFromdate ,RamadanTodate = @RamadanTodate ,
Ramadan_Eligibility = @Ramadan_Eligibility ,PunchExceptionFromdate = @PunchExceptionFromdate ,PunchExceptionTodate = @PunchExceptionTodate ,
PunchException_Eligibility = @PunchException_Eligibility ,MaternityFromdate = @MaternityFromdate ,MaternityTodate = @MaternityTodate ,Maternity_Eligibility = @Maternity_Eligibility,
MaternityBreakHours = @MaternityBreakHours ,ChildDateofBirth = @ChildDateofBirth ,WorkHourPerdayFromdate = @WorkHourPerdayFromdate ,WorkHourPerdayTodate = @WorkHourPerdayTodate ,
WorkHourPerday_Eligibility = @WorkHourPerday_Eligibility ,WorkHourPerday = @WorkHourPerday ,WorkHourPerWeekFromdate = @WorkHourPerWeekFromdate ,
WorkHourPerWeekTodate = @WorkHourPerWeekTodate ,WorkHourPerWeek_Eligibility = @WorkHourPerWeek_Eligibility ,WorkHourPerWeek = @WorkHourPerWeek ,
WorkHourPerMonthFromdate = @WorkHourPerMonthFromdate ,WorkHourPerMonthTodate = @WorkHourPerMonthTodate ,WorkHourPerMonth_Eligibility = @WorkHourPerMonth_Eligibility ,
WorkHourPerMonth = @WorkHourPerMonth ,Terminationdate = @Terminationdate ,LineManagerFromdate = @LineManagerFromdate ,LineManagerTodate = @LineManagerTodate ,
LineManagerID = @LineManagerID where EmployeeCode = @EmployeeCode

		
		

	End

	

	IF @Mode='D'
	Begin
		print 2 
	End

END


