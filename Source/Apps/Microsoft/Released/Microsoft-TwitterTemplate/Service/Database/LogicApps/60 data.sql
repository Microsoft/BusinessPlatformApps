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
INSERT pbist_twitter.[configuration] (configuration_group, configuration_subgroup, [name], [value], [visible])
    VALUES (N'SolutionTemplate', N'Twitter', N'version', N'1.0', 0),
           (N'SolutionTemplate', N'Twitter', N'versionImage', N'https://bpstservice.azurewebsites.net/api/telemetry/Microsoft-TwitterTemplate', 1),
           ( N'SolutionTemplate', N'SSAS', N'ProcessOnNextSchedule', N'1', 0),
           ( N'SolutionTemplate', N'SSAS', N'LastProcessedDateTime', N'', 0),
           ( N'SolutionTemplate', N'SSAS', N'LastProcessedStatus', N'', 0),
           ( N'SolutionTemplate', N'SSAS', N'CurrentStatus', N'', 0);
GO


INSERT pbist_twitter.[minimum_tweets] (MinimumTweets) VALUES (1), (2), (3), (4), (5), (10), (20), (50), (100);
go
