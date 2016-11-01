Create procedure [dbo].[SaveSAXPushData]
( 
@CardNo varchar(30),
@PunchTime datetime,
@PunchDate Datetime,
@DeviceID varchar(30),
@VerifyMode varchar(30),
@Status varchar(30)
) 
AS
begin

	SET NOCOUNT ON;
	
	declare @Emp_Code varchar(30)
	
	--select @Emp_Code=Emp_Code from EmployeeMaster  where Emp_Card_No=@CardNo
	
	   INSERT INTO  Trans_Raw#_All(EmpId,Punch_Time, PunchDate,Verification_Code,CardNo,Deviceid,status,status1,ReceivedTime)
	   VALUES (@Emp_Code,@PunchTime,@PunchDate,@VerifyMode,@CardNo,@DeviceID,@Status,'',GETDATE())

	  -- if(@Emp_Code is NOT null)
		 --begin
			-- INSERT INTO  Trans_Raw#_Temp(EmpId,Punch_Time, PunchDate,Verification_Code,CardNo,Deviceid,status,status1)
			-- VALUES (@Emp_Code,@PunchTime,@PunchDate,@VerifyMode,@CardNo,@DeviceID,@Status,'')
		 --end
	  -- else
		 --begin
			--  INSERT INTO  Unprocessed_Punches(EmpId,Punch_Time, PunchDate,Verification_Code,CardNo,Deviceid,status,status1)
			-- VALUES (@Emp_Code,@PunchTime,@PunchDate,@VerifyMode,@CardNo,@DeviceID,@Status,'')
		 --end
		
	SET NOCOUNT OFF;
End	
Go

Create procedure [dbo].[SaveSAXPushLastDate]
(
@DeviceID varchar(30),
@StampFlag int,
@Stamp bigint,
@LastPunchDateTime varchar(25)
)
as 
begin
	--*****Attendance Stamp update*****
	if(@StampFlag=0)
	begin
		if(Not exists(select LastPunchDateTime from LastDateSAXPush where DeviceID=@DeviceID))
			begin
				INSERT INTO LastDateSAXPush(DeviceId,DataStamp,LastPunchDateTime)
								  values (@DeviceID,@Stamp,@LastPunchDateTime)
								  insert into Device_Location (DeviceID) values (@DeviceID)
			end
		else
			begin
				update LastDateSAXPush set DataStamp=@Stamp, LastPunchDateTime=@LastPunchDateTime where DeviceID=@DeviceID
			end
	end
	
	--*****OperationLog Stamp update*****
	if(@StampFlag=1)
	begin
		if(Not exists(select LastPunchDateTime from LastDateSAXPush where DeviceID=@DeviceID))
			begin
				INSERT INTO LastDateSAXPush(DeviceId,TemplateStamp)
								 values (@DeviceID,@Stamp)
								 insert into Device_Location (DeviceID) values (@DeviceID)
			end
		else
			begin
				update LastDateSAXPush set TemplateStamp=@Stamp where DeviceID=@DeviceID
			end
	end
	
end
Go

Create procedure [dbo].[SaveSAXPushTemplate]
(
@DeviceID varchar(30),
@EnrollID varchar(50),
@FPID varchar(10),
@FPTemplate varchar(max),
@Size varchar(10)
)
as
Begin

	Declare @Name varchar(100)
	select @Name=Emp_Name from EmployeeMaster where Emp_Card_No=@EnrollID
	
	if (NOT EXISTS(select EnrollId from SAXPushTemplate where EnrollID=@EnrollID and FPID=@FPID))
		begin
			insert into  SAXPushTemplate(DeviceID,EnrollId,Name,FPID,FPTemplate,Size,ReceivedTime)
						         values (@DeviceID,@EnrollID,@Name,@FPID, @FPTemplate,@Size, GETDATE())				
		end
	else
		begin
			update SAXPushTemplate set DeviceIDRecent=@DeviceID,
											FPTemplate=@FPTemplate,
											Size=@Size,
											Name=@Name,
											ReceivedTimeRecent=GETDATE()
		  where EnrollId=@EnrollID and FPID=@FPID	
		end	
end
Go

Create procedure [dbo].[SaveSAXPushCardPassword]
(
@DeviceID varchar(30),
@EnrollID varchar(50),
@Privilege int,
@Name varchar(100),
@Password varchar(50),
@Card varchar(50)
)
as
Begin

			if (NOT EXISTS(select EnrollId from SAXPushTemplate where EnrollID=@EnrollID))
				begin
					insert into  SAXPushTemplate(DeviceID,EnrollId,Name,Privilege,Password,Card,ReceivedTime)
											 values (@DeviceID,@EnrollID,@Name,@Privilege,@Password,@Card, GETDATE())
				end
					
				if(	@Name IS NOT NULL)
					begin
						update SAXPushTemplate set Name=@Name,Privilege=@Privilege, ReceivedTimeRecent=GETDATE(),DeviceIDRecent=@DeviceID where EnrollId=@EnrollID
					end
				else		
					begin
						update S set S.Name=E.Emp_Name from SAXPushTemplate S join EmployeeMaster E on S.EnrollId=E.Emp_Card_No where E.Emp_Card_No=@EnrollID
					end
					
				
				if(	@Password IS NOT NULL)
				begin
					update SAXPushTemplate set Password=@Password,ReceivedTimeRecent=GETDATE(),DeviceIDRecent=@DeviceID where EnrollId=@EnrollID
				end
				
				if(	@Card IS NOT NULL)
				begin
					update SAXPushTemplate set Card=@Card,ReceivedTimeRecent=GETDATE(),DeviceIDRecent=@DeviceID where EnrollId=@EnrollID
				end		
end
Go

Create procedure [dbo].[SaveTemplateUpload]
as 
begin

	declare @DeviceID varchar(30)

	truncate table EnrollmentIDlist
	
	truncate table DeviceIDList

	declare DeviceIDCursor Cursor  for select Distinct(deviceID) from Lastdatesaxpush

	open DeviceIDCursor
	
	Fetch next from DeviceIDCursor into @DeviceID

	while @@FETCH_STATUS=0
		begin

			Insert into EnrollmentIDlist(EnrollID,FPID,FPTemplate,Name,Password,Card,Size,DeviceID,Status)
			select EnrollID,FPID,FPTemplate,Name,Password,Card,Size,@DeviceID,NULL from SAXPushTemplate

			 Fetch next from DeviceIDCursor into @DeviceID
		end
	Close DeviceIDCursor
	
	Deallocate DeviceIDCursor 
	
	insert into DeviceIdList(DeviceID) select Distinct(DeviceID) from Lastdatesaxpush
	update DeviceIdList set Flag=0


end
Go

Create procedure [dbo].[ReadAllData]
(
 @DeviceID varchar(30), 
 @ReadAllDataCommand Varchar(max) output
)
AS
Begin
/*Declare All variable*/	
		
	Declare @Command Varchar(max)
	Declare @DeviceID_DB Varchar(max)
	
	if (EXISTS(select DeviceID from EnrollmentIDList where DeviceID=@DeviceID and Action='ReadAll' and Status is NULL))
		Begin			
			set @ReadAllDataCommand='C:ID2:CHECKCHAR(13)'
			select @ReadAllDataCommand
			Update EnrollmentIDList set Status=1 where DeviceID=@DeviceID  and Action='ReadAll' and Status is NULL
		End
	else	
		Begin
			set @ReadAllDataCommand='NO'
			select @ReadAllDataCommand
		End
End
Go

Create procedure [dbo].[RebootDevice]
(
 @DeviceID varchar(30), 
 @RebootCommand Varchar(max) output
)
AS
Begin
/*Declare All variable*/	
		
	Declare @Command Varchar(max)
	Declare @DeviceID_DB Varchar(max)
	
	if (EXISTS(select DeviceID from EnrollmentIDList where DeviceID=@DeviceID and Action='Reboot' and Status is NULL))
		Begin
			set @RebootCommand='C:ID1:REBOOTCHAR(13)'
			select @RebootCommand
			Update EnrollmentIDList set Status=1 where DeviceID=@DeviceID  and Action='Reboot' and Status is NULL
		End
	else	
		Begin
			set @RebootCommand='NO'
			select @RebootCommand
		End
End
Go

Create procedure [dbo].[UploadSAXPushTemplate]
@DeviceID Varchar(20) ,
@EnrollID Varchar(20) ,
@UploadCommand Varchar(max) output
AS
Begin
	
	/*Declare All variable*/		
	Declare @InsertUserCommand Varchar(max),@Qry Varchar(max),
		
			@EmpName Varchar(200),@Card  Varchar(30),@Group Varchar(10),@TimeZone Varchar(10),
			@PRI Varchar(10) ,@Valid Varchar(10),@Password Varchar(30),@Command  Varchar(max),
			
			@FingerTemplate0  Varchar(max),@FingerTemplate1  Varchar(max),@FingerTemplate2 Varchar(max), @FingerTemplate3  Varchar(max),
			@FingerTemplate4  Varchar(max),@FingerTemplate5  Varchar(max),@FingerTemplate6  Varchar(max),@FingerTemplate7  Varchar(max),
			@FingerTemplate8  Varchar(max),@FingerTemplate9  Varchar(max),
			
			@TemplateCommand0 Varchar(max),@TemplateCommand1 Varchar(max),@TemplateCommand2 Varchar(max),@TemplateCommand3 Varchar(max),
			@TemplateCommand4 Varchar(max),@TemplateCommand5 Varchar(max),@TemplateCommand6 Varchar(max),@TemplateCommand7 Varchar(max),
			@TemplateCommand8 Varchar(max),@TemplateCommand9 Varchar(max),	
					
			@FP0Length Varchar(10),@FP1Length Varchar(10),@FP2Length Varchar(10),@FP3Length Varchar(10),@FP4Length Varchar(10),
			@FP5Length Varchar(10),@FP6Length Varchar(10),@FP7Length Varchar(10),@FP8Length Varchar(10),@FP9Length Varchar(10)
				
		
			/* Initialize All Veriables*/
			SET @FingerTemplate0=null SET @FingerTemplate1=null SET @FingerTemplate2=null SET @FingerTemplate3=null SET @FingerTemplate4=null 
			SET @FingerTemplate5=null SET @FingerTemplate6=null SET @FingerTemplate7=null SET @FingerTemplate8=null SET @FingerTemplate9=null 

			SET @FP0Length=null SET @FP1Length=null SET @FP2Length=null SET @FP3Length=null SET @FP4Length=null 
			SET @FP5Length=null SET @FP6Length=null SET @FP7Length=null SET @FP8Length=null SET @FP9Length=null 

			SET @TemplateCommand0=null SET @TemplateCommand1=null SET @TemplateCommand2=null SET @TemplateCommand3=null SET @TemplateCommand4=null
			SET @TemplateCommand5=null SET @TemplateCommand6=null SET @TemplateCommand7=null SET @TemplateCommand8=null SET @TemplateCommand9=null  

		    SET @Password=null  SET @PRI=0 SET @EmpName=NULL SET @Group=NULL SET @Card=NULL SET @TimeZone=NULL SET @Valid=1
				
			/*Read Card,Password,Name as per index*/
			select @Card =(select top 1 Card from EnrollmentIDList where EnrollId=@EnrollID	and ( Card is not null and Card!=''))
			select @Password =(select top 1  Password from EnrollmentIDList where EnrollId=@EnrollID and (Password is not null	and Password!=''))
			select @EmpName= (select top 1 Name from EnrollmentIDList where EnrollId=@EnrollID and ( Name is not null and Name!=''))
			select @PRI= (select top 1 Privilege from EnrollmentIDList where EnrollId=@EnrollID and ( Privilege is not null and Privilege!=''))
			
			/*query to create user*/
			SET @InsertUserCommand  ='C:ID60:DATA UPDATE USERINFO PIN='+@EnrollID+'  Name='+isnull(@EmpName,'')+'  Pri='+ISNULL(@PRI,'')+'  Passwd='+isnull(@Password,'')+'  Card=['+ISNULL(@Card,'')+']  Grp=01  TZ='+ISNULL(@TimeZone,'')+'CHAR(13)'			
			
			SET @Command= @InsertUserCommand 
			
			/*query to create FP0*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=0))
			begin
				select @FingerTemplate0 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=0
				select @FP0Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=0
				SET @TemplateCommand0   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=0  Size='+isnull(@FP0Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate0,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand0
			end
			
			/*query to create FP1*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=1))
			begin
				select @FingerTemplate1 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=1
				select @FP1Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=1
				SET @TemplateCommand1   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=1  Size='+isnull(@FP1Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate1,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand1
			end
			
			/*query to create FP2*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=2))
			begin			
				select @FingerTemplate2 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=2
				select @FP2Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=2
				SET @TemplateCommand2   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=2  Size='+isnull(@FP2Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate2,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand2
			end
					
			/*query to create FP3*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=3))
			begin
				select @FingerTemplate3 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=3
				select @FP3Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=3
				SET @TemplateCommand3   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=3  Size='+isnull(@FP3Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate3,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand3
			end
						
			/*query to create FP4*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=4))
			begin
				select @FingerTemplate4 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=4
				select @FP4Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=4
				SET @TemplateCommand4   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=4  Size='+isnull(@FP4Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate4,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand4
			end
			
			/*query to create FP5*/			
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=5))
			begin
				select @FingerTemplate5 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=5
				select @FP5Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=5
				SET @TemplateCommand5   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=5  Size='+isnull(@FP5Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate5,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand5
			end
			
			/*query to create FP6*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=6))
			begin
				select @FingerTemplate6 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=6
				select @FP6Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=6
				SET @TemplateCommand6   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=6  Size='+isnull(@FP6Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate6,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand6
			end
			
			/*query to create FP7*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=7))
			begin
				select @FingerTemplate7 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=7
				select @FP7Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=7
				SET @TemplateCommand7   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=7  Size='+isnull(@FP7Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate7,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand7
			end
			
			/*query to create FP8*/			
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=8))
			begin
				select @FingerTemplate8 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=8
				select @FP8Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=8
				SET @TemplateCommand8   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=8  Size='+isnull(@FP8Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate8,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand8
			end
						
			/*query to create FP9*/
			if (EXISTS(select FPTemplate from EnrollmentIDList where EnrollID=@EnrollID and FPID=9))
			begin
				select @FingerTemplate9 =FPTemplate from EnrollmentIDList where EnrollId=@EnrollID	 and FPID=9
				select @FP9Length = Size from EnrollmentIDList where EnrollId=@EnrollID and FPID=9
				SET @TemplateCommand9   ='C:ID100:DATA UPDATE FINGERTMP PIN='+@EnrollID+'  FID=9  Size='+isnull(@FP9Length,'')+'  Valid='+ISNULL(@Valid,'')+'  TMP='+isnull(@FingerTemplate9,'')+'CHAR(13)'
				SET @Command= @Command + @TemplateCommand9
			end
	
			/*say this template has been sent to Device*/
			update EnrollmentIDList set Status=0 where EnrollID=@EnrollID and DeviceID=@DeviceID
  SET @UploadCommand=@Command 
  select @UploadCommand
	
END
Go

Create Proc [dbo].[DeleteEnrollment]
( 
 @DeviceID varchar(30),
 @DeleteCommand varchar(max) output
)
As 
Begin  
	/*Declare All variable*/		
	Declare @Command Varchar(max), @EnrollID varchar(100)
			
	select @EnrollID =(select top 1 EnrollId from EnrollmentIDList where Status is null and DeviceID=@DeviceID  and Action='Reboot'  )
	set @DeleteCommand='C:ID65:DATA DELETE  USERINFO  PIN='+@EnrollID+'CHAR(13)'		
	update EnrollmentIDList set Status=0 where EnrollID=@EnrollID and DeviceID=@DeviceID
	
	select @DeleteCommand
End