
 create FUNCTION [dbo].[getFirstNumeric](
    @s VARCHAR(50)
)  
RETURNS bigint AS 
BEGIN

set @s = substring(@s,patindex('%[0-9]%',@s),len(@s)-patindex('%[0-9]%',@s) + 1) 
if patindex('%[^0-9]%',@s) = 0
    return @s
set @s = substring(@s,1,patindex('%[^0-9]%',@s)-1) 

return cast(@s as bigint )
end