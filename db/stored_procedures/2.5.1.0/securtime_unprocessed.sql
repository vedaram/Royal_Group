GO
/****** Object:  StoredProcedure [dbo].[securtime_unprocessed]    Script Date: 4/29/2016 8:10:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[securtime_unprocessed]
(
	@piWhereClouse varchar(max)=null
)
as

begin

	declare @cardno varchar(15),@empid varchar(15),@count int,@index int,@cnttrans int,@fromdate datetime,@todate datetime, @employeeids varchar(max)

	select distinct(cardno) from unprocessed_punches

	declare @tempunprocessed_punches as table

	(
		tid int identity(1,1),

		tempcardno varchar(15)
	)

	insert into @tempunprocessed_punches (tempcardno) select distinct(cardno) from unprocessed_punches

	select @count=count(*) from @tempunprocessed_punches

	set @index=1

	while(@index<=@count)

	begin

		set @empid=null

		select @cardno=tempcardno from @tempunprocessed_punches where tid=@index

		select @empid=emp_code from employeemaster where emp_card_no=@cardno

		if(@empid is not null and @empid!='')

		begin

			--select * from unprocessed_punches
			set @employeeids = CONCAT( @empid, ',',@employeeids);

			update unprocessed_punches set empid=@empid where cardno=@cardno

		end

		set @index=@index+1

	end  --end while

	SET @employeeids = Case @employeeids when null then null else (case LEN(@employeeids) when 0 then @employeeids else LEFT(@employeeids, LEN(@employeeids) - 1) end ) end


	Declare @sql varchar(max)

	If (@piWhereClouse!=Null Or @piWhereClouse!='')

	Begin

		Set @sql='insert into Trans_Raw# (EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid,status,status1) select EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid,status,status1 from Unprocessed_Punches '+@piWhereClouse
	
	End

	Else

	begin

		Set @sql='insert into Trans_Raw# (EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid,status,status1) select EmpId,Punch_Time,PunchDate,Verification_Code,CardNo,Deviceid,status,status1 from Unprocessed_Punches where EmpId != '''''
	
	End

	Exec (@sql)

	select @cnttrans=count(*) from trans_raw#

	if(@cnttrans>0)

	begin
		
		Declare @deletesql varchar(max)

		If(@piWhereClouse!=Null And @piWhereClouse!='')
		Begin
			Set @deletesql='delete Unprocessed_Punches '+@piWhereClouse
		End
		Else
		begin
			Set @deletesql='delete Unprocessed_Punches where EmpId != '''''
		End

		Exec (@deletesql)

		select @fromdate=min(punchdate) from trans_raw#

		select @todate=max(punchdate) from trans_raw#

		print @employeeids

		 exec securtimereprocess_Empid @fromdate,@todate,@employeeids

	end



end



