SET NOCOUNT ON;

EXEC pbist_sccm.sp_populatecomputer;
EXEC pbist_sccm.sp_populatecomputermalware;
EXEC pbist_sccm.sp_populatecomputerprogram;
EXEC pbist_sccm.sp_populatecomputerupdate;
EXEC pbist_sccm.sp_populatemalware;
EXEC pbist_sccm.sp_populateprogram;
EXEC pbist_sccm.sp_populatescanhistory;
EXEC pbist_sccm.sp_populatesite;
EXEC pbist_sccm.sp_populateupdate;
EXEC pbist_sccm.sp_populateuser;
EXEC pbist_sccm.sp_populateusercomputer;
EXEC pbist_sccm.sp_populatecollection;
EXEC pbist_sccm.sp_populatecomputercollection;

DELETE FROM pbist_sccm.[configuration] WHERE name='lastLoadTimestamp' AND configuration_group='SolutionTemplate' AND configuration_subgroup='System Center';
INSERT INTO pbist_sccm.[configuration](configuration_group, configuration_subgroup, NAME, [value], [visible]) VALUES ('SolutionTemplate', 'System Center', 'lastLoadTimestamp', GetDate(), 1);
