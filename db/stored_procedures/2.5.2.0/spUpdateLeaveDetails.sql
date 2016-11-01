USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[SpUpdateLeaveStatus]    Script Date: 11-05-2016 01:25:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--sp_helptext SpUpdateLeaveStatus

ALTER Procedure [dbo].[SpUpdateLeaveStatus]  --MODIFIED ON 18102014 BY VIKASH TO CHECK IF CANCELLED THEN PREVIOUS STATUS SHOULD REFLECT

(
	@piFlag INT,  

	@piLeaveid INT,

	@piactioncomment varchar(max),

	@piImdtmngrflag INT=0
)  

as   

BEGIN  

	declare @from datetime,@chklv varchar(max),@hls int,@to datetime,@empid as varchar(20),@leave_code varchar(max),@count as int,@index as int,@dates as datetime, @chkdecflag int, @getleavebalance INT, @getcheckdecleave int, @GetStatus varchar(max)

	declare @templeave table

	(

		tid int identity(1,1),

		empid varchar(10),

		ldates datetime,

		leavecode varchar(max),

		todate datetime

	)

	select @chkdecflag = chkdecleave from leave1 where leave_id=@piLeaveid

	select @hls=hl_status from Leave1 where Leave_id=@piLeaveid

	select @from=StartDate,@to=EndDate from Leave1 where Leave_id=@piLeaveid 

	select @empid =EMPID,@leave_code=LeaveType from Leave1 where Leave_id=@piLeaveid 

	if @piFlag=3

	begin

		if @chkdecflag is null

		begin

			set @chkdecflag= 1

		end

		else

		begin

			set @chkdecflag = @chkdecflag+1

		end

		Update Leave1 Set Flag = @piFlag, chkdecleave=@chkdecflag, Actioncomment=@piactioncomment,Mflag=@piImdtmngrflag,ApprovalLevel=@piImdtmngrflag where Leave_id = @piLeaveid 

	end

	else

	begin



		Update Leave1 Set Flag = @piFlag, Actioncomment=@piactioncomment,Mflag=@piImdtmngrflag,ApprovalLevel=@piImdtmngrflag where Leave_id = @piLeaveid 

		--Update Lossonpay Set Flag = @piFlag, Actioncomment=@piactioncomment where Leave_id = @piLeaveid

	end 

	if @piFlag=2 

	begin

		set @count =1

		set @index =1



		select @getcheckdecleave = chkdecleave from Leave1 where EMPID=@empid and StartDate=@from and Leave_id=@piLeaveid

		if @getcheckdecleave=1

		begin

			exec spUpsertCancelledLeaveDetails 'I',@empid, @leave_code, @from, @to, '', @empid, '', 0, 0 

		end

		while DATEDIFF(dd,@from,@to)>=0

		begin

			insert into @templeave(empid,ldates,leavecode,todate) values (@empid,@from,@leave_code,@to) 

			set @from=DATEADD(dd,1,@from)

			set @count=@count+1

		end    

		while @index<@count

		begin

			select @dates =ldates,@chklv=leavecode from @templeave where tid=@index

			set @getleavebalance=0

 

			if @chklv='CO'

			begin

				update MASTERPROCESSDAILYDATA set Status='CO' where Emp_ID=@empid and PDate=@dates

			end

			else if @chklv='V'

			Begin

				update MASTERPROCESSDAILYDATA set Status='V' where Emp_ID=@empid and PDate=@dates

			End

			else if @chklv='OD'

			Begin

				update MASTERPROCESSDAILYDATA set Status='OD' where Emp_ID=@empid and PDate=@dates

			End

			else if @hls='1'

			Begin

				--update MASTERPROCESSDAILYDATA set Status='HL' where Emp_ID=@empid and PDate=@dates

				if(@to =DATEDIFF(dd,1,@from))

				begin

					update MASTERPROCESSDAILYDATA set Status ='PHL' where Emp_ID=@empid and PDate=@dates and  Status ='P'

					update MASTERPROCESSDAILYDATA set Status ='AHL' where Emp_ID=@empid and PDate=@dates and  Status ='A'



				end

			End

		else 

		Begin

			--set @GetStatus=null

			--select @GetStatus = status from MASTERPROCESSDAILYDATA where Emp_ID=@empid and PDate=@dates

			--if @GetStatus='A'

			--begin

			update MASTERPROCESSDAILYDATA set Status='L' where Emp_ID=@empid and PDate=@dates

			-- end

			--else

			--begin

			--   update MASTERPROCESSDAILYDATA set Status=@GetStatus where Emp_ID=@empid and PDate=@dates

			--end

		End	

		set @index=@index+1

		end

	end

	else if @piFlag=4--Starting new code added by vikash on 30092014 to check if leave is going to cancel then it should change status in processing table

	begin

		set @count =1

		set @index =1

		declare @empid1 varchar(max)

		while DATEDIFF(dd,@from,@to)>=0

		begin

			insert into @templeave(empid,ldates,leavecode,todate) values (@empid,@from,@leave_code,@to) 

			set @from=DATEADD(dd,1,@from)

			set @count=@count+1

		end    

		while @index<@count

		begin

			select @dates =ldates,@chklv=leavecode, @empid1=empid from @templeave where tid=@index

			IF EXISTS(SELECT 1 FROM MASTERPROCESSDAILYDATA WHERE EMP_ID=@EMPID1 AND PDATE=@DATES AND In_Punch IS NOT NULL AND Out_Punch IS NOT NULL)

			BEGIN

				update MASTERPROCESSDAILYDATA set Status='P' where Emp_ID=@empid1 and PDate=@dates

			END

			ELSE IF EXISTS (SELECT 1 FROM MASTERPROCESSDAILYDATA WHERE EMP_ID=@EMPID1 AND PDATE=@DATES AND In_Punch IS NOT NULL AND Out_Punch IS NULL)

			BEGIN

				update MASTERPROCESSDAILYDATA set Status='MS' where Emp_ID=@empid1 and PDate=@dates

			END

			ELSE 

			BEGIN

				update MASTERPROCESSDAILYDATA set Status='A' where Emp_ID=@empid1 and PDate=@dates

			END

			set @index=@index+1

		end

	end--end code 

END
