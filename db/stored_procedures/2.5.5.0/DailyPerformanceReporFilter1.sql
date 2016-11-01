
/****** Object:  StoredProcedure [dbo].[DailyPerformanceReporFilter1]    Script Date: 06/22/2016 18:43:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[DailyPerformanceReporFilter1] (@where varchar(max))
as Begin    --modified on 03112014-1630
 truncate table DailyPerformancereport1 
  declare @sql varchar(max)
 set @sql=null
 --set @sql = 'insert into DailyPerformancereport1(Emp_ID, Emp_Name, PDate, Comp_Name, Shift_In, Shift_Out, In_Punch, Out_Punch, OT, WorkHRs,LUNCHHRS,PERSONALHRS,BUSINESSHRS, LateBy, EarlyBy, Status,BreakIn,BreakOut,sot,breakname,breakname1,breakname2,[BreakOut EarlyBy],[BreakIn LateBy],Dept_Name,Cat_Name,Desig_Name,Shift_Name,total_hrs)select Emp_ID, Emp_Name, PDate, Comp_Name , Shift_In, Shift_Out, convert(varchar(5),convert(varchar,convert(time(7),In_Punch))),convert(varchar(5),convert(varchar,convert(time(7),Out_Punch))), convert(varchar(5),OT), convert(varchar(5),WorkHRs), convert(varchar(5),LUNCHHRS),convert(varchar(5),PERSONALHRS),convert(varchar(5),BUSINESSHRS),convert(varchar(5),LateBy),convert(varchar(5),EarlyBy), Status,convert(varchar(5),convert(time,BreakIn)),convert(varchar(5),convert(time,BreakOut)), convert(varchar(5),SOT),breakname,breakname1,breakname2,Convert(Varchar(5),BreakOut_EarlyBy),Convert(varchar(5),BreakIn_LateBy),Dept_Name,Cat_Name,Desig_Name,Shift_Name,convert(varchar(5),convert(time,Total_Hrs)) from MASTERPROCESSDAILYDATA '+@where+'  order by PDate, Emp_ID'  

  set @sql = 'insert into DailyPerformancereport1(Emp_ID, Emp_Name, PDate, Comp_Name, Shift_In, Shift_Out, In_Punch, Out_Punch, OT, WorkHRs,LUNCHHRS,
  PERSONALHRS,BUSINESSHRS, LateBy, EarlyBy, Status,BreakIn,BreakOut,sot,breakname,breakname1,breakname2,[BreakOut EarlyBy],[BreakIn LateBy],Dept_Name,
  Cat_Name,Desig_Name,Shift_Name,total_hrs , Employee_Category  )select Emp_ID, Emp_Name, PDate, Comp_Name , convert(char(5),convert(dateTime,Shift_In),108),
   convert(char(5),
  convert(dateTime,Shift_out),108), convert(varchar(5),convert(varchar,convert(time(7),In_Punch))),convert(varchar(5),convert(varchar,convert(time(7),
  Out_Punch))), convert(varchar(5),OT), convert(varchar(5),WorkHRs), convert(varchar(5),LUNCHHRS),convert(varchar(5),PERSONALHRS),convert(varchar(5),
  BUSINESSHRS),convert(varchar(5),LateBy),convert(varchar(5),EarlyBy), Status,convert(varchar(5),convert(time,BreakIn)),convert(varchar(5),
  convert(time,BreakOut)), convert(varchar(5),SOT),breakname,breakname1,breakname2,Convert(Varchar(5),BreakOut_EarlyBy),Convert(varchar(5),BreakIn_LateBy)
  ,Dept_Name,Cat_Name,Desig_Name,Shift_Name,convert(varchar(5),convert(time,Total_Hrs)) , EmployeeCategory   from MASTERPROCESSDAILYDATA '+@where+'  order by PDate, Emp_ID'
 execute (@sql)

	--UPDATE DailyPerformancereport1 SET WorkHRs=
	--				case when status='P' then 
	--						( 
	--							Case when WorkHrs > Total_hrs and E.OT_Eligibility=1 and ECM.Includeprocess=0 then Total_hrs 
	--							else 
	--									case when  WorkHrs > ECM.Totalhrs and E.OT_Eligibility=1 and ECM.Includeprocess=1 then  ECM.Totalhrs else WorkHrs END
	--							END
	--						)							
	--				Else
	--					 case when (status in ('M','MI','MO') and SOT is  null) then 						 
	--					  ( 
	--							Case when WorkHrs > Total_hrs and E.OT_Eligibility=1 and ECM.Includeprocess=0 then Total_hrs 
	--							else 
	--									case when  WorkHrs > ECM.Totalhrs and E.OT_Eligibility=1 and ECM.Includeprocess=1 then  ECM.Totalhrs else WorkHrs END
	--							END
	--					   )
	--					 Else
	--						case when status in ('WOP','HP') or SOT is not null then null else WorkHrs END
	--						END
	--				END		

	--			from DailyPerformancereport1 TM inner join EmployeeMaster E on E.Emp_code=TM.Emp_ID left outer join employeeCategoryMaster ECM on E.EMP_Employee_category=ECM.EMPCategoryCode 
--New Update code 
 /*
 update DailyPerformancereport1 set WorkHRs=null where SOT is not null
 
 update DailyPerformancereport1 set 
		WorkHRs=(
			case when WorkHrs > Total_hrs and E.OT_Eligibility=1 and ECM.Includeprocess=0 then  Total_hrs 
				else 
					case when  WorkHrs> ECM.Totalhrs and E.OT_Eligibility=1 and ECM.Includeprocess=1 then  ECM.Totalhrs else WorkHrs 
				end
			end
		)
	from DailyPerformancereport1 DPR1 inner join  EmployeeMaster E on DPR1.Emp_ID=E.Emp_Code
	left outer join employeeCategoryMaster ECM on E.EMP_Employee_category=ECM.EMPCategoryCode 
	*/

UPDATE DAILYPERFORMANCEREPORT1 SET OT=NULL
UPDATE DAILYPERFORMANCEREPORT1 SET OT=OTHRS FROM OVERTIME O JOIN DAILYPERFORMANCEREPORT1 D ON O.EMPID=D.EMP_ID AND O.OTDATE=D.PDate AND O.FLAG=2
--update dailyperformancereport1 set Status='L*' where Status='L' and deductleaveflag=1
--Changes by Vinay ends

End