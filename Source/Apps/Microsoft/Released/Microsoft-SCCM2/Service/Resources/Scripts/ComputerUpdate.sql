SET NOCOUNT ON;

SELECT machineid,
       ucs.ci_id,
       laststatuschangetime,
       laststatuschecktime,
       ucs.[status]
FROM   vsms_update_compliancestatus ucs
INNER JOIN vsms_softwareupdate u ON ucs.ci_id = u.ci_id
INNER JOIN vsms_r_system c  ON ucs.machineid = c.itemkey AND u.isenabled = 1 AND u.issuperseded = 0
WHERE  c.obsolete0 = 0 AND c.decommissioned0 = 0;
