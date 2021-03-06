USE [STW_DB]
GO
/****** Object:  StoredProcedure [dbo].[SP_FILTERDAILYDATAfor5punch]    Script Date: 06/22/2016 18:40:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery2.sql|7|0|C:\Users\admin\AppData\Local\Temp\~vs9AEC.sql
ALTER PROCEDURE [dbo].[SP_FILTERDAILYDATAfor5punch]
 @fromDate date,
   @toDate date,
   @Where varchar(max),
   @employee_count varchar(max)= ''  

AS
BEGIN

Declare
@noofpunchpairs int,
@EmpIDIndex int,
@PunchDateIndex int,
@CountEmpID int,
@CountPDate int,
@v_EmpID varchar(12)='''',
@v_PDate datetime = NULL,
@v_IN datetime = NULL,
@v_OUT datetime = NULL,
@EmployeeName varchar(max)='''',
@CompanyName varchar(max),
@DeptName varchar(max),
@CategoreyName varchar(max),
@EmployeeCategoryname varchar(max) , 
@DesignationName varchar(max),
@Shift_Name varchar(max),
@lateby varchar(max)='''',
@EarlyBy varchar(max)='''',
@tot1 datetime,
@sumtotmin int,
@sumtot1 datetime,
@countrow int,
@initialrow int,
@do1 int,
@do2 int,
@do3 int,
@do4 int,
@do5 int,
@d1 int ,
@d2 int ,
@d3 int,
@d4 int ,
@d5 int ,
@d6 int ,
@d7 int ,
@d8 int ,
@d9 int ,
@d10 int,
@FirstIn dateTime,
@TotalWorkHrsInInt int,
@TotalWorkHrsOutInt int ,
@dov1 varchar(max) ,
@Ip1 datetime ,
@Op1 datetime ,
@Ip2 datetime ,
@Op2 datetime ,
@Ip3 datetime ,
@Op3 datetime ,
@Ip4 datetime ,
@Op4 datetime ,
@Ip5 datetime ,
@Op5 datetime,
   @SqlQuery1 varchar(max),
    @SqlQuery varchar(max)

Declare @TempEmpID table
(
EmpIDIndex int identity(1,1),
EmpID varchar(12)
)
Declare @TempPDate table
(
PDateIndex int identity(1,1),
PDate datetime
)

-- set the variables
set @EmpIDIndex = 1
set @PunchDateIndex = 1

truncate table Dailyreporttable123  
	truncate table temp_process_data  
	truncate table TempEmpID

-- inserting the employee details order by empid in ascending order
	Set @SqlQuery1 = 'Insert into TempEmpID(EmpID)  '+@employee_count
	execute(@SqlQuery1)
select @CountEmpID = COUNT(EmpID) from TempEmpID

insert into @TempPDate(PDate) select distinct (pdate) from process_data where pdate between @fromDate and @toDate
select @CountPdate = COUNT(PDate) from @TempPDate

-- processing for each employee
While (@EmpIDIndex <= @CountEmpID)
begin
set @v_EmpID=''''
select @v_EmpID = EmpID from TempEmpID where EmpIDIndex = @EmpIDIndex
select @EmployeeName =Emp_Name from EmployeeMaster where Emp_Code =@v_EmpID
select @CompanyName = CompanyName from CompanyMaster where CompanyCode = (select Emp_Company from EmployeeMaster where Emp_Code =@v_EmpID)
select @DeptName = DeptName from DeptMaster where DeptCode = (select Emp_Department from EmployeeMaster where Emp_Code =@v_EmpID)
select @CategoreyName = branchName from branchMaster where branchCode = (select Emp_branch from EmployeeMaster where Emp_Code =@v_EmpID)
select @EmployeeCategoryname = EmpCategoryName from EmployeeCategoryMaster where EmpCategoryCode = (select Emp_Employee_Category from EmployeeMaster where Emp_Code =@v_EmpID)

select @DesignationName = DesigName from DesigMaster where DesigCode = (select Emp_Designation from EmployeeMaster where Emp_Code =@v_EmpID)
select @DesignationName = EmpCategoryName from EmployeeCategoryMaster where EmpCategoryCode = (select Emp_Employee_Category from EmployeeMaster where Emp_Code =@v_EmpID)
select @Shift_Name = Shift_Desc from Shift where Shift_Code =(select Emp_Shift_Detail from EmployeeMaster where Emp_Code =@v_EmpID)
set @PunchDateIndex = 1
-- processing for each employee for each date
While (@PunchDateIndex <= @CountPDate)
begin
set @v_PDate=null
set @initialrow = 1
select @v_PDate= pdate from @TempPDate where PDateIndex=@PunchDateIndex
truncate table temp_process_data


insert into temp_process_data(Empid,pdate,In_punch,Out_punch,sum) select top 5 Empid,pdate,In_punch,Out_punch,Sum from process_data where Empid =@v_EmpID and pdate =@v_PDate  -- changes on 19 jul 14
set @Ip1 = null set @Ip2 = null set @Ip3 = null set @Ip4 = null set @Ip5 = null set @Op1 = null set @Op2 = null set @Op3 = null set @Op4 = null set @Op5 = null
set @d1 =null set @d2 =null set @d3 =null set @d4 =null set @d4 =null set @d5 =null set @d6 =null set @d7 =null set @d8 =null set @d9 =null set @d10 =null set @FirstIn=null

	select @FirstIn= In_punch from temp_process_data where id='1'
if(@FirstIn is null)
	select @FirstIn= Out_punch from temp_process_data where id='1'

select  @Ip1 = In_punch,@d1= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),In_punch) from temp_process_data where id='1'
select  @Op1 = Out_punch,@d2= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),Out_punch) from temp_process_data where id='1'
select  @Ip2 = In_punch,@d3= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),In_punch) from temp_process_data where id='2'
select  @Op2 = Out_punch,@d4= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),Out_punch) from temp_process_data where id='2'
select  @Ip3 = In_punch,@d5= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),In_punch) from temp_process_data where id='3'
select  @Op3 = Out_punch,@d6= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),Out_punch) from temp_process_data where id='3'
select  @Ip4 = In_punch,@d7= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),In_punch) from temp_process_data where id='4'
select  @Op4 = Out_punch,@d8= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),Out_punch) from temp_process_data where id='4'
select  @Ip5 = In_punch,@d9= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),In_punch) from temp_process_data where id='5'
select  @Op5 = Out_punch,@d10= DAtediff(minute,Convert(datetime,CONVERT(date,@FirstIn)),Out_punch) from temp_process_data where id='5'

select @TotalWorkHrsInInt = Case when ISNULL(@d2,0)=0 OR ISNULL(@d1,0)=0 then 0 else (ISNULL(@d2,0)-ISNULL(@d1,0)) end +
							Case when ISNULL(@d4,0)=0 OR ISNULL(@d3,0)=0 then 0 else (ISNULL(@d4,0)-ISNULL(@d3,0)) end +
							Case when ISNULL(@d6,0)=0 OR ISNULL(@d5,0)=0 then 0 else (ISNULL(@d6,0)-ISNULL(@d5,0)) end +
							Case when ISNULL(@d8,0)=0 OR ISNULL(@d7,0)=0 then 0 else (ISNULL(@d8,0)-ISNULL(@d7,0)) end +
							Case when ISNULL(@d10,0)=0 OR ISNULL(@d9,0)=0 then 0 else (ISNULL(@d10,0)-ISNULL(@d9,0)) end

select @TotalWorkHrsOutInt = Case when ISNULL(@d3,0)=0 OR ISNULL(@d2,0)=0 then 0 else (ISNULL(@d3,0)-ISNULL(@d2,0)) end +
							Case when ISNULL(@d5,0)=0 OR ISNULL(@d4,0)=0 then 0 else (ISNULL(@d5,0)-ISNULL(@d4,0)) end +
							Case when ISNULL(@d7,0)=0 OR ISNULL(@d6,0)=0 then 0 else (ISNULL(@d7,0)-ISNULL(@d6,0)) end +
							Case when ISNULL(@d9,0)=0 OR ISNULL(@d8,0)=0 then 0 else (ISNULL(@d9,0)-ISNULL(@d8,0)) end

set @lateby = DAteadd(MINUTE,@TotalWorkHrsInInt,0)
set @EarlyBy = DAteadd(MINUTE,@TotalWorkHrsOutInt,0)
insert into Dailyreporttable123(Emp_ID,Emp_Name,pdate,dept_name , cat_name , EmployeeCategory ,  shift_name , In_Punch1,Out_Punch,In_Punch2 , Out_Punch2,In_Punch3, Out_Punch3 , In_Punch4 , Out_Punch4 , In_Punch5 , Out_Punch5 , LateBy , EarlyBy,breatime,breaktimeint) 
values(@v_EmpID,@EmployeeName,@v_PDate,@DeptName ,@CategoreyName,@EmployeeCategoryname , @Shift_Name ,  @Ip1,@Op1,@Ip2,@Op2, @Ip3,@Op3,@Ip4,@Op4,@Ip5,@Op5,@lateby ,
@EarlyBy,CONVERT(varchar(5),DATEADD(minute,@TotalWorkHrsInInt,0),108),CONVERT(varchar(5),DATEADD(minute,@TotalWorkHrsOutInt,0),108))



set @PunchDateIndex= @PunchDateIndex + 1
end
set @EmpIDIndex = @EmpIDIndex + 1

end
truncate table Dailyreporttable123_Reporting
delete from Dailyreporttable123 where in_punch1 is null 
Insert into Dailyreporttable123_Reporting select * from Dailyreporttable123  order by pdate , emp_id
	

end
