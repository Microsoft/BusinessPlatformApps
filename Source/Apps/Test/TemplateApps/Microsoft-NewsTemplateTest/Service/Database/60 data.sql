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
INSERT bpst_news.[configuration] (configuration_group, configuration_subgroup, [name], [value],[visible])
    VALUES (N'SolutionTemplate', N'News', N'version', N'1.0', 0),
		   (N'SolutionTemplate', N'News', N'versionImage', N'https://bpstservice.azurewebsites.net/api/telemetry/Microsoft-NewsTemplate', 1);
go
