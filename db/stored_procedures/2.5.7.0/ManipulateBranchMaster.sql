
ALTER PROCEDURE [dbo].[ManipulateBranchMaster] 
	@Mode varchar (max),
	@CompanyCode varchar(max)=null,
	@HolidayCode varchar(max)=null,
	@NewBranchCode varchar(max)=null,
	@BranchCode varchar(max),
	@BranchName varchar(max)=null,
	@Address varchar(max)=null,
	@Phone varchar(max)=null,
	@Fax varchar(max)=null,
	@Email varchar(max)=null,
	@HrIncharge varchar(max)=null,
	@AlternativeHrIncharge varchar(max)=null		
AS
BEGIN

		declare @countValue int=0,@managerId varchar(250)=null
						if(@HrIncharge is not null and @HrIncharge!='' )
						begin
						   set @managerId=@HrIncharge
						end
						
					    else if(@AlternativeHrIncharge is not null and  @AlternativeHrIncharge!='')
						begin
						  set @managerId=@AlternativeHrIncharge
						end
	IF @Mode='I'
	BEGIN		
	
        
		insert into BranchMaster(BranchCode, BranchName , CompanyCode  ,HolidayCode ,BranchAddress ,PhoneNo ,FaxNo , Email , HrIncharge , AlternativeHrIncharge)
		values(@BranchCode , @BranchName , @CompanyCode  ,@HolidayCode, @Address , @Phone , @Fax , @Email , @HrIncharge , @AlternativeHrIncharge )

		--mapping TbManagerHrBranchMapping
						select @countValue=count(*) from TbManagerHrBranchMapping where BranchCode=@BranchCode  and CompanyCode=@CompanyCode
						if(@countValue>0)
							begin
								update TbManagerHrBranchMapping set  ManagerID=@managerId, BranchCode = @BranchCode , CompanyCode=@CompanyCode where BranchCode=@BranchCode  and CompanyCode=@CompanyCode
							end
						else
							begin
					    
								insert into TbManagerHrBranchMapping(ManagerID,BranchCode,CompanyCode)values(@managerId,@BranchCode,@CompanyCode)
							end
						 
					 
				    
	END
	
	IF @Mode ='U'
	BEGIN
		update BranchMaster set  BranchName = @BranchName , HolidayCode=@HolidayCode ,BranchAddress = @Address , PhoneNo = @Phone , FaxNo = @Fax , Email = @Email , HrIncharge = @HrIncharge , AlternativeHrIncharge = @AlternativeHrIncharge
		where BranchCode = @BranchCode and CompanyCode = @CompanyCode 	

		
			    select @countValue=count(*) from TbManagerHrBranchMapping where BranchCode=@BranchCode  and CompanyCode=@CompanyCode
			    if(@countValue>0)
					begin
					    update TbManagerHrBranchMapping set ManagerID=@managerId, BranchCode = @BranchCode , CompanyCode=@CompanyCode where BranchCode=@BranchCode  and CompanyCode=@CompanyCode
					end
				else
				    begin
					    
						insert into TbManagerHrBranchMapping(ManagerID,BranchCode,CompanyCode)values(@managerId,@BranchCode,@CompanyCode)
					end
	END
	
	IF @Mode='D'
	BEGIN
		delete from BranchMaster where BranchCode =@BranchCode 	
	END	
	
END
