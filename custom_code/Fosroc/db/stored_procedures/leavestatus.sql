
/****** Object:  StoredProcedure [dbo].[leavestatus]    Script Date: 06/25/2016 17:23:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[leavestatus] ( @empid varchar(50) , @pdate datetime , @Status varchar(5) output)  
as

begin  

declare @emp_category varchar(20) , @dayname varchar(50) , @employeeshifttype int , @ShiftCode varchar(10) , @weekoff1 varchar(20) , @weekoff2 varchar(20)
declare @pdateprevious datetime , @pdateprevious2 datetime ,@countprevdaypunch int , @countprev2daypunch int ,@autoshiftcode varchar(50) ,@dayfinal varchar(40)
declare @day varchar(50) , @daytemp varchar(50) , @month int , @year int 
set @emp_category = null  set @dayname = null set @employeeshifttype = null set @ShiftCode = null set @weekoff1 = null set @weekoff2 = null
set  @Status = null   set @pdateprevious = null  set @pdateprevious2 = null set @countprevdaypunch = 0  set @countprev2daypunch = 0 
set @autoshiftcode = null set @daytemp = 'day'
select @emp_category = Emp_Employee_Category from employeemaster where emp_code = @empid
if (@emp_category = '004')
begin
set @Status = 'L'
--break;
end
else
begin
		select @dayname = datepart(dw , @pdate)
		if ( @dayname = 'Sunday')
		begin
		set @Status = 'WO'
		end
		else
		begin
				select @employeeshifttype = chkautoshift from employeemaster where emp_code = @empid 
				if (  @employeeshifttype = 0 or @employeeshifttype = null or @employeeshifttype = '')
				begin
				select @ShiftCode = Emp_Shift_Detail from EmployeeMaster where Emp_Code=@empid
				select @weekoff1=WeeklyOff1, @weekoff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode 
						if ( @dayname = @weekoff1 or @dayname = @weekoff2)
						begin
						set @Status = 'WO'
						end
						else
						begin
						set @Status = 'L'
						end
				end
				else
				begin
				select @pdateprevious = dateadd(day , -1 , @pdate)
				select @pdateprevious2 = dateadd(day , -2 , @pdate)
				select @countprevdaypunch = count(*) from process_data where empid =@empid  and pdate  =  @pdateprevious
				select @countprev2daypunch = count(*) from process_data where empid =@empid  and pdate  =  @pdateprevious2
					if ( @countprevdaypunch = 0 and @countprev2daypunch = 0 )
					begin
					set @Status = 'L'
					end
					if ( @countprevdaypunch = 0 and @countprev2daypunch > 0 ) 
					begin
					select @ShiftCode = shift_code  from masterprocessdailydata where Emp_id=@empid and pdate = @pdateprevious2
					select @weekoff1=WeeklyOff1, @weekoff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode
					select @dayname = datepart(dw , @pdate)
					select @weekoff1=WeeklyOff1, @weekoff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode 
						if ( @dayname = @weekoff1 or @dayname = @weekoff2)
						begin
						set @Status = 'WO'
						end
						else
						begin
						set @Status = 'L'
						end
					end
					if ( @countprevdaypunch > 0 )
					begin
					select @ShiftCode = shift_code  from masterprocessdailydata where Emp_id=@empid and pdate = @pdateprevious
					select @weekoff1=WeeklyOff1, @weekoff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode
					select @dayname = datepart(dw , @pdate)
					select @weekoff1=WeeklyOff1, @weekoff2=WeeklyOff2  from Shift where Shift_Code=@ShiftCode 
						if ( @dayname = @weekoff1 or @dayname = @weekoff2)
						begin
						set @Status = 'WO'
						end
						else
						begin
						set @Status = 'L'
						end
					end
				end
		end
 
end

select @Status
end