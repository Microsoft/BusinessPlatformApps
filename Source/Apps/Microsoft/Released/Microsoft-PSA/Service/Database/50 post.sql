SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

/*
 Tables to drop
*/
DROP TABLE dbo.systemuser;
DROP TABLE dbo.StatusMetadata;
DROP TABLE dbo.StateMetadata;
DROP TABLE dbo.salesorderdetail;
DROP TABLE dbo.salesorder;
DROP TABLE dbo.quotedetail;
DROP TABLE dbo.quote;
DROP TABLE dbo.OptionSetMetadata;
DROP TABLE dbo.opportunity;
DROP TABLE dbo.msdyn_transactioncategory;
DROP TABLE dbo.msdyn_timeentry;
DROP TABLE dbo.msdyn_resourcerequirementdetail;
DROP TABLE dbo.msdyn_resourcerequirement;
DROP TABLE dbo.msdyn_resourcerequest;
DROP TABLE dbo.msdyn_project;
DROP TABLE dbo.msdyn_projecttask;
DROP TABLE dbo.msdyn_organizationalunit;
DROP TABLE dbo.msdyn_orderlineresourcecategory;
DROP TABLE dbo.msdyn_estimateline;
DROP TABLE dbo.msdyn_actual;
DROP TABLE dbo.GlobalOptionSetMetadata;
DROP TABLE dbo.bookingstatus;
DROP TABLE dbo.bookableresourcecategoryassn;
DROP TABLE dbo.bookableresourcecategory;
DROP TABLE dbo.bookableresourcebooking;
DROP TABLE dbo.bookableresource;
DROP TABLE dbo.account;


/*
 Tables to truncate
*/
-- TRUNCATE TABLE ...;