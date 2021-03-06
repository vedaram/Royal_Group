USE [STW_DB]
GO
/****** Object:  UserDefinedFunction [dbo].[FetchTotalCounts]    Script Date: 06/22/2016 11:20:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER  FUNCTION [dbo].[FetchTotalCounts]( @mode int )

RETURNS @TotalCalculation TABLE 
(Total_Hours varchar(10),Total_Lateby varchar(10),Total_Earlyby varchar(10),TotalDays varchar(10),TotalEmployee varchar(10))
AS
begin
	
	
	
	
	declare @SUMWORKHRS varchar(max),@totaldays1 int,@maxdate1 datetime,@mindate1 datetime,@totalemp int,@totalearlyby varchar(max),@totallateby varchar(max)
	if ( @mode = 1 )
	begin
		select @maxdate1=max(pdate) from DailyPerformancereport1
		select @mindate1=min(pdate) from DailyPerformancereport1
		set @totaldays1=datediff(day,@mindate1,@maxdate1)
		set @totaldays1=@totaldays1+1
		set @totalemp=null
		select @totalemp= count(distinct emp_id) from DailyPerformancereport1
		select @SUMWORKHRS= cast(cast((sum(DATEPART(hour,WorkHRs)) * 60 + (sum(DATEPART(minute,WorkHRs)))) / 60  as int) as varchar(50)) + ':' 	
		+ cast(sum(DATEPART(minute,WorkHRs)) % 60 as varchar(2))from DailyPerformancereport1
 
		select @totalearlyby=	cast(cast((sum(DATEPART(hour,earlyby)) * 60 + 
		(sum(DATEPART(minute,earlyby)))) / 60  as int) as varchar(50)) + ':' 	
		+ cast(sum(DATEPART(minute,earlyby)) % 60 as varchar(2))from DailyPerformancereport1  
 
 
		select @totallateby=	cast(cast((sum(DATEPART(hour,lateby)) * 60 + 
		(sum(DATEPART(minute,lateby)))) / 60  as int) as varchar(50)) + ':' 	
		+ cast(sum(DATEPART(minute,lateby)) % 60 as varchar(2))from DailyPerformancereport1 
	end


	
	
		
	INSERT INTO @TotalCalculation values(@SUMWORKHRS,@totallateby,@totalearlyby,@totaldays1,@totalemp)
	

   return
end