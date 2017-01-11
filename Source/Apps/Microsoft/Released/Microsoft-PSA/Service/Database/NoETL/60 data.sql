SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/* POPULATE DATE TABLE */
BEGIN TRANSACTION

DECLARE @startyear INT = Datepart(year, Getdate()) - 1;
DECLARE @endyear INT = Datepart(year, Getdate()) + 10;
DECLARE @date DATE = Datefromparts(@startyear, 1, 1);
DECLARE @endDate DATE = Datefromparts(@endyear, 1, 1);
DECLARE @week_start_date DATE;
DECLARE @week_end_date DATE;
DECLARE @month_start_date DATE;
DECLARE @month_end_date DATE;
DECLARE @quarter_start_date DATE;
DECLARE @quarter_end_date DATE;
DECLARE @year INT;
DECLARE @quarter INT;
DECLARE @month INT;
DECLARE @week INT;

WHILE @date < @endDate
BEGIN
	SET @year = Datepart(year, @date);
	SET @quarter = Datepart(quarter, @date);
	SET @month = Datepart(month, @date);
	SET @week = Datepart(week, @date);
	SET @week_start_date = Dateadd(day, 1 - Datepart(weekday, @date), @date);
	SET @week_end_date = Dateadd(day, 6, @week_start_date);
	SET @month_start_date = Datefromparts(@year, @month, 1);
	SET @month_end_date = Dateadd(day, -1, Dateadd(month, 1, @month_start_date));
	SET @quarter_start_date = Datefromparts(@year, ( @quarter - 1 ) * 3 + 1, 1);
	SET @quarter_end_date = Dateadd(day, -1, Dateadd(month, 3, @quarter_start_date));

	INSERT psa.date
		 ([full_date],
		  [year],
		  [quarter],
		  [quarter_start_date],
		  [quarter_end_date],
		  [quarter_name],
		  [quarter_abbrevyear],
		  [month],
		  [month_start_date],
		  [month_end_date],
		  [month_name],
		  [month_abbrev],
		  [month_abbrevyear],
		  [day_of_year],
		  [day_of_month],
		  [day_of_week],
		  [day_name],
		  [day_abbrev],
		  [weeek_of_year],
		  [week_start_date],
		  [week_end_date],
		  [week_time_frame])
	VALUES( @date,
		  @year,
		  @quarter,
		  @quarter_start_date,
		  @quarter_end_date,
		  Concat('Q', @quarter),
		  Concat('Q', @quarter, ' ', @year),
		  @month,
		  @month_start_date,
		  @month_end_date,
		  Datename(month, @date),
		  Substring(Datename(month, @date), 0, 4),
		  Concat(Substring(Datename(month, @date), 0, 4), ' ', @year),
		  Datepart(dayofyear, @date),
		  Datepart(day, @date),
		  Datepart(weekday, @date),
		  Datename(weekday, @date),
		  Substring(Datename(weekday, @date), 0, 4),
		  @week,
		  @week_start_date,
		  @week_end_date,
		  Concat(Concat(@week_start_date, ' - '), @week_end_date) );

	SET @date = Dateadd(day, 1, @date);
END;

COMMIT TRANSACTION;
go

/* POPULATE CAPACITY TABLE
   Assumes start of the week is Sunday:
   
   SET DATEFIRST 7
*/
BEGIN TRANSACTION;
INSERT INTO psa.WeekDayCapacity(day_of_week, capacityMinutes) VALUES (1,   0);  -- SUNDAY
INSERT INTO psa.WeekDayCapacity(day_of_week, capacityMinutes) VALUES (2, 480);  -- MONDAY
INSERT INTO psa.WeekDayCapacity(day_of_week, capacityMinutes) VALUES (3, 480);  -- TUESDAY
INSERT INTO psa.WeekDayCapacity(day_of_week, capacityMinutes) VALUES (4, 480);  -- WEDNESDAY
INSERT INTO psa.WeekDayCapacity(day_of_week, capacityMinutes) VALUES (5, 480);  -- THURSDAY
INSERT INTO psa.WeekDayCapacity(day_of_week, capacityMinutes) VALUES (6, 480);  -- FRIDAY
INSERT INTO psa.WeekDayCapacity(day_of_week, capacityMinutes) VALUES (7,   0);  -- SATURDAY
COMMIT TRANSACTION;
go

/************************************
* Configuration values              *
*************************************/
INSERT psa.[configuration] (configuration_group, configuration_subgroup, [name], [value],[visible])
    VALUES (N'SolutionTemplate', N'PSA', N'version', N'1.0', 0),
           (N'SolutionTemplate', N'PSA', N'versionImage', N'https://bpstservice.azurewebsites.net/api/telemetry/Microsoft-SCCMTemplate', 1);
go

