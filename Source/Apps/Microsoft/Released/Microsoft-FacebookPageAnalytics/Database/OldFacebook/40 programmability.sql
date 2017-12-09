SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE [fb].[Merge]
AS
BEGIN

---------------------Merge Users-------------------------------------
BEGIN TRAN

DELETE t 
FROM fb.[Users] as t
INNER JOIN fb.StagingComments s 
ON s.[From Id] = t.[Id];
INSERT INTO fb.[Users]
(
     [Id]
	,[Name]
)
SELECT DISTINCT
	[From Id] AS [Id]
	,[From Name] AS [Name]
 FROM fb.StagingComments

 DELETE t 
FROM fb.[Users] as t
INNER JOIN fb.StagingPosts s 
ON s.[From Id] = t.[Id];
INSERT INTO fb.[Users]
(
     [Id]
	,[Name]
)
SELECT DISTINCT
	[From Id] AS [Id]
	,[From Name] AS [Name]
 FROM fb.StagingPosts

 COMMIT

---------------------Merge Comments-------------------------------------
BEGIN TRAN
DELETE t 
FROM fb.[Comments] as t
INNER JOIN fb.StagingComments s 
ON t.Id = s.Id;


INSERT INTO fb.[Comments]
(
	 [Id]
	,[Created Date]
	,[Message]
	,[From Id]
	,[From Name]
	,[Post Id]
	,[Page]
    ,[PageDisplayName]
    ,[PageId]
)
SELECT DISTINCT
	[Id]
	,[Created Date]
	,[Message]
	,[From Id]
	,[From Name]
	,[Post Id]
	,[Page]
    ,[PageDisplayName]
    ,[PageId]
 FROM fb.StagingComments
    
TRUNCATE TABLE [fb].[StagingComments];	 
COMMIT
---------------------Merge HashTags-------------------------------------

BEGIN TRAN
DELETE t 
FROM fb.HashTags as t
INNER JOIN fb.StagingHashTags s 
ON t.Id = s.Id;

INSERT INTO [fb].[HashTags] 
(
	[Id]
	,[HashTags]
)
SELECT DISTINCT   
	[Id]
	,[HashTags]
FROM [fb].[StagingHashTags];

TRUNCATE TABLE [fb].[StagingHashTags];
COMMIT
---------------------Merge KeyPhrases-------------------------------------

BEGIN TRAN
DELETE t 
FROM fb.KeyPhrase as t
INNER JOIN fb.StagingKeyPhrase s 
ON t.Id = s.Id;
INSERT INTO [fb].[KeyPhrase] 
(
	[Id]
	,[KeyPhrase]
)
    
SELECT DISTINCT  
		[Id]
		,[KeyPhrase]
	FROM [fb].[StagingKeyPhrase];

TRUNCATE TABLE [fb].[StagingKeyPhrase];	 	 
COMMIT
---------------------Merge Posts-------------------------------------
BEGIN TRAN
DELETE t 
FROM fb.Posts as t
INNER JOIN fb.StagingPosts s 
ON t.Id = s.Id;

INSERT INTO fb.Posts
(
	 [Id]
	,[Created Date]
	,[Message]
	,[From Id]
	,[From Name]
	,[Media]
	,[Page]
    ,[PageDisplayName]
    ,[PageId]
    ,[Total Comments]
)
SELECT DISTINCT
	 [Id]
	,[Created Date]
	,[Message]
	,[From Id]
	,[From Name]
	,[Media]
	,[Page]
    ,[PageDisplayName]
    ,[PageId]
	,[Total Comments]
    FROM fb.StagingPosts
TRUNCATE TABLE [fb].StagingPosts;
COMMIT

---------------------Merge Reactions-------------------------------------
BEGIN TRAN
DELETE t 
FROM [fb].[Reactions] as t
INNER JOIN [fb].[StagingReactions] s 
ON t.Id = s.Id;

INSERT INTO [fb].[Reactions]
(
	[Id]
	,[Reaction Type]
	,[Count]
)
SELECT DISTINCT  
	[Id]
	,[Reaction Type]
	,[Count]
FROM [fb].[StagingReactions]
TRUNCATE TABLE [fb].[StagingReactions];
COMMIT

---------------------Merge Sentiment-------------------------------------
BEGIN TRAN
DELETE t 
FROM [fb].[Sentiment] as t
INNER JOIN [fb].[StagingSentiment] s 
ON t.Id = s.Id;
    
INSERT INTO [fb].[Sentiment]
(
	[Id]
	,[Sentiment]
)
SELECT DISTINCT
	 [Id]
	,[Sentiment]
FROM [fb].[StagingSentiment]
TRUNCATE TABLE [fb].[StagingSentiment]; 
COMMIT

END
go

CREATE PROCEDURE [fb].[UpdateEdges]
AS
BEGIN

BEGIN TRAN
TRUNCATE TABLE fb.[Edges];

INSERT INTO fb.[Edges]
(
     SourceVertex
    ,TargetVertex
    ,EdgeWeight
    ,PageId
)
(
    SELECT 
    tbl1.[From Id] as SourceVertex, 
    tbl2.[From Id] as TargetVertex,
    count(1) as EdgeWeight,
    tbl1.[PageId]  as PageId
    FROM [fb].Comments tbl1 join [fb].Comments tbl2 on tbl1.[Post Id] = tbl2.[Post Id]
    WHERE tbl1.[From Id] !=  tbl1.[PageId] and tbl2.[From Id] !=  tbl2.[PageId] and tbl1.[From Id] != tbl2.[From Id] 
    group by tbl1.[From Id], tbl2.[From Id], tbl1.[PageId]
    having count(1) > 2 
);

COMMIT

END
GO
