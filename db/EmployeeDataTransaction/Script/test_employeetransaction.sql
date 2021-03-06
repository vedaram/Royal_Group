




create proc [dbo].[test_employeetransaction]
@EmpTabletype Employee_Transaction1 readonly , @empid varchar(100)
--- Here i am assign User_Define_Table_Type to Variable and making it readonly
as
begin
declare @transactionid int , @fromdate date , @todate date , 
@transactiondata varchar(max) , @statusflag varchar(10) , @transactiontype int 

truncate table EmployeeTransactionDummy

SET IDENTITY_INSERT [dbo].[EmployeeTransactionDummy] ON 
Insert into EmployeeTransactionDummy
(
id,TransactionType , FromDate , ToDate , TransactionData , statusflag

)
select
id,TransactionType , FromDate , ToDate , TransactionData  , statusflag
from @EmpTabletype   -- Here i am Select Records from User_Define_Table_Type
SET IDENTITY_INSERT [dbo].[EmployeeTransactionDummy] Off

update EmployeeTransactionDummy set empid = @empid

DECLARE Transactiondata CURSOR for select id , transactiontype  , fromdate , todate , transactiondata, statusflag  from EmployeeTransactionDummy 

 open Transactiondata

 FETCH NEXT FROM Transactiondata into @transactionid, @transactiontype , @fromdate , @todate , @transactiondata  , @statusflag

	WHILE @@FETCH_STATUS = 0
	 begin
				
			 if (upper(@statusflag) = 'I')
			 begin
				
				 --if exists ( select 1 from EmployeeTransactionData   where
				 -- ( (Fromdate  between   convert(date , @fromdate) and convert(date , @todate)) or 
					--(Todate between convert(date , @fromdate) and convert(date , @todate)))  and empid = @empid)
				 -- begin
				--  update EmployeeTransactionData set fromdate = @fromdate , todate = @todate , 
				 -- TransactionData = @transactiondata where id   = @transactionid  and empid = @empid
				  -- update EmployeeTransactionData set fromdate = @fromdate , todate = @todate , 
				  --TransactionData = @transactiondata where fromdate =  @fromdate and todate =  @todate   and empid = @empid and transactiontype = @transactiontype
				  --end
				--else
				  begin
						insert into  EmployeeTransactionData(empid , transactiontype , fromdate , todate , transactiondata , isactive)
						values (@empid,@transactiontype, @fromdate , @todate , @transactiondata ,1 )
				  end
			 end

			 if (upper(@statusflag) = 'M')
			 begin
				 if exists ( select 1 from EmployeeTransactionData   where
				  ( (Fromdate  between   convert(date , @fromdate) and convert(date , @todate)) or 
					(Todate between convert(date , @fromdate) and convert(date , @todate))) and empid = @empid)
				  begin
				 -- update EmployeeTransactionData set fromdate = @fromdate , todate = @todate , 
				 -- TransactionData = @transactiondata where id   = @transactionid  and empid = @empid
				   update EmployeeTransactionData set fromdate = @fromdate , todate = @todate , 
				  TransactionData = @transactiondata where id = @transactionid   and empid = @empid and transactiontype = @transactiontype
				  end
				  else
				  begin
				  insert into  EmployeeTransactionData(empid , transactiontype , fromdate , todate , transactiondata , isactive)
				  values (@empid,@transactiontype, @fromdate , @todate , @transactiondata ,1 )
				  end
			 end

			  if (upper(@statusflag) = 'D')
			 begin
				 
				  update EmployeeTransactionData set isactive = 0  where id   = @transactionid  and empid = @empid
				  
			 end

			 FETCH NEXT FROM Transactiondata into @transactionid, @transactiontype , @fromdate , @todate , @transactiondata  , @statusflag
	 end

	 close Transactiondata

	 DEALLOCATE Transactiondata 
	 update employeetransactiondata set transactiondata = fromdate where transactiontype = 9 
end