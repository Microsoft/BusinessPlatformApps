SET NOCOUNT ON;

WITH hwplatform AS
(
    SELECT cs.resourceid           MachineID,
           cs.manufacturer0        Manufacturer,
           cs.model0               [Model],
           cs.systemtype0          [Platform],
           cs.totalphysicalmemory0 [Physical Memory]
    FROM   dbo.v_gs_computer_system cs INNER JOIN (SELECT resourceid, MAX([TimeStamp]) AS [TimeStamp] FROM v_gs_computer_system GROUP BY resourceid) lts
	          ON cs.[TimeStamp]=lts.[TimeStamp] AND cs.ResourceID=lts.ResourceID
)
SELECT itemkey                         machineid,
       client.sitecode                 sitecode,
       c.name0                         [name],
       c.operating_system_name_and0 AS [operating system],
       c.client_type0                  [client type],
       hwp.manufacturer,
       hwp.[model],
       hwp.[platform],
       hwp.[physical memory]
FROM   vsms_r_system c
           LEFT JOIN v_clientmachines AS client ON c.itemkey = client.resourceid
           LEFT JOIN hwplatform AS hwp ON c.itemkey = hwp.machineid
WHERE
    decommissioned0=0 AND obsolete0=0 AND c.name0 NOT LIKE N'%|%';
