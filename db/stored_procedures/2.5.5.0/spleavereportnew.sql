USE [FosrocSecurTimeweb ]
GO
/****** Object:  StoredProcedure [dbo].[spleavereportnew]    Script Date: 01-07-2016 13:32:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER procedure [dbo].[spleavereportnew]
	as
	begin
	---variable declaration section start
	declare @empid varchar(15),@empname varchar(max),@compcode varchar(10),@compname varchar(200),@desigcode varchar(10),
	@designame varchar(100),@branchcode varchar(10),@branchname varchar(100),@empcatcode varchar(10),@empcatname varchar(100),@leavename varchar(100),
	@totalel decimal(5,1),@eltaken decimal(5,1),@elbalance decimal(5,1),@elexcessdays decimal(5,1),@elreason varchar(max),
	@totalscel decimal(5,1),@sceltaken decimal(5,1),@scelbalance decimal(5,1),@scelexcessdays decimal(5,1),@scelreason varchar(max),
	@totalcl decimal(5,1),@cltaken decimal(5,1),@clbalance decimal(5,1),@clexcessdays decimal(5,1),@clreason varchar(max),
	@totalsl decimal(5,1),@sltaken decimal(5,1),@slbalance decimal(5,1),@slexcessdays decimal(5,1),@slreason varchar(max),
	@index int,@count int,
	@janel decimal (5,1),@febel decimal(5,1),@marchel decimal(5,1),@aprel decimal(5,1),@mayel decimal (5,1),@junel decimal(5,1),@julyel decimal(5,1),
	@augel decimal (5,1),@sepel decimal(5,1),@octel decimal(5,1),@novel decimal(5,1),@decel decimal(5,1),
	
	@janscel decimal (5,1),@febscel decimal(5,1),@marchscel decimal(5,1),@aprscel decimal(5,1),@mayscel decimal (5,1),@junscel decimal(5,1),@julyscel decimal(5,1),
	@augscel decimal (5,1),@sepscel decimal(5,1),@octscel decimal(5,1),@novscel decimal(5,1),@decscel decimal(5,1),
	
	@jansl decimal (5,1),@febsl decimal(5,1),@marchsl decimal(5,2),@aprsl decimal(5,1),@maysl decimal (5,1),@junsl decimal(5,1),@julysl decimal(5,1),
	@augsl decimal (5,1),@sepsl decimal(5,1),@octsl decimal(5,1),@novsl decimal(5,1),@decsl decimal(5,1),
	
	@jancl decimal (5,1),@febcl decimal(5,1),@marchcl decimal(5,1),@aprcl decimal(5,1),@maycl decimal (5,1),@juncl decimal(5,1),@julycl decimal(5,1),
	@augcl decimal (5,1),@sepcl decimal(5,1),@octcl decimal(5,1),@novcl decimal(5,1),@deccl decimal(5,1)
	-- new changes 
	declare @total decimal(5,1),@taken decimal(5,1),@balance decimal(5,1),@excessdays decimal(5,1),@reason varchar(max),
	@jan decimal (5,1),@feb decimal(5,1),@march decimal(5,1),@apr decimal(5,1),@may decimal (5,1),@jun decimal(5,1),@july decimal(5,1),
	@aug decimal (5,1),@sep decimal(5,1),@oct decimal(5,1),@nov decimal(5,1),@dec decimal(5,1)
	-----------------
		begin 
				declare @categoryindex int  , @categorycode varchar(10) , @totalcategorycount int , @leavestatus varchar(10) , @appendstring varchar(20) , @recordcount int 
				declare @leaveindex int  , @leavecode varchar(10) , @totalleavecount int 
			truncate table leavereport
				declare CategoryandLeavecode Cursor  for select distinct EmployeeCategory  from EmployeeCategoryLeave

				open CategoryandLeavecode
	
				Fetch next from CategoryandLeavecode into @categorycode 

				while @@FETCH_STATUS=0
					begin
					
						truncate table storeempid
						insert into storeempid (EID) select emp_code from employeemaster where   Emp_Status=1 and Emp_Employee_Category in ( @categorycode)
						
						select @count=count(*) from storeempid 
						set @index=1
						while(@index<=@count)
						begin
						SET @EMPID=NULL 
						set @empname=null 
						set @compname=null
						set @designame= null
						set @branchname=null
						set @empcatcode=null 
						set @janel =  null
						set @febel =  null
						set @marchel =  null
						set @aprel =  null
						set @mayel =  null
						set @junel =  null
						set @julyel =  null
						set @augel =  null
						set @sepel = null
						set @octel =  null
						set @novel =  null
						set @decel =  null
						set @totalel =null
						set @eltaken =null
						set @elbalance=null
						set @elexcessdays= null
						set @jansl =  null
						set @febsl =  null
						set @marchsl =  null
						set @aprsl =  null
						set @maysl =  null
						set @junsl =  null
						set @julysl =  null
						set @augsl =  null
						set @sepsl = null
						set @octsl =  null
						set @novsl =  null
						set @decsl =  null
						set @totalsl =null
						set @sltaken =null
						set @slbalance=null
						set @slexcessdays= null
						set @jancl =  null
						set @febcl =  null
						set @marchcl =  null
						set @aprcl =  null
						set @maycl =  null
						set @juncl =  null
						set @julycl =  null
						set @augcl =  null
						set @sepcl = null
						set @octcl =  null
						set @novcl =  null
						set @deccl =  null
						set @totalcl =null
						set @cltaken =null
						set @clbalance=null
						set @clexcessdays= null
						set @janscel =  null
						set @febscel =  null
						set @marchscel =  null
						set @aprscel =  null
						set @mayscel =  null
						set @junscel =  null
						set @julyscel =  null
						set @augscel =  null
						set @sepscel = null
						set @octscel =  null
						set @novscel =  null
						set @decscel =  null
						set @totalscel =null
						set @sceltaken =null
						set @scelbalance=null
						set @scelexcessdays= null
						declare Leavecode Cursor  for select distinct LeaveCode  from EmployeeCategoryLeave where EmployeeCategory = @categorycode 
						open Leavecode
						Fetch next from Leavecode  into @leavecode 

						while @@FETCH_STATUS=0
						begin
						select  @leavestatus = leavestatus from EmployeeCategoryLeave where leavecode = @leavecode and EmployeeCategory = @categorycode

						select @empid=EID FROM STOREEMPID WHERE ID=@INDEX
						SELECT @EMPNAME=EMP_name FROM EMPLOYEEmaster WHERE emp_code=@empid --and emp_employee_category in ('001','002')
						select @compname=companyname from companymaster where companycode in (select Emp_Company from employeemaster where emp_code=@empid)
						select @designame=designame from desigmaster where desigcode in (select emp_designation from employeemaster where emp_code=@empid)
						select @branchcode=emp_branch from employeemaster where emp_code=@empid
						select @branchname=branchname from branchmaster where branchcode in (select emp_branch from employeemaster where emp_code=@empid)
						select @empcatcode=emp_employee_category from employeemaster where emp_code=@empid

						
						if ( @leavestatus = 'el')
						begin
						select @totalel =max_leaves from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @eltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @elbalance=leave_balance from employee_leave where emp_code=@empid and leave_code=@leavecode

						if(@eltaken>@totalel)
						begin
						set @elexcessdays= @eltaken-@totalel
						end
			----calculating month wise EL leave balance--------
						exec @janel =  getmonthleaveapplied 1,@empid,@leavecode,0.0
						exec @febel =  getmonthleaveapplied 2,@empid,@leavecode,0.0
						exec @marchel =  getmonthleaveapplied 3,@empid,@leavecode,0.0
						exec @aprel =  getmonthleaveapplied 4,@empid,@leavecode,0.0
						exec @mayel =  getmonthleaveapplied 5,@empid,@leavecode,0.0
						exec @junel =  getmonthleaveapplied 6,@empid,@leavecode,0.0
						exec @julyel =  getmonthleaveapplied 7,@empid,@leavecode,0.0
						exec @augel =  getmonthleaveapplied 8,@empid,@leavecode,0.0
						exec @sepel =  getmonthleaveapplied 9,@empid,@leavecode,0.0
						exec @octel =  getmonthleaveapplied 10,@empid,@leavecode,0.0
						exec @novel =  getmonthleaveapplied 11,@empid,@leavecode,0.0
						exec @decel =  getmonthleaveapplied 12,@empid,@leavecode,0.0
						end

						if ( @leavestatus = 'sl')
						begin
						select @totalsl =max_leaves from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @sltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @slbalance=leave_balance from employee_leave where emp_code=@empid and leave_code=@leavecode

						if(@sltaken>@totalsl)
						begin
						set @slexcessdays= @sltaken-@totalsl
						end
			----calculating month wise SL leave balance--------
						exec @jansl =  getmonthleaveapplied 1,@empid,@leavecode,0.0
						exec @febsl =  getmonthleaveapplied 2,@empid,@leavecode,0.0
						exec @marchsl =  getmonthleaveapplied 3,@empid,@leavecode,0.0
						exec @aprsl =  getmonthleaveapplied 4,@empid,@leavecode,0.0
						exec @maysl =  getmonthleaveapplied 5,@empid,@leavecode,0.0
						exec @junsl =  getmonthleaveapplied 6,@empid,@leavecode,0.0
						exec @julysl =  getmonthleaveapplied 7,@empid,@leavecode,0.0
						exec @augsl =  getmonthleaveapplied 8,@empid,@leavecode,0.0
						exec @sepsl =  getmonthleaveapplied 9,@empid,@leavecode,0.0
						exec @octsl =  getmonthleaveapplied 10,@empid,@leavecode,0.0
						exec @novsl =  getmonthleaveapplied 11,@empid,@leavecode,0.0
						exec @decsl =  getmonthleaveapplied 12,@empid,@leavecode,0.0
						end

						if ( @leavestatus = 'cl')
						begin
						select @totalcl=max_leaves from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @cltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @clbalance=leave_balance from employee_leave where emp_code=@empid and leave_code=@leavecode

						if(@cltaken>@totalcl)
						begin
						set @clexcessdays= @cltaken-@totalcl
						end
						----calculating month wise EL leave balance--------
						exec @jancl=  getmonthleaveapplied 1,@empid,@leavecode,0.0
						exec @febcl=  getmonthleaveapplied 2,@empid,@leavecode,0.0
						exec @marchcl=  getmonthleaveapplied 3,@empid,@leavecode,0.0
						exec @aprcl=  getmonthleaveapplied 4,@empid,@leavecode,0.0
						exec @maycl=  getmonthleaveapplied 5,@empid,@leavecode,0.0
						exec @juncl=  getmonthleaveapplied 6,@empid,@leavecode,0.0
						exec @julycl=  getmonthleaveapplied 7,@empid,@leavecode,0.0
						exec @augcl=  getmonthleaveapplied 8,@empid,@leavecode,0.0
						exec @sepcl=  getmonthleaveapplied 9,@empid,@leavecode,0.0
						exec @octcl=  getmonthleaveapplied 10,@empid,@leavecode,0.0
						exec @novcl=  getmonthleaveapplied 11,@empid,@leavecode,0.0
						exec @deccl=  getmonthleaveapplied 12,@empid,@leavecode,0.0
						end

						if ( @leavestatus = 'SCEL')
						begin
						select @totalscel=max_leaves from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @sceltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code=@leavecode
						select @scelbalance=leave_balance from employee_leave where emp_code=@empid and leave_code=@leavecode

						if(@sceltaken>@totalscel)
						begin
						set @scelexcessdays= @sceltaken-@totalscel
						end
						----calculating month wise EL leave balance--------
						exec @janscel=  getmonthleaveapplied 1,@empid,@leavecode,0.0
						exec @febscel=  getmonthleaveapplied 2,@empid,@leavecode,0.0
						exec @marchscel= getmonthleaveapplied 3,@empid,@leavecode,0.0
						exec @aprscel=  getmonthleaveapplied 4,@empid,@leavecode,0.0
						exec @mayscel=  getmonthleaveapplied 5,@empid,@leavecode,0.0
						exec @junscel=  getmonthleaveapplied 6,@empid,@leavecode,0.0
						exec @julyscel=  getmonthleaveapplied 7,@empid,@leavecode,0.0
						exec @augscel=  getmonthleaveapplied 8,@empid,@leavecode,0.0
						exec @sepscel=  getmonthleaveapplied 9,@empid,@leavecode,0.0
						exec @octscel=  getmonthleaveapplied 10,@empid,@leavecode,0.0
						exec @novscel=  getmonthleaveapplied 11,@empid,@leavecode,0.0
						exec @decscel=  getmonthleaveapplied 12,@empid,@leavecode,0.0
						end
				select @recordcount = count (* )  from leavereport where empid = @empid 
				if ( @recordcount > 0 )

				begin
				UPDATE leavereport
					SET 
      [elavailed] = @eltaken , 
      [elentitlement] = @totalel
      ,[elbalance] = @elbalance
      ,[excessdaysel] = @elexcessdays
      ,[elreason] = @elreason
      ,[scelavailed] = @sceltaken
      ,[scelentitlemant] = @totalscel
      ,[scelbalance] = @scelbalance
      ,[excessdayscel] = @scelexcessdays
      ,[scelreason] = @scelreason
      ,[clavailed] = @cltaken
      ,[clentitlement] = @totalcl
      ,[clbalance] = @clbalance
      ,[excessdayscl] = @clexcessdays
      ,[clreason] = @clreason
      ,[slavailed] = @sltaken
      ,[slentitlement] = @totalsl
      ,[slbalance] = @slbalance
      ,[excessdayssl] = @slexcessdays
      ,[slreason] = @slreason
      ,[janel] = @janel
      ,[febel] = @febel
      ,[marchel] = @marchel
      ,[aprel] = @aprel
      ,[mayel] = @mayel
      ,[junel] = @junel
      ,[julyel] = @julyel
      ,[augel] = @augel
      ,[sepel] = @sepel
      ,[octel] = @octel
      ,[novel] = @novel
      ,[decel] = @decel
      ,[janscel] = @janscel
      ,[febscel] = @febscel
      ,[marchscel] = @marchscel
      ,[aprscel] = @aprscel
      ,[mayscel] = @mayscel
      ,[junscel] = @junscel
      ,[julyscel] = @julyscel
      ,[augscel] = @augscel
      ,[sepscel] = @sepscel
      ,[octscel] = @octscel
      ,[novscel] = @novscel
      ,[decscel] = @decscel
      ,[jansl] = @jansl
      ,[febsl] = @febsl
      ,[marchsl] =  @marchsl
      ,[aprsl] = @aprsl
      ,[maysl] = @maysl
      ,[junsl] = @junsl
      ,[julysl] = @julysl
      ,[augsl] = @augsl
      ,[sepsl] = @sepsl
      ,[octsl] = @octsl
      ,[novsl] = @novsl
      ,[decsl] = @decsl
      ,[jancl] = @jancl
      ,[febcl] = @febcl
      ,[marchcl] = @marchcl
      ,[aprcl] = @aprcl
      ,[maycl] = @maycl
      ,[juncl] = @juncl
      ,[julycl] = @julycl
      ,[augcl] = @augcl
      ,[sepcl] = @sepcl
      ,[octcl] = @octcl
      ,[novcl] = @novcl
      ,[deccl] = @deccl
	
			 WHERE  empid = @empid		 
				end
				else
				begin  
				INSERT INTO [leavereport]
           ([empid]
           ,[empname]
           ,[company]
           ,[designation]
           ,[branch]
           ,[empcategory]
           ,[elavailed]
           ,[elentitlement]
           ,[elbalance]
           ,[excessdaysel]
           ,[elreason]
           ,[scelavailed]
           ,[scelentitlemant]
           ,[scelbalance]
           ,[excessdayscel]
           ,[scelreason]
           ,[clavailed]
           ,[clentitlement]
           ,[clbalance]
           ,[excessdayscl]
           ,[clreason]
           ,[slavailed]
           ,[slentitlement]
           ,[slbalance]
           ,[excessdayssl]
           ,[slreason]
           ,[janel]
           ,[febel]
           ,[marchel]
           ,[aprel]
           ,[mayel]
           ,[junel]
           ,[julyel]
           ,[augel]
           ,[sepel]
           ,[octel]
           ,[novel]
           ,[decel]
           ,[janscel]
           ,[febscel]
           ,[marchscel]
           ,[aprscel]
           ,[mayscel]
           ,[junscel]
           ,[julyscel]
           ,[augscel]
           ,[sepscel]
           ,[octscel]
           ,[novscel]
           ,[decscel]
           ,[jansl]
           ,[febsl]
           ,[marchsl]
           ,[aprsl]
           ,[maysl]
           ,[junsl]
           ,[julysl]
           ,[augsl]
           ,[sepsl]
           ,[octsl]
           ,[novsl]
           ,[decsl]
           ,[jancl]
           ,[febcl]
           ,[marchcl]
           ,[aprcl]
           ,[maycl]
           ,[juncl]
           ,[julycl]
           ,[augcl]
           ,[sepcl]
           ,[octcl]
           ,[novcl]
           ,[deccl])
     VALUES  (@empid,@empname,@compname,@designame,@branchname,@empcatcode,@eltaken,@totalel,@elbalance,@elexcessdays,@elreason,
     @sceltaken,@totalscel,@scelbalance,@scelexcessdays,@scelreason,@cltaken,@totalcl,@clbalance,@clexcessdays,@clreason,
     @sltaken,@totalsl,@slbalance,@slexcessdays,@slreason,@janel,@febel,@marchel,@aprel,@mayel,@junel,@julyel,@augel,@sepel,@octel,@novel,@decel,
     @janscel,@febscel,@marchscel,@aprscel,@mayscel,@junscel,@julyscel,@augscel,@sepscel,@octscel,@novscel,@decscel,
     @jansl,@febsl,@marchsl,@aprsl,@maysl,@junsl,@julysl,@augsl,@sepsl,@octsl,@novsl,@decsl,
     @jancl,@febcl,@marchcl,@aprcl,@maycl,@juncl,@julycl,@augcl,@sepcl,@octcl,@novcl,@deccl)
	 end
			 Fetch next from Leavecode  into @leavecode 
					set @leavestatus= null
					
				end		
				Close Leavecode		
				Deallocate Leavecode
						SET @INDEX=@INDEX+1
						end
					
					 
				
					Fetch next from CategoryandLeavecode into @categorycode 
					end 
				Close CategoryandLeavecode		
				Deallocate CategoryandLeavecode 
		end
				--Declare @distinctcategory table
				--(
				--categoryid int identity(1,1),
				--categorycode varchar(12)
				--)

				--Declare @distinctleave table
				--(
				--id int identity(1,1),
				--categorycode varchar(12),
				--leavecode varchar(12)
				--)
				
				--insert into   @distinctcategory(categorycode ) select  distinct EmployeeCategory  from EmployeeCategoryLeave
				--set  @categoryindex  = 1 
				--select @totalcategorycount = count(*) from @distinctcategory 
				--while ( @categoryindex <= @totalcategorycount)
				--begin
				--select @categorycode = categorycode from @distinctcategory where categoryid = @categoryindex 

				--insert into @distinctleave (categorycode ,leavecode  ) select EmployeeCategory , leavecode from EmployeeCategoryLeave where EmployeeCategory = @categorycode

				--set @leaveindex = 1 
				--select @totalleavecount = count(*) from @distinctleave 
				--		while (@leaveindex < = @totalleavecount) 
				--		begin
				--		select @leavecode = leavecode from @distinctleave where id =  @leaveindex  
				--		set @leaveindex= @leaveindex + 1 
				--		print 1 
				--		end
				--print 1 
				--delete from    @distinctleave
				--set @categoryindex = @categoryindex + 1 
				--end




	----
	----variable section end
	--truncate table leavereport
	----truncate table storeempid
	----select * from storeempid
	----select * from employee_leave
	--insert into storeempid (EID) select emp_code from employeemaster where   Emp_Status=1 and Emp_Employee_Category in ( select distinct EmployeeCategory from EmployeeCategoryLeave)
	----insert into storeempid values('S001')
	--select @count=count(*) from storeempid 
	--set @index=1
	--	while(@index<=@count)
	--	begin
	--	SET @EMPID=NULL 
	--	set @empname=null 
	--	set @compname=null
	--	set @designame= null
	--    set @branchname=null
	--    set @empcatcode=null 
		
	--	select @empid=EID FROM STOREEMPID WHERE ID=@INDEX
	--	SELECT @EMPNAME=EMP_name FROM EMPLOYEEmaster WHERE emp_code=@empid --and emp_employee_category in ('001','002')
	--	select @compname=companyname from companymaster where companycode in (select Emp_Company from employeemaster where emp_code=@empid)
	--	select @designame=designame from desigmaster where desigcode in (select emp_designation from employeemaster where emp_code=@empid)
	--	select @branchcode=emp_branch from employeemaster where emp_code=@empid
	--	select @branchname=branchname from branchmaster where branchcode in (select emp_branch from employeemaster where emp_code=@empid)
	--	select @empcatcode=emp_employee_category from employeemaster where emp_code=@empid
	--		if(@empcatcode='001')   -----permanent employee start
	--		begin
	--		select @totalel =max_leaves from employee_leave where emp_code=@empid and leave_code='L01'
	--		select @eltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L01'
	--		select @elbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L01'
	--		if(@eltaken>@totalel)
	--		begin
	--		set @elexcessdays= @eltaken-@totalel
	--		end
	--		----calculating month wise EL leave balance--------
	--		exec @janel =  getmonthleaveapplied 1,@empid,'L01',0.0
	--		exec @febel =  getmonthleaveapplied 2,@empid,'L01',0.0
	--		exec @marchel =  getmonthleaveapplied 3,@empid,'L01',0.0
	--		exec @aprel =  getmonthleaveapplied 4,@empid,'L01',0.0
	--		exec @mayel =  getmonthleaveapplied 5,@empid,'L01',0.0
	--		exec @junel =  getmonthleaveapplied 6,@empid,'L01',0.0
	--		exec @julyel =  getmonthleaveapplied 7,@empid,'L01',0.0
	--		exec @augel =  getmonthleaveapplied 8,@empid,'L01',0.0
	--		exec @sepel =  getmonthleaveapplied 9,@empid,'L01',0.0
	--		exec @octel =  getmonthleaveapplied 10,@empid,'L01',0.0
	--		exec @novel =  getmonthleaveapplied 11,@empid,'L01',0.0
	--		exec @decel =  getmonthleaveapplied 12,@empid,'L01',0.0
	--		----end el balance-----------
			
	--		select @totalscel =max_leaves from employee_leave where emp_code=@empid and leave_code='L02'
	--		select @sceltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L02'
	--		select @scelbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L02'
	--		 if(@sceltaken>@totalscel)
	--		begin
	--		set @scelexcessdays= @sceltaken-@totalscel
	--		end
	--		-------------staff casula cumsick leave----------
	--		exec @janscel =  getmonthleaveapplied 1,@empid,'L02',0.0
	--		exec @febscel =  getmonthleaveapplied 2,@empid,'L02',0.0
	--		exec @marchscel =  getmonthleaveapplied 3,@empid,'L02',0.0
	--		exec @aprscel =  getmonthleaveapplied 4,@empid,'L02',0.0
	--		exec @mayscel =  getmonthleaveapplied 5,@empid,'L02',0.0
	--		exec @junscel =  getmonthleaveapplied 6,@empid,'L02',0.0
	--		exec @julyscel =  getmonthleaveapplied 7,@empid,'L02',0.0
	--		exec @augscel =  getmonthleaveapplied 8,@empid,'L02',0.0
	--		exec @sepscel =  getmonthleaveapplied 9,@empid,'L02',0.0
	--		exec @octscel =  getmonthleaveapplied 10,@empid,'L02',0.0
	--		exec @novscel =  getmonthleaveapplied 11,@empid,'L02',0.0
	--		exec @decscel =  getmonthleaveapplied 12,@empid,'L02',0.0
			
	--		---- end----------------------
			
	--		if(@sceltaken>@totalscel)
	--		begin
	--		set @scelexcessdays=@sceltaken-@totalscel
	--		end
			
	--		end      --- permanent employee
			
	--			else	if(@empcatcode='002' and (@branchcode='004' or @branchcode='005'))   --------------blr fac workmen
	--				begin
	--				select @totalel =max_leaves from employee_leave where emp_code=@empid and leave_code='L03'
	--			    select @eltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L03'
	--			    select @elbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L03'
	--							if(@eltaken>@totalel)
	--					begin
	--					set @elexcessdays= @eltaken-@totalel
	--					end
				    
	--			    --------el forworker----------------
	--			    exec  getmonthleaveapplied 1,@empid,'L03',@janel output
	--				exec  getmonthleaveapplied 2,@empid,'L03',@febel output
	--				exec getmonthleaveapplied 3,@empid,'L03',@marchel output
	--				exec getmonthleaveapplied 4,@empid,'L03',@aprel output
	--				exec getmonthleaveapplied 5,@empid,'L03',@mayel output
	--				exec getmonthleaveapplied 6,@empid,'L03',@junel output
	--				exec getmonthleaveapplied 7,@empid,'L03',@julyel output
	--				exec getmonthleaveapplied 8,@empid,'L03',@augel output
	--				exec getmonthleaveapplied 9,@empid,'L03',@sepel output
	--				exec getmonthleaveapplied 10,@empid,'L03',@octel output
	--				exec getmonthleaveapplied 11,@empid,'L03',@novel output
	--				exec getmonthleaveapplied 12,@empid,'L03',@decel output
	--			    ----------end
									
				    
	--			    select @totalsl =max_leaves from employee_leave where emp_code=@empid and leave_code='L04'
	--			    select @sltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L04'
	--			    select @slbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L04'
	--			     if(@sltaken>@totalsl)
	--		begin
	--		set @slexcessdays= @sltaken-@totalsl
	--		end
				    
	--			     --------sl forworker----------------
	--			    exec getmonthleaveapplied 1,@empid,'L04',@jansl output
	--				exec getmonthleaveapplied 2,@empid,'L04',@febsl output
	--				exec getmonthleaveapplied 3,@empid,'L04',@marchsl output
	--				exec getmonthleaveapplied 4,@empid,'L04',@aprsl output
	--				exec getmonthleaveapplied 5,@empid,'L04',@maysl output
	--				exec getmonthleaveapplied 6,@empid,'L04',@junsl output
	--				exec getmonthleaveapplied 7,@empid,'L04',@julysl output
	--				exec getmonthleaveapplied 8,@empid,'L04',@augsl output
	--				exec getmonthleaveapplied 9,@empid,'L04',@sepsl output
	--				exec getmonthleaveapplied 10,@empid,'L04',@octsl output
	--				exec getmonthleaveapplied 11,@empid,'L04',@novsl output
	--				exec getmonthleaveapplied 12,@empid,'L04',@decsl output
	--			    ----------end
				    
				                  
				    
	--			    select @totalcl =max_leaves from employee_leave where emp_code=@empid and leave_code='L05'
	--			    select @cltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L05'
	--			    select @clbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L05'
	--			        if(@cltaken>@totalcl)
	--		begin
	--		set @clexcessdays= @cltaken-@totalcl
	--		end
				    
	--			      --------cl forworker----------------
	--			    exec getmonthleaveapplied 1,@empid,'L05',@jancl output
	--				exec getmonthleaveapplied 2,@empid,'L05',@febcl output
	--				exec getmonthleaveapplied 3,@empid,'L05',@marchcl output
	--				exec getmonthleaveapplied 4,@empid,'L05',@aprcl output
	--				exec getmonthleaveapplied 5,@empid,'L05',@maycl output
	--				exec getmonthleaveapplied 6,@empid,'L05',@juncl output
	--				exec getmonthleaveapplied 7,@empid,'L05',@julycl output
	--				exec getmonthleaveapplied 8,@empid,'L05',@augcl output
	--				exec getmonthleaveapplied 9,@empid,'L05',@sepcl output
	--				exec getmonthleaveapplied 10,@empid,'L05',@octcl output
	--				exec getmonthleaveapplied 11,@empid,'L05',@novcl output
	--				exec getmonthleaveapplied 12,@empid,'L05',@deccl output
	--			    ----------end
	--				end                  ----------blr fac workmen end
					
	--				else 	if(@empcatcode='002' and @branchcode='032')           ----RDP start
	--					begin
	--					select @totalel =max_leaves from employee_leave where emp_code=@empid and leave_code='L06'
	--					select @eltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L06'
	--					select @elbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L06'
	--					    if(@eltaken>@totalel)
	--		begin
	--		set @elexcessdays= @eltaken-@totalel
	--		end
	--					----el for ----------
	--				     exec getmonthleaveapplied 1,@empid,'L06',@janel output
	--					exec getmonthleaveapplied 2,@empid,'L06',@febel output
	--					exec getmonthleaveapplied 3,@empid,'L06',@marchel output
	--					exec getmonthleaveapplied 4,@empid,'L06',@aprel output
	--					exec getmonthleaveapplied 5,@empid,'L06',@mayel output
	--					exec getmonthleaveapplied 6,@empid,'L06',@junel output
	--					exec getmonthleaveapplied 7,@empid,'L06',@julyel output
	--					exec getmonthleaveapplied 8,@empid,'L06',@augel output
	--					exec getmonthleaveapplied 9,@empid,'L06',@sepel output
	--					exec getmonthleaveapplied 10,@empid,'L06',@octel output
	--					exec getmonthleaveapplied 11,@empid,'L06',@novel output
	--					exec getmonthleaveapplied 12,@empid,'L06',@decel output
	--			------------------------end
					    
	--					select @totalsl =max_leaves from employee_leave where emp_code=@empid and leave_code='L07'
	--					select @sltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L07'
	--					select @slbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L07'
	--					    if(@sltaken>@totalsl)
	--		begin
	--		set @slexcessdays= @sltaken-@totalsl
	--		end
						
	--					  --------sl forworker----------------
	--			    exec getmonthleaveapplied 1,@empid,'L07',@jansl output
	--				exec getmonthleaveapplied 2,@empid,'L07',@febsl output
	--				exec getmonthleaveapplied 3,@empid,'L07',@marchsl output
	--				exec getmonthleaveapplied 4,@empid,'L07',@aprsl output
	--				exec getmonthleaveapplied 5,@empid,'L07',@maysl output
	--				exec getmonthleaveapplied 6,@empid,'L07',@junsl output
	--				exec getmonthleaveapplied 7,@empid,'L07',@julysl output
	--				exec getmonthleaveapplied 8,@empid,'L07',@augsl output
	--				exec getmonthleaveapplied 9,@empid,'L07',@sepsl output
	--				exec getmonthleaveapplied 10,@empid,'L07',@octsl output
	--				exec getmonthleaveapplied 11,@empid,'L07',@novsl output
	--				exec getmonthleaveapplied 12,@empid,'L07',@decsl  output
	--			    ----------end
					    
	--					select @totalcl =max_leaves from employee_leave where emp_code=@empid and leave_code='L08'
	--					select @cltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L08'
	--					select @clbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L08'
	--					    if(@cltaken>@totalcl)
	--		begin
	--		set @clexcessdays= @cltaken-@totalcl
	--		end
						
	--					    --------cl forworker----------------
	--			    exec getmonthleaveapplied 1,@empid,'L08',@jancl output
	--				exec getmonthleaveapplied 2,@empid,'L08',@febcl output
	--				exec getmonthleaveapplied 3,@empid,'L08',@marchcl output
	--				exec getmonthleaveapplied 4,@empid,'L08',@aprcl output
	--				exec getmonthleaveapplied 5,@empid,'L08',@maycl output
	--				exec getmonthleaveapplied 6,@empid,'L08',@juncl output
	--				exec getmonthleaveapplied 7,@empid,'L08',@julycl output
	--				exec getmonthleaveapplied 8,@empid,'L08',@augcl output
	--				exec getmonthleaveapplied 9,@empid,'L08',@sepcl output
	--				exec getmonthleaveapplied 10,@empid,'L08',@octcl output
	--				exec getmonthleaveapplied 11,@empid,'L08',@novcl output
	--				exec getmonthleaveapplied 12,@empid,'L08',@deccl output
	--			    ----------end
	--					end       --RDP end-----
						
	--				else 	if(@empcatcode='002' and @branchcode='002')   -------ANk fac
	--					begin
	--					select @totalel =max_leaves from employee_leave where emp_code=@empid and leave_code='L09'
	--					select @eltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L09'
	--					select @elbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L09'
	--					    if(@eltaken>@totalel)
	--		begin
	--		set @elexcessdays= @eltaken-@totalel
	--		end
	--					----el for ----------
	--					declare @testel decimal(5,1)
	--				    exec getmonthleaveapplied 1,@empid,'L09',@janel output
	--					exec getmonthleaveapplied 2,@empid,'L09',@febel output
	--					exec getmonthleaveapplied 3,@empid,'L09',@marchel output
	--					exec getmonthleaveapplied 4,@empid,'L09',@aprel output
	--					exec getmonthleaveapplied 5,@empid,'L09',@mayel output
	--					exec getmonthleaveapplied 6,@empid,'L09',@junel output
	--					exec getmonthleaveapplied 7,@empid,'L09',@julyel output
	--					exec getmonthleaveapplied 8,@empid,'L09',@augel output
	--					exec getmonthleaveapplied 9,@empid,'L09',@sepel output
	--					exec getmonthleaveapplied 10,@empid,'L09',@octel output
	--					exec getmonthleaveapplied 11,@empid,'L09',@novel output
	--					exec getmonthleaveapplied 12,@empid,'L09',@decel output
	--			------------------------end
					    
	--					select @totalsl =max_leaves from employee_leave where emp_code=@empid and leave_code='L10'
	--					select @sltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L10'
	--					select @slbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L10'
	--					    if(@sltaken>@totalsl)
	--		begin
	--		set @slexcessdays= @sltaken-@totalsl
	--		end
						
	--					  --------sl forworker----------------
	--			    exec getmonthleaveapplied 1,@empid,'L10',@jansl output
	--				exec getmonthleaveapplied 2,@empid,'L10',@febsl output
	--				exec getmonthleaveapplied 3,@empid,'L10',@marchsl output
	--				exec getmonthleaveapplied 4,@empid,'L10',@aprsl output
	--				exec getmonthleaveapplied 5,@empid,'L10',@maysl output
	--				exec getmonthleaveapplied 6,@empid,'L10',@junsl output
	--				exec getmonthleaveapplied 7,@empid,'L10',@julysl output
	--				exec getmonthleaveapplied 8,@empid,'L10',@augsl output
	--				exec getmonthleaveapplied 9,@empid,'L10',@sepsl output
	--				exec getmonthleaveapplied 10,@empid,'L10',@octsl output
	--				exec getmonthleaveapplied 11,@empid,'L10',@novsl output
	--				exec getmonthleaveapplied 12,@empid,'L10',@decsl output
	--			    ----------end
					    
	--					select @totalcl =max_leaves from employee_leave where emp_code=@empid and leave_code='L11'
	--					select @cltaken =leaves_applied from employee_leave where emp_code=@empid and leave_code='L11'
	--					select @clbalance=leave_balance from employee_leave where emp_code=@empid and leave_code='L11'
						
	--					    --------cl forworker----------------
	--			    exec getmonthleaveapplied 1,@empid,'L11',@jansl output
	--				exec getmonthleaveapplied 2,@empid,'L11',@febcl output
	--				exec getmonthleaveapplied 3,@empid,'L11',@marchcl output
	--				exec getmonthleaveapplied 4,@empid,'L11',@aprcl output
	--				exec getmonthleaveapplied 5,@empid,'L11',@maycl output
	--				exec getmonthleaveapplied 6,@empid,'L11',@juncl output
	--				exec getmonthleaveapplied 7,@empid,'L11',@julycl output
	--				exec getmonthleaveapplied 8,@empid,'L11',@augcl output
	--				exec getmonthleaveapplied 9,@empid,'L11',@sepcl output
	--				exec getmonthleaveapplied 10,@empid,'L11',@octcl output
	--				exec getmonthleaveapplied 11,@empid,'L11',@novcl output
	--				exec getmonthleaveapplied 12,@empid,'L11',@deccl output
	--			    ----------end
	--					end
						
						
	--					           if(@sltaken>@totalsl)
	--						       begin
	--						       set @slexcessdays=@sltaken-@totalsl
	--						       end
	--						        if(@cltaken>@totalcl)
	--						       begin
	--						       set @clexcessdays=@cltaken-@totalcl
	--						       end
	--						       if(@eltaken>@totalel)
	--						       begin
	--						       set @elexcessdays=@eltaken-@totalel
	--						       end
	--						 INSERT INTO [leavereport]
 --          ([empid]
 --          ,[empname]
 --          ,[company]
 --          ,[designation]
 --          ,[branch]
 --          ,[empcategory]
 --          ,[elavailed]
 --          ,[elentitlement]
 --          ,[elbalance]
 --          ,[excessdaysel]
 --          ,[elreason]
 --          ,[scelavailed]
 --          ,[scelentitlemant]
 --          ,[scelbalance]
 --          ,[excessdayscel]
 --          ,[scelreason]
 --          ,[clavailed]
 --          ,[clentitlement]
 --          ,[clbalance]
 --          ,[excessdayscl]
 --          ,[clreason]
 --          ,[slavailed]
 --          ,[slentitlement]
 --          ,[slbalance]
 --          ,[excessdayssl]
 --          ,[slreason]
 --          ,[janel]
 --          ,[febel]
 --          ,[marchel]
 --          ,[aprel]
 --          ,[mayel]
 --          ,[junel]
 --          ,[julyel]
 --          ,[augel]
 --          ,[sepel]
 --          ,[octel]
 --          ,[novel]
 --          ,[decel]
 --          ,[janscel]
 --          ,[febscel]
 --          ,[marchscel]
 --          ,[aprscel]
 --          ,[mayscel]
 --          ,[junscel]
 --          ,[julyscel]
 --          ,[augscel]
 --          ,[sepscel]
 --          ,[octscel]
 --          ,[novscel]
 --          ,[decscel]
 --          ,[jansl]
 --          ,[febsl]
 --          ,[marchsl]
 --          ,[aprsl]
 --          ,[maysl]
 --          ,[junsl]
 --          ,[julysl]
 --          ,[augsl]
 --          ,[sepsl]
 --          ,[octsl]
 --          ,[novsl]
 --          ,[decsl]
 --          ,[jancl]
 --          ,[febcl]
 --          ,[marchcl]
 --          ,[aprcl]
 --          ,[maycl]
 --          ,[juncl]
 --          ,[julycl]
 --          ,[augcl]
 --          ,[sepcl]
 --          ,[octcl]
 --          ,[novcl]
 --          ,[deccl])
 --    VALUES  (@empid,@empname,@compname,@designame,@branchname,@empcatcode,@eltaken,@totalel,@elbalance,@elexcessdays,@elreason,
 --    @sceltaken,@totalscel,@scelbalance,@scelexcessdays,@scelreason,@cltaken,@totalcl,@clbalance,@clexcessdays,@clreason,
 --    @sltaken,@totalsl,@slbalance,@slexcessdays,@slreason,@janel,@febel,@marchel,@aprel,@mayel,@junel,@julyel,@augel,@sepel,@octel,@novel,@decel,
 --    @janscel,@febscel,@marchscel,@aprscel,@mayscel,@junscel,@julyscel,@augscel,@sepscel,@octscel,@novscel,@decscel,
 --    @jansl,@febsl,@marchsl,@aprsl,@maysl,@junsl,@julysl,@augsl,@sepsl,@octsl,@novsl,@decsl,
 --    @jancl,@febcl,@marchcl,@aprcl,@maycl,@juncl,@julycl,@augcl,@sepcl,@octcl,@novcl,@deccl)
	--	SET @INDEX=@INDEX+1
	--	end

	end
