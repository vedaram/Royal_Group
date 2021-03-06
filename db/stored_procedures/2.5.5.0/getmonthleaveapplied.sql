

create procedure [dbo].[getmonthleaveapplied]  (@monthnumber int,@empidnum varchar(15),@leavecode varchar(10),@balance decimal(5,1) output)
	as 
	begin
	declare @month int,@year int,@firstday datetime,@lastday datetime ,@count int,@count1 int ,@index int,@index1 int,@empid varchar(15),
	@lfrom datetime,@lto datetime,@hlstatus int,@lapp decimal (5,1),@lapphalf decimal (5,1),@lfull decimal (5,1)
		Declare @leaves table
	(
	lindex int identity(1,1),
	empid varchar(15),
	fdate datetime,
	tdate datetime,
	hl_status int
	)
	 declare @templeave table
(
  tid int identity(1,1),
  empid varchar(10),
  ldates datetime,
  hlflvststatus int
)
set @lapp=0.0
set @lapphalf=1
set @lfull=0.0
	set @month=@monthnumber
	set @year= year(getdate())
	select @firstday=cast(rtrim(@year*10000+@month*100+1) as datetime)
	SELECT @lastday= DATEADD(ms, -3, DATEADD(mm, DATEDIFF(m, 0, @firstday) + 1, 0))
	insert into @leaves (empid,fdate,tdate,hl_status)select empid,startdate,enddate,hl_status from Leave1 where startdate between @firstday and @lastday and empid=@empidnum and leavetype=@leavecode
	--insert into @leaves (empid,fdate,tdate,hl_status)select empid,startdate,enddate,hl_status from Lossonpay where startdate between @firstday and @lastday and empid=@empidnum and leavetype=@leavecode
	set @index=1
	truncate table templeave
	select @count=COUNT(empid)from @leaves
		while(@index<=@count)
		begin
		select @empid=empid,@lfrom=fdate,@lto=tdate,@hlstatus=hl_status from @leaves where lindex=@index
			while DATEDIFF(dd,@lfrom,@lto)>=0
					 begin
					  insert into templeave(empid,ldates,hlflvststatus) values (@empid,@lfrom,@hlstatus)
					  set @lfrom=DATEADD(dd,1,@lfrom)
					end
					set @index=@index+1
		end
		select @lapp=count(*) from templeave where ldates between @firstday and @lastday and hlflvststatus=0
		select @lapphalf=count(*) from templeave where ldates between @firstday and @lastday and hlflvststatus=1
		set @lapphalf=0.5*@lapphalf
		set @lfull=@lapp+@lapphalf
		set @balance=@lfull
		return @balance
	
	end
