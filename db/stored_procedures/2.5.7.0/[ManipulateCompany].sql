
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ManipulateCompany]
(
	@Mode varchar(max),
	@CompanyCode varchar(max),	
	@CompanyName varchar(max),
	@Address varchar(max),
	@Phone varchar(max),
	@Fax varchar(max),
	@Email varchar(max),
	@Pin varchar(max),
	@Url varchar(max)

)
as	
BEGIN
	
	IF @Mode='I'
	Begin
		
		insert into CompanyMaster(CompanyCode,CompanyName,[Address],Phone,Fax,Email,PIN,URL,LeaveCreditDay,LeaveCreditmonth)
		values(@CompanyCode,@CompanyName,@Address,@Phone,@Fax,@Email,@Pin,@Url,1,1)
		
	End
	
	IF @Mode='U'
	Begin
	
		update CompanyMaster set CompanyName=@CompanyName,[Address]=@Address,Phone=@Phone,Fax=@Fax,Email=@Email,
		PIN=@Pin,URL=@Url where CompanyCode=@CompanyCode
	
	End
	
	IF @Mode='D'
	Begin
	
		delete from CompanyMaster where CompanyCode=@CompanyCode
	
	End
END


