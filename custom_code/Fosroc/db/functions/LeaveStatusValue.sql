


create FUNCTION [dbo].LeaveStatusValue (@str NVARCHAR(4000))

RETURNS NVARCHAR(2000)

AS

BEGIN

	DECLARE @retval NVARCHAR(2000),@Levecode Varchar(50)=null,@leaveName varchar(50)=null

	Select @leaveName=status from LeaveMAster where LeaveCode=@str

	

	

		SET @retval = @leaveName



	



	RETURN @retval;

END
