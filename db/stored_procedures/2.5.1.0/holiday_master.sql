
GO
/****** Object:  StoredProcedure [dbo].[ManipulateHolidayMaster]    Script Date: 4/11/2016 12:41:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author: <Author,,Name>
-- Create date: <Create Date,,>
-- Description: <Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ManipulateHolidayMaster]
@Mode nvarchar(1),
@holcode nvarchar(max)=null,
@holname nvarchar(max)=null,
@holfrom date=null,
@holto date=null,
@holgrpcode nvarchar(max),
@companycode nvarchar(max),
@origholcode nvarchar(max)=null,
@holtype nvarchar(max)=null

AS
BEGIN
declare @form as datetime,
@to as datetime
IF @Mode='I'
BEGIN

insert into HolidayListDetails values(@holcode , @holname , @holfrom , @holto, @companycode,@holtype)
--while(DATEDIFF(dd,@holfrom,@holto)>=0)
--begin

--insert into holidaylist(hdate,hgroup) values(@holfrom,@holgrpcode)
--set @holfrom=DATEADD(dd,1,@holfrom)
--end
END

IF @Mode='U'
BEGIN
select @form=holfrom,@to=holto from HolidayListDetails where holcode=@origholcode and companycode=@companycode

delete from holidaylist where hgroup in (select holgrpcode from Holidaymaster where holcode=@origholcode and companycode=@companycode) and hdate between @form and @to
update HolidayListDetails set holcode = @holcode , holtype=@holtype, holname = @holname, holfrom = @holfrom , holto = @holto where companycode = @companycode and holcode = @origholcode
update HolidayMaster set  holname = @holname,holyear=year(@form), holfrom = @holfrom , holto = @holto where  holcode = @holcode and year(holfrom)=year(@holfrom)
 if ( @holtype = 'Flexible')
 begin
if exists ( select flexibleholcode from EmpFlexibleHoliday where  flexibleholcode = @holcode )
begin  
update EmpFlexibleHoliday set  holname = @holname,holyear=year(@form), holfrom = @holfrom , holto = @holto where  flexibleholcode = @holcode and year(holfrom)=year(@holfrom)
end
else
begin
truncate table storeempid
declare @empindex int = 0 
declare @totalemp int = 0 
declare @empcode varchar(30)
declare @empbranch varchar(30)
set @empindex = 1 
	insert into storeempid(EID) select Emp_Code from employeemaster where Emp_Branch in
(select branchcode  from branchmaster where holidaycode in (select holgrpcode from holidaymaster where holcode= @holcode)) AND  emp_status in ('1','3')
select  @totalemp = count(EID) from storeempid
while ( @empindex <= @totalemp )
			begin
			select @empcode = EID from StoreEmpID where Id =@empindex
			select @empbranch = emp_branch from employeemaster where emp_code = @empcode 
			select @holgrpcode = holidaycode from branchmaster where branchcode = @empbranch
			insert into EmpFlexibleHoliday values(@empcode ,@holgrpcode , @holcode ,'0',@holfrom,@holto,year(@form),@holname,'Available')
			set @empindex = @empindex + 1 
			end
	
end
end
--if  not exists ( select 

--while(DATEDIFF(dd,@holfrom,@holto)>=0)
--begin

--insert into holidaylist(hdate,hgroup) values(@holfrom,@holgrpcode)
--set @holfrom=DATEADD(dd,1,@holfrom)
--end

		
		declare hol_listcursor cursor
	
		for select distinct holgrpcode from Holidaymaster where holcode=@origholcode and companycode=@companycode
		open hol_listcursor
		fetch next from hol_listcursor into @holgrpcode
	     while(@@FETCH_STATUS=0)
          Begin
		  select @holfrom =holfrom,@holto=holto from Holidaymaster where holcode=@origholcode and companycode=@companycode
			 if ( @holtype != 'Flexible')
			 begin
			while(@holfrom<=@holto)
			begin  
			 
				insert into holidaylist(hdate,hgroup) values(@holfrom,@holgrpcode)  
				set @holfrom=DATEADD(dd,1,@holfrom) 
			end  
			end
		  fetch next from hol_listcursor into @holgrpcode
          End
			Close hol_listcursor
			deallocate hol_listcursor
END

IF @Mode='D'
BEGIN
delete from holidaylist where hgroup=@holgrpcode and hdate between @holfrom and @holto
delete from HolidayListDetails where holcode = @origholcode and companycode = @companycode
END
END


