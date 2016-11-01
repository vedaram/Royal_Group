ALTER FUNCTION [dbo].[FetchEmployees](@piuserid varchar(100))

RETURNS @employees TABLE (Empid varchar(100),compid varchar(100))
AS
begin
 declare @empid  varchar(max),

  @compid VARCHAR(20),

  @temp   int,

  @temp1 int,

  @temp2 int,

  @temp3  int,

  @temp4  int


 SELECT @temp=COUNT(*) FROM Login WHERE Access = 0 AND Empid = @piuserid 

 IF @temp > 0 

 begin

  INSERT INTO @employees(Empid) SELECT DISTINCT Emp_code  FROM EmployeeMaster 

 end

 else

 begin

  if(@piuserid='admin')

  begin

   INSERT INTO @employees(Empid) SELECT DISTINCT Emp_code  FROM EmployeeMaster

  end

 end

 SELECT @temp1= count(*)   FROM login WHERE Access = 2 AND Empid = @piuserid

 IF @temp1 > 0 

 begin

   INSERT INTO @employees(Empid)(SELECT e.Emp_Code  FROM EmployeeMaster e INNER JOIN login ul  ON ul.EmpID  = e.Emp_Code WHERE UserName = @piuserid)

 end 

 SELECT @temp2=count(*)  FROM login WHERE Access = 1 AND Empid = @piuserid 

 IF  @temp2 > 0

 begin

  DECLARE @Emp_id varchar(max)
   ,@cnt1 int

  
  SELECT @Emp_id= e.Emp_Code   FROM EmployeeMaster e JOIN login ul ON ul.EmpID  = e.Emp_Code WHERE ul.Empid = @piuserid

  SELECT  @cnt1 = count(Emp_Code) FROM EmployeeMaster WHERE Managerid = @Emp_id 

  if @cnt1>0 
  begin

   INSERT INTO @employees(Empid) (SELECT Emp_Code  FROM EmployeeMaster WHERE (Managerid = @Emp_id or Emp_Branch In(Select BranchCode from TbManagerHrBranchMapping Where ManagerID=@piuserid))   )

   SELECT distinct @compid =Emp_Company  FROM EmployeeMaster WHERE Emp_Code = @Emp_id

   insert into @employees (empid,compid) values (@Emp_id,@compid)

  END
  else
  begin  

    SELECT distinct @compid= Emp_company FROM EmployeeMaster WHERE Emp_Code = @Emp_id

    insert into @employees (empid,compid) values (@Emp_id,@compid)       

  end

 end

 SELECT @temp3=count(*)  FROM login WHERE Access = 3 AND Empid = @piuserid 

 /*#Bipin To get list of employees under Branches assigned to HrManager*/
 if @temp3 > 0

 Begin

  INSERT INTO @employees(Empid) (SELECT Emp_Code  FROM EmployeeMaster WHERE  Emp_Branch In(Select BranchCode from TbManagerHrBranchMapping Where ManagerID=@piuserid))
 
 End

 -- To chekc the delegation manager and inlcude the employees

 if exists (Select * from TbAsignDelegation Where DelidationManagerID=@piuserid And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate))
 begin
  INSERT INTO @employees(Empid) select Emp_Code From employeemaster where ManagerId in (Select managerid from TbAsignDelegation Where DelidationManagerID=@piuserid And DeliationStatus=1 and Convert(date,Getdate())>=Convert(date,Fromdate) And Convert(date,Getdate())<=Convert(date,Todate))

 end
 
   return
end