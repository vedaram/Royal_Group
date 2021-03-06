
ALTER  PROCEDURE [dbo].[spSavePunchForApproval]      
(          
 @piMode  as char,    -- I for Insert, U for Update       
 @piEmpCode  as varchar(50),      
 @piWorkDate as varchar(20), 
 @pioutDate as varchar(20), 
 @piInPunch  as varchar(20),       
 @piOutPunch as varchar(20),        
 @piRemarks  as varchar(200),        
 @piCreatedBy as varchar(50),      
 @piModifiedBy as varchar(50),
 @PiBrkOut as varchar(20)=null,
 @PiBrkIn as varchar(20)=null, 
 @pipdate as varchar(40)=null      
)            
as            
BEGIN       
	declare @PiBrkHours_Min int,@PiBrkHours_Time datetime,@brkhrs datetime,@fromdate datetime,@todate datetime
	--default we are setting follwing two values while inserting to PunchForApproval table first time
	declare @Hr_approval int=0,@manager_approval int=1
			if(@pibrkin is not null and @pibrkout is not null)
			begin
			set @brkhrs= convert(datetime,@pibrkin)-convert(datetime,@pibrkout)
			end
 
    SET NOCOUNT ON  
    if @piMode ='I' 
    begin 
	      -- cheking @piCreatedBy is manager if he is, then manual punch consider as submitted by manager so,  manual punch approval will go to hr
		  	  if exists(select 1 from employeemaster  where Emp_code=@piCreatedBy and ismanager=1)
			  begin
				set @Hr_approval=1
				set @manager_approval=0
			  end
    
     insert into PunchForApproval(EmpCode, workdate, InPunch, OutPunch, ReasonForManualPunch, Approvalstatus,Approve, outdate,BreakOut,BreakIn,brk,Hr_approval,manager_approval) values (@piEmpCode,@piWorkDate,@piInPunch,@piOutPunch,@piRemarks,1,1, @pioutDate,@PiBrkOut,@PiBrkIn,@brkhrs,@Hr_approval,@manager_approval)  
	  if exists(select 1 from MASTERPROCESSDAILYDATA where Emp_ID=@piEmpCode)----Change by madhu on 18-12-2015 to inserteven if there are no record in MPDD---
	 begin
	     select @todate=min(pdate) from MASTERPROCESSDAILYDATA where Emp_ID=@piEmpCode
	 end
	 else
	 begin
	     set @todate=convert(date,getdate())
	 end
	 set @fromdate=@piWorkDate
	 if(@fromdate<@todate)
	 begin 
	 exec InsertMPDDManualPunches @piEmpCode,@fromdate,@todate
	 end
 end
 else if @piMode ='U'
 begin 
 update PunchForApproval set WorkDate =@piWorkDate,InPunch =@piInPunch,OutPunch =@piOutPunch ,ReasonForManualPunch =@piRemarks,outdate=@pioutDate,brk=@brkhrs where EmpCode =@piEmpCode and WorkDate =@piWorkDate 
 end
 SET NOCOUNT OFF  
    
END
