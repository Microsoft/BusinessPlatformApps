SET ANSI_NULLS              ON;
SET ANSI_PADDING            ON;
SET ANSI_WARNINGS           ON;
SET ANSI_NULL_DFLT_ON       ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER       ON;
go

CREATE PROCEDURE pbist_sccm.sp_get_last_updatetime AS
BEGIN
    SET NOCOUNT ON;

    SELECT CONVERT(datetime, [value], 126) AS lastLoadTimestamp FROM pbist_sccm.[configuration] WHERE name='lastLoadTimestamp' AND configuration_group='SolutionTemplate' AND configuration_subgroup='System Center';
END;
GO

CREATE PROCEDURE pbist_sccm.sp_get_replication_counts AS
BEGIN
    SET NOCOUNT ON;

    SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; -- it's ok for these to be somewhat inaccurate
     
    WITH TableCounts(EntityName, [Count]) AS
    (
        SELECT ta.[name] AS EntityName, SUM(pa.[rows]) AS [Count]
        FROM sys.tables ta INNER JOIN sys.partitions pa ON pa.[OBJECT_ID] = ta.[OBJECT_ID]
                           INNER JOIN sys.schemas sc ON ta.[schema_id] = sc.[schema_id]
        WHERE
            sc.name='pbist_sccm' AND ta.is_ms_shipped = 0 AND pa.index_id IN (0,1) AND
            ta.name IN ('clienthealthclientinstalledversiondetails_staging', 'clienthealthclientsinventorystatisticslast30days_staging', 'clienthealthcountassignedclientsbysite_staging', 'clienthealthcountclientsinstalledversion_staging', 'clienthealthcountclientsinstalledversionpersite_staging', 'clienthealthevaluationcount_staging', 'clienthealthsummary_staging', 'clienthealthwuaversionallclients_staging', 'collection_staging', 'compliancesettingcompliancebycomputers_staging', 'compliancesettingcomplianceciforallbaseline_staging', 'componentstatus_staging', 'computer_staging', 'computercollection_staging', 'computermalware_staging', 'computerprogram_staging', 'computerupdate_staging', 'configmgrserverhealthbackupstatus_staging', 'configmgrserverhealthcomponentstatusdetails_staging', 'configmgrserverhealthcountcomponentstatusmessagesbyseverity_staging', 'distributionpointscompliancebydpstatus_staging', 'distributionpointscompliancebypackagesstatus_staging', 'distributionpointsconfigurationstatus_staging', 'distributionpointsstatus_staging', 'malware_staging', 'overallcompliancebaseline_staging', 'program_staging', 'replicationstatus_staging', 'replicationstatusdetails_staging', 'replicationstatusreplicationgroup_staging', 'scanhistory_staging', 'site_staging', 'softwaredistribution_staging', 'softwaredistributionoverallapplicationdeployment_staging', 'softwaredistributionoverallpackagedeployment_staging', 'softwaredistributionoverallpckappdpdistribution_staging', 'tasksequencedeployment_staging', 'tasksequencedpcontentstatusreferencedpackagestasksequence_staging', 'tasksequenceoveralltasksequencebycomputerdeployment_staging', 'tasksequenceoveralltasksequencebytasksequencedeployment_staging', 'update_staging', 'user_staging', 'usercomputer_staging')
        GROUP BY ta.[name]
    )
    SELECT UPPER(LEFT(EntityName, 1)) + LOWER(SUBSTRING(EntityName, 2, 100)) AS EntityName, [Count] FROM TableCounts;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_get_prior_content AS
BEGIN
    SET NOCOUNT ON;

    SELECT Count(*) AS ExistingObjectCount
    FROM   INFORMATION_SCHEMA.TABLES
    WHERE  table_schema = 'pbist_sccm' AND
           table_name IN ('clienthealthclientinstalledversiondetails', 'clienthealthclientsinventorystatisticslast30days', 'clienthealthcountassignedclientsbysite', 'clienthealthcountclientsinstalledversion', 'clienthealthcountclientsinstalledversionpersite', 'clienthealthevaluationcount', 'clienthealthsummary', 'clienthealthwuaversionallclients', 'collection', 'compliancesettingcompliancebycomputers', 'compliancesettingcomplianceciforallbaseline', 'componentstatus', 'computer', 'computercollection', 'computermalware', 'computerprogram', 'computerupdate', 'configmgrserverhealthbackupstatus', 'configmgrserverhealthcomponentstatusdetails', 'configmgrserverhealthcountcomponentstatusmessagesbyseverity', 'configuration', 'date', 'distributionpointscompliancebydpstatus', 'distributionpointscompliancebypackagesstatus', 'distributionpointsconfigurationstatus', 'distributionpointsstatus', 'malware', 'operatingsystem', 'overallcompliancebaseline', 'program', 'replicationstatus', 'replicationstatusdetails', 'replicationstatusreplicationgroup', 'scanhistory', 'site', 'softwaredistribution', 'softwaredistributionoverallapplicationdeployment', 'softwaredistributionoverallpackagedeployment', 'softwaredistributionoverallpckappdpdistribution', 'tasksequencedeployment', 'tasksequencedpcontentstatusreferencedpackagestasksequence', 'tasksequenceoveralltasksequencebycomputerdeployment', 'tasksequenceoveralltasksequencebytasksequencedeployment', 'update', 'user', 'usercomputer',
                          'clienthealthclientinstalledversiondetails_staging', 'clienthealthclientsinventorystatisticslast30days_staging', 'clienthealthcountassignedclientsbysite_staging', 'clienthealthcountclientsinstalledversion_staging', 'clienthealthcountclientsinstalledversionpersite_staging', 'clienthealthevaluationcount_staging', 'clienthealthsummary_staging', 'clienthealthwuaversionallclients_staging', 'collection_staging', 'compliancesettingcompliancebycomputers_staging', 'compliancesettingcomplianceciforallbaseline_staging', 'componentstatus_staging', 'computer_staging', 'computercollection_staging', 'computermalware_staging', 'computerprogram_staging', 'computerupdate_staging', 'configmgrserverhealthbackupstatus_staging', 'configmgrserverhealthcomponentstatusdetails_staging', 'configmgrserverhealthcountcomponentstatusmessagesbyseverity_staging', 'distributionpointscompliancebydpstatus_staging', 'distributionpointscompliancebypackagesstatus_staging', 'distributionpointsconfigurationstatus_staging', 'distributionpointsstatus_staging', 'malware_staging', 'overallcompliancebaseline_staging', 'program_staging', 'replicationstatus_staging', 'replicationstatusdetails_staging', 'replicationstatusreplicationgroup_staging', 'scanhistory_staging', 'site_staging', 'softwaredistribution_staging', 'softwaredistributionoverallapplicationdeployment_staging', 'softwaredistributionoverallpackagedeployment_staging', 'softwaredistributionoverallpckappdpdistribution_staging', 'tasksequencedeployment_staging', 'tasksequencedpcontentstatusreferencedpackagestasksequence_staging', 'tasksequenceoveralltasksequencebycomputerdeployment_staging', 'tasksequenceoveralltasksequencebytasksequencedeployment_staging', 'update_staging', 'user_staging', 'usercomputer_staging');
END;
GO


CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthClientInstalledVersionDetails AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthClientInstalledVersionDetails;
	INSERT INTO pbist_sccm.ClientHealthClientInstalledVersionDetails SELECT * FROM pbist_sccm.ClientHealthClientInstalledVersionDetails_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthClientInstalledVersionDetails_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthClientsinventorystatisticslast30days AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthClientsinventorystatisticslast30days;
	INSERT INTO pbist_sccm.ClientHealthClientsinventorystatisticslast30days SELECT * FROM pbist_sccm.ClientHealthClientsinventorystatisticslast30days_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthClientsinventorystatisticslast30days_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthCountAssignedClientsBySite AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthCountAssignedClientsBySite;
	INSERT INTO pbist_sccm.ClientHealthCountAssignedClientsBySite SELECT * FROM pbist_sccm.ClientHealthCountAssignedClientsBySite_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthCountAssignedClientsBySite_staging;
	COMMIT;
END;

GO

CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthCountClientsInstalledVersion AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthCountClientsInstalledVersion;
	INSERT INTO pbist_sccm.ClientHealthCountClientsInstalledVersion SELECT * FROM pbist_sccm.ClientHealthCountClientsInstalledVersion_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthCountClientsInstalledVersion_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthCountClientsInstalledVersionPerSite AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthCountClientsInstalledVersionPerSite;
	INSERT INTO pbist_sccm.ClientHealthCountClientsInstalledVersionPerSite SELECT * FROM pbist_sccm.ClientHealthCountClientsInstalledVersionPerSite_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthCountClientsInstalledVersionPerSite_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthEvaluationCount AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthEvaluationCount;
	INSERT INTO pbist_sccm.ClientHealthEvaluationCount SELECT * FROM pbist_sccm.ClientHealthEvaluationCount_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthEvaluationCount_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthSummary AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthSummary;
	INSERT INTO pbist_sccm.ClientHealthSummary SELECT * FROM pbist_sccm.ClientHealthSummary_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthSummary_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ClientHealthWUAVersionAllClients AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ClientHealthWUAVersionAllClients;
	INSERT INTO pbist_sccm.ClientHealthWUAVersionAllClients SELECT * FROM pbist_sccm.ClientHealthWUAVersionAllClients_staging;
	TRUNCATE TABLE pbist_sccm.ClientHealthWUAVersionAllClients_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ComplianceSettingComplianceByComputers AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ComplianceSettingComplianceByComputers;
	INSERT INTO pbist_sccm.ComplianceSettingComplianceByComputers SELECT * FROM pbist_sccm.ComplianceSettingComplianceByComputers_staging;
	TRUNCATE TABLE pbist_sccm.ComplianceSettingComplianceByComputers_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ComplianceSettingComplianceCIForAllBaseline AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ComplianceSettingComplianceCIForAllBaseline;
	INSERT INTO pbist_sccm.ComplianceSettingComplianceCIForAllBaseline SELECT * FROM pbist_sccm.ComplianceSettingComplianceCIForAllBaseline_staging;
	TRUNCATE TABLE pbist_sccm.ComplianceSettingComplianceCIForAllBaseline_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ComponentStatus AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ComponentStatus;
	INSERT INTO pbist_sccm.ComponentStatus SELECT * FROM pbist_sccm.ComponentStatus_staging;
	TRUNCATE TABLE pbist_sccm.ComponentStatus_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ConfigMgrServerHealthBackupStatus AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ConfigMgrServerHealthBackupStatus;
	INSERT INTO pbist_sccm.ConfigMgrServerHealthBackupStatus SELECT * FROM pbist_sccm.ConfigMgrServerHealthBackupStatus_staging;
	TRUNCATE TABLE pbist_sccm.ConfigMgrServerHealthBackupStatus_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ConfigMgrServerHealthComponentStatusDetails AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ConfigMgrServerHealthComponentStatusDetails;
	INSERT INTO pbist_sccm.ConfigMgrServerHealthComponentStatusDetails SELECT * FROM pbist_sccm.ConfigMgrServerHealthComponentStatusDetails_staging;
	TRUNCATE TABLE pbist_sccm.ConfigMgrServerHealthComponentStatusDetails_staging;
	COMMIT;
END;

GO

CREATE PROCEDURE pbist_sccm.sp_populate_ConfigMgrServerHealthCountComponentStatusMessagesBySeverity AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ConfigMgrServerHealthCountComponentStatusMessagesBySeverity;
	INSERT INTO pbist_sccm.ConfigMgrServerHealthCountComponentStatusMessagesBySeverity SELECT * FROM pbist_sccm.ConfigMgrServerHealthCountComponentStatusMessagesBySeverity_staging;
	TRUNCATE TABLE pbist_sccm.ConfigMgrServerHealthCountComponentStatusMessagesBySeverity_staging;
	COMMIT;
END;

GO


CREATE PROCEDURE pbist_sccm.sp_populate_DistributionPoints AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    TRUNCATE TABLE pbist_sccm.DistributionPoints;

    INSERT INTO pbist_sccm.DistributionPoints(failed, inprogress, success)
    SELECT failed, inprogress, success
    FROM pbist_sccm.DistributionPoints_staging;

    TRUNCATE TABLE pbist_sccm.DistributionPoints_staging;
    
    COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_DistributionPointsCompliancebyDPStatus AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.DistributionPointsCompliancebyDPStatus;
	INSERT INTO pbist_sccm.DistributionPointsCompliancebyDPStatus SELECT * FROM pbist_sccm.DistributionPointsCompliancebyDPStatus_staging;
	TRUNCATE TABLE pbist_sccm.DistributionPointsCompliancebyDPStatus_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_DistributionPointsCompliancebyPackagesStatus AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.DistributionPointsCompliancebyPackagesStatus;
	INSERT INTO pbist_sccm.DistributionPointsCompliancebyPackagesStatus SELECT * FROM pbist_sccm.DistributionPointsCompliancebyPackagesStatus_staging;
	TRUNCATE TABLE pbist_sccm.DistributionPointsCompliancebyPackagesStatus_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_DistributionPointsConfigurationStatus AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.DistributionPointsConfigurationStatus;
	INSERT INTO pbist_sccm.DistributionPointsConfigurationStatus SELECT * FROM pbist_sccm.DistributionPointsConfigurationStatus_staging;
	TRUNCATE TABLE pbist_sccm.DistributionPointsConfigurationStatus_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_DistributionPointsStatus AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.DistributionPointsStatus;
	INSERT INTO pbist_sccm.DistributionPointsStatus SELECT * FROM pbist_sccm.DistributionPointsStatus_staging;
	TRUNCATE TABLE pbist_sccm.DistributionPointsStatus_staging;
	COMMIT;
END;

GO

CREATE PROCEDURE pbist_sccm.sp_populate_OverallComplianceBaseline AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.OverallComplianceBaseline;
	INSERT INTO pbist_sccm.OverallComplianceBaseline SELECT * FROM pbist_sccm.OverallComplianceBaseline_staging;
	TRUNCATE TABLE pbist_sccm.OverallComplianceBaseline_staging;
	COMMIT;
END;

GO

CREATE PROCEDURE pbist_sccm.sp_populate_ReplicationStatus AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ReplicationStatus;
	INSERT INTO pbist_sccm.ReplicationStatus SELECT * FROM pbist_sccm.ReplicationStatus_staging;
	TRUNCATE TABLE pbist_sccm.ReplicationStatus_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ReplicationStatusDetails AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ReplicationStatusDetails;
	INSERT INTO pbist_sccm.ReplicationStatusDetails SELECT * FROM pbist_sccm.ReplicationStatusDetails_staging;
	TRUNCATE TABLE pbist_sccm.ReplicationStatusDetails_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_ReplicationStatusReplicationGroup AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.ReplicationStatusReplicationGroup;
	INSERT INTO pbist_sccm.ReplicationStatusReplicationGroup SELECT * FROM pbist_sccm.ReplicationStatusReplicationGroup_staging;
	TRUNCATE TABLE pbist_sccm.ReplicationStatusReplicationGroup_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_SoftwareDistribution AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.SoftwareDistribution;
	INSERT INTO pbist_sccm.SoftwareDistribution SELECT * FROM pbist_sccm.SoftwareDistribution_staging;
	TRUNCATE TABLE pbist_sccm.SoftwareDistribution_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_SoftwareDistributionOverallApplicationDeployment AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.SoftwareDistributionOverallApplicationDeployment;
	INSERT INTO pbist_sccm.SoftwareDistributionOverallApplicationDeployment SELECT * FROM pbist_sccm.SoftwareDistributionOverallApplicationDeployment_staging;
	TRUNCATE TABLE pbist_sccm.SoftwareDistributionOverallApplicationDeployment_staging;
	COMMIT;
END;

GO

CREATE PROCEDURE pbist_sccm.sp_populate_SoftwareDistributionOverallPackageDeployment AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.SoftwareDistributionOverallPackageDeployment;
	INSERT INTO pbist_sccm.SoftwareDistributionOverallPackageDeployment SELECT * FROM pbist_sccm.SoftwareDistributionOverallPackageDeployment_staging;
	TRUNCATE TABLE pbist_sccm.SoftwareDistributionOverallPackageDeployment_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_SoftwareDistributionOverallPckAPPDPdistribution AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.SoftwareDistributionOverallPckAPPDPdistribution;
	INSERT INTO pbist_sccm.SoftwareDistributionOverallPckAPPDPdistribution SELECT * FROM pbist_sccm.SoftwareDistributionOverallPckAPPDPdistribution_staging;
	TRUNCATE TABLE pbist_sccm.SoftwareDistributionOverallPckAPPDPdistribution_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_TaskSequenceDeployment AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.TaskSequenceDeployment;
	INSERT INTO pbist_sccm.TaskSequenceDeployment SELECT * FROM pbist_sccm.TaskSequenceDeployment_staging;
	TRUNCATE TABLE pbist_sccm.TaskSequenceDeployment_staging;
	COMMIT;
END;

GO

CREATE PROCEDURE pbist_sccm.sp_populate_TasksequenceDPContentStatusReferencedPackagesTaskSequence AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.TasksequenceDPContentStatusReferencedPackagesTaskSequence;
	INSERT INTO pbist_sccm.TasksequenceDPContentStatusReferencedPackagesTaskSequence SELECT * FROM pbist_sccm.TasksequenceDPContentStatusReferencedPackagesTaskSequence_staging;
	TRUNCATE TABLE pbist_sccm.TasksequenceDPContentStatusReferencedPackagesTaskSequence_staging;
	COMMIT;
END;

GO

CREATE PROCEDURE pbist_sccm.sp_populate_TasksequenceOverallTaskSequenceByComputerDeployment AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.TasksequenceOverallTaskSequenceByComputerDeployment;
	INSERT INTO pbist_sccm.TasksequenceOverallTaskSequenceByComputerDeployment SELECT * FROM pbist_sccm.TasksequenceOverallTaskSequenceByComputerDeployment_staging;
	TRUNCATE TABLE pbist_sccm.TasksequenceOverallTaskSequenceByComputerDeployment_staging;
	COMMIT;
END;
GO

CREATE PROCEDURE pbist_sccm.sp_populate_TasksequenceOverallTaskSequencebyTaskSequenceDeployment AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.TasksequenceOverallTaskSequencebyTaskSequenceDeployment;
	INSERT INTO pbist_sccm.TasksequenceOverallTaskSequencebyTaskSequenceDeployment SELECT * FROM pbist_sccm.TasksequenceOverallTaskSequencebyTaskSequenceDeployment_staging;
	TRUNCATE TABLE pbist_sccm.TasksequenceOverallTaskSequencebyTaskSequenceDeployment_staging;
	COMMIT;
END;
GO

-- =============================================
-- Description: Move the rows from  Collection_staging to Collection.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populatecollection AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    TRUNCATE TABLE pbist_sccm.collection;

    INSERT INTO pbist_sccm.collection(collectionid, [collection name])
    SELECT collectionid, [collection name]
    FROM pbist_sccm.collection_staging;

    TRUNCATE TABLE pbist_sccm.collection_staging;
    COMMIT;

END;
GO

CREATE PROCEDURE pbist_sccm.sp_populatecomputer AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;


    -- Mark rows as deleted not found in the staging table
    UPDATE pbist_sccm.computer
    SET    [deleted date] = Getdate()
    FROM   pbist_sccm.computer
    WHERE  pbist_sccm.computer.[deleted date] IS NULL AND
            pbist_sccm.computer.machineid NOT IN (SELECT DISTINCT machineid FROM pbist_sccm.computer_staging)

    -- Merge the rest.
    MERGE pbist_sccm.computer AS Target
    using (SELECT machineid,
                sitecode,
                name,
                [operating system],
                [client type],
                manufacturer,
                model,
                [platform],
                [physical memory]
            FROM   pbist_sccm.computer_staging) AS Source
    ON ( Target.machineid = source.machineid )
    WHEN matched AND ( Target.sitecode <> Source.sitecode OR Target.name <> Source.name OR Target.[operating system] <>
    Source.[operating system]
    OR Target.[client type] <> Source.[client type] OR Target.manufacturer <> Source.manufacturer OR Target.model <> Source.model OR
    Target.[platform] <> Source.[platform] OR Target.[physical memory] <> Source.[physical memory] ) THEN
    UPDATE SET Target.sitecode = Source.sitecode,
                Target.name = Source.name,
                Target.[operating system] = Source.[operating system],
                Target.[client type] = Source.[client type],
                Target.manufacturer = Source.manufacturer,
                Target.model = Source.model,
                Target.[platform] = Source.[platform],
                Target.[physical memory] = Source.[physical memory],
				Target.[deleted date] = NULL
    WHEN NOT matched BY target THEN
    INSERT ( machineid,
                sitecode,
                name,
                [operating system],
                [client type],
                manufacturer,
                model,
                [platform],
                [physical memory])
    VALUES ( Source.machineid,
                Source.sitecode,
                Source.name,
                Source.[operating system],
                Source.[client type],
                Source.manufacturer,
                Source.model,
                Source.[platform],
                Source.[physical memory] );

    TRUNCATE TABLE pbist_sccm.computer_staging;

    COMMIT;
END;
GO

-- =============================================
-- Description: Move the rows from  ComputerCollection_staging to ComputerCollection.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populatecomputercollection AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    TRUNCATE TABLE pbist_sccm.computercollection;

    INSERT INTO pbist_sccm.computercollection(collectionid,resourceid)
    SELECT collectionid, resourceid
    FROM pbist_sccm.computercollection_staging;

    TRUNCATE TABLE pbist_sccm.computercollection_staging;
    
    COMMIT;
END;
GO

-- =============================================
-- Description: Merges rows from ComputerMalware_staging into ComputerMalware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populatecomputermalware AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @retention int;

    BEGIN TRANSACTION;

    -- Get the configured value for data retention from configuration table
    SELECT @retention = value
    FROM pbist_sccm.vw_configuration
    WHERE name = 'dataretentiondays';

    -- IF null, use the default day (60 days)
    SET @retention = Coalesce(@retention, 60);

    DECLARE @mindate DATE = DATEADD(d,-@retention,GETDATE());
    
    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.computermalware;

    -- Copy the contents from the staging table
    INSERT INTO pbist_sccm.computermalware
                (threatid,
                 machineid,
                 [detection date],
                 [observer product name],
                 [observer product version],
                 [observer detection],
                 [remediation type],
                 [remediation result],
                 [remediation error code],
                 [remediation pending action],
                 [is active malware])
    SELECT threatid,
           machineid,
           [detection date],
           [observer product name],
           [observer product version],
           [observer detection],
           [remediation type],
           [remediation result],
           [remediation error code],
           [remediation pending action],
           [is active malware]
    FROM   pbist_sccm.computermalware_staging
    WHERE  [detection date] >= @mindate;

    TRUNCATE TABLE pbist_sccm.computermalware_staging;

    COMMIT;
END;
GO

-- ============================================= 
-- Description: Merges rows from ComputerProgram_staging into ComputerProgram.
CREATE PROCEDURE pbist_sccm.sp_populatecomputerprogram AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @result_code INT;

    -- Logically, this is a simple insert from staging. But it's a lot of data. Drop + rename is much faster
    DROP TABLE pbist_sccm.computerprogram;
    EXEC @result_code = sp_rename 'pbist_sccm.computerprogram_staging', 'computerprogram', 'OBJECT';
    
    -- The * should obey the column order used when the table was created
    SELECT TOP 0 * INTO pbist_sccm.computerprogram_staging FROM pbist_sccm.computerprogram;
END
GO

-- =============================================
-- Description: Merges rows from ComputerMalware_staging into ComputerMalware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populatecomputerupdate AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @retention int;

    BEGIN TRANSACTION;

    -- Get the configured value for data retention from configuration table
    SELECT @retention = value FROM pbist_sccm.vw_configuration WHERE name='dataretentiondays' AND [configuration group]='SolutionTemplate' AND [configuration subgroup]='System Center';
    -- IF null, use the default day (60 days)
    SET @retention = Coalesce(@retention, 60);

    DECLARE @mindate DATE = DATEADD(d,-@retention,GETDATE());
    
    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.computerupdate;
    
    -- Copy the contents from the staging table
    INSERT INTO pbist_sccm.computerupdate
                (machineid,
                ci_id,
                laststatuschangetime,
                laststatuschecktime,
                status)
    SELECT machineid,
            ci_id,
            laststatuschangetime,
            laststatuschecktime,
            status
    FROM   pbist_sccm.computerupdate_staging
    WHERE  laststatuschangetime >= @mindate;

    TRUNCATE TABLE pbist_sccm.computerupdate_staging;

    COMMIT;
END;
GO

-- =============================================
-- Description: Merges rows from Malware_staging into Malware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populatemalware AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.malware;

    -- Copy the contents from the staging table
    INSERT INTO pbist_sccm.malware
                (threatid,
                [malware name],
                [malware severity],
                [malware category])
    SELECT threatid,
            [malware name],
            [malware severity],
            [malware category]
    FROM   pbist_sccm.malware_staging;

    TRUNCATE TABLE pbist_sccm.malware_staging;

    COMMIT;
END ;
GO

-- =============================================
-- Description: Merges rows from Program_staging into Malware. This is a simple truncate and insert.
CREATE PROCEDURE pbist_sccm.sp_populateprogram AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    -- Empty the original table
    TRUNCATE TABLE pbist_sccm.program;
    INSERT INTO pbist_sccm.program([program name], publisher, [version])  SELECT [program name], publisher, [version] FROM pbist_sccm.program_staging;
    TRUNCATE TABLE pbist_sccm.program_staging;

    COMMIT;
END;
GO

-- =============================================
-- Description: Append the rows from ScanHistory_staging to ScanHistory. This is a simply appending rows with the recent date.
CREATE PROCEDURE pbist_sccm.sp_populatescanhistory
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @retention INT;

    BEGIN TRANSACTION;
    -- Get the configured value for data retention from configuration table
    SELECT @retention = value FROM pbist_sccm.vw_configuration WHERE name='dataretentiondays';

    -- IF null, use the default day (60 days)
    SET @retention = COALESCE(@retention, 60);

    DECLARE @mindate DATE = Dateadd(d, -@retention, Getdate());
    DECLARE @mindatekey INT = 10000 * Year(@mindate) + 100 * Month(@mindate) + Day(@mindate);

    MERGE INTO pbist_sccm.scanhistory dest
    USING (SELECT machineid,
                date_key,
                sitecode,
                [client type],
                [enabled],
                [client version],
                [real time protection enabled],
                [on access protection enabled],
                [input/output protection enabled],
                [behavior monitor enabled],
                [antivirus enabled],
                [antispyware enabled],
                [nis enabled],
                [quick scan age (days)],
                [full scan age (days)],
                [signature age (days)],
                [engine version],
                [antivirus signature version],
                [missing important update count],
                [missing critical update count],
                [client active status],
                [health evaluation result],
                [health evaluation],
                [last online],
                [health status message],
                [client state],
                [last ddr],
                [last hw],
                [last sw],
                [last status message],
                [last policy request],
                [last scan time]
            FROM   pbist_sccm.scanhistory_staging) AS src(machineid, date_key, sitecode, [client type], [enabled], [client version],
                                                        [real time protection enabled], [on access protection enabled], [input/output protection enabled], [behavior monitor enabled],
                                                        [antivirus enabled], [antispyware enabled], [nis enabled], [quick scan age (days)], [full scan age (days)], [signature age (days)],
                                                        [engine version], [antivirus signature version], [missing important update count], [missing critical update count], [client active status],
                                                        [health evaluation result], [health evaluation], [last online], [health status message], [client state], [last ddr], [last hw], [last sw],
                                                        [last status message], [last policy request], [last scan time])
    ON dest.machineid = src.machineid AND dest.date_key = src.date_key
    WHEN matched THEN
    UPDATE SET machineid = src.machineid,
                date_key = src.date_key,
                sitecode = src.sitecode,
                [client type] = src.[client type],
                [enabled] = src.[enabled],
                [client version] = src.[client version],
                [real time protection enabled] = src.[real time protection enabled],
                [on access protection enabled] = src.[on access protection enabled],
                [input/output protection enabled] = src.[input/output protection enabled],
                [behavior monitor enabled] = src.[behavior monitor enabled],
                [antivirus enabled] = src.[antivirus enabled],
                [antispyware enabled] = src.[antispyware enabled],
                [nis enabled] = src.[nis enabled],
                [quick scan age (days)] = src.[quick scan age (days)],
                [full scan age (days)] = src.[full scan age (days)],
                [signature age (days)] = src.[signature age (days)],
                [engine version] = src.[engine version],
                [antivirus signature version] = src.[antivirus signature version],
                [missing important update count] = src.[missing important update count],
                [missing critical update count] = src.[missing critical update count],
                [client active status] = src.[client active status],
                [health evaluation result] = src.[health evaluation result],
                [health evaluation] = src.[health evaluation],
                [last online] = src.[last online],
                [health status message] = src.[health status message],
                [client state] = src.[client state],
                [last ddr] = src.[last ddr],
                [last hw] = src.[last hw],
                [last sw] = src.[last sw],
                [last status message] = src.[last status message],
                [last policy request] = src.[last policy request],
                [last scan time] = src.[last scan time]
    WHEN NOT matched THEN
    INSERT (machineid,
            date_key,
            sitecode,
            [client type],
            [enabled],
            [client version],
            [real time protection enabled],
            [on access protection enabled],
            [input/output protection enabled],
            [behavior monitor enabled],
            [antivirus enabled],
            [antispyware enabled],
            [nis enabled],
            [quick scan age (days)],
            [full scan age (days)],
            [signature age (days)],
            [engine version],
            [antivirus signature version],
            [missing important update count],
            [missing critical update count],
            [client active status],
            [health evaluation result],
            [health evaluation],
            [last online],
            [health status message],
            [client state],
            [last ddr],
            [last hw],
            [last sw],
            [last status message],
            [last policy request],
            [last scan time])
    VALUES (src.machineid,
            src.date_key,
            src.sitecode,
            src.[client type],
            src.[enabled],
            src.[client version],
            src.[real time protection enabled],
            src.[on access protection enabled],
            src.[input/output protection enabled],
            src.[behavior monitor enabled],
            src.[antivirus enabled],
            src.[antispyware enabled],
            src.[nis enabled],
            src.[quick scan age (days)],
            src.[full scan age (days)],
            src.[signature age (days)],
            src.[engine version],
            src.[antivirus signature version],
            src.[missing important update count],
            src.[missing critical update count],
            src.[client active status],
            src.[health evaluation result],
            src.[health evaluation],
            src.[last online],
            src.[health status message],
            src.[client state],
            src.[last ddr],
            src.[last hw],
            src.[last sw],
            src.[last status message],
            src.[last policy request],
            src.[last scan time]);

    TRUNCATE TABLE pbist_sccm.scanhistory_staging;

    -- Purge records if the data is older than x days
    DELETE FROM pbist_sccm.scanhistory WHERE date_key < @mindatekey;

    COMMIT;
END;
GO

-- =============================================
-- Description: Append the rows from  Site_staging to Site.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populatesite AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    TRUNCATE TABLE pbist_sccm.[site];
    INSERT INTO pbist_sccm.[site]
           (sitecode,
            [site name],
            [version],
            [server name],
            [availability],
            [location],
            overalllinkstatus,
            [client successfully communicating with mp],
            [client failing to communicate with mp],
            [health check count],
            [ok health check count],
            [av definition compliance count],
            [scep count],
            [av reach count])
    SELECT sitecode,
            [site name],
            [version],
            [server name],
            [availability],
            [location],
            overalllinkstatus,
            [client successfully communicating with mp],
            [client failing to communicate with mp],
            [health check count],
            [ok health check count],
            [av definition compliance count],
            [scep count],
            [av reach count]
    FROM   pbist_sccm.site_staging;

    TRUNCATE TABLE pbist_sccm.site_staging;

    COMMIT;
END;
GO

-- =============================================
-- Description: Append the rows from  Site_staging to Site.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populateupdate AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    TRUNCATE TABLE pbist_sccm.[update];
    INSERT INTO pbist_sccm.[update]
           (ci_id,
            articleid,
            bulletinid,
            title,
            severity,
            severityname,
            infoURL)
    SELECT ci_id,
            articleid,
            bulletinid,
            title,
            severity,
            [severity name],
            infourl
    FROM   pbist_sccm.update_staging;

    TRUNCATE TABLE pbist_sccm.update_staging;

    COMMIT;
END;
GO

-- =============================================
-- Description: Move the rows from  User_staging to User.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populateuser AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;
    TRUNCATE TABLE pbist_sccm.[user];

    INSERT INTO pbist_sccm.[user](username,[full name])
    SELECT username, [full name]
    FROM pbist_sccm.user_staging;

    TRUNCATE TABLE pbist_sccm.user_staging;
    
    COMMIT;
END;
GO

-- =============================================
-- Description: Move the rows from  UserComputer_staging to UserComputer.
-- =============================================
CREATE PROCEDURE pbist_sccm.sp_populateusercomputer AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION;

    TRUNCATE TABLE pbist_sccm.usercomputer;

    INSERT INTO pbist_sccm.usercomputer(machineid, username, [full name])
    SELECT machineid, username, [full name]
    FROM pbist_sccm.usercomputer_staging;

    TRUNCATE TABLE pbist_sccm.usercomputer_staging;
    COMMIT;
END;
GO

CREATE PROCEDURE [pbist_sccm].[sp_populate_SCEPDefinition] AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRANSACTION;
	TRUNCATE TABLE pbist_sccm.SCEPDefinition;
	INSERT INTO pbist_sccm.SCEPDefinition SELECT * FROM pbist_sccm.SCEPDefinition_staging;
	TRUNCATE TABLE pbist_sccm.SCEPDefinition_staging;
	COMMIT;
END;
go

