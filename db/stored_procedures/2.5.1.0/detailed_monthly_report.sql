USE [SecurTimeWebMyntra2.5]
GO

/****** Object:  StoredProcedure [dbo].[PrepareMonthlyTimeSheet]    Script Date: 4/9/2016 10:31:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER procedure  [dbo].[PrepareMonthlyTimeSheet]
(
	@fromDate datetime,
	@toDate datetime,
	@whereCondition NVarchar(max)='where 1=1'
)--modified on 29102014 1722
As
BEGIN
/*****        This  procedure will use the self inner join queries  for #MPDD table to get the necessary columns and INSERT the  column
	  values  directly into the #DetailedMonthlyReport table so that it can be used to generate reports. The purpose of using this join query is to
	  get column values in multiple rows into a single row (Ex in_1,In_2...).The join query something looks like fallowing
	 
	--================================================================================================================================================================
	--exec('INSERT into #DetailedMonthlyReport('+@INSERTcolumnInfo+') Select '+@selectColumnInfo+' from '+@TableWithJoinInfo+'')--
	 Fallowing are the  varchar variables used to hold necessary information,
	 @INSERTcolumnInfo=In_1,Out_1,In_2,Out_2...
	 @selectColumnInfo= M1.In_punch as In_1,M1.Out_punch as Out_1,M2.In_punch as In_2.... 
	 @TableWithJoinInfo=#MPDD M1 inner join  #MPDD M2  on M1.Emp_ID= M2.Emp_ID and   M1.pdate=Convert(char,@FromDate) and m2.pdate= Convert(char, DateAdd(day,1,@FromDate))
	--================================================================================================================================================================
	 
	 After INSERTing data into #DetailedMonthlyReport	table, few  of its columns are UPDATEd with required calculation Ex:- Total worked Hrs, Total OverTime etc...For example look at the bottom of this page.								
******/
	Declare 
			@totalDays int,
			@selectColumnInfo varchar(max),
	        @INSERTcolumnInfo varchar(max),
			@TableWithJoinInfo Varchar(max), 
			@qry varchar(max),
			@workHour varchar(max),
			@noOfDays int,
			@minDate date,
			@maxDate date,
			@index int,
			@count int,
			@i int,
			@iChar varchar(30),
			@j int,
			@jChar varchar(30),
			@mdate varchar(max),
			@Nqry NVarchar(max) ,
			@SelectQry NVarchar(max) 		 
				
		--================================================================================================================================================================	
		--Creating temporary tables for processing the reporting data
			
			select * into #MPDD from 	MASTERPROCESSDAILYDATA where 1=0
			select * into #MPDDIndividual from MASTERPROCESSDAILYDATA where 1=0
			select * into #DetailedMonthlyReport from DetailedMonthlyReport where 1=0			
	 	   
		--================================================================================================================================================================	
		 -- Taking data only between fromdata and Todate into temp table   #MPDD
		   
		    SET @Qry=N'INSERT into #MPDD(emp_id,emp_name,pdate,In_punch,BreakOut,BreakIn,Out_Punch,Status,WorkHrs,Total_Hrs,LateBy,EarlyBy,OT,SOT,Shift_In,Shift_Out,shift_Code,Comp_Name,Dept_Name,Desig_Name,Cat_Name,Shift_Name) select emp_id,emp_name,pdate,In_punch,BreakOut,BreakIn,Out_Punch,Status,WorkHrs,Total_Hrs,LateBy,EarlyBy,OT,SOT,Shift_In,Shift_Out,shift_Code,Comp_Name,Dept_Name,Desig_Name,Cat_Name,Shift_Name from MASTERPROCESSDAILYDATA  '+@whereCondition+' and  PDate between '''+Convert(char,@fromDate)+''' and '''+Convert(char,@toDate)+''''-- Getting the data between given @fromDate and @toDate from MASTERPROCESSDAILYDATA into #MPDD to deal with data for  duration of only one month .    		  
    		exec(@Qry)			
    
	--================================================================================================================================================================
	--Updating work hour based on OT elibility,EmployeeCategoryThreshold etc
	
			UPDATE #MPDD SET WorkHRs=
					case when status='P' then 
							( 
								Case when WorkHrs > Total_hrs and E.OT_Eligibility=1 and ECM.Includeprocess=0 then Total_hrs 
								else 
										case when  WorkHrs > ECM.Totalhrs and E.OT_Eligibility=1 and ECM.Includeprocess=1 then  ECM.Totalhrs else WorkHrs END
								END
							)							
					Else
						 case when (status in ('M','MI','MO') and SOT is  null) then 						 
						  ( 
								Case when WorkHrs > Total_hrs and E.OT_Eligibility=1 and ECM.Includeprocess=0 then Total_hrs 
								else 
										case when  WorkHrs > ECM.Totalhrs and E.OT_Eligibility=1 and ECM.Includeprocess=1 then  ECM.Totalhrs else WorkHrs END
								END
						   )
						 Else
							case when status in ('WOP','HP') or SOT is not null then null else WorkHrs END
							END
					END		

				from #MPDD TM inner join EmployeeMaster E on E.Emp_code=TM.Emp_ID left outer join employeeCategoryMaster ECM on E.EMP_Employee_category=ECM.EMPCategoryCode      
				
	--================================================================================================================================================================
	--Taking Only approved OT
					
			 select * into #TempOverTime from OverTime where 1=0--Creating table and inserting data from overtime to #TempOverTime
			 INSERT into  #TempOverTime Select EmpId,OTDate,OTHrs,Flag,EmpName,ModifiedBy,Mflag,ApprovedbyId,ApprovedbyName,ApprovalLevel from OverTime  where OTDate  between @fromDate and @ToDate
					
			 UPDATE M SET OT= case when  OT.flag=2 then OT else null END 
			 FROM  #TempOverTime OT inner join  #MPDD M on  OT.EMPID = M.Emp_ID and OT.OTDate = M.PDate and OT.flag is not null
				
	--================================================================================================================================================================
    --Taking data from #MPDD(MasterProcessdailydata) into detailedmonthly table format using self join 
			create clustered index indexMPDD on #MPDD (EMP_ID,pdate)
			create clustered index indexMPDDIndividual on #MPDDIndividual (EMP_ID,pdate)
			
			SET @mdate= ''+convert(char(10),@fromDate)+' to ' + convert(char(10),@toDate)+''
			SET @totalDays= DATEDIFF(day,@fromDate,@toDate)+1
			
			Select @minDate= MIN(PDate) from #MPDD where PDate>=@fromDate --Getting the Min date which exist Between given @fromDate and @toDate in #MPDD
			Select @maxDate= MAX(PDate) from #MPDD where PDate<=@toDate   --Getting the Max date which exist Between given @fromDate and @toDate in #MPDD  (these two will help in inner self joins)
			
			SET @noOfDays=DATEDIFF(DAY,@minDate,@maxDate)+1 --No of unique dates exists  Between @minDate and @maxDate  in #MPDD
			SET @index= DATEDIFF(DAY,@fromDate,@minDate)+1
			SET @i=@index
			SET @iChar=CONVERT(char,@i)
		
			
			SET @selectColumnInfo=' M'+@iChar+'.Emp_ID,M'+@iChar+'.Emp_Name,M'+@iChar+'.Comp_Name, M'+@iChar+'.CAT_NAME, M'+@iChar+'.DEPT_NAME, M'+@iChar+'.DESIG_NAME' 		  
			SET @INSERTcolumnInfo='Emp_ID,Emp_Name,Comp_Name,CAT_NAME,DEPT_NAME,DESIG_NAME'
			
			SET @selectColumnInfo= @selectColumnInfo+', Convert(char(10),Datepart(day,M'+@iChar+'.pdate)) as PDate_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.In_Punch))) as In_'+@iChar+',Convert(char(5),convert(time,( M'+@iChar+'.BreakOut))) as BreakOut_'+@iChar+' ,Convert(char(5),convert(time,(M'+@iChar+'.BreakIn)))  as BreakIn_'+@iChar+',Convert(char(5),convert(time,( M'+@iChar+'.Out_Punch))) as Out_'+@iChar+',Convert(char(5),M'+@iChar+'.WorkHRs) as Work_'+@iChar+',M'+@iChar+'.status as Status_'+@iChar+',coalesce((convert(char(5),M'+@iChar+'.OT)),(convert(char(5),M'+@iChar+'.SOT))) as OT'+@iChar+',convert(char(5),M'+@iChar+'.SOT) as SOT'+@iChar+''						
			SET @INSERTcolumnInfo= @INSERTcolumnInfo+ ',PDate_'+@iChar+', In_'+@iChar+',BreakOut_'+@iChar+',BreakIn_'+@iChar+',Out_'+@iChar+',Work_'+@iChar+',Status_'+@iChar+',OT'+@iChar+',SOT'+@iChar+''						
					
			SET @count=1
			
			while(@count < @noOfDays)--if we have more than one day data then this block will work
				BEGIN
					SET @iChar=Convert(int,@iChar)+1								
					SET @selectColumnInfo= @selectColumnInfo+',Convert(char(10), Datepart(day,M'+@iChar+'.pdate)) as PDate_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.In_Punch))) as In_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.BreakOut))) as BreakOut_'+@iChar+' ,Convert(char(5),convert(time,(M'+@iChar+'.BreakIn)))  as BreakIn_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.Out_Punch))) as Out_'+@iChar+',Convert(char(5),M'+@iChar+'.WorkHRs) as Work_'+@iChar+', M'+@iChar+'.status as Status_'+@iChar+',coalesce((convert(char(5),M'+@iChar+'.OT)),(convert(char(5),M'+@iChar+'.SOT))) as OT'+@iChar+',convert(char(5),M'+@iChar+'.SOT) as SOT'+@iChar+''							
					SET @INSERTcolumnInfo= @INSERTcolumnInfo+ ',PDate_'+@iChar+', In_'+@iChar+',BreakOut_'+@iChar+',BreakIn_'+@iChar+',Out_'+@iChar+',Work_'+@iChar+',Status_'+@iChar+',OT'+@iChar+',SOT'+@iChar+''						
					SET @count=@count+1				
				END
			
			SET @i=@index
			SET @j=@i
			SET @jChar=CONVERT(char,@j)	
			SET @iChar=CONVERT(char,@i)	
			SET @iChar=Convert(int,@iChar)+1
			SET @count=1
			if(@noOfDays=1)
				BEGIN
					SET @TableWithJoinInfo= ' #MPDD M'+@jChar+' where   M'+@jChar+'.pdate='''+Convert(char,@minDate)+''''
				END
			Else
			   BEGIN	
			       SET @TableWithJoinInfo= ' #MPDD M'+@jChar+''
			   END
			   
			while(@count <@noOfDays)
				BEGIN
					SET @TableWithJoinInfo = @TableWithJoinInfo + 'inner join  #MPDD M'+@iChar+' on M'+@jChar+'.Emp_ID= M'+@iChar+'.Emp_ID  and  M'+@jChar+'.pdate='''+Convert(char,@minDate)+''' and M'+@iChar+'.pdate= '''+Convert(char, DateAdd(day,@count,@minDate))+''''
					SET @iChar=Convert(int,@iChar)+1
					SET @count=@count+1				
				END
		
				  SET @Nqry='INSERT into #DetailedMonthlyReport('+@INSERTcolumnInfo+') Select '+@selectColumnInfo+' from '+@TableWithJoinInfo+''                
				
				  EXEC(@NQry)	
				  
						
  --================================================================================================================================================================
  /* The fallowing code is to get the data for detailedmonthly report table for employees  who are missed in the join query .
  it happens when when emp joins the company  later(joining date grater than @minDate) or gets terminated before(leaving date less than @maxDate) in the month where we are generating monthly report the company*/				
  			  
  		
				declare @Empid varchar(20)		
				declare cur cursor 
				for select distinct M.Emp_ID from #MPDD M left outer join #DetailedMonthlyReport D on M.Emp_ID=D.Emp_Id  where D.Emp_Id is  null 
				open cur
					fetch next from cur into @empid
					while(@@FETCH_STATUS=0 and @Count>0)
					BEGIN
								SET @minDate=null SET @maxDate=null								
						        truncate table #MPDDIndividual
								INSERT into #MPDDIndividual(emp_id,emp_name,pdate,In_punch,BreakOut,BreakIn,Out_Punch,Status,WorkHrs,Total_Hrs,LateBy,EarlyBy,OT,SOT,Shift_In,Shift_Out,shift_Code,Comp_Name,Dept_Name,Desig_Name,Cat_Name,Shift_Name) 
    							select emp_id,emp_name,pdate,In_punch,BreakOut,BreakIn,Out_Punch,Status,WorkHrs,Total_Hrs,LateBy,EarlyBy,OT,SOT,Shift_In,Shift_Out,shift_Code,Comp_Name,Dept_Name,Desig_Name,Cat_Name,Shift_Name from MASTERPROCESSDAILYDATA  where emp_id=@empid and  PDate between @fromDate and @toDate-- Getting the data between given @fromDate and @toDate from MASTERPROCESSDAILYDATA into #MPDDIndividual to deal with data for  duration of only one month .
			
					   			Select @minDate= MIN(PDate) from #MPDDIndividual where PDate>=@fromDate --Getting the Min date which exist Between given @fromDate and @toDate in #MPDDIndividual
								Select @maxDate= MAX(PDate) from #MPDDIndividual where PDate<=@toDate   --Getting the Max date which exist Between given @fromDate and @toDate in #MPDDIndividual  (these two will help in inner self joins)
								
								SET @noOfDays=DATEDIFF(DAY,@minDate,@maxDate)+1 --No of unique dates exists  Between @minDate and @maxDate  in #MPDDIndividual
								SET @index= DATEDIFF(DAY,@fromDate,@minDate)+1
								SET @i=@index
								SET @iChar=CONVERT(char,@i)							
								
								SET @selectColumnInfo=' M'+@iChar+'.Emp_ID,M'+@iChar+'.Emp_Name,M'+@iChar+'.Comp_Name, M'+@iChar+'.CAT_NAME, M'+@iChar+'.DEPT_NAME, M'+@iChar+'.DESIG_NAME' 		  
								SET @INSERTcolumnInfo='Emp_ID,Emp_Name,Comp_Name,CAT_NAME,DEPT_NAME,DESIG_NAME'
								
								SET @selectColumnInfo= @selectColumnInfo+', Convert(char(10),Datepart(day,M'+@iChar+'.pdate)) as PDate_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.In_Punch))) as In_'+@iChar+',Convert(char(5),convert(time,( M'+@iChar+'.BreakOut))) as BreakOut_'+@iChar+' ,Convert(char(5),convert(time,(M'+@iChar+'.BreakIn)))  as BreakIn_'+@iChar+',Convert(char(5),convert(time,( M'+@iChar+'.Out_Punch))) as Out_'+@iChar+',Convert(char(5),M'+@iChar+'.WorkHRs) as Work_'+@iChar+',M'+@iChar+'.status as Status_'+@iChar+',coalesce((convert(char(5),M'+@iChar+'.OT)),(convert(char(5),M'+@iChar+'.SOT))) as OT'+@iChar+',convert(char(5),M'+@iChar+'.SOT) as SOT'+@iChar+''						
								SET @INSERTcolumnInfo= @INSERTcolumnInfo+ ',PDate_'+@iChar+', In_'+@iChar+',BreakOut_'+@iChar+',BreakIn_'+@iChar+',Out_'+@iChar+',Work_'+@iChar+',Status_'+@iChar+',OT'+@iChar+',SOT'+@iChar+''						
										
								SET @count=1								
								while(@count < @noOfDays)--if we have more than one day data then this block will work
									BEGIN
										SET @iChar=Convert(int,@iChar)+1								
										SET @selectColumnInfo= @selectColumnInfo+',Convert(char(10), Datepart(day,M'+@iChar+'.pdate)) as PDate_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.In_Punch))) as In_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.BreakOut))) as BreakOut_'+@iChar+' ,Convert(char(5),convert(time,(M'+@iChar+'.BreakIn)))  as BreakIn_'+@iChar+', Convert(char(5),convert(time,(M'+@iChar+'.Out_Punch))) as Out_'+@iChar+',Convert(char(5),M'+@iChar+'.WorkHRs) as Work_'+@iChar+', M'+@iChar+'.status as Status_'+@iChar+',coalesce((convert(char(5),M'+@iChar+'.OT)),(convert(char(5),M'+@iChar+'.SOT))) as OT'+@iChar+',convert(char(5),M'+@iChar+'.SOT) as SOT'+@iChar+''							
										SET @INSERTcolumnInfo= @INSERTcolumnInfo+ ',PDate_'+@iChar+', In_'+@iChar+',BreakOut_'+@iChar+',BreakIn_'+@iChar+',Out_'+@iChar+',Work_'+@iChar+',Status_'+@iChar+',OT'+@iChar+',SOT'+@iChar+''						
										SET @count=@count+1				
									END
								
								SET @i=@index
								SET @j=@i
								SET @jChar=CONVERT(char,@j)			
								SET @iChar=CONVERT(char,@i)			
								SET @iChar=Convert(int,@iChar)+1
								SET @count=1
								if(@noOfDays=1)
									BEGIN
										SET @TableWithJoinInfo= ' #MPDDIndividual M'+@jChar+' where   M'+@jChar+'.pdate='''+Convert(char,@minDate)+''''
									END
								Else
								   BEGIN	
									   SET @TableWithJoinInfo= ' #MPDDIndividual M'+@jChar+''
								   END
								   
								while(@count <@noOfDays)
									BEGIN
										SET @TableWithJoinInfo = @TableWithJoinInfo + 'inner join  #MPDDIndividual M'+@iChar+' on M'+@jChar+'.Emp_ID= M'+@iChar+'.Emp_ID  and  M'+@jChar+'.pdate='''+Convert(char,@minDate)+''' and M'+@iChar+'.pdate= '''+Convert(char, DateAdd(day,@count,@minDate))+''''
										SET @iChar=Convert(int,@iChar)+1
										SET @count=@count+1				
									END
							
									  SET @Nqry='INSERT into #DetailedMonthlyReport('+@INSERTcolumnInfo+') Select '+@selectColumnInfo+' from '+@TableWithJoinInfo+''
					                  Exec(@Nqry)  											
								fetch next from cur into @empid	
					END
					close cur
					deallocate cur

	--================================================================================================================================================================
    /*  Updation Part: Total work hour, Total OT,Total week offs,Total leaves throught the month */				
				
				
				UPDATE #DetailedMonthlyReport SET CheckDate = @fromDate,totdays=@totalDays,mdate=@mdate					
  					
  				UPDATE D set Present= 
  				(select  COUNT(M.Status) from #MPDD M  where (M.PDate between @Fromdate and @Todate) and  (M.Status ='P' or M.Status='OD' or M.Status='WOP' or M.Status='HP' or  M.Status='M' or M.Status='MI' or M.Status='MO') and D.EMp_id= M.Emp_id)
  			+    convert(decimal(4,1),(select  COUNT(M.Status) from #MPDD M where (M.PDate between @Fromdate and @Todate) and   (M.Status ='PHL' or M.Status = 'AHP' )  and D.EMp_id= M.Emp_id)/convert(decimal(4,2),2) ) from #DetailedMonthlyReport D                 
  			        			
  				 
  				UPDATE D SET Absent= 
  				(select  COUNT(M.Status) from #MPDD M where D.EMp_id= M.Emp_id and (M.PDate between @Fromdate and @Todate) and  ( M.Status='A'))
  			+    convert(decimal(4,1),(select  COUNT(M.Status) from #MPDD M Where D.EMp_id= M.Emp_id and (M.PDate between @Fromdate and @Todate) and   (M.Status ='AHL'  OR M.Status='AHP'))/convert(decimal(4,1),2))from #DetailedMonthlyReport D                   
  				    				  
  				 	 
  				UPDATE D SET Leave= 
  				(select  COUNT(M.Status) from #MPDD M  where D.EMp_id= M.Emp_id and (M.PDate between @Fromdate and @Todate) and  ( M.Status='L' or M.Status='CO') )
  			+   convert(decimal(4,1),(select  COUNT(M.Status) from #MPDD M  where D.EMp_id= M.Emp_id and (M.PDate between @Fromdate and @Todate) and   (M.Status ='AHL' or status='PHL' ))/convert(decimal(4,1),2) )    from #DetailedMonthlyReport D                                 
  				 
  			
				UPDATE D SET Holiday=   	
  			       (select  COUNT(M.Status) from #MPDD M where  D.EMp_id= M.Emp_id and (M.PDate between @Fromdate and @Todate) and  ( M.Status='H'))from #DetailedMonthlyReport D                   
  				 
					
				UPDATE D SET woff =
  				 (select  COUNT(M.Status) from #MPDD M   where  D.EMp_id= M.Emp_id and (M.PDate between @Fromdate and @Todate) and  ( M.Status='WO' or M.Status='woff'))from #DetailedMonthlyReport D                   
  				 
  				
  				--UPDATE #DetailedMonthlyReport  
  				--SET Shift_NAME =  				
  			 --   (select S.Shift_Desc from EmployeeMaster E inner join shift S on E.Emp_Shift_Detail=S.Shift_Code inner join  #DetailedMonthlyReport D on D.EMp_id= E.Emp_Code)
  			
  			 			   
  			    UPDATE D SET HrsWrkd=
  			      (select cast(cast((sum(DATEPART(hour,WorkHRs)) * 60 + (sum(DATEPART(minute,WorkHRs)))) / 60  as int) as varchar(50)) + ':' 	+ cast(sum(DATEPART(minute,WorkHRs)) % 60 as varchar(2))
				from #MPDD M  where D.EMp_id= M.Emp_ID and PDate between @Fromdate and @Todate)from #DetailedMonthlyReport D                   
  				 	
  				
  				UPDATE D SET OverTime=  				 	  						  
					(SELECT	convert(varchar,((coalesce(sum((datepart(hour,M.ot)*60)+datepart(minute,M.ot)),0)+ 
											  coalesce(sum((datepart(hour,M.SOT)*60)+datepart(minute,M.SOT)),0))/60))
					   +':'+
							convert(varchar,((coalesce(sum((datepart(hour,M.ot)*60)+datepart(minute,M.ot)),0)+
											  coalesce(sum((datepart(hour,M.sot)*60)+datepart(minute,M.SOT)),0))%60))
		  		from #MPDD M where D.EMp_id= M.Emp_ID and PDate between @Fromdate and @Todate)from #DetailedMonthlyReport D                   
				  
				  		
				  				
				UPDATE D SET SOT=
				   (select cast(cast((sum(DATEPART(hour,M.SOT)) * 60 + (sum(DATEPART(minute,M.SOT)))) / 60  as int) as varchar(50)) + ':' 	+ cast(sum(DATEPART(minute,M.SOT)) % 60 as varchar(2))
	    			from #MPDD M where D.EMp_id= M.Emp_ID and PDate between @Fromdate and @Todate)from #DetailedMonthlyReport D                   
  				    
  				  				
  				   
  			   UPDATE D  SET TotalEarlyBy=
  			       (select cast(cast((sum(DATEPART(hour,EarlyBy)) * 60 + (sum(DATEPART(minute,EarlyBy)))) / 60  as int) as varchar(50)) + ':' 	+   Right('00'+isnull(cast(sum(DATEPART(minute,EarlyBy)) % 60 as varchar(2)),''),2)
				from #MPDD M where D.EMp_id= M.Emp_ID  and PDate between @Fromdate and @Todate)from #DetailedMonthlyReport D                   
  				 
  				
  			    
  			   UPDATE D  SET TotalLateBy=
  			       (select cast(cast((sum(DATEPART(hour,LateBy)) * 60 + (sum(DATEPART(minute,LateBy)))) / 60  as int) as varchar(50)) + ':' 	+  Right('00'+isnull( cast(sum(DATEPART(minute,LateBy)) % 60 as varchar(2)),''),2)
				from #MPDD M  where D.EMp_id= M.Emp_ID and  PDate between @Fromdate and @Todate )from #DetailedMonthlyReport D                   
  			   
  					 	
  		select * from #DetailedMonthlyReport order by emp_id
 
  			  	
  			  
  			   
END
















/*
Examples:-

select  m1.emp_id, m1.Status as status1,m2.status  as status2 ,m3.status  as status3,M4.STATUS  as status4
 from
   #MPDD m1 
    inner join #MPDD m2 on m1.Emp_ID=m2.Emp_ID and  m1.pdate='2013-04-05' and m2.PDate='2013-04-06'
    inner join #MPDD m3 on m1.Emp_ID=m3.Emp_ID  and  m1.pdate='2013-04-05' and m3.PDate='2013-04-07'
    inner join  #MPDD M4 on m1.Emp_ID=M4.Emp_ID  and m1.pdate='2013-04-05' and m4.PDate='2013-04-08' 
 */

/*Regards*/
/*By Vinay */
		
GO


