SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE VIEW [fb].[CommentsPostsView] AS
(

SELECT 
       [Id]
      ,[Created Date]
      ,CONVERT(date, [Created Date]) AS [Created Date Only]
      ,[Message]
      ,[From Id]
      ,[From Name]
      ,[Page]
      ,[PageDisplayName]
      ,[PageId]
      ,[media]
      ,[Id] AS [Post Id]
      ,'Post' AS [Type]
      ,CASE WHEN CONVERT(nvarchar,[From Id]) = [PageId] THEN 'Internal' ELSE 'External' END AS PostType
      ,CASE WHEN CONVERT(nvarchar,[From Id]) = [PageId] THEN '1' ELSE '0' END AS IsPageOwner
  FROM [fb].[Posts]

  UNION ALL 

  SELECT 
       [Id]
      ,[Created Date]
      ,CONVERT(date, [Created Date]) AS [Created Date Only]
      ,[Message]
      ,[From Id]
      ,[From Name]
      ,[Page]
      ,[PageDisplayName]
      ,[PageId]
      ,null AS [media]
      ,[Post Id]
      ,'Comment' AS [Type]
      ,CASE WHEN CONVERT(nvarchar,[From Id]) = [PageId] THEN 'Internal' ELSE 'External' END AS PostType
      ,CASE WHEN CONVERT(nvarchar,[From Id]) = [PageId] THEN '1' ELSE '0' END AS IsPageOwner
  FROM [fb].[Comments]
  )
GO


CREATE VIEW [fb].[HashTagView] AS (

SELECT
	[Id]
    ,[HashTags]
FROM [fb].[HashTags]
)
GO


CREATE VIEW [fb].[KeyPhraseView] AS (
SELECT 
       [Id]
      ,[KeyPhrase]
FROM [fb].[KeyPhrase]
)
GO


CREATE VIEW [fb].[PostsView] AS (
SELECT 
      [Id]
      ,[Total Comments]
	  ,CASE WHEN CONVERT(nvarchar,[From Id]) = [PageId] THEN 'Internal' ELSE 'External' END AS PostType
  FROM [fb].[Posts]
  )

GO

CREATE VIEW [fb].[ReactionsView] AS (
SELECT [Id]
      ,[Reaction Type]
      ,[Count]
FROM [fb].[Reactions]
)
GO 

CREATE VIEW [fb].[SentimentView] AS (
SELECT [Id]
      ,[Sentiment]
FROM [fb].[Sentiment]
)
GO

CREATE VIEW [fb].[UsersView] AS (
SELECT [Id]
      ,[Name]
FROM [fb].[Users]
)
GO

CREATE VIEW [fb].[PagesView] AS (
SELECT DISTINCT 
        [PageId]
      ,[PageDisplayName]
FROM [fb].[Posts]
)
GO

CREATE VIEW [fb].[EdgesView] AS (
SELECT  
     [SourceVertex]
	 ,s.[Name] AS [SourceVertexName] 
    ,[TargetVertex]
	,t.[Name] AS [TargetVertexName] 
    ,[EdgeWeight] 
	,[PageId] 
FROM [fb].[Edges]
JOIN fb.[Users] s ON s.Id = [SourceVertex] 
JOIN fb.[Users] t ON t.Id = [TargetVertex]
)

GO
