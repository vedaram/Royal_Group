


 create  FUNCTION [dbo].[getbranchcode](
    @companycode VARCHAR(50)
)  
RETURNS varchar(20) AS 
BEGIN
declare @branch_code varchar(20) , @companycodefirstsplit varchar(20) , @lastbranchcode varchar(20) , @companycodesecondsplit varchar(20) , 
@branchcodesplit varchar(20) , @len int ,  @branchcodefirstletter varchar(1)


select @companycodefirstsplit = SUBSTRING(@companycode ,charindex('-',@companycode)+1, len(@companycode) )
select @companycodesecondsplit = SUBSTRING(@companycode ,charindex('-',@companycode)+1, len(@companycode) )
select @lastbranchcode = max(branchcode ) from branchmaster where companycode = @companycode
select @branchcodesplit  =   SUBSTRING(@lastbranchcode ,charindex('-',@lastbranchcode)+1, len(@lastbranchcode) )
select @branchcodefirstletter = substring(@branchcodesplit , 1 , 1)
		if (@lastbranchcode is null or @lastbranchcode = '')
		begin
			if (@companycodesecondsplit is null or @companycodesecondsplit = '')
			begin
			set @branchcodesplit =  concat(@companycodefirstsplit ,'-', '00' )
			end

			else

			begin
			set @branchcodesplit =  concat(@companycodesecondsplit ,'-', '00' )
			end
		end
		else
				begin
				if ( @branchcodefirstletter  != '0')
				begin
				set @branchcodesplit = concat(SUBSTRING(@lastbranchcode ,1,  CHARINDEX('-', @lastbranchcode)-1),'-' , @branchcodesplit + 1)
				end
				else
				begin
				set @branchcodesplit =  concat(SUBSTRING(@lastbranchcode ,1,  CHARINDEX('-', @lastbranchcode)-1) ,'-', '0' , @branchcodesplit + 1)
				end
		end

return @branchcodesplit
end