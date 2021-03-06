



ALTER PROCEDURE [dbo].[ManipulateEmployee]

(

	@Mode varchar(max),

	@Emp_Code varchar(max),

	@Emp_New_Code varchar(max)=null,

	@Emp_Name varchar(max)=null,

	@Emp_Dob date=null,

	@Emp_Gender varchar(max)=null,

	@Emp_Address varchar(max)=null,

	@Emp_Phone varchar(max)=null,

	@Emp_Email varchar(max)=null,

	@Emp_Doj date=null,

	@Emp_Dol date=null,	

	@Emp_Company varchar(max)=null,

	@Emp_Branch varchar(max)=null,

	@Emp_Department varchar(max)=null,

	@Emp_Designation varchar(max)=null,

	@Emp_Employee_Category varchar(max)=null,

	@Emp_Card_No varchar(max)=null,

	@Emp_Shift_Detail varchar(max)=null,

	@Emp_Status varchar(max),

	@Emp_Photo image=null,

	@OT_Eligibility varchar(1)=null,	

	@Passport_No varchar(30)=null,

	@Passprt_Exp_Date date=null,

	@Emirates_No varchar(30)=null,	

	@_Nationality varchar(max)=null,

	@Emergency_Contact_No varchar(max)=null,

	@Visa_Exp_Date Date=null,	

	@IsManager varchar(max)=null,

	@managerid varchar(max)=null,

	@IsHR Varchar(10)=null,

	@IsAutoShiftEligible int=0,

	@religion varchar(50)=null

	)

as	

BEGIN

	Declare @CategoryCount int, @NewCategory varchar(100), @OldCategory varchar(100), @NewRuleCode varchar(100), @OldRuleCode varchar(100)
	
	IF @Mode='I'

	Begin

	-- sanjeev 24-08-2015

		insert into EmployeeMaster(Emp_Code,Emp_Name,Emp_Dob,Emp_Gender,Emp_Address,Emp_Phone,Emp_Email,

		Emp_Doj,Emp_Company,Emp_Branch,Emp_Department,Emp_Designation,

		Emp_Employee_Category,Emp_Card_no,Emp_Shift_Detail,Emp_Status,Emp_Photo,OT_Eligibility,Passport_No,Passport_Exp_Date,

		Emirates_No,Nationality,Emergency_Contact_No,Visa_Exp_Date,IsManager,ManagerId,Emp_Dol,IsHR,IsAutoShiftEligible , Employee_Religion)

		values(@Emp_Code ,@Emp_Name,@Emp_Dob,@Emp_Gender,@Emp_Address,@Emp_Phone,@Emp_Email,

		@Emp_Doj,@Emp_Company,@Emp_Branch,@Emp_Department,@Emp_Designation,

		@Emp_Employee_Category,@Emp_Card_No,@Emp_Shift_Detail,@Emp_Status,@Emp_Photo,@OT_Eligibility,@Passport_No,@Passprt_Exp_Date,@Emirates_No

		,@_Nationality,@Emergency_Contact_No,@Visa_Exp_Date,@IsManager,@managerid,@Emp_Dol,@IsHR,@IsAutoShiftEligible , @religion)

		exec AutoCreditLeavePolicy @Emp_Code,1

	End

	

	IF @Mode='U'

	Begin

	-- sanjeev 24-08-2015
		Set @CategoryCount =(Select Count(*)  from EmployeeMaster where Emp_code=@Emp_Code and Emp_Employee_Category=@Emp_Employee_Category )

		/*OLD records*/
		Select @OldCategory = Emp_Employee_Category from EmployeeMaster where Emp_code=@Emp_Code
		if(@OldCategory!=@Emp_Employee_Category)
		exec GetRuleCode @Emp_Code, @OldRuleCode output

		Select @NewCategory = @Emp_Employee_Category

		update EmployeeMaster set Emp_Code=@Emp_Code,Emp_Name=@Emp_Name,Emp_Dob=@Emp_Dob,Emp_Gender=@Emp_Gender,Emp_Address=@Emp_Address,

		Emp_Phone=@Emp_Phone,Emp_Email=@Emp_Email,Emp_Doj=@Emp_Doj,

		Emp_Company=@Emp_Company,Emp_Branch=@Emp_Branch,

		Emp_Department=@Emp_Department,Emp_Designation=@Emp_Designation,Emp_Employee_Category=@Emp_Employee_Category,

		Emp_Card_No=@Emp_Card_No,Emp_Shift_Detail=@Emp_Shift_Detail,Emp_Status=@Emp_Status,Emp_Photo=@Emp_Photo,Passport_No=@Passport_No,

		Passport_Exp_Date=@Passprt_Exp_Date,Emirates_No=@Emirates_No,Nationality = @_Nationality,Emergency_Contact_No = @Emergency_Contact_No,

		Visa_Exp_Date = @Visa_Exp_Date,IsManager=@IsManager,ManagerId=@managerid,Emp_Dol=@Emp_Dol,IsHR=@IsHR,IsAutoShiftEligible=@IsAutoShiftEligible 
		
		, Employee_Religion = @religion  where Emp_Code=@Emp_Code

		
		IF(@OldCategory!=@Emp_Employee_Category)
		Begin
			/*New rule code*/
			exec GetRuleCode @Emp_Code, @NewRuleCode output
			
			IF not exists(select 1 from CategoryChangeHistory where EmpID= @Emp_Code)
			Begin
				insert into CategoryChangeHistory values (@Emp_Code,@OldCategory,@OldRuleCode,@NewCategory,@NewRuleCode,convert(Date,getdate()))
			End
			Else
			Begin
				update CategoryChangeHistory set  OldCategory=@OldCategory,OldRuleCode=@OldRuleCode,NewCategory=@NewCategory,NewRuleCode=@NewRuleCode, ChangeDate=convert(Date,getdate()) where EmpID= @Emp_Code
			End

			exec AutoCreditLeavePolicy @Emp_Code,2
		End

	End

	

	IF @Mode='D'
	Begin
		delete from EmployeeMaster where Emp_Code=@Emp_Code
	End

END


