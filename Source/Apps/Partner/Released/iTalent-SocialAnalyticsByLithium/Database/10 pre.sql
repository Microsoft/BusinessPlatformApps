SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
GO


-- Stored Procedures
DROP PROCEDURE IF EXISTS [it].[SyncData];
GO

-- Drop Tables, if exists
DROP TABLE IF EXISTS [it].[Badges]
DROP TABLE IF EXISTS [it].[Boards]
DROP TABLE IF EXISTS [it].[Categories]
DROP TABLE IF EXISTS [it].[Date]
DROP TABLE IF EXISTS [it].[ETLAudit]
DROP TABLE IF EXISTS [it].[Kudos]
DROP TABLE IF EXISTS [it].[Messages]
DROP TABLE IF EXISTS [it].[Parameters]
DROP TABLE IF EXISTS [it].[References]
DROP TABLE IF EXISTS [it].[STG_Boards]
DROP TABLE IF EXISTS [it].[STG_Categories]
DROP TABLE IF EXISTS [it].[STG_Kudos]
DROP TABLE IF EXISTS [it].[STG_Messages]
DROP TABLE IF EXISTS [it].[STG_UserBadges]
DROP TABLE IF EXISTS [it].[STG_Users]
DROP TABLE IF EXISTS [it].[UserBadges]
DROP TABLE IF EXISTS [it].[Users]
GO


-- Create the schema if not exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name='it')
BEGIN
    EXEC ('CREATE SCHEMA it AUTHORIZATION dbo');
END;
GO
