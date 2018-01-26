SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE View [vs].[rpt_WorkItemRevision]
AS
SELECT [WorkOrderRevisionId]
      ,[NativeId]
      ,[AreaId]
      ,[AreaPath]
      ,[TeamProject]
      ,[NodeName]
      ,[AreaLevel1]
      ,[RevisionNumber]
      ,[AuthorizedDate]
      ,[RevisedDate]
      ,[IterationId]
      ,REPLACE([IterationPath],'\PI 1','') as [IterationPath]
      ,[IterationLevel1]
      ,[IterationLevel2]
      ,[WorkItemType]
      ,[State]
      ,[Reason]
      ,[AssignedTo]
      ,[CreatedDate]
      ,[CreatedBy]
      ,[ChangedDate]
      ,[ChangedBy]
      ,[AuthorizedAs]
      ,[NativeAuthorizedAsPersonId]
      ,[Watermark]
      ,[AttachedFileCount]
      ,[HyperLinkCount]
      ,[ExternalLinkCount]
      ,[RelatedLinkCount]
      ,[Title]
      ,[BoardColumnDone]
      ,[ActivatedDate]
      ,[ActivatedBy]
      ,[ResolvedDate]
      ,[ResolvedBy]
      ,[Priority]
      ,[StackRank]
      ,[ValueArea]
      ,[BoardColumn]
      ,[StateChangeDate]
      ,[StoryPoints]
      ,[ClosedDate]
      ,[ClosedBy]
      ,[Risk]
      ,[StartDate]
      ,[DueDate]
      ,[Effort]
      ,[Activity]
      ,[RemainingWork]
      ,[OriginalEstimate]
      ,[CompletedWork]
      ,[Severity]
      ,[FinishDate]
      ,[BacklogPriority]
      ,[ETLImportDate]
	  ,CASE WHEN MAX([RevisionNumber]) OVER (Partition by [NativeId]) = [RevisionNumber]
			THEN 1
			Else 0	END as IsLastRevision
	  ,CASE WHEN MAX([RevisionNumber]) OVER (Partition by [NativeId],CONVERT(DATE,[ChangedDate])) = [RevisionNumber]
			THEN 1
			Else 0	END as IsDayLastRevision
	  ,CONVERT(int,CONVERT(char(8),[CreatedDate], 112))		as CreatedDateKey
	  ,CASE WHEN [DueDate] < GETDATE() AND MAX([RevisionNumber]) OVER (Partition by [NativeId]) = [RevisionNumber]
			THEN 1
			ELSE 0 END				as IsPastDue
	  ,CASE WHEN [State] = 'Closed' OR [State] = 'Done' OR [State] = 'Resolved'
			THEN 1
			ELSE 0 END as IsClosed
  FROM [vs].[WorkItemRevision]
go

Create View [vs].[rpt_Date]
AS
SELECT [date_key] as DateId
      ,[full_date] as [Date]
      ,[day_of_week] as [DayOfWeek]
      ,[day_num_in_month] as [DayOfMonth]
      ,[day_name] as [DayName]
      ,[day_abbrev] as DayAbbr
      ,[weekday_flag] as IsWeekday
      ,[week_num_in_year] as WeekOfYear
      ,[week_begin_date] as FirstDayOfWeek
      ,[month] as [Month]
      ,[month_name] as [MonthName]
      ,[month_abbrev] as [MonthAbbr]
      ,[quarter] as [Quarter]
      ,[year] as [Year]
      ,[yearmo] as YYYYMM
	  ,CASE WHEN [full_date] = CONVERT(DATE,GETDATE()) 
			THEN 1 
			ELSE 0 END as IsToday
  FROM [vs].[Date]
go

Create View [vs].[rpt_Iteration]
AS
SELECT d.[Date] as [DateId]
	  ,w.[StartingP]
	  ,w.[TeamProject]
	  ,DATEDIFF(DAY,[StartDate],[FinishDate]) as [Days]
	  ,CONVERT(Decimal(5,2),w.[StartingP]) / CONVERT(Decimal(5,2),DATEDIFF(DAY,[StartDate],[FinishDate])+1) as IdealBurn
	  ,
		w.[StartingP] - 
			SUM(CONVERT(Decimal(5,2),w.[StartingP]) 
			/ CONVERT(Decimal(5,2),DATEDIFF(DAY,[StartDate],[FinishDate])+1)) OVER (
																					PARTITION BY [Path] 
																					ORDER BY d.[Date] 
																					ROWS UNBOUNDED PRECEDING)
			   as IdealTasksRemaining
	  ,ISNULL(c.[Completed],0) as [TasksClosed]
	  ,SUM(ISNULL(c.[Completed],0)) OVER (
										PARTITION BY [Path]
										ORDER BY d.[Date]
										ROWS UNBOUNDED PRECEDING) AS TotalCompleted
	  ,w.[StartingP] - SUM(ISNULL(c.[Completed],0)) OVER (
													PARTITION BY [Path]
													ORDER BY d.[Date]
													ROWS UNBOUNDED PRECEDING) AS ActualTasksRemaining
	  ,[IterationId]
      ,[NativeId]
      ,[Name]
      ,[Path]
      ,[StartDate]
      ,[FinishDate]
      ,[url]
      ,[ETLImportDate]
  FROM [vs].[Iteration] a
	LEFT JOIN [vs].[rpt_Date] d
		ON d.[Date] BETWEEN a.[StartDate] AND a.[FinishDate]
	LEFT JOIN (SELECT 
					w.TeamProject
					,w.[IterationPath]
					,COUNT(DISTINCT NativeId) as StartingP
				FROM [vs].[rpt_WorkItemRevision] w
				GROUP BY 
						w.TeamProject
						,w.[IterationPath]
				) w
		ON a.[Path] = w.[IterationPath]
	LEFT JOIN (SELECT 
					w.[TeamProject]
					,CONVERT(Date,w.[ClosedDate]) as [ClosedDate]
					,w.[IterationPath]
					,COUNT(DISTINCT [NativeId]) as [Completed]
				FROM [vs].[rpt_WorkItemRevision] w
				GROUP BY 
						w.[TeamProject]
						,CONVERT(Date,w.[ClosedDate])
						,[IterationPath]
				) c
		ON a.[Path] = c.[IterationPath]
		AND c.[ClosedDate] = d.[Date]
go

Create View [vs].[rpt_Person]
AS
SELECT [PersonId]
      ,[NativeId]
      ,[UserName]
      ,[UserEmail]
      ,[Name_Email]
      ,[ETLImportDate]
  FROM [vs].[Person]
go

Create View [vs].[rpt_Project]
AS
SELECT [ProjectId]
      ,[NativeId]
      ,[Name]
      ,[Description]
      ,[url]
	  ,LEFT([url],CHARINDEX('/',[url],10)) + REPLACE([Name],' ','%20') + '/_home' as ProjectHomeLink
      ,[State]
      ,[Revision]
      ,[Visibility]
      ,[ETLImportDate]
  FROM [vs].[Project]
go
