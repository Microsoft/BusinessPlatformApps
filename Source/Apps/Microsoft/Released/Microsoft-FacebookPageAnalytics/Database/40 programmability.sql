SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE [fbpa].[Merge]
AS
BEGIN
---------------------Merge Clicks---------------------------------------
BEGIN TRAN

MERGE fbpa.Clicks AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_Clicks) AS SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_Clicks;

COMMIT

---------------------Merge PageContent-------------------------------------
BEGIN TRAN
SET ANSI_NULLS              ON;
MERGE fbpa.PageContent AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageContent) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId)
OUTPUT($action);

TRUNCATE TABLE fbpa.STAGING_PageContent;

COMMIT

---------------------Merge PageEngagement-------------------------------------
BEGIN TRAN

MERGE fbpa.PageEngagement AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageEngagement) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PageEngagement;

COMMIT

---------------------Merge PageImpressions-------------------------------------
BEGIN TRAN

MERGE fbpa.PageImpressions AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageImpressions) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PageImpressions;

COMMIT

---------------------Merge PagePost-------------------------------------
BEGIN TRAN

MERGE fbpa.PagePost AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PagePost) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PagePost;

COMMIT

---------------------Merge PagePostEngagement-------------------------------------
BEGIN TRAN

MERGE fbpa.PagePostEngagement AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PagePostEngagement) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PagePostEngagement;

COMMIT

---------------------Merge PagePostImpressions-------------------------------------
BEGIN TRAN

MERGE fbpa.PagePostImpressions AS TARGET
USING
( 
SELECT DISTINCT
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PagePostImpressions) as SOURCE
ON TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
([Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PagePostImpressions;

COMMIT

---------------------Merge PagePostReactions-------------------------------------
BEGIN TRAN

MERGE fbpa.PagePostReactions AS TARGET
USING
( 
SELECT DISTINCT
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PagePostReactions) as SOURCE
ON TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
([Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PagePostReactions;

COMMIT

---------------------Merge PagePostStoriesAndPeopleTalkingAboutThis-------------------------------------
BEGIN TRAN

MERGE fbpa.PagePostStoriesAndPeopleTalkingAboutThis AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PagePostStoriesAndPeopleTalkingAboutThis) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PagePostStoriesAndPeopleTalkingAboutThis;

COMMIT

---------------------Merge PageReactions-------------------------------------
BEGIN TRAN

MERGE fbpa.PageReactions AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageReactions) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PageReactions;

COMMIT

---------------------Merge PageUserDemographics-------------------------------------
BEGIN TRAN

MERGE fbpa.PageUserDemographics AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageUserDemographics) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PageUserDemographics;

COMMIT

---------------------Merge PageVideoPosts-------------------------------------
BEGIN TRAN

MERGE fbpa.PageVideoPosts AS TARGET
USING
( 
SELECT DISTINCT
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageVideoPosts) as SOURCE
ON  TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
([Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PageVideoPosts;

COMMIT

---------------------Merge PageVideoViews-------------------------------------
BEGIN TRAN

MERGE fbpa.PageVideoViews AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageVideoViews) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PageVideoViews;

COMMIT

---------------------Merge PageViews-------------------------------------
BEGIN TRAN

MERGE fbpa.PageViews AS TARGET
USING
( 
SELECT DISTINCT
EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId
FROM fbpa.STAGING_PageViews) as SOURCE
ON  TARGET.EndTime = SOURCE.EndTime 
AND TARGET.[Name] = SOURCE.[Name]
AND (TARGET.[Entry Name] = SOURCE.[Entry Name] OR (TARGET.[Entry Name] IS NULL AND SOURCE.[Entry Name] IS NULL))
AND (TARGET.[Value] = SOURCE.[Value] OR (TARGET.[Value] IS NULL AND SOURCE.[Value] IS NULL))
AND TARGET.[Period] = SOURCE.[Period]
AND TARGET.[Id] = SOURCE.[Id]
WHEN MATCHED THEN
UPDATE SET
TARGET.EndTime = SOURCE.EndTime,
TARGET.[Name] = SOURCE.[Name],
TARGET.[Entry Name] = SOURCE.[Entry Name],
TARGET.[Value] = SOURCE.[Value],
TARGET.[Period] = SOURCE.[Period],
TARGET.[Id] = SOURCE.[Id]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
(EndTime,
[Name],
[Entry Name],
[Value],
[Period],
[Title],
[Description],
[Id],
PageId)
VALUES
(SOURCE.EndTime,
SOURCE.[Name],
SOURCE.[Entry Name],
SOURCE.[Value],
SOURCE.[Period],
SOURCE.[Title],
SOURCE.[Description],
SOURCE.[Id],
SOURCE.PageId);

TRUNCATE TABLE fbpa.STAGING_PageViews;

COMMIT

---------------------Merge PagePostsTo-------------------------------------
BEGIN TRAN

MERGE fbpa.PagePostsTo AS TARGET
USING
( 
SELECT DISTINCT
[Id],
[PageId],
[Created Time],
[Updated Time],
[To Id],
[To Name]
FROM fbpa.STAGING_PagePostsTo) as SOURCE
ON  TARGET.[Created Time] = SOURCE.[Created Time] 
AND TARGET.[PageId] = SOURCE.[PageId]
AND TARGET.[To Id] = SOURCE.[To Id]
AND TARGET.[To Name] = SOURCE.[To Name]
WHEN MATCHED AND SOURCE.[Updated Time] > TARGET.[Updated Time] THEN
UPDATE SET
TARGET.[Id] = SOURCE.[Id],
TARGET.[PageId] = SOURCE.[PageId],
TARGET.[Created Time] = SOURCE.[Created Time],
TARGET.[Updated Time] = SOURCE.[Updated Time],
TARGET.[To Id] = SOURCE.[To Id],
TARGET.[To Name] = SOURCE.[To Name]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
([Id],
[PageId],
[Created Time],
[Updated Time],
[To Id],
[To Name])
VALUES
(SOURCE.[Id],
SOURCE.[PageId],
SOURCE.[Created Time],
SOURCE.[Updated Time],
SOURCE.[To Id],
SOURCE.[To Name]);

TRUNCATE TABLE fbpa.STAGING_PagePostsTo;

COMMIT

---------------------Merge PagePostsInfo-------------------------------------
BEGIN TRAN

MERGE fbpa.PagePostsInfo AS TARGET
USING
( 
SELECT [Id],
       [PageId],
       MAX([Message]) as [Message],
       [Created Time],
       MAX([Updated Time]) as [Updated Time],
       MAX([Icon]) as [Icon],
       MAX([Story]) as [Story] ,
       MAX([Link]) as [Link],
       MAX([Status Type]) as [Status Type],
       MAX([Is Hidden]) as [Is Hidden],
       MAX([Is Published]) as [Is Published],
       MAX([Name]) as [Name],
       MAX([Object]) as [Object],
       MAX([Permalink URL]) as [Permalink URL],
       MAX([Picture]) as [Picture],
       MAX([Source]) as [Source],
       MAX([Shares]) as [Shares],
       MAX([Type]) as [Type]
FROM fbpa.STAGING_PagePostsInfo
GROUP BY [Id], [PageId], [Created Time]) as SOURCE
ON  TARGET.[Id] = SOURCE.[Id] 
AND (TARGET.[PageId] = SOURCE.[PageId] OR (TARGET.[PageId] IS NULL AND SOURCE.[PageId] IS NULL))
AND (TARGET.[Created Time] = SOURCE.[Created Time]  OR (TARGET.[Created Time] IS NULL AND SOURCE.[Created Time] IS NULL))
AND (TARGET.[Object] = SOURCE.[Object]  OR (TARGET.[Object] IS NULL AND SOURCE.[Object] IS NULL))
AND (TARGET.[Permalink URL] = SOURCE.[Permalink URL]  OR (TARGET.[Permalink URL] IS NULL AND SOURCE.[Permalink URL] IS NULL))
WHEN MATCHED AND SOURCE.[Updated Time] > TARGET.[Updated Time] THEN
UPDATE SET
TARGET.[Id] = SOURCE.[Id],
TARGET.[PageId] = SOURCE.[PageId],
TARGET.[Message] = SOURCE.[Message],
TARGET.[Created Time] = SOURCE.[Created Time],
TARGET.[Updated Time] = SOURCE.[Updated Time],
TARGET.[Icon] = SOURCE.[Icon],
TARGET.[Story] = SOURCE.[Story],
TARGET.[Link] = SOURCE.[Link],
TARGET.[Status Type] = SOURCE.[Status Type],
TARGET.[Is Hidden] = SOURCE.[Is Hidden],
TARGET.[Is Published] = SOURCE.[Is Published],
TARGET.[Name] = SOURCE.[Name],
TARGET.[Object] = SOURCE.[Object],
TARGET.[Permalink URL] = SOURCE.[Permalink URL],
TARGET.[Picture] = SOURCE.[Picture],
TARGET.[Source] = SOURCE.[Source],
TARGET.[Shares] = SOURCE.[Shares],
TARGET.[Type] = SOURCE.[Type]
WHEN NOT MATCHED BY TARGET THEN
INSERT 
([Id],
[PageId],
[Message],
[Created Time],
[Updated Time],
[Icon],
[Story],
[Link],
[Status Type],
[Is Hidden],
[Is Published],
[Name],
[Object],
[Permalink URL],
[Picture],
[Source],
[Shares],
[Type])
VALUES
(SOURCE.[Id],
SOURCE.[PageId],
SOURCE.[Message],
SOURCE.[Created Time],
SOURCE.[Updated Time],
SOURCE.[Icon],
SOURCE.[Story],
SOURCE.[Link],
SOURCE.[Status Type],
SOURCE.[Is Hidden],
SOURCE.[Is Published],
SOURCE.[Name],
SOURCE.[Object],
SOURCE.[Permalink URL],
SOURCE.[Picture],
SOURCE.[Source],
SOURCE.[Shares],
SOURCE.[Type]);


TRUNCATE TABLE fbpa.STAGING_PagePostsInfo;

COMMIT

END
GO
