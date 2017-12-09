SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/************************************
* Date values                       *
*************************************/

SET NOCOUNT ON;

BEGIN TRANSACTION;

declare @startDate as date
declare @endDate as date
declare @curDate as date
declare @dayName as nvarchar(50)
declare @dayAbbrev as nvarchar(10)
declare @weekDayFlag as char(1)

declare @dateKey as int
declare @dayOfWeek as int
declare @dayOfMonth as int
declare @weekNoOfYear as int
declare @weekBeginDate as date
declare @monthNo as tinyint
declare @monthName as nvarchar(50)
declare @monthAbbrev as nvarchar(10)
declare @quarter as tinyint
declare @year as smallint
declare @yearmo as int
declare @sameDayYearAgo as date

-- Go back 3 years from the first day of the current year
Select @startDate = dateadd(yy, -3, dateadd(dd, 1-datepart(dy, getdate()), getdate()))

-- Go forward 3 years from the last day of the current year
Select @endDate = dateadd(yy, 4, dateadd(dd, -datepart(dy, getdate()), getdate()))

select @curDate = @startDate

while @curDate <= @endDate
Begin

	select @dateKey = datepart(yyyy,@curDate)*10000 + datepart(mm,@curDate)*100 + datepart(dd, @curDate)
	select @dayOfWeek = datepart(dw,@curDate)
	select @dayOfMonth = day(@curDate)
	select @dayName =  datename(dw,@curDate)
	select @dayAbbrev =  left(@dayName,3)
	select @weekDayFlag = CASE WHEN (@@DATEFIRST+@dayOfWeek) % 7 <2  THEN 'y' ELSE 'n' END
	select @weekNoOfYear = datepart(wk,@curDate)
	select @weekBeginDate = dateadd(dd,-@dayOfWeek+1, @curDate)
	select @monthNo = datepart(m, @curDate)
	select @monthName = datename(mm, @curDate)
	select @monthAbbrev = Left(@monthName, 3)
	select @quarter = datepart(q, @curDate)
	select @year = datepart(yy, @curDate)
	select @yearmo = @year*100+@monthNo
	select @sameDayYearAgo = dateadd(yy,-1,@curDate)

	-- Do the actual insert
	INSERT [pbist_sccm].[date] ([date_key], [full_date], [day_of_week], [day_num_in_month], [day_name], [day_abbrev]
	,[weekday_flag], [week_num_in_year], [week_begin_date], [month], [month_name],
	[month_abbrev], [quarter], [year],  [yearmo], same_day_year_ago_date)  
	VALUES (@dateKey,@curDate, @dayOfWeek, @dayOfMonth, @dayName, @dayAbbrev
	,@weekDayFlag, @weekNoOfYear, @weekBeginDate,  @monthNo, @monthName
	,@monthAbbrev, @quarter, @year, @yearmo,@sameDayYearAgo )

	-- Go to the next day
	Select @curDate = dateadd(dd,1,@curDate)
End

COMMIT TRANSACTION;



/************************************
* Configuration values              *
*************************************/
INSERT pbist_sccm.[configuration] (configuration_group, configuration_subgroup, [name], [value],[visible])
    VALUES (N'SolutionTemplate', N'System Center', N'version', N'1.0', 0),
           (N'SolutionTemplate', N'System Center', N'endpointcompliancetarget', N'0.99', 1),
           (N'SolutionTemplate', N'System Center', N'healthevaluationtarget', N'0.99', 1),
           (N'SolutionTemplate', N'System Center', N'dataretentiondays', N'120', 1),
           (N'SolutionTemplate', N'System Center', N'lastLoadTimestamp', '2017-01-01T01:00:00.000', 1),
           (N'SolutionTemplate', N'System Center', N'versionImage', N'https://bpstservice.azurewebsites.net/api/telemetry/Microsoft-SCCMTemplate', 1);
go

