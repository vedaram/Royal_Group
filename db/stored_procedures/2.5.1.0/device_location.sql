/****** Object:  StoredProcedure [dbo].[SpUpdateDeviceLocation]    Script Date: 19/04/2016 10:59:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================    
-- Author:  <Author,,Name>    
-- Create date: <Create Date,,>    
-- Description: <Description,,>    
-- =============================================    
ALTER PROCEDURE [dbo].[SpUpdateDeviceLocation]    
(    
 @Mode varchar(max),    
 @Location varchar(50)=null,    
 @deviceserialno varchar(max)=null,  
 @branchCode varchar(max)=null  
)    
as     
BEGIN    
 declare @count int    
 IF @Mode='I'    
 Begin    
      
  --insert into DeviceSerialNo(DeviceLocation,DeviceSNo)    
  --values(@Location,@deviceserialno)    
  --select @count=count(*) from devicetype    

  -- sanjeev 10/07/2015

  insert into Device_Location( Deviceid,deviceLocation,branchcode)    
  values(@deviceserialno,@Location,@branchCode)    
  select @count=count(*) from devicetype
   
      
 End    
     
 IF @Mode='U'    
 Begin    
     --DeviceSNo=@deviceserialno    
  --update DeviceSerialNo set DeviceLocation=@Location
  --where DeviceSNo=@deviceserialno    
  update Device_Location set DeviceLocation=@Location, Deviceid=@deviceserialno , branchcode=@branchCode
  where  Deviceid=@deviceserialno
     
 End    
     
 IF @Mode='D'    
 Begin    
     
  --delete from DeviceSerialNo where DeviceLocation=@Location  
  delete from Device_Location where Deviceid=@deviceserialno        
    
 End    
END 
