SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE VIEW fbpa.fb_insights AS 
(
SELECT 
id, 
CAST(EndTime as date) as end_time, 
[Entry Name] as metric, 
[Value] as [value],
'insights/' + [Name] + '/' + [Period] as [key_id], 
id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
FROM [fbpa].[Clicks]
UNION 
	SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PageContent]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PageEngagement]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PageImpressions]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PagePost]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PagePostEngagement]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PagePostStoriesAndPeopleTalkingAboutThis]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PageReactions]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PageUserDemographics]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PageVideoViews]
UNION
    SELECT 
	id, 
	CAST(EndTime as date) as end_time, 
	[Entry Name] as metric, 
	CAST([Value] as decimal) as [value],
	'insights/' + [Name] + '/' + [Period] as [key_id], 
	id + '/' + REPLACE(CONVERT(VARCHAR(24),EndTime, 111),'/','-') as control_id 
	FROM [fbpa].[PageViews]
)
GO

CREATE VIEW fbpa.fb_metrics AS 
(
SELECT 
Id as id,
[Name] as [name], 
[Period] as [period],
[Title] as [title],
[Description] as [description],
CASE 
	WHEN ([Description] LIKE '%Total Count%') 
	THEN 'Total Count' ELSE 'Unique Users' END as [status],
[PageId] as idpage,
'insights/' + [Name] + '/' + [Period] as [key_id]
FROM [fbpa].[Clicks]
UNION 
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PageContent]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PageEngagement]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PageImpressions]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PagePost]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PagePostEngagement]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PagePostStoriesAndPeopleTalkingAboutThis]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PageReactions]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PageUserDemographics]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id]
	FROM [fbpa].[PageVideoViews]
UNION
	SELECT 
	Id as id,
	[Name] as [name], 
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
	CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	[PageId] as idpage,
	'insights/' + [Name] + '/' + [Period] as [key_id] 
	FROM [fbpa].[PageViews]
)
GO

CREATE VIEW fbpa.fb_posts AS
(
	SELECT
	DISTINCT [Id] as id,
	[Name] as [name],
	[Message] as [message],
	CAST([Created Time] as date) as [create_time],
	[Updated Time] as [update_time],
	[Link] as [link],
	[Type] as [type],
	[Icon] as [icon],
	[Story] as [story],
	[Source] as [source],
	[Status Type] as [status type],
	[PageId] as [page_id],
	[Object] as [object_id],
	[Is Hidden] as [is_hidden],
	[Is Published] as [is_published],
	[Picture] as [picture],
	CAST([Created Time] as time) as [time],
	DATEPART(HOUR, [Created Time]) as [hour]
	FROM [fbpa].[PagePostsInfo]
)
GO

  CREATE VIEW fbpa.fb_post_insights AS
(
	SELECT
	[Id] as [id],
	[Entry Name] as [metric],
	CAST([Value] as decimal) as [value],
	[Name] as [name],
	[Period] as [period],
	[Title] as [title],
	[Description] as [description],
		CASE 
		WHEN ([Description] LIKE '%Total Count%') 
		THEN 'Total Count' ELSE 'Unique Users' END as [status],
	SUBSTRING([Id],0,CHARINDEX('/insights',[Id])) as [post_id],
	'insights/' + [Name] + '_' + [Period] as [key_id]
	FROM [fbpa].[PagePostImpressions]
	UNION
		SELECT
		[Id] as [id],
		[Entry Name] as [metric],
		CAST([Value] as decimal) as [value],
		[Name] as [name],
		[Period] as [period],
		[Title] as [title],
		[Description] as [description],
			CASE 
			WHEN ([Description] LIKE '%Total Count%') 
			THEN 'Total Count' ELSE 'Unique Users' END as [status],
		SUBSTRING([Id],0,CHARINDEX('/insights',[Id])) as [post_id],
		'insights/' + [Name] + '_' + [Period] as [key_id]
		FROM [fbpa].[PagePostReactions] 
	UNION
		SELECT
		[Id] as [id],
		[Entry Name] as [metric],
		CAST([Value] as decimal) as [value],
		[Name] as [name],
		[Period] as [period],
		[Title] as [title],
		[Description] as [description],
			CASE 
			WHEN ([Description] LIKE '%Total Count%') 
			THEN 'Total Count' ELSE 'Unique Users' END as [status],
		SUBSTRING([Id],0,CHARINDEX('/insights',[Id])) as [post_id],
		'insights/' + [Name] + '_' + [Period] as [key_id]
		FROM [fbpa].[PageVideoPosts]
)
GO

CREATE VIEW fbpa.[page] AS
(
	SELECT 
	  [idpage] as [idpage],
	  [name] as [name]
	  FROM	[fbpa].[PageTable]
) 
GO

CREATE VIEW fbpa.[calendar] AS 
(
	SELECT 
	full_date as end_time,
	FORMAT(full_date,'M/d/yy') as [Date],
	[year] as [Year],
	[month] as [MonthSort],
	[month_abbrev] as [Month],
	CONCAT('Q',[quarter]) as [Quarter],
	[day_num_in_month] as [Day],
	[day_abbrev] as [Weekday],
	[day_of_week] - 1 as [WeekdaySort],
	CONCAT('W',[week_num_in_year]) as [Week]
	FROM [fbpa].[Date]
)
GO
