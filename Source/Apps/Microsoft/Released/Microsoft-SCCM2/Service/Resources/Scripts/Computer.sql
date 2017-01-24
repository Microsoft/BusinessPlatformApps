SET NOCOUNT ON;

WITH hwplatform AS
(
    SELECT resourceid                   MachineID,
           Max(cs.manufacturer0)        Manufacturer,
           Max(cs.model0)               [Model],
           Max(cs.systemtype0)          [Platform],
           Max(cs.totalphysicalmemory0) [Physical Memory]
    FROM   v_gs_computer_system cs
    GROUP  BY resourceid
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
