USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[spUpsertLeaveotherDetails]    Script Date: 11-05-2016 01:26:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER  PROCEDURE [dbo].[spUpsertLeaveotherDetails]                

(                    

 @piMode as char,    -- I for Insert, U for Update                 

 @piEmpCode as varchar(100),                

 @piLeaveType as varchar(5),                

 @piFromDate as varchar(20),                

 @piToDate as varchar(20),                

 @piCreatedBy as varchar(50),                

 @piModifiedBy as varchar(50),

 @piReason as varchar(120),

 @piTimeStamp as varchar(max)=null,

 @piAddress varchar(max)=null,

 @piLatitude float=null,

 @piLongitude float=null,

 @piSource varchar(max)=null,               

 @piLeaveDetailsID as bigint = 0,

 @piapprovallevel Int =Null              

)                      

as

begin

	Declare @Ld int,@DT AS DATETIME,@DTTO AS DATETIME, @DTFROM AS DATETIME

	SET @DT = CONVERT(datetime,@piFromDate,103) 

	SET @DTFROM = CONVERT(datetime,@piFromDate,103)                

	SET @DTTO = CONVERT(datetime,@piFromDate,103)        


	IF @piMode='I'                

	BEGIN                

		INSERT INTO Leave_Details(Emp_ID, Leave_ID, Leave_From, Leave_To, CreatedDate, CreatedBy,ApprovalLevel)                 
		VALUES( @piEmpCode, @piLeaveType, CONVERT(datetime,@piFromDate,103), CONVERT(datetime,@piToDate,103), getDate(), @piCreatedBy,@piapprovallevel)                

		select @ld = scope_identity()               

	END                

	IF @piMode='U'              

	BEGIN                

		UPDATE Leave_Details SET  Leave_ID=@piLeaveType, Leave_From=@piFromDate, Leave_To=@piToDate,                 

		ModifiedDate=getDate(), ModifiedBy=@piModifiedBy WHERE LeaveDetails_ID=@piLeaveDetailsID          

		select @ld = @piLeaveDetailsID          

	END  

 

	WHILE @DT<=@dtTO                

	BEGIN       

  

	if (@piLeaveType='OD')

	Begin   

	     INSERT INTO ODLEAVE(EMPID,LDATE,STARTDATE,ENDDATE,LEAVETYPE,FLAG,LeaveDetails_id,ReasonForLeave, TimeStamp, SourceAddress, SourceLatitude, SourceLongitude,Source,ApprovalLevel)                

		 --INSERT INTO ODLEAVE(EMPID,LDATE,STARTDATE,ENDDATE,LEAVETYPE,FLAG,LeaveDetails_id,ReasonForLeave, TimeStamp, Address, Latitude, Longitude,Source)                

		 VALUES (@piEmpCode,@DT,@DTFROM,CONVERT(datetime,@piToDate,103),@piLeaveType,1,@Ld,@piReason, @piTimeStamp, @piAddress, @piLatitude, @piLongitude,@piSource,@piapprovallevel)                

	End

	Else

	Begin

			INSERT INTO Leave1(EMPID,LDATE,STARTDATE,ENDDATE,LEAVETYPE,FLAG,LeaveDetails_id,ReasonForLeave,ApprovalLevel)                

			VALUES (@piEmpCode,@DT,@DTFROM,CONVERT(datetime,@piToDate,103),@piLeaveType,1,@Ld,@piReason,@piapprovallevel)   

	End

	SET @DT = DATEADD(d,1,@DT)                

  END 

End
