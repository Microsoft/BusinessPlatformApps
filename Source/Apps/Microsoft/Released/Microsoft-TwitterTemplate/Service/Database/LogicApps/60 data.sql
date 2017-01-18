SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go


/************************************
* Configuration values              *
*************************************/
INSERT pbist_twitter.[configuration] (configuration_group, configuration_subgroup, [name], [value],[visible])
    VALUES (N'SolutionTemplate', N'Twitter', N'version', N'1.0', 0),
		   (N'SolutionTemplate', N'Twitter', N'versionImage', N'https://bpstservice.azurewebsites.net/api/telemetry/Microsoft-TwitterTemplate', 1);
go

INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (0)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (1)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (2)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (3)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (4)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (5)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (10)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (20)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (50)
go
INSERT [pbist_twitter].[minimum_tweets] ([MinimumTweets]) VALUES (100)
go
